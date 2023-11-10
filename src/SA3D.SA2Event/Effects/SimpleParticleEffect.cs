using SA3D.Common.IO;
using SA3D.SA2Event.Effects.Enums;
using System;

namespace SA3D.SA2Event.Effects
{
	/// <summary>
	/// Simple particle effect structure.
	/// </summary>
	public struct SimpleParticleEffect : IFrame, IEquatable<SimpleParticleEffect>
	{
		/// <summary>
		/// Size of the structure in bytes.
		/// </summary>
		public const uint StructSize = 56;

		/// <inheritdoc/>
		public uint Frame { get; set; }

		/// <summary>
		/// Type of particle.
		/// </summary>
		public SimpleParticleType Type { get; set; }

		/// <summary>
		/// ID of the particle motion to play this particle on.
		/// </summary>
		public byte MotionID { get; set; }

		/// <summary>
		/// Event texture ID (used for pulse).
		/// </summary>
		public float TextureID { get; set; }

		/// <summary>
		/// Pulse control mode.
		/// </summary>
		public float PulseControl { get; set; }

		/// <summary>
		/// Unknown.
		/// </summary>
		public float Unknown { get; set; }

		/// <summary>
		/// Scale of the particle.
		/// </summary>
		public float Scale { get; set; }


		/// <summary>
		/// Creates a new particle effect.
		/// </summary>
		/// <param name="frame">Frame at which the effect starts playing.</param>
		/// <param name="type">Type of particle.</param>
		/// <param name="motionID">ID of the particle motion to play this particle on.</param>
		/// <param name="textureID">Event texture ID (used for pulse).</param>
		/// <param name="pulseControl">Pulse control mode.</param>
		/// <param name="unknown">Unknown.</param>
		/// <param name="scale">Scale of the particle.</param>
		public SimpleParticleEffect(uint frame, SimpleParticleType type, byte motionID, float textureID, float pulseControl, float unknown, float scale)
		{
			Frame = frame;
			Type = type;
			MotionID = motionID;
			TextureID = textureID;
			PulseControl = pulseControl;
			Unknown = unknown;
			Scale = scale;
		}


		/// <summary>
		/// Writes the simple particle effect to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		public readonly void Write(EndianStackWriter writer)
		{
			writer.WriteUInt(Frame);
			writer.WriteByte((byte)Type);
			writer.WriteByte(MotionID);
			writer.WriteEmpty(2);
			writer.WriteFloat(TextureID);
			writer.WriteFloat(PulseControl);
			writer.WriteFloat(Unknown);
			writer.WriteFloat(Scale);
			writer.WriteEmpty(32);
		}

		/// <summary>
		/// Reads a simple particle effect off an endian stack reader.
		/// </summary>
		/// <param name="reader">Reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <returns>The simple particle effect that was read.</returns>
		public static SimpleParticleEffect Read(EndianStackReader reader, uint address)
		{
			return new(
				reader.ReadUInt(address),
				(SimpleParticleType)reader[address + 4],
				reader[address + 5],
				reader.ReadFloat(address + 8),
				reader.ReadFloat(address + 0xC),
				reader.ReadFloat(address + 0x10),
				reader.ReadFloat(address + 0x14)
				);
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is SimpleParticleEffect effect &&
				   Frame == effect.Frame &&
				   Type == effect.Type &&
				   MotionID == effect.MotionID &&
				   TextureID == effect.TextureID &&
				   PulseControl == effect.PulseControl &&
				   Unknown == effect.Unknown &&
				   Scale == effect.Scale;
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(Frame, Type, MotionID, TextureID, PulseControl, Unknown, Scale);
		}

		readonly bool IEquatable<SimpleParticleEffect>.Equals(SimpleParticleEffect other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two simple particle effects for equality.
		/// </summary>
		/// <param name="left">Lefthand simple particle effect.</param>
		/// <param name="right">Righthand simple particle effect.</param>
		/// <returns>Whether the two simple particle effects are equal</returns>
		public static bool operator ==(SimpleParticleEffect left, SimpleParticleEffect right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two screen effects for inequality.
		/// </summary>
		/// <param name="left">Lefthand screen effect.</param>
		/// <param name="right">Righthand screen effect.</param>
		/// <returns>Whether the two screen effects are inequal</returns>
		public static bool operator !=(SimpleParticleEffect left, SimpleParticleEffect right)
		{
			return !(left == right);
		}


		/// <inheritdoc/>
		public override readonly string ToString()
		{
			return Frame == 0 ? "-" : $"[{Frame}] - {Type} / {MotionID} -- {TextureID:F1} / {PulseControl:F1} / {Unknown:F1} / {Scale:F3}";
		}
	}
}
