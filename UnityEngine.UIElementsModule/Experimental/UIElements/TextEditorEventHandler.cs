using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class TextEditorEventHandler
	{
		protected TextEditorEventHandler(TextEditorEngine editorEngine, ITextInputField textInputField)
		{
			this.editorEngine = editorEngine;
			this.textInputField = textInputField;
			this.textInputField.SyncTextEngine();
		}

		private protected TextEditorEngine editorEngine { protected get; private set; }

		private protected ITextInputField textInputField { protected get; private set; }

		public virtual void ExecuteDefaultActionAtTarget(EventBase evt)
		{
		}

		public virtual void ExecuteDefaultAction(EventBase evt)
		{
			if (evt.GetEventTypeId() == EventBase<FocusEvent>.TypeId())
			{
				this.editorEngine.OnFocus();
				this.editorEngine.SelectAll();
			}
			else if (evt.GetEventTypeId() == EventBase<BlurEvent>.TypeId())
			{
				this.editorEngine.OnLostFocus();
				this.editorEngine.SelectNone();
			}
		}
	}
}
