using System;

namespace UnityEngine.UIElements.UIR
{
	internal class GradientRemap : LinkedPoolItem<GradientRemap>
	{
		public void Reset()
		{
			this.origIndex = 0;
			this.destIndex = 0;
			this.location = default(RectInt);
			this.isAtlassed = false;
		}

		public int origIndex;

		public int destIndex;

		public RectInt location;

		public GradientRemap next;

		public bool isAtlassed;
	}
}
