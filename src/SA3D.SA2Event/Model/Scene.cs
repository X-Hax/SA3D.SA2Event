using SA3D.Common.IO;
using SA3D.Modeling.Animation;
using SA3D.Modeling.Structs;
using SA3D.SA2Event.Animation;
using System;
using System.Collections.Generic;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Continuous scene of an event. The "cut" in a cutscene.
	/// </summary>
	public class Scene
	{
		/// <summary>
		/// Size of the structure in bytes.
		/// </summary>
		public const int StructSize = 32;

		/// <summary>
		/// Event entries rendered in the scene specifically.
		/// </summary>
		public List<EventEntry> Entries { get; }

		/// <summary>
		/// Camera animations to be played.
		/// </summary>
		public List<EventMotion> CameraAnimations { get; }

		/// <summary>
		/// Motions of particles in the scene. Motion index corresponds to particle index in effects file.
		/// </summary>
		public List<Motion?> ParticleMotions { get; }

		/// <summary>
		/// Big the cat entry.
		/// </summary>
		public BigTheCatEntry? BigTheCat { get; set; }

		/// <summary>
		/// Number of frames (at 30 fps) that the scene takes to play.
		/// </summary>
		public int FrameCount { get; set; }


		/// <summary>
		/// Creates a new scene.
		/// </summary>
		/// <param name="frameCount">Number of frames (at 30 fps) that the scene takes to play.</param>
		public Scene(int frameCount)
		{
			Entries = new();
			CameraAnimations = new();
			ParticleMotions = new();
			FrameCount = frameCount;
		}


		/// <summary>
		/// Writes an array of indices to the camera motions to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="motionLUT">LUT with reference keys for all event motion pairs.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>Pointer to where the array was written.</returns>
		public uint WriteCameraMotionArray(EndianStackWriter writer, Dictionary<EventMotion, uint> motionLUT, PointerLUT lut)
		{
			uint onWrite()
			{
				uint result = writer.PointerPosition;
				if(CameraAnimations.Count == 0)
				{
					writer.WriteEmpty(4);
				}
				else
				{
					foreach(EventMotion motion in CameraAnimations)
					{
						writer.WriteUInt(motionLUT.GetMotionKey(motion));
					}
				}

				return result;
			}

			return lut.GetAddAddress(CameraAnimations, onWrite);
		}

		/// <summary>
		/// Writes an array of indices to the particle motions to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="motionLUT">LUT with reference keys for all event motion pairs.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>Pointer to where the array was written.</returns>
		public uint WriteParticleMotionArray(EndianStackWriter writer, Dictionary<EventMotion, uint> motionLUT, PointerLUT lut)
		{
			uint onWrite()
			{
				uint result = writer.PointerPosition;

				if(ParticleMotions.Count == 0)
				{
					writer.WriteEmpty(4);
				}
				else
				{
					foreach(Motion? motion in ParticleMotions)
					{
						writer.WriteUInt(motionLUT.GetMotionKey(motion));
					}
				}

				return result;
			}

			return lut.GetAddAddress(ParticleMotions, onWrite);
		}

		/// <summary>
		/// Writes all motion index arrays of scenes to an endian stack writer.
		/// </summary>
		/// <param name="scenes">Scenes to write the motion arrays of.</param>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="motionLUT">LUT with reference keys for all event motion pairs.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		public static void WriteMotionArrays(IEnumerable<Scene> scenes, EndianStackWriter writer, Dictionary<EventMotion, uint> motionLUT, PointerLUT lut)
		{
			foreach(Scene scene in scenes)
			{
				scene.WriteCameraMotionArray(writer, motionLUT, lut);
			}

			foreach(Scene scene in scenes)
			{
				scene.WriteParticleMotionArray(writer, motionLUT, lut);
			}

			foreach(Scene scene in scenes)
			{
				scene.BigTheCat?.WriteMotionArray(writer, motionLUT, lut);
			}
		}


		/// <summary>
		/// Writes the scene structure to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <exception cref="InvalidOperationException"></exception>
		public void Write(EndianStackWriter writer, PointerLUT lut)
		{
			uint bigAddr = 0;

			if((!lut.All.TryGetAddress(Entries, out uint entityAddr))
				|| !lut.All.TryGetAddress(CameraAnimations, out uint camAnimAddr)
				|| !lut.All.TryGetAddress(ParticleMotions, out uint particleAddr)
				|| (BigTheCat != null && !lut.All.TryGetAddress(BigTheCat, out bigAddr)))
			{
				throw new InvalidOperationException("Scene Content has not yet been written!");
			}

			writer.WriteUInt(entityAddr);
			writer.WriteInt(Entries.Count);
			writer.WriteUInt(camAnimAddr);
			writer.WriteInt(CameraAnimations.Count);
			writer.WriteUInt(particleAddr);
			writer.WriteInt(ParticleMotions.Count);
			writer.WriteUInt(bigAddr);
			writer.WriteInt(FrameCount);
		}


		/// <summary>
		/// Reads a dreamcast formatted scene off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The scene that was read.</returns>
		public static Scene ReadDC(EndianStackReader reader, uint address, PointerLUT lut)
		{
			Scene result = new(reader.ReadInt(address + 0x1C));

			uint entityAddr = reader.ReadPointer(address);
			int entityCount = reader.ReadInt(address + 4);
			for(int i = 0; i < entityCount; i++)
			{
				EventEntry entity = EventEntry.ReadDC(reader, ref entityAddr, lut);
				if(entity.Model != null || entity.GCModel != null)
				{
					result.Entries.Add(entity);
				}
			}

			uint camAddr = reader.ReadPointer(address + 8);
			int camCount = reader.ReadInt(address + 0xC);
			for(int i = 0; i < camCount; i++)
			{
				EventMotion motion = EventMotion.ReadDCCameraMotion(reader, camAddr, lut);
				result.CameraAnimations.Add(motion);
				camAddr += 4;
			}

			uint particleAddr = reader.ReadPointer(address + 0x10);
			int particleCount = reader.ReadInt(address + 0x14);
			for(int i = 0; i < particleCount; i++)
			{
				Motion? motion = null;
				if(reader.TryReadPointer(particleAddr, out uint motionAddr))
				{
					motion = Motion.Read(reader, motionAddr, 1, lut);
				}

				result.ParticleMotions.Add(motion);
				particleAddr += 4;
			}

			if(reader.TryReadPointer(address + 0x18, out uint bigAddr))
			{
				result.BigTheCat = BigTheCatEntry.ReadDC(reader, bigAddr, lut);
			}

			return result;
		}

		/// <summary>
		/// Reads a gamecube formatted scene off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="motions">Event motion array to utilize. Needed for fetching motion indices.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The scene that was read.</returns>
		public static Scene ReadGC(EndianStackReader reader, uint address, EventMotion[] motions, PointerLUT lut)
		{
			Scene result = new(reader.ReadInt(address + 0x1C));

			uint entityAddr = reader.ReadPointer(address);
			int entityCount = reader.ReadInt(address + 4);
			for(int i = 0; i < entityCount; i++)
			{
				EventEntry entity = EventEntry.ReadGC(reader, ref entityAddr, motions, lut);
				if(entity.Model != null || entity.GCModel != null)
				{
					result.Entries.Add(entity);
				}
			}

			uint camAddr = reader.ReadPointer(address + 8);
			int camCount = reader.ReadInt(address + 0xC);
			for(int i = 0; i < camCount; i++)
			{
				result.CameraAnimations.Add(motions[reader.ReadInt(camAddr)]);
				camAddr += 4;
			}

			uint particleAddr = reader.ReadPointer(address + 0x10);
			int particleCount = reader.ReadInt(address + 0x14);
			for(int i = 0; i < particleCount; i++)
			{
				result.ParticleMotions.Add(motions[reader.ReadInt(particleAddr)].Animation);
				particleAddr += 4;
			}

			if(reader.TryReadPointer(address + 0x18, out uint bigAddr))
			{
				result.BigTheCat = BigTheCatEntry.ReadGC(reader, bigAddr, motions, lut);
			}

			return result;
		}

	}
}
