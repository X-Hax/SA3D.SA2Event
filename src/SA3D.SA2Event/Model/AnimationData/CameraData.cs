using Amicitia.IO.Binary;
using SA3D.Common;
using SA3D.Common.IO;
using SA3D.Common.Lookup;
using SA3D.Modeling.Structs;
using System.Numerics;

namespace SA3D.SA2Event.Model.AnimationData
{
	/// <summary>
	/// Camera data container.
	/// </summary>
	public class CameraData : ILabel, IBinarySerializable
	{
		/// <inheritdoc/>
		public string LabelPrefix => "Camera_";

		/// <inheritdoc/>
		public string Label { get; set; }


		/// <summary>
		/// World space position.
		/// </summary>
		public Vector3 Position { get; set; }

		/// <summary>
		/// Roll angle (radians).
		/// </summary>
		public float Roll { get; set; }

		/// <summary>
		/// Field of view angle (radians).
		/// </summary>
		public float FieldOfView { get; set; }

		/// <summary>
		/// Near clipping depth.
		/// </summary>
		public float NearClip { get; set; }

		/// <summary>
		/// Far clipping depth.
		/// </summary>
		public float FarClip { get; set; }

		/// <summary>
		/// Local X-Axis.
		/// </summary>
		public Vector3 DirX { get; set; }

		/// <summary>
		/// Local Y-Axis.
		/// </summary>
		public Vector3 DirY { get; set; }

		/// <summary>
		/// Local Z-Axis.
		/// </summary>
		public Vector3 DirZ { get; set; }


		/// <summary>
		/// Creates new camera data
		/// </summary>
		public CameraData()
		{
			DirX = Vector3.UnitX;
			DirY = Vector3.UnitY;
			DirZ = Vector3.UnitZ;
			Label = LabelPrefix.GenerateIdentifier();
		}


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader)
		{
			Position = reader.ReadVector3();
			DirZ = reader.ReadVector3();
			Roll = reader.ReadSingle(FloatIOType.BAMS32);
			FieldOfView = reader.ReadSingle(FloatIOType.BAMS32);
			NearClip = reader.ReadSingle();
			FarClip = reader.ReadSingle();
			DirX = reader.ReadVector3();
			DirY = reader.ReadVector3();
		}

		/// <inheritdoc/>
		public void Write(BinaryObjectWriter writer)
		{
			writer.WriteVector3(Position);
			writer.WriteVector3(DirZ);
			writer.WriteSingle(Roll, FloatIOType.BAMS32);
			writer.WriteSingle(FieldOfView, FloatIOType.BAMS32);
			writer.WriteSingle(NearClip);
			writer.WriteSingle(FarClip);
			writer.WriteVector3(DirX);
			writer.WriteVector3(DirY);
		}
	}
}
