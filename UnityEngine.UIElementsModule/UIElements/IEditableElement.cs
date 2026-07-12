using System;

namespace UnityEngine.UIElements
{
	internal interface IEditableElement
	{
		Action editingStarted { get; set; }

		Action editingEnded { get; set; }
	}
}
