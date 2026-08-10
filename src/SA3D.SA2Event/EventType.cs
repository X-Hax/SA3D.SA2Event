using Amicitia.IO.Binary;
using Amicitia.IO.Streams;
using SA3D.Common.IO;
using SA3D.SA2Event.Model;
using System;

namespace SA3D.SA2Event
{
	/// <summary>
	/// Target system type for SA2 events.
	/// </summary>
	public enum EventType
	{
		/// <summary>
		/// Dreamcast Beta build.
		/// </summary>
		dcbeta,

		/// <summary>
		/// Dreamcast release
		/// </summary>
		dc,

		/// <summary>
		/// Incomplete dreamcast release event in battle build.
		/// </summary>
		dcgc,

		/// <summary>
		/// Gamecube release and ports.
		/// </summary>
		gc
	}

	/// <summary>
	/// Extension methods for the <see cref="EventType"/> enum.
	/// </summary>
	public static class EventTypeExtensions
	{
		/// <summary>
		/// Evaluates the file type by checking specific bytes in an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <returns>The event type.</returns>
		public static EventType EvaluateEventType(BinaryObjectReader reader)
		{
			EventType result;

			using EndiannessToken endiannesToken = reader.WithEndian(Endianness.Little);
			using SeekToken at = reader.At();

			if(reader.ReadByte() != 0x81)
			{
				reader.Skip(0x1F);
				uint upgradeAddr = (uint)(reader.ReadUInt32() + EventType.dc.GetMainOffsetOrigin());

				reader.SeekPosition(upgradeAddr + 0x134);
				uint betaCheck = reader.ReadUInt32();

				result = betaCheck is < 0xC600000 and not 0
					? EventType.dcbeta
					: EventType.dc;
			}
			else
			{
				reader.Skip(0x27);
				result = reader.ReadUInt32() is not 0 and not 0x01000000
					? EventType.dcgc
					: EventType.gc;
			}

			return result;
		}

		/// <summary>
		/// Returns the endianness for the given event type.
		/// </summary>
		/// <param name="type">Type to get the endiannes of</param>
		/// <exception cref="ArgumentException"></exception>
		public static Endianness GetEndianness(this EventType type)
		{
			return type switch
			{
				EventType.dcbeta
				or EventType.dc => Endianness.Little,
				EventType.dcgc
				or EventType.gc => Endianness.Big,
				_ => throw new ArgumentException($"Type \"{type}\" invalid", nameof(type)),
			};
		}

		/// <summary>
		/// Returns the main files imagebase for the given event type.
		/// </summary>
		/// <param name="type">Type to get the image base of.</param>
		/// <returns>The imagebase.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static long GetMainOffsetOrigin(this EventType type)
		{
			return type switch
			{
				EventType.dcbeta
				or EventType.dc => -0xC600000,
				EventType.dcgc => -0x812FFE60,
				EventType.gc => -0x8125FE60,
				_ => throw new ArgumentException($"Type \"{type}\" invalid", nameof(type)),
			};
		}

		/// <summary>
		/// Returns the texture files imagebase for the given event type.
		/// </summary>
		/// <param name="type">Type to get the image base of.</param>
		/// <returns>The imagebase.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static long GetTextureOffsetOrigin(this EventType type)
		{
			return type switch
			{
				EventType.dcbeta
				or EventType.dc => -0xCBC0000,
				EventType.dcgc => -0x818BFE60,
				EventType.gc => 0,
				_ => throw new ArgumentException($"Type \"{type}\" invalid", nameof(type)),
			};
		}

		/// <summary>
		/// Returns the offset origin for subtitle files of the given event type.
		/// </summary>
		/// <param name="type">Type to get the image base of.</param>
		/// <returns>The imagebase.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static long GetSubtitleOffsetOrigin(this EventType type)
		{
			return type switch
			{
				EventType.dcbeta
				or EventType.dc => -0xCBD0000,
				EventType.dcgc
				or EventType.gc => -0x817AFE60,
				_ => throw new ArgumentException($"Type \"{type}\" invalid", nameof(type)),
			};
		}

		/// <summary>
		/// Returns the number of <see cref="OverlayUpgrade"/>s per event type
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static int GetOverlayUpgradeCount(this EventType type)
		{
			return type switch
			{
				EventType.dcbeta => 14,
				EventType.gc => 18,
				EventType.dc
				or EventType.dcgc
				or _ => 16,
			};
		}
	}
}
