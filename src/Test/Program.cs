using SA3D.Modeling.Mesh.Chunk;
using SA3D.Modeling.Mesh.Chunk.PolyChunks;
using SA3D.Modeling.Mesh.Gamecube;
using SA3D.Modeling.Mesh.Gamecube.Parameters;
using SA3D.Modeling.ObjectData;
using SA3D.SA2Event;

internal class Program
{
	private static void Main()
	{
		Event data = Event.ReadFromFiles("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Sonic Adventure 2\\resource\\gd_PC\\event\\e0004");

		HashSet<Node> models = data.ModelData.GetModels(true);

		foreach(Node node in models)
		{
			if(node.GetAttachFormat() == SA3D.Modeling.Mesh.AttachFormat.GC)
			{
				foreach(GCAttach attach in node.GetTreeAttachEnumerable().Cast<GCAttach>())
				{
					foreach(GCMesh mesh in attach.OpaqueMeshes)
					{
						for(int i = 0; i < mesh.Parameters.Length; i++)
						{
							if(mesh.Parameters[i] is not GCTextureParameter param)
							{
								continue;
							}

							if(param.TextureID > 0)
							{
								param.TextureID--;
							}

							mesh.Parameters[i] = param;
						}
					}
				}
			}
			else
			{
				foreach(ChunkAttach attach in node.GetTreeAttachEnumerable().Cast<ChunkAttach>())
				{
					if(attach.PolyChunks == null)
					{
						continue;
					}

					foreach(TextureChunk texChunk in attach.PolyChunks.OfType<TextureChunk>())
					{
						if(texChunk.TextureID > 0)
						{
							texChunk.TextureID--;
						}
					}
				}
			}
		}

		data.WriteToFiles("C:\\Program Files (x86)\\Steam\\steamapps\\common\\Sonic Adventure 2\\mods\\EventTest\\gd_PC\\event\\e0004");
	}
}