using SA3D.Common.IO;
using SA3D.Modeling.Mesh.Chunk;
using SA3D.Modeling.Mesh.Chunk.PolyChunks;
using SA3D.Modeling.Mesh.Chunk.Structs;
using SA3D.Modeling.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;

namespace SA3D.SA2Event.Animation
{
	/// <summary>
	/// Surface animation structures.
	/// </summary>
	public class SurfaceAnimation
	{
		/// <summary>
		/// Initial texture ID used. Determines which texture sequence to use.
		/// </summary>
		public int TextureID { get; set; }

		/// <summary>
		/// Texture chunk that gets animated by texture sequences.
		/// </summary>
		public TextureChunk TextureChunk { get; set; }

		/// <summary>
		/// Stripchunk of which the texture coordinates were animated.
		/// </summary>
		public StripChunk? StripChunk { get; set; }

		/// <summary>
		/// Texture coordinate frames. Key is the strip index.
		/// </summary>
		public Dictionary<int, List<TexcoordFrame>> TexcoordFrames { get; }


		/// <summary>
		/// Creates a new surface animation.
		/// </summary>
		/// <param name="textureID">Initial texture ID used. Determines which texture sequence to use.</param>
		/// <param name="textureChunk">Texture chunk that gets animated by texture sequences.</param>
		/// <param name="polygonChunk">Stripchunk of which the texture coordinates were animated.</param>
		public SurfaceAnimation(int textureID, TextureChunk textureChunk, StripChunk? polygonChunk)
		{
			TextureID = textureID;
			TextureChunk = textureChunk;
			StripChunk = polygonChunk;
			TexcoordFrames = [];
		}


		/// <summary>
		/// Reads surface animations off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		/// <exception cref="InvalidDataException"></exception>
		public static SurfaceAnimation Read(EndianStackReader reader, uint address, PointerLUT lut)
		{
			int textureID = reader.ReadInt(address);
			uint targetTexIDAddr = reader.ReadPointer(address + 4);
			TextureChunk textureChunk =
				(TextureChunk)(lut.PolyChunks.GetValue(targetTexIDAddr - 2)
					?? throw new InvalidOperationException("Could not find uv animation texture chunk!"));

			StripChunk? polyChunk = null;
			uint polyChunkAddr = 0;
			int uvDataLength = reader.ReadInt(address + 8);

			if(reader.TryReadPointer(address + 0xC, out uint uvDataAddr) && uvDataLength > 0)
			{
				// find the poly chunk
				uint uvAddr = reader.ReadPointer(uvDataAddr);
				Dictionary<uint, PolyChunk> dict = lut.PolyChunks.GetDictFrom();

				polyChunkAddr = dict.Keys.Where(x => x < uvAddr).Max();
				PolyChunk chunk = dict[polyChunkAddr];

				if(polyChunkAddr + chunk.ByteSize < uvAddr)
				{
					throw new InvalidOperationException("The chunk used by the uv animation is not yet read!");
				}
				else if(chunk is not Modeling.Mesh.Chunk.PolyChunks.StripChunk)
				{
					throw new InvalidDataException("The poly chunk used by the surface animation is not a strip chunk!");
				}

				polyChunk = (StripChunk)chunk;
			}

			SurfaceAnimation result = new(textureID, textureChunk, polyChunk);

			if(polyChunk != null)
			{
				Dictionary<uint, (int strip, int corner)> indexLut = GetUVIndexLut(polyChunkAddr, reader.ImageBase, polyChunk);

				// reading the data
				float multiplier = polyChunk.HasHDTexcoords ? 1f / 1023f : 1f / 255f;
				for(int i = 0; i < uvDataLength; i++)
				{
					uint uvAddr = reader.ReadUInt(uvDataAddr);
					uvDataAddr += 4;
					Vector2 texcoords = reader.ReadVector2(ref uvDataAddr, FloatIOType.Short) * multiplier;

					(int strip, int corner) = indexLut[uvAddr];

					if(!result.TexcoordFrames.TryGetValue(strip, out List<TexcoordFrame>? uvList))
					{
						uvList = [];
						result.TexcoordFrames.Add(strip, uvList);
					}

					uvList.Add(new TexcoordFrame(corner, texcoords));
				}
			}

			return result;
		}

