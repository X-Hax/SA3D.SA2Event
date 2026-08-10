using Amicitia.IO.Binary;
using Amicitia.IO.Binary.Extensions;
using SA3D.Common.IO;
using SA3D.Common.Lookup;
using SA3D.Modeling.TexName;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Texture name list file used by events
	/// </summary>
	public class EventTextureNameListFile : IFileSerializable<EventType>
	{
		/// <summary>
		/// Texture name list of the file
		/// </summary>
		public TextureNameList TextureNames { get; set; }


		/// <summary>
		/// Initializes a new instance of the <see cref="EventTextureNameListFile"/> class with a specified 
		/// </summary>
		public EventTextureNameListFile(TextureNameList textureNames)
		{
			TextureNames = textureNames;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventTextureNameListFile"/> class
		/// </summary>
		public EventTextureNameListFile() : this(new()) { }


		void IBinarySerializable<EventType>.Read(BinaryObjectReader reader, EventType context)
		{
			TextureNames = reader.ReadObjectOffset<TextureNameList, OffsetLUT>(new());
		}

		void IBinarySerializable<EventType>.Write(BinaryObjectWriter writer, EventType context)
		{
			writer.WriteObjectOffset(TextureNames, new());
		}


		bool IFileSerializable<EventType>.CheckCanReadFile(BinaryObjectReader reader, EventType context, ref FileIOInfo fileInfo)
		{
			fileInfo.Endianness ??= context.GetEndianness();
			fileInfo.OffsetOrigin ??= context.GetTextureOffsetOrigin();
			return true;
		}

		bool IFileSerializable<EventType>.CheckCanWriteFile(EventType context, ref FileIOInfo fileInfo)
		{
			fileInfo.Endianness ??= context.GetEndianness();
			fileInfo.OffsetOrigin ??= context.GetTextureOffsetOrigin();
			return true;
		}
	}
}
