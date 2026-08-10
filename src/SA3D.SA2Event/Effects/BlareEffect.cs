using Amicitia.IO.Binary;
using SA3D.SA2Event.Model;
using System;

namespace SA3D.SA2Event.Effects
{
	/// <summary>
	/// A type of motion blur effect that went unused.
	/// </summary>
	public struct BlareEffect : IFrame, IEquatable<BlareEffect>, IBinarySerializable
	{
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

		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader)
		{
			Frame = reader.ReadUInt32();
			Duration = reader.ReadInt32();
			ModelIndex1 = reader.ReadByte();
			ModelIndex2 = reader.ReadByte();
			ModelIndex3 = reader.ReadByte();
			ModelIndex4 = reader.ReadByte();
			ModelIndex5 = reader.ReadByte();
			ModelIndex6 = reader.ReadByte();
			reader.Skip(2);
			GhostLifeSpan = reader.ReadInt32();
			reader.Skip(44);
		}

		/// <inheritdoc/>
		public readonly void Write(BinaryObjectWriter writer)
		{
			writer.WriteUInt32(Frame);
			writer.WriteInt32(Duration);

			writer.WriteByte(ModelIndex1);
			writer.WriteByte(ModelIndex2);
			writer.WriteByte(ModelIndex3);
			writer.WriteByte(ModelIndex4);
			writer.WriteByte(ModelIndex5);
			writer.WriteByte(ModelIndex6);

			writer.Skip(2);
			writer.WriteInt32(GhostLifeSpan);
			writer.Skip(44);
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
