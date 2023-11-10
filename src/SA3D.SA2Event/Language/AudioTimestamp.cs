using SA3D.Common.IO;
using System;

namespace SA3D.SA2Event.Language
{
	/// <summary>
	/// Audio replay timestamp.
	/// </summary>
	public struct AudioTimestamp : IFrame, IEquatable<AudioTimestamp>
	{
		/// <summary>
		/// Size of the structure in bytes.
		/// </summary>
		public const uint StructSize = 72;

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


		/// <summary>
		/// Writes the audio timestamp to an endian stack writer.
		/// </summary>
		/// <param name="writer">The writer to write to.</param>
		/// <exception cref="InvalidOperationException"></exception>
		public readonly void Write(EndianStackWriter writer)
		{
			writer.WriteUInt(Frame);
			writer.WriteUShort(MasterListVoiceIndex);
			writer.WriteUShort(AFSVoiceIndex);

			string musicname = MusicName ?? string.Empty;

			if(musicname.Length > 64)
			{
				throw new InvalidOperationException("Filename too long! must be <= 16 characters long");
			}

			writer.WriteString(musicname);
			if(musicname.Length < 64)
			{
				writer.WriteEmpty((uint)(64 - musicname.Length));
			}
		}

		/// <summary>
		/// Reads an audio timestamp off an endian stack reader.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="address">Address at which to start reading.</param>
		/// <returns>The audio timestamp that was read.</returns>
		public static AudioTimestamp Read(EndianStackReader reader, uint address)
		{
			return new(
				reader.ReadUInt(address),
				reader.ReadUShort(address + 4),
				reader.ReadUShort(address + 6),
				reader.ReadStringLimited(address + 8, 64, out _));
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
