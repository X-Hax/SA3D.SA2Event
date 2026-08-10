using Amicitia.IO.Binary;
using SA3D.Common;
using SA3D.Common.IO;
using SA3D.Common.Lookup;
using SA3D.Modeling.Mesh.Chunk;
using SA3D.Modeling.Mesh.Chunk.PolyChunks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SA3D.SA2Event.Model.AnimationData
{
	/// <summary>
	/// A single surface animation frame
	/// </summary>
	public class ChunkTextureAnimationFrame : ILabel, IBinarySerializable<EventModelIOContext>
	{
		/// <summary>
		/// Label prefix for <see cref="TexcoordFrames"/>
		/// </summary>
		public const string TexcoordFramesLabelPrefix = "TexCoordFrames_";

		/// <inheritdoc/>
		public string LabelPrefix => "ChunkTextureAnimationFrame_";

		/// <inheritdoc/>
		public string Label { get; set; }


		/// <summary>
		/// Initial texture ID used. Determines which texture sequence to use.
		/// </summary>
		public int TextureID { get; set; }

		/// <summary>
		/// Texture chunk that gets animated by texture sequences.
		/// </summary>
		public TextureChunk? TextureChunk { get; set; }

		/// <summary>
		/// Stripchunk of which the texture coordinates were animated.
		/// </summary>
		public StripChunk? StripChunk { get; set; }

		/// <summary>
		/// Texture coordinate frames. Key is the strip index.
		/// </summary>
		public LabeledArray<TexCoordFrame>? TexcoordFrames { get; set; }


		/// <summary>
		/// Creates a new, empty frame
		/// </summary>
		public ChunkTextureAnimationFrame()
		{
			Label = LabelPrefix.GenerateIdentifier();
		}


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader, EventModelIOContext context)
		{
			TextureID = reader.ReadInt32();
			TextureChunk = (TextureChunk?)context.OffsetLUT.PolyChunks.GetValue(reader.ReadOffsetValue() - 2);

			int texCoordFramesLength = reader.ReadInt32();
			long uvDataOffset = reader.ReadOffsetValue();
			if(texCoordFramesLength == 0 || uvDataOffset == reader.OffsetHandler.NullOffset)
			{
				return;
			}

			long stripChunkOffset;
			using(reader.AtOffset(uvDataOffset))
			{
				Dictionary<long, PolyChunk> dict = context.OffsetLUT.PolyChunks.GetDictFrom();
				IEnumerable<long> offetsBefore = dict.Keys.Where(x => x < uvDataOffset);
				if(!offetsBefore.Any())
				{
					throw new InvalidOperationException("A surface animation references an either invalid or yet-to-be-read strip chunk!");
				}

				stripChunkOffset = offetsBefore.Max();
				StripChunk = dict[stripChunkOffset] is StripChunk stripChunk
					? stripChunk
					: throw new InvalidOperationException("A surface animation references an either invalid or yet-to-be-read strip chunk!");
			}

			TexCoordFrameIOContext texcoordFrameContext = TexCoordFrameIOContext.CalculateFromChunk(StripChunk, stripChunkOffset);
			TexcoordFrames = reader.ReadLabeledObjectArrayAtOffset<TexCoordFrame, TexCoordFrameIOContext>(uvDataOffset, texCoordFramesLength, TexcoordFramesLabelPrefix, texcoordFrameContext, context.OffsetLUT)
				?? throw reader.ReadNullReference(nameof(ChunkTextureAnimationFrame), nameof(TexcoordFrames), uvDataOffset);

		}

		/// <inheritdoc/>
		public void Write(BinaryObjectWriter writer, EventModelIOContext context)
		{
			writer.WriteInt32(TextureID);

			if(TextureChunk == null)
			{
				writer.WriteOffsetValue(writer.OffsetHandler.NullOffset);
			}
			else if(!context.OffsetLUT.PolyChunks.TryGetOffset(TextureChunk, out long textureChunkOffset))
			{
				throw new InvalidOperationException("Referenced texture chunk has not been written!");
			}
			else
			{
				writer.WriteOffsetValue(textureChunkOffset + 2);
			}

			if(TexcoordFrames == null)
			{
				writer.WriteInt32(0);
				writer.WriteOffsetValue(writer.OffsetHandler.NullOffset);
			}
			else if(StripChunk == null)
			{
				throw new InvalidOperationException("Cannot write texcoord frames without strip chunk!");
			}
			else if(!context.OffsetLUT.PolyChunks.TryGetOffset(StripChunk, out long stripChunkOffset))
			{
				throw new InvalidOperationException("Referenced strip chunk has not been written!");
			}
			else
			{
				TexCoordFrameIOContext texcoordFrameContext = TexCoordFrameIOContext.CalculateFromChunk(StripChunk, stripChunkOffset);
				writer.WriteObjectArray(TexcoordFrames, texcoordFrameContext, context.OffsetLUT);
			}
		}
	}
}
