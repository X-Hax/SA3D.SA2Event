using SA3D.Common.IO;
using SA3D.Modeling.Structs;
using System;
using System.Collections.Generic;

namespace SA3D.SA2Event.Animation
{
	/// <summary>
	/// Contains surface animation data.
	/// </summary>
	public class SurfaceAnimationData
	{
		/// <summary>
		/// Size of the structure in bytes.
		/// </summary>
		public const uint StructSize = 0xC;

		/// <summary>
		/// Animation blocks, one per model, in the data.
		/// </summary>
		public List<SurfaceAnimationBlock> AnimationBlocks { get; }

		/// <summary>
		/// Texture animation sequences.
		/// </summary>
		public List<TextureAnimSequence> TextureSequences { get; }

		/// <summary>
		/// Creates new surface animation data.
		/// </summary>
		public SurfaceAnimationData()
		{
			AnimationBlocks = [];
			TextureSequences = [];
		}


		/// <summary>
		/// Writes the surface animation data to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="textureSequenceArray">Whether to write all texture sequences as an array, otherwise only the first one is written.</param>
		/// <param name="lut">Pointer refernces to utilize.</param>
		/// <returns>The pointer at which the data was written.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public uint Write(EndianStackWriter writer, bool textureSequenceArray, PointerLUT lut)
		{
			uint blockAddress = SurfaceAnimationBlock.WriteBlockArray(writer, AnimationBlocks, lut);

			uint sequenceAddress = 0;
			if(textureSequenceArray && TextureSequences.Count > 0)
			{
				sequenceAddress = writer.PointerPosition;
				foreach(TextureAnimSequence item in TextureSequences)
				{
					item.Write(writer);
				}
			}

			uint address = writer.PointerPosition;
			writer.WriteUInt(blockAddress);

			if(textureSequenceArray)
			{
				writer.WriteUInt(sequenceAddress);
				writer.WriteInt(TextureSequences.Count);
			}
			else if(TextureSequences.Count > 0)
			{
				TextureSequences[0].Write(writer);
			}

			return address;
		}

		/// <summary>
		/// Reads surface animation data off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="textureSequenceArray">Whether the surface animations are stored in an array.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The data that was read.</returns>
		public static SurfaceAnimationData Read(EndianStackReader reader, uint address, bool textureSequenceArray, PointerLUT lut)
		{
			SurfaceAnimationData result = new();

			uint animBlockAddr = reader.ReadPointer(address);
			while(SurfaceAnimationBlock.Read(reader, animBlockAddr, lut) is SurfaceAnimationBlock block)
			{
				result.AnimationBlocks.Add(block);
				animBlockAddr += SurfaceAnimationBlock.StructSize;
			}

			if(textureSequenceArray)
			{
				uint tsAddr = reader.ReadPointer(address + 4);
				uint tsCount = reader.ReadUInt(address + 8);
				for(int i = 0; i < tsCount; i++)
				{
					result.TextureSequences.Add(TextureAnimSequence.Read(reader, ref tsAddr));
				}
			}
			else
			{
				uint tsAddr = address + 4;
				result.TextureSequences.Add(TextureAnimSequence.Read(reader, ref tsAddr));
			}

			return result;
		}

	}
}
