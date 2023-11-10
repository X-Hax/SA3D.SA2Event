using SA3D.Modeling.Structs;
using System;
using System.Numerics;

namespace SA3D.SA2Event.Animation
{
	/// <summary>
	/// Frame for animated texture coordinates.
	/// </summary>
	public struct TexcoordFrame : IEquatable<TexcoordFrame>
	{
		/// <summary>
		/// Absolute index of the corner in a strip chunk.
		/// </summary>
		public int CornerIndex { get; set; }

		/// <summary>
		/// Texcoord to set.
		/// </summary>
		public Vector2 TexCoord { get; set; }

		/// <summary>
		/// Creates a new texcoord frame.
		/// </summary>
		/// <param name="polyIndex">Absolute index of the corner in a strip chunk.</param>
		/// <param name="texCoord">Texcoord to set.</param>
		public TexcoordFrame(int polyIndex, Vector2 texCoord)
		{
			CornerIndex = polyIndex;
			TexCoord = texCoord;
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is TexcoordFrame frame &&
				   CornerIndex == frame.CornerIndex &&
				   TexCoord.Equals(frame.TexCoord);
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(CornerIndex, TexCoord);
		}

		readonly bool IEquatable<TexcoordFrame>.Equals(TexcoordFrame other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two texture coordinate frames for equality.
		/// </summary>
		/// <param name="left">Lefthand texture coordinate frame.</param>
		/// <param name="right">Righthand texture coordinate frame.</param>
		/// <returns>Whether the two texture coordinate frames are equal</returns>
		public static bool operator ==(TexcoordFrame left, TexcoordFrame right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two texture coordinate frames for inequality.
		/// </summary>
		/// <param name="left">Lefthand texture coordinate frame.</param>
		/// <param name="right">Righthand texture coordinate frame.</param>
		/// <returns>Whether the two texture coordinate frames are inequal</returns>
		public static bool operator !=(TexcoordFrame left, TexcoordFrame right)
		{
			return !(left == right);
		}


		/// <inheritdoc/>
		public override readonly string ToString()
		{
			return $"[{CornerIndex}] {TexCoord.DebugString()}";
		}
	}
}
