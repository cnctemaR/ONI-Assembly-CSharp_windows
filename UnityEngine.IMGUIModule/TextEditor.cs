using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;
using UnityEngine.TextCore.Text;

namespace UnityEngine
{
	public class TextEditor
	{
		public bool isMultiline
		{
			get
			{
				return this.m_TextEditing.multiline;
			}
			set
			{
				this.m_TextEditing.multiline = value;
			}
		}

		public bool hasHorizontalCursor
		{
			get
			{
				return this.m_TextSelecting.hasHorizontalCursorPos;
			}
			set
			{
				this.m_TextSelecting.hasHorizontalCursorPos = value;
			}
		}

		public bool showCursor
		{
			get
			{
				return this.m_TextSelecting.revealCursor;
			}
			set
			{
				this.m_TextSelecting.revealCursor = value;
			}
		}

		internal bool m_HasFocus
		{
			get
			{
				return this.focus;
			}
			set
			{
				this.focus = value;
			}
		}

		[Obsolete("Please use 'text' instead of 'content'", true)]
		public GUIContent content
		{
			get
			{
				throw new NotImplementedException("Please use 'text' instead of 'content'");
			}
			set
			{
				throw new NotImplementedException("Please use 'text' instead of 'content'");
			}
		}

		public string text
		{
			get
			{
				return this.m_TextEditing.text;
			}
			set
			{
				string text = value ?? "";
				bool flag = this.m_TextEditing.text == text;
				if (!flag)
				{
					this.m_TextEditing.SetTextWithoutNotify(text);
					this.m_Content.SetTextWithoutNotify(text);
					this.textWithWhitespace = text;
					this.UpdateTextHandle();
				}
			}
		}

		internal string textWithWhitespace
		{
			get
			{
				return string.IsNullOrEmpty(this.m_TextWithWhitespace) ? GUIContent.k_ZeroWidthSpace : this.m_TextWithWhitespace;
			}
			set
			{
				this.m_TextWithWhitespace = value + GUIContent.k_ZeroWidthSpace;
			}
		}

		public Rect position { get; set; }

		internal virtual Rect localPosition
		{
			get
			{
				return this.style.padding.Remove(this.position);
			}
		}

		public int cursorIndex
		{
			get
			{
				return this.m_TextSelecting.cursorIndex;
			}
			set
			{
				this.m_TextSelecting.cursorIndex = value;
			}
		}

		internal int stringCursorIndex
		{
			get
			{
				return this.m_TextEditing.stringCursorIndex;
			}
			set
			{
				this.m_TextEditing.stringCursorIndex = value;
			}
		}

		public int selectIndex
		{
			get
			{
				return this.m_TextSelecting.selectIndex;
			}
			set
			{
				this.m_TextSelecting.selectIndex = value;
			}
		}

		internal int stringSelectIndex
		{
			get
			{
				return this.m_TextEditing.stringSelectIndex;
			}
			set
			{
				this.m_TextEditing.stringSelectIndex = value;
			}
		}

		public TextEditor.DblClickSnapping doubleClickSnapping
		{
			get
			{
				return this.m_TextSelecting.dblClickSnap;
			}
			set
			{
				this.m_TextSelecting.dblClickSnap = value;
			}
		}

		public int altCursorPosition
		{
			get
			{
				return this.m_TextSelecting.iAltCursorPos;
			}
			set
			{
				this.m_TextSelecting.iAltCursorPos = value;
			}
		}

		[RequiredByNativeCode]
		public TextEditor()
		{
			GUIStyle none = GUIStyle.none;
			this.m_TextHandle = IMGUITextHandle.GetTextHandle(none, this.position, this.textWithWhitespace, Color.white);
			this.m_TextHandle.AddToPermanentCacheAndGenerateMesh();
			this.m_TextSelecting = new TextSelectingUtilities(this.m_TextHandle);
			this.m_TextEditing = new TextEditingUtilities(this.m_TextSelecting, this.m_TextHandle, this.m_Content.text);
			this.m_Content.OnTextChanged += this.OnContentTextChangedHandle;
			TextEditingUtilities textEditing = this.m_TextEditing;
			textEditing.OnTextChanged = (Action)Delegate.Combine(textEditing.OnTextChanged, new Action(this.OnTextChangedHandle));
			this.style = none;
			TextSelectingUtilities textSelecting = this.m_TextSelecting;
			textSelecting.OnCursorIndexChange = (Action)Delegate.Combine(textSelecting.OnCursorIndexChange, new Action(this.OnCursorIndexChange));
			TextSelectingUtilities textSelecting2 = this.m_TextSelecting;
			textSelecting2.OnSelectIndexChange = (Action)Delegate.Combine(textSelecting2.OnSelectIndexChange, new Action(this.OnSelectIndexChange));
		}

