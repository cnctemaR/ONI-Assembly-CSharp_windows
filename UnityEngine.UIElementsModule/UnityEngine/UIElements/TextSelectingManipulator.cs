using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
	internal class TextSelectingManipulator
	{
		internal bool isClicking
		{
			get
			{
				return this.m_IsClicking;
			}
			private set
			{
				bool flag = this.m_IsClicking == value;
				if (!flag)
				{
					this.m_IsClicking = value;
				}
			}
		}

		public TextSelectingManipulator(TextElement textElement)
		{
			this.m_TextElement = textElement;
			this.m_SelectingUtilities = new TextSelectingUtilities(this.m_TextElement.uitkTextHandle);
			TextSelectingUtilities selectingUtilities = this.m_SelectingUtilities;
			selectingUtilities.OnCursorIndexChange = (Action)Delegate.Combine(selectingUtilities.OnCursorIndexChange, new Action(this.OnCursorIndexChange));
			TextSelectingUtilities selectingUtilities2 = this.m_SelectingUtilities;
			selectingUtilities2.OnSelectIndexChange = (Action)Delegate.Combine(selectingUtilities2.OnSelectIndexChange, new Action(this.OnSelectIndexChange));
			TextSelectingUtilities selectingUtilities3 = this.m_SelectingUtilities;
			selectingUtilities3.OnRevealCursorChange = (Action)Delegate.Combine(selectingUtilities3.OnRevealCursorChange, new Action(this.OnRevealCursor));
		}

		internal int cursorIndex
		{
			get
			{
				TextSelectingUtilities selectingUtilities = this.m_SelectingUtilities;
				return (selectingUtilities != null) ? selectingUtilities.cursorIndex : (-1);
			}
			set
			{
				this.m_SelectingUtilities.cursorIndex = value;
			}
		}

		internal int selectIndex
		{
			get
			{
				TextSelectingUtilities selectingUtilities = this.m_SelectingUtilities;
				return (selectingUtilities != null) ? selectingUtilities.selectIndex : (-1);
			}
			set
			{
				this.m_SelectingUtilities.selectIndex = value;
			}
		}

		private void OnRevealCursor()
		{
			this.m_TextElement.IncrementVersion(VersionChangeType.Repaint);
		}

		private void OnSelectIndexChange()
		{
			this.m_TextElement.IncrementVersion(VersionChangeType.Repaint);
			bool flag = this.HasSelection() && this.m_TextElement.focusController != null;
			if (flag)
			{
				this.m_TextElement.focusController.selectedTextElement = this.m_TextElement;
			}
			bool revealCursor = this.m_SelectingUtilities.revealCursor;
			if (revealCursor)
			{
				Action<bool> updateScrollOffset = this.m_TextElement.edition.UpdateScrollOffset;
				if (updateScrollOffset != null)
				{
					updateScrollOffset(false);
				}
			}
		}

		private void OnCursorIndexChange()
		{
			this.m_TextElement.IncrementVersion(VersionChangeType.Repaint);
			bool flag = this.HasSelection() && this.m_TextElement.focusController != null;
			if (flag)
			{
				this.m_TextElement.focusController.selectedTextElement = this.m_TextElement;
			}
			bool revealCursor = this.m_SelectingUtilities.revealCursor;
			if (revealCursor)
			{
				Action<bool> updateScrollOffset = this.m_TextElement.edition.UpdateScrollOffset;
				if (updateScrollOffset != null)
				{
					updateScrollOffset(false);
				}
			}
		}

		internal bool RevealCursor()
		{
			return this.m_SelectingUtilities.revealCursor;
		}

		internal bool HasSelection()
		{
			return this.m_SelectingUtilities.hasSelection;
		}

		internal bool HasFocus()
		{
			return this.m_TextElement.hasFocus;
		}

		internal void HandleEventBubbleUp(EventBase evt)
		{
			bool flag = evt is BlurEvent;
			if (flag)
			{
				this.m_TextElement.uitkTextHandle.RemoveFromPermanentCache();
			}
			else
			{
				bool flag2 = (!(evt is PointerMoveEvent) && !(evt is MouseMoveEvent)) || this.isClicking;
				if (flag2)
				{
					this.m_TextElement.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
				}
			}
			if (!(evt is FocusEvent))
			{
				if (!(evt is BlurEvent))
				{
					ValidateCommandEvent validateCommandEvent = evt as ValidateCommandEvent;
					if (validateCommandEvent == null)
					{
						ExecuteCommandEvent executeCommandEvent = evt as ExecuteCommandEvent;
						if (executeCommandEvent == null)
						{
							KeyDownEvent keyDownEvent = evt as KeyDownEvent;
							if (keyDownEvent == null)
							{
								PointerDownEvent pointerDownEvent = evt as PointerDownEvent;
								if (pointerDownEvent == null)
								{
									PointerMoveEvent pointerMoveEvent = evt as PointerMoveEvent;
									if (pointerMoveEvent == null)
									{
										PointerUpEvent pointerUpEvent = evt as PointerUpEvent;
										if (pointerUpEvent != null)
										{
											this.OnPointerUpEvent(pointerUpEvent);
										}
									}
									else
									{
										this.OnPointerMoveEvent(pointerMoveEvent);
									}
								}
								else
								{
									this.OnPointerDownEvent(pointerDownEvent);
								}
							}
							else
							{
								this.OnKeyDown(keyDownEvent);
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
					this.OnBlurEvent();
				}
			}
			else
			{
				this.OnFocusEvent();
			}
		}

		private void OnFocusEvent()
		{
			this.selectAllOnMouseUp = false;
			bool isClicking = this.isClicking;
			if (isClicking)
			{
				this.selectAllOnMouseUp = this.m_TextElement.selection.selectAllOnMouseUp;
			}
			this.m_SelectingUtilities.OnFocus(this.m_TextElement.selection.selectAllOnFocus && !this.isClicking);
		}

		private void OnBlurEvent()
		{
			this.selectAllOnMouseUp = this.m_TextElement.selection.selectAllOnMouseUp;
		}

		private void OnKeyDown(KeyDownEvent evt)
		{
			bool flag = !this.m_TextElement.hasFocus;
			if (!flag)
			{
				bool flag2 = this.m_SelectingUtilities.HandleKeyEvent(evt.keyCode, evt.modifiers);
				if (flag2)
				{
					evt.StopPropagation();
				}
			}
		}

		private void OnPointerDownEvent(PointerDownEvent evt)
		{
			Vector3 vector = evt.localPosition - this.m_TextElement.contentRect.min;
			bool flag = evt.button == 0;
			if (flag)
			{
				bool flag2 = evt.timestamp - this.m_LastMouseDownTimeStamp < (long)Event.GetDoubleClickTime();
				if (flag2)
				{
					this.m_ConsecutiveMouseDownCount++;
				}
				else
				{
					this.m_ConsecutiveMouseDownCount = 1;
				}
				bool flag3 = this.m_ConsecutiveMouseDownCount == 2 && this.m_TextElement.selection.doubleClickSelectsWord;
				if (flag3)
				{
					bool flag4 = this.cursorIndex == 0 && this.cursorIndex != this.selectIndex;
					if (flag4)
					{
						this.m_SelectingUtilities.MoveCursorToPosition_Internal(vector, evt.shiftKey);
					}
					this.m_SelectingUtilities.SelectCurrentWord();
					this.m_SelectingUtilities.MouseDragSelectsWholeWords(true);
					this.m_SelectingUtilities.DblClickSnap(TextEditor.DblClickSnapping.WORDS);
				}
				else
				{
					bool flag5 = this.m_ConsecutiveMouseDownCount == 3 && this.m_TextElement.selection.tripleClickSelectsLine;
					if (flag5)
					{
						this.m_SelectingUtilities.SelectCurrentParagraph();
						this.m_SelectingUtilities.MouseDragSelectsWholeWords(true);
						this.m_SelectingUtilities.DblClickSnap(TextEditor.DblClickSnapping.PARAGRAPHS);
					}
					else
					{
						this.m_SelectingUtilities.MoveCursorToPosition_Internal(vector, evt.shiftKey);
						Action<bool> updateScrollOffset = this.m_TextElement.edition.UpdateScrollOffset;
						if (updateScrollOffset != null)
						{
							updateScrollOffset(false);
						}
						this.m_SelectingUtilities.MouseDragSelectsWholeWords(false);
						this.m_SelectingUtilities.DblClickSnap(TextEditor.DblClickSnapping.WORDS);
					}
				}
				this.m_LastMouseDownTimeStamp = evt.timestamp;
				this.isClicking = true;
				this.m_TextElement.CapturePointer(evt.pointerId);
				this.m_ClickStartPosition = vector;
				evt.StopPropagation();
			}
		}

		internal void ResetClickCount()
		{
			this.m_ConsecutiveMouseDownCount = 0;
		}

		private void OnPointerMoveEvent(PointerMoveEvent evt)
		{
			bool flag = !this.isClicking;
			if (!flag)
			{
				Vector3 vector = evt.localPosition - this.m_TextElement.contentRect.min;
				this.m_Dragged = this.m_Dragged || this.MoveDistanceQualifiesForDrag(this.m_ClickStartPosition, vector);
				bool dragged = this.m_Dragged;
				if (dragged)
				{
					this.m_SelectingUtilities.SelectToPosition(vector);
					Action<bool> updateScrollOffset = this.m_TextElement.edition.UpdateScrollOffset;
					if (updateScrollOffset != null)
					{
						updateScrollOffset(false);
					}
					this.selectAllOnMouseUp = this.m_TextElement.selection.selectAllOnMouseUp && !this.m_SelectingUtilities.hasSelection;
				}
				evt.StopPropagation();
			}
		}

		private void OnPointerUpEvent(PointerUpEvent evt)
		{
			bool flag = evt.button != 0 || !this.isClicking;
			if (!flag)
			{
				bool flag2 = this.selectAllOnMouseUp;
				if (flag2)
				{
					this.m_SelectingUtilities.SelectAll();
				}
				this.selectAllOnMouseUp = false;
				this.m_Dragged = false;
				this.isClicking = false;
				this.m_TextElement.ReleasePointer(evt.pointerId);
				evt.StopPropagation();
			}
		}

		private void OnValidateCommandEvent(ValidateCommandEvent evt)
		{
			bool flag = !this.m_TextElement.hasFocus;
			if (!flag)
			{
				string commandName = evt.commandName;
				string text = commandName;
				if (!(text == "Cut") && !(text == "Paste") && !(text == "Delete") && !(text == "UndoRedoPerformed"))
				{
					if (!(text == "Copy"))
					{
						if (!(text == "SelectAll"))
						{
						}
					}
					else
					{
						bool flag2 = !this.m_SelectingUtilities.hasSelection;
						if (flag2)
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
			bool flag = !this.m_TextElement.hasFocus;
			if (!flag)
			{
				string commandName = evt.commandName;
				string text = commandName;
				if (!(text == "OnLostFocus"))
				{
					if (!(text == "Copy"))
					{
						if (text == "SelectAll")
						{
							this.m_SelectingUtilities.SelectAll();
							evt.StopPropagation();
						}
					}
					else
					{
						this.m_SelectingUtilities.Copy();
						evt.StopPropagation();
					}
				}
				else
				{
					evt.StopPropagation();
				}
			}
		}

		private bool MoveDistanceQualifiesForDrag(Vector2 start, Vector2 current)
		{
			return (start - current).sqrMagnitude >= 16f;
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
		internal TextSelectingUtilities m_SelectingUtilities;

		private bool selectAllOnMouseUp;

		private TextElement m_TextElement;

		private Vector2 m_ClickStartPosition;

		private bool m_Dragged;

		private bool m_IsClicking;

		private const int k_DragThresholdSqr = 16;

		private int m_ConsecutiveMouseDownCount;

		private long m_LastMouseDownTimeStamp = 0L;
	}
}
