using System;

namespace UnityEngine.UIElements.StyleSheets
{
	[Serializable]
	internal struct ScalableImage
	{
		public override string ToString()
		{
			return string.Format("{0}: {1}, {2}: {3}", new object[] { "normalImage", this.normalImage, "highResolutionImage", this.highResolutionImage });
		}

		public Texture2D normalImage;

		public Texture2D highResolutionImage;
	}
}
