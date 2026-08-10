using Amicitia.IO.Binary;
using SA3D.Modeling.Structs;
using SA3D.SA2Event.Effects.Enums;
using System;

namespace SA3D.SA2Event.Effects
{
	/// <summary>
	/// Effect that renders a color/texture over the screen.
	/// </summary>
	public struct ScreenEffect : IFrame, IEquatable<ScreenEffect>, IBinarySerializable
	{
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


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader)
		{
			Frame = reader.ReadUInt32();
			Type = (ScreenEffectType)reader.ReadByte();
			reader.Skip(3);
			Color= reader.ReadObject<Color, ColorIOType>(ColorIOType.ARGB8_32);
			FadeOut = reader.ReadByte() != 0;
			reader.Skip(1);
			TextureID = reader.ReadUInt16();
			FrameTime = reader.ReadUInt32();
			PositionX = reader.ReadInt16();
			PositionY = reader.ReadInt16();
			Width = reader.ReadSingle();
			Height = reader.ReadSingle();
			reader.Skip(32);
		}

		/// <inheritdoc/>
		public readonly void Write(BinaryObjectWriter writer)
		{
			writer.WriteUInt32(Frame);
			writer.WriteByte((byte)Type);
			writer.Skip(3);
			writer.WriteObject(Color, ColorIOType.ARGB8_32);
			writer.WriteByte((byte)(FadeOut ? 1 : 0));
			writer.Skip(1);
			writer.WriteUInt16(TextureID);
			writer.WriteUInt32(FrameTime);
			writer.WriteInt16(PositionX);
			writer.WriteInt16(PositionY);
			writer.WriteSingle(Width);
			writer.WriteSingle(Height);
			writer.Skip(32);
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
