using SA3D.Archival;
using SA3D.Common.IO;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SA3D.SA2Event.Language
{
	/// <summary>
	/// Data handler for subtitle files.
	/// </summary>
	public class SubtitleFile
	{
		/// <summary>
		/// List of subtitles for every event (by ID).
		/// </summary>
		public SortedDictionary<uint, List<SubtitleText>> Texts { get; }

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
			Texts = new();
			TextEncoding = encoding;
		}


		/// <summary>
		/// Writes the subtitle file to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		public void Write(EndianStackWriter writer)
		{
			uint tableStart = writer.Position;
			writer.WriteEmpty((uint)(Texts.Count * 0xC));
			writer.WriteUInt(uint.MaxValue);
			writer.WriteEmpty(8);

			(uint index, uint addr, uint pos, int count)[] table
				= new (uint index, uint addr, uint pos, int count)[Texts.Count];

			int i = 0;
			foreach(KeyValuePair<uint, List<SubtitleText>> collection in Texts)
			{
				table[i++] = (collection.Key, writer.PointerPosition, writer.Position, collection.Value.Count);
				writer.WriteEmpty((uint)(8 * collection.Value.Count));
			}

			writer.Stream.Seek(tableStart, SeekOrigin.Begin);
			foreach((uint index, uint addr, _, int count) in table)
			{
				writer.WriteUInt(index);
				writer.WriteUInt(addr);
				writer.WriteInt(count);
			}

			writer.Stream.Seek(0, SeekOrigin.End);

			i = 0;
			foreach(KeyValuePair<uint, List<SubtitleText>> collection in Texts)
			{
				uint[] stringPointers = new uint[collection.Value.Count];
				for(int j = 0; j < stringPointers.Length; j++)
				{
					stringPointers[j] = writer.PointerPosition;
					SubtitleText text = collection.Value[j];
					if(text.Centered)
					{
						writer.WriteByte(7);
					}

					writer.WriteStringNullterminated(text.Text, TextEncoding);
				}

				writer.Stream.Seek(table[i++].pos, SeekOrigin.Begin);

				for(int j = 0; j < stringPointers.Length; j++)
				{
					writer.WriteInt(collection.Value[j].Character);
					writer.WriteUInt(stringPointers[j]);
				}

				writer.Stream.Seek(0, SeekOrigin.End);
			}
		}

		/// <summary>
		/// Writes the subtitle file to byte data.
		/// </summary>
		/// <param name="imageBase">The image base to use.</param>
		/// <param name="bigEndian">Whether to encode in big endian.</param>
		/// <param name="compress">Whether to compress the data with PRS.</param>
		/// <returns>The written out byte data.</returns>
		public byte[] WriteToData(uint imageBase, bool bigEndian, bool compress = true)
		{
			byte[] result;

			using(MemoryStream stream = new())
			{
				EndianStackWriter writer = new(stream, imageBase, bigEndian);
				Write(writer);

				result = stream.ToArray();
			}

			if(compress)
			{
				result = PRS.CompressPRS(result);
			}

			return result;
		}

		/// <summary>
		/// Writes the subtitle file to a file.
		/// </summary>
		/// <param name="file">The path to the file to write to.</param>
		/// <param name="imageBase">The image base to use.</param>
		/// <param name="bigEndian">Whether to encode in big endian.</param>
		/// <param name="compress">Whether to compress the data with PRS.</param>
		/// <returns>The written out byte data.</returns>
		public void WriteToFile(string file, uint imageBase, bool bigEndian, bool compress = true)
		{
			File.WriteAllBytes(file, WriteToData(imageBase, bigEndian));
		}


		/// <summary>
		/// Reads a subtitle file off an endian stack reader.
		/// </summary>
		/// <param name="reader">Reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="encoding">Text encoding to decode texts with.</param>
		/// <returns>The subtitle file that was read.</returns>
		public static SubtitleFile Read(EndianStackReader reader, uint address, Encoding encoding)
		{
			SubtitleFile result = new(encoding);

			uint index = reader.ReadUInt(address);
			while(index != uint.MaxValue)
			{
				List<SubtitleText> subtitles = new();
				result.Texts.Add(index, subtitles);

				uint stringListPtr = reader.ReadPointer(address + 4);
				string[] strings = new string[reader.ReadInt(address + 8)];

				for(int i = 0; i < strings.Length; i++)
				{
					int characterIndex = reader.ReadInt(stringListPtr);
					uint stringPtr = reader.ReadPointer(stringListPtr + 4);
					bool centered = reader[stringPtr] == 7;
					string text = reader.ReadNullterminatedString(stringPtr + (centered ? 1u : 0), encoding);
					subtitles.Add(new(characterIndex, centered, text));

					stringListPtr += 8;
				}

				address += 0xC;
				index = reader.ReadUInt(address);
			}

			return result;
		}

		/// <summary>
		/// Reads a subtitle file off byte data.
		/// </summary>
		/// <param name="data">Data to read.</param>
		/// <param name="imagebase">Imagebase to use.</param>
		/// <param name="bigEndian">Whether to decode in big endian.</param>
		/// <param name="compressed">Whether the data needs to be decompressed with PRS.</param>
		/// <param name="encoding">Text encoding to decode texts with.</param>
		/// <returns>The subtitle file that was read.</returns>
		public static SubtitleFile ReadFromData(byte[] data, uint imagebase, bool bigEndian, bool compressed, Encoding encoding)
		{
			if(compressed)
			{
				data = PRS.DecompressPRS(data);
			}

			using(EndianStackReader reader = new(data, imagebase, bigEndian))
			{
				return Read(reader, 0, encoding);
			}
		}

		/// <summary>
		/// Reads a subtitle file off byte data.
		/// </summary>
		/// <param name="path">Path to the file to read.</param>
		/// <param name="imagebase">Imagebase to use.</param>
		/// <param name="bigEndian">Whether to decode in big endian.</param>
		/// <param name="compressed">Whether the data needs to be decompressed with PRS.</param>
		/// <param name="encoding">Text encoding to decode texts with.</param>
		/// <returns>The subtitle file that was read.</returns>
		public static SubtitleFile ReadFromFile(string path, uint imagebase, bool bigEndian, bool compressed, Encoding encoding)
		{
			return ReadFromData(File.ReadAllBytes(path), imagebase, bigEndian, compressed, encoding);
		}
	
	}
}
