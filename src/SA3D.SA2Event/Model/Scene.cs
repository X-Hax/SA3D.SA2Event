using Amicitia.IO.Binary;
using SA3D.Common;
using SA3D.Common.IO;
using SA3D.Common.Lookup;
using SA3D.Modeling.AnimationData;
using SA3D.SA2Event.Model.AnimationData;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Continuous scene of an event. The "cut" in a cutscene.
	/// </summary>
	public class Scene : IBinarySerializable<EventModelIOContext>
	{
		/// <summary>
		/// Label prefix for <see cref="Models"/>
		/// </summary>
		public const string ModelsLabelPrefix = "SceneModels_";

		/// <summary>
		/// Label prefix for <see cref="CameraAnimations"/>
		/// </summary>
		public const string CameraAnimationsLabelPrefix = "SceneCameraAnimations_";

		/// <summary>
		/// Label prefix for <see cref="ParticleAnimations"/>
		/// </summary>
		public const string ParticleAnimationsLabelPrefix = "SceneParticleAnimations_";

		/// <summary>
		/// Event entries rendered in the scene specifically.
		/// </summary>
		public LabeledArray<EventModel> Models { get; set; }

		/// <summary>
		/// Camera animations to be played.
		/// </summary>
		public LabeledArray<CameraAnimation>? CameraAnimations { get; set; }

		/// <summary>
		/// Motions of particles in the scene. Motion index corresponds to particle index in effects file.
		/// </summary>
		public LabeledArray<Animation?>? ParticleAnimations { get; set; }

		/// <summary>
		/// Big the cat entry.
		/// </summary>
		public BigTheCatModel? BigTheCat { get; set; }

		/// <summary>
		/// Number of frames (at 30 fps) that the scene takes to play.
		/// </summary>
		public int FrameCount { get; set; }


		/// <summary>
		/// Creates a new scene.
		/// </summary>
		public Scene()
		{
			Models = new(ModelsLabelPrefix.GenerateIdentifier());
			CameraAnimations = new(CameraAnimationsLabelPrefix.GenerateIdentifier());
			ParticleAnimations = new(ParticleAnimationsLabelPrefix.GenerateIdentifier());
		}


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader, EventModelIOContext context)
		{
			long eventModelOffset = reader.ReadOffsetValue();
			int eventModelCount = reader.ReadInt32();

			Models = reader.ReadLabeledObjectArrayAtOffset<EventModel, EventModelIOContext>(
				eventModelOffset,
				eventModelCount,
				ModelsLabelPrefix,
				context,
				context.OffsetLUT)
				?? throw reader.ReadNullReference(nameof(Scene), nameof(Models), eventModelOffset);


			long cameraAnimationsOffset = reader.ReadOffsetValue();
			int cameraAnimationsCount = reader.ReadInt32();

			CameraAnimations = reader.ReadLabeledObjectArrayAtOffset(
				r => context.ReadAnimation<CameraAnimation>(r, 1)
					?? throw r.ReadNullReference(nameof(Scene), "CameraAnimations[]"),
				cameraAnimationsOffset,
				cameraAnimationsCount,
				CameraAnimationsLabelPrefix,
				context.OffsetLUT);


			long particleAnimationsOffset = reader.ReadOffsetValue();
			int particleAnimationsCount = reader.ReadInt32();

			ParticleAnimations = reader.ReadLabeledObjectArrayAtOffset(
				r => context.ReadAnimation<Animation>(r, 1),
				particleAnimationsOffset,
				particleAnimationsCount,
				ParticleAnimationsLabelPrefix,
				context.OffsetLUT);

			BigTheCat = reader.ReadObjectOffset<BigTheCatModel, EventModelIOContext>(context, context.OffsetLUT);
			FrameCount = reader.ReadInt32();
		}

		/// <inheritdoc/>
		public void Write(BinaryObjectWriter writer, EventModelIOContext context)
		{
			writer.WriteObjectArrayOffset(Models, context, context.OffsetLUT);
			writer.WriteInt32(Models.Length);

			writer.WriteObjectArrayOffset(
				context.WriteAnimation,
				CameraAnimations,
				context.OffsetLUT
			);
			writer.WriteInt32(CameraAnimations?.Length ?? 0);

			writer.WriteObjectArrayOffset(
				context.WriteAnimation,
				ParticleAnimations,
				context.OffsetLUT
			);
			writer.WriteInt32(ParticleAnimations?.Length ?? 0);

			writer.WriteObjectOffset(BigTheCat, context, context.OffsetLUT);
			writer.WriteInt32(FrameCount);
		}
	}
}
