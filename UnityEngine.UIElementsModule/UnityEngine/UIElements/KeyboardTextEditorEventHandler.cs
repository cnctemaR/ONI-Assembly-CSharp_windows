using System;

namespace UnityEngine.UIElements
{
	internal class KeyboardTextEditorEventHandler : TextEditorEventHandler
	{
		public KeyboardTextEditorEventHandler(TextElement textElement, TextEditingUtilities editingUtilities)
			: base(textElement, editingUtilities)
		{
			editingUtilities.multiline = textElement.edition.multiline;
		}

		public override void HandleEventBubbleUp(EventBase evt)
		{
			base.HandleEventBubbleUp(evt);
			KeyDownEvent keyDownEvent = evt as KeyDownEvent;
			if (keyDownEvent == null)
			{
				ValidateCommandEvent validateCommandEvent = evt as ValidateCommandEvent;
				if (validateCommandEvent == null)
				{
					ExecuteCommandEvent executeCommandEvent = evt as ExecuteCommandEvent;
					if (executeCommandEvent == null)
					{
						FocusEvent focusEvent = evt as FocusEvent;
						if (focusEvent == null)
						{
							BlurEvent blurEvent = evt as BlurEvent;
							if (blurEvent == null)
							{
								NavigationMoveEvent navigationMoveEvent = evt as NavigationMoveEvent;
								if (navigationMoveEvent == null)
								{
									NavigationSubmitEvent navigationSubmitEvent = evt as NavigationSubmitEvent;
									if (navigationSubmitEvent == null)
									{
										NavigationCancelEvent navigationCancelEvent = evt as NavigationCancelEvent;
										if (navigationCancelEvent == null)
										{
											IMEEvent imeevent = evt as IMEEvent;
											if (imeevent != null)
											{
												this.OnIMEInput(imeevent);
											}
										}
										else
										{
											this.OnNavigationEvent<NavigationCancelEvent>(navigationCancelEvent);
										}
									}
									else
									{
										this.OnNavigationEvent<NavigationSubmitEvent>(navigationSubmitEvent);
									}
								}
								else
								{
									this.OnNavigationEvent<NavigationMoveEvent>(navigationMoveEvent);
								}
							}
							else
							{
								this.OnBlur(blurEvent);
							}
						}
						else
						{
							this.OnFocus(focusEvent);
						}
					}
					else
					{
						this.OnExecuteCommandEvent(executeCommandEvent);
					}
				}
				else
				{
					this.OnValidateCommandEvent(validateCommandEvent);
				}
			}
			else
			{
				this.OnKeyDown(keyDownEvent);
			}
		}

		private void OnFocus(FocusEvent _)
		{
			GUIUtility.imeCompositionMode = IMECompositionMode.On;
			this.textElement.edition.SaveValueAndText();
		}

		private void OnBlur(BlurEvent _)
		{
			GUIUtility.imeCompositionMode = IMECompositionMode.Auto;
		}

		private void OnIMEInput(IMEEvent _)
		{
			bool isCompositionActive = this.editingUtilities.isCompositionActive;
			bool flag = this.editingUtilities.UpdateImeState() || isCompositionActive != this.editingUtilities.isCompositionActive;
			if (flag)
			{
				this.UpdateLabel(true);
			}
		}

