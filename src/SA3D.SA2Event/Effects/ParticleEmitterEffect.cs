using SA3D.Common.IO;
using SA3D.Modeling.Structs;
using System;
using System.Numerics;

namespace SA3D.SA2Event.Effects
{
	/// <summary>
	/// Particle emitter effect.
	/// </summary>
	public struct ParticleEmitterEffect : IFrame, IEquatable<ParticleEmitterEffect>
	{
		/// <summary>
		/// Size of the structure in bytes.
		/// </summary>
		public const uint StructSize = 64;

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

		/// <summary>
		/// Creates a new particle emitter effect.
		/// </summary>
		/// <param name="position">World space position of the emitter.</param>
		/// <param name="unknown2">Unknown.</param>
		/// <param name="unknown3">Unknown.</param>
		/// <param name="unknown4">Unknown.</param>
		/// <param name="unknown5">Unknown.</param>
		/// <param name="unknown6">Unknown.</param>
		/// <param name="frame">Frame at which the emitter starts playing.</param>
		/// <param name="spread">Direction in which the particles spread (?)</param>
		/// <param name="count">Count (?).</param>
		/// <param name="unknown9">Unknown.</param>
		/// <param name="type">Type (?).</param>
		/// <param name="unknown11">Unknown.</param>
		public ParticleEmitterEffect(
			Vector3 position,
			Vector3 unknown2,
			ushort unknown3,
			ushort unknown4,
			ushort unknown5,
			ushort unknown6,
			uint frame,
			Vector3 spread,
			int count,
			int unknown9,
			int type,
			int unknown11)
		{
			Position = position;
			Unknown2 = unknown2;
			Unknown3 = unknown3;
			Unknown4 = unknown4;
			Unknown5 = unknown5;
			Unknown6 = unknown6;
			Frame = frame;
			Spread = spread;
			Count = count;
			Unknown9 = unknown9;
			Type = type;
			Unknown11 = unknown11;
		}


		/// <summary>
		/// Writes the particle emitter effect to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		public readonly void Write(EndianStackWriter writer)
		{
			writer.WriteVector3(Position);
			writer.WriteVector3(Unknown2);
			writer.WriteUShort(Unknown3);
			writer.WriteUShort(Unknown4);
			writer.WriteUShort(Unknown5);
			writer.WriteUShort(Unknown6);
			writer.WriteUInt(Frame);
			writer.WriteVector3(Spread);
			writer.WriteInt(Count);
			writer.WriteInt(Unknown9);
			writer.WriteInt(Type);
			writer.WriteInt(Unknown11);
		}

		/// <summary>
		/// Reads a particle emitter effect off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <returns></returns>
		public static ParticleEmitterEffect Read(EndianStackReader reader, uint address)
		{
			return new(
				reader.ReadVector3(address),
				reader.ReadVector3(address + 0xC),
				reader.ReadUShort(address + 0x18),
				reader.ReadUShort(address + 0x1A),
				reader.ReadUShort(address + 0x1C),
				reader.ReadUShort(address + 0x1E),
				reader.ReadUInt(address + 0x20),
				reader.ReadVector3(address + 0x24),
				reader.ReadInt(address + 0x30),
				reader.ReadInt(address + 0x34),
				reader.ReadInt(address + 0x38),
				reader.ReadInt(address + 0x3C));
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
