using System;

namespace UnityEngine.UIElements
{
	internal class TouchScreenTextEditorEventHandler : TextEditorEventHandler
	{
		internal static long Frame { get; private set; }

		internal static TouchScreenKeyboard activeTouchScreenKeyboard { get; private set; }

		public TouchScreenTextEditorEventHandler(TextElement textElement, TextEditingUtilities editingUtilities)
			: base(textElement, editingUtilities)
		{
		}

		private void PollTouchScreenKeyboard()
		{
			this.m_TouchKeyboardAllowsInPlaceEditing = TouchScreenKeyboard.isInPlaceEditingAllowed;
			bool flag = TouchScreenKeyboard.isSupported && !this.m_TouchKeyboardAllowsInPlaceEditing;
			if (flag)
			{
				bool flag2 = this.m_TouchKeyboardPoller == null;
				if (flag2)
				{
					TextElement textElement = this.textElement;
					this.m_TouchKeyboardPoller = ((textElement != null) ? textElement.schedule.Execute(new Action(this.DoPollTouchScreenKeyboard)).Every(100L) : null);
				}
				else
				{
					this.m_TouchKeyboardPoller.Resume();
				}
			}
		}

		private void DoPollTouchScreenKeyboard()
		{
			TouchScreenTextEditorEventHandler.Frame += 1L;
			bool flag = this.editingUtilities.TouchScreenKeyboardShouldBeUsed();
			if (flag)
			{
				bool flag2 = this.textElement.m_TouchScreenKeyboard == null;
				if (!flag2)
				{
					ITextEdition edition = this.textElement.edition;
					TouchScreenKeyboard touchScreenKeyboard = this.textElement.m_TouchScreenKeyboard;
					string text = touchScreenKeyboard.text;
					bool flag3 = touchScreenKeyboard.status > TouchScreenKeyboard.Status.Visible;
					if (flag3)
					{
						bool flag4 = touchScreenKeyboard.status == TouchScreenKeyboard.Status.Canceled;
						if (flag4)
						{
							edition.RestoreValueAndText();
						}
						else
						{
							text = touchScreenKeyboard.text;
							bool flag5 = this.editingUtilities.text != text;
							if (flag5)
							{
								edition.UpdateText(text);
								this.textElement.uitkTextHandle.Update();
							}
						}
						this.CloseTouchScreenKeyboard();
						bool flag6 = !edition.isDelayed;
						if (flag6)
						{
							Action updateValueFromText = edition.UpdateValueFromText;
							if (updateValueFromText != null)
							{
								updateValueFromText();
							}
						}
						bool flag7 = (!string.IsNullOrEmpty(touchScreenKeyboard.text) || string.IsNullOrEmpty(edition.placeholder)) && !edition.isDelayed;
						if (flag7)
						{
							Action updateTextFromValue = edition.UpdateTextFromValue;
							if (updateTextFromValue != null)
							{
								updateTextFromValue();
							}
						}
						this.textElement.Blur();
					}
					else
					{
						bool flag8 = this.editingUtilities.text == text;
						if (!flag8)
						{
							bool hideMobileInput = edition.hideMobileInput;
							if (hideMobileInput)
							{
								bool flag9 = this.editingUtilities.text != text;
								if (flag9)
								{
									bool flag10 = false;
									this.editingUtilities.text = "";
									foreach (char c in text)
									{
										bool flag11 = !edition.AcceptCharacter(c);
										if (flag11)
										{
											return;
										}
										bool flag12 = c > '\0';
										if (flag12)
										{
											TextEditingUtilities editingUtilities = this.editingUtilities;
											editingUtilities.text += c.ToString();
											flag10 = true;
										}
									}
									bool flag13 = flag10;
									if (flag13)
									{
										this.UpdateStringPositionFromKeyboard();
									}
									edition.UpdateText(this.editingUtilities.text);
									this.textElement.uitkTextHandle.ComputeSettingsAndUpdate();
								}
								else
								{
									bool flag14 = !this.m_IsClicking && touchScreenKeyboard != null && touchScreenKeyboard.canGetSelection;
									if (flag14)
									{
										this.UpdateStringPositionFromKeyboard();
									}
								}
							}
							else
							{
								edition.UpdateText(text);
								this.textElement.uitkTextHandle.ComputeSettingsAndUpdate();
							}
							bool flag15 = !edition.isDelayed;
							if (flag15)
							{
								Action updateValueFromText2 = edition.UpdateValueFromText;
								if (updateValueFromText2 != null)
								{
									updateValueFromText2();
								}
							}
							bool flag16 = (!string.IsNullOrEmpty(touchScreenKeyboard.text) || string.IsNullOrEmpty(edition.placeholder)) && !edition.isDelayed;
							if (flag16)
							{
								Action updateTextFromValue2 = edition.UpdateTextFromValue;
								if (updateTextFromValue2 != null)
								{
									updateTextFromValue2();
								}
							}
							Action<bool> updateScrollOffset = this.textElement.edition.UpdateScrollOffset;
							if (updateScrollOffset != null)
							{
								updateScrollOffset(false);
							}
						}
					}
				}
			}
			else
			{
				this.CloseTouchScreenKeyboard();
			}
		}

		private void UpdateStringPositionFromKeyboard()
		{
			bool flag = this.textElement.m_TouchScreenKeyboard == null;
			if (!flag)
			{
				RangeInt selection = this.textElement.m_TouchScreenKeyboard.selection;
				int start = selection.start;
				int end = selection.end;
				bool flag2 = this.textElement.selection.selectIndex != start;
				if (flag2)
				{
					this.textElement.selection.selectIndex = start;
				}
				bool flag3 = this.textElement.selection.cursorIndex != end;
				if (flag3)
				{
					this.textElement.selection.cursorIndex = end;
				}
			}
		}

