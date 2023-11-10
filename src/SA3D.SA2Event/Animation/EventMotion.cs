using SA3D.Common;
using SA3D.Common.IO;
using SA3D.Modeling.Animation;
using SA3D.Modeling.Structs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace SA3D.SA2Event.Animation
{
	/// <summary>
	/// Event motion that can contain camera and motion data.
	/// </summary>
	public struct EventMotion : IEquatable<EventMotion>
	{
		/// <summary>
		/// Animation of the event motion.
		/// </summary>
		public Motion? Animation { get; set; }

		/// <summary>
		/// Cameradata accompanying the animation.
		/// </summary>
		public CameraData? Camera { get; set; }


		/// <summary>
		/// Creates a new event motion.
		/// </summary>
		/// <param name="animation">Animation of the event motion.</param>
		/// <param name="camera">Cameradata accompanying the animation.</param>
		public EventMotion(Motion? animation, CameraData? camera)
		{
			Animation = animation;
			Camera = camera;
		}


		/// <summary>
		/// Creates a new event motion from a motion. Creates camera data if the motion is a camera motion.
		/// </summary>
		/// <param name="motion">The motion to create the event motion from.</param>
		/// <returns>The created event motion.</returns>
		public static EventMotion CreateFromMotion(Motion motion)
		{
			CameraData? camera = null;
			if(motion.IsCameraMotion)
			{
				Frame frame = motion.Keyframes[0].GetFrameAt(0);

				Vector3 dirY = frame.Target ?? Vector3.UnitY;
				Vector3 tangent = MathHelper.RotateNormal(dirY, Vector3.UnitZ, frame.Angle ?? 0);
				Vector3 dirX = Vector3.Normalize(Vector3.Cross(dirY, tangent));
				Vector3 dirZ = Vector3.Normalize(Vector3.Cross(dirY, dirX));

				camera = new(
					frame.Position ?? default,
					frame.Roll ?? 0,
					frame.Angle ?? MathF.Tau / 4,
					1,
					100000,
					dirX,
					dirY,
					dirZ);
			}

			return new(motion, camera);
		}


		/// <summary>
		/// Reads a dreamcast events camera motion off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The read camera motion.</returns>
		/// <exception cref="InvalidDataException"></exception>
		public static EventMotion ReadDCCameraMotion(EndianStackReader reader, uint address, PointerLUT lut)
		{
			Motion? motion = null;
			CameraData? cameraData = null;

			if(reader.TryReadPointer(address, out uint motionAddr))
			{
				motion = Motion.Read(reader, motionAddr, 1, lut);

				if(reader.TryReadPointer(motionAddr + 0xC, out uint cameraAddr))
				{
					cameraData = CameraData.Read(reader, cameraAddr, lut);
					if(reader.ReadPointer(motionAddr + 0x10) != motionAddr)
					{
						throw new InvalidDataException("Camera motion pointer referencing different animation!");
					}
				}
			}

			return new(motion, cameraData);
		}

		/// <summary>
		/// Reads an event motion off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The event motion that was read.</returns>
		/// <exception cref="InvalidDataException"></exception>
		public static EventMotion Read(EndianStackReader reader, uint address, PointerLUT lut)
		{
			Motion? motion = null;
			CameraData? cameraData = null;

			if(reader.ReadUInt(address) != uint.MaxValue)
			{
				uint motionAddr = reader.ReadPointer(address);
				uint modelCount = reader.ReadUInt(address + 4);
				motion = Motion.Read(reader, motionAddr, modelCount, lut);

				uint typeCheck = reader.ReadUInt(motionAddr + 8);
				if(modelCount == 1 && typeCheck == 0x01C10004)
				{
					cameraData = CameraData.Read(reader, reader.ReadPointer(motionAddr + 0xC), lut);
					if(reader.ReadPointer(motionAddr + 0x10) != motionAddr)
					{
						throw new InvalidDataException("Camera motion pointer referencing different animation!");
					}
				}
			}

			return new(motion, cameraData);
		}

		/// <summary>
		/// Reads motion data off an endian stack reader. Advances address to the end of the array.
		/// </summary>
		/// <param name="reader">The reader to read from</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The event motions that were read.</returns>
		public static EventMotion[] ReadMotions(EndianStackReader reader, ref uint address, PointerLUT lut)
		{
			List<EventMotion> motions = new();

			while(reader.ReadULong(address) != 0)
			{
				motions.Add(Read(reader, address, lut));
				address += 8;
			}

			address += 8;

			return motions.ToArray();
		}


		/// <summary>
		/// Writes an array of event motion data to an endian stack writer and returns the address for each event motion.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="motions">The event motions to write.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>Addresses for each event motion.</returns>
		public static Dictionary<EventMotion, uint> WriteMotionContents(EndianStackWriter writer, EventMotion[] motions, PointerLUT lut)
		{
			byte[] motionHead = new byte[Motion.StructSize];

			foreach(EventMotion motion in motions)
			{
				if(motion.Animation == null)
				{
					continue;
				}

				motion.Camera?.Write(writer, lut);
			}

			Dictionary<EventMotion, uint> result = new();

			foreach(EventMotion eventMotion in motions)
			{
				if(eventMotion.Animation == null || result.ContainsKey(eventMotion))
				{
					continue;
				}

				uint eventMotionAddress;

				if(lut.Motions.TryGetAddress(eventMotion.Animation, out uint motionAddress))
				{
					motionAddress -= writer.ImageBase;
					uint prevPos = writer.Position;
					writer.Seek(motionAddress, SeekOrigin.Begin);
					writer.Stream.Read(motionHead, 0, motionHead.Length);
					writer.Seek(prevPos, SeekOrigin.Begin);

					eventMotionAddress = writer.PointerPosition;
					writer.Write(motionHead);
				}
				else
				{
					eventMotionAddress = eventMotion.Animation.Write(writer, lut);
				}

				if(eventMotion.Camera != null)
				{
					uint cameraAddress = lut.All.GetAddress(eventMotion.Camera)!.Value;
					writer.WriteUInt(cameraAddress);
					writer.WriteUInt(eventMotionAddress);
				}

				result.Add(eventMotion, eventMotionAddress);
			}

			return result;
		}


		/// <summary>
		/// Writes an array of event motions to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="motions">The event motions to write.</param>
		public static void WriteMotions(EndianStackWriter writer, EventMotion[] motions)
		{
			uint startPos = writer.Position;
			writer.WriteEmpty((uint)(motions.Length + 1) * 8);

			PointerLUT lut = new();
			Dictionary<EventMotion, uint> pointers = WriteMotionContents(writer, motions, lut);

			writer.Seek(startPos, SeekOrigin.Begin);

			foreach(EventMotion motion in motions)
			{
				if(!pointers.TryGetValue(motion, out uint pointer))
				{
					pointer = uint.MaxValue;
				}

				writer.WriteUInt(pointer);
				writer.WriteUInt(motion.Animation?.ModelCount ?? 0u);
			}

			writer.SeekEnd();
		}

		/// <summary>
		/// Writes an array of event motions to byte data.
		/// </summary>
		/// <param name="motions">The event motions to write.</param>
		/// <returns>The written byte data.</returns>
		public static byte[] WriteMotionsToData(EventMotion[] motions)
		{
			using(MemoryStream stream = new())
			{
				EndianStackWriter writer = new(stream, bigEndian: true);
				WriteMotions(writer, motions);
				return stream.ToArray();
			}
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is EventMotion motion &&
				   motion.Animation == Animation &&
				   motion.Camera == Camera;
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(Animation, Camera);
		}

		readonly bool IEquatable<EventMotion>.Equals(EventMotion other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two event motions for equality.
		/// </summary>
		/// <param name="left">Lefthand event motion.</param>
		/// <param name="right">Righthand event motion.</param>
		/// <returns>Whether the two event motions are equal</returns>
		public static bool operator ==(EventMotion left, EventMotion right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two event motions for inequality.
		/// </summary>
		/// <param name="left">Lefthand event motion.</param>
		/// <param name="right">Righthand event motion.</param>
		/// <returns>Whether the two event motions are inequal</returns>
		public static bool operator !=(EventMotion left, EventMotion right)
		{
			return !(left == right);
		}


		/// <inheritdoc/>
		public override readonly string ToString()
		{
			return $"{(Animation == null ? '-' : 'X')}, {(Camera == null ? '-' : 'X')}";
		}

	}
}
