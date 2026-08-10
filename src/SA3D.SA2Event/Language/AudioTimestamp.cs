using Amicitia.IO.Binary;
using System;

namespace SA3D.SA2Event.Language
{
	/// <summary>
	/// Audio replay timestamp.
	/// </summary>
	public struct AudioTimestamp : IFrame, IEquatable<AudioTimestamp>, IBinarySerializable
	{
		/// <summary>
		/// Frame at which the audio shoul start playing.
		/// </summary>
		public uint Frame { get; set; }

		/// <summary>
		/// ndex to the voice audio to play via the master voice list.
		/// <br/> Usually consist of the local voice index + 1000 * event ID.
		/// </summary>
		public ushort MasterListVoiceIndex { get; set; }

		/// <summary>
		/// ndex to the voice audio to play in the AFS archive storing all voice audios.
		/// </summary>
		public ushort AFSVoiceIndex { get; set; }

		/// <summary>
		/// ame of the music to play.
		/// </summary>
		public string MusicName { get; set; }


		/// <summary>
		/// Creates a new audio timestamp.
		/// </summary>
		/// <param name="frame">Frame at which the audio shoul start playing.</param>
		/// <param name="masterListVoiceIndex">Index to the voice audio to play via the master voice list.</param>
		/// <param name="afsVoiceIndex">Index to the voice audio to play in the AFS archive storing all voice audios.</param>
		/// <param name="musicName">Name of the music to play.</param>
		public AudioTimestamp(uint frame, ushort masterListVoiceIndex, ushort afsVoiceIndex, string musicName)
		{
			Frame = frame;
			MasterListVoiceIndex = masterListVoiceIndex;
			AFSVoiceIndex = afsVoiceIndex;
			MusicName = musicName;
		}


		/// <inheritdoc/>
		public void Read(BinaryObjectReader reader)
		{
			Frame = reader.ReadUInt32();
			MasterListVoiceIndex = reader.ReadUInt16();
			AFSVoiceIndex = reader.ReadUInt16();
			MusicName = reader.ReadString(StringBinaryFormat.FixedLength, 64);
		}

		/// <inheritdoc/>
		public readonly void Write(BinaryObjectWriter writer)
		{
			writer.WriteUInt32(Frame);
			writer.WriteUInt16(MasterListVoiceIndex);
			writer.WriteUInt16(AFSVoiceIndex);

			string musicname = MusicName ?? string.Empty;

			if(musicname.Length > 64)
			{
				throw new InvalidOperationException("Music name too long! must be <= 64 characters long");
			}

			writer.WriteString(StringBinaryFormat.FixedLength, musicname, 64);
		}


		/// <inheritdoc/>
		public override readonly bool Equals(object? obj)
		{
			return obj is AudioTimestamp timestamp &&
				   Frame == timestamp.Frame &&
				   MasterListVoiceIndex == timestamp.MasterListVoiceIndex &&
				   AFSVoiceIndex == timestamp.AFSVoiceIndex &&
				   MusicName == timestamp.MusicName;
		}

		/// <inheritdoc/>
		public override readonly int GetHashCode()
		{
			return HashCode.Combine(Frame, MasterListVoiceIndex, AFSVoiceIndex, MusicName);
		}

		readonly bool IEquatable<AudioTimestamp>.Equals(AudioTimestamp other)
		{
			return Equals(other);
		}

		/// <summary>
		/// Compares two audio timestamps for equality.
		/// </summary>
		/// <param name="left">Lefthand audio timestamp.</param>
		/// <param name="right">Righthand audio timestamp.</param>
		/// <returns>Whether the two audio timestamps are equal</returns>
		public static bool operator ==(AudioTimestamp left, AudioTimestamp right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Compares two audio timestamps for inequality.
		/// </summary>
		/// <param name="left">Lefthand audio timestamp.</param>
		/// <param name="right">Righthand audio timestamp.</param>
		/// <returns>Whether the two audio timestamps are inequal</returns>
		public static bool operator !=(AudioTimestamp left, AudioTimestamp right)
		{
			return !(left == right);
		}


		/// <inheritdoc/>
		public override readonly string ToString()
		{
			return $"{Frame} {MasterListVoiceIndex:X4} {AFSVoiceIndex:X4} {MusicName}";
		}
	}
}
