using System;

namespace SA3D.SA2Event.Language
{
	/// <summary>
	/// A single subtitle text line.
	/// </summary>
	public struct SubtitleText : IEquatable<SubtitleText>
	{
		/// <summary>
		/// ID of the character that the text belongs to.
		/// </summary>
		public int Character { get; set; }

		/// <summary>
		/// Whether the text is centered on the screen (?).
		/// </summary>
		public bool Centered { get; set; }

		/// <summary>
		/// The text.
		/// </summary>
		public string Text { get; set; }

		/// <summary>
		/// Creates a new subtitle text.
		/// </summary>
		/// <param name="character">ID of the character that the text belongs to.</param>
		/// <param name="centered">Whether the text is centered on the screen (?).</param>
		/// <param name="text">The text.</param>
		public SubtitleText(int character, bool centered, string text)
		{
			Character = character;
			Centered = centered;
			Text = text;
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is SubtitleText text &&
				   Character == text.Character &&
				   Centered == text.Centered &&
				   Text == text.Text;
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(Character, Centered, Text);
		}

		readonly bool IEquatable<SubtitleText>.Equals(SubtitleText other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two subtitle texts for equality.
		/// </summary>
		/// <param name="left">Lefthand subtitle text.</param>
		/// <param name="right">Righthand subtitle text.</param>
		/// <returns>Whether the two subtitle texts are equal</returns>
		public static bool operator ==(SubtitleText left, SubtitleText right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two subtitle texts for inequality.
		/// </summary>
		/// <param name="left">Lefthand subtitle text.</param>
		/// <param name="right">Righthand subtitle text.</param>
		/// <returns>Whether the two subtitle texts are inequal</returns>
		public static bool operator !=(SubtitleText left, SubtitleText right)
		{
			return !(left == right);
		}


		/// <inheritdoc/>
		public override readonly string ToString()
		{
			return Text;
		}

	}
}
