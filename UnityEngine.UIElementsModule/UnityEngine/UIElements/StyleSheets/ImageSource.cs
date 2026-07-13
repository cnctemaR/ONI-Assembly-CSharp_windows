using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements.StyleSheets
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
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
