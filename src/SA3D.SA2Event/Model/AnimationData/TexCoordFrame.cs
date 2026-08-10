using Amicitia.IO.Binary;
using SA3D.Common.IO;
using SA3D.Modeling.Structs;
using System;
using System.Numerics;

namespace SA3D.SA2Event.Model.AnimationData
{
	/// <summary>
	/// Frame for animated texture coordinates.
	/// </summary>
	public struct TexCoordFrame : IEquatable<TexCoordFrame>, IBinarySerializable<TexCoordFrameIOContext>
	{
		/// <summary>
		/// Index of the the affected strip in a strip chunk.
		/// </summary>
		public int StripIndex { get; set; }

		/// <summary>
		/// Index of the affected corner in a chunk strip.
		/// </summary>
		public int CornerIndex { get; set; }

		/// <summary>
		/// Texcoord to set.
		/// </summary>
		public Vector2 TextureCoordinates { get; set; }


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader, TexCoordFrameIOContext context)
		{
			long texcoordOffset = reader.ReadOffsetValue();
			(StripIndex, CornerIndex) = context.TexcoordIndexLUT[texcoordOffset];
			TextureCoordinates = reader.ReadVector2(FloatIOType.Short);
		}

		/// <inheritdoc/>
		public readonly void Write(BinaryObjectWriter writer, TexCoordFrameIOContext context)
		{
			long texcoordOffset = context.TexcoordOffsetLUT[StripIndex][CornerIndex];
			writer.WriteOffsetValue(texcoordOffset);
			writer.WriteVector2(TextureCoordinates, FloatIOType.Short);
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is TexCoordFrame frame &&
				   StripIndex == frame.StripIndex &&
				   CornerIndex == frame.CornerIndex &&
				   TextureCoordinates.Equals(frame.TextureCoordinates);
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(StripIndex, CornerIndex, TextureCoordinates);
		}

		readonly bool IEquatable<TexCoordFrame>.Equals(TexCoordFrame other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two texture coordinate frames for equality.
		/// </summary>
		/// <param name="left">Lefthand texture coordinate frame.</param>
		/// <param name="right">Righthand texture coordinate frame.</param>
		/// <returns>Whether the two texture coordinate frames are equal</returns>
		public static bool operator ==(TexCoordFrame left, TexCoordFrame right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two texture coordinate frames for inequality.
		/// </summary>
		/// <param name="left">Lefthand texture coordinate frame.</param>
		/// <param name="right">Righthand texture coordinate frame.</param>
		/// <returns>Whether the two texture coordinate frames are inequal</returns>
		public static bool operator !=(TexCoordFrame left, TexCoordFrame right)
		{
			return !(left == right);
		}


		/// <inheritdoc/>
		public override readonly string ToString()
		{
			return $"[{StripIndex}][{CornerIndex}] {TextureCoordinates.DebugString()}";
		}
	}
}
