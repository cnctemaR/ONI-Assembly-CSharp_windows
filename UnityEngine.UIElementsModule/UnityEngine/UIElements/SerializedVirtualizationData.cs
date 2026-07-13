using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
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
