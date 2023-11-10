using System;
using System.Numerics;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// A single reflection plane.
	/// </summary>
	public struct Reflection : IEquatable<Reflection>
	{
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


		/// <summary>
		/// Creates a new reflection
		/// </summary>
		/// <param name="transparency">Transparency of the reflection.</param>
		/// <param name="vertex1">First world space position of the reflection plane.</param>
		/// <param name="vertex2">Second world space position of the reflection plane.</param>
		/// <param name="vertex3">Third world space position of the reflection plane.</param>
		/// <param name="vertex4">Fourth world space position of the reflection plane.</param>
		public Reflection(int transparency, Vector3 vertex1, Vector3 vertex2, Vector3 vertex3, Vector3 vertex4)
		{
			Transparency = transparency;
			Vertex1 = vertex1;
			Vertex2 = vertex2;
			Vertex3 = vertex3;
			Vertex4 = vertex4;
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
