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
			AnimationBlocks = new();
			TextureSequences = new();
		}


		/// <summary>
		/// Writes the surface animation data to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="lut">Pointer refernces to utilize.</param>
		/// <returns>The pointer at which the data was written.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public uint Write(EndianStackWriter writer, PointerLUT lut)
		{
			uint blockAddress = SurfaceAnimationBlock.WriteBlockArray(writer, AnimationBlocks, lut);

			uint sequenceAddress = 0;
			if(TextureSequences.Count > 0)
			{
				sequenceAddress = writer.PointerPosition;
				foreach(TextureAnimSequence item in TextureSequences)
				{
					item.Write(writer);
				}
			}

			uint address = writer.PointerPosition;
			writer.WriteUInt(blockAddress);
			writer.WriteUInt(sequenceAddress);
			writer.WriteInt(TextureSequences.Count);

			return address;
		}

		/// <summary>
		/// Reads surface animation data off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The data that was read.</returns>
		public static SurfaceAnimationData Read(EndianStackReader reader, uint address, PointerLUT lut)
		{
			SurfaceAnimationData result = new();

			uint animBlockAddr = reader.ReadPointer(address);
			while(SurfaceAnimationBlock.Read(reader, animBlockAddr, lut) is SurfaceAnimationBlock block)
			{
				result.AnimationBlocks.Add(block);
				animBlockAddr += SurfaceAnimationBlock.StructSize;
			}

			uint tsAddr = reader.ReadPointer(address + 4);
			uint tsCount = reader.ReadUInt(address + 8);
			for(int i = 0; i < tsCount; i++)
			{
				result.TextureSequences.Add(TextureAnimSequence.Read(reader, ref tsAddr));
			}

			return result;
		}

	}
}
