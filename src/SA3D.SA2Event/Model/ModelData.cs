using Amicitia.IO.Binary;
using SA3D.Common;
using SA3D.Common.IO;
using SA3D.Common.Lookup;
using SA3D.Modeling.ObjectData;
using SA3D.Modeling.Structs;
using SA3D.Modeling.TexName;
using SA3D.SA2Event.Model.AnimationData;
using System;
using System.Data;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Model data of an event.
	/// </summary>
	public class ModelData : IFileSerializable<EventModelIOContext>
	{
		/// <summary>
		/// Label prefix for <see cref="Scenes"/>
		/// </summary>
		public const string ScenesLabelPrefix = "Scenes_";

		/// <summary>
		/// Label prefix for <see cref="TextureNameList"/>
		/// </summary>
		public const string TextureNameListLabelPrefix = "EventTextureList_";

		/// <summary>
		/// Label prefix for <see cref="TextureNameList.TextureNames"/>
		/// </summary>
		public const string TextureNameListArrayLabelPrefix = "EventTextureNames_";

		/// <summary>
		/// Label prefix for <see cref="TextureDimensions"/>
		/// </summary>
		public const string TextureDimensionsLabelPrefix = "TextureDimensions_";

		/// <summary>
		/// Label prefix for <see cref="BlareModels"/>
		/// </summary>
		public const string BlareModelsLabelPrefix = "BlareModels_";

		/// <summary>
		/// Label prefix for <see cref="IntegratedUpgrades"/>
		/// </summary>
		public const string IntegratedUpgradesLabelPrefix = "IntegratedUpgrades_";

		/// <summary>
		/// Label prefix for <see cref="OverlayUpgrades"/>
		/// </summary>
		public const string OverlayUpgradesLabelPrefix = "OverlayUpgrades_";


		/// <summary>
		/// The events type and target system.
		/// </summary>
		public EventType EventType { get; set; }

		/// <summary>
		/// Scenes in the event. First scene is the root scene and contains only models reused in the animated scenes. 
		/// <br/> The root scene also cannot have any animations.
		/// </summary>
		public LabeledArray<Scene> Scenes { get; set; }

		/// <summary>
		/// Internal texture name list.
		/// </summary>
		public TextureNameList TextureNameList { get; set; }

		/// <summary>
		/// Pixel dimensions for the events textures.
		/// </summary>
		public LabeledArray<(short, short)> TextureDimensions { get; set; }

		/// <summary>
		/// Reflection planes.
		/// </summary>
		public ReflectionData Reflections { get; set; }

		/// <summary>
		/// Models using the blare effects. Always [64].
		/// </summary>
		public LabeledArray<Node?> BlareModels { get; set; }

		/// <summary>
		/// Upgrades integrated into character models that are just hidden based on upgrade state. Must always be [93]
		/// </summary>
		public LabeledArray<Node?> IntegratedUpgrades { get; set; }

		/// <summary>
		/// Tails related data
		/// </summary>
		public TailsData? TailsData { get; set; }

		/// <summary>
		/// Overlay upgrades. Must always be [18].
		/// </summary>
		public LabeledArray<OverlayUpgrade> OverlayUpgrades { get; set; }

		/// <summary>
		/// Surface animations
		/// </summary>
		public SurfaceAnimationData? SurfaceAnimations { get; set; }

		/// <summary>
		/// Enables shadow casting in the event.
		/// </summary>
		public bool EnableDropShadows { get; set; }


		/// <summary>
		/// Creates new, empty event model data.
		/// </summary>
		public ModelData()
		{
			string identifier = StringExtensions.GenerateIdentifier();

			Scenes = new(ScenesLabelPrefix + identifier);

			TextureNameList = new(
				TextureNameListLabelPrefix + identifier,
				new(TextureNameListArrayLabelPrefix + identifier)
			);

			TextureDimensions = new(TextureDimensionsLabelPrefix + identifier);
			Reflections = new();
			BlareModels = new(BlareModelsLabelPrefix + identifier, 64);
			IntegratedUpgrades = new(IntegratedUpgradesLabelPrefix + identifier, 93);
			OverlayUpgrades = new(OverlayUpgradesLabelPrefix + identifier, 18);
		}


		/// <summary>
		/// Validate whether contents can be written as is
		/// </summary>
		public void ValidateContents()
		{
			if(Scenes.Length == 0)
			{
				throw new DataException("Model data has no scenes!");
			}

			if(TextureDimensions.Length != TextureNameList.TextureNames.Length)
			{
				throw new DataException("Texture name list and texture dimensions have different numbers of textures!");
			}

			if(BlareModels.Length != 64)
			{
				throw new DataException("Blare models must have a length of 64!");
			}

			if(IntegratedUpgrades.Length != 93)
			{
				throw new DataException("Integrated upgrade array must have a length of 93!");
			}

			int expectedOUCount = EventType.GetOverlayUpgradeCount();
			if(OverlayUpgrades.Length != expectedOUCount)
			{
				throw new DataException($"Integrated upgrade array must have a length of {expectedOUCount}!");
			}
		}


		void IBinarySerializable<EventModelIOContext>.Read(BinaryObjectReader reader, EventModelIOContext context)
		{
			EventType = context.EventType;

			ModelOffsetLUT lut = context.OffsetLUT;

			long scenesOffset = reader.ReadOffsetValue();

			TextureNameList = reader.ReadObjectOffset<TextureNameList, OffsetLUT>(lut, lut)
				?? throw reader.ReadNullReference(nameof(ModelData), nameof(TextureNameList));

			int sceneCount = reader.ReadInt32() + 1;
			Scenes = reader.ReadLabeledObjectArrayAtOffset<Scene, EventModelIOContext>(
				scenesOffset, sceneCount, ScenesLabelPrefix, context, lut)
				?? throw reader.ReadNullReference(nameof(ModelData), nameof(Scenes), scenesOffset);

			TextureDimensions = reader.ReadLabeledObjectArrayOffset(
				r => (r.ReadInt16(), r.ReadInt16()),
				TextureNameList.TextureNames.Length, TextureDimensionsLabelPrefix, lut)
				?? throw reader.ReadNullReference(nameof(ModelData), nameof(TextureDimensions));

			Reflections = reader.ReadObjectOffset<ReflectionData, EventModelIOContext>(context, lut)
				?? throw reader.ReadNullReference(nameof(ModelData), nameof(Reflections));

			IOContext nodeContext = new()
			{
				MeshFormat = Format.Chunk,
				OffsetLUT = lut
			};

			BlareModels = reader.ReadLabeledObjectArrayOffset(
				r => r.ReadObjectOffset<Node, IOContext>(nodeContext, nodeContext.OffsetLUT),
				64, BlareModelsLabelPrefix, nodeContext.OffsetLUT)
				?? throw reader.ReadNullReference(nameof(ModelData), nameof(BlareModels));

			IntegratedUpgrades = reader.ReadLabeledObjectArrayOffset(
				r => r.ReadObjectOffset<Node, IOContext>(nodeContext, nodeContext.OffsetLUT),
				93, IntegratedUpgradesLabelPrefix, nodeContext.OffsetLUT)
				?? throw reader.ReadNullReference(nameof(ModelData), nameof(IntegratedUpgrades));

			TailsData = reader.ReadObjectOffset<TailsData, EventModelIOContext>(context, lut);

			OverlayUpgrades = reader.ReadLabeledObjectArrayOffset<OverlayUpgrade, EventModelIOContext>(
				EventType.GetOverlayUpgradeCount(), OverlayUpgradesLabelPrefix, context, lut)
				?? throw reader.ReadNullReference(nameof(ModelData), nameof(OverlayUpgrades));

			if(EventType != EventType.dcbeta)
			{
				SurfaceAnimations = reader.ReadObjectOffset<SurfaceAnimationData, EventModelIOContext>(context, lut);
			}

			if(EventType == EventType.gc)
			{
				EnableDropShadows = reader.ReadUInt32() > 0;
			}
		}

		void IBinarySerializable<EventModelIOContext>.Write(BinaryObjectWriter writer, EventModelIOContext context)
		{
			if(context.EventType != EventType)
			{
				throw new InvalidOperationException("Event type of model data and context do not match!");
			}

			ValidateContents();

			ModelOffsetLUT lut = context.OffsetLUT;

			writer.WriteObjectArrayOffset(Scenes, context, lut);
			writer.WriteObjectOffset<TextureNameList, OffsetLUT>(TextureNameList, lut, lut);
			writer.WriteInt32(Scenes.Length - 1);

			writer.WriteObjectArrayOffset((w, v) =>
			{
				w.WriteInt16(v.Item1);
				w.WriteInt16(v.Item2);
			}, TextureDimensions, lut);

			writer.WriteObjectOffset(Reflections, context, lut);

			IOContext nodeContext = new()
			{
				MeshFormat = Format.Chunk,
				OffsetLUT = lut
			};

			writer.WriteObjectArrayOffset(
				(w, v) => w.WriteObjectOffset(v, nodeContext, lut),
				BlareModels, lut
			);

			writer.WriteObjectArrayOffset(
				(w, v) => w.WriteObjectOffset(v, nodeContext, lut),
				IntegratedUpgrades, lut
			);

			writer.WriteObjectOffset(TailsData, context, lut);
			writer.WriteObjectArrayOffset(OverlayUpgrades, context, lut);

			if(EventType != EventType.dcbeta)
			{
				writer.WriteObjectOffset(SurfaceAnimations, context, lut);
			}

			if(EventType == EventType.gc)
			{
				writer.WriteUInt32(EnableDropShadows ? 1u : 0u);
			}
		}


		bool IFileSerializable<EventModelIOContext>.CheckCanReadFile(BinaryObjectReader reader, EventModelIOContext context, ref FileIOInfo fileInfo)
		{
			EventType contextType;
			try
			{
				contextType = context.EventType;
			}
			catch
			{
				context.EventType = EventTypeExtensions.EvaluateEventType(reader);
				contextType = context.EventType;
			}

			fileInfo.Endianness ??= contextType.GetEndianness();
			fileInfo.OffsetOrigin ??= contextType.GetMainOffsetOrigin();
			return true;
		}

		bool IFileSerializable<EventModelIOContext>.CheckCanWriteFile(EventModelIOContext context, ref FileIOInfo fileInfo)
		{
			EventType contextType;
			try
			{
				contextType = context.EventType;
			}
			catch
			{
				context.EventType = EventType;
				contextType = context.EventType;
			}

			if(contextType != EventType)
			{
				return false;
			}

			fileInfo.Endianness ??= contextType.GetEndianness();
			fileInfo.OffsetOrigin ??= contextType.GetMainOffsetOrigin();
			return true;
		}

	}
}
