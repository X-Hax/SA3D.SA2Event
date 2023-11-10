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
	public struct ObjectLighting : IFrame, IEquatable<ObjectLighting>
	{
		/// <summary>
		/// Struct size in bytes.
		/// </summary>
		public const uint StructSize = 68;

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


		/// <summary>
		/// 
		/// </summary>
		/// <param name="frame">Frame at which the effect starts playing.</param>
		/// <param name="fade">The way in which the lighting should fade in.</param>
		/// <param name="direction">Direction of the light.</param>
		/// <param name="diffuse">Diffuse color of the light</param>
		/// <param name="intensity">Ambient color intensity</param>
		/// <param name="ambient">Ambient color.</param>
		public ObjectLighting(uint frame, LightFadeMode fade, Vector3 direction, Color diffuse, float intensity, Color ambient)
		{
			Frame = frame;
			Fade = fade;
			Direction = direction;
			Diffuse = diffuse;
			Intensity = intensity;
			Ambient = ambient;
		}


		/// <summary>
		/// Writes the object lighting to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		public readonly void Write(EndianStackWriter writer)
		{
			writer.WriteUInt(Frame);
			writer.WriteUInt((uint)Fade);
			writer.WriteVector3(Direction);

			writer.WriteFloat(Diffuse.RedF);
			writer.WriteFloat(Diffuse.GreenF);
			writer.WriteFloat(Diffuse.BlueF);

			writer.WriteFloat(Intensity);

			writer.WriteFloat(Ambient.RedF);
			writer.WriteFloat(Ambient.GreenF);
			writer.WriteFloat(Ambient.BlueF);

			writer.WriteEmpty(20);
		}

		/// <summary>
		/// Reads object lighting off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <returns>The object lighting that was read.</returns>
		public static ObjectLighting Read(EndianStackReader reader, uint address)
		{
			Color ReadColor(uint addr)
			{
				Vector3 values = reader.ReadVector3(addr);
				return new()
				{
					RedF = values.X,
					GreenF = values.Y,
					BlueF = values.Z
				};
			}

			return new(
				reader.ReadUInt(address),
				(LightFadeMode)reader.ReadUInt(address + 4),
				reader.ReadVector3(address + 8),
				ReadColor(address + 0x14),
				reader.ReadFloat(address + 0x20),
				ReadColor(address + 0x24));
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
