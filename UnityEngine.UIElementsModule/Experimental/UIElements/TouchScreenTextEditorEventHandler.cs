using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class TouchScreenTextEditorEventHandler : TextEditorEventHandler
	{
		public TouchScreenTextEditorEventHandler(TextEditorEngine editorEngine, ITextInputField textInputField)
			: base(editorEngine, textInputField)
		{
			this.secureText = string.Empty;
		}

		public string secureText
		{
			get
			{
				return this.m_SecureText;
			}
			set
			{
				string text = value ?? string.Empty;
				if (text != this.m_SecureText)
				{
					this.m_SecureText = text;
				}
			}
		}

		public override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);
			long num = EventBase<MouseDownEvent>.TypeId();
			if (evt.GetEventTypeId() == num)
			{
				base.textInputField.SyncTextEngine();
				base.textInputField.UpdateText(base.editorEngine.text);
				base.textInputField.CaptureMouse();
				base.editorEngine.keyboardOnScreen = TouchScreenKeyboard.Open(string.IsNullOrEmpty(this.secureText) ? base.textInputField.text : this.secureText, TouchScreenKeyboardType.Default, true, base.editorEngine.multiline, !string.IsNullOrEmpty(this.secureText));
				base.editorEngine.UpdateScrollOffset();
				evt.StopPropagation();
			}
		}

		private string m_SecureText;
	}
}
