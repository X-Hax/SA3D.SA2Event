using SA3D.Common.IO;
using SA3D.Modeling.Animation;
using SA3D.Modeling.ObjectData;
using SA3D.Modeling.ObjectData.Enums;
using SA3D.Modeling.Structs;
using SA3D.SA2Event.Animation;
using System;
using System.Collections.Generic;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Big the cat model entry.
	/// </summary>
	public class BigTheCatEntry
	{
		/// <summary>
		/// Model to use.
		/// </summary>
		public Node? Model { get; set; }

		/// <summary>
		/// Motion array.
		/// </summary>
		public List<(Motion? nodeAnimation, Motion? shapeAnimation)> Motions { get; }

		/// <summary>
		/// Unknown.
		/// </summary>
		public int Unknown { get; set; }

		/// <summary>
		/// Creates a new Big the Cat entry.
		/// </summary>
		/// <param name="model">Model </param>
		/// <param name="unknown"></param>
		public BigTheCatEntry(Node? model, int unknown)
		{
			Model = model;
			Motions = new();
			Unknown = unknown;
		}

		/// <summary>
		/// Writes the dreamcast formatted motion array to an endian stack writer. Returns 0 if motions are empty.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="motionLUT">LUT with reference keys for all event motion pairs.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>Pointer to the written array.</returns>
		public uint WriteMotionArray(EndianStackWriter writer, Dictionary<EventMotion, uint> motionLUT, PointerLUT lut)
		{
			if(Motions.Count == 0)
			{
				return 0;
			}

			uint onWrite()
			{
				uint result = writer.PointerPosition;

				foreach((Motion? nodeAnim, Motion? shapeAnim) in Motions)
				{
					writer.WriteUInt(motionLUT.GetMotionKey(nodeAnim));
					writer.WriteUInt(motionLUT.GetMotionKey(shapeAnim));
				}

				return result;
			}

			return lut.GetAddAddress(Motions, onWrite);
		}

		/// <summary>
		/// Writes the big the cat entry to an endian stack writer.
		/// </summary>
		/// <remarks>
		/// Model and motion lists need to manually be written beforehand. 
		/// <br/> Write the motion array with <see cref="WriteMotionArray"/>.
		/// </remarks>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <exception cref="InvalidOperationException"></exception>
		public void Write(EndianStackWriter writer, PointerLUT lut)
		{
			uint modelAddr = 0;
			uint motionAddr = 0;
			if((Model != null && !lut.Nodes.TryGetAddress(Model, out modelAddr))
				|| (Motions.Count > 0 && !lut.All.TryGetAddress(Motions, out motionAddr)))
			{
				throw new InvalidOperationException("Big the Cat content has not been written yet");
			}

			writer.WriteUInt(modelAddr);
			writer.WriteUInt(motionAddr);
			writer.WriteInt(Motions.Count);
			writer.WriteInt(Unknown);
		}


		/// <summary>
		/// Reads a dreamcast formatted big the cat entry off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The big the cat entry that was read.</returns>
		public static BigTheCatEntry ReadDC(EndianStackReader reader, uint address, PointerLUT lut)
		{
			Node? model = null;
			if(reader.TryReadPointer(address, out uint modelAddr))
			{
				model = Node.Read(reader, modelAddr, ModelFormat.SA2, lut);
			}

			int unknown = reader.ReadInt(address + 0xC);
			BigTheCatEntry result = new(model, unknown);

			if(model != null)
			{
				uint animatedCount = (uint)model.GetAnimTreeNodeCount();
				if(reader.TryReadPointer(address + 4, out uint motionAddr))
				{
					int count = reader.ReadInt(address + 8);
					for(int i = 0; i < count; i++)
					{
						Motion? nodeAnim = null;
						Motion? shapeAnim = null;

						if(reader.TryReadPointer(motionAddr, out uint firstAddr))
						{
							nodeAnim = Motion.Read(reader, firstAddr, animatedCount, lut);
						}

						if(reader.TryReadPointer(motionAddr + 4, out uint secondAddr))
						{
							shapeAnim = Motion.Read(reader, secondAddr, animatedCount, lut);
						}

						result.Motions.Add((nodeAnim, shapeAnim));
						motionAddr += 8;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Reads a dreamcast formatted big the cat entry off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="motions">Event motion array to utilize. Needed for fetching motion indices.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The big the cat entry that was read.</returns>
		public static BigTheCatEntry ReadGC(EndianStackReader reader, uint address, EventMotion[] motions, PointerLUT lut)
		{
			Node? node = null;
			if(reader.TryReadPointer(address, out uint modelAddr))
			{
				node = Node.Read(reader, modelAddr, ModelFormat.SA2, lut);
			}

			int unknown = reader.ReadInt(address + 0xC);

			BigTheCatEntry result = new(node, unknown);

			if(reader.TryReadPointer(address + 4, out uint motionAddr))
			{
				int count = reader.ReadInt(address + 8);
				for(int i = 0; i < count; i++)
				{
					Motion? nodeAnim = motions[reader.ReadInt(motionAddr)].Animation;
					Motion? shapeAnim = motions[reader.ReadInt(motionAddr + 4)].Animation;

					result.Motions.Add((nodeAnim, shapeAnim));
					motionAddr += 8;
				}
			}

			return result;
		}

	}
}
