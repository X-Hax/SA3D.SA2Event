using Amicitia.IO.Binary;
using SA3D.Common.IO;
using SA3D.Modeling.AnimationData;
using SA3D.Modeling.Structs;
using System;
using System.Collections.Generic;

namespace SA3D.SA2Event.Model
{
	/// <summary>
	/// Event binary de/serialization context
	/// </summary>
	public class EventModelIOContext
	{
		private EventType? _eventType;

		/// <summary>
		/// Format that the mesh data is serialized with
		/// </summary>
		public EventType EventType
		{
			get
			{
				if(_eventType == null)
				{
					throw new InvalidOperationException("Event type has not yet been specified!");
				}

				return _eventType.Value;
			}
			set => _eventType = value;
		}

		/// <summary>
		/// Pointer lookup table
		/// </summary>
		public ModelOffsetLUT OffsetLUT { get; }


		/// <summary>
		/// Reader for reading animation data
		/// </summary>
		public BinaryObjectReader? AnimationReader { get; }

		/// <summary>
		/// Offset lookup table for <see cref="AnimationReader"/>
		/// </summary>
		public ModelOffsetLUT? AnimationReaderOffsetLUT { get; }

		/// <summary>
		/// Output animations for when writing <see cref="EventType.gc"/> events
		/// </summary>
		public List<Animation?>? OutputAnimations { get; }

		private EventModelIOContext(
			BinaryObjectReader? animationReader,
			ModelOffsetLUT? animationReaderOffsetLUT,
			List<Animation?>? outputAnimations)
		{
			OffsetLUT = new();
			AnimationReader = animationReader;
			AnimationReaderOffsetLUT = animationReaderOffsetLUT;
			OutputAnimations = outputAnimations;
		}


		/// <summary>
		/// Creates an event animation IO handler for reading
		/// </summary>
		public static EventModelIOContext CreateForReading(BinaryObjectReader? reader)
		{
			return new(reader, reader == null ? null : new(), null);
		}

		/// <summary>
		/// Creates an event animation IO handler for writing
		/// </summary>
		public static EventModelIOContext CreateForWriting()
		{
			return new(null, null, []);
		}


		/// <summary>
		/// Reads an animation reference from either <paramref name="reader"/> itself, or from <see cref="AnimationReader"/> (if the event type is GC)
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="reader">The reader where the animation is referenced</param>
		/// <param name="keyframeCount">Number of keyframe sets in the animation</param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public T? ReadAnimation<T>(BinaryObjectReader reader, uint keyframeCount) where T : Animation, new()
		{
			if(OutputAnimations != null)
			{
				throw new InvalidOperationException("Handler not set up for reading!");
			}

			if(EventType != EventType.gc)
			{
				AnimationIOContext animationContext = new()
				{
					KeyframeSetCount = keyframeCount,
					OffsetLUT = OffsetLUT
				};

				return reader.ReadObjectOffset<T, AnimationIOContext>(animationContext, OffsetLUT);
			}

			if(AnimationReader == null)
			{
				throw new InvalidOperationException("No animation reader provided for GC events!");
			}

			int index = reader.ReadInt32();
			long indexPosition = AnimationReader.OffsetHandler.CalculateOffset(index * sizeof(long));

			using(AnimationReader.At(indexPosition, System.IO.SeekOrigin.Begin))
			{
				long animationOffset = AnimationReader.ReadOffsetValue();
				if(animationOffset == uint.MaxValue)
				{
					return null;
				}

				uint keyframeSetCount = AnimationReader.ReadUInt32();

				AnimationIOContext animationContext = new()
				{
					KeyframeSetCount = keyframeSetCount,
					OffsetLUT = AnimationReaderOffsetLUT!
				};

				return AnimationReader.ReadObjectAtOffset<T, AnimationIOContext>(animationOffset, animationContext, AnimationReaderOffsetLUT!);
			}
		}

		/// <summary>
		/// Writes a reference to an animation (When <see cref="EventType.gc"/>, then an index to the animation in <see cref="OutputAnimations"/>, else written directly to <paramref name="writer"/>)
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="animation"></param>
		/// <exception cref="InvalidOperationException"></exception>
		public void WriteAnimation(BinaryObjectWriter writer, Animation? animation)
		{
			if(OutputAnimations == null)
			{
				throw new InvalidOperationException("Handler not set up for writing!");
			}

			if(EventType != EventType.gc)
			{
				AnimationIOContext animationContext = new()
				{
					OffsetLUT = OffsetLUT
				};

				writer.WriteObjectOffset(animation, animationContext, OffsetLUT);
				return;
			}

			int index = OutputAnimations.IndexOf(animation);

			if(index == -1)
			{
				index = OutputAnimations.Count;
				OutputAnimations.Add(animation);
			}

			writer.WriteInt32(index);
		}
	}
}
