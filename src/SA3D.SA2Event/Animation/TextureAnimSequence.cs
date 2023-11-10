using SA3D.Common.IO;
using System;

namespace SA3D.SA2Event.Animation
{
	/// <summary>
	/// Texture sequence specification for looping through a specific number of textures.
	/// </summary>
	public struct TextureAnimSequence : IEquatable<TextureAnimSequence>
	{
		/// <summary>
		/// Size of the structure in bytes.
		/// </summary>
		public const uint StructSize = 8;

		/// <summary>
		/// Texture index to start at.
		/// </summary>
		public int TextureID { get; set; }

		/// <summary>
		/// Number of textures in the sequence.
		/// </summary>
		public int TextureCount { get; set; }

		/// <summary>
		/// Creates a new sequence.
		/// </summary>
		/// <param name="textureID">Texture index to start at.</param>
		/// <param name="textureCount">Number of textures in the sequence.</param>
		public TextureAnimSequence(int textureID, int textureCount)
		{
			TextureID = textureID;
			TextureCount = textureCount;
		}


		/// <summary>
		/// Writes a texture anim sequence to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		public readonly void Write(EndianStackWriter writer)
		{
			writer.WriteInt(TextureID);
			writer.WriteInt(TextureCount);
		}

		/// <summary>
		/// Reads a texture anim sequence off an endian stack reader. Advances the address by the number of bytes read.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <returns>The texture anim sequence that was read.</returns>
		public static TextureAnimSequence Read(EndianStackReader reader, ref uint address)
		{
			TextureAnimSequence result = new(
				reader.ReadInt(address),
				reader.ReadInt(address + 4));

			address += 8;
			return result;
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is TextureAnimSequence sequence &&
				   TextureID == sequence.TextureID &&
				   TextureCount == sequence.TextureCount;
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(TextureID, TextureCount);
		}

		readonly bool IEquatable<TextureAnimSequence>.Equals(TextureAnimSequence other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two texture sequences for equality.
		/// </summary>
		/// <param name="left">Lefthand texture sequence.</param>
		/// <param name="right">Righthand texture sequence.</param>
		/// <returns>Whether the two texture sequences are equal</returns>
		public static bool operator ==(TextureAnimSequence left, TextureAnimSequence right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two texture sequences for inequality.
		/// </summary>
		/// <param name="left">Lefthand texture sequence.</param>
		/// <param name="right">Righthand texture sequence.</param>
		/// <returns>Whether the two texture sequences are inequal</returns>
		public static bool operator !=(TextureAnimSequence left, TextureAnimSequence right)
		{
			return !(left == right);
		}


		/// <inheritdoc/>
		public override readonly string ToString()
		{
			return $"{TextureID} {TextureCount}";
		}
	}
}
