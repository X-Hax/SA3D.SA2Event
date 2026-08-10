using Amicitia.IO.Binary;
using System;

namespace SA3D.SA2Event.Model.AnimationData
{
	/// <summary>
	/// Texture sequence specification for looping through a specific number of textures.
	/// </summary>
	public struct TextureAnimationSequence : IEquatable<TextureAnimationSequence>, IBinarySerializable
	{
		/// <summary>
		/// Texture index to start at.
		/// </summary>
		public int TextureID { get; set; }

		/// <summary>
		/// Number of textures in the sequence.
		/// </summary>
		public int TextureCount { get; set; }


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader)
		{
			TextureID = reader.ReadInt32();
			TextureCount = reader.ReadInt32();
		}

		/// <inheritdoc/>
		public readonly void Write(BinaryObjectWriter writer)
		{
			writer.WriteInt32(TextureID);
			writer.WriteInt32(TextureCount);
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is TextureAnimationSequence sequence &&
				   TextureID == sequence.TextureID &&
				   TextureCount == sequence.TextureCount;
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(TextureID, TextureCount);
		}

		readonly bool IEquatable<TextureAnimationSequence>.Equals(TextureAnimationSequence other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two texture sequences for equality.
		/// </summary>
		/// <param name="left">Lefthand texture sequence.</param>
		/// <param name="right">Righthand texture sequence.</param>
		/// <returns>Whether the two texture sequences are equal</returns>
		public static bool operator ==(TextureAnimationSequence left, TextureAnimationSequence right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two texture sequences for inequality.
		/// </summary>
		/// <param name="left">Lefthand texture sequence.</param>
		/// <param name="right">Righthand texture sequence.</param>
		/// <returns>Whether the two texture sequences are inequal</returns>
		public static bool operator !=(TextureAnimationSequence left, TextureAnimationSequence right)
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
