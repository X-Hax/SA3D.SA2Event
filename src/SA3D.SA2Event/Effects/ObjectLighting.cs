using Amicitia.IO.Binary;
using SA3D.Common.IO;
using SA3D.Modeling.Structs;
using SA3D.SA2Event.Effects.Enums;
using System;
using System.Numerics;

namespace SA3D.SA2Event.Effects
{
	/// <summary>
	/// Scene wide lighting affecting 3D models.
	/// </summary>
	public struct ObjectLighting : IFrame, IEquatable<ObjectLighting>, IBinarySerializable
	{
		/// <inheritdoc/>
		public uint Frame { get; set; }

		/// <summary>
		/// The way in which the lighting should fade in.
		/// </summary>
		public LightFadeMode Fade { get; set; }

		/// <summary>
		/// Direction of the light.
		/// </summary>
		public Vector3 Direction { get; set; }

		/// <summary>
		/// Diffuse color of the light
		/// </summary>
		public Color Diffuse { get; set; }

		/// <summary>
		/// Ambient color intensity
		/// </summary>
		public float Intensity { get; set; }

		/// <summary>
		/// Ambient color.
		/// </summary>
		public Color Ambient { get; set; }


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader)
		{
			Color ReadColor()
			{
				Vector3 values = reader.ReadVector3();
				return new()
				{
					RedF = values.X,
					GreenF = values.Y,
					BlueF = values.Z
				};
			}

			Frame = reader.ReadUInt32();
			Fade = (LightFadeMode)reader.ReadUInt32();
			Direction = reader.ReadVector3();
			Diffuse = ReadColor();
			Intensity = reader.ReadSingle();
			Ambient = ReadColor();
			reader.Skip(20);
		}

		/// <inheritdoc/>
		public readonly void Write(BinaryObjectWriter writer)
		{
			writer.WriteUInt32(Frame);
			writer.WriteUInt32((uint)Fade);
			writer.WriteVector3(Direction);

			writer.WriteSingle(Diffuse.RedF);
			writer.WriteSingle(Diffuse.GreenF);
			writer.WriteSingle(Diffuse.BlueF);

			writer.WriteSingle(Intensity);

			writer.WriteSingle(Ambient.RedF);
			writer.WriteSingle(Ambient.GreenF);
			writer.WriteSingle(Ambient.BlueF);

			writer.Skip(20);
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is ObjectLighting lighting &&
				   Frame == lighting.Frame &&
				   Fade == lighting.Fade &&
				   Direction.Equals(lighting.Direction) &&
				   Diffuse.Equals(lighting.Diffuse) &&
				   Intensity == lighting.Intensity &&
				   Ambient.Equals(lighting.Ambient);
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(Frame, Fade, Direction, Diffuse, Intensity, Ambient);
		}

		readonly bool IEquatable<ObjectLighting>.Equals(ObjectLighting other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two object lighting effects for equality.
		/// </summary>
		/// <param name="left">Lefthand object lighting.</param>
		/// <param name="right">Righthand object lighting.</param>
		/// <returns>Whether the two object lighting effects are equal</returns>
		public static bool operator ==(ObjectLighting left, ObjectLighting right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two object lighting effects for inequality.
		/// </summary>
		/// <param name="left">Lefthand object lighting.</param>
		/// <param name="right">Righthand object lighting.</param>
		/// <returns>Whether the two object lighting effects are inequal</returns>
		public static bool operator !=(ObjectLighting left, ObjectLighting right)
		{
			return !(left == right);
		}


		/// <inheritdoc/>
		public override readonly string ToString()
		{
			return Frame == 0 ? "-" : $"[{Frame}] {Fade} - {Diffuse} / {Intensity} / {Ambient}";
		}
	}
}
