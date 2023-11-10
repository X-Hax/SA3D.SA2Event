using SA3D.Modeling.Animation;
using System.Collections.Generic;

namespace SA3D.SA2Event.Animation
{
	internal static class MotionUtils
	{
		public static uint GetMotionKey(this Dictionary<EventMotion, uint> motionLUT, EventMotion motion)
		{
			if(motionLUT.TryGetValue(motion, out uint result))
			{
				return result;
			}

			return 0;
		}

		public static uint GetMotionKey(this Dictionary<EventMotion, uint> motionLUT, Motion? motion)
		{
			return motionLUT.GetMotionKey(new EventMotion(motion, null));
		}

	}
}
