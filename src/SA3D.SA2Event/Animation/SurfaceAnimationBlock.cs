using SA3D.Common.IO;
using SA3D.Modeling.ObjectData;
using SA3D.Modeling.Structs;
using System;
using System.Collections.Generic;

namespace SA3D.SA2Event.Animation
{
	/// <summary>
	/// A block of surface animations targetting a specific model.
	/// </summary>
	public class SurfaceAnimationBlock
	{
		/// <summary>
		/// Size of the structure in bytes.
		/// </summary>
		public const uint StructSize = 0xC;

		/// <summary>
		/// The targeted model.
		/// </summary>
		public Node Model { get; set; }

		/// <summary>
		/// Animations contained in the block.
		/// </summary>
		public List<SurfaceAnimation?> Animations { get; }


		/// <summary>
		/// Creates new surface animations.
		/// </summary>
		/// <param name="model">The targeted model.</param>
		public SurfaceAnimationBlock(Node model)
		{
			Model = model;
			Animations = new();
		}


		/// <summary>
		/// Writes the animations to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		public void WriteAnimations(EndianStackWriter writer, PointerLUT lut)
		{
			foreach(SurfaceAnimation? anim in Animations)
			{
				anim?.Write(writer, lut);
			}
		}

		/// <summary>
		/// Write the animation list to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The address at which the animations were written.</returns>
		/// <exception cref="InvalidOperationException"></exception>
		public uint WriteAnimationList(EndianStackWriter writer, PointerLUT lut)
		{
			uint onWrite()
			{
				uint result = writer.PointerPosition;

				foreach(SurfaceAnimation? anim in Animations)
				{
					if(anim == null)
					{
						writer.WriteEmpty(4);
					}
					else if(lut.All.TryGetAddress(anim, out uint animAddr))
					{
						writer.WriteUInt(animAddr);
					}
					else
					{
						throw new InvalidOperationException("Surface animation has not been written(?)");
					}
				}

				return result;
			}

			return lut.GetAddAddress(Animations, onWrite);
		}

		/// <summary>
		/// Writes the block header to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <exception cref="InvalidOperationException"></exception>
		public void WriteBlock(EndianStackWriter writer, PointerLUT lut)
		{
			if(!lut.Nodes.TryGetAddress(Model, out uint modelAddr))
			{
				throw new InvalidOperationException("Animation block model has not been written!");
			}

			if(!lut.All.TryGetAddress(Animations, out uint animAddr))
			{
				throw new InvalidOperationException("UV animation list has not been written!");
			}

			writer.WriteUInt(modelAddr);
			writer.WriteInt(Animations.Count);
			writer.WriteUInt(animAddr);
		}

		/// <summary>
		/// Writes an array of surface animation blocks to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="blocks">The blocks to write.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns></returns>
		public static uint WriteBlockArray(EndianStackWriter writer, IEnumerable<SurfaceAnimationBlock> blocks, PointerLUT lut)
		{
			foreach(SurfaceAnimationBlock? block in blocks)
			{
				block?.WriteAnimations(writer, lut);
			}

			foreach(SurfaceAnimationBlock? block in blocks)
			{
				block?.WriteAnimationList(writer, lut);
			}

			uint result = writer.PointerPosition;
			foreach(SurfaceAnimationBlock block in blocks)
			{
				block.WriteBlock(writer, lut);
			}

			writer.WriteEmpty(StructSize);

			return result;
		}


		/// <summary>
		/// Reads a surface animation block off an endian stack reader. Returns null if empty/marks end.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The surface animation block that was read.</returns>
		public static SurfaceAnimationBlock? Read(EndianStackReader reader, uint address, PointerLUT lut)
		{
			if(!reader.TryReadPointer(address, out uint modelAddr))
			{
				return null;
			}

			if(!lut.Nodes.TryGetValue(modelAddr, out Node? animModel))
			{
				throw new InvalidOperationException("Animation block model has not been read!");
			}

			SurfaceAnimationBlock result = new(animModel);

			if(reader.TryReadPointer(address + 8, out uint animationAddr))
			{
				int animCount = reader.ReadInt(address + 4);
				for(int i = 0; i < animCount; i++)
				{
					SurfaceAnimation? anim = null;
					if(reader.TryReadPointer(animationAddr, out uint animEntryAddr))
					{
						anim = SurfaceAnimation.Read(reader, animEntryAddr, lut);
					}

					result.Animations.Add(anim);
					animationAddr += 4;
				}
			}

			return result;
		}

		/// <summary>
		/// Reads an array of surface animation blocks off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The surface animation blocks that were read.</returns>
		public static List<SurfaceAnimationBlock> ReadArray(EndianStackReader reader, uint address, PointerLUT lut)
		{
			List<SurfaceAnimationBlock> result = new();
			while(Read(reader, address, lut) is SurfaceAnimationBlock block)
			{
				result.Add(block);
				address += StructSize;
			}

			return result;
		}

	}
}
