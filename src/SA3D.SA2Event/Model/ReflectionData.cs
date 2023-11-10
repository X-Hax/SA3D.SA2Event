using SA3D.Common.IO;
using SA3D.Modeling.Structs;
using System;
using System.Collections.Generic;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Handles reflection planes.
	/// </summary>
	public class ReflectionData
	{
		/// <summary>
		/// Size of the structure in bytes.
		/// </summary>
		public const uint StructSize = 0x88;

		/// <summary>
		/// All reflection planes part of the data.
		/// </summary>
		public List<Reflection> Reflections { get; set; }


		/// <summary>
		/// Creates new reflection data.
		/// </summary>
		public ReflectionData()
		{
			Reflections = new();
		}


		/// <summary>
		/// Writes the reflection data to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <returns>The pointer to the written reflection data.</returns>
		public uint Write(EndianStackWriter writer)
		{
			uint count = (uint)Math.Min(Reflections.Count, 32);

			uint quadAddr = 0;
			if(count > 0)
			{
				quadAddr = writer.PointerPosition;

				foreach(Reflection reflection in Reflections)
				{
					writer.WriteVector3(reflection.Vertex1);
					writer.WriteVector3(reflection.Vertex2);
					writer.WriteVector3(reflection.Vertex3);
					writer.WriteVector3(reflection.Vertex4);
				}
			}

			uint address = writer.PointerPosition;
			writer.WriteUInt(count);
			for(int i = 0; i < count; i++)
			{
				writer.WriteInt(Reflections[i].Transparency);
			}

			writer.WriteEmpty(4 * (32 - count));
			writer.WriteUInt(quadAddr);

			return address;
		}

		/// <summary>
		/// Reads reflection data off an endian stack reader.
		/// </summary>
		/// <param name="reader">Reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <returns>The reflection data that was read.</returns>
		public static ReflectionData Read(EndianStackReader reader, uint address)
		{
			ReflectionData result = new();

			uint reflectionCount = reader.ReadUInt(address);
			if(reflectionCount == 0)
			{
				return result;
			}

			uint quadAddr = reader.ReadPointer(address + 0x84);
			uint transparencyAddr = address + 4;
			for(int i = 0; i < reflectionCount; i++)
			{
				Reflection reflection = new(
					reader.ReadInt(transparencyAddr),
					reader.ReadVector3(ref quadAddr),
					reader.ReadVector3(ref quadAddr),
					reader.ReadVector3(ref quadAddr),
					reader.ReadVector3(ref quadAddr));

				result.Reflections.Add(reflection);
				transparencyAddr += 4;
			}

			return result;
		}

	}
}
