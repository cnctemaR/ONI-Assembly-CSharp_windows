using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.UIR
{
	internal class DrawParams
	{
		public void Reset()
		{
			this.view.Clear();
			this.view.Push(Matrix4x4.identity);
			this.scissor.Clear();
			this.scissor.Push(DrawParams.k_UnlimitedRect);
			this.defaultMaterial.Clear();
		}

		internal static readonly Rect k_UnlimitedRect = new Rect(-100000f, -100000f, 200000f, 200000f);

		internal static readonly Rect k_FullNormalizedRect = new Rect(-1f, -1f, 2f, 2f);

		internal readonly Stack<Matrix4x4> view = new Stack<Matrix4x4>(8);

		internal readonly Stack<Rect> scissor = new Stack<Rect>(8);

		internal readonly List<Material> defaultMaterial = new List<Material>(8);

		internal readonly List<MaterialPropertyBlock> props = new List<MaterialPropertyBlock>(8);
	}
}
