using SA3D.Common.IO;
using SA3D.Modeling.Animation;
using SA3D.Modeling.ObjectData;
using SA3D.Modeling.ObjectData.Enums;
using SA3D.Modeling.Structs;
using SA3D.SA2Event.Animation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Event model entry.
	/// </summary>
	public class EventEntry
	{
		/// <summary>
		/// Size of the dreamcast structure in bytes.
		/// </summary>
		public const uint StructSizeDC = 32;

		/// <summary>
		/// Size of the gamecube+ports structure in bytes.
		/// </summary>
		public const uint StructSizeGC = 44;


		/// <summary>
		/// Chunk model of the entry.
		/// </summary>
		public Node? Model { get; set; }

		/// <summary>
		/// Node animation to play on the model.
		/// </summary>
		public Motion? Animation { get; set; }

		/// <summary>
		/// Shape animation to play on the model (chunk only).
		/// </summary>
		public Motion? ShapeAnimation { get; set; }

		/// <summary>
		/// Gamecube model.
		/// </summary>
		public Node? GCModel { get; set; }

		/// <summary>
		/// Shadow caster model.
		/// </summary>
		public Node? ShadowModel { get; set; }

		/// <summary>
		/// Unknown
		/// </summary>
		public uint Unknown { get; set; }

		/// <summary>
		/// Initial world space position of the model.
		/// </summary>
		public Vector3 Position { get; set; }

		/// <summary>
		/// Attributes storing various rendering and behavior properties.
		/// </summary>
		public EventEntryAttribute Attributes { get; set; }

		/// <summary>
		/// Rendering layer. Used for advanced transparency sorting.
		/// </summary>
		public uint Layer { get; set; }

		/// <summary>
		/// Returns either <see cref="Model"/> or <see cref="GCModel"/>, depending on which is not null. 
		/// <br/> If both are available, <see cref="Model"/> is returned, although this should never be the case.
		/// </summary>
		public Node DisplayModel => (Model ?? GCModel) ?? throw new InvalidOperationException("No display model!");


		/// <summary>
		/// Automatically assigns animation attributes for animated scene entries.
		/// </summary>
		public void AutoAnimationAttributes()
		{
			if(Animation == null)
			{
				Attributes |= EventEntryAttribute.Scene_NoNodeAnimation;
			}
			else
			{
				Attributes &= ~EventEntryAttribute.Scene_NoNodeAnimation;
			}

			if(ShapeAnimation == null)
			{
				Attributes |= EventEntryAttribute.Scene_NoShapeAnimation;
			}
			else
			{
				Attributes &= ~EventEntryAttribute.Scene_NoShapeAnimation;
			}
		}


		/// <summary>
		/// Reads a dreamcast formatted event entry off an endian stack reader. Advances the address by the number of bytes read.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The event entry that was read.</returns>
		/// <exception cref="InvalidDataException"></exception>
		public static EventEntry ReadDC(EndianStackReader reader, ref uint address, PointerLUT lut)
		{
			EventEntry result = new();

			if(!reader.TryReadPointer(address, out uint modelAddr))
			{
				throw new InvalidDataException("Entity model is null!");
			}

			result.Model = Node.Read(reader, modelAddr, ModelFormat.SA2, lut);

			if(reader.TryReadPointer(address + 4, out uint motionAddr))
			{
				result.Animation = Motion.Read(reader, motionAddr, (uint)result.Model.GetAnimTreeNodeCount(), lut);
			}

			if(reader.TryReadPointer(address + 8, out uint morphMotionAddr))
			{
				result.ShapeAnimation = Motion.Read(reader, morphMotionAddr, (uint)result.Model.GetMorphTreeNodeCount(), lut);
			}

			result.Unknown = reader.ReadUInt(address + 0x0C);
			result.Position = reader.ReadVector3(address + 0x10);
			result.Attributes = (EventEntryAttribute)reader.ReadUInt(address + 0x1C);

			address += StructSizeDC;
			return result;
		}

		/// <summary>
		/// Reads a gamecube formatted event entry off an endian stack reader. Advances the address by the number of bytes read.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="motions">Event motion array to utilize. Needed for fetching motion indices.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The event entry that was read.</returns>
		public static EventEntry ReadGC(EndianStackReader reader, ref uint address, EventMotion[] motions, PointerLUT lut)
		{
			EventEntry result = new();

			uint addr = address;
			Node? ReadNode(uint offset, ModelFormat format)
			{
				Node? node = null;
				if(reader.TryReadPointer(addr + offset, out uint modelAddr))
				{
					node = Node.Read(reader, modelAddr, format, lut);
				}

				return node;
			}

			result.Model = ReadNode(0, ModelFormat.SA2);
			result.Animation = motions[reader.ReadUInt(address + 4)].Animation;
			result.ShapeAnimation = motions[reader.ReadUInt(address + 8)].Animation;
			result.GCModel = ReadNode(0xC, ModelFormat.SA2B);
			result.ShadowModel = ReadNode(0x10, ModelFormat.SA2);
			result.Unknown = reader.ReadUInt(address + 0x14);
			result.Position = reader.ReadVector3(address + 0x18);
			result.Attributes = (EventEntryAttribute)reader.ReadUInt(address + 0x24);
			result.Layer = reader.ReadUInt(address + 0x28);

			address += StructSizeGC;

			return result;
		}


		/// <summary>
		/// Writes the dreamcast formatted event entry to an endain stack writer.
		/// </summary>
		/// <remarks>
		/// Model and animations need to be written manually beforehand!
		/// </remarks>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <exception cref="InvalidOperationException"></exception>
		public void WriteDC(EndianStackWriter writer, PointerLUT lut)
		{
			uint animAddr = 0;
			uint shapeAnimAddr = 0;

			if(Model == null
				|| !lut.Nodes.TryGetAddress(Model, out uint modelAddr)
				|| (Animation != null && !lut.Motions.TryGetAddress(Animation, out animAddr))
				|| (ShapeAnimation != null && !lut.Motions.TryGetAddress(ShapeAnimation, out shapeAnimAddr)))
			{
				throw new InvalidOperationException("Entity Content has not yet been written!");
			}

			writer.WriteUInt(modelAddr);
			writer.WriteUInt(animAddr);
			writer.WriteUInt(shapeAnimAddr);
			writer.WriteUInt(Unknown);
			writer.WriteVector3(Position);
			writer.WriteUInt((uint)Attributes);
		}

		/// <summary>
		/// Writes the gamecube formatted event entry to an endain stack writer.
		/// </summary>
		/// <remarks>
		/// Models need to be written manually beforehand!
		/// </remarks>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="motionLUT">LUT with reference keys for all event motion pairs.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <exception cref="InvalidOperationException"></exception>
		public void WriteGC(EndianStackWriter writer, Dictionary<EventMotion, uint> motionLUT, PointerLUT lut)
		{
			uint modelAddr = 0;
			uint gcModelAddr = 0;
			uint shadowModelAddr = 0;

			if((Model != null && !lut.Nodes.TryGetAddress(Model, out modelAddr))
				|| (GCModel != null && !lut.Nodes.TryGetAddress(GCModel, out gcModelAddr))
				|| (ShadowModel != null && !lut.Nodes.TryGetAddress(ShadowModel, out shadowModelAddr)))
			{
				throw new InvalidOperationException("Entity Content has not yet been written!");
			}

			writer.WriteUInt(modelAddr);
			writer.WriteUInt(motionLUT.GetMotionKey(Animation));
			writer.WriteUInt(motionLUT.GetMotionKey(ShapeAnimation));
			writer.WriteUInt(gcModelAddr);
			writer.WriteUInt(shadowModelAddr);
			writer.WriteUInt(Unknown);
			writer.WriteVector3(Position);
			writer.WriteUInt((uint)Attributes);
			writer.WriteUInt(Layer);
		}


		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{DisplayModel?.Label ?? null}";
		}
	}
}
