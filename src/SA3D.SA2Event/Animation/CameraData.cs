using SA3D.Common;
using SA3D.Common.IO;
using SA3D.Modeling.Structs;
using System.Numerics;

namespace SA3D.SA2Event.Animation
{
	/// <summary>
	/// Camera data container.
	/// </summary>
	public class CameraData
	{
		/// <summary>
		/// Size of the structure in bytes.
		/// </summary>
		public const uint StructSize = 0x40;

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
		/// Creates new camera data.
		/// </summary>
		/// <param name="position">World space position.</param>
		/// <param name="roll">Roll angle (radians).</param>
		/// <param name="fieldOfView">Field of view angle (radians).</param>
		/// <param name="nearClip">Near clipping depth.</param>
		/// <param name="farClip">Far clipping depth.</param>
		/// <param name="dirX">Local X-Axis.</param>
		/// <param name="dirY">Local Y-Axis.</param>
		/// <param name="dirZ">Local Z-Axis.</param>
		public CameraData(Vector3 position, float roll, float fieldOfView, float nearClip, float farClip, Vector3 dirX, Vector3 dirY, Vector3 dirZ)
		{
			Position = position;
			Roll = roll;
			FieldOfView = fieldOfView;
			NearClip = nearClip;
			FarClip = farClip;
			DirX = dirX;
			DirY = dirY;
			DirZ = dirZ;
		}


		/// <summary>
		/// Writes the camera data to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The pointer to the written data.</returns>
		public uint Write(EndianStackWriter writer, PointerLUT lut)
		{
			uint onWrite()
			{
				uint result = writer.PointerPosition;

				writer.WriteVector3(Position);
				writer.WriteVector3(DirZ);
				writer.WriteInt(MathHelper.RadToBAMS(Roll));
				writer.WriteInt(MathHelper.RadToBAMS(FieldOfView));
				writer.WriteFloat(NearClip);
				writer.WriteFloat(FarClip);
				writer.WriteVector3(DirX);
				writer.WriteVector3(DirY);

				return result;
			}

			return lut.GetAddAddress(this, onWrite);
		}

		/// <summary>
		/// Reads camera data off an endian stack reader.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="address"></param>
		/// <param name="lut"></param>
		/// <returns></returns>
		public static CameraData Read(EndianStackReader reader, uint address, PointerLUT lut)
		{
			CameraData onRead()
			{
				return new(
					reader.ReadVector3(address),
					MathHelper.BAMSToRad(reader.ReadInt(address + 0x18)),
					MathHelper.BAMSToRad(reader.ReadInt(address + 0x1C)),
					reader.ReadFloat(address + 0x20),
					reader.ReadFloat(address + 0x24),
					reader.ReadVector3(address + 0x28),
					reader.ReadVector3(address + 0x34),
					reader.ReadVector3(address + 0xC));
			}

			return lut.GetAddValue(address, onRead);
		}
	}
}
