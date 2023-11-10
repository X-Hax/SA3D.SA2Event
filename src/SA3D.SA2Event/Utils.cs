using SA3D.Common.IO;
using System;
using System.Collections.Generic;

namespace SA3D.SA2Event
{
	internal static class Utils
	{
		public static void ReplaceContents<T>(this T[] output, IEnumerable<T> newValues)
		{
			Array.Clear(output);
			int i = 0;
			foreach(T screeneffect in newValues)
			{
				output[i] = screeneffect;
				i++;
			}
		}

		public static void ReadArray<T>(this EndianStackReader data, uint address, Func<EndianStackReader, uint, T> read, uint size, T[] result)
		{
			for(int i = 0; i < result.Length; i++)
			{
				result[i] = read(data, address);
				address += size;
			}
		}

	}
}
