using System;

namespace Geometry
{
	public struct HorizontalEvent
	{
		public HorizontalEvent(float x, Strip strip, bool isStart)
		{
			this.x = x;
			this.strip = strip;
			this.isStart = isStart;
		}

		public float x;

		public Strip strip;

		public bool isStart;
	}
}
