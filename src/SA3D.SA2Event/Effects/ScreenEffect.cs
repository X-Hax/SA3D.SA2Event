using SA3D.Common.IO;
using SA3D.Modeling.Structs;
using SA3D.SA2Event.Effects.Enums;
using System;

namespace SA3D.SA2Event.Effects
{
	/// <summary>
	/// Effect that renders a color/texture over the screen.
	/// </summary>
	public struct ScreenEffect : IFrame, IEquatable<ScreenEffect>
	{
		/// <summary>
		/// Size of the structure in bytes.
		/// </summary>
		public const uint StructSize = 64;

		/// <inheritdoc/>
		public uint Frame { get; set; }

		/// <summary>
		/// Type of the screen effect.
		/// </summary>
		public ScreenEffectType Type { get; set; }

		/// <summary>
		/// Color of the screen effect.
		/// </summary>
		public Color Color { get; set; }

		/// <summary>
		/// Whether to fade out.
		/// </summary>
		public bool FadeOut { get; set; }

		/// <summary>
		/// ID of the event texture to render. Used with <see cref="ScreenEffectType.TextureCutIn"/> and <see cref="ScreenEffectType.TextureFadeIn"/>.
		/// </summary>
		public ushort TextureID { get; set; }

		/// <summary>
		/// How long the screen effect should last (in frames).
		/// </summary>
		public uint FrameTime { get; set; }

		/// <summary>
		/// Horizontal position.
		/// </summary>
		public short PositionX { get; set; }

		/// <summary>
		/// Vertical position.
		/// </summary>
		public short PositionY { get; set; }

		/// <summary>
		/// Width of the screen effect.
		/// </summary>
		public float Width { get; set; }

		/// <summary>
		/// Height of the screen effect.
		/// </summary>
		public float Height { get; set; }


		/// <summary>
		/// Creates a new screen effect.
		/// </summary>
		/// <param name="frame">Frame at which the effect starts playing.</param>
		/// <param name="type">Type of the screen effect.</param>
		/// <param name="color">Color of the screen effect.</param>
		/// <param name="fadeOut">Whether to fade out.</param>
		/// <param name="textureID">ID of the event texture to render. Used with <see cref="ScreenEffectType.TextureCutIn"/> and <see cref="ScreenEffectType.TextureFadeIn"/>.</param>
		/// <param name="frameTime">How long the screen effect should last (in frames).</param>
		/// <param name="positionX">Horizontal position.</param>
		/// <param name="positionY">Vertical position.</param>
		/// <param name="width">Width of the screen effect.</param>
		/// <param name="height">Height of the screen effect.</param>
		public ScreenEffect(uint frame, ScreenEffectType type, Color color, bool fadeOut, ushort textureID, uint frameTime, short positionX, short positionY, float width, float height)
		{
			Frame = frame;
			Type = type;
			Color = color;
			FadeOut = fadeOut;
			TextureID = textureID;
			FrameTime = frameTime;
			PositionX = positionX;
			PositionY = positionY;
			Width = width;
			Height = height;
		}

		/// <summary>
		/// Writes the screen effect to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		public readonly void Write(EndianStackWriter writer)
		{
			writer.WriteUInt(Frame);
			writer.WriteByte((byte)Type);
			writer.WriteEmpty(3);
			writer.WriteColor(Color, ColorIOType.ARGB8_32);
			writer.WriteByte((byte)(FadeOut ? 1 : 0));
			writer.WriteEmpty(1);
			writer.WriteUShort(TextureID);
			writer.WriteUInt(FrameTime);
			writer.WriteShort(PositionX);
			writer.WriteShort(PositionY);
			writer.WriteFloat(Width);
			writer.WriteFloat(Height);
			writer.WriteEmpty(32);
		}

		/// <summary>
		/// Reads a screen effect off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <returns></returns>
		public static ScreenEffect Read(EndianStackReader reader, uint address)
		{
			return new(
				reader.ReadUInt(address),
				(ScreenEffectType)reader[address + 4],
				reader.ReadColor(address + 8, ColorIOType.ARGB8_32),
				reader[address + 0xC] > 0,
				reader.ReadUShort(address + 0xE),
				reader.ReadUInt(address + 0x10),
				reader.ReadShort(address + 0x14),
				reader.ReadShort(address + 0x16),
				reader.ReadFloat(address + 0x18),
				reader.ReadFloat(address + 0x1C));
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is ScreenEffect effect &&
				   Frame == effect.Frame &&
				   Type == effect.Type &&
				   Color.Equals(effect.Color) &&
				   FadeOut == effect.FadeOut &&
				   TextureID == effect.TextureID &&
				   FrameTime == effect.FrameTime &&
				   PositionX == effect.PositionX &&
				   PositionY == effect.PositionY &&
				   Width == effect.Width &&
				   Height == effect.Height;
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			HashCode hash = new();
			hash.Add(Frame);
			hash.Add(Type);
			hash.Add(Color);
			hash.Add(FadeOut);
			hash.Add(TextureID);
			hash.Add(FrameTime);
			hash.Add(PositionX);
			hash.Add(PositionY);
			hash.Add(Width);
			hash.Add(Height);
			return hash.ToHashCode();
		}

		/// <inheritdoc/>
		readonly bool IEquatable<ScreenEffect>.Equals(ScreenEffect other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two screen effects for equality.
		/// </summary>
		/// <param name="left">Lefthand screen effect.</param>
		/// <param name="right">Righthand screen effect.</param>
		/// <returns>Whether the two screen effects are equal</returns>
		public static bool operator ==(ScreenEffect left, ScreenEffect right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two screen effects for inequality.
		/// </summary>
		/// <param name="left">Lefthand screen effect.</param>
		/// <param name="right">Righthand screen effect.</param>
		/// <returns>Whether the two screen effects are inequal</returns>
		public static bool operator !=(ScreenEffect left, ScreenEffect right)
		{
			return !(left == right);
		}


		/// <inheritdoc/>
		public override readonly string ToString()
		{
			return Frame == 0 ? "-" : $"[{Frame}] {Type} - {Color}";
		}
	}
}