		private void OnTextChangedHandle()
		{
			this.m_Content.SetTextWithoutNotify(this.text);
			this.textWithWhitespace = this.text;
			this.UpdateTextHandle();
		}

		private void OnContentTextChangedHandle()
		{
			this.text = this.m_Content.text;
			this.textWithWhitespace = this.text;
		}

		public void OnFocus()
		{
			this.m_HasFocus = true;
			this.m_TextSelecting.OnFocus(true);
		}

		public void OnLostFocus()
		{
			this.m_HasFocus = false;
		}

		public bool HasClickedOnLink(Vector2 mousePosition, out string linkData)
		{
			Vector2 vector = mousePosition + this.scrollOffset;
			linkData = "";
			int num = this.m_TextHandle.FindIntersectingLink(vector - new Vector2(this.position.x, this.position.y), true);
			bool flag = num < 0;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				LinkInfo linkInfo = this.m_TextHandle.textInfo.linkInfo[num];
				bool flag3 = linkInfo.linkId != null && linkInfo.linkIdLength > 0;
				if (flag3)
				{
					linkData = new string(linkInfo.linkId);
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
			}
			return flag2;
		}

		public bool HasClickedOnHREF(Vector2 mousePosition, out string href)
		{
			Vector2 vector = mousePosition + this.scrollOffset;
			href = "";
			int num = this.m_TextHandle.FindIntersectingLink(vector - new Vector2(this.position.x, this.position.y), true);
			bool flag = num < 0;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				LinkInfo linkInfo = this.m_TextHandle.textInfo.linkInfo[num];
				bool flag3 = linkInfo.hashCode == 2535353;
				if (flag3)
				{
					bool flag4 = linkInfo.linkId != null && linkInfo.linkIdLength > 0;
					if (flag4)
					{
						href = new string(linkInfo.linkId);
						bool flag5 = !href.StartsWith("href");
						if (flag5)
						{
							return false;
						}
						bool flag6 = href.StartsWith("href=\"") || href.StartsWith("href='");
						if (flag6)
						{
							href = href.Substring(6, href.Length - 7);
						}
						else
						{
							href = href.Substring(5, href.Length - 6);
						}
						bool flag7 = Uri.IsWellFormedUriString(href, UriKind.Absolute);
						if (flag7)
						{
							return true;
						}
					}
				}
				flag2 = false;
			}
			return flag2;
		}

		public bool HandleKeyEvent(Event e)
		{
			return this.m_TextEditing.HandleKeyEvent(e) || this.m_TextSelecting.HandleKeyEvent(e);
		}

		public bool DeleteLineBack()
		{
			return this.m_TextEditing.DeleteLineBack();
		}

		public bool DeleteWordBack()
		{
			return this.m_TextEditing.DeleteWordBack();
		}

		public bool DeleteWordForward()
		{
			return this.m_TextEditing.DeleteWordForward();
		}

		public bool Delete()
		{
			return this.m_TextEditing.Delete();
		}

		public bool CanPaste()
		{
			return this.m_TextEditing.CanPaste();
		}

		public bool Backspace()
		{
			return this.m_TextEditing.Backspace();
		}

		public void SelectAll()
		{
			this.m_TextSelecting.SelectAll();
		}

		public void SelectNone()
		{
			this.m_TextSelecting.SelectNone();
		}

		public bool hasSelection
		{
			get
			{
				return this.m_TextSelecting.hasSelection;
			}
		}

		public string SelectedText
		{
			get
			{
				return this.m_TextSelecting.selectedText;
			}
		}

		public bool DeleteSelection()
		{
			return this.m_TextEditing.DeleteSelection();
		}

		public void ReplaceSelection(string replace)
		{
			this.m_TextEditing.ReplaceSelection(replace);
		}

		public void Insert(char c)
		{
			this.m_TextEditing.Insert(c);
		}

		public void MoveSelectionToAltCursor()
		{
			this.m_TextEditing.MoveSelectionToAltCursor();
		}

		public void MoveRight()
		{
			this.m_TextSelecting.MoveRight();
		}

		public void MoveLeft()
		{
			this.m_TextSelecting.MoveLeft();
		}

		public void MoveUp()
		{
			this.m_TextSelecting.MoveUp();
		}

