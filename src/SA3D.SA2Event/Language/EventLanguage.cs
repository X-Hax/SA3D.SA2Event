using SA3D.Common.IO;
using System.IO;

namespace SA3D.SA2Event.Language
{
	/// <summary>
	/// Event subtitle and audio timestamps
	/// </summary>
	public class EventLanguageTimestamps
	{
		/// <summary>
		/// Subtitle timestamps. Each timestamp is reponsible for the subtitle at the index it is placed at within the array.
		/// </summary>
		public SubtitleTimestamp[] SubtitlesTimestamps { get; }

		/// <summary>
		/// Audio timestamps for music and voice lines.
		/// </summary>
		public AudioTimestamp[] AudioTimestamps { get; }


		/// <summary>
		/// Creates a new set of language timestamps.
		/// </summary>
		public EventLanguageTimestamps()
		{
			SubtitlesTimestamps = new SubtitleTimestamp[256];
			AudioTimestamps = new AudioTimestamp[512];
		}


		/// <summary>
		/// Writes the language timestamps to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		public void Write(EndianStackWriter writer)
		{
			for(int i = 0; i < SubtitlesTimestamps.Length; i++)
			{
				SubtitlesTimestamps[i].Write(writer);
			}

			for(int i = 0; i < AudioTimestamps.Length; i++)
			{
				AudioTimestamps[i].Write(writer);
			}
		}

		/// <summary>
		/// Writes the language timestamps to byte data.
		/// </summary>
		/// <param name="bigEndian">Whether to write in big endian.</param>
		/// <returns>The written byte data.</returns>
		public byte[] WriteToData(bool bigEndian)
		{
			using(MemoryStream stream = new())
			{
				EndianStackWriter writer = new(stream, bigEndian: bigEndian);
				Write(writer);
				return stream.ToArray();
			}
		}

		/// <summary>
		/// Reads language timestamps off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <returns>The language timestamps that were read.</returns>
		public static EventLanguageTimestamps Read(EndianStackReader reader)
		{
			EventLanguageTimestamps result = new();

			reader.ReadArray(0, SubtitleTimestamp.Read, SubtitleTimestamp.StructSize, result.SubtitlesTimestamps);
			reader.ReadArray(0x800, AudioTimestamp.Read, AudioTimestamp.StructSize, result.AudioTimestamps);

			return result;
		}

	}
}
