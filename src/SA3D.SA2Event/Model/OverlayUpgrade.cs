using SA3D.Common.IO;
using SA3D.Modeling.ObjectData;
using SA3D.Modeling.ObjectData.Enums;
using SA3D.Modeling.Structs;
using System;
using System.Collections.Generic;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Upgrade model information. Renders a specific node on top of a target node.
	/// </summary>
	public struct OverlayUpgrade : IEquatable<OverlayUpgrade>
	{
		/// <summary>
		/// Size of the structure in bytes.
		/// </summary>
		public const uint StructSize = 20;

		/// <summary>
		/// Upgrade structure index to actual upgrade index.
		/// </summary>
		public static readonly int[] UpgradeEventLUT = new int[]
		{
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
        };

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


		/// <summary>
		/// Creates a new overlay upgrade.
		/// </summary>
		/// <param name="root">Root node of the target nodes.</param>
		/// <param name="target1">First node at which a model should be rendered.</param>
		/// <param name="model1">First model that should be rendered at the (first) target.</param>
		/// <param name="target2">Second node at which a model should be rendered.</param>
		/// <param name="model2">Second model that should be rendered at the (Second) target.</param>
		public OverlayUpgrade(Node? root, Node? target1, Node? model1, Node? target2, Node? model2)
		{
			Root = root;
			Target1 = target1;
			Model1 = model1;
			Target2 = target2;
			Model2 = model2;
		}


		/// <summary>
		/// Writes the models of the upgrade to an endian stack writer.
		/// </summary>
		/// <param name="writer">Writer to write to.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		public readonly void WriteModels(EndianStackWriter writer, PointerLUT lut)
		{
			Model1?.Write(writer, ModelFormat.SA2, lut);
			Model2?.Write(writer, ModelFormat.SA2, lut);
		}


		/// <summary>
		/// Writes the overlay upgrade struct to an endian stack writer.
		/// </summary>
		/// <remarks>
		/// Models needs to be written manually beforehand, optimally via <see cref="WriteModels"/>
		/// </remarks>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		public readonly void Write(EndianStackWriter writer, PointerLUT lut)
		{
			writer.WriteUInt(Root.GetAddress(lut));
			writer.WriteUInt(Target1.GetAddress(lut));
			writer.WriteUInt(Model1.GetAddress(lut));
			writer.WriteUInt(Target2.GetAddress(lut));
			writer.WriteUInt(Model2.GetAddress(lut));
		}

		/// <summary>
		/// Reads an overlay upgrade structure off an endian stack reader.
		/// </summary>
		/// <param name="reader">Reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The overlay upgrade that was read.</returns>
		public static OverlayUpgrade Read(EndianStackReader reader, uint address, PointerLUT lut)
		{
			Node? ReadNode(uint offset)
			{
				if(reader.TryReadPointer(address + offset, out uint nodeAddr))
				{
					return Node.Read(reader, nodeAddr, ModelFormat.SA2, lut);
				}

				return null;
			}

			return new(
			   ReadNode(0),
			   ReadNode(4),
			   ReadNode(8),
			   ReadNode(12),
			   ReadNode(16));
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
