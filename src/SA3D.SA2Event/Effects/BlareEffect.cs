using SA3D.Common.IO;
using SA3D.SA2Event.Model;
using System;

namespace SA3D.SA2Event.Effects
{
	/// <summary>
	/// A type of motion blur effect that went unused.
	/// </summary>
	public struct BlareEffect : IFrame, IEquatable<BlareEffect>
	{
		/// <summary>
		/// Size of the structure in bytes.
		/// </summary>
		public const uint StructSize = 64;

		/// <summary>
		/// Default value structure.
		/// </summary>
		public static readonly BlareEffect Default = new(0, 0, 0);


		/// <inheritdoc/>
		public uint Frame { get; set; }

		/// <summary>
		/// Duration in frames in which ghosts should be continually spawned.
		/// </summary>
		public int Duration { get; set; }

		/// <summary>
		/// First index referencing a model in <see cref="ModelData.BlareModels"/>. <see cref="byte.MaxValue"/> indicates null.
		/// </summary>
		public byte ModelIndex1 { get; set; }

		/// <summary>
		/// Second index referencing a model in <see cref="ModelData.BlareModels"/>. <see cref="byte.MaxValue"/> indicates null.
		/// </summary>
		public byte ModelIndex2 { get; set; }

		/// <summary>
		/// Third index referencing a model in <see cref="ModelData.BlareModels"/>. <see cref="byte.MaxValue"/> indicates null.
		/// </summary>
		public byte ModelIndex3 { get; set; }

		/// <summary>
		/// Fourth index referencing a model in <see cref="ModelData.BlareModels"/>. <see cref="byte.MaxValue"/> indicates null.
		/// </summary>
		public byte ModelIndex4 { get; set; }

		/// <summary>
		/// Fifth index referencing a model in <see cref="ModelData.BlareModels"/>. <see cref="byte.MaxValue"/> indicates null.
		/// </summary>
		public byte ModelIndex5 { get; set; }

		/// <summary>
		/// Sixth index referencing a model in <see cref="ModelData.BlareModels"/>. <see cref="byte.MaxValue"/> indicates null.
		/// </summary>
		public byte ModelIndex6 { get; set; }

		/// <summary>
		/// Number of task cycles / frames it takes for the ghost to fade out.
		/// </summary>
		public int GhostLifeSpan { get; set; }


		/// <summary>
		/// Creates a new blare effect.
		/// </summary>
		/// <param name="frame">Frame at which the effect starts playing.</param>
		/// <param name="duration">Duration in frames in which ghosts should be continually spawned.</param>
		/// <param name="modelIndex1">First index referencing a model in <see cref="ModelData.BlareModels"/>. <see cref="byte.MaxValue"/> indicates null.</param>
		/// <param name="modelIndex2">Second index referencing a model in <see cref="ModelData.BlareModels"/>. <see cref="byte.MaxValue"/> indicates null.</param>
		/// <param name="modelIndex3">Third index referencing a model in <see cref="ModelData.BlareModels"/>. <see cref="byte.MaxValue"/> indicates null.</param>
		/// <param name="modelIndex4">Fourth index referencing a model in <see cref="ModelData.BlareModels"/>. <see cref="byte.MaxValue"/> indicates null.</param>
		/// <param name="modelIndex5">Fifth index referencing a model in <see cref="ModelData.BlareModels"/>. <see cref="byte.MaxValue"/> indicates null.</param>
		/// <param name="modelIndex6">Sixth index referencing a model in <see cref="ModelData.BlareModels"/>. <see cref="byte.MaxValue"/> indicates null.</param>
		/// <param name="ghostLifeSpan">Number of task cycles / frames it takes for the ghost to fade out.</param>
		public BlareEffect(uint frame, int duration, byte modelIndex1, byte modelIndex2, byte modelIndex3, byte modelIndex4, byte modelIndex5, byte modelIndex6, int ghostLifeSpan)
		{
			Frame = frame;
			Duration = duration;
			ModelIndex1 = modelIndex1;
			ModelIndex2 = modelIndex2;
			ModelIndex3 = modelIndex3;
			ModelIndex4 = modelIndex4;
			ModelIndex5 = modelIndex5;
			ModelIndex6 = modelIndex6;
			GhostLifeSpan = ghostLifeSpan;
		}

