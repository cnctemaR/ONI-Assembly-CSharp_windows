using System;

namespace UnityEngine.Experimental.UIElements
{
	public struct Flex
	{
		public Flex(float g, float s = 1f, float b = 0f)
		{
			this.grow = g;
			this.shrink = s;
			this.basis = b;
		}

		public float grow { get; set; }

		public float shrink { get; set; }

		public float basis { get; set; }
	}
}
