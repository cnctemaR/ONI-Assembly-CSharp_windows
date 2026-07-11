using System;

namespace UnityEngine.Experimental.UIElements
{
	internal class KeyboardTextEditorEventHandler : TextEditorEventHandler
	{
		public KeyboardTextEditorEventHandler(TextEditorEngine editorEngine, ITextInputField textInputField)
			: base(editorEngine, textInputField)
		{
		}

		public override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);
			if (evt.GetEventTypeId() == EventBase<MouseDownEvent>.TypeId())
			{
				this.OnMouseDown(evt as MouseDownEvent);
			}
			else if (evt.GetEventTypeId() == EventBase<MouseUpEvent>.TypeId())
			{
				this.OnMouseUp(evt as MouseUpEvent);
			}
			else if (evt.GetEventTypeId() == EventBase<MouseMoveEvent>.TypeId())
			{
				this.OnMouseMove(evt as MouseMoveEvent);
			}
			else if (evt.GetEventTypeId() == EventBase<KeyDownEvent>.TypeId())
			{
				this.OnKeyDown(evt as KeyDownEvent);
			}
			else if (evt.GetEventTypeId() == EventBase<ValidateCommandEvent>.TypeId())
			{
				this.OnValidateCommandEvent(evt as ValidateCommandEvent);
			}
			else if (evt.GetEventTypeId() == EventBase<ExecuteCommandEvent>.TypeId())
			{
				this.OnExecuteCommandEvent(evt as ExecuteCommandEvent);
			}
		}

		private void OnMouseDown(MouseDownEvent evt)
		{
			base.textInputField.SyncTextEngine();
			this.m_Changed = false;
			if (!base.textInputField.hasFocus)
			{
				base.editorEngine.m_HasFocus = true;
				base.editorEngine.MoveCursorToPosition_Internal(evt.localMousePosition, evt.button == 0 && evt.shiftKey);
				if (evt.button == 0)
				{
					base.textInputField.TakeMouseCapture();
				}
				evt.StopPropagation();
			}
			else if (evt.button == 0)
			{
				if (evt.clickCount == 2 && base.textInputField.doubleClickSelectsWord)
				{
					base.editorEngine.SelectCurrentWord();
					base.editorEngine.DblClickSnap(TextEditor.DblClickSnapping.WORDS);
					base.editorEngine.MouseDragSelectsWholeWords(true);
					this.m_DragToPosition = false;
				}
				else if (evt.clickCount == 3 && base.textInputField.tripleClickSelectsLine)
				{
					base.editorEngine.SelectCurrentParagraph();
					base.editorEngine.MouseDragSelectsWholeWords(true);
					base.editorEngine.DblClickSnap(TextEditor.DblClickSnapping.PARAGRAPHS);
					this.m_DragToPosition = false;
				}
				else
				{
					base.editorEngine.MoveCursorToPosition_Internal(evt.localMousePosition, evt.shiftKey);
					this.m_SelectAllOnMouseUp = false;
				}
				base.textInputField.TakeMouseCapture();
				evt.StopPropagation();
			}
			else if (evt.button == 1)
			{
				if (base.editorEngine.cursorIndex == base.editorEngine.selectIndex)
				{
					base.editorEngine.MoveCursorToPosition_Internal(evt.localMousePosition, false);
				}
				this.m_SelectAllOnMouseUp = false;
				this.m_DragToPosition = false;
			}
			base.editorEngine.UpdateScrollOffset();
		}

		private void OnMouseUp(MouseUpEvent evt)
		{
			if (evt.button == 0)
			{
				if (base.textInputField.HasMouseCapture())
				{
					base.textInputField.SyncTextEngine();
					this.m_Changed = false;
					if (this.m_Dragged && this.m_DragToPosition)
					{
						base.editorEngine.MoveSelectionToAltCursor();
					}
					else if (this.m_PostponeMove)
					{
						base.editorEngine.MoveCursorToPosition_Internal(evt.localMousePosition, evt.shiftKey);
					}
					else if (this.m_SelectAllOnMouseUp)
					{
						this.m_SelectAllOnMouseUp = false;
					}
					base.editorEngine.MouseDragSelectsWholeWords(false);
					base.textInputField.ReleaseMouseCapture();
					this.m_DragToPosition = true;
					this.m_Dragged = false;
					this.m_PostponeMove = false;
					evt.StopPropagation();
					base.editorEngine.UpdateScrollOffset();
				}
			}
		}

		private void OnMouseMove(MouseMoveEvent evt)
		{
			if (evt.button == 0)
			{
				if (base.textInputField.HasMouseCapture())
				{
					base.textInputField.SyncTextEngine();
					this.m_Changed = false;
					if (!evt.shiftKey && base.editorEngine.hasSelection && this.m_DragToPosition)
					{
						base.editorEngine.MoveAltCursorToPosition(evt.localMousePosition);
					}
					else
					{
						if (evt.shiftKey)
						{
							base.editorEngine.MoveCursorToPosition_Internal(evt.localMousePosition, evt.shiftKey);
						}
						else
						{
							base.editorEngine.SelectToPosition(evt.localMousePosition);
						}
						this.m_DragToPosition = false;
						this.m_SelectAllOnMouseUp = !base.editorEngine.hasSelection;
					}
					this.m_Dragged = true;
					evt.StopPropagation();
					base.editorEngine.UpdateScrollOffset();
				}
			}
		}

		private void OnKeyDown(KeyDownEvent evt)
		{
			if (base.textInputField.hasFocus)
			{
				base.textInputField.SyncTextEngine();
				this.m_Changed = false;
				if (base.editorEngine.HandleKeyEvent(evt.imguiEvent))
				{
					if (base.textInputField.text != base.editorEngine.text)
					{
						this.m_Changed = true;
					}
					evt.StopPropagation();
				}
				else
				{
					if (evt.keyCode == KeyCode.Tab || evt.character == '\t')
					{
						return;
					}
					evt.StopPropagation();
					char character = evt.character;
					if (character == '\n' && !base.editorEngine.multiline && !evt.altKey)
					{
						return;
					}
					if (!base.textInputField.AcceptCharacter(character))
					{
						return;
					}
					Font font = base.editorEngine.style.font;
					if ((font != null && font.HasCharacter(character)) || character == '\n')
					{
						base.editorEngine.Insert(character);
						this.m_Changed = true;
					}
					else if (character == '\0')
					{
						if (!string.IsNullOrEmpty(Input.compositionString))
						{
							base.editorEngine.ReplaceSelection("");
							this.m_Changed = true;
						}
					}
				}
				if (this.m_Changed)
				{
					base.editorEngine.text = base.textInputField.CullString(base.editorEngine.text);
					base.textInputField.UpdateText(base.editorEngine.text);
				}
				base.editorEngine.UpdateScrollOffset();
			}
		}

		private void OnValidateCommandEvent(ValidateCommandEvent evt)
		{
			if (base.textInputField.hasFocus)
			{
				base.textInputField.SyncTextEngine();
				this.m_Changed = false;
				string commandName = evt.commandName;
				if (commandName != null)
				{
					if (!(commandName == "Cut") && !(commandName == "Copy"))
					{
						if (!(commandName == "Paste"))
						{
							if (!(commandName == "SelectAll") && !(commandName == "Delete"))
							{
								if (!(commandName == "UndoRedoPerformed"))
								{
								}
							}
						}
						else if (!base.editorEngine.CanPaste())
						{
							return;
						}
					}
					else if (!base.editorEngine.hasSelection)
					{
						return;
					}
				}
				evt.StopPropagation();
			}
		}

		private void OnExecuteCommandEvent(ExecuteCommandEvent evt)
		{
			if (base.textInputField.hasFocus)
			{
				base.textInputField.SyncTextEngine();
				this.m_Changed = false;
				bool flag = false;
				string text = base.editorEngine.text;
				if (base.textInputField.hasFocus)
				{
					string commandName = evt.commandName;
					if (commandName != null)
					{
						if (commandName == "OnLostFocus")
						{
							evt.StopPropagation();
							return;
						}
						if (!(commandName == "Cut"))
						{
							if (commandName == "Copy")
							{
								base.editorEngine.Copy();
								evt.StopPropagation();
								return;
							}
							if (!(commandName == "Paste"))
							{
								if (commandName == "SelectAll")
								{
									base.editorEngine.SelectAll();
									evt.StopPropagation();
									return;
								}
								if (commandName == "Delete")
								{
									if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
									{
										base.editorEngine.Delete();
									}
									else
									{
										base.editorEngine.Cut();
									}
									flag = true;
								}
							}
							else
							{
								base.editorEngine.Paste();
								flag = true;
							}
						}
						else
						{
							base.editorEngine.Cut();
							flag = true;
						}
					}
					if (flag)
					{
						if (text != base.editorEngine.text)
						{
							this.m_Changed = true;
						}
						evt.StopPropagation();
					}
					if (this.m_Changed)
					{
						base.editorEngine.text = base.textInputField.CullString(base.editorEngine.text);
						base.textInputField.UpdateText(base.editorEngine.text);
						evt.StopPropagation();
					}
					base.editorEngine.UpdateScrollOffset();
				}
			}
		}

		public void PreDrawCursor(string newText)
		{
			base.textInputField.SyncTextEngine();
			this.m_PreDrawCursorText = base.editorEngine.text;
			int num = base.editorEngine.cursorIndex;
			if (!string.IsNullOrEmpty(Input.compositionString))
			{
				base.editorEngine.text = newText.Substring(0, base.editorEngine.cursorIndex) + Input.compositionString + newText.Substring(base.editorEngine.selectIndex);
				num += Input.compositionString.Length;
			}
			else
			{
				base.editorEngine.text = newText;
			}
			base.editorEngine.text = base.textInputField.CullString(base.editorEngine.text);
			num = Math.Min(num, base.editorEngine.text.Length);
			base.editorEngine.graphicalCursorPos = base.editorEngine.style.GetCursorPixelPosition(base.editorEngine.localPosition, new GUIContent(base.editorEngine.text), num);
		}

		public void PostDrawCursor()
		{
			base.editorEngine.text = this.m_PreDrawCursorText;
		}

		internal bool m_Changed;

		private bool m_Dragged;

		private bool m_DragToPosition = true;

		private bool m_PostponeMove;

		private bool m_SelectAllOnMouseUp = true;

		private string m_PreDrawCursorText;
	}
}
