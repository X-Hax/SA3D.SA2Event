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
		/// Returns the main files imagebase for the given event type.
		/// </summary>
		/// <param name="type">Type to get the image base of.</param>
		/// <returns>The imagebase.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static uint GetMainImageBase(this EventType type)
		{
			return type switch
			{
				EventType.dcbeta
				or EventType.dc => 0xC600000u,
				EventType.dcgc => 0x812FFE60u,
				EventType.gc => 0x8125FE60u,
				_ => throw new ArgumentException($"Type \"{type}\" invalid", nameof(type)),
			};
		}

		/// <summary>
		/// Returns the texture files imagebase for the given event type.
		/// </summary>
		/// <param name="type">Type to get the image base of.</param>
		/// <returns>The imagebase.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static uint GetTextureImageBase(this EventType type)
		{
			return type switch
			{
				EventType.dcbeta
				or EventType.dc => 0xCBC0000u,
				EventType.dcgc => 0x818BFE60,
				EventType.gc => 0,
				_ => throw new ArgumentException($"Type \"{type}\" invalid", nameof(type)),
			};
		}

		/// <summary>
		/// Returns the subtitle files imagebase for the 
		/// </summary>
		/// <param name="type">Type to get the image base of.</param>
		/// <returns>The imagebase.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static uint GetSubtitleImageBase(this EventType type)
		{
			return type switch
			{
				EventType.dcbeta
				or EventType.dc => 0xCBD0000u,
				EventType.dcgc
				or EventType.gc => 0x817AFE60u,
				_ => throw new ArgumentException($"Type \"{type}\" invalid", nameof(type)),
			};
		}

		/// <summary>
		/// Returns the endianness for thr given event type.
		/// </summary>
		/// <param name="type">Type to get the endiannes of</param>
		/// <returns>Whether the event type uses big endian.</returns>
		/// <exception cref="ArgumentException"></exception>
		public static bool GetBigEndian(this EventType type)
		{
			return type switch
			{
				EventType.dcbeta
				or EventType.dc => false,
				EventType.dcgc
				or EventType.gc => true,
				_ => throw new ArgumentException($"Type \"{type}\" invalid", nameof(type)),
			};
		}
	}
}
