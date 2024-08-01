using SA3D.Archival;
using SA3D.Common.IO;
using SA3D.Common.Lookup;
using SA3D.Modeling.Animation;
using SA3D.Modeling.ObjectData;
using SA3D.Modeling.ObjectData.Enums;
using SA3D.Modeling.Structs;
using SA3D.SA2Event.Animation;
using SA3D.Texturing.Texname;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Model data of an event.
	/// </summary>
	public class ModelData
	{
		/// <summary>
		/// The events type and target system.
		/// </summary>
		public EventType Type { get; set; }

		/// <summary>
		/// Scenes in the event. First scene is the root scene and contains only models reused in the animated scenes. 
		/// <br/> The root scene also cannot have any animations.
		/// </summary>
		public List<Scene> Scenes { get; set; }

		/// <summary>
		/// Internal texture name list.
		/// </summary>
		public TextureNameList TextureNameList { get; set; }

		/// <summary>
		/// Pixel dimensions for the events textures.
		/// </summary>
		public (short, short)[] TextureDimensions { get; set; }

		/// <summary>
		/// Reflection planes.
		/// </summary>
		public ReflectionData Reflections { get; set; }

		/// <summary>
		/// Models using the blare effects. Always [64].
		/// </summary>
		public Node?[] BlareModels { get; } = new Node[64];

		/// <summary>
		/// Upgrades integrated into character models that are just hidden based on upgrade state. Always [31,3].
		/// <br/> [x,0] and [x,1] are upgrades, while [x,2] is the default.
		/// </summary>
		public Node?[,] IntegratedUpgrades { get; } = new Node[31, 3];

		/// <summary>
		/// Node reference to the root node of Tails tails. Used for procedural vertex animation.
		/// </summary>
		public Node? TailsTails { get; set; }

		/// <summary>
		/// Overlay upgrades. Always [18].
		/// </summary>
		public OverlayUpgrade[] OverlayUpgrades { get; set; } = new OverlayUpgrade[18];

		/// <summary>
		/// Surface animations
		/// </summary>
		public SurfaceAnimationData? SurfaceAnimations { get; set; }

		/// <summary>
		/// Enables shadow casting in the event.
		/// </summary>
		public bool EnableDropShadows { get; set; }

		/// <summary>
		/// Creates a new empty event.
		/// </summary>
		/// <param name="type">The events type and target system.</param>
		public ModelData(EventType type)
		{
			Type = type;
			Scenes = new();
			Reflections = new();
			TextureNameList = new("TEXLIST", new LabeledArray<TextureName>("TEXNAMEARRAY", 0));
			TextureDimensions = Array.Empty<(short, short)>();
		}

		/// <summary>
		/// Creates a new event from existing data.
		/// </summary>
		/// <param name="type">The events type and target system.</param>
		/// <param name="scenes">Scenes in the event</param>
		/// <param name="reflections">Reflection planes.</param>
		/// <param name="textureNameList">Internal texturename list.</param>
		/// <param name="textureDimensions">Pixel dimensions for the events textures.</param>
		public ModelData(
			EventType type,
			List<Scene> scenes,
			ReflectionData reflections,
			TextureNameList textureNameList,
			(short, short)[] textureDimensions)
		{
			Type = type;
			Scenes = scenes;
			Reflections = reflections;
			TextureNameList = textureNameList;
			TextureDimensions = textureDimensions;
		}


		/// <summary>
		/// Returns all models that are part of the event.
		/// </summary>
		/// <param name="includeOverlayUpgrades">Whether to include overlay upgrades too.</param>
		/// <returns>The models.</returns>
		public HashSet<Node> GetModels(bool includeOverlayUpgrades)
		{
			HashSet<Node> nodes = new();

			void AddModel(Node? node)
			{
				if(node != null)
				{
					nodes.Add(node.GetRootNode());
				}
			}

			foreach(Scene scene in Scenes)
			{
				foreach(EventEntry entity in scene.Entries)
				{
					AddModel(entity.Model);
					AddModel(entity.GCModel);
					AddModel(entity.ShadowModel);
				}

				if(scene.BigTheCat != null)
				{
					AddModel(scene.BigTheCat.Model);
				}
			}

			if(includeOverlayUpgrades)
			{
				foreach(OverlayUpgrade upgrade in OverlayUpgrades)
				{
					AddModel(upgrade.Model1);
					AddModel(upgrade.Model2);
				}

			}

			return nodes;
		}

		/// <summary>
		/// Returns all models that are part of the event and not animated.
		/// </summary>
		/// <param name="includeOverlayUpgrades">Whether to include overlay upgrades too.</param>
		/// <returns>The models.</returns>
		public HashSet<Node> GetNonAnimatedModels(bool includeOverlayUpgrades)
		{
			HashSet<Node> nodes = GetModels(includeOverlayUpgrades);

			void RemoveModel(Node? node, Motion? motion)
			{
				if(node != null && node.Parent == null && motion != null)
				{
					nodes.Remove(node);
				}
			}

			foreach(Scene scene in Scenes)
			{
				foreach(EventEntry entity in scene.Entries)
				{
					RemoveModel(entity.Model, entity.Animation);
					RemoveModel(entity.GCModel, entity.Animation);
					RemoveModel(entity.ShadowModel, entity.Animation);
				}

				if(scene.BigTheCat != null
					&& scene.BigTheCat.Model != null
					&& scene.BigTheCat.Motions.Any(x => x.shapeAnimation != null || x.nodeAnimation != null))
				{
					nodes.Remove(scene.BigTheCat.Model);
				}
			}

			return nodes;
		}

		/// <summary>
		/// Returns an array of all (event) motions in the event.
		/// </summary>
		/// <returns>The event motions.</returns>
		public EventMotion[] GetEventMotions()
		{
			HashSet<EventMotion> result = new()
			{
				new(null, null)
			};

			void AddMotion(Motion? motion)
			{
				if(motion == null)
				{
					return;
				}

				result.Add(new(motion, null));
			}

			foreach(Scene scene in Scenes)
			{
				foreach(EventEntry entity in scene.Entries)
				{
					AddMotion(entity.Animation);
					AddMotion(entity.ShapeAnimation);
				}

				if(scene.BigTheCat != null)
				{
					foreach((Motion? a, Motion? b) in scene.BigTheCat.Motions)
					{
						AddMotion(a);
						AddMotion(b);
					}
				}

				foreach(Motion? particleMotion in scene.ParticleMotions)
				{
					AddMotion(particleMotion);
				}

				foreach(EventMotion eventMotion in scene.CameraAnimations)
				{
					result.Add(eventMotion);
				}
			}

			return result.ToArray();
		}


		/// <summary>
		/// Evaluates the file type by checking specific bytes in an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <returns>The event type.</returns>
		public static EventType EvaluateEventType(EndianStackReader reader)
		{
			EventType result;
			reader.PushBigEndian(false);

			if(reader[0] != 0x81)
			{
				uint upgradeAddr = reader.ReadUInt(0x20) - EventType.dc.GetMainImageBase();
				uint betaCheck = reader.ReadUInt(upgradeAddr + 0x134);

				result = betaCheck is < 0xC600000 and not 0
					? EventType.dcbeta
					: EventType.dc;
			}
			else
			{
				result = reader.ReadUInt(0x28) is not 0 and not 0x01000000
					? EventType.dcgc
					: EventType.gc;
			}

			reader.PopEndian();
			return result;
		}

		/// <summary>
		/// Reads model data off endian stack readers.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="motionReader">Motion data reader to read from. Used with <see cref="EventType.gc"/>.</param>
		/// <param name="lut">Pointer references to utilize.</param>
		/// <returns>The model data that was read.</returns>
		/// <exception cref="NullReferenceException"></exception>
		public static ModelData Read(EndianStackReader reader, EndianStackReader? motionReader, PointerLUT? lut = null)
		{
			EventType type = EvaluateEventType(reader);

			reader.ImageBase = type.GetMainImageBase();
			reader.PushBigEndian(type.GetBigEndian());

			lut ??= new();

			// scenes
			Scene[] scenes = new Scene[reader.ReadInt(8) + 1];
			uint sceneAddr = reader.ReadPointer(0);

			try
			{
				if(type == EventType.gc)
				{
					if(motionReader == null)
					{
						throw new NullReferenceException("Motion data is required!");
					}

					uint motionAddress = 0;
					EventMotion[] motions = EventMotion.ReadMotions(motionReader, ref motionAddress, new());

					for(int i = 0; i < scenes.Length; i++)
					{
						scenes[i] = Scene.ReadGC(reader, sceneAddr, motions, lut);
						sceneAddr += Scene.StructSize;
					}
				}
				else
				{
					for(int i = 0; i < scenes.Length; i++)
					{
						scenes[i] = Scene.ReadDC(reader, sceneAddr, lut);
						sceneAddr += Scene.StructSize;
					}
				}

				// texture list stuff
				TextureNameList textureNameList = TextureNameList.Read(reader, reader.ReadPointer(4), lut.Labels);

				uint texdimAddr = reader.ReadPointer(0xC);
				(short, short)[] textureDimensions = new (short, short)[textureNameList.TextureNames.Length];
				for(int i = 0; i < textureDimensions.Length; i++)
				{
					short x = reader.ReadShort(texdimAddr);
					short y = reader.ReadShort(texdimAddr + 2);
					textureDimensions[i] = (x, y);
					texdimAddr += 4;
				}

				ReflectionData reflectionControl = ReflectionData.Read(reader, reader.ReadPointer(0x10));

				ModelData result = new(type, new(scenes), reflectionControl, textureNameList, textureDimensions);


				uint blareAddr = reader.ReadPointer(0x14);
				for(int i = 0; i < result.BlareModels.Length; i++)
				{
					if(reader.TryReadPointer(blareAddr, out uint nodeAddr))
					{
						result.BlareModels[i] = lut.Nodes.GetValue(nodeAddr);
					}

					blareAddr += 4;
				}

				uint overrideUpgradeAddr = reader.ReadPointer(0x18);
				for(int i = 0; i < result.IntegratedUpgrades.GetLength(1); i++)
				{
					for(int j = 0; j < result.IntegratedUpgrades.GetLength(0); j++)
					{
						if(reader.TryReadPointer(overrideUpgradeAddr, out uint nodeAddr))
						{
							result.IntegratedUpgrades[j, i] = lut.Nodes.GetValue(nodeAddr);
						}

						overrideUpgradeAddr += 4;
					}
				}

				if(reader.TryReadPointer(reader.ReadPointer(0x1C), out uint tailsTailsAddr))
				{
					result.TailsTails = lut.Nodes.GetValue(tailsTailsAddr);
				}

				uint upgradeAddr = reader.ReadPointer(0x20);
				int upgradeCount = type switch
				{
					EventType.dcbeta => 14,
					EventType.gc => 18,
					EventType.dc
					or EventType.dcgc
					or _ => 16,
				};

				for(int i = 0; i < upgradeCount; i++)
				{
					result.OverlayUpgrades[i] = OverlayUpgrade.Read(reader, upgradeAddr, lut);
					upgradeAddr += OverlayUpgrade.StructSize;
				}

				if(type != EventType.dcbeta && reader.TryReadPointer(0x24, out uint uvAnimAddr))
				{
					result.SurfaceAnimations = SurfaceAnimationData.Read(reader, uvAnimAddr, type != EventType.dc, lut);
				}

				if(type == EventType.gc)
				{
					result.EnableDropShadows = reader.ReadUInt(0x28) > 0;
				}

				return result;
			}
			finally
			{
				reader.PopEndian();
			}
		}

		/// <summary>
		/// Reads model data off event source data.
		/// </summary>
		/// <param name="source">The event source data to read.</param>
		/// <returns>The model data that was read.</returns>
		public static ModelData Read(EventSource source)
		{
			using(EndianStackReader reader = new(source.Model))
			using(EndianStackReader? motionData = source.Motion != null ? new EndianStackReader(source.Motion, bigEndian: true) : null)
			{
				return Read(reader, motionData);
			}
		}


		#region Writing

		/// <summary>
		/// Writes the model data to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <param name="motions">The resulting event motion array. For dc types, it gets written to the same file, otherwise it needs to be stored in an e####_motion.bin file.</param>
		public void Write(EndianStackWriter writer, out EventMotion[] motions)
		{
			PointerLUT lut = new();

			uint start = writer.Position;
			writer.WriteEmpty(
				Type == EventType.gc
				? EnableDropShadows ? 64 : 44u
				: 40u);

			WriteModelData(writer, lut);
			motions = GetEventMotions();

			Dictionary<EventMotion, uint> motionLUT;

			if(Type != EventType.gc)
			{
				motionLUT = EventMotion.WriteMotionContents(writer, motions, lut);
			}
			else
			{
				motionLUT = new();
				uint index = 0;
				foreach(EventMotion motion in motions)
				{
					if(!motionLUT.ContainsKey(motion))
					{
						motionLUT.Add(motion, index);
						index++;
					}
				}
			}

			uint[] eventData = WriteEventData(writer, motionLUT, lut);

			uint prevPos = writer.Position;
			writer.Seek(start, SeekOrigin.Begin);

			foreach(uint value in eventData)
			{
				writer.WriteUInt(value);
			}

			writer.Seek(prevPos, SeekOrigin.Begin);
		}

		/// <summary>
		/// Writes model data to byte data.
		/// </summary>
		/// <param name="motions">The resulting event motion array. For dc types, it gets written to the same file, otherwise it needs to be stored in an e####_motion.bin file.</param>
		/// <returns>The written byte data.</returns>
		public byte[] WriteToBytes(out EventMotion[] motions)
		{
			using(MemoryStream stream = new())
			{
				EndianStackWriter writer = new(stream, Type.GetMainImageBase(), Type.GetBigEndian());
				Write(writer, out motions);
				return stream.ToArray();
			}
		}

		/// <summary>
		/// Writes the event data to model and motion files (if type of <see cref="EventType.gc"/>.
		/// </summary>
		/// <param name="filepath">Path to the file to write to.</param>
		public void WriteToFiles(string filepath)
		{
			byte[] modeldata = PRS.CompressPRS(WriteToBytes(out EventMotion[] motions));

			File.WriteAllBytes(filepath, modeldata);

			if(Type == EventType.gc)
			{
				byte[] motionData = EventMotion.WriteMotionsToBytes(motions);

				string motionFilepath = Path.Join(
					Path.GetDirectoryName(filepath),
					Path.GetFileNameWithoutExtension(filepath) + "motion.bin");

				File.WriteAllBytes(motionFilepath, motionData);
			}
		}


		private void WriteModelData(EndianStackWriter writer, PointerLUT lut)
		{
			foreach(OverlayUpgrade upgrade in OverlayUpgrades)
			{
				upgrade.WriteModels(writer, lut);
			}

			foreach(Scene scene in Scenes.Reverse<Scene>())
			{
				scene.BigTheCat?.Model?.Write(writer, ModelFormat.SA2, lut);

				foreach(EventEntry entity in scene.Entries.Reverse<EventEntry>())
				{
					if(Type != EventType.gc && entity.Model == null)
					{
						throw new NullReferenceException("Entity model is null!");
					}

					entity.Model?.Write(writer, ModelFormat.SA2, lut);

					if(Type == EventType.gc)
					{
						entity.ShadowModel?.Write(writer, ModelFormat.SA2, lut);
						entity.GCModel?.Write(writer, ModelFormat.SA2B, lut);
					}
				}
			}
		}

		private void WriteEntries(EndianStackWriter writer, Dictionary<EventMotion, uint> motionLUT, PointerLUT lut)
		{
			foreach(Scene scene in Scenes)
			{
				_ = lut.GetAddAddress(scene.Entries, (array) =>
				{
					uint result = writer.PointerPosition;

					if(scene.Entries.Count == 0)
					{
						writer.WriteEmpty(4);
					}
					else
					{
						foreach(EventEntry entity in array)
						{
							if(Type == EventType.gc)
							{
								entity.WriteGC(writer, motionLUT, lut);
							}
							else
							{
								entity.WriteDC(writer, lut);
							}
						}
					}

					return result;
				});
			}

			foreach(Scene scene in Scenes)
			{
				if(scene.BigTheCat == null)
				{
					continue;
				}

				_ = lut.GetAddAddress(scene.BigTheCat, (big) =>
				{
					uint result = writer.PointerPosition;
					scene.BigTheCat.Write(writer, lut);
					return result;
				});
			}
		}

		private uint[] WriteEventData(EndianStackWriter writer, Dictionary<EventMotion, uint> motionLUT, PointerLUT lut)
		{
			uint[] eventData = new uint[Type == EventType.gc ? 11 : 10];

			// reserve texture list
			uint texnameListAddr = writer.Position;
			writer.WriteEmpty((uint)(8 + (TextureNameList.TextureNames.Length * TextureName.StructSize)));

			eventData[3] = writer.PointerPosition;
			foreach((short x, short y) in TextureDimensions)
			{
				writer.WriteShort(x);
				writer.WriteShort(y);
			}

			Scene.WriteMotionArrays(Scenes, writer, motionLUT, lut);

			/***********************************************/

			eventData[4] = Reflections.Write(writer);

			eventData[5] = writer.PointerPosition;
			for(int i = 0; i < BlareModels.Length; i++)
			{
				writer.WriteUInt(BlareModels[i].GetAddress(lut));
			}

			eventData[6] = writer.PointerPosition;
			for(int i = 0; i < IntegratedUpgrades.GetLength(1); i++)
			{
				for(int j = 0; j < IntegratedUpgrades.GetLength(0); j++)
				{
					writer.WriteUInt(IntegratedUpgrades[j, i].GetAddress(lut));
				}
			}

			eventData[7] = writer.PointerPosition;
			uint tailsAddr = 0;
			try
			{
				tailsAddr = TailsTails.GetAddress(lut);
			}
			catch { }

			writer.WriteUInt(tailsAddr);


			eventData[8] = writer.PointerPosition;
			int upgradeCount = Type switch
			{
				EventType.dcbeta => 14,
				EventType.gc => 18,
				EventType.dc
				or EventType.dcgc
				or _ => 16,
			};

			for(int i = 0; i < upgradeCount; i++)
			{
				OverlayUpgrades[i].Write(writer, lut);
			}

			/***********************************************/

			if(SurfaceAnimations != null && Type != EventType.dcbeta)
			{
				eventData[9] = SurfaceAnimations.Write(writer, Type != EventType.dc, lut);
			}

			WriteEntries(writer, motionLUT, lut);

			eventData[0] = writer.PointerPosition;
			eventData[2] = (uint)Scenes.Count - 1;
			foreach(Scene scene in Scenes)
			{
				scene.Write(writer, lut);
			}

			if(Type == EventType.gc)
			{
				eventData[10] = EnableDropShadows ? 1u : 0;
			}

			// write texture list labels
			foreach(TextureName texName in TextureNameList.TextureNames)
			{
				if(texName.Name == null)
				{
					continue;
				}

				lut.Labels.Add(writer.PointerPosition, texName.Name);
				writer.WriteStringNullterminated(texName.Name);
			}

			uint prevPos = writer.Position;
			writer.Seek(texnameListAddr, SeekOrigin.Begin);
			eventData[1] = TextureNameList.Write(writer, lut.Labels);

			writer.Seek(prevPos, SeekOrigin.Begin);

			return eventData;
		}

		#endregion
	}
}