		public void MoveDown()
		{
			this.m_TextSelecting.MoveDown();
		}

		public void MoveLineStart()
		{
			this.m_TextSelecting.MoveLineStart();
		}

		public void MoveLineEnd()
		{
			this.m_TextSelecting.MoveLineEnd();
		}

		public void MoveGraphicalLineStart()
		{
			this.m_TextSelecting.MoveGraphicalLineStart();
		}

		public void MoveGraphicalLineEnd()
		{
			this.m_TextSelecting.MoveGraphicalLineEnd();
		}

		public void MoveTextStart()
		{
			this.m_TextSelecting.MoveTextStart();
		}

		public void MoveTextEnd()
		{
			this.m_TextSelecting.MoveTextEnd();
		}

		public void MoveParagraphForward()
		{
			this.m_TextSelecting.MoveParagraphForward();
		}

		public void MoveParagraphBackward()
		{
			this.m_TextSelecting.MoveParagraphBackward();
		}

		public void MoveCursorToPosition(Vector2 cursorPosition)
		{
			this.MoveCursorToPosition_Internal(cursorPosition, Event.current.shift);
		}

		protected internal void MoveCursorToPosition_Internal(Vector2 cursorPosition, bool shift)
		{
			this.m_TextSelecting.MoveCursorToPosition_Internal(this.GetLocalCursorPosition(cursorPosition), shift);
		}

		public void MoveAltCursorToPosition(Vector2 cursorPosition)
		{
			this.m_TextSelecting.MoveAltCursorToPosition(this.GetLocalCursorPosition(cursorPosition));
		}

		public bool IsOverSelection(Vector2 cursorPosition)
		{
			return this.m_TextSelecting.IsOverSelection(this.GetLocalCursorPosition(cursorPosition));
		}

		public void SelectToPosition(Vector2 cursorPosition)
		{
			this.m_TextSelecting.SelectToPosition(this.GetLocalCursorPosition(cursorPosition));
		}

		private Vector2 GetLocalCursorPosition(Vector2 cursorPosition)
		{
			return cursorPosition - this.style.Internal_GetTextRectOffset(this.position, this.m_Content, new Vector2(this.m_TextHandle.preferredSize.x, (this.m_TextHandle.preferredSize.y > 0f) ? this.m_TextHandle.preferredSize.y : this.style.lineHeight)) + this.scrollOffset;
		}

		public void SelectLeft()
		{
			this.m_TextSelecting.SelectLeft();
		}

		public void SelectRight()
		{
			this.m_TextSelecting.SelectRight();
		}

		public void SelectUp()
		{
			this.m_TextSelecting.SelectUp();
		}

		public void SelectDown()
		{
			this.m_TextSelecting.SelectDown();
		}

		public void SelectTextEnd()
		{
			this.m_TextSelecting.SelectTextEnd();
		}

		public void SelectTextStart()
		{
			this.m_TextSelecting.SelectTextStart();
		}

		public void MouseDragSelectsWholeWords(bool on)
		{
			this.m_TextSelecting.MouseDragSelectsWholeWords(on);
		}

		public void DblClickSnap(TextEditor.DblClickSnapping snapping)
		{
			this.m_TextSelecting.DblClickSnap(snapping);
		}

		public void MoveWordRight()
		{
			this.m_TextSelecting.MoveWordRight();
		}

		public void MoveToStartOfNextWord()
		{
			this.m_TextSelecting.MoveToStartOfNextWord();
		}

		public void MoveToEndOfPreviousWord()
		{
			this.m_TextSelecting.MoveToEndOfPreviousWord();
		}

		public void SelectToStartOfNextWord()
		{
			this.m_TextSelecting.SelectToStartOfNextWord();
		}

		public void SelectToEndOfPreviousWord()
		{
			this.m_TextSelecting.SelectToEndOfPreviousWord();
		}

		public int FindStartOfNextWord(int p)
		{
			return this.m_TextSelecting.FindStartOfNextWord(p);
		}

		public void MoveWordLeft()
		{
			this.m_TextSelecting.MoveWordLeft();
		}

		public void SelectWordRight()
		{
			this.m_TextSelecting.SelectWordRight();
		}

		public void SelectWordLeft()
		{
			this.m_TextSelecting.SelectWordLeft();
		}

		public void ExpandSelectGraphicalLineStart()
		{
			this.m_TextSelecting.ExpandSelectGraphicalLineStart();
		}

