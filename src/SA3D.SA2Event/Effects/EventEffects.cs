using Amicitia.IO.Binary;
using SA3D.Common.IO;
using SA3D.SA2Event.Language;
using System.Collections.ObjectModel;

namespace SA3D.SA2Event.Effects
{
	/// <summary>
	/// Event effect data.
	/// </summary>
	public class EventEffects : IFileSerializable<EventType>
	{
		/// <summary>
		/// Fallback language info.
		/// </summary>
		public EventLanguageTimestamps BaseLanguageTimestamps { get; set; }

		/// <summary>
		/// Screen effects.
		/// </summary>
		public ScreenEffect[] ScreenEffects { get; }

		/// <summary>
		/// Simple particle effects.
		/// </summary>
		public SimpleParticleEffect[] Particles { get; }

		/// <summary>
		/// Four sets of Object lighting.
		/// </summary>
		public ReadOnlyCollection<ObjectLighting[]> Lighting { get; }

		/// <summary>
		/// Blare effects.
		/// </summary>
		public BlareEffect[] BlareEffects { get; }

		/// <summary>
		/// Particle emitter effects.
		/// </summary>
		public ParticleEmitterEffect[] ParticleEmitterEffects { get; }

		/// <summary>
		/// Video overlay effects
		/// </summary>
		public VideoOverlayEffect[] VideoOverlayEffects { get; }


		/// <summary>
		/// Creates a new set of event effects.
		/// </summary>
		public EventEffects()
		{
			BaseLanguageTimestamps = new();
			ScreenEffects = new ScreenEffect[64];
			Particles = new SimpleParticleEffect[2048];
			Lighting = new ReadOnlyCollection<ObjectLighting[]>(
				[
					new ObjectLighting[256],
					new ObjectLighting[256],
					new ObjectLighting[256],
					new ObjectLighting[256]
				]
			);

			BlareEffects = new BlareEffect[64];
			ParticleEmitterEffects = new ParticleEmitterEffect[64];
			VideoOverlayEffects = new VideoOverlayEffect[64];
		}


		void IBinarySerializable<EventType>.Read(BinaryObjectReader reader, EventType context)
		{
			EventLanguageTimestamps timestamps = BaseLanguageTimestamps;
			reader.ReadObject(ref timestamps);

			ScreenEffects.ReadToObjectArray(reader);
			Particles.ReadToObjectArray(reader);

			foreach(ObjectLighting[] lighting in Lighting)
			{
				lighting.ReadToObjectArray(reader);
			}

			BlareEffects.ReadToObjectArray(reader);
			ParticleEmitterEffects.ReadToObjectArray(reader);
			VideoOverlayEffects.ReadToObjectArray(reader);
		}

		void IBinarySerializable<EventType>.Write(BinaryObjectWriter writer, EventType context)
		{
			writer.WriteObject(BaseLanguageTimestamps);

			writer.WriteObjectArray(ScreenEffects);
			writer.WriteObjectArray(Particles);

			foreach(ObjectLighting[] lighting in Lighting)
			{
				writer.WriteObjectArray(lighting);
			}

			writer.WriteObjectArray(BlareEffects);
			writer.WriteObjectArray(ParticleEmitterEffects);
			writer.WriteObjectArray(VideoOverlayEffects);
		}


		bool IFileSerializable<EventType>.CheckCanReadFile(BinaryObjectReader reader, EventType context, ref FileIOInfo fileInfo)
		{
			fileInfo.Endianness ??= context.GetEndianness();
			return true;
		}

		bool IFileSerializable<EventType>.CheckCanWriteFile(EventType context, ref FileIOInfo fileInfo)
		{
			fileInfo.Endianness ??= context.GetEndianness();
			return true;
		}
	}
}
