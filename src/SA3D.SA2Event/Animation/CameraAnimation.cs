using Amicitia.IO.Binary;
using SA3D.Common.IO;
using SA3D.Modeling.AnimationData;

namespace SA3D.SA2Event.Model.AnimationData
{
	/// <summary>
	/// Event camera animation
	/// </summary>
	public class CameraAnimation : Animation
	{
		/// <inheritdoc/>
		public override string LabelPrefix => "CameraAnimation_";

		/// <summary>
		/// Camera data associated with this animation
		/// </summary>
		public CameraData Camera { get; set; }

		/// <summary>
		/// Reference to another camera animation. Purpose unknown, usually points to self
		/// </summary>
		public CameraAnimation OtherAnimation { get; set; }


		/// <summary>
		/// Creates a new, empty camera animation with camera data and <see langword="this"/> for <see cref="OtherAnimation"/> 
		/// </summary>
		public CameraAnimation()
		{
			Camera = new();
			OtherAnimation = this;
		}

		/// <inheritdoc/>
		protected override void Read(BinaryObjectReader reader, AnimationIOContext context)
		{
			base.Read(reader, context);

			Camera = reader.ReadObjectOffset<CameraData>(context.OffsetLUT)
				?? throw reader.ReadNullReference(nameof(CameraAnimation), nameof(Camera));

			OtherAnimation = reader.ReadObjectOffset<CameraAnimation, AnimationIOContext>(context, context.OffsetLUT)
				?? throw reader.ReadNullReference(nameof(CameraAnimation), nameof(OtherAnimation));
		}

		/// <inheritdoc/>
		protected override void Write(BinaryObjectWriter writer, AnimationIOContext context)
		{
			base.Write(writer, context);

			writer.WriteObjectOffset(Camera, context.OffsetLUT);
			writer.WriteObjectOffset(OtherAnimation, context, context.OffsetLUT);
		}
	}
}
