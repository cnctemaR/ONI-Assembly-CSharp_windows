using System;
using System.Collections.Generic;

namespace UnityEngine.UIElements.UIR
{
	internal class DrawParams
	{
		public void Reset(Rect _viewport, Matrix4x4 _projection)
		{
			this.viewport = _viewport;
			this.projection = _projection;
			this.view.Clear();
			this.view.Push(new ViewTransform
			{
				transform = Matrix4x4.identity,
				clipRect = DrawParams.k_UnlimitedRect.ToVector4()
			});
			this.scissor.Clear();
			this.scissor.Push(DrawParams.k_UnlimitedRect);
		}

		internal static readonly Rect k_UnlimitedRect = new Rect(-100000f, -100000f, 200000f, 200000f);

		internal Rect viewport;

		internal Matrix4x4 projection;

		internal readonly Stack<ViewTransform> view = new Stack<ViewTransform>(8);

		internal readonly Stack<Rect> scissor = new Stack<Rect>(8);
	}
}
