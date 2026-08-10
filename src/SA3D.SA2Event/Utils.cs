using Amicitia.IO.Binary;

namespace SA3D.SA2Event
{
	internal static class Utils
	{
		public static void ReadToObjectArray<T>(this T[] array, BinaryObjectReader reader) where T : IBinarySerializable, new()
		{
			for(int i = 0; i < array.Length; i++)
			{
				array[i] = reader.ReadObject<T>();
			}
		}
	}
}
