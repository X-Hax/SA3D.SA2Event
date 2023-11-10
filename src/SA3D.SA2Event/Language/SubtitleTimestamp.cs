using SA3D.Common.IO;
using System;

namespace SA3D.SA2Event.Language
{
	/// <summary>
	/// Timestamp at which a subtitle should be played.
	/// </summary>
	public struct SubtitleTimestamp : IFrame, IEquatable<SubtitleTimestamp>
	{
		/// <summary>
		/// Size of the structure in bytes.
		/// </summary>
		public const uint StructSize = 8;


		/// <inheritdoc/>
		public uint Frame { get; set; }

		/// <summary>
		/// Number of frames for which the subtitle should be visible.
		/// </summary>
		public uint Duration { get; set; }


		/// <summary>
		/// Creates a new subtitle timestamp.
		/// </summary>
		/// <param name="frame">Frame at which the effect starts playing.</param>
		/// <param name="duration">Number of frames for which the subtitle should be visible.</param>
		public SubtitleTimestamp(uint frame, uint duration)
		{
			Frame = frame;
			Duration = duration;
		}


		/// <summary>
		/// Writes the subtitle timestamp to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		public readonly void Write(EndianStackWriter writer)
		{
			writer.WriteUInt(Frame);
			writer.WriteUInt(Duration);
		}

		/// <summary>
		/// Reads a subtitle timestamp off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <returns></returns>
		public static SubtitleTimestamp Read(EndianStackReader reader, uint address)
		{
			return new(
				reader.ReadUInt(address),
				reader.ReadUInt(address + 4));
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