		/// <summary>
		/// Creates a new blare effect with no model indices set.
		/// </summary>
		/// <param name="frame">Frame at which the effect starts playing.</param>
		/// <param name="duration">Duration in frames in which ghosts should be continually spawned.</param>
		/// <param name="ghostLifeSpan">Number of task cycles / frames it takes for the ghost to fade out.</param>
		public BlareEffect(uint frame, int duration, int ghostLifeSpan)
		{
			Frame = frame;
			Duration = duration;
			ModelIndex1 = byte.MaxValue;
			ModelIndex2 = byte.MaxValue;
			ModelIndex3 = byte.MaxValue;
			ModelIndex4 = byte.MaxValue;
			ModelIndex5 = byte.MaxValue;
			ModelIndex6 = byte.MaxValue;
			GhostLifeSpan = ghostLifeSpan;
		}


		/// <summary>
		/// Writes the blare effect to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		public readonly void Write(EndianStackWriter writer)
		{
			writer.WriteUInt(Frame);
			writer.WriteInt(Duration);

			writer.WriteByte(ModelIndex1);
			writer.WriteByte(ModelIndex2);
			writer.WriteByte(ModelIndex3);
			writer.WriteByte(ModelIndex4);
			writer.WriteByte(ModelIndex5);
			writer.WriteByte(ModelIndex6);

			writer.WriteEmpty(2);
			writer.WriteInt(GhostLifeSpan);
			writer.WriteEmpty(44);
		}

		/// <summary>
		/// Reads a blare effect off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <returns>The blare effect that was read.</returns>
		public static BlareEffect Read(EndianStackReader reader, uint address)
		{
			return new(
				reader.ReadUInt(address),
				reader.ReadInt(address + 4),
				reader[address + 8],
				reader[address + 9],
				reader[address + 0xA],
				reader[address + 0xB],
				reader[address + 0xC],
				reader[address + 0xD],
				reader.ReadInt(address + 0x10));
		}

		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is BlareEffect effect &&
				   Frame == effect.Frame &&
				   Duration == effect.Duration &&
				   ModelIndex1 == effect.ModelIndex1 &&
				   ModelIndex2 == effect.ModelIndex2 &&
				   ModelIndex3 == effect.ModelIndex3 &&
				   ModelIndex4 == effect.ModelIndex4 &&
				   ModelIndex5 == effect.ModelIndex5 &&
				   ModelIndex6 == effect.ModelIndex6 &&
				   GhostLifeSpan == effect.GhostLifeSpan;
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			HashCode hash = new();
			hash.Add(Frame);
			hash.Add(Duration);
			hash.Add(ModelIndex1);
			hash.Add(ModelIndex2);
			hash.Add(ModelIndex3);
			hash.Add(ModelIndex4);
			hash.Add(ModelIndex5);
			hash.Add(ModelIndex6);
			hash.Add(GhostLifeSpan);
			return hash.ToHashCode();
		}

		readonly bool IEquatable<BlareEffect>.Equals(BlareEffect other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two blare effects for equality.
		/// </summary>
		/// <param name="left">Lefthand blare effect.</param>
		/// <param name="right">Righthand blare effect.</param>
		/// <returns>Whether the two blare effects are equal</returns>
		public static bool operator ==(BlareEffect left, BlareEffect right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two blare effects for inequality.
		/// </summary>
		/// <param name="left">Lefthand blare effect.</param>
		/// <param name="right">Righthand blare effect.</param>
		/// <returns>Whether the two blare effects are inequal</returns>
		public static bool operator !=(BlareEffect left, BlareEffect right)
		{
			return !(left == right);
		}


		/// <inheritdoc/>
		public override readonly string ToString()
		{
			return Frame == 0 ? "-" : $"[{Frame}] {Duration}, [{ModelIndex1}, {ModelIndex2}, {ModelIndex3}, {ModelIndex4}, {ModelIndex5}, {ModelIndex6}], {GhostLifeSpan}";
		}
	}
}
