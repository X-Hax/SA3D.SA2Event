using Amicitia.IO.Binary;
using Amicitia.IO.Streams;
using SA3D.Archival;
using SA3D.Common.IO;
using SA3D.Modeling.TexName;
using SA3D.SA2Event.Effects;
using SA3D.SA2Event.Language;
using SA3D.SA2Event.Model;
using SA3D.SA2Event.Model.AnimationData;
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
		public EventType Type => ModelData.EventType;

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
		public IArchive? TextureArchive { get; set; }

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
		public Event(ModelData modelData, EventEffects? effects, IArchive? textureArchive, TextureNameList? externalTexList)
		{
			ModelData = modelData;
			Effects = effects;
			LanguageTimestamps = [];
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
			using MemoryStream? animationStream = source.Animations == null ? null : new(source.Animations);
			BinaryObjectReader? animationReader = animationStream == null ? null : new(animationStream, StreamOwnership.Retain, Endianness.Big);
			EventModelIOContext context = EventModelIOContext.CreateForReading(animationReader);
			ModelData modelData = FileUtil.ReadFile<ModelData, EventModelIOContext>(source.Model, context);

			IArchive? archive = null;
			if(source.Textures != null && !IArchive.TryReadArchiveFromBytes(source.Textures, out archive, PRSDetectionMode.Never))
			{
				throw new InvalidDataException("Failed to read texture data!");
			}

			EventEffects? effects = source.Effects?.ReadFile<EventEffects, EventType>(context.EventType);
			TextureNameList? externalTexList = source.Texlist?.ReadFile<EventTextureNameListFile, EventType>(context.EventType)?.TextureNames;
			Event result = new(modelData, effects, archive, externalTexList);

			foreach(KeyValuePair<EventLanguage, byte[]> item in source.LanguageTimestamps)
			{
				EventLanguageTimestamps timestamps = FileUtil.ReadFile<EventLanguageTimestamps, EventType>(item.Value, context.EventType);
				result.LanguageTimestamps.Add(item.Key, timestamps);
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
			EventModelIOContext context = EventModelIOContext.CreateForWriting();
			context.EventType = Type;

			byte[] modelData = ModelData.WriteFileToBytes(context);
			byte[]? effects = Effects?.WriteFileToBytes(context.EventType);
			byte[]? textures = TextureArchive?.WriteToBytes();

			byte[]? texList = ExternalTexlist == null ? null : new EventTextureNameListFile(ExternalTexlist).WriteFileToBytes(context.EventType);
			byte[]? motionData = context.EventType == EventType.gc ? new EventAnimationFile([.. context.OutputAnimations!]).WriteToBytes() : null;

			Dictionary<EventLanguage, byte[]> languageInfo = [];
			foreach(KeyValuePair<EventLanguage, EventLanguageTimestamps> item in LanguageTimestamps)
			{
				languageInfo.Add(item.Key, item.Value.WriteFileToBytes(context.EventType));
			}

			return new EventSource(null, modelData, motionData, textures, texList, effects, languageInfo);
		}

		/// <summary>
		/// Write the event out as files
		/// </summary>
		/// <param name="filepath">The file path to the main file</param>
		/// <param name="compress">Whether to compress the event data</param>
		public void WriteToFiles(string filepath, bool compress = true)
		{
			WriteToSource().WriteToFiles(filepath, compress);
		}
	}
}
