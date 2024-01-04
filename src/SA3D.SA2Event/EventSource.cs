using SA3D.Archival;
using System;
using System.Collections.Generic;
using System.IO;

namespace SA3D.SA2Event
{
	/// <summary>
	/// Event byte source data.
	/// </summary>
	public class EventSource
	{
		/// <summary>
		/// Base file path. Holds the file path to the model file without file extension, which is the common path between all event file. 
		/// <br/> Vanilla is usually formatted like: "[path to]/Sonic Adventure 2/resource/gd_PC/event/e####".
		/// </summary>
		public string? BaseFilepath { get; set; }

		/// <summary>
		/// Decompressed Model data. Vanilla is filename is usually formatted like "e####.prs".
		/// </summary>
		public byte[] Model { get; }

		/// <summary>
		/// Motion data. Used in gamecube and ports. Vanilla filename is usually formatted like "e####motion.bin".
		/// </summary>
		public byte[]? Motion { get; }

		/// <summary>
		/// Decompressed texture archive. Vanilla filename is usually formatted like "e####texture.prs".
		/// </summary>
		public byte[]? Textures { get; }

		/// <summary>
		/// Decompressed external texturename list. Vanilla filename usually formatted like "e####texlist.prs".
		/// </summary>
		public byte[]? Texlist { get; }

		/// <summary>
		/// Decompressed effect data. Vanilla filename usually formatted like "e####_0.prs".
		/// </summary>
		public byte[]? Effects { get; }

		/// <summary>
		/// Decompressed language data. Vanilla filenames usually formatted like "e####_#.prs".
		/// </summary>
		public Dictionary<EventLanguage, byte[]> LanguageTimestamps { get; }


		/// <summary>
		/// Creates new event source.
		/// </summary>
		/// <param name="baseFilePath">Base file path</param>
		/// <param name="model">Decompressed Model data.</param>
		/// <param name="motion">Motion data.</param>
		/// <param name="textures">Decompressed texture archive.</param>
		/// <param name="texlist">Decompressed external texturename list.</param>
		/// <param name="effects">Decompressed effect data.</param>
		/// <param name="languageTimestamps">Decompressed language data.</param>
		public EventSource(string? baseFilePath, byte[] model, byte[]? motion, byte[]? textures, byte[]? texlist, byte[]? effects, Dictionary<EventLanguage, byte[]> languageTimestamps)
		{
			BaseFilepath = baseFilePath;
			Model = model;
			Motion = motion;
			Textures = textures;
			Texlist = texlist;
			Effects = effects;
			LanguageTimestamps = languageTimestamps;
		}


		/// <summary>
		/// Returns the base from the given model file path.
		/// </summary>
		/// <param name="modelFilepath">Path to the event model file ("e####.prs")</param>
		/// <returns>The base file path.</returns>
		public static string GetEventBaseFilepath(string modelFilepath)
		{
			string absolutePath = Path.GetFullPath(modelFilepath);
			string? dir = Path.GetDirectoryName(absolutePath);
			string filename = Path.GetFileNameWithoutExtension(absolutePath);
			return dir == null ? filename : Path.Join(dir, filename);
		}


		/// <summary>
		/// Writes the event source to files. Uses base file path if <paramref name="filepath"/> is empty.
		/// </summary>
		/// <param name="filepath">The base file path to write to. If left null, <see cref="BaseFilepath"/> will be used.</param>
		/// <param name="compress">Whether to compress the files. Game requires compressed files.</param>
		/// <exception cref="InvalidOperationException"></exception>
		public void WriteToFiles(string? filepath = null, bool compress = true)
		{
			filepath = filepath == null
				? BaseFilepath ?? throw new InvalidOperationException("Source has no path, nor has a path been passed over")
				: GetEventBaseFilepath(filepath);

			void WriteFile(byte[]? data, string append, bool compressfile)
			{
				if(data == null)
				{
					return;
				}

				if(compress && compressfile)
				{
					data = PRS.CompressPRS(data);
				}

				File.WriteAllBytes(filepath + append, data);
			}

			WriteFile(Model, ".prs", true);
			WriteFile(Motion, "motion.bin", false);
			WriteFile(Textures, "texture.prs", true);
			WriteFile(Texlist, "texlist.prs", true);
			WriteFile(Effects, "_0.prs", true);

			foreach(KeyValuePair<EventLanguage, byte[]> item in LanguageTimestamps)
			{
				WriteFile(item.Value, $"_{(char)item.Key}.prs", true);
			}
		}

		/// <summary>
		/// Reads event source data from event files.
		/// </summary>
		/// <param name="filepath">Path to the model file or the base file path.</param>
		/// <returns>The event source.</returns>
		/// <exception cref="FileNotFoundException"></exception>
		public static EventSource ReadFromFiles(string filepath)
		{
			string basePath = GetEventBaseFilepath(filepath);

			byte[]? GetFile(string append)
			{
				string path = basePath + append;
				if(!File.Exists(path))
				{
					return null;
				}

				byte[] result = File.ReadAllBytes(path);
				if(Path.GetExtension(path).ToLower() == ".prs")
				{
					result = PRS.DecompressPRS(result);
				}

				return result;
			}

			byte[] GetFileExc(string append)
			{
				byte[]? result = GetFile(append);
				if(result == null)
				{
					string filename = Path.GetFileNameWithoutExtension(basePath + append);
					throw new FileNotFoundException($"Can't find file \"{filename}\"!", $"{filename}");
				}

				return result;
			}

			byte[] main = GetFileExc(".prs");
			byte[]? motion = GetFile("motion.bin");
			byte[]? textures = GetFile("texture.prs");
			byte[]? texlist = GetFile("texlist.prs");
			byte[]? effects = GetFile("_0.prs");

			Dictionary<EventLanguage, byte[]> languageSource = new();

			foreach(EventLanguage language in Enum.GetValues<EventLanguage>())
			{
				byte[]? langData = GetFile($"_{(char)language}.prs");
				if(langData != null)
				{
					languageSource.Add(language, langData);
				}
			}

			return new(basePath, main, motion, textures, texlist, effects, languageSource);
		}

	}
}
