using System;
using System.Collections.Generic;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	public class TextEditor
	{
		[RequiredByNativeCode]
		public TextEditor()
		{
		}

		[Obsolete("Please use 'text' instead of 'content'", false)]
		public GUIContent content
		{
			get
			{
				return this.m_Content;
			}
			set
			{
				this.m_Content = value;
			}
		}

		public string text
		{
			get
			{
				return this.m_Content.text;
			}
			set
			{
				this.m_Content.text = value ?? string.Empty;
				this.EnsureValidCodePointIndex(ref this.m_CursorIndex);
				this.EnsureValidCodePointIndex(ref this.m_SelectIndex);
			}
		}

		public Rect position
		{
			get
			{
				return this.m_Position;
			}
			set
			{
				if (!(this.m_Position == value))
				{
					this.m_Position = value;
					this.UpdateScrollOffset();
				}
			}
		}

		internal virtual Rect localPosition
		{
			[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
			get
			{
				return this.position;
			}
		}

		public int cursorIndex
		{
			get
			{
				return this.m_CursorIndex;
			}
			set
			{
				int cursorIndex = this.m_CursorIndex;
				this.m_CursorIndex = value;
				this.EnsureValidCodePointIndex(ref this.m_CursorIndex);
				if (this.m_CursorIndex != cursorIndex)
				{
					this.m_RevealCursor = true;
					this.OnCursorIndexChange();
				}
			}
		}

		public int selectIndex
		{
			get
			{
				return this.m_SelectIndex;
			}
			set
			{
				int selectIndex = this.m_SelectIndex;
				this.m_SelectIndex = value;
				this.EnsureValidCodePointIndex(ref this.m_SelectIndex);
				if (this.m_SelectIndex != selectIndex)
				{
					this.OnSelectIndexChange();
				}
			}
		}

		private void ClearCursorPos()
		{
			this.hasHorizontalCursorPos = false;
			this.m_iAltCursorPos = -1;
		}

		public TextEditor.DblClickSnapping doubleClickSnapping
		{
			get
			{
				return this.m_DblClickSnap;
			}
			set
			{
				this.m_DblClickSnap = value;
			}
		}

		public int altCursorPosition
		{
			get
			{
				return this.m_iAltCursorPos;
			}
			set
			{
				this.m_iAltCursorPos = value;
			}
		}

		public void OnFocus()
		{
			if (this.multiline)
			{
				int num = 0;
				this.selectIndex = num;
				this.cursorIndex = num;
			}
			else
			{
				this.SelectAll();
			}
			this.m_HasFocus = true;
		}

		public void OnLostFocus()
		{
			this.m_HasFocus = false;
			this.scrollOffset = Vector2.zero;
		}

		private void GrabGraphicalCursorPos()
		{
			if (!this.hasHorizontalCursorPos)
			{
				this.graphicalCursorPos = this.style.GetCursorPixelPosition(this.localPosition, this.m_Content, this.cursorIndex);
				this.graphicalSelectCursorPos = this.style.GetCursorPixelPosition(this.localPosition, this.m_Content, this.selectIndex);
				this.hasHorizontalCursorPos = false;
			}
		}

		public bool HandleKeyEvent(Event e)
		{
			this.InitKeyActions();
			EventModifiers modifiers = e.modifiers;
			e.modifiers &= ~EventModifiers.CapsLock;
			bool flag;
			if (TextEditor.s_Keyactions.ContainsKey(e))
			{
				TextEditor.TextEditOp textEditOp = TextEditor.s_Keyactions[e];
				this.PerformOperation(textEditOp);
				e.modifiers = modifiers;
				flag = true;
			}
			else
			{
				e.modifiers = modifiers;
				flag = false;
			}
			return flag;
		}

		public bool DeleteLineBack()
		{
			bool flag;
			if (this.hasSelection)
			{
				this.DeleteSelection();
				flag = true;
			}
			else
			{
				int num = this.cursorIndex;
				int num2 = num;
				while (num2-- != 0)
				{
					if (this.text[num2] == '\n')
					{
						num = num2 + 1;
						break;
					}
				}
				if (num2 == -1)
				{
					num = 0;
				}
				if (this.cursorIndex != num)
				{
					this.m_Content.text = this.text.Remove(num, this.cursorIndex - num);
					int num3 = num;
					this.cursorIndex = num3;
					this.selectIndex = num3;
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		public bool DeleteWordBack()
		{
			bool flag;
			if (this.hasSelection)
			{
				this.DeleteSelection();
				flag = true;
			}
			else
			{
				int num = this.FindEndOfPreviousWord(this.cursorIndex);
				if (this.cursorIndex != num)
				{
					this.m_Content.text = this.text.Remove(num, this.cursorIndex - num);
					int num2 = num;
					this.cursorIndex = num2;
					this.selectIndex = num2;
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		public bool DeleteWordForward()
		{
			bool flag;
			if (this.hasSelection)
			{
				this.DeleteSelection();
				flag = true;
			}
			else
			{
				int num = this.FindStartOfNextWord(this.cursorIndex);
				if (this.cursorIndex < this.text.Length)
				{
					this.m_Content.text = this.text.Remove(this.cursorIndex, num - this.cursorIndex);
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		public bool Delete()
		{
			bool flag;
			if (this.hasSelection)
			{
				this.DeleteSelection();
				flag = true;
			}
			else if (this.cursorIndex < this.text.Length)
			{
				this.m_Content.text = this.text.Remove(this.cursorIndex, this.NextCodePointIndex(this.cursorIndex) - this.cursorIndex);
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool CanPaste()
		{
			return GUIUtility.systemCopyBuffer.Length != 0;
		}

		public bool Backspace()
		{
			bool flag;
			if (this.hasSelection)
			{
				this.DeleteSelection();
				flag = true;
			}
			else if (this.cursorIndex > 0)
			{
				int num = this.PreviousCodePointIndex(this.cursorIndex);
				this.m_Content.text = this.text.Remove(num, this.cursorIndex - num);
				int num2 = num;
				this.cursorIndex = num2;
				this.selectIndex = num2;
				this.ClearCursorPos();
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public void SelectAll()
		{
			this.cursorIndex = 0;
			this.selectIndex = this.text.Length;
			this.ClearCursorPos();
		}

		public void SelectNone()
		{
			this.selectIndex = this.cursorIndex;
			this.ClearCursorPos();
		}

		public bool hasSelection
		{
			get
			{
				return this.cursorIndex != this.selectIndex;
			}
		}

		public string SelectedText
		{
			get
			{
				string text;
				if (this.cursorIndex == this.selectIndex)
				{
					text = "";
				}
				else if (this.cursorIndex < this.selectIndex)
				{
					text = this.text.Substring(this.cursorIndex, this.selectIndex - this.cursorIndex);
				}
				else
				{
					text = this.text.Substring(this.selectIndex, this.cursorIndex - this.selectIndex);
				}
				return text;
			}
		}

		public bool DeleteSelection()
		{
			bool flag;
			if (this.cursorIndex == this.selectIndex)
			{
				flag = false;
			}
			else
			{
				if (this.cursorIndex < this.selectIndex)
				{
					this.m_Content.text = this.text.Substring(0, this.cursorIndex) + this.text.Substring(this.selectIndex, this.text.Length - this.selectIndex);
					this.selectIndex = this.cursorIndex;
				}
				else
				{
					this.m_Content.text = this.text.Substring(0, this.selectIndex) + this.text.Substring(this.cursorIndex, this.text.Length - this.cursorIndex);
					this.cursorIndex = this.selectIndex;
				}
				this.ClearCursorPos();
				flag = true;
			}
			return flag;
		}

		public void ReplaceSelection(string replace)
		{
			this.DeleteSelection();
			this.m_Content.text = this.text.Insert(this.cursorIndex, replace);
			this.selectIndex = (this.cursorIndex += replace.Length);
			this.ClearCursorPos();
		}

		public void Insert(char c)
		{
			this.ReplaceSelection(c.ToString());
		}

		public void MoveSelectionToAltCursor()
		{
			if (this.m_iAltCursorPos != -1)
			{
				int iAltCursorPos = this.m_iAltCursorPos;
				string selectedText = this.SelectedText;
				this.m_Content.text = this.text.Insert(iAltCursorPos, selectedText);
				if (iAltCursorPos < this.cursorIndex)
				{
					this.cursorIndex += selectedText.Length;
					this.selectIndex += selectedText.Length;
				}
				this.DeleteSelection();
				int num = iAltCursorPos;
				this.cursorIndex = num;
				this.selectIndex = num;
				this.ClearCursorPos();
			}
		}

		public void MoveRight()
		{
			this.ClearCursorPos();
			if (this.selectIndex == this.cursorIndex)
			{
				this.cursorIndex = this.NextCodePointIndex(this.cursorIndex);
				this.DetectFocusChange();
				this.selectIndex = this.cursorIndex;
			}
			else if (this.selectIndex > this.cursorIndex)
			{
				this.cursorIndex = this.selectIndex;
			}
			else
			{
				this.selectIndex = this.cursorIndex;
			}
		}

		public void MoveLeft()
		{
			if (this.selectIndex == this.cursorIndex)
			{
				this.cursorIndex = this.PreviousCodePointIndex(this.cursorIndex);
				this.selectIndex = this.cursorIndex;
			}
			else if (this.selectIndex > this.cursorIndex)
			{
				this.selectIndex = this.cursorIndex;
			}
			else
			{
				this.cursorIndex = this.selectIndex;
			}
			this.ClearCursorPos();
		}

		public void MoveUp()
		{
			if (this.selectIndex < this.cursorIndex)
			{
				this.selectIndex = this.cursorIndex;
			}
			else
			{
				this.cursorIndex = this.selectIndex;
			}
			this.GrabGraphicalCursorPos();
			this.graphicalCursorPos.y = this.graphicalCursorPos.y - 1f;
			int cursorStringIndex = this.style.GetCursorStringIndex(this.localPosition, this.m_Content, this.graphicalCursorPos);
			this.selectIndex = cursorStringIndex;
			this.cursorIndex = cursorStringIndex;
			if (this.cursorIndex <= 0)
			{
				this.ClearCursorPos();
			}
		}

		public void MoveDown()
		{
			if (this.selectIndex > this.cursorIndex)
			{
				this.selectIndex = this.cursorIndex;
			}
			else
			{
				this.cursorIndex = this.selectIndex;
			}
			this.GrabGraphicalCursorPos();
			this.graphicalCursorPos.y = this.graphicalCursorPos.y + (this.style.lineHeight + 5f);
			int cursorStringIndex = this.style.GetCursorStringIndex(this.localPosition, this.m_Content, this.graphicalCursorPos);
			this.selectIndex = cursorStringIndex;
			this.cursorIndex = cursorStringIndex;
			if (this.cursorIndex == this.text.Length)
			{
				this.ClearCursorPos();
			}
		}

		public void MoveLineStart()
		{
			int num = ((this.selectIndex >= this.cursorIndex) ? this.cursorIndex : this.selectIndex);
			int num2 = num;
			int num3;
			while (num2-- != 0)
			{
				if (this.text[num2] == '\n')
				{
					num3 = num2 + 1;
					this.cursorIndex = num3;
					this.selectIndex = num3;
					return;
				}
			}
			num3 = 0;
			this.cursorIndex = num3;
			this.selectIndex = num3;
		}

		public void MoveLineEnd()
		{
			int num = ((this.selectIndex <= this.cursorIndex) ? this.cursorIndex : this.selectIndex);
			int i = num;
			int length = this.text.Length;
			int num2;
			while (i < length)
			{
				if (this.text[i] == '\n')
				{
					num2 = i;
					this.cursorIndex = num2;
					this.selectIndex = num2;
					return;
				}
				i++;
			}
			num2 = length;
			this.cursorIndex = num2;
			this.selectIndex = num2;
		}

		public void MoveGraphicalLineStart()
		{
			int graphicalLineStart = this.GetGraphicalLineStart((this.cursorIndex >= this.selectIndex) ? this.selectIndex : this.cursorIndex);
			this.selectIndex = graphicalLineStart;
			this.cursorIndex = graphicalLineStart;
		}

		public void MoveGraphicalLineEnd()
		{
			int graphicalLineEnd = this.GetGraphicalLineEnd((this.cursorIndex <= this.selectIndex) ? this.selectIndex : this.cursorIndex);
			this.selectIndex = graphicalLineEnd;
			this.cursorIndex = graphicalLineEnd;
		}

		public void MoveTextStart()
		{
			int num = 0;
			this.cursorIndex = num;
			this.selectIndex = num;
		}

		public void MoveTextEnd()
		{
			int length = this.text.Length;
			this.cursorIndex = length;
			this.selectIndex = length;
		}

		private int IndexOfEndOfLine(int startIndex)
		{
			int num = this.text.IndexOf('\n', startIndex);
			return (num == -1) ? this.text.Length : num;
		}

		public void MoveParagraphForward()
		{
			this.cursorIndex = ((this.cursorIndex <= this.selectIndex) ? this.selectIndex : this.cursorIndex);
			if (this.cursorIndex < this.text.Length)
			{
				int num = this.IndexOfEndOfLine(this.cursorIndex + 1);
				this.cursorIndex = num;
				this.selectIndex = num;
			}
		}

		public void MoveParagraphBackward()
		{
			this.cursorIndex = ((this.cursorIndex >= this.selectIndex) ? this.selectIndex : this.cursorIndex);
			if (this.cursorIndex > 1)
			{
				int num = this.text.LastIndexOf('\n', this.cursorIndex - 2) + 1;
				this.cursorIndex = num;
				this.selectIndex = num;
			}
			else
			{
				int num = 0;
				this.cursorIndex = num;
				this.selectIndex = num;
			}
		}

		public void MoveCursorToPosition(Vector2 cursorPosition)
		{
			this.MoveCursorToPosition_Internal(cursorPosition, Event.current.shift);
		}

		protected internal void MoveCursorToPosition_Internal(Vector2 cursorPosition, bool shift)
		{
			this.selectIndex = this.style.GetCursorStringIndex(this.localPosition, this.m_Content, cursorPosition + this.scrollOffset);
			if (!shift)
			{
				this.cursorIndex = this.selectIndex;
			}
			this.DetectFocusChange();
		}

		public void MoveAltCursorToPosition(Vector2 cursorPosition)
		{
			int cursorStringIndex = this.style.GetCursorStringIndex(this.localPosition, this.m_Content, cursorPosition + this.scrollOffset);
			this.m_iAltCursorPos = Mathf.Min(this.text.Length, cursorStringIndex);
			this.DetectFocusChange();
		}

		public bool IsOverSelection(Vector2 cursorPosition)
		{
			int cursorStringIndex = this.style.GetCursorStringIndex(this.localPosition, this.m_Content, cursorPosition + this.scrollOffset);
			return cursorStringIndex < Mathf.Max(this.cursorIndex, this.selectIndex) && cursorStringIndex > Mathf.Min(this.cursorIndex, this.selectIndex);
		}

		public void SelectToPosition(Vector2 cursorPosition)
		{
			if (!this.m_MouseDragSelectsWholeWords)
			{
				this.cursorIndex = this.style.GetCursorStringIndex(this.localPosition, this.m_Content, cursorPosition + this.scrollOffset);
			}
			else
			{
				int cursorStringIndex = this.style.GetCursorStringIndex(this.localPosition, this.m_Content, cursorPosition + this.scrollOffset);
				this.EnsureValidCodePointIndex(ref cursorStringIndex);
				this.EnsureValidCodePointIndex(ref this.m_DblClickInitPos);
				if (this.m_DblClickSnap == TextEditor.DblClickSnapping.WORDS)
				{
					if (cursorStringIndex < this.m_DblClickInitPos)
					{
						this.cursorIndex = this.FindEndOfClassification(cursorStringIndex, TextEditor.Direction.Backward);
						this.selectIndex = this.FindEndOfClassification(this.m_DblClickInitPos, TextEditor.Direction.Forward);
					}
					else
					{
						this.cursorIndex = this.FindEndOfClassification(cursorStringIndex, TextEditor.Direction.Forward);
						this.selectIndex = this.FindEndOfClassification(this.m_DblClickInitPos, TextEditor.Direction.Backward);
					}
				}
				else if (cursorStringIndex < this.m_DblClickInitPos)
				{
					if (cursorStringIndex > 0)
					{
						this.cursorIndex = this.text.LastIndexOf('\n', Mathf.Max(0, cursorStringIndex - 2)) + 1;
					}
					else
					{
						this.cursorIndex = 0;
					}
					this.selectIndex = this.text.LastIndexOf('\n', this.m_DblClickInitPos);
				}
				else
				{
					if (cursorStringIndex < this.text.Length)
					{
						this.cursorIndex = this.IndexOfEndOfLine(cursorStringIndex);
					}
					else
					{
						this.cursorIndex = this.text.Length;
					}
					this.selectIndex = this.text.LastIndexOf('\n', Mathf.Max(0, this.m_DblClickInitPos - 2)) + 1;
				}
			}
		}

		public void SelectLeft()
		{
			if (this.m_bJustSelected && this.cursorIndex > this.selectIndex)
			{
				int cursorIndex = this.cursorIndex;
				this.cursorIndex = this.selectIndex;
				this.selectIndex = cursorIndex;
			}
			this.m_bJustSelected = false;
			this.cursorIndex = this.PreviousCodePointIndex(this.cursorIndex);
		}

		public void SelectRight()
		{
			if (this.m_bJustSelected && this.cursorIndex < this.selectIndex)
			{
				int cursorIndex = this.cursorIndex;
				this.cursorIndex = this.selectIndex;
				this.selectIndex = cursorIndex;
			}
			this.m_bJustSelected = false;
			this.cursorIndex = this.NextCodePointIndex(this.cursorIndex);
		}

		public void SelectUp()
		{
			this.GrabGraphicalCursorPos();
			this.graphicalCursorPos.y = this.graphicalCursorPos.y - 1f;
			this.cursorIndex = this.style.GetCursorStringIndex(this.localPosition, this.m_Content, this.graphicalCursorPos);
		}

		public void SelectDown()
		{
			this.GrabGraphicalCursorPos();
			this.graphicalCursorPos.y = this.graphicalCursorPos.y + (this.style.lineHeight + 5f);
			this.cursorIndex = this.style.GetCursorStringIndex(this.localPosition, this.m_Content, this.graphicalCursorPos);
		}

		public void SelectTextEnd()
		{
			this.cursorIndex = this.text.Length;
		}

		public void SelectTextStart()
		{
			this.cursorIndex = 0;
		}

		public void MouseDragSelectsWholeWords(bool on)
		{
			this.m_MouseDragSelectsWholeWords = on;
			this.m_DblClickInitPos = this.cursorIndex;
		}

		public void DblClickSnap(TextEditor.DblClickSnapping snapping)
		{
			this.m_DblClickSnap = snapping;
		}

		private int GetGraphicalLineStart(int p)
		{
			Vector2 cursorPixelPosition = this.style.GetCursorPixelPosition(this.localPosition, this.m_Content, p);
			cursorPixelPosition.x = 0f;
			return this.style.GetCursorStringIndex(this.localPosition, this.m_Content, cursorPixelPosition);
		}

		private int GetGraphicalLineEnd(int p)
		{
			Vector2 cursorPixelPosition = this.style.GetCursorPixelPosition(this.localPosition, this.m_Content, p);
			cursorPixelPosition.x += 5000f;
			return this.style.GetCursorStringIndex(this.localPosition, this.m_Content, cursorPixelPosition);
		}

		private int FindNextSeperator(int startPos)
		{
			int length = this.text.Length;
			while (startPos < length && this.ClassifyChar(startPos) != TextEditor.CharacterType.LetterLike)
			{
				startPos = this.NextCodePointIndex(startPos);
			}
			while (startPos < length && this.ClassifyChar(startPos) == TextEditor.CharacterType.LetterLike)
			{
				startPos = this.NextCodePointIndex(startPos);
			}
			return startPos;
		}

		private int FindPrevSeperator(int startPos)
		{
			startPos = this.PreviousCodePointIndex(startPos);
			while (startPos > 0 && this.ClassifyChar(startPos) != TextEditor.CharacterType.LetterLike)
			{
				startPos = this.PreviousCodePointIndex(startPos);
			}
			int num;
			if (startPos == 0)
			{
				num = 0;
			}
			else
			{
				while (startPos > 0 && this.ClassifyChar(startPos) == TextEditor.CharacterType.LetterLike)
				{
					startPos = this.PreviousCodePointIndex(startPos);
				}
				if (this.ClassifyChar(startPos) == TextEditor.CharacterType.LetterLike)
				{
					num = startPos;
				}
				else
				{
					num = this.NextCodePointIndex(startPos);
				}
			}
			return num;
		}

		public void MoveWordRight()
		{
			this.cursorIndex = ((this.cursorIndex <= this.selectIndex) ? this.selectIndex : this.cursorIndex);
			int num = this.FindNextSeperator(this.cursorIndex);
			this.selectIndex = num;
			this.cursorIndex = num;
			this.ClearCursorPos();
		}

		public void MoveToStartOfNextWord()
		{
			this.ClearCursorPos();
			if (this.cursorIndex != this.selectIndex)
			{
				this.MoveRight();
			}
			else
			{
				int num = this.FindStartOfNextWord(this.cursorIndex);
				this.selectIndex = num;
				this.cursorIndex = num;
			}
		}

		public void MoveToEndOfPreviousWord()
		{
			this.ClearCursorPos();
			if (this.cursorIndex != this.selectIndex)
			{
				this.MoveLeft();
			}
			else
			{
				int num = this.FindEndOfPreviousWord(this.cursorIndex);
				this.selectIndex = num;
				this.cursorIndex = num;
			}
		}

		public void SelectToStartOfNextWord()
		{
			this.ClearCursorPos();
			this.cursorIndex = this.FindStartOfNextWord(this.cursorIndex);
		}

		public void SelectToEndOfPreviousWord()
		{
			this.ClearCursorPos();
			this.cursorIndex = this.FindEndOfPreviousWord(this.cursorIndex);
		}

		private TextEditor.CharacterType ClassifyChar(int index)
		{
			TextEditor.CharacterType characterType;
			if (char.IsWhiteSpace(this.text, index))
			{
				characterType = TextEditor.CharacterType.WhiteSpace;
			}
			else if (char.IsLetterOrDigit(this.text, index) || this.text[index] == '\'')
			{
				characterType = TextEditor.CharacterType.LetterLike;
			}
			else
			{
				characterType = TextEditor.CharacterType.Symbol;
			}
			return characterType;
		}

		public int FindStartOfNextWord(int p)
		{
			int length = this.text.Length;
			int num;
			if (p == length)
			{
				num = p;
			}
			else
			{
				TextEditor.CharacterType characterType = this.ClassifyChar(p);
				if (characterType != TextEditor.CharacterType.WhiteSpace)
				{
					p = this.NextCodePointIndex(p);
					while (p < length && this.ClassifyChar(p) == characterType)
					{
						p = this.NextCodePointIndex(p);
					}
				}
				else if (this.text[p] == '\t' || this.text[p] == '\n')
				{
					return this.NextCodePointIndex(p);
				}
				if (p == length)
				{
					num = p;
				}
				else
				{
					if (this.text[p] == ' ')
					{
						while (p < length && this.ClassifyChar(p) == TextEditor.CharacterType.WhiteSpace)
						{
							p = this.NextCodePointIndex(p);
						}
					}
					else if (this.text[p] == '\t' || this.text[p] == '\n')
					{
						return p;
					}
					num = p;
				}
			}
			return num;
		}

		private int FindEndOfPreviousWord(int p)
		{
			int num;
			if (p == 0)
			{
				num = p;
			}
			else
			{
				p = this.PreviousCodePointIndex(p);
				while (p > 0 && this.text[p] == ' ')
				{
					p = this.PreviousCodePointIndex(p);
				}
				TextEditor.CharacterType characterType = this.ClassifyChar(p);
				if (characterType != TextEditor.CharacterType.WhiteSpace)
				{
					while (p > 0 && this.ClassifyChar(this.PreviousCodePointIndex(p)) == characterType)
					{
						p = this.PreviousCodePointIndex(p);
					}
				}
				num = p;
			}
			return num;
		}

		public void MoveWordLeft()
		{
			this.cursorIndex = ((this.cursorIndex >= this.selectIndex) ? this.selectIndex : this.cursorIndex);
			this.cursorIndex = this.FindPrevSeperator(this.cursorIndex);
			this.selectIndex = this.cursorIndex;
		}

		public void SelectWordRight()
		{
			this.ClearCursorPos();
			int selectIndex = this.selectIndex;
			if (this.cursorIndex < this.selectIndex)
			{
				this.selectIndex = this.cursorIndex;
				this.MoveWordRight();
				this.selectIndex = selectIndex;
				this.cursorIndex = ((this.cursorIndex >= this.selectIndex) ? this.selectIndex : this.cursorIndex);
			}
			else
			{
				this.selectIndex = this.cursorIndex;
				this.MoveWordRight();
				this.selectIndex = selectIndex;
			}
		}

		public void SelectWordLeft()
		{
			this.ClearCursorPos();
			int selectIndex = this.selectIndex;
			if (this.cursorIndex > this.selectIndex)
			{
				this.selectIndex = this.cursorIndex;
				this.MoveWordLeft();
				this.selectIndex = selectIndex;
				this.cursorIndex = ((this.cursorIndex <= this.selectIndex) ? this.selectIndex : this.cursorIndex);
			}
			else
			{
				this.selectIndex = this.cursorIndex;
				this.MoveWordLeft();
				this.selectIndex = selectIndex;
			}
		}

		public void ExpandSelectGraphicalLineStart()
		{
			this.ClearCursorPos();
			if (this.cursorIndex < this.selectIndex)
			{
				this.cursorIndex = this.GetGraphicalLineStart(this.cursorIndex);
			}
			else
			{
				int cursorIndex = this.cursorIndex;
				this.cursorIndex = this.GetGraphicalLineStart(this.selectIndex);
				this.selectIndex = cursorIndex;
			}
		}

		public void ExpandSelectGraphicalLineEnd()
		{
			this.ClearCursorPos();
			if (this.cursorIndex > this.selectIndex)
			{
				this.cursorIndex = this.GetGraphicalLineEnd(this.cursorIndex);
			}
			else
			{
				int cursorIndex = this.cursorIndex;
				this.cursorIndex = this.GetGraphicalLineEnd(this.selectIndex);
				this.selectIndex = cursorIndex;
			}
		}

		public void SelectGraphicalLineStart()
		{
			this.ClearCursorPos();
			this.cursorIndex = this.GetGraphicalLineStart(this.cursorIndex);
		}

		public void SelectGraphicalLineEnd()
		{
			this.ClearCursorPos();
			this.cursorIndex = this.GetGraphicalLineEnd(this.cursorIndex);
		}

		public void SelectParagraphForward()
		{
			this.ClearCursorPos();
			bool flag = this.cursorIndex < this.selectIndex;
			if (this.cursorIndex < this.text.Length)
			{
				this.cursorIndex = this.IndexOfEndOfLine(this.cursorIndex + 1);
				if (flag && this.cursorIndex > this.selectIndex)
				{
					this.cursorIndex = this.selectIndex;
				}
			}
		}

		public void SelectParagraphBackward()
		{
			this.ClearCursorPos();
			bool flag = this.cursorIndex > this.selectIndex;
			if (this.cursorIndex > 1)
			{
				this.cursorIndex = this.text.LastIndexOf('\n', this.cursorIndex - 2) + 1;
				if (flag && this.cursorIndex < this.selectIndex)
				{
					this.cursorIndex = this.selectIndex;
				}
			}
			else
			{
				int num = 0;
				this.cursorIndex = num;
				this.selectIndex = num;
			}
		}

		public void SelectCurrentWord()
		{
			int cursorIndex = this.cursorIndex;
			if (this.cursorIndex < this.selectIndex)
			{
				this.cursorIndex = this.FindEndOfClassification(cursorIndex, TextEditor.Direction.Backward);
				this.selectIndex = this.FindEndOfClassification(cursorIndex, TextEditor.Direction.Forward);
			}
			else
			{
				this.cursorIndex = this.FindEndOfClassification(cursorIndex, TextEditor.Direction.Forward);
				this.selectIndex = this.FindEndOfClassification(cursorIndex, TextEditor.Direction.Backward);
			}
			this.ClearCursorPos();
			this.m_bJustSelected = true;
		}

		private int FindEndOfClassification(int p, TextEditor.Direction dir)
		{
			int num;
			if (this.text.Length == 0)
			{
				num = 0;
			}
			else
			{
				if (p == this.text.Length)
				{
					p = this.PreviousCodePointIndex(p);
				}
				TextEditor.CharacterType characterType = this.ClassifyChar(p);
				for (;;)
				{
					if (dir != TextEditor.Direction.Backward)
					{
						if (dir == TextEditor.Direction.Forward)
						{
							p = this.NextCodePointIndex(p);
							if (p == this.text.Length)
							{
								goto Block_7;
							}
						}
					}
					else
					{
						p = this.PreviousCodePointIndex(p);
						if (p == 0)
						{
							break;
						}
					}
					if (this.ClassifyChar(p) != characterType)
					{
						goto Block_8;
					}
				}
				return (this.ClassifyChar(0) != characterType) ? this.NextCodePointIndex(0) : 0;
				Block_7:
				return this.text.Length;
				Block_8:
				if (dir == TextEditor.Direction.Forward)
				{
					num = p;
				}
				else
				{
					num = this.NextCodePointIndex(p);
				}
			}
			return num;
		}

		public void SelectCurrentParagraph()
		{
			this.ClearCursorPos();
			int length = this.text.Length;
			if (this.cursorIndex < length)
			{
				this.cursorIndex = this.IndexOfEndOfLine(this.cursorIndex) + 1;
			}
			if (this.selectIndex != 0)
			{
				this.selectIndex = this.text.LastIndexOf('\n', this.selectIndex - 1) + 1;
			}
		}

		public void UpdateScrollOffsetIfNeeded(Event evt)
		{
			if (evt.type != EventType.Repaint && evt.type != EventType.Layout)
			{
				this.UpdateScrollOffset();
			}
		}

		[VisibleToOtherModules]
		internal void UpdateScrollOffset()
		{
			int cursorIndex = this.cursorIndex;
			this.graphicalCursorPos = this.style.GetCursorPixelPosition(new Rect(0f, 0f, this.position.width, this.position.height), this.m_Content, cursorIndex);
			Rect rect = this.style.padding.Remove(this.position);
			Vector2 vector = new Vector2(this.style.CalcSize(this.m_Content).x, this.style.CalcHeight(this.m_Content, this.position.width));
			if (vector.x < this.position.width)
			{
				this.scrollOffset.x = 0f;
			}
			else if (this.m_RevealCursor)
			{
				if (this.graphicalCursorPos.x + 1f > this.scrollOffset.x + rect.width)
				{
					this.scrollOffset.x = this.graphicalCursorPos.x - rect.width;
				}
				if (this.graphicalCursorPos.x < this.scrollOffset.x + (float)this.style.padding.left)
				{
					this.scrollOffset.x = this.graphicalCursorPos.x - (float)this.style.padding.left;
				}
			}
			if (vector.y < rect.height)
			{
				this.scrollOffset.y = 0f;
			}
			else if (this.m_RevealCursor)
			{
				if (this.graphicalCursorPos.y + this.style.lineHeight > this.scrollOffset.y + rect.height + (float)this.style.padding.top)
				{
					this.scrollOffset.y = this.graphicalCursorPos.y - rect.height - (float)this.style.padding.top + this.style.lineHeight;
				}
				if (this.graphicalCursorPos.y < this.scrollOffset.y + (float)this.style.padding.top)
				{
					this.scrollOffset.y = this.graphicalCursorPos.y - (float)this.style.padding.top;
				}
			}
			if (this.scrollOffset.y > 0f && vector.y - this.scrollOffset.y < rect.height + (float)this.style.padding.top + (float)this.style.padding.bottom)
			{
				this.scrollOffset.y = vector.y - rect.height - (float)this.style.padding.top - (float)this.style.padding.bottom;
			}
			this.scrollOffset.y = ((this.scrollOffset.y >= 0f) ? this.scrollOffset.y : 0f);
			this.m_RevealCursor = false;
		}

		public void DrawCursor(string newText)
		{
			string text = this.text;
			int num = this.cursorIndex;
			if (Input.compositionString.Length > 0)
			{
				this.m_Content.text = newText.Substring(0, this.cursorIndex) + Input.compositionString + newText.Substring(this.selectIndex);
				num += Input.compositionString.Length;
			}
			else
			{
				this.m_Content.text = newText;
			}
			this.graphicalCursorPos = this.style.GetCursorPixelPosition(new Rect(0f, 0f, this.position.width, this.position.height), this.m_Content, num);
			Vector2 contentOffset = this.style.contentOffset;
			this.style.contentOffset -= this.scrollOffset;
			this.style.Internal_clipOffset = this.scrollOffset;
			Input.compositionCursorPos = this.graphicalCursorPos + new Vector2(this.position.x, this.position.y + this.style.lineHeight) - this.scrollOffset;
			if (Input.compositionString.Length > 0)
			{
				this.style.DrawWithTextSelection(this.position, this.m_Content, this.controlID, this.cursorIndex, this.cursorIndex + Input.compositionString.Length, true);
			}
			else
			{
				this.style.DrawWithTextSelection(this.position, this.m_Content, this.controlID, this.cursorIndex, this.selectIndex);
			}
			if (this.m_iAltCursorPos != -1)
			{
				this.style.DrawCursor(this.position, this.m_Content, this.controlID, this.m_iAltCursorPos);
			}
			this.style.contentOffset = contentOffset;
			this.style.Internal_clipOffset = Vector2.zero;
			this.m_Content.text = text;
		}

		private bool PerformOperation(TextEditor.TextEditOp operation)
		{
			this.m_RevealCursor = true;
			switch (operation)
			{
			case TextEditor.TextEditOp.MoveLeft:
				this.MoveLeft();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveRight:
				this.MoveRight();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveUp:
				this.MoveUp();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveDown:
				this.MoveDown();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveLineStart:
				this.MoveLineStart();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveLineEnd:
				this.MoveLineEnd();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveTextStart:
				this.MoveTextStart();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveTextEnd:
				this.MoveTextEnd();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveGraphicalLineStart:
				this.MoveGraphicalLineStart();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveGraphicalLineEnd:
				this.MoveGraphicalLineEnd();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveWordLeft:
				this.MoveWordLeft();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveWordRight:
				this.MoveWordRight();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveParagraphForward:
				this.MoveParagraphForward();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveParagraphBackward:
				this.MoveParagraphBackward();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveToStartOfNextWord:
				this.MoveToStartOfNextWord();
				goto IL_02BA;
			case TextEditor.TextEditOp.MoveToEndOfPreviousWord:
				this.MoveToEndOfPreviousWord();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectLeft:
				this.SelectLeft();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectRight:
				this.SelectRight();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectUp:
				this.SelectUp();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectDown:
				this.SelectDown();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectTextStart:
				this.SelectTextStart();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectTextEnd:
				this.SelectTextEnd();
				goto IL_02BA;
			case TextEditor.TextEditOp.ExpandSelectGraphicalLineStart:
				this.ExpandSelectGraphicalLineStart();
				goto IL_02BA;
			case TextEditor.TextEditOp.ExpandSelectGraphicalLineEnd:
				this.ExpandSelectGraphicalLineEnd();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectGraphicalLineStart:
				this.SelectGraphicalLineStart();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectGraphicalLineEnd:
				this.SelectGraphicalLineEnd();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectWordLeft:
				this.SelectWordLeft();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectWordRight:
				this.SelectWordRight();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectToEndOfPreviousWord:
				this.SelectToEndOfPreviousWord();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectToStartOfNextWord:
				this.SelectToStartOfNextWord();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectParagraphBackward:
				this.SelectParagraphBackward();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectParagraphForward:
				this.SelectParagraphForward();
				goto IL_02BA;
			case TextEditor.TextEditOp.Delete:
				return this.Delete();
			case TextEditor.TextEditOp.Backspace:
				return this.Backspace();
			case TextEditor.TextEditOp.DeleteWordBack:
				return this.DeleteWordBack();
			case TextEditor.TextEditOp.DeleteWordForward:
				return this.DeleteWordForward();
			case TextEditor.TextEditOp.DeleteLineBack:
				return this.DeleteLineBack();
			case TextEditor.TextEditOp.Cut:
				return this.Cut();
			case TextEditor.TextEditOp.Copy:
				this.Copy();
				goto IL_02BA;
			case TextEditor.TextEditOp.Paste:
				return this.Paste();
			case TextEditor.TextEditOp.SelectAll:
				this.SelectAll();
				goto IL_02BA;
			case TextEditor.TextEditOp.SelectNone:
				this.SelectNone();
				goto IL_02BA;
			}
			Debug.Log("Unimplemented: " + operation);
			IL_02BA:
			return false;
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
			bool flag;
			if (this.isPasswordField)
			{
				flag = false;
			}
			else
			{
				this.Copy();
				flag = this.DeleteSelection();
			}
			return flag;
		}

		public void Copy()
		{
			if (this.selectIndex != this.cursorIndex)
			{
				if (!this.isPasswordField)
				{
					string text;
					if (this.cursorIndex < this.selectIndex)
					{
						text = this.text.Substring(this.cursorIndex, this.selectIndex - this.cursorIndex);
					}
					else
					{
						text = this.text.Substring(this.selectIndex, this.cursorIndex - this.selectIndex);
					}
					GUIUtility.systemCopyBuffer = text;
				}
			}
		}

		private static string ReplaceNewlinesWithSpaces(string value)
		{
			value = value.Replace("\r\n", " ");
			value = value.Replace('\n', ' ');
			value = value.Replace('\r', ' ');
			return value;
		}

		public bool Paste()
		{
			string text = GUIUtility.systemCopyBuffer;
			bool flag;
			if (text != "")
			{
				if (!this.multiline)
				{
					text = TextEditor.ReplaceNewlinesWithSpaces(text);
				}
				this.ReplaceSelection(text);
				flag = true;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		private static void MapKey(string key, TextEditor.TextEditOp action)
		{
			TextEditor.s_Keyactions[Event.KeyboardEvent(key)] = action;
		}

		private void InitKeyActions()
		{
			if (TextEditor.s_Keyactions == null)
			{
				TextEditor.s_Keyactions = new Dictionary<Event, TextEditor.TextEditOp>();
				TextEditor.MapKey("left", TextEditor.TextEditOp.MoveLeft);
				TextEditor.MapKey("right", TextEditor.TextEditOp.MoveRight);
				TextEditor.MapKey("up", TextEditor.TextEditOp.MoveUp);
				TextEditor.MapKey("down", TextEditor.TextEditOp.MoveDown);
				TextEditor.MapKey("#left", TextEditor.TextEditOp.SelectLeft);
				TextEditor.MapKey("#right", TextEditor.TextEditOp.SelectRight);
				TextEditor.MapKey("#up", TextEditor.TextEditOp.SelectUp);
				TextEditor.MapKey("#down", TextEditor.TextEditOp.SelectDown);
				TextEditor.MapKey("delete", TextEditor.TextEditOp.Delete);
				TextEditor.MapKey("backspace", TextEditor.TextEditOp.Backspace);
				TextEditor.MapKey("#backspace", TextEditor.TextEditOp.Backspace);
				if (SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX)
				{
					TextEditor.MapKey("^left", TextEditor.TextEditOp.MoveGraphicalLineStart);
					TextEditor.MapKey("^right", TextEditor.TextEditOp.MoveGraphicalLineEnd);
					TextEditor.MapKey("&left", TextEditor.TextEditOp.MoveWordLeft);
					TextEditor.MapKey("&right", TextEditor.TextEditOp.MoveWordRight);
					TextEditor.MapKey("&up", TextEditor.TextEditOp.MoveParagraphBackward);
					TextEditor.MapKey("&down", TextEditor.TextEditOp.MoveParagraphForward);
					TextEditor.MapKey("%left", TextEditor.TextEditOp.MoveGraphicalLineStart);
					TextEditor.MapKey("%right", TextEditor.TextEditOp.MoveGraphicalLineEnd);
					TextEditor.MapKey("%up", TextEditor.TextEditOp.MoveTextStart);
					TextEditor.MapKey("%down", TextEditor.TextEditOp.MoveTextEnd);
					TextEditor.MapKey("#home", TextEditor.TextEditOp.SelectTextStart);
					TextEditor.MapKey("#end", TextEditor.TextEditOp.SelectTextEnd);
					TextEditor.MapKey("#^left", TextEditor.TextEditOp.ExpandSelectGraphicalLineStart);
					TextEditor.MapKey("#^right", TextEditor.TextEditOp.ExpandSelectGraphicalLineEnd);
					TextEditor.MapKey("#^up", TextEditor.TextEditOp.SelectParagraphBackward);
					TextEditor.MapKey("#^down", TextEditor.TextEditOp.SelectParagraphForward);
					TextEditor.MapKey("#&left", TextEditor.TextEditOp.SelectWordLeft);
					TextEditor.MapKey("#&right", TextEditor.TextEditOp.SelectWordRight);
					TextEditor.MapKey("#&up", TextEditor.TextEditOp.SelectParagraphBackward);
					TextEditor.MapKey("#&down", TextEditor.TextEditOp.SelectParagraphForward);
					TextEditor.MapKey("#%left", TextEditor.TextEditOp.ExpandSelectGraphicalLineStart);
					TextEditor.MapKey("#%right", TextEditor.TextEditOp.ExpandSelectGraphicalLineEnd);
					TextEditor.MapKey("#%up", TextEditor.TextEditOp.SelectTextStart);
					TextEditor.MapKey("#%down", TextEditor.TextEditOp.SelectTextEnd);
					TextEditor.MapKey("%a", TextEditor.TextEditOp.SelectAll);
					TextEditor.MapKey("%x", TextEditor.TextEditOp.Cut);
					TextEditor.MapKey("%c", TextEditor.TextEditOp.Copy);
					TextEditor.MapKey("%v", TextEditor.TextEditOp.Paste);
					TextEditor.MapKey("^d", TextEditor.TextEditOp.Delete);
					TextEditor.MapKey("^h", TextEditor.TextEditOp.Backspace);
					TextEditor.MapKey("^b", TextEditor.TextEditOp.MoveLeft);
					TextEditor.MapKey("^f", TextEditor.TextEditOp.MoveRight);
					TextEditor.MapKey("^a", TextEditor.TextEditOp.MoveLineStart);
					TextEditor.MapKey("^e", TextEditor.TextEditOp.MoveLineEnd);
					TextEditor.MapKey("&delete", TextEditor.TextEditOp.DeleteWordForward);
					TextEditor.MapKey("&backspace", TextEditor.TextEditOp.DeleteWordBack);
					TextEditor.MapKey("%backspace", TextEditor.TextEditOp.DeleteLineBack);
				}
				else
				{
					TextEditor.MapKey("home", TextEditor.TextEditOp.MoveGraphicalLineStart);
					TextEditor.MapKey("end", TextEditor.TextEditOp.MoveGraphicalLineEnd);
					TextEditor.MapKey("%left", TextEditor.TextEditOp.MoveWordLeft);
					TextEditor.MapKey("%right", TextEditor.TextEditOp.MoveWordRight);
					TextEditor.MapKey("%up", TextEditor.TextEditOp.MoveParagraphBackward);
					TextEditor.MapKey("%down", TextEditor.TextEditOp.MoveParagraphForward);
					TextEditor.MapKey("^left", TextEditor.TextEditOp.MoveToEndOfPreviousWord);
					TextEditor.MapKey("^right", TextEditor.TextEditOp.MoveToStartOfNextWord);
					TextEditor.MapKey("^up", TextEditor.TextEditOp.MoveParagraphBackward);
					TextEditor.MapKey("^down", TextEditor.TextEditOp.MoveParagraphForward);
					TextEditor.MapKey("#^left", TextEditor.TextEditOp.SelectToEndOfPreviousWord);
					TextEditor.MapKey("#^right", TextEditor.TextEditOp.SelectToStartOfNextWord);
					TextEditor.MapKey("#^up", TextEditor.TextEditOp.SelectParagraphBackward);
					TextEditor.MapKey("#^down", TextEditor.TextEditOp.SelectParagraphForward);
					TextEditor.MapKey("#home", TextEditor.TextEditOp.SelectGraphicalLineStart);
					TextEditor.MapKey("#end", TextEditor.TextEditOp.SelectGraphicalLineEnd);
					TextEditor.MapKey("^delete", TextEditor.TextEditOp.DeleteWordForward);
					TextEditor.MapKey("^backspace", TextEditor.TextEditOp.DeleteWordBack);
					TextEditor.MapKey("%backspace", TextEditor.TextEditOp.DeleteLineBack);
					TextEditor.MapKey("^a", TextEditor.TextEditOp.SelectAll);
					TextEditor.MapKey("^x", TextEditor.TextEditOp.Cut);
					TextEditor.MapKey("^c", TextEditor.TextEditOp.Copy);
					TextEditor.MapKey("^v", TextEditor.TextEditOp.Paste);
					TextEditor.MapKey("#delete", TextEditor.TextEditOp.Cut);
					TextEditor.MapKey("^insert", TextEditor.TextEditOp.Copy);
					TextEditor.MapKey("#insert", TextEditor.TextEditOp.Paste);
				}
			}
		}

		public void DetectFocusChange()
		{
			this.OnDetectFocusChange();
		}

		internal virtual void OnDetectFocusChange()
		{
			if (this.m_HasFocus && this.controlID != GUIUtility.keyboardControl)
			{
				this.OnLostFocus();
			}
			if (!this.m_HasFocus && this.controlID == GUIUtility.keyboardControl)
			{
				this.OnFocus();
			}
		}

		internal virtual void OnCursorIndexChange()
		{
		}

		internal virtual void OnSelectIndexChange()
		{
		}

		private void ClampTextIndex(ref int index)
		{
			index = Mathf.Clamp(index, 0, this.text.Length);
		}

		private void EnsureValidCodePointIndex(ref int index)
		{
			this.ClampTextIndex(ref index);
			if (!this.IsValidCodePointIndex(index))
			{
				index = this.NextCodePointIndex(index);
			}
		}

		private bool IsValidCodePointIndex(int index)
		{
			return index >= 0 && index <= this.text.Length && (index == 0 || index == this.text.Length || !char.IsLowSurrogate(this.text[index]));
		}

		private int PreviousCodePointIndex(int index)
		{
			if (index > 0)
			{
				index--;
			}
			while (index > 0 && char.IsLowSurrogate(this.text[index]))
			{
				index--;
			}
			return index;
		}

		private int NextCodePointIndex(int index)
		{
			if (index < this.text.Length)
			{
				index++;
			}
			while (index < this.text.Length && char.IsLowSurrogate(this.text[index]))
			{
				index++;
			}
			return index;
		}

		public TouchScreenKeyboard keyboardOnScreen = null;

		public int controlID = 0;

		public GUIStyle style = GUIStyle.none;

		public bool multiline = false;

		public bool hasHorizontalCursorPos = false;

		public bool isPasswordField = false;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal bool m_HasFocus;

		public Vector2 scrollOffset = Vector2.zero;

		private GUIContent m_Content = new GUIContent();

		private Rect m_Position;

		private int m_CursorIndex = 0;

		private int m_SelectIndex = 0;

		private bool m_RevealCursor = false;

		public Vector2 graphicalCursorPos;

		public Vector2 graphicalSelectCursorPos;

		private bool m_MouseDragSelectsWholeWords = false;

		private int m_DblClickInitPos = 0;

		private TextEditor.DblClickSnapping m_DblClickSnap = TextEditor.DblClickSnapping.WORDS;

		private bool m_bJustSelected = false;

		private int m_iAltCursorPos = -1;

		private string oldText;

		private int oldPos;

		private int oldSelectPos;

		private static Dictionary<Event, TextEditor.TextEditOp> s_Keyactions;

		public enum DblClickSnapping : byte
		{
			WORDS,
			PARAGRAPHS
		}

		private enum CharacterType
		{
			LetterLike,
			Symbol,
			Symbol2,
			WhiteSpace
		}

		private enum Direction
		{
			Forward,
			Backward
		}

		private enum TextEditOp
		{
			MoveLeft,
			MoveRight,
			MoveUp,
			MoveDown,
			MoveLineStart,
			MoveLineEnd,
			MoveTextStart,
			MoveTextEnd,
			MovePageUp,
			MovePageDown,
			MoveGraphicalLineStart,
			MoveGraphicalLineEnd,
			MoveWordLeft,
			MoveWordRight,
			MoveParagraphForward,
			MoveParagraphBackward,
			MoveToStartOfNextWord,
			MoveToEndOfPreviousWord,
			SelectLeft,
			SelectRight,
			SelectUp,
			SelectDown,
			SelectTextStart,
			SelectTextEnd,
			SelectPageUp,
			SelectPageDown,
			ExpandSelectGraphicalLineStart,
			ExpandSelectGraphicalLineEnd,
			SelectGraphicalLineStart,
			SelectGraphicalLineEnd,
			SelectWordLeft,
			SelectWordRight,
			SelectToEndOfPreviousWord,
			SelectToStartOfNextWord,
			SelectParagraphBackward,
			SelectParagraphForward,
			Delete,
			Backspace,
			DeleteWordBack,
			DeleteWordForward,
			DeleteLineBack,
			Cut,
			Copy,
			Paste,
			SelectAll,
			SelectNone,
			ScrollStart,
			ScrollEnd,
			ScrollPageUp,
			ScrollPageDown
		}
	}
}
