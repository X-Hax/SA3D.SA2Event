using Amicitia.IO.Binary;
using SA3D.SA2Event.Effects.Enums;
using System;

namespace SA3D.SA2Event.Effects
{
	/// <summary>
	/// Effect for playing a video over the .
	/// </summary>
	public struct VideoOverlayEffect : IFrame, IEquatable<VideoOverlayEffect>, IBinarySerializable
	{
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


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader)
		{
			Frame = reader.ReadUInt32();
			PositionX = reader.ReadInt16();
			PositionY = reader.ReadInt16();
			Depth = reader.ReadSingle();
			Type = (VideoOverlayType)reader.ReadByte();
			TargetTextureID = reader.ReadByte();
			reader.Skip(2);
			Filename = reader.ReadString(StringBinaryFormat.FixedLength, 48);
		}

		/// <inheritdoc/>
		public readonly void Write(BinaryObjectWriter writer)
		{
			writer.WriteUInt32(Frame);
			writer.WriteInt16(PositionX);
			writer.WriteInt16(PositionY);
			writer.WriteSingle(Depth);
			writer.WriteByte((byte)Type);
			writer.WriteByte(TargetTextureID);
			writer.Skip(2);

			string filename = Filename ?? string.Empty;

			if(filename.Length > 48)
			{
				throw new InvalidOperationException("Filename can be at most 48 characters long!");
			}

			writer.WriteString(StringBinaryFormat.FixedLength, filename, 48);
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
