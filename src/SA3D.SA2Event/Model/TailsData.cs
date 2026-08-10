using Amicitia.IO.Binary;
using SA3D.Common;
using SA3D.Common.IO;
using SA3D.Common.Lookup;
using SA3D.Modeling.ObjectData;
using SA3D.Modeling.Structs;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Tails related data
	/// </summary>
	public class TailsData : ILabel, IBinarySerializable<EventModelIOContext>
	{
		/// <inheritdoc/>
		public string LabelPrefix => "TailsTails_";

		/// <inheritdoc/>
		public string Label { get; set; }

		/// <summary>
		/// Node reference to the root node of Tails tails. Used for procedural vertex animation.
		/// </summary>
		public Node? Model { get; set; }


		/// <summary>
		/// Creates a new set of tails tails
		/// </summary>
		public TailsData()
		{
			Label = LabelPrefix.GenerateIdentifier();
		}


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader, EventModelIOContext context)
		{
			IOContext nodeContext = new()
			{
				MeshFormat = Format.Chunk,
				OffsetLUT = context.OffsetLUT
			};

			Model = reader.ReadObjectOffset<Node, IOContext>(nodeContext, context.OffsetLUT);
		}

		/// <inheritdoc/>
		public void Write(BinaryObjectWriter writer, EventModelIOContext context)
		{
			IOContext nodeContext = new()
			{
				MeshFormat = Format.Chunk,
				OffsetLUT = context.OffsetLUT
			};

			writer.WriteObjectOffset(Model, nodeContext, context.OffsetLUT);
		}
	}
}
