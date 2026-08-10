using SA3D.Modeling.Mesh.Chunk;
using SA3D.Modeling.Mesh.Chunk.PolyChunks;
using SA3D.Modeling.Mesh.Chunk.Structs;
using System.Collections.Generic;

namespace SA3D.SA2Event.Model.AnimationData
{
	/// <summary>
	/// Context for reading and writing 
	/// </summary>
	public struct TexCoordFrameIOContext
	{
		/// <summary>
		/// Used for converting from texcoord offsets to strip-corner indices
		/// </summary>
		public Dictionary<long, (int strip, int corner)> TexcoordIndexLUT { get; set; }

		/// <summary>
		/// Used for converting from strip-corner indices to texcoord offsets
		/// </summary>
		public long[][] TexcoordOffsetLUT { get; set; }

		/// <summary>
		/// Calculates the context for reading and writing surface animation texture frames
		/// </summary>
		/// <param name="chunk">The chunk to calculate offsets for</param>
		/// <param name="chunkOffset">The offset to which the strip chunk was written</param>
		/// <returns></returns>
		public static TexCoordFrameIOContext CalculateFromChunk(StripChunk chunk, long chunkOffset)
		{
			// address + chunk header + texcoord offset
			long offset = chunkOffset + 6 + 2;

			uint attributeSize = 2u * (uint)chunk.TriangleAttributeCount;

			uint structSize = (uint)(2u
				+ (chunk.Type.GetStripTexCoordCount() * 4u)
				+ (chunk.Type.CheckStripHasNormals() ? 12u : 0u)
				+ (chunk.Type.CheckStripHasColors() ? 4u : 0u))
				+ attributeSize;

			TexCoordFrameIOContext result = new()
			{
				TexcoordIndexLUT = [],
				TexcoordOffsetLUT = new long[chunk.Strips.Length][]
			};

			for(int i = 0; i < chunk.Strips.Length; i++)
			{
				offset += 2; // skip strip header
				ChunkStrip strip = chunk.Strips[i];
				long[] offsetLUT = new long[strip.Corners.Length];

				for(int j = 0; j < strip.Corners.Length; j++, offset += structSize)
				{
					result.TexcoordIndexLUT.Add(offset, (i, j));
					offsetLUT[j] = offset;

					if(j < 2)
					{
						offset -= attributeSize;
					}
				}

				result.TexcoordOffsetLUT[i] = offsetLUT;
			}

			return result;
		}
	}
}
