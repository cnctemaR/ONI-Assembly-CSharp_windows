using System;

namespace UnityEngine.UIElements
{
	internal interface IPointerOrMouseEvent
	{
		int pointerId { get; }

		Vector3 position { get; }

		Vector3 deltaPosition { get; set; }
	}
}