		private void OnKeyDown(KeyDownEvent evt)
		{
			bool flag = !this.textElement.hasFocus;
			if (!flag)
			{
				this.m_Changed = false;
				bool flag2 = false;
				bool flag3 = this.editingUtilities.HandleKeyEvent(evt.keyCode, evt.modifiers);
				if (flag3)
				{
					bool flag4 = this.textElement.text != this.editingUtilities.text;
					if (flag4)
					{
						this.m_Changed = true;
					}
					evt.StopPropagation();
				}
				else
				{
					char c = evt.character;
					bool flag5 = evt.actionKey && (!evt.altKey || c == '\0');
					if (flag5)
					{
						return;
					}
					bool flag6 = (evt.keyCode >= KeyCode.F1 && evt.keyCode <= KeyCode.F15) || (evt.keyCode >= KeyCode.F16 && evt.keyCode <= KeyCode.F24);
					if (flag6)
					{
						return;
					}
					bool flag7 = evt.altKey && c == '\0';
					if (flag7)
					{
						return;
					}
					bool flag8 = c == '\t' && evt.keyCode == KeyCode.None && evt.modifiers == EventModifiers.None;
					if (flag8)
					{
						return;
					}
					bool flag9 = evt.keyCode == KeyCode.Tab || (evt.keyCode == KeyCode.Tab && evt.character == '\t' && evt.modifiers == EventModifiers.Shift);
					if (flag9)
					{
						bool flag10 = !this.textElement.edition.multiline || evt.shiftKey;
						if (flag10)
						{
							bool flag11 = evt.ShouldSendNavigationMoveEvent();
							if (flag11)
							{
								this.textElement.focusController.FocusNextInDirection(this.textElement, evt.shiftKey ? VisualElementFocusChangeDirection.left : VisualElementFocusChangeDirection.right);
								evt.StopPropagation();
							}
							return;
						}
						bool flag12 = !evt.ShouldSendNavigationMoveEvent();
						if (flag12)
						{
							return;
						}
					}
					bool flag13 = !this.textElement.edition.multiline && (evt.keyCode == KeyCode.KeypadEnter || evt.keyCode == KeyCode.Return);
					if (flag13)
					{
						this.m_ShouldInvokeUpdateValue = true;
					}
					evt.StopPropagation();
					bool flag14 = (this.textElement.edition.multiline ? (c == '\n' && evt.shiftKey) : ((c == '\n' || c == '\r' || c == '\n') && !evt.altKey));
					if (flag14)
					{
						this.ApplyTextIfNeeded();
						Action moveFocusToCompositeRoot = this.textElement.edition.MoveFocusToCompositeRoot;
						if (moveFocusToCompositeRoot != null)
						{
							moveFocusToCompositeRoot();
						}
						return;
					}
					bool flag15 = evt.keyCode == KeyCode.Escape;
					if (flag15)
					{
						this.textElement.edition.RestoreValueAndText();
						Action updateValueFromText = this.textElement.edition.UpdateValueFromText;
						if (updateValueFromText != null)
						{
							updateValueFromText();
						}
						Action moveFocusToCompositeRoot2 = this.textElement.edition.MoveFocusToCompositeRoot;
						if (moveFocusToCompositeRoot2 != null)
						{
							moveFocusToCompositeRoot2();
						}
					}
					bool flag16 = evt.keyCode == KeyCode.Tab;
					if (flag16)
					{
						c = '\t';
					}
					bool flag17 = !this.textElement.edition.AcceptCharacter(c);
					if (flag17)
					{
						this.ApplyTextIfNeeded();
						return;
					}
					bool flag18 = c >= ' ' || evt.keyCode == KeyCode.Tab || (this.textElement.edition.multiline && !evt.altKey && (c == '\n' || c == '\r' || c == '\n'));
					if (flag18)
					{
						this.m_Changed = this.editingUtilities.Insert(c);
					}
					else
					{
						bool isCompositionActive = this.editingUtilities.isCompositionActive;
						flag2 = true;
						bool flag19 = this.editingUtilities.UpdateImeState() || isCompositionActive != this.editingUtilities.isCompositionActive;
						if (flag19)
						{
							this.m_Changed = true;
						}
					}
				}
				bool flag20 = this.m_Changed || this.m_ShouldInvokeUpdateValue;
				if (flag20)
				{
					this.UpdateLabel(flag2);
				}
				Action<bool> updateScrollOffset = this.textElement.edition.UpdateScrollOffset;
				if (updateScrollOffset != null)
				{
					updateScrollOffset(evt.keyCode == KeyCode.Backspace);
				}
			}
		}

		private void ApplyTextIfNeeded()
		{
			bool shouldInvokeUpdateValue = this.m_ShouldInvokeUpdateValue;
			if (shouldInvokeUpdateValue)
			{
				Action updateValueFromText = this.textElement.edition.UpdateValueFromText;
				if (updateValueFromText != null)
				{
					updateValueFromText();
				}
				this.m_ShouldInvokeUpdateValue = false;
			}
		}

