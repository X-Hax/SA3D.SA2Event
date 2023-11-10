using SA3D.Common.IO;
using SA3D.SA2Event.Language;
using System.Collections.ObjectModel;
using System.IO;

namespace SA3D.SA2Event.Effects
{
	/// <summary>
	/// Event effect data.
	/// </summary>
	public class EventEffects
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
				new ObjectLighting[][] {
					new ObjectLighting[256],
					new ObjectLighting[256],
					new ObjectLighting[256],
					new ObjectLighting[256]});

			BlareEffects = new BlareEffect[64];
			ParticleEmitterEffects = new ParticleEmitterEffect[64];
			VideoOverlayEffects = new VideoOverlayEffect[64];
		}




		/// <summary>
		/// Writes the event effects to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		public void Write(EndianStackWriter writer)
		{
			BaseLanguageTimestamps.Write(writer);

			foreach(ScreenEffect screenEffect in ScreenEffects)
			{
				screenEffect.Write(writer);
			}

			foreach(SimpleParticleEffect particle in Particles)
			{
				particle.Write(writer);
			}

			foreach(ObjectLighting[] lighting in Lighting)
			{
				foreach(ObjectLighting light in lighting)
				{
					light.Write(writer);
				}
			}

			foreach(BlareEffect blareEffect in BlareEffects)
			{
				blareEffect.Write(writer);
			}

			foreach(ParticleEmitterEffect particleEmitter in ParticleEmitterEffects)
			{
				particleEmitter.Write(writer);
			}

			foreach(VideoOverlayEffect videoOverlay in VideoOverlayEffects)
			{
				videoOverlay.Write(writer);
			}
		}

		/// <summary>
		/// Writes the event effects to byte data.
		/// </summary>
		/// <param name="bigEndian">Whether to write using big endian.</param>
		/// <returns>The written data.</returns>
		public byte[] WriteToData(bool bigEndian)
		{
			using(MemoryStream stream = new())
			{
				EndianStackWriter writer = new(stream, bigEndian: bigEndian);
				Write(writer);
				return stream.ToArray();
			}
		}

		/// <summary>
		/// Reads event effects off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <returns>The event effects that were read.</returns>
		public static EventEffects Read(EndianStackReader reader)
		{
			EventEffects result = new()
			{
				BaseLanguageTimestamps = EventLanguageTimestamps.Read(reader)
			};

			reader.ReadArray(0x9800, ScreenEffect.Read, ScreenEffect.StructSize, result.ScreenEffects);
			reader.ReadArray(0xA800, SimpleParticleEffect.Read, SimpleParticleEffect.StructSize, result.Particles);

			reader.ReadArray(0x26800, ObjectLighting.Read, ObjectLighting.StructSize, result.Lighting[0]);
			reader.ReadArray(0x2AC00, ObjectLighting.Read, ObjectLighting.StructSize, result.Lighting[1]);
			reader.ReadArray(0x2F000, ObjectLighting.Read, ObjectLighting.StructSize, result.Lighting[2]);
			reader.ReadArray(0x33400, ObjectLighting.Read, ObjectLighting.StructSize, result.Lighting[3]);

			reader.ReadArray(0x37800, BlareEffect.Read, BlareEffect.StructSize, result.BlareEffects);
			reader.ReadArray(0x38800, ParticleEmitterEffect.Read, ParticleEmitterEffect.StructSize, result.ParticleEmitterEffects);
			reader.ReadArray(0x39800, VideoOverlayEffect.Read, VideoOverlayEffect.StructSize, result.VideoOverlayEffects);

			return result;
		}

		/// <summary>
		/// Reads event effects off byte data.
		/// </summary>
		/// <param name="data">The data to read.</param>
		/// <param name="bigEndian">Whether to read using big endian.</param>
		/// <returns>The event effects that were read.</returns>
		public static EventEffects ReadFromData(byte[] data, bool bigEndian)
		{
			using(EndianStackReader reader = new(data, bigEndian: bigEndian))
			{
				return Read(reader);
			}
		}

		/// <summary>
		/// Reads event effects off event source data.
		/// </summary>
		/// <param name="source">The event source to read from.</param>
		/// <param name="bigEndian">Whether to read using big endian.</param>
		/// <returns>The event effects that were read.</returns>
		public static EventEffects? ReadFromEventSource(EventSource source, bool bigEndian)
		{
			if(source.Effects == null)
			{
				return null;
			}

			return ReadFromData(source.Effects, bigEndian);
		}
	}
}
