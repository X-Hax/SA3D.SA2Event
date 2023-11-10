using SA3D.Modeling.ObjectData;
using SA3D.Modeling.Structs;
using System;

namespace SA3D.SA2Event.Model
{
	internal static class ModelUtils
	{
		public static uint GetAddress(this Node? node, PointerLUT lut)
		{
			if(node == null)
			{
				return 0;
			}
			else if(lut.Nodes.TryGetAddress(node, out uint nodeAddr))
			{
				return nodeAddr;
			}
			else
			{
				throw new InvalidOperationException($"Node \"{node.Label}\" is not part of the scene entities!");
			}
		}
	}
}
