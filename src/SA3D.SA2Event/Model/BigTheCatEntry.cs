using Amicitia.IO.Binary;
using SA3D.Common;
using SA3D.Common.IO;
using SA3D.Common.Lookup;
using SA3D.Modeling.AnimationData;
using SA3D.Modeling.ObjectData;
using SA3D.Modeling.Structs;
using AnimationSet = (SA3D.Modeling.AnimationData.Animation? nodeAnimation, SA3D.Modeling.AnimationData.Animation? shapeAnimation);

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Big the cat model.
	/// </summary>
	public class BigTheCatModel : ILabel, IBinarySerializable<EventModelIOContext>
	{
		/// <summary>
		/// Label prefix for <see cref="Animations"/>
		/// </summary>
		public const string AnimationsLabelPrefix = "BigTheCatAnimations_";

		/// <inheritdoc/>
		public string LabelPrefix => "BigTheCatModel_";

		/// <inheritdoc/>
		public string Label { get; set; }

		/// <summary>
		/// Model to use.
		/// </summary>
		public Node? Model { get; set; }

		/// <summary>
		/// Motion array.
		/// </summary>
		public LabeledArray<AnimationSet>? Animations { get; set; }

		/// <summary>
		/// Unknown.
		/// </summary>
		public int Unknown { get; set; }


		/// <summary>
		/// Creates a new, empty Big the Cat entry.
		/// </summary>
		public BigTheCatModel()
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

			Model = reader.ReadObjectOffset<Node, IOContext>(modelContext, context.OffsetLUT);

			long animationsOffset = reader.ReadOffsetValue();
			int animationsCount = reader.ReadInt32();

			Animations = reader.ReadLabeledObjectArrayAtOffset(
				r =>
				{
					Animation? nodeAnimation = context.ReadAnimation<Animation>(r, (uint)(Model?.GetAnimTreeNodeCount() ?? 0));
					Animation? shapeAnimation = context.ReadAnimation<Animation>(r, (uint)(Model?.GetMorphTreeNodeCount() ?? 0));
					return (nodeAnimation, shapeAnimation);
				},
				animationsOffset, animationsCount, AnimationsLabelPrefix, context.OffsetLUT
			);

			Unknown = reader.ReadInt32();
		}

		/// <inheritdoc/>
		public void Write(BinaryObjectWriter writer, EventModelIOContext context)
		{
			IOContext modelContext = new()
			{
				MeshFormat = Format.Chunk,
				OffsetLUT = context.OffsetLUT
			};

			writer.WriteObjectOffset(Model, modelContext, context.OffsetLUT);

			writer.WriteObjectArrayOffset((w, v) =>
			{
				context.WriteAnimation(w, v.nodeAnimation);
				context.WriteAnimation(w, v.shapeAnimation);
			}, Animations, context.OffsetLUT);

			writer.WriteInt32(Animations?.Length ?? 0);
			writer.WriteInt32(Unknown);
		}
	}
}
