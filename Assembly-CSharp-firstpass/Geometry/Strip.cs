using System;

namespace Geometry
{
	public class Strip
	{
		public Strip(float yMin, float yMax, bool subtract)
		{
			this.yMin = yMin;
			this.yMax = yMax;
			this.subtract = subtract;
		}

		public float yMin;

		public float yMax;

		public bool subtract;
	}
}