		public void ExpandSelectGraphicalLineEnd()
		{
			this.m_TextSelecting.ExpandSelectGraphicalLineEnd();
		}

		public void SelectGraphicalLineStart()
		{
			this.m_TextSelecting.SelectGraphicalLineStart();
		}

		public void SelectGraphicalLineEnd()
		{
			this.m_TextSelecting.SelectGraphicalLineEnd();
		}

		public void SelectParagraphForward()
		{
			this.m_TextSelecting.SelectParagraphForward();
		}

		public void SelectParagraphBackward()
		{
			this.m_TextSelecting.SelectParagraphBackward();
		}

		public void SelectCurrentWord()
		{
			this.m_TextSelecting.SelectCurrentWord();
		}

		public void SelectCurrentParagraph()
		{
			this.m_TextSelecting.SelectCurrentParagraph();
		}

		public void UpdateScrollOffsetIfNeeded(Event evt)
		{
			bool flag = evt.type != EventType.Repaint && evt.type != EventType.Layout;
			if (flag)
			{
				this.UpdateScrollOffset();
			}
		}

		internal void UpdateTextHandle()
		{
			this.m_TextHandle = IMGUITextHandle.GetTextHandle(this.style, this.style.padding.Remove(this.position), this.textWithWhitespace, Color.white);
			this.m_TextHandle.AddToPermanentCacheAndGenerateMesh();
			this.m_TextEditing.textHandle = this.m_TextHandle;
			this.m_TextSelecting.textHandle = this.m_TextHandle;
		}

		[VisibleToOtherModules]
		internal void UpdateScrollOffset()
		{
			float num = this.scrollOffset.x;
			float num2 = this.scrollOffset.y;
			this.graphicalCursorPos = this.style.GetCursorPixelPosition(new Rect(0f, 0f, this.position.width, this.position.height), this.m_Content, this.m_TextSelecting.cursorIndexNoValidation);
			Rect rect = this.style.padding.Remove(this.position);
			Vector2 vector = this.graphicalCursorPos;
			vector.x -= (float)this.style.padding.left;
			vector.y -= (float)this.style.padding.top;
			Vector2 vector2 = (this.previousContentSize = this.style.GetPreferredSize(this.m_Content.textWithWhitespace, this.position));
			bool flag = vector2.x < rect.width;
			if (flag)
			{
				num = 0f;
			}
			else
			{
				bool showCursor = this.showCursor;
				if (showCursor)
				{
					bool flag2 = vector.x > this.scrollOffset.x + rect.width - 1f;
					if (flag2)
					{
						num = vector.x - rect.width + 1f;
					}
					else
					{
						bool flag3 = vector.x < this.scrollOffset.x;
						if (flag3)
						{
							num = Mathf.Max(vector.x, 0f);
						}
						else
						{
							bool flag4 = this.previousContentSize.x != vector2.x && vector.x < rect.x + Math.Abs(vector2.x + 1f - rect.width);
							if (flag4)
							{
								num = Mathf.Max(rect.width - vector.x, 0f);
							}
						}
					}
				}
			}
			bool flag5 = Mathf.Round(vector2.y) <= Mathf.Round(rect.height) || rect.height == 0f;
			if (flag5)
			{
				num2 = 0f;
			}
			else
			{
				bool flag6 = this.showCursor && Math.Abs(this.lastCursorPos.y - vector.y) > 0.05f;
				if (flag6)
				{
					bool flag7 = vector.y + this.style.lineHeight > this.scrollOffset.y + rect.height;
					if (flag7)
					{
						num2 = vector.y - rect.height + this.style.lineHeight;
					}
					else
					{
						bool flag8 = vector.y < this.style.lineHeight + this.scrollOffset.y;
						if (flag8)
						{
							num2 = vector.y - this.style.lineHeight;
						}
					}
				}
			}
			bool flag9 = this.scrollOffset.x != num || this.scrollOffset.y != num2;
			if (flag9)
			{
				this.scrollOffset = new Vector2(num, (num2 < 0f) ? 0f : num2);
			}
			this.lastCursorPos = vector;
		}

