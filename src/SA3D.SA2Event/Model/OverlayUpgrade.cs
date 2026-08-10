using Amicitia.IO.Binary;
using SA3D.Common.IO;
using SA3D.Modeling.ObjectData;
using SA3D.Modeling.Structs;
using System;
using System.Collections.Generic;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Upgrade model information. Renders a specific node on top of a target node.
	/// </summary>
	public struct OverlayUpgrade : IEquatable<OverlayUpgrade>, IBinarySerializable<EventModelIOContext>
	{
		/// <summary>
		/// Upgrade structure index to actual upgrade index.
		/// </summary>
		public static readonly int[] UpgradeEventLUT =
		[
			0, // sonic light shoes
            3, // sonic flame ring
            4, // sonic bounce bracelet
            2, // sonic magic gloves
            16, // shadow air shoes
            18, // shadow flame ring
            10, // knuckles shovel claw
            10, // knuckles shovel claw
            12, // knuckles hammer gloves
            12, // knuckles hammer gloves
            11, // knuckles sunglasses
            13, // knuckles air necklace
            25, // rouge picknails
            26, // rouge treasurescope
            27, // rouge iron boots
            -2, // unused
            -2, // unused
            -2, // unused
        ];

		/// <summary>
		/// Root node of the target nodes.
		/// </summary>
		public Node? Root { get; set; }

		/// <summary>
		/// First node at which a model should be rendered.
		/// </summary>
		public Node? Target1 { get; set; }

		/// <summary>
		/// First model that should be rendered at the (first) target.
		/// </summary>
		public Node? Model1 { get; set; }

		/// <summary>
		/// Second node at which a model should be rendered.
		/// </summary>
		public Node? Target2 { get; set; }

		/// <summary>
		/// Second model that should be rendered at the (Second) target.
		/// </summary>
		public Node? Model2 { get; set; }


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader, EventModelIOContext context)
		{
			IOContext modelContext = new()
			{
				MeshFormat = Format.Chunk,
				OffsetLUT = context.OffsetLUT
			};

			Root = reader.ReadObjectOffset<Node, IOContext>(modelContext, context.OffsetLUT);
			Target1 = reader.ReadObjectOffset<Node, IOContext>(modelContext, context.OffsetLUT);
			Model1 = reader.ReadObjectOffset<Node, IOContext>(modelContext, context.OffsetLUT);
			Target2 = reader.ReadObjectOffset<Node, IOContext>(modelContext, context.OffsetLUT);
			Model2 = reader.ReadObjectOffset<Node, IOContext>(modelContext, context.OffsetLUT);
		}

		/// <inheritdoc/>
		public void Write(BinaryObjectWriter writer, EventModelIOContext context)
		{
			IOContext modelContext = new()
			{
				MeshFormat = Format.Chunk,
				OffsetLUT = context.OffsetLUT
			};

			writer.WriteObjectOffset(Root, modelContext, context.OffsetLUT);
			writer.WriteObjectOffset(Target1, modelContext, context.OffsetLUT);
			writer.WriteObjectOffset(Model1, modelContext, context.OffsetLUT);
			writer.WriteObjectOffset(Target2, modelContext, context.OffsetLUT);
			writer.WriteObjectOffset(Model2, modelContext, context.OffsetLUT);
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is OverlayUpgrade upgrade &&
				   EqualityComparer<Node?>.Default.Equals(Root, upgrade.Root) &&
				   EqualityComparer<Node?>.Default.Equals(Target1, upgrade.Target1) &&
				   EqualityComparer<Node?>.Default.Equals(Model1, upgrade.Model1) &&
				   EqualityComparer<Node?>.Default.Equals(Target2, upgrade.Target2) &&
				   EqualityComparer<Node?>.Default.Equals(Model2, upgrade.Model2);
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(Root, Target1, Model1, Target2, Model2);
		}

		readonly bool IEquatable<OverlayUpgrade>.Equals(OverlayUpgrade other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two object overlay upgrades for equality.
		/// </summary>
		/// <param name="left">Lefthand object lighting.</param>
		/// <param name="right">Righthand object lighting.</param>
		/// <returns>Whether the two object overlay upgrades are equal</returns>
		public static bool operator ==(OverlayUpgrade left, OverlayUpgrade right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two object overlay upgrades for inequality.
		/// </summary>
		/// <param name="left">Lefthand object lighting.</param>
		/// <param name="right">Righthand object lighting.</param>
		/// <returns>Whether the two object overlay upgrades are inequal</returns>
		public static bool operator !=(OverlayUpgrade left, OverlayUpgrade right)
		{
			return !(left == right);
		}


		/// <inheritdoc/>
		public override readonly string ToString()
		{
			static char GetChar(Node? node)
			{
				return node == null ? '-' : 'X';
			}

			return "" + GetChar(Root) + GetChar(Target1) + GetChar(Model1) + GetChar(Target2) + GetChar(Model2);
		}

	}
}
