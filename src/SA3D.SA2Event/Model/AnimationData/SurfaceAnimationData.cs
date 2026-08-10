using Amicitia.IO.Binary;
using SA3D.Common;
using SA3D.Common.IO;
using SA3D.Common.Lookup;

namespace SA3D.SA2Event.Model.AnimationData
{
	/// <summary>
	/// Contains surface animation data.
	/// </summary>
	public class SurfaceAnimationData : ILabel, IBinarySerializable<EventModelIOContext>
	{
		/// <summary>
		/// Label prefix for <see cref="ChunkTextureAnimations"/>
		/// </summary>
		public const string ChunkTextureAnimationsLabelPrefix = "ChunkTextureAnimations_";

		/// <summary>
		/// Label prefix for <see cref="TextureAnimationSequences"/>
		/// </summary>
		public const string TextureAnimationSequencesLabelPrefix = "TextureAnimationSequences_";


		/// <inheritdoc/>
		public string LabelPrefix => "SurfaceAnimationData_";

		/// <inheritdoc/>
		public string Label { get; set; }

		/// <summary>
		/// Animation blocks, one per model, in the data.
		/// </summary>
		public LabeledArray<ChunkTextureAnimation>? ChunkTextureAnimations { get; set; }

		/// <summary>
		/// Texture animation sequences.
		/// </summary>
		public LabeledArray<TextureAnimationSequence>? TextureAnimationSequences { get; set; }


		/// <summary>
		/// Creates a new, empty set of surface animation data
		/// </summary>
		public SurfaceAnimationData()
		{
			Label = LabelPrefix.GenerateIdentifier();
		}


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader, EventModelIOContext context)
		{
			ChunkTextureAnimations = reader.ReadLUTItemAtOffset<LabeledArray<ChunkTextureAnimation>>(reader.ReadOffsetValue(), context.OffsetLUT, ChunkTextureAnimationsLabelPrefix, (r, dst) => ChunkTextureAnimation.ReadArray(r, dst, context));

			if(context.EventType > EventType.dc)
			{
				int textureAnimationSequences = reader.ReadInt32();
				TextureAnimationSequences = reader.ReadLabeledObjectArrayOffset<TextureAnimationSequence>(textureAnimationSequences, TextureAnimationSequencesLabelPrefix, context.OffsetLUT);
			}
			else
			{
				TextureAnimationSequences = new([reader.ReadObject<TextureAnimationSequence>()]);
			}
		}

		/// <inheritdoc/>
		public void Write(BinaryObjectWriter writer, EventModelIOContext context)
		{
			writer.WriteObjectOffset(ChunkTextureAnimations.EmptyNull(), (w, v) => ChunkTextureAnimation.WriteArray(w, v, context), context.OffsetLUT);

			if(context.EventType > EventType.dc)
			{
				writer.WriteInt32(TextureAnimationSequences?.Length ?? 0);
				writer.WriteObjectArrayOffset(TextureAnimationSequences, context.OffsetLUT);
			}
			else
			{
				TextureAnimationSequence sequence = TextureAnimationSequences?.Length > 0 ? TextureAnimationSequences[0] : default;
				writer.WriteObject(sequence);
			}
		}
	}
}