		private void UpdateLabel(bool generatePreview)
		{
			string text = this.editingUtilities.text;
			bool flag = this.editingUtilities.UpdateImeState();
			bool flag2 = flag && this.editingUtilities.ShouldUpdateImeWindowPosition();
			if (flag2)
			{
				this.editingUtilities.SetImeWindowPosition(new Vector2(this.textElement.worldBound.x, this.textElement.worldBound.y));
			}
			string text2 = (generatePreview ? this.editingUtilities.GeneratePreviewString(this.textElement.enableRichText) : this.editingUtilities.text);
			this.textElement.edition.UpdateText(text2);
			bool flag3 = !this.textElement.edition.isDelayed || this.m_ShouldInvokeUpdateValue;
			if (flag3)
			{
				Action updateValueFromText = this.textElement.edition.UpdateValueFromText;
				if (updateValueFromText != null)
				{
					updateValueFromText();
				}
				this.m_ShouldInvokeUpdateValue = false;
			}
			bool flag4 = flag;
			if (flag4)
			{
				this.editingUtilities.text = text;
				this.editingUtilities.EnableCursorPreviewState();
			}
			this.textElement.uitkTextHandle.ComputeSettingsAndUpdate();
		}

		private void OnValidateCommandEvent(ValidateCommandEvent evt)
		{
			bool flag = !this.textElement.hasFocus;
			if (!flag)
			{
				string commandName = evt.commandName;
				string text = commandName;
				if (!(text == "Copy") && !(text == "SelectAll"))
				{
					if (!(text == "Cut"))
					{
						if (!(text == "Paste"))
						{
							if (!(text == "Delete"))
							{
								if (!(text == "UndoRedoPerformed"))
								{
								}
							}
						}
						else
						{
							bool flag2 = !this.editingUtilities.CanPaste();
							if (flag2)
							{
								return;
							}
						}
					}
					else
					{
						bool flag3 = !this.textElement.selection.HasSelection();
						if (flag3)
						{
							return;
						}
					}
					evt.StopPropagation();
				}
			}
		}

		private void OnExecuteCommandEvent(ExecuteCommandEvent evt)
		{
			bool flag = !this.textElement.hasFocus;
			if (!flag)
			{
				this.m_Changed = false;
				bool flag2 = false;
				string text = this.editingUtilities.text;
				string commandName = evt.commandName;
				string text2 = commandName;
				if (!(text2 == "OnLostFocus"))
				{
					if (!(text2 == "Cut"))
					{
						if (!(text2 == "Paste"))
						{
							if (text2 == "Delete")
							{
								this.editingUtilities.Cut();
								flag2 = true;
								evt.StopPropagation();
							}
						}
						else
						{
							this.editingUtilities.Paste();
							flag2 = true;
							evt.StopPropagation();
						}
					}
					else
					{
						this.editingUtilities.Cut();
						flag2 = true;
						evt.StopPropagation();
					}
					bool flag3 = flag2;
					if (flag3)
					{
						bool flag4 = text != this.editingUtilities.text;
						if (flag4)
						{
							this.m_Changed = true;
						}
						evt.StopPropagation();
					}
					bool changed = this.m_Changed;
					if (changed)
					{
						this.UpdateLabel(true);
					}
					Action<bool> updateScrollOffset = this.textElement.edition.UpdateScrollOffset;
					if (updateScrollOffset != null)
					{
						updateScrollOffset(false);
					}
				}
				else
				{
					evt.StopPropagation();
				}
			}
		}

		private void OnNavigationEvent<TEvent>(NavigationEventBase<TEvent> evt) where TEvent : NavigationEventBase<TEvent>, new()
		{
			bool flag = evt.deviceType == NavigationDeviceType.Keyboard || evt.deviceType == NavigationDeviceType.Unknown;
			if (flag)
			{
				evt.StopPropagation();
				this.textElement.focusController.IgnoreEvent(evt);
			}
		}

		internal bool m_Changed;

		internal bool m_ShouldInvokeUpdateValue;

		private const int k_LineFeed = 10;

		private const int k_Space = 32;
	}
}
