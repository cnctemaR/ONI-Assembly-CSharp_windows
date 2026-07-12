using System;

namespace UnityEngine.UIElements
{
	internal class TextEditingManipulator
	{
		private bool touchScreenTextFieldChanged
		{
			get
			{
				bool touchScreenTextFieldInitialized = this.m_TouchScreenTextFieldInitialized;
				TextEditingUtilities textEditingUtilities = this.editingUtilities;
				bool? flag = ((textEditingUtilities != null) ? new bool?(textEditingUtilities.TouchScreenKeyboardShouldBeUsed()) : null);
				return !((touchScreenTextFieldInitialized == flag.GetValueOrDefault()) & (flag != null));
			}
		}

		public TextEditingManipulator(TextElement textElement)
		{
			this.m_TextElement = textElement;
			this.editingUtilities = new TextEditingUtilities(textElement.selectingManipulator.m_SelectingUtilities, textElement.uitkTextHandle, textElement.text);
			this.InitTextEditorEventHandler();
		}

		private void InitTextEditorEventHandler()
		{
			TextEditingUtilities textEditingUtilities = this.editingUtilities;
			this.m_TouchScreenTextFieldInitialized = textEditingUtilities != null && textEditingUtilities.TouchScreenKeyboardShouldBeUsed();
			bool touchScreenTextFieldInitialized = this.m_TouchScreenTextFieldInitialized;
			if (touchScreenTextFieldInitialized)
			{
				this.editingEventHandler = new TouchScreenTextEditorEventHandler(this.m_TextElement, this.editingUtilities);
			}
			else
			{
				this.editingEventHandler = new KeyboardTextEditorEventHandler(this.m_TextElement, this.editingUtilities);
			}
		}

		internal void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			bool isReadOnly = this.m_TextElement.edition.isReadOnly;
			if (!isReadOnly)
			{
				FocusInEvent focusInEvent = evt as FocusInEvent;
				if (focusInEvent == null)
				{
					FocusOutEvent focusOutEvent = evt as FocusOutEvent;
					if (focusOutEvent != null)
					{
						this.OnFocusOutEvent(focusOutEvent);
					}
				}
				else
				{
					this.OnFocusInEvent(focusInEvent);
				}
				TextEditorEventHandler textEditorEventHandler = this.editingEventHandler;
				if (textEditorEventHandler != null)
				{
					textEditorEventHandler.ExecuteDefaultActionAtTarget(evt);
				}
			}
		}

		private void OnFocusInEvent(FocusInEvent _)
		{
			this.m_TextElement.edition.SaveValueAndText();
			this.m_TextElement.focusController.selectedTextElement = this.m_TextElement;
			bool touchScreenTextFieldChanged = this.touchScreenTextFieldChanged;
			if (touchScreenTextFieldChanged)
			{
				this.InitTextEditorEventHandler();
			}
			bool flag = this.m_HardwareKeyboardPoller == null;
			if (flag)
			{
				this.m_HardwareKeyboardPoller = this.m_TextElement.schedule.Execute(delegate
				{
					bool touchScreenTextFieldChanged2 = this.touchScreenTextFieldChanged;
					if (touchScreenTextFieldChanged2)
					{
						this.InitTextEditorEventHandler();
						this.m_TextElement.Blur();
					}
				}).Every(250L);
			}
			else
			{
				this.m_HardwareKeyboardPoller.Resume();
			}
		}

		private void OnFocusOutEvent(FocusOutEvent _)
		{
			IVisualElementScheduledItem hardwareKeyboardPoller = this.m_HardwareKeyboardPoller;
			if (hardwareKeyboardPoller != null)
			{
				hardwareKeyboardPoller.Pause();
			}
			this.editingUtilities.OnBlur();
		}

		private TextElement m_TextElement;

		internal TextEditorEventHandler editingEventHandler;

		internal TextEditingUtilities editingUtilities;

		private bool m_TouchScreenTextFieldInitialized;

		private IVisualElementScheduledItem m_HardwareKeyboardPoller = null;
	}
}
