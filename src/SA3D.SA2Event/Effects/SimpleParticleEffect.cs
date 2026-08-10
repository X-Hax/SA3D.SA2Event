using Amicitia.IO.Binary;
using SA3D.SA2Event.Effects.Enums;
using System;

namespace SA3D.SA2Event.Effects
{
	/// <summary>
	/// Simple particle effect structure.
	/// </summary>
	public struct SimpleParticleEffect : IFrame, IEquatable<SimpleParticleEffect>, IBinarySerializable
	{
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


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader)
		{
			Frame = reader.ReadUInt32();
			Type = (SimpleParticleType)reader.ReadByte();
			MotionID = reader.ReadByte();
			reader.Skip(2);
			TextureID = reader.ReadSingle();
			PulseControl = reader.ReadSingle();
			Unknown = reader.ReadSingle();
			Scale = reader.ReadSingle();
			reader.Skip(32);
		}

		/// <inheritdoc/>
		public readonly void Write(BinaryObjectWriter writer)
		{
			writer.WriteUInt32(Frame);
			writer.WriteByte((byte)Type);
			writer.WriteByte(MotionID);
			writer.Skip(2);
			writer.WriteSingle(TextureID);
			writer.WriteSingle(PulseControl);
			writer.WriteSingle(Unknown);
			writer.WriteSingle(Scale);
			writer.Skip(32);
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
