using Amicitia.IO.Binary;
using SA3D.Common.IO;
using SA3D.Common.Lookup;
using System.Collections.Generic;
using System.Text;

namespace SA3D.SA2Event.Language
{
	/// <summary>
	/// Data handler for subtitle files.
	/// </summary>
	public class SubtitleFile : IFileSerializable<EventType>
	{
		/// <summary>
		/// Label prefix for arrays in <see cref="Texts"/>
		/// </summary>
		public const string TextArrayLabelPrefix = "TextArray_";

		/// <summary>
		/// List of subtitles for every event (by ID).
		/// </summary>
		public SortedDictionary<uint, LabeledArray<SubtitleText>> Texts { get; set; } = [];

		/// <summary>
		/// Encoding to use.
		/// </summary>
		public Encoding TextEncoding { get; set; }


		/// <summary>
		/// Creates a new subtitle file handler.
		/// </summary>
		/// <param name="encoding">The encoding to use.</param>
		public SubtitleFile(Encoding encoding)
		{
			Texts = [];
			TextEncoding = encoding;
		}


		void IBinarySerializable<EventType>.Read(BinaryObjectReader reader, EventType context)
		{
			TextEncoding = reader.Encoding;
			OffsetLUT lut = new();

			while(reader.ReadUInt32() is uint index && index != uint.MaxValue)
			{
				long textOffset = reader.ReadOffsetValue();
				int textCount = reader.ReadInt32();
				LabeledArray<SubtitleText> subtitles = reader.ReadLabeledObjectArrayAtOffset<SubtitleText>(textOffset, textCount, TextArrayLabelPrefix, lut)
					?? throw reader.ReadNullReference(nameof(SubtitleFile), "Subtitles");
				Texts.Add(index, subtitles);
			}
		}

		void IBinarySerializable<EventType>.Write(BinaryObjectWriter writer, EventType context)
		{
			OffsetLUT lut = new();

			foreach(KeyValuePair<uint, LabeledArray<SubtitleText>> texts in Texts)
			{
				writer.WriteUInt32(texts.Key);
				writer.WriteObjectArrayOffset(texts.Value, lut);
				writer.WriteInt32(texts.Value.Length);
			}

			writer.WriteUInt32(uint.MaxValue);
		}


		bool IFileSerializable<EventType>.CheckCanReadFile(BinaryObjectReader reader, EventType context, ref FileIOInfo fileInfo)
		{
			fileInfo.Endianness ??= context.GetEndianness();
			fileInfo.OffsetOrigin ??= context.GetSubtitleOffsetOrigin();
			return true;
		}

		bool IFileSerializable<EventType>.CheckCanWriteFile(EventType context, ref FileIOInfo fileInfo)
		{
			fileInfo.Endianness ??= context.GetEndianness();
			fileInfo.OffsetOrigin ??= context.GetSubtitleOffsetOrigin();
			return true;
		}
	}
}
