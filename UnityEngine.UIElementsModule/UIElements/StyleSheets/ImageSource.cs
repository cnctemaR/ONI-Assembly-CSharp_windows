using System;

namespace UnityEngine.UIElements.StyleSheets
{
	internal struct ImageSource
	{
		public bool IsNull()
		{
			return this.texture == null && this.sprite == null && this.vectorImage == null && this.renderTexture == null;
		}

		public Texture2D texture;

		public Sprite sprite;

		public VectorImage vectorImage;

		public RenderTexture renderTexture;
	}
}