		/// <summary>
		/// Writes the surface animation to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>Address at which the animations were written</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public uint Write(EndianStackWriter writer, PointerLUT lut)
		{
			uint onWrite()
			{
				uint uvDataAddr = 0;
				if(StripChunk != null && TexcoordFrames.Count > 0)
				{
					if(!lut.PolyChunks.TryGetAddress(StripChunk, out uint polyChunkAddr))
					{
						throw new InvalidOperationException("Referenced UV animation data has not been written!");
					}

					uint[][] uvAddrLut = GetUVAddrLut(polyChunkAddr, StripChunk);

					uvDataAddr = writer.PointerPosition;
					float multiplier = StripChunk.HasHDTexcoords ? 1023f : 255f;

					foreach(KeyValuePair<int, List<TexcoordFrame>> uvList in TexcoordFrames)
					{
						uint[] stripAddrLut = uvAddrLut[uvList.Key];
						foreach(TexcoordFrame uv in uvList.Value)
						{
							writer.WriteUInt(stripAddrLut[uv.CornerIndex]);
							writer.WriteVector2(uv.TexCoord * multiplier, FloatIOType.Short);
						}
					}
				}

				if(!lut.PolyChunks.TryGetAddress(TextureChunk, out uint textureChunkAddr))
				{
					throw new InvalidOperationException("Referenced UV animation data has not been written!");
				}

				uint result = writer.PointerPosition;

				writer.WriteInt(TextureID);
				writer.WriteUInt(textureChunkAddr + 2);
				writer.WriteInt(TexcoordFrames.Count);
				writer.WriteUInt(uvDataAddr);

				return result;
			}

			return lut.GetAddAddress(this, onWrite);
		}


		private static Dictionary<uint, (int strip, int corner)> GetUVIndexLut(uint chunkAddress, uint imageBase, StripChunk chunk)
		{
			// address + chunk header + texcoord offset
			uint stripAddr = imageBase + chunkAddress + 6 + 2;

			uint triAttribSize = 2u * (uint)chunk.TriangleAttributeCount;

			uint structSize = (uint)(2u
				+ (chunk.TexcoordCount * 4u)
				+ (chunk.HasNormals ? 12u : 0u)
				+ (chunk.HasColors ? 4u : 0u))
				+ triAttribSize;

			Dictionary<uint, (int strip, int corner)> indexLut = [];
			for(int i = 0; i < chunk.Strips.Length; i++)
			{
				stripAddr += 2; // skip strip header
				ChunkStrip strip = chunk.Strips[i];
				int uvIndex = 0;

				for(int j = 0; j < strip.Corners.Length; j++)
				{
					indexLut.Add(stripAddr, (i, uvIndex));

					uvIndex++;
					stripAddr += structSize;

					if(j < 2)
					{
						stripAddr -= triAttribSize;
					}
				}
			}

			return indexLut;
		}

		private static uint[][] GetUVAddrLut(uint chunkAddress, StripChunk chunk)
		{
			if(chunk.TexcoordCount == 0)
			{
				throw new FormatException("Stripchunk has no texture coordinates!");
			}

			// address + chunk header + texcoord offset
			uint stripAddr = chunkAddress + 6 + 2;

			uint triAttribSize = 2u * (uint)chunk.TriangleAttributeCount;

			uint structSize = (uint)(2u
				+ (chunk.TexcoordCount * 4u)
				+ (chunk.HasNormals ? 12u : 0u)
				+ (chunk.HasColors ? 4u : 0u))
				+ triAttribSize;

			uint[][] result = new uint[chunk.Strips.Length][];
			for(int i = 0; i < chunk.Strips.Length; i++)
			{
				ChunkStrip strip = chunk.Strips[i];
				stripAddr += 2; // skip strip header

				uint[] addresses = new uint[strip.Corners.Length];

				for(int j = 0; j < strip.Corners.Length; j++)
				{
					addresses[j] = stripAddr;
					stripAddr += structSize;

					if(j < 2)
					{
						stripAddr -= triAttribSize;
					}
				}

				result[i] = addresses;
			}

			return result.ToArray();
		}
	}
}
