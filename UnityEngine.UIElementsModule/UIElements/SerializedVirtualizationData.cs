using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	internal class SerializedVirtualizationData
	{
		public Vector2 scrollOffset;

		public int firstVisibleIndex;

		public float contentPadding;

		public float contentHeight;

		public int anchoredItemIndex;

		public float anchorOffset;
	}
}
