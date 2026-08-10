using Amicitia.IO.Binary;
using SA3D.Common.IO;

namespace SA3D.SA2Event.Language
{
	/// <summary>
	/// Event subtitle and audio timestamps
	/// </summary>
	public class EventLanguageTimestamps : IFileSerializable<EventType>
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


		void IBinarySerializable<EventType>.Read(BinaryObjectReader reader, EventType context)
		{
			SubtitlesTimestamps.ReadToObjectArray(reader);
			AudioTimestamps.ReadToObjectArray(reader);
		}

		void IBinarySerializable<EventType>.Write(BinaryObjectWriter writer, EventType context)
		{
			writer.WriteObjectArray(SubtitlesTimestamps);
			writer.WriteObjectArray(AudioTimestamps);
		}


		bool IFileSerializable<EventType>.CheckCanReadFile(BinaryObjectReader reader, EventType context, ref FileIOInfo fileInfo)
		{
			fileInfo.Endianness ??= context.GetEndianness();
			return true;
		}

		bool IFileSerializable<EventType>.CheckCanWriteFile(EventType context, ref FileIOInfo fileInfo)
		{
			fileInfo.Endianness ??= context.GetEndianness();
			return true;
		}

	}
}
