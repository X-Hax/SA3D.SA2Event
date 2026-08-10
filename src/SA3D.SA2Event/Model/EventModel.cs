using Amicitia.IO.Binary;
using SA3D.Common;
using SA3D.Common.IO;
using SA3D.Common.Lookup;
using SA3D.Modeling.AnimationData;
using SA3D.Modeling.ObjectData;
using SA3D.Modeling.Structs;
using System;
using System.IO;
using System.Numerics;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Event model.
	/// </summary>
	public class EventModel : ILabel, IBinarySerializable<EventModelIOContext>
	{
		/// <inheritdoc/>
		public string LabelPrefix => "EventModel_";

		/// <inheritdoc/>
		public string Label { get; set; }

		/// <summary>
		/// Chunk model of the entry.
		/// </summary>
		public Node? Model { get; set; }

		/// <summary>
		/// Node animation to play on the model.
		/// </summary>
		public Animation? Animation { get; set; }

		/// <summary>
		/// Shape animation to play on the model (chunk only).
		/// </summary>
		public Animation? ShapeAnimation { get; set; }

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
		/// Creates a new, blank event model
		/// </summary>
		public EventModel()
		{
			Label = LabelPrefix.GenerateIdentifier();
		}


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader, EventModelIOContext context)
		{
			IOContext modelContext = new()
			{
				MeshFormat = Format.Chunk,
				OffsetLUT = context.OffsetLUT
			};

			Model = reader.ReadObjectOffset<Node, IOContext>(modelContext, modelContext.OffsetLUT);

			if(context.EventType != EventType.gc && Model == null)
			{
				throw new InvalidDataException("Event model has no model reference!");
			}

			Animation = context.ReadAnimation<Animation>(reader, (uint)(Model?.GetAnimTreeNodeCount() ?? 0));
			ShapeAnimation = context.ReadAnimation<Animation>(reader, (uint)(Model?.GetMorphTreeNodeCount() ?? 0));

			if(context.EventType == EventType.gc)
			{
				modelContext.MeshFormat = Format.Ginja;
				GCModel = reader.ReadObjectOffset<Node, IOContext>(modelContext, modelContext.OffsetLUT);

				modelContext.MeshFormat = Format.Chunk;
				ShadowModel = reader.ReadObjectOffset<Node, IOContext>(modelContext, modelContext.OffsetLUT);
			}

			Unknown = reader.ReadUInt32();
			Position = reader.ReadVector3();
			Attributes = (EventEntryAttribute)reader.ReadUInt32();

			if(context.EventType == EventType.gc)
			{
				Layer = reader.ReadUInt32();
			}
		}

		/// <inheritdoc/>
		public void Write(BinaryObjectWriter writer, EventModelIOContext context)
		{
			if(context.EventType != EventType.gc && Model == null)
			{
				throw new InvalidDataException("Event model has no model reference!");
			}

			IOContext modelContext = new()
			{
				MeshFormat = Format.Chunk,
				OffsetLUT = context.OffsetLUT
			};

			writer.WriteObjectOffset(Model, modelContext, context.OffsetLUT);
			context.WriteAnimation(writer, Animation);
			context.WriteAnimation(writer, ShapeAnimation);

			if(context.EventType == EventType.gc)
			{
				modelContext.MeshFormat = Format.Ginja;
				writer.WriteObjectOffset(GCModel, modelContext, context.OffsetLUT);

				modelContext.MeshFormat = Format.Chunk;
				writer.WriteObjectOffset(ShadowModel, modelContext, context.OffsetLUT);
			}

			writer.WriteUInt32(Unknown);
			writer.WriteVector3(Position);
			writer.WriteUInt32((uint)Attributes);

			if(context.EventType == EventType.gc)
			{
				writer.WriteUInt32(Layer);
			}
		}

		/// <inheritdoc/>
		public override string ToString()
		{
			return $"{DisplayModel?.Label ?? null}";
		}

	}
}
