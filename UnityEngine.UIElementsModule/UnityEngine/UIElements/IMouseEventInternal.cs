using System;

namespace UnityEngine.UIElements
{
	internal interface IMouseEventInternal
	{
		IPointerEvent sourcePointerEvent { get; }

		bool recomputeTopElementUnderMouse { get; }
	}
}
