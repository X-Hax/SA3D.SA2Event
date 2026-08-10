using Amicitia.IO.Binary;
using SA3D.Common;
using SA3D.Common.IO;
using SA3D.Common.Lookup;
using System;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Handles reflection planes.
	/// </summary>
	public class ReflectionData : ILabel, IBinarySerializable<EventModelIOContext>
	{
		/// <summary>
		/// Label prefix for <see cref="Reflections"/>
		/// </summary>
		public const string ReflectionsLabelPrefix = "Reflections_";

		/// <inheritdoc/>
		public string LabelPrefix => "ReflectionData_";

		/// <inheritdoc/>
		public string Label { get; set; }

		/// <summary>
		/// All reflection planes part of the data.
		/// </summary>
		public LabeledArray<Reflection> Reflections { get; set; }


		/// <summary>
		/// Creates new reflection data.
		/// </summary>
		public ReflectionData()
		{
			Label = LabelPrefix.GenerateIdentifier();
			Reflections = new(ReflectionsLabelPrefix.GenerateIdentifier(), 0);
		}


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader, EventModelIOContext context)
		{
			int reflectionCount = reader.ReadInt32();

			long transparencyOffset = reader.GetPositionOffset();
			reader.Skip(0x80);
			long reflectionsOffset = reader.ReadOffsetValue();

			if(reflectionCount == 0)
			{
				return;
			}

			Reflections = reader.ReadLabeledObjectArrayAtOffset<Reflection, Reflection.IOMode>(reflectionsOffset, reflectionCount, ReflectionsLabelPrefix, Reflection.IOMode.Vertices, context.OffsetLUT)
				?? throw reader.ReadNullReference(nameof(ReflectionData), nameof(Reflections));

			using(reader.AtOffset(transparencyOffset))
			{
				for(int i = 0; i < Reflections.Length; i++)
				{
					// TODO: verify if this actually works
					Reflections[i].Read(reader, Reflection.IOMode.Transparency);
				}
			}
		}

		/// <inheritdoc/>
		public void Write(BinaryObjectWriter writer, EventModelIOContext context)
		{
			if(Reflections.Length > 32)
			{
				throw new InvalidOperationException("Breached the reflection plane limit of 32!");
			}

			writer.WriteInt32(Reflections.Length);
			writer.WriteObjectArray(Reflections, Reflection.IOMode.Transparency);

			if(Reflections.Length < 32)
			{
				writer.Skip((32 - Reflections.Length) * sizeof(float));
			}

			writer.WriteObjectArrayOffset(Reflections, context.OffsetLUT);
		}
	}
}
