using System;

namespace UnityEngine.UIElements
{
	internal class TextEditingManipulator
	{
		internal TextEditorEventHandler editingEventHandler
		{
			get
			{
				return this.m_EditingEventHandler;
			}
			set
			{
				bool flag = this.m_EditingEventHandler == value;
				if (!flag)
				{
					TextEditorEventHandler editingEventHandler = this.m_EditingEventHandler;
					if (editingEventHandler != null)
					{
						editingEventHandler.UnregisterCallbacksFromTarget(this.m_TextElement);
					}
					this.m_EditingEventHandler = value;
					TextEditorEventHandler editingEventHandler2 = this.m_EditingEventHandler;
					if (editingEventHandler2 != null)
					{
						editingEventHandler2.RegisterCallbacksOnTarget(this.m_TextElement);
					}
				}
			}
		}

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

		public void Reset()
		{
			this.editingEventHandler = null;
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

		internal void HandleEventBubbleUp(EventBase evt)
		{
			bool isReadOnly = this.m_TextElement.edition.isReadOnly;
			if (!isReadOnly)
			{
				bool flag = evt is BlurEvent;
				if (flag)
				{
					this.m_TextElement.uitkTextHandle.RemoveFromPermanentCache();
				}
				else
				{
					bool flag2 = (!(evt is PointerMoveEvent) && !(evt is MouseMoveEvent)) || this.m_TextElement.selectingManipulator.isClicking;
					if (flag2)
					{
						this.m_TextElement.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
					}
				}
				if (!(evt is FocusInEvent))
				{
					if (evt is FocusOutEvent)
					{
						this.OnFocusOutEvent();
					}
				}
				else
				{
					this.OnFocusInEvent();
				}
				TextEditorEventHandler editingEventHandler = this.editingEventHandler;
				if (editingEventHandler != null)
				{
					editingEventHandler.HandleEventBubbleUp(evt);
				}
			}
		}

		private void OnFocusInEvent()
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

		private void OnFocusOutEvent()
		{
			IVisualElementScheduledItem hardwareKeyboardPoller = this.m_HardwareKeyboardPoller;
			if (hardwareKeyboardPoller != null)
			{
				hardwareKeyboardPoller.Pause();
			}
			this.editingUtilities.OnBlur();
		}

		private readonly TextElement m_TextElement;

		private TextEditorEventHandler m_EditingEventHandler;

		internal TextEditingUtilities editingUtilities;

		private bool m_TouchScreenTextFieldInitialized;

		private IVisualElementScheduledItem m_HardwareKeyboardPoller = null;
	}
}
