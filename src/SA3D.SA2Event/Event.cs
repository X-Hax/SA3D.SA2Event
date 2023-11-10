using SA3D.Archival;
using SA3D.Common.IO;
using SA3D.SA2Event.Animation;
using SA3D.SA2Event.Effects;
using SA3D.SA2Event.Language;
using SA3D.SA2Event.Model;
using SA3D.Texturing.Texname;
using System.Collections.Generic;
using System.IO;

namespace SA3D.SA2Event
{
	/// <summary>
	/// Sonic adventure 2 event data.
	/// </summary>
	public class Event
	{
		/// <summary>
		/// Type of the event.
		/// </summary>
		public EventType Type => ModelData.Type;

		/// <summary>
		/// Model, animation and more data.
		/// </summary>
		public ModelData ModelData { get; set; }

		/// <summary>
		/// Effect data, such as particles, lights, timestamps, etc.
		/// </summary>
		public EventEffects? Effects { get; set; }

		/// <summary>
		/// Language specific subtitles and audio timestamps. Overrides the base language timestamps in <see cref="Effects"/>.
		/// </summary>
		public Dictionary<EventLanguage, EventLanguageTimestamps> LanguageTimestamps { get; }

		/// <summary>
		/// Texture archive. Should be either PAK, PVM or GVM.
		/// </summary>
		public Archive? TextureArchive { get; set; }

		/// <summary>
		/// External texture name list.
		/// </summary>
		public TextureNameList? ExternalTexlist { get; set; }


		/// <summary>
		/// Creates a new sa2 event.
		/// </summary>
		/// <param name="modelData">Model, animation and more data.</param>
		/// <param name="effects">Effect data, such as particles, lights, timestamps, etc.</param>
		/// <param name="textureArchive">Archive storing texture files.</param>
		/// <param name="externalTexList">External texture name list.</param>
		public Event(ModelData modelData, EventEffects? effects, Archive? textureArchive, TextureNameList? externalTexList)
		{
			ModelData = modelData;
			Effects = effects;
			LanguageTimestamps = new();
			TextureArchive = textureArchive;
			ExternalTexlist = externalTexList;
		}


		/// <summary>
		/// Reads event data from source data.
		/// </summary>
		/// <param name="source">The event source to read.</param>
		/// <returns>The event that was read.</returns>
		public static Event ReadFromSource(EventSource source)
		{
			ModelData mainData = ModelData.Read(source);
			bool bigEndian = mainData.Type.GetBigEndian();

			Archive? archive = source.Textures == null ? null : Archive.ReadArchive(source.Textures, 0);
			EventEffects? effects = EventEffects.ReadFromEventSource(source, bigEndian);

			TextureNameList? externalTexList = null;
			if(source.Texlist != null)
			{
				using(EndianStackReader texListReader = new(source.Texlist, mainData.Type.GetTextureImageBase(), bigEndian))
				{
					externalTexList = TextureNameList.Read(texListReader, texListReader.ReadPointer(0), new());
				}
			}

			Event result = new(mainData, effects, archive, externalTexList);

			foreach(KeyValuePair<EventLanguage, byte[]> item in source.LanguageTimestamps)
			{
				// language and effect files have no pointers, so no need for an image base
				using(EndianStackReader reader = new(item.Value, 0, bigEndian))
				{
					result.LanguageTimestamps.Add(item.Key, EventLanguageTimestamps.Read(reader));
				}
			}

			return result;
		}

		/// <summary>
		/// Reads event data from files.
		/// </summary>
		/// <param name="filepath">Path to the model file or the base file path.</param>
		/// <returns>The event that was read.</returns>
		public static Event ReadFromFiles(string filepath)
		{
			EventSource source = EventSource.ReadFromFiles(filepath);
			return ReadFromSource(source);
		}

		/// <summary>
		/// Writes the event out as byte data and stores it in an event source.
		/// </summary>
		/// <returns>The written event source.</returns>
		public EventSource WriteToSource()
		{
			bool bigEndian = Type.GetBigEndian();

			byte[] mainData = ModelData.WriteToData(out EventMotion[] motions);
			byte[]? effects = Effects?.WriteToData(bigEndian);
			byte[]? textures = TextureArchive?.WriteArchiveToBytes();
			byte[]? texList = WriteTexList();
			byte[]? motionData = Type == EventType.gc ? EventMotion.WriteMotionsToData(motions) : null;

			Dictionary<EventLanguage, byte[]> languageInfo = new();
			foreach(KeyValuePair<EventLanguage, EventLanguageTimestamps> item in LanguageTimestamps)
			{
				languageInfo.Add(item.Key, item.Value.WriteToData(bigEndian));
			}

			return new EventSource(null, mainData, motionData, textures, texList, effects, languageInfo);
		}

		/// <summary>
		/// Writes the event out as multiple files.
		/// </summary>
		/// <param name="filepath">Path to the model file or the base file path.</param>
		public void WriteToFiles(string filepath)
		{
			WriteToSource().WriteToFiles(filepath);
		}

		private byte[]? WriteTexList()
		{
			if(ExternalTexlist == null)
			{
				return null;
			}

			using(MemoryStream stream = new())
			{
				EndianStackWriter writer = new(stream);

				switch(Type)
				{
					case EventType.dcbeta:
					case EventType.dc:
						writer.ImageBase = 0xCBC0000;
						break;
					case EventType.dcgc:
						writer.PushBigEndian(true);
						writer.ImageBase = 0x818BFE60;
						break;
					default:
					case EventType.gc:
						writer.PushBigEndian(true);
						break;
				}

				writer.WriteEmpty(4);
				uint addr = ExternalTexlist.Write(writer, new());
				writer.SeekStart();
				writer.WriteUInt(addr);
				writer.SeekEnd();

				return stream.ToArray();
			}
		}
	}
}
