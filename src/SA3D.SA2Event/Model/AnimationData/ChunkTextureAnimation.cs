using Amicitia.IO.Binary;
using SA3D.Common;
using SA3D.Common.IO;
using SA3D.Common.Lookup;
using SA3D.Modeling.ObjectData;
using System.Collections.Generic;

namespace SA3D.SA2Event.Model.AnimationData
{
	/// <summary>
	/// A block of surface animations targetting a specific model.
	/// </summary>
	public class ChunkTextureAnimation : ILabel, IBinarySerializable<EventModelIOContext>
	{
		/// <summary>
		/// Label prefix for <see cref="Frames"/>
		/// </summary>
		public const string FramesLabelPrefix = "Frames_";

		/// <inheritdoc/>
		public string LabelPrefix => "ChunkTextureAnimation_";

		/// <inheritdoc/>
		public string Label { get; set; }

		/// <summary>
		/// The targeted model.
		/// </summary>
		public Node? Model { get; set; }

		/// <summary>
		/// Animations contained in the block.
		/// </summary>
		public LabeledArray<ChunkTextureAnimationFrame?>? Frames { get; set; }


		/// <summary>
		/// Creates a new, empty chunk texture animation
		/// </summary>
		public ChunkTextureAnimation()
		{
			Label = LabelPrefix.GenerateIdentifier();
		}


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader, EventModelIOContext context)
		{
			Modeling.Structs.IOContext modelContext = new()
			{
				MeshFormat = Modeling.Structs.Format.Chunk,
				OffsetLUT = context.OffsetLUT
			};

			Model = reader.ReadObjectOffset<Node, Modeling.Structs.IOContext>(modelContext);

			int frameCount = reader.ReadInt32();
			Frames = reader.ReadLabeledObjectArrayOffset(
				r => r.ReadObjectOffset<ChunkTextureAnimationFrame, EventModelIOContext>(context, context.OffsetLUT),
				frameCount, FramesLabelPrefix, context.OffsetLUT
			);
		}

		internal static void ReadArray(BinaryObjectReader reader, LabeledArray<ChunkTextureAnimation> result, EventModelIOContext context)
		{
			List<ChunkTextureAnimation> animations = [];
			while(reader.ReadObjectOffset<ChunkTextureAnimation, EventModelIOContext>(context, context.OffsetLUT) is ChunkTextureAnimation animation)
			{
				animations.Add(animation);
			}

			result.Array = [.. animations];
		}

		/// <inheritdoc/>
		public void Write(BinaryObjectWriter writer, EventModelIOContext context)
		{
			Modeling.Structs.IOContext modelContext = new()
			{
				MeshFormat = Modeling.Structs.Format.Chunk,
				OffsetLUT = context.OffsetLUT
			};

			writer.WriteObjectOffset(Model, modelContext, context.OffsetLUT);
			writer.WriteInt32(Frames?.Length ?? 0);
			writer.WriteObjectArrayOffset((w, v) => w.WriteObjectOffset(v, context, context.OffsetLUT), Frames, context.OffsetLUT);
		}

		internal static void WriteArray(BinaryObjectWriter writer, IEnumerable<ChunkTextureAnimation> animations, EventModelIOContext context)
		{
			foreach(ChunkTextureAnimation animation in animations)
			{
				writer.WriteObjectOffset(animation, context, context.OffsetLUT);
			}

			writer.WriteOffsetValue(writer.OffsetHandler.NullOffset);
		}
	}
}
