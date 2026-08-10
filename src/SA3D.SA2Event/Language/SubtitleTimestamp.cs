using Amicitia.IO.Binary;
using System;

namespace SA3D.SA2Event.Language
{
	/// <summary>
	/// Timestamp at which a subtitle should be played.
	/// </summary>
	public struct SubtitleTimestamp : IFrame, IEquatable<SubtitleTimestamp>, IBinarySerializable
	{
		/// <inheritdoc/>
		public uint Frame { get; set; }

		/// <summary>
		/// Number of frames for which the subtitle should be visible.
		/// </summary>
		public uint Duration { get; set; }


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader)
		{
			Frame = reader.ReadUInt32();
			Duration = reader.ReadUInt32();
		}

		/// <inheritdoc/>
		public readonly void Write(BinaryObjectWriter writer)
		{
			writer.WriteUInt32(Frame);
			writer.WriteUInt32(Duration);
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is SubtitleTimestamp timestamp &&
				   Frame == timestamp.Frame &&
				   Duration == timestamp.Duration;
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(Frame, Duration);
		}

		/// <inheritdoc/>
		readonly bool IEquatable<SubtitleTimestamp>.Equals(SubtitleTimestamp other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two subtitle timestamps for equality.
		/// </summary>
		/// <param name="left">Lefthand subtitle timestamp.</param>
		/// <param name="right">Righthand subtitle timestamp.</param>
		/// <returns>Whether the two subtitle timestamps are equal</returns>
		public static bool operator ==(SubtitleTimestamp left, SubtitleTimestamp right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two subtitle timestamps for inequality.
		/// </summary>
		/// <param name="left">Lefthand subtitle timestamp.</param>
		/// <param name="right">Righthand subtitle timestamp.</param>
		/// <returns>Whether the two subtitle timestamps are inequal</returns>
		public static bool operator !=(SubtitleTimestamp left, SubtitleTimestamp right)
		{
			return !(left == right);
		}
	

		/// <inheritdoc/>
		public override readonly string ToString()
		{
			return Frame == 0 ? "[-]" : $"[{Frame}] {Duration}";
		}
	}
}
