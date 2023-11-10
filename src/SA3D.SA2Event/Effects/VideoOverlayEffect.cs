using SA3D.Common.IO;
using SA3D.SA2Event.Effects.Enums;
using System;

namespace SA3D.SA2Event.Effects
{
	/// <summary>
	/// Effect for playing a video over the .
	/// </summary>
	public struct VideoOverlayEffect : IFrame, IEquatable<VideoOverlayEffect>
	{
		/// <summary>
		/// Size of the structure in bytes.
		/// </summary>
		public const uint StructSize = 64;

		/// <inheritdoc/>
		public uint Frame { get; set; }

		/// <summary>
		/// Horizontal position of the overlay.
		/// </summary>
		public short PositionX { get; set; }

		/// <summary>
		/// Vertical position of the overlay.
		/// </summary>
		public short PositionY { get; set; }

		/// <summary>
		/// Z-Depth at which the overlay should be rendered.
		/// </summary>
		public float Depth { get; set; }

		/// <summary>
		/// Type of overlay.
		/// </summary>
		public VideoOverlayType Type { get; set; }

		/// <summary>
		/// Texture ID to render out to. Used for <see cref="VideoOverlayType.Mesh"/>.
		/// </summary>
		public byte TargetTextureID { get; set; }

		/// <summary>
		/// Name of the file to play.
		/// </summary>
		public string Filename { get; set; }

		/// <summary>
		/// Creates a new video overlay effect.
		/// </summary>
		/// <param name="frame">Frame at which the effect starts playing.</param>
		/// <param name="positionX">Horizontal position of the overlay.</param>
		/// <param name="positionY">Vertical position of the overlay.</param>
		/// <param name="depth">Z-Depth at which the overlay should be rendered.</param>
		/// <param name="type">Type of overlay.</param>
		/// <param name="targetTextureID">Texture ID to render out to. Used for <see cref="VideoOverlayType.Mesh"/>.</param>
		/// <param name="filename">Name of the file to play.</param>
		public VideoOverlayEffect(uint frame, short positionX, short positionY, float depth, VideoOverlayType type, byte targetTextureID, string filename)
		{
			Frame = frame;
			PositionX = positionX;
			PositionY = positionY;
			Depth = depth;
			Type = type;
			TargetTextureID = targetTextureID;
			Filename = filename;
		}

		/// <summary>
		/// Writes the video overlay to an endian stack writer.
		/// </summary>
		/// <param name="writer"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public readonly void Write(EndianStackWriter writer)
		{
			writer.WriteUInt(Frame);
			writer.WriteShort(PositionX);
			writer.WriteShort(PositionY);
			writer.WriteFloat(Depth);
			writer.WriteByte((byte)Type);
			writer.WriteByte(TargetTextureID);
			writer.WriteEmpty(2);

			string filename = Filename ?? string.Empty;

			if(filename.Length > 47)
			{
				throw new InvalidOperationException("Filename too long! must be < 48 characters long.");
			}

			writer.WriteString(filename);
			writer.WriteEmpty((uint)(48 - filename.Length));
		}

		/// <summary>
		/// Reads a video overlay effect off an endian stack reader.
		/// </summary>
		/// <param name="reader">Reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <returns></returns>
		public static VideoOverlayEffect Read(EndianStackReader reader, uint address)
		{
			return new(
				reader.ReadUInt(address),
				reader.ReadShort(address + 4),
				reader.ReadShort(address + 6),
				reader.ReadFloat(address + 8),
				(VideoOverlayType)reader[address + 0xC],
				reader[address + 0xD],
				reader.ReadNullterminatedString(address + 0x10));
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is VideoOverlayEffect overlay &&
				   Frame == overlay.Frame &&
				   PositionX == overlay.PositionX &&
				   PositionY == overlay.PositionY &&
				   Depth == overlay.Depth &&
				   Type == overlay.Type &&
				   TargetTextureID == overlay.TargetTextureID &&
				   Filename == overlay.Filename;
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(Frame, PositionX, PositionY, Depth, Type, TargetTextureID, Filename);
		}

		readonly bool IEquatable<VideoOverlayEffect>.Equals(VideoOverlayEffect other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two video overlay effects for equality.
		/// </summary>
		/// <param name="left">Lefthand video overlay effect.</param>
		/// <param name="right">Righthand video overlay effect.</param>
		/// <returns>Whether the two video overlay effects are equal</returns>
		public static bool operator ==(VideoOverlayEffect left, VideoOverlayEffect right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two video overlay effects for inequality.
		/// </summary>
		/// <param name="left">Lefthand video overlay effect.</param>
		/// <param name="right">Righthand video overlay effect.</param>
		/// <returns>Whether the two video overlay effects are inequal</returns>
		public static bool operator !=(VideoOverlayEffect left, VideoOverlayEffect right)
		{
			return !(left == right);
		}


		/// <inheritdoc/>
		public override readonly string ToString()
		{
			return Frame == 0 ? "-" : $"[{Frame}] - {Filename}";
		}

	}
}
