using Amicitia.IO.Binary;
using SA3D.Common.IO;
using SA3D.Modeling.AnimationData;
using SA3D.Modeling.Structs;
using System;

namespace SA3D.SA2Event.Model.AnimationData
{
	/// <summary>
	/// Event animation file (writing only)
	/// </summary>
	public class EventAnimationFile : IBinarySerializable<ModelOffsetLUT>, IFileSerializable
	{
		/// <summary>
		/// Animations in the animation file
		/// </summary>
		public Animation?[] Animations { get; set; } = [];


		/// <summary>
		/// Initializes a new instance of the <see cref="EventAnimationFile"/> class with specified animations
		/// </summary>
		public EventAnimationFile(Animation?[] animations)
		{
			Animations = animations;
		}


		void IBinarySerializable<ModelOffsetLUT>.Read(BinaryObjectReader reader, ModelOffsetLUT lut)
		{
			throw new InvalidOperationException("EventAnimationFile only supports writing, since reading depends on context! Please read animation data by deserializing ModelData with an appropriately setup EventModelIOContext.");
		}

		void IBinarySerializable<ModelOffsetLUT>.Write(BinaryObjectWriter writer, ModelOffsetLUT lut)
		{
			AnimationIOContext context = new()
			{
				OffsetLUT = lut
			};

			foreach(Animation? animation in Animations)
			{
				if(animation == null)
				{
					writer.WriteOffsetValue(uint.MaxValue);
				}
				else
				{
					writer.WriteObjectOffset(animation, context, lut);
				}

				writer.WriteInt32(animation?.KeyframeSets.Length ?? 0);
			}

			writer.WriteUInt64(0);
		}


		bool IFileSerializable.CheckCanReadFile(BinaryObjectReader reader, ref FileIOInfo fileInfo)
		{
			fileInfo.Endianness ??= Endianness.Big;
			return true;
		}

		void IFileSerializable.ReadFile(BinaryObjectReader fileReader, FileIOInfo fileInfo)
		{
			EventAnimationFile dst = this;
			fileReader.ReadObject(ref dst, new ModelOffsetLUT());
		}

		bool IFileSerializable.CheckCanWriteFile(ref FileIOInfo fileInfo)
		{
			fileInfo.Endianness ??= Endianness.Big;
			return true;
		}

		void IFileSerializable.WriteFile(BinaryObjectWriter fileWriter, FileIOInfo fileInfo)
		{
			fileWriter.WriteObject(this, new ModelOffsetLUT());
		}
	}
}