		private void CloseTouchScreenKeyboard()
		{
			bool flag = this.textElement.m_TouchScreenKeyboard != null;
			if (flag)
			{
				this.textElement.m_TouchScreenKeyboard.active = false;
				this.textElement.m_TouchScreenKeyboard = null;
				IVisualElementScheduledItem touchKeyboardPoller = this.m_TouchKeyboardPoller;
				if (touchKeyboardPoller != null)
				{
					touchKeyboardPoller.Pause();
				}
				TouchScreenKeyboard.hideInput = true;
			}
			TouchScreenTextEditorEventHandler.activeTouchScreenKeyboard = null;
		}

		private void OpenTouchScreenKeyboard()
		{
			ITextEdition edition = this.textElement.edition;
			TouchScreenKeyboard.hideInput = edition.hideMobileInput;
			this.textElement.m_TouchScreenKeyboard = TouchScreenKeyboard.Open(this.textElement.text, edition.keyboardType, !edition.isPassword && edition.autoCorrection, edition.multiline, edition.isPassword);
			bool hideMobileInput = edition.hideMobileInput;
			if (hideMobileInput)
			{
				int selectIndex = this.textElement.selection.selectIndex;
				int cursorIndex = this.textElement.selection.cursorIndex;
				int num = ((selectIndex < cursorIndex) ? (cursorIndex - selectIndex) : (selectIndex - cursorIndex));
				int num2 = ((selectIndex < cursorIndex) ? selectIndex : cursorIndex);
				this.textElement.m_TouchScreenKeyboard.selection = new RangeInt(num2, num);
			}
			else
			{
				TouchScreenKeyboard touchScreenKeyboard = this.textElement.m_TouchScreenKeyboard;
				string text = this.textElement.m_TouchScreenKeyboard.text;
				touchScreenKeyboard.selection = new RangeInt((text != null) ? text.Length : 0, 0);
			}
			TouchScreenTextEditorEventHandler.activeTouchScreenKeyboard = this.textElement.m_TouchScreenKeyboard;
		}

		public override void HandleEventBubbleUp(EventBase evt)
		{
			base.HandleEventBubbleUp(evt);
			bool flag = !this.editingUtilities.TouchScreenKeyboardShouldBeUsed() || this.textElement.edition.isReadOnly;
			if (!flag)
			{
				if (!(evt is PointerDownEvent))
				{
					PointerUpEvent pointerUpEvent = evt as PointerUpEvent;
					if (pointerUpEvent == null)
					{
						if (!(evt is FocusInEvent))
						{
							FocusOutEvent focusOutEvent = evt as FocusOutEvent;
							if (focusOutEvent != null)
							{
								this.OnFocusOutEvent(focusOutEvent);
							}
						}
						else
						{
							this.OnFocusInEvent();
						}
					}
					else
					{
						this.OnPointerUpEvent(pointerUpEvent);
					}
				}
				else
				{
					this.OnPointerDownEvent();
				}
			}
		}

		private void OnPointerDownEvent()
		{
			this.m_IsClicking = true;
			bool flag = this.textElement.m_TouchScreenKeyboard != null && this.textElement.edition.hideMobileInput;
			if (flag)
			{
				int num = this.textElement.selection.cursorIndex;
				string text = this.textElement.m_TouchScreenKeyboard.text;
				int num2 = ((text != null) ? text.Length : 0);
				bool flag2 = num < 0;
				if (flag2)
				{
					num = 0;
				}
				bool flag3 = num > num2;
				if (flag3)
				{
					num = num2;
				}
				this.textElement.m_TouchScreenKeyboard.selection = new RangeInt(num, 0);
			}
		}

		private void OnPointerUpEvent(PointerUpEvent evt)
		{
			this.m_IsClicking = false;
			evt.StopPropagation();
		}

		private void OnFocusInEvent()
		{
			bool flag = this.textElement.m_TouchScreenKeyboard != null;
			if (!flag)
			{
				this.OpenTouchScreenKeyboard();
				bool flag2 = this.textElement.m_TouchScreenKeyboard != null;
				if (flag2)
				{
					this.PollTouchScreenKeyboard();
				}
				this.textElement.edition.SaveValueAndText();
				Action<bool> updateScrollOffset = this.textElement.edition.UpdateScrollOffset;
				if (updateScrollOffset != null)
				{
					updateScrollOffset(false);
				}
			}
		}

		private void OnFocusOutEvent(FocusOutEvent evt)
		{
			TextElement textElement = (TextElement)evt.target;
			TextElement textElement2 = textElement.focusController.m_LastPendingFocusedElement as TextElement;
			bool flag = textElement2 == textElement || textElement2 == null || textElement2.edition.keyboardType != textElement.edition.keyboardType || textElement2.edition.multiline != textElement.edition.multiline || textElement2.edition.hideMobileInput != textElement.edition.hideMobileInput;
			if (flag)
			{
				this.CloseTouchScreenKeyboard();
			}
			else
			{
				this.textElement.m_TouchScreenKeyboard = null;
				IVisualElementScheduledItem touchKeyboardPoller = this.m_TouchKeyboardPoller;
				if (touchKeyboardPoller != null)
				{
					touchKeyboardPoller.Pause();
				}
			}
		}

		private IVisualElementScheduledItem m_TouchKeyboardPoller = null;

		private bool m_TouchKeyboardAllowsInPlaceEditing = false;

		private bool m_IsClicking = false;
	}
}