		public void DrawCursor(string newText)
		{
			string text = this.text;
			int cursorIndex = this.cursorIndex;
			bool flag = GUIUtility.compositionString.Length > 0;
			if (flag)
			{
				this.m_Content.text = newText.Substring(0, this.cursorIndex) + GUIUtility.compositionString + newText.Substring(this.selectIndex);
			}
			else
			{
				this.m_Content.text = newText;
			}
			this.graphicalCursorPos = this.style.GetCursorPixelPosition(this.position, this.m_Content, cursorIndex) + new Vector2(0f, this.style.lineHeight);
			Vector2 contentOffset = this.style.contentOffset;
			this.style.contentOffset -= this.scrollOffset;
			this.style.Internal_clipOffset = this.scrollOffset;
			GUIUtility.compositionCursorPos = GUIClip.UnclipToWindow(this.graphicalCursorPos - this.scrollOffset);
			bool flag2 = GUIUtility.compositionString.Length > 0;
			if (flag2)
			{
				this.style.DrawWithTextSelection(this.position, this.m_Content, this.controlID, this.cursorIndex, this.cursorIndex + GUIUtility.compositionString.Length, true);
			}
			else
			{
				this.style.DrawWithTextSelection(this.position, this.m_Content, this.controlID, this.cursorIndex, this.selectIndex);
			}
			bool flag3 = this.m_TextSelecting.iAltCursorPos != -1;
			if (flag3)
			{
				this.style.DrawCursor(this.position, this.m_Content, this.controlID, this.m_TextSelecting.iAltCursorPos);
			}
			this.style.contentOffset = contentOffset;
			this.style.Internal_clipOffset = Vector2.zero;
			this.m_Content.text = text;
		}

		public void SaveBackup()
		{
			this.oldText = this.text;
			this.oldPos = this.cursorIndex;
			this.oldSelectPos = this.selectIndex;
		}

		public void Undo()
		{
			this.m_Content.text = this.oldText;
			this.cursorIndex = this.oldPos;
			this.selectIndex = this.oldSelectPos;
		}

		public bool Cut()
		{
			bool flag = this.isPasswordField;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this.m_TextEditing.Cut();
				this.UpdateTextHandle();
				flag2 = flag3;
			}
			return flag2;
		}

		public void Copy()
		{
			bool flag = this.isPasswordField;
			if (!flag)
			{
				this.m_TextSelecting.Copy();
			}
		}

		internal Rect[] GetHyperlinksRect()
		{
			Rect[] hyperlinkRects = this.style.GetHyperlinkRects(this.m_TextHandle, this.localPosition);
			for (int i = 0; i < hyperlinkRects.Length; i++)
			{
				Rect[] array = hyperlinkRects;
				int num = i;
				array[num].position = array[num].position - this.scrollOffset;
			}
			return hyperlinkRects;
		}

		public bool Paste()
		{
			return this.m_TextEditing.Paste();
		}

		public void DetectFocusChange()
		{
			this.OnDetectFocusChange();
		}

		internal virtual void OnDetectFocusChange()
		{
			bool flag = this.m_HasFocus && this.controlID != GUIUtility.keyboardControl;
			if (flag)
			{
				this.OnLostFocus();
			}
			bool flag2 = !this.m_HasFocus && this.controlID == GUIUtility.keyboardControl;
			if (flag2)
			{
				this.OnFocus();
			}
		}

		internal virtual void OnCursorIndexChange()
		{
			this.UpdateScrollOffset();
		}

		internal virtual void OnSelectIndexChange()
		{
			this.UpdateScrollOffset();
		}

		private readonly GUIContent m_Content = new GUIContent();

		private TextSelectingUtilities m_TextSelecting;

		internal TextEditingUtilities m_TextEditing;

		internal IMGUITextHandle m_TextHandle;

		public TouchScreenKeyboard keyboardOnScreen = null;

		public int controlID = 0;

		public GUIStyle style;

		[Obsolete("'multiline' has been deprecated. Changes to this member will not be observed. Use 'isMultiline' instead.", true)]
		public bool multiline;

		[Obsolete("'hasHorizontalCursorPos' has been deprecated. Changes to this member will not be observed. Use 'hasHorizontalCursor' instead.", true)]
		public bool hasHorizontalCursorPos = false;

		public bool isPasswordField = false;

		public Vector2 scrollOffset;

		[Obsolete("'revealCursor' has been deprecated. Changes to this member will not be observed. Use 'showCursor' instead.", true)]
		public bool revealCursor;

		private bool focus;

		private string m_TextWithWhitespace;

		public Vector2 graphicalCursorPos;

		public Vector2 graphicalSelectCursorPos;

		private Vector2 lastCursorPos = Vector2.zero;

		private Vector2 previousContentSize = Vector2.zero;

		private string oldText;

		private int oldPos;

		private int oldSelectPos;

		public enum DblClickSnapping : byte
		{
			WORDS,
			PARAGRAPHS
		}
	}
}
