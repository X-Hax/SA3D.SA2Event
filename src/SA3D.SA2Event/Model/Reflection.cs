using Amicitia.IO.Binary;
using SA3D.Common.IO;
using System;
using System.Numerics;
using static SA3D.SA2Event.Model.Reflection;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// A single reflection plane.
	/// </summary>
	public struct Reflection : IEquatable<Reflection>, IBinarySerializable<IOMode>
	{
		/// <summary>
		/// What part of the reflection should be read/written
		/// </summary>
		public enum IOMode
		{
			/// <summary>
			/// <see cref="Transparency"/>
			/// </summary>
			Transparency,

			/// <summary>
			/// <see cref="Vertex1"/>, <see cref="Vertex2"/>, <see cref="Vertex3"/> and <see cref="Vertex4"/>
			/// </summary>
			Vertices
		}

		/// <summary>
		/// Transparency of the reflection.
		/// </summary>
		public int Transparency { get; set; }

		/// <summary>
		/// First world space position of the reflection plane.
		/// </summary>
		public Vector3 Vertex1 { get; set; }

		/// <summary>
		/// Second world space position of the reflection plane.
		/// </summary>
		public Vector3 Vertex2 { get; set; }

		/// <summary>
		/// Third world space position of the reflection plane.
		/// </summary>
		public Vector3 Vertex3 { get; set; }

		/// <summary>
		/// Fourth world space position of the reflection plane.
		/// </summary>
		public Vector3 Vertex4 { get; set; }


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader, IOMode mode)
		{
			if(mode == IOMode.Transparency)
			{
				Transparency = reader.ReadInt32();
			}
			else
			{
				Vertex1 = reader.ReadVector3();
				Vertex2 = reader.ReadVector3();
				Vertex3 = reader.ReadVector3();
				Vertex4 = reader.ReadVector3();
			}
		}

		/// <inheritdoc/>
		public readonly void Write(BinaryObjectWriter writer, IOMode mode)
		{
			if(mode == IOMode.Transparency)
			{
				writer.WriteInt32(Transparency);
			}
			else
			{
				writer.WriteVector3(Vertex1);
				writer.WriteVector3(Vertex2);
				writer.WriteVector3(Vertex3);
				writer.WriteVector3(Vertex4);
			}
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is Reflection reflection &&
				   Transparency == reflection.Transparency &&
				   Vertex1.Equals(reflection.Vertex1) &&
				   Vertex2.Equals(reflection.Vertex2) &&
				   Vertex3.Equals(reflection.Vertex3) &&
				   Vertex4.Equals(reflection.Vertex4);
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(Transparency, Vertex1, Vertex2, Vertex3, Vertex4);
		}

		readonly bool IEquatable<Reflection>.Equals(Reflection other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two object reflections for equality.
		/// </summary>
		/// <param name="left">Lefthand object lighting.</param>
		/// <param name="right">Righthand object lighting.</param>
		/// <returns>Whether the two object reflections are equal</returns>
		public static bool operator ==(Reflection left, Reflection right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two object reflections for inequality.
		/// </summary>
		/// <param name="left">Lefthand object lighting.</param>
		/// <param name="right">Righthand object lighting.</param>
		/// <returns>Whether the two object reflections are inequal</returns>
		public static bool operator !=(Reflection left, Reflection right)
		{
			return !(left == right);
		}


		/// <inheritdoc/>
		public override readonly string ToString()
		{
			return $"{Transparency:F3}";
		}
	}
}
