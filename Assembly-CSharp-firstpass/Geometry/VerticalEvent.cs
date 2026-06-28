using System;

namespace Geometry
{
	public struct VerticalEvent
	{
		public VerticalEvent(float y, bool isStart, bool subtract)
		{
			this.y = y;
			this.isStart = isStart;
			this.subtract = subtract;
		}

		public float y;

		public bool isStart;

		public bool subtract;
	}
}
