using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	internal class SliderState
	{
		[RequiredByNativeCode]
		public SliderState()
		{
		}

		public float dragStartPos;

		public float dragStartValue;

		public bool isDragging;
	}
}
