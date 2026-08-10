using Amicitia.IO.Binary;
using SA3D.Common.IO;
using SA3D.Modeling.Structs;
using System;
using System.Numerics;

namespace SA3D.SA2Event.Effects
{
	/// <summary>
	/// Particle emitter effect.
	/// </summary>
	public struct ParticleEmitterEffect : IFrame, IEquatable<ParticleEmitterEffect>, IBinarySerializable
	{
		/// <summary>
		/// World space position of the emitter.
		/// </summary>
		public Vector3 Position { get; set; }

		/// <summary>
		/// Unknown.
		/// </summary>
		public Vector3 Unknown2 { get; set; }

		/// <summary>
		/// Unknown.
		/// </summary>
		public ushort Unknown3 { get; set; }

		/// <summary>
		/// Unknown.
		/// </summary>
		public ushort Unknown4 { get; set; }

		/// <summary>
		/// Unknown.
		/// </summary>
		public ushort Unknown5 { get; set; }

		/// <summary>
		/// Unknown.
		/// </summary>
		public ushort Unknown6 { get; set; }

		/// <inheritdoc/>
		public uint Frame { get; set; }

		/// <summary>
		/// Direction in which the particles spread (?)
		/// </summary>
		public Vector3 Spread { get; set; }

		/// <summary>
		/// Count (?).
		/// </summary>
		public int Count { get; set; }

		/// <summary>
		/// Unknown.
		/// </summary>
		public int Unknown9 { get; set; }

		/// <summary>
		/// Type (?).
		/// </summary>
		public int Type { get; set; }

		/// <summary>
		/// Unknown.
		/// </summary>
		public int Unknown11 { get; set; }


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader)
		{
			Position = reader.ReadVector3();
			Unknown2 = reader.ReadVector3();
			Unknown3 = reader.ReadUInt16();
			Unknown4 = reader.ReadUInt16();
			Unknown5 = reader.ReadUInt16();
			Unknown6 = reader.ReadUInt16();
			Frame = reader.ReadUInt32();
			Spread = reader.ReadVector3();
			Count = reader.ReadInt32();
			Unknown9 = reader.ReadInt32();
			Type = reader.ReadInt32();
			Unknown11 = reader.ReadInt32();
		}

		/// <inheritdoc/>
		public readonly void Write(BinaryObjectWriter writer)
		{ 
			writer.WriteVector3(Position);
			writer.WriteVector3(Unknown2);
			writer.WriteUInt16(Unknown3);
			writer.WriteUInt16(Unknown4);
			writer.WriteUInt16(Unknown5);
			writer.WriteUInt16(Unknown6);
			writer.WriteUInt32(Frame);
			writer.WriteVector3(Spread);
			writer.WriteInt32(Count);
			writer.WriteInt32(Unknown9);
			writer.WriteInt32(Type);
			writer.WriteInt32(Unknown11);
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is ParticleEmitterEffect effect &&
				   Position.Equals(effect.Position) &&
				   Unknown2.Equals(effect.Unknown2) &&
				   Unknown3 == effect.Unknown3 &&
				   Unknown4 == effect.Unknown4 &&
				   Unknown5 == effect.Unknown5 &&
				   Unknown6 == effect.Unknown6 &&
				   Frame == effect.Frame &&
				   Spread.Equals(effect.Spread) &&
				   Count == effect.Count &&
				   Unknown9 == effect.Unknown9 &&
				   Type == effect.Type &&
				   Unknown11 == effect.Unknown11;
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			HashCode hash = new();
			hash.Add(Position);
			hash.Add(Unknown2);
			hash.Add(Unknown3);
			hash.Add(Unknown4);
			hash.Add(Unknown5);
			hash.Add(Unknown6);
			hash.Add(Frame);
			hash.Add(Spread);
			hash.Add(Count);
			hash.Add(Unknown9);
			hash.Add(Type);
			hash.Add(Unknown11);
			return hash.ToHashCode();
		}

		readonly bool IEquatable<ParticleEmitterEffect>.Equals(ParticleEmitterEffect other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two particle emitter effects for equality.
		/// </summary>
		/// <param name="left">Lefthand emitter.</param>
		/// <param name="right">Righthand emitter.</param>
		/// <returns>Whether the two particle emitter effects are equal</returns>
		public static bool operator ==(ParticleEmitterEffect left, ParticleEmitterEffect right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two particle emitter effects for inequality.
		/// </summary>
		/// <param name="left">Lefthand emitter.</param>
		/// <param name="right">Righthand emitter.</param>
		/// <returns>Whether the two particle emitter effects are inequal</returns>
		public static bool operator !=(ParticleEmitterEffect left, ParticleEmitterEffect right)
		{
			return !(left == right);
		}


		/// <inheritdoc/>
		public override readonly string ToString()
		{
			return Equals(default(ParticleEmitterEffect)) ? "-" : "X";
		}


	}
}
