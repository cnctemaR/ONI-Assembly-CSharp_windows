using System;

namespace UnityEngine.UIElements
{
	internal class TextEditorEventHandler
	{
		protected TextEditorEventHandler(TextElement textElement, TextEditingUtilities editingUtilities)
		{
			this.textElement = textElement;
			this.editingUtilities = editingUtilities;
		}

		public virtual void ExecuteDefaultActionAtTarget(EventBase evt)
		{
		}

		protected TextElement textElement;

		protected TextEditingUtilities editingUtilities;
	}
}
