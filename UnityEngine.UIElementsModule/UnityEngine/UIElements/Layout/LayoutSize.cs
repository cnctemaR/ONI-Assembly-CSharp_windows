using System;

namespace UnityEngine.UIElements.Layout
{
	internal struct LayoutSize
	{
		public LayoutSize(float width, float height)
		{
			this.width = width;
			this.height = height;
		}

		public float width;

		public float height;
	}
}
