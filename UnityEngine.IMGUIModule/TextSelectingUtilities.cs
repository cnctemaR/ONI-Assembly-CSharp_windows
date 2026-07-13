using System;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;

namespace UnityEngine
{
	internal class TextSelectingUtilities
	{
		public bool hasSelection
		{
			get
			{
				return this.cursorIndex != this.selectIndex;
			}
		}

		public bool revealCursor
		{
			get
			{
				return this.m_RevealCursor;
			}
			set
			{
				bool flag = this.m_RevealCursor != value;
				if (flag)
				{
					this.m_RevealCursor = value;
					Action onRevealCursorChange = this.OnRevealCursorChange;
					if (onRevealCursorChange != null)
					{
						onRevealCursorChange();
					}
				}
			}
		}

		private int m_CharacterCount
		{
			get
			{
				return this.m_TextHandle.textInfo.characterCount;
			}
		}

		private int characterCount
		{
			get
			{
				return (this.m_CharacterCount > 0 && this.m_TextHandle.textInfo.textElementInfo[this.m_CharacterCount - 1].character == '\u200b') ? (this.m_CharacterCount - 1) : this.m_CharacterCount;
			}
		}

		private TextElementInfo[] m_TextElementInfos
		{
			get
			{
				return this.m_TextHandle.textInfo.textElementInfo;
			}
		}

		public int cursorIndex
		{
			get
			{
				return this.EnsureValidCodePointIndex(this.m_CursorIndex);
			}
			set
			{
				bool flag = this.m_CursorIndex != value;
				if (flag)
				{
					this.SetCursorIndexWithoutNotify(value);
					Action onCursorIndexChange = this.OnCursorIndexChange;
					if (onCursorIndexChange != null)
					{
						onCursorIndexChange();
					}
				}
			}
		}

		internal void SetCursorIndexWithoutNotify(int index)
		{
			this.m_CursorIndex = index;
		}

		public int selectIndex
		{
			get
			{
				return this.EnsureValidCodePointIndex(this.m_SelectIndex);
			}
			set
			{
				bool flag = this.m_SelectIndex != value;
				if (flag)
				{
					this.SetSelectIndexWithoutNotify(value);
					Action onSelectIndexChange = this.OnSelectIndexChange;
					if (onSelectIndexChange != null)
					{
						onSelectIndexChange();
					}
				}
			}
		}

		internal void SetSelectIndexWithoutNotify(int index)
		{
			this.m_SelectIndex = index;
		}

		public string selectedText
		{
			get
			{
				bool flag = this.cursorIndex == this.selectIndex;
				string text;
				if (flag)
				{
					text = "";
				}
				else
				{
					bool flag2 = this.cursorIndex < this.selectIndex;
					if (flag2)
					{
						text = this.m_TextHandle.Substring(this.cursorIndex, this.selectIndex - this.cursorIndex);
					}
					else
					{
						text = this.m_TextHandle.Substring(this.selectIndex, this.cursorIndex - this.selectIndex);
					}
				}
				return text;
			}
		}

		public TextSelectingUtilities(TextHandle textHandle)
		{
			this.m_TextHandle = textHandle;
		}

		internal bool HandleKeyEvent(Event e)
		{
			this.InitKeyActions();
			EventModifiers modifiers = e.modifiers;
			e.modifiers &= ~EventModifiers.CapsLock;
			bool flag = TextSelectingUtilities.s_KeySelectOps.ContainsKey(e);
			bool flag2;
			if (flag)
			{
				TextSelectOp textSelectOp = TextSelectingUtilities.s_KeySelectOps[e];
				this.PerformOperation(textSelectOp);
				e.modifiers = modifiers;
				flag2 = true;
			}
			else
			{
				e.modifiers = modifiers;
				flag2 = false;
			}
			return flag2;
		}

		private bool PerformOperation(TextSelectOp operation)
		{
			switch (operation)
			{
			case TextSelectOp.SelectLeft:
				this.SelectLeft();
				return false;
			case TextSelectOp.SelectRight:
				this.SelectRight();
				return false;
			case TextSelectOp.SelectUp:
				this.SelectUp();
				return false;
			case TextSelectOp.SelectDown:
				this.SelectDown();
				return false;
			case TextSelectOp.SelectTextStart:
				this.SelectTextStart();
				return false;
			case TextSelectOp.SelectTextEnd:
				this.SelectTextEnd();
				return false;
			case TextSelectOp.ExpandSelectGraphicalLineStart:
				this.ExpandSelectGraphicalLineStart();
				return false;
			case TextSelectOp.ExpandSelectGraphicalLineEnd:
				this.ExpandSelectGraphicalLineEnd();
				return false;
			case TextSelectOp.SelectGraphicalLineStart:
				this.SelectGraphicalLineStart();
				return false;
			case TextSelectOp.SelectGraphicalLineEnd:
				this.SelectGraphicalLineEnd();
				return false;
			case TextSelectOp.SelectWordLeft:
				this.SelectWordLeft();
				return false;
			case TextSelectOp.SelectWordRight:
				this.SelectWordRight();
				return false;
			case TextSelectOp.SelectToEndOfPreviousWord:
				this.SelectToEndOfPreviousWord();
				return false;
			case TextSelectOp.SelectToStartOfNextWord:
				this.SelectToStartOfNextWord();
				return false;
			case TextSelectOp.SelectParagraphBackward:
				this.SelectParagraphBackward();
				return false;
			case TextSelectOp.SelectParagraphForward:
				this.SelectParagraphForward();
				return false;
			case TextSelectOp.Copy:
				this.Copy();
				return false;
			case TextSelectOp.SelectAll:
				this.SelectAll();
				return false;
			case TextSelectOp.SelectNone:
				this.SelectNone();
				return false;
			}
			Debug.Log("Unimplemented: " + operation.ToString());
			return false;
		}

		private static void MapKey(string key, TextSelectOp action)
		{
			TextSelectingUtilities.s_KeySelectOps[Event.KeyboardEvent(key)] = action;
		}

		private void InitKeyActions()
		{
			bool flag = TextSelectingUtilities.s_KeySelectOps != null;
			if (!flag)
			{
				TextSelectingUtilities.s_KeySelectOps = new Dictionary<Event, TextSelectOp>();
				TextSelectingUtilities.MapKey("#left", TextSelectOp.SelectLeft);
				TextSelectingUtilities.MapKey("#right", TextSelectOp.SelectRight);
				TextSelectingUtilities.MapKey("#up", TextSelectOp.SelectUp);
				TextSelectingUtilities.MapKey("#down", TextSelectOp.SelectDown);
				bool flag2 = SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX;
				if (flag2)
				{
					TextSelectingUtilities.MapKey("#home", TextSelectOp.SelectTextStart);
					TextSelectingUtilities.MapKey("#end", TextSelectOp.SelectTextEnd);
					TextSelectingUtilities.MapKey("#^left", TextSelectOp.ExpandSelectGraphicalLineStart);
					TextSelectingUtilities.MapKey("#^right", TextSelectOp.ExpandSelectGraphicalLineEnd);
					TextSelectingUtilities.MapKey("#^up", TextSelectOp.SelectParagraphBackward);
					TextSelectingUtilities.MapKey("#^down", TextSelectOp.SelectParagraphForward);
					TextSelectingUtilities.MapKey("#&left", TextSelectOp.SelectWordLeft);
					TextSelectingUtilities.MapKey("#&right", TextSelectOp.SelectWordRight);
					TextSelectingUtilities.MapKey("#&up", TextSelectOp.SelectParagraphBackward);
					TextSelectingUtilities.MapKey("#&down", TextSelectOp.SelectParagraphForward);
					TextSelectingUtilities.MapKey("#%left", TextSelectOp.ExpandSelectGraphicalLineStart);
					TextSelectingUtilities.MapKey("#%right", TextSelectOp.ExpandSelectGraphicalLineEnd);
					TextSelectingUtilities.MapKey("#%up", TextSelectOp.SelectTextStart);
					TextSelectingUtilities.MapKey("#%down", TextSelectOp.SelectTextEnd);
					TextSelectingUtilities.MapKey("%a", TextSelectOp.SelectAll);
					TextSelectingUtilities.MapKey("%c", TextSelectOp.Copy);
				}
				else
				{
					TextSelectingUtilities.MapKey("#^left", TextSelectOp.SelectToEndOfPreviousWord);
					TextSelectingUtilities.MapKey("#^right", TextSelectOp.SelectToStartOfNextWord);
					TextSelectingUtilities.MapKey("#^up", TextSelectOp.SelectParagraphBackward);
					TextSelectingUtilities.MapKey("#^down", TextSelectOp.SelectParagraphForward);
					TextSelectingUtilities.MapKey("#home", TextSelectOp.SelectGraphicalLineStart);
					TextSelectingUtilities.MapKey("#end", TextSelectOp.SelectGraphicalLineEnd);
					TextSelectingUtilities.MapKey("^a", TextSelectOp.SelectAll);
					TextSelectingUtilities.MapKey("^c", TextSelectOp.Copy);
					TextSelectingUtilities.MapKey("^insert", TextSelectOp.Copy);
				}
			}
		}

		public void ClearCursorPos()
		{
			this.hasHorizontalCursorPos = false;
			this.iAltCursorPos = -1;
		}

		public void OnFocus(bool selectAll = true)
		{
			if (selectAll)
			{
				this.SelectAll();
			}
			this.revealCursor = true;
		}

		public void SelectAll()
		{
			this.cursorIndex = 0;
			this.selectIndex = int.MaxValue;
			this.ClearCursorPos();
		}

		public void SelectNone()
		{
			this.selectIndex = this.cursorIndex;
			this.ClearCursorPos();
		}

		public void SelectLeft()
		{
			bool bJustSelected = this.m_bJustSelected;
			if (bJustSelected)
			{
				bool flag = this.cursorIndex > this.selectIndex;
				if (flag)
				{
					int cursorIndex = this.cursorIndex;
					this.cursorIndex = this.selectIndex;
					this.selectIndex = cursorIndex;
				}
			}
			this.m_bJustSelected = false;
			this.cursorIndex = this.PreviousCodePointIndex(this.cursorIndex);
		}

		public void SelectRight()
		{
			bool bJustSelected = this.m_bJustSelected;
			if (bJustSelected)
			{
				bool flag = this.cursorIndex < this.selectIndex;
				if (flag)
				{
					int cursorIndex = this.cursorIndex;
					this.cursorIndex = this.selectIndex;
					this.selectIndex = cursorIndex;
				}
			}
			this.m_bJustSelected = false;
			this.cursorIndex = this.NextCodePointIndex(this.cursorIndex);
		}

		public void SelectUp()
		{
			this.cursorIndex = this.m_TextHandle.LineUpCharacterPosition(this.cursorIndex);
		}

		public void SelectDown()
		{
			this.cursorIndex = this.m_TextHandle.LineDownCharacterPosition(this.cursorIndex);
		}

		public void SelectTextEnd()
		{
			this.cursorIndex = this.characterCount;
		}

		public void SelectTextStart()
		{
			this.cursorIndex = 0;
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

		public void SelectWordRight()
		{
			this.ClearCursorPos();
			int selectIndex = this.selectIndex;
			bool flag = this.cursorIndex < this.selectIndex;
			if (flag)
			{
				this.selectIndex = this.cursorIndex;
				this.MoveWordRight();
				this.selectIndex = selectIndex;
				this.cursorIndex = ((this.cursorIndex < this.selectIndex) ? this.cursorIndex : this.selectIndex);
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
			bool flag = this.cursorIndex > this.selectIndex;
			if (flag)
			{
				this.selectIndex = this.cursorIndex;
				this.MoveWordLeft();
				this.selectIndex = selectIndex;
				this.cursorIndex = ((this.cursorIndex > this.selectIndex) ? this.cursorIndex : this.selectIndex);
			}
			else
			{
				this.selectIndex = this.cursorIndex;
				this.MoveWordLeft();
				this.selectIndex = selectIndex;
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
			bool flag2 = this.cursorIndex < this.characterCount;
			if (flag2)
			{
				this.cursorIndex = this.IndexOfEndOfLine(this.cursorIndex + 1);
				bool flag3 = flag && this.cursorIndex > this.selectIndex;
				if (flag3)
				{
					this.cursorIndex = this.selectIndex;
				}
			}
		}

		public void SelectParagraphBackward()
		{
			this.ClearCursorPos();
			bool flag = this.cursorIndex > this.selectIndex;
			bool flag2 = this.cursorIndex > 1;
			if (flag2)
			{
				this.cursorIndex = this.m_TextHandle.LastIndexOf('\n', this.cursorIndex - 2) + 1;
				bool flag3 = flag && this.cursorIndex < this.selectIndex;
				if (flag3)
				{
					this.cursorIndex = this.selectIndex;
				}
			}
			else
			{
				this.selectIndex = (this.cursorIndex = 0);
			}
		}

		public void SelectCurrentWord()
		{
			int cursorIndex = this.cursorIndex;
			bool flag = this.cursorIndex < this.selectIndex;
			if (flag)
			{
				this.cursorIndex = this.FindEndOfClassification(cursorIndex, TextSelectingUtilities.Direction.Backward);
				this.selectIndex = this.FindEndOfClassification(cursorIndex, TextSelectingUtilities.Direction.Forward);
			}
			else
			{
				this.cursorIndex = this.FindEndOfClassification(cursorIndex, TextSelectingUtilities.Direction.Forward);
				this.selectIndex = this.FindEndOfClassification(cursorIndex, TextSelectingUtilities.Direction.Backward);
			}
			this.ClearCursorPos();
			this.m_bJustSelected = true;
		}

		public void SelectCurrentParagraph()
		{
			this.ClearCursorPos();
			int characterCount = this.characterCount;
			bool flag = this.cursorIndex < characterCount;
			if (flag)
			{
				this.cursorIndex = this.IndexOfEndOfLine(this.cursorIndex);
			}
			bool flag2 = this.selectIndex != 0;
			if (flag2)
			{
				this.selectIndex = this.m_TextHandle.LastIndexOf('\n', this.selectIndex - 1) + 1;
			}
		}

		public void MoveRight()
		{
			this.ClearCursorPos();
			bool flag = this.selectIndex == this.cursorIndex;
			if (flag)
			{
				this.cursorIndex = this.NextCodePointIndex(this.cursorIndex);
				this.selectIndex = this.cursorIndex;
			}
			else
			{
				bool flag2 = this.selectIndex > this.cursorIndex;
				if (flag2)
				{
					this.cursorIndex = this.selectIndex;
				}
				else
				{
					this.selectIndex = this.cursorIndex;
				}
			}
		}

		public void MoveLeft()
		{
			bool flag = this.selectIndex == this.cursorIndex;
			if (flag)
			{
				this.cursorIndex = this.PreviousCodePointIndex(this.cursorIndex);
				this.selectIndex = this.cursorIndex;
			}
			else
			{
				bool flag2 = this.selectIndex > this.cursorIndex;
				if (flag2)
				{
					this.selectIndex = this.cursorIndex;
				}
				else
				{
					this.cursorIndex = this.selectIndex;
				}
			}
			this.ClearCursorPos();
		}

		public void MoveUp()
		{
			bool flag = this.selectIndex < this.cursorIndex;
			if (flag)
			{
				this.selectIndex = this.cursorIndex;
			}
			else
			{
				this.cursorIndex = this.selectIndex;
			}
			this.cursorIndex = (this.selectIndex = this.m_TextHandle.LineUpCharacterPosition(this.cursorIndex));
			bool flag2 = this.cursorIndex <= 0;
			if (flag2)
			{
				this.ClearCursorPos();
			}
		}

		public void MoveDown()
		{
			bool flag = this.selectIndex > this.cursorIndex;
			if (flag)
			{
				this.selectIndex = this.cursorIndex;
			}
			else
			{
				this.cursorIndex = this.selectIndex;
			}
			this.cursorIndex = (this.selectIndex = this.m_TextHandle.LineDownCharacterPosition(this.cursorIndex));
			bool flag2 = this.cursorIndex == this.characterCount;
			if (flag2)
			{
				this.ClearCursorPos();
			}
		}

		public void MoveLineStart()
		{
			int num = ((this.selectIndex < this.cursorIndex) ? this.selectIndex : this.cursorIndex);
			int num2 = num;
			while (num2-- != 0)
			{
				bool flag = this.m_TextElementInfos[num2].character == '\n';
				if (flag)
				{
					this.selectIndex = (this.cursorIndex = num2 + 1);
					return;
				}
			}
			this.selectIndex = (this.cursorIndex = 0);
		}

		public void MoveLineEnd()
		{
			int num = ((this.selectIndex > this.cursorIndex) ? this.selectIndex : this.cursorIndex);
			int i = num;
			int characterCount = this.characterCount;
			while (i < characterCount)
			{
				bool flag = this.m_TextElementInfos[i].character == '\n';
				if (flag)
				{
					this.selectIndex = (this.cursorIndex = i);
					return;
				}
				i++;
			}
			this.selectIndex = (this.cursorIndex = characterCount);
		}

		public void MoveGraphicalLineStart()
		{
			this.cursorIndex = (this.selectIndex = this.GetGraphicalLineStart((this.cursorIndex < this.selectIndex) ? this.cursorIndex : this.selectIndex));
		}

		public void MoveGraphicalLineEnd()
		{
			this.cursorIndex = (this.selectIndex = this.GetGraphicalLineEnd((this.cursorIndex > this.selectIndex) ? this.cursorIndex : this.selectIndex));
		}

		public void MoveTextStart()
		{
			this.selectIndex = (this.cursorIndex = 0);
		}

		public void MoveTextEnd()
		{
			this.selectIndex = (this.cursorIndex = this.characterCount);
		}

		public void MoveParagraphForward()
		{
			this.cursorIndex = ((this.cursorIndex > this.selectIndex) ? this.cursorIndex : this.selectIndex);
			bool flag = this.cursorIndex < this.characterCount;
			if (flag)
			{
				this.selectIndex = (this.cursorIndex = this.IndexOfEndOfLine(this.cursorIndex + 1));
			}
		}

		public void MoveParagraphBackward()
		{
			this.cursorIndex = ((this.cursorIndex < this.selectIndex) ? this.cursorIndex : this.selectIndex);
			bool flag = this.cursorIndex > 1;
			if (flag)
			{
				this.selectIndex = (this.cursorIndex = this.m_TextHandle.LastIndexOf('\n', this.cursorIndex - 2) + 1);
			}
			else
			{
				this.selectIndex = (this.cursorIndex = 0);
			}
		}

		public void MoveWordRight()
		{
			this.cursorIndex = ((this.cursorIndex > this.selectIndex) ? this.cursorIndex : this.selectIndex);
			this.cursorIndex = (this.selectIndex = this.FindNextSeperator(this.cursorIndex));
			this.ClearCursorPos();
		}

		public void MoveToStartOfNextWord()
		{
			this.ClearCursorPos();
			bool flag = this.cursorIndex != this.selectIndex;
			if (flag)
			{
				this.MoveRight();
			}
			else
			{
				this.cursorIndex = (this.selectIndex = this.FindStartOfNextWord(this.cursorIndex));
			}
		}

		public void MoveToEndOfPreviousWord()
		{
			this.ClearCursorPos();
			bool flag = this.cursorIndex != this.selectIndex;
			if (flag)
			{
				this.MoveLeft();
			}
			else
			{
				this.cursorIndex = (this.selectIndex = this.FindEndOfPreviousWord(this.cursorIndex));
			}
		}

		public void MoveWordLeft()
		{
			this.cursorIndex = ((this.cursorIndex < this.selectIndex) ? this.cursorIndex : this.selectIndex);
			this.cursorIndex = this.FindPrevSeperator(this.cursorIndex);
			this.selectIndex = this.cursorIndex;
		}

		public void MouseDragSelectsWholeWords(bool on)
		{
			this.m_MouseDragSelectsWholeWords = on;
			this.m_DblClickInitPosStart = ((this.cursorIndex < this.selectIndex) ? this.cursorIndex : this.selectIndex);
			this.m_DblClickInitPosEnd = ((this.cursorIndex < this.selectIndex) ? this.selectIndex : this.cursorIndex);
		}

		public void ExpandSelectGraphicalLineStart()
		{
			this.ClearCursorPos();
			bool flag = this.cursorIndex < this.selectIndex;
			if (flag)
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
			bool flag = this.cursorIndex > this.selectIndex;
			if (flag)
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

		public void DblClickSnap(TextEditor.DblClickSnapping snapping)
		{
			this.dblClickSnap = snapping;
		}

		protected internal void MoveCursorToPosition_Internal(Vector2 cursorPosition, bool shift)
		{
			this.selectIndex = this.m_TextHandle.GetCursorIndexFromPosition(cursorPosition, true);
			bool flag = !shift;
			if (flag)
			{
				this.cursorIndex = this.selectIndex;
			}
		}

		public void SelectToPosition(Vector2 cursorPosition)
		{
			bool flag = this.characterCount == 0;
			if (!flag)
			{
				bool flag2 = !this.m_MouseDragSelectsWholeWords;
				if (flag2)
				{
					this.cursorIndex = this.m_TextHandle.GetCursorIndexFromPosition(cursorPosition, true);
				}
				else
				{
					int num = this.m_TextHandle.GetCursorIndexFromPosition(cursorPosition, true);
					num = this.EnsureValidCodePointIndex(num);
					bool flag3 = this.dblClickSnap == TextEditor.DblClickSnapping.WORDS;
					if (flag3)
					{
						bool flag4 = num <= this.m_DblClickInitPosStart;
						if (flag4)
						{
							this.cursorIndex = this.FindEndOfClassification(num, TextSelectingUtilities.Direction.Backward);
							this.selectIndex = this.FindEndOfClassification(this.m_DblClickInitPosEnd - 1, TextSelectingUtilities.Direction.Forward);
						}
						else
						{
							bool flag5 = num >= this.m_DblClickInitPosEnd;
							if (flag5)
							{
								this.cursorIndex = this.FindEndOfClassification(num - 1, TextSelectingUtilities.Direction.Forward);
								this.selectIndex = this.FindEndOfClassification(this.m_DblClickInitPosStart + 1, TextSelectingUtilities.Direction.Backward);
							}
							else
							{
								this.cursorIndex = this.m_DblClickInitPosStart;
								this.selectIndex = this.m_DblClickInitPosEnd;
							}
						}
					}
					else
					{
						bool flag6 = num <= this.m_DblClickInitPosStart;
						if (flag6)
						{
							bool flag7 = num > 0;
							if (flag7)
							{
								this.cursorIndex = this.m_TextHandle.LastIndexOf('\n', Mathf.Max(0, num - 1)) + 1;
							}
							else
							{
								this.cursorIndex = 0;
							}
							this.selectIndex = this.m_TextHandle.LastIndexOf('\n', Mathf.Min(this.characterCount - 1, this.m_DblClickInitPosEnd + 1));
						}
						else
						{
							bool flag8 = num >= this.m_DblClickInitPosEnd;
							if (flag8)
							{
								bool flag9 = num < this.characterCount;
								if (flag9)
								{
									this.cursorIndex = this.IndexOfEndOfLine(num);
								}
								else
								{
									this.cursorIndex = this.characterCount;
								}
								this.selectIndex = this.m_TextHandle.LastIndexOf('\n', Mathf.Max(0, this.m_DblClickInitPosEnd - 2)) + 1;
							}
							else
							{
								this.cursorIndex = this.m_DblClickInitPosStart;
								this.selectIndex = this.m_DblClickInitPosEnd;
							}
						}
					}
				}
			}
		}

		private int FindNextSeperator(int startPos)
		{
			int characterCount = this.characterCount;
			while (startPos < characterCount && this.ClassifyChar(startPos) > TextSelectingUtilities.CharacterType.LetterLike)
			{
				startPos = this.NextCodePointIndex(startPos);
			}
			while (startPos < characterCount && this.ClassifyChar(startPos) == TextSelectingUtilities.CharacterType.LetterLike)
			{
				startPos = this.NextCodePointIndex(startPos);
			}
			return startPos;
		}

		private int FindPrevSeperator(int startPos)
		{
			startPos = this.PreviousCodePointIndex(startPos);
			while (startPos > 0 && this.ClassifyChar(startPos) > TextSelectingUtilities.CharacterType.LetterLike)
			{
				startPos = this.PreviousCodePointIndex(startPos);
			}
			bool flag = startPos == 0;
			int num;
			if (flag)
			{
				num = 0;
			}
			else
			{
				while (startPos > 0 && this.ClassifyChar(startPos) == TextSelectingUtilities.CharacterType.LetterLike)
				{
					startPos = this.PreviousCodePointIndex(startPos);
				}
				bool flag2 = this.ClassifyChar(startPos) == TextSelectingUtilities.CharacterType.LetterLike;
				if (flag2)
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

		public int FindStartOfNextWord(int p)
		{
			int characterCount = this.characterCount;
			bool flag = p == characterCount;
			int num;
			if (flag)
			{
				num = p;
			}
			else
			{
				TextSelectingUtilities.CharacterType characterType = this.ClassifyChar(p);
				bool flag2 = characterType != TextSelectingUtilities.CharacterType.WhiteSpace;
				if (flag2)
				{
					p = this.NextCodePointIndex(p);
					while (p < characterCount && this.ClassifyChar(p) == characterType)
					{
						p = this.NextCodePointIndex(p);
					}
				}
				else
				{
					bool flag3 = this.m_TextElementInfos[p].character == '\t' || this.m_TextElementInfos[p].character == '\n';
					if (flag3)
					{
						return this.NextCodePointIndex(p);
					}
				}
				bool flag4 = p == characterCount;
				if (flag4)
				{
					num = p;
				}
				else
				{
					bool flag5 = this.m_TextElementInfos[p].character == ' ';
					if (flag5)
					{
						while (p < characterCount && this.ClassifyChar(p) == TextSelectingUtilities.CharacterType.WhiteSpace)
						{
							p = this.NextCodePointIndex(p);
						}
					}
					else
					{
						bool flag6 = this.m_TextElementInfos[p].character == '\t' || this.m_TextElementInfos[p].character == '\n';
						if (flag6)
						{
							return p;
						}
					}
					num = p;
				}
			}
			return num;
		}

		public int FindEndOfPreviousWord(int p)
		{
			bool flag = p == 0;
			int num;
			if (flag)
			{
				num = p;
			}
			else
			{
				p = this.PreviousCodePointIndex(p);
				while (p > 0 && this.m_TextElementInfos[p].character == ' ')
				{
					p = this.PreviousCodePointIndex(p);
				}
				TextSelectingUtilities.CharacterType characterType = this.ClassifyChar(p);
				bool flag2 = characterType != TextSelectingUtilities.CharacterType.WhiteSpace;
				if (flag2)
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

		private int FindEndOfClassification(int p, TextSelectingUtilities.Direction dir)
		{
			bool flag = this.characterCount == 0;
			int num;
			if (flag)
			{
				num = 0;
			}
			else
			{
				bool flag2 = p == this.characterCount;
				if (flag2)
				{
					p = this.PreviousCodePointIndex(p);
				}
				TextSelectingUtilities.CharacterType characterType = this.ClassifyChar(p);
				bool flag3 = characterType == TextSelectingUtilities.CharacterType.NewLine;
				if (flag3)
				{
					num = p;
				}
				else
				{
					for (;;)
					{
						if (dir != TextSelectingUtilities.Direction.Forward)
						{
							if (dir == TextSelectingUtilities.Direction.Backward)
							{
								p = this.PreviousCodePointIndex(p);
								bool flag4 = p == 0;
								if (flag4)
								{
									break;
								}
							}
						}
						else
						{
							p = this.NextCodePointIndex(p);
							bool flag5 = p == this.characterCount;
							if (flag5)
							{
								goto Block_8;
							}
						}
						if (this.ClassifyChar(p) != characterType)
						{
							goto Block_9;
						}
					}
					return (this.ClassifyChar(0) == characterType) ? 0 : this.NextCodePointIndex(0);
					Block_8:
					return this.characterCount;
					Block_9:
					bool flag6 = dir == TextSelectingUtilities.Direction.Forward;
					if (flag6)
					{
						num = p;
					}
					else
					{
						num = this.NextCodePointIndex(p);
					}
				}
			}
			return num;
		}

		private int ClampTextIndex(int index)
		{
			return Mathf.Clamp(index, 0, this.characterCount);
		}

		internal int EnsureValidCodePointIndex(int index)
		{
			index = this.ClampTextIndex(index);
			bool flag = !this.IsValidCodePointIndex(index);
			if (flag)
			{
				index = this.NextCodePointIndex(index);
			}
			return index;
		}

		private bool IsValidCodePointIndex(int index)
		{
			bool flag = index < 0 || index > this.characterCount;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = index == 0 || index == this.characterCount;
				flag2 = flag3 || !char.IsLowSurrogate(this.m_TextElementInfos[index].character);
			}
			return flag2;
		}

		private int IndexOfEndOfLine(int startIndex)
		{
			int num = this.m_TextHandle.IndexOf('\n', startIndex);
			return (num != -1) ? num : this.characterCount;
		}

		public int PreviousCodePointIndex(int index)
		{
			bool flag = index > 0;
			if (flag)
			{
				index--;
			}
			while (index > 0 && char.IsLowSurrogate(this.m_TextElementInfos[index].character))
			{
				index--;
			}
			return index;
		}

		public int NextCodePointIndex(int index)
		{
			bool flag = index < this.characterCount;
			if (flag)
			{
				index++;
			}
			while (index < this.characterCount && char.IsLowSurrogate(this.m_TextElementInfos[index].character))
			{
				index++;
			}
			return index;
		}

		private int GetGraphicalLineStart(int p)
		{
			Vector2 cursorPositionFromStringIndexUsingLineHeight = this.m_TextHandle.GetCursorPositionFromStringIndexUsingLineHeight(p, false, true);
			cursorPositionFromStringIndexUsingLineHeight.y -= 1f / GUIUtility.pixelsPerPoint;
			cursorPositionFromStringIndexUsingLineHeight.x = 0f;
			return this.m_TextHandle.GetCursorIndexFromPosition(cursorPositionFromStringIndexUsingLineHeight, true);
		}

		private int GetGraphicalLineEnd(int p)
		{
			Vector2 cursorPositionFromStringIndexUsingLineHeight = this.m_TextHandle.GetCursorPositionFromStringIndexUsingLineHeight(p, false, true);
			cursorPositionFromStringIndexUsingLineHeight.y -= 1f / GUIUtility.pixelsPerPoint;
			cursorPositionFromStringIndexUsingLineHeight.x += 5000f;
			return this.m_TextHandle.GetCursorIndexFromPosition(cursorPositionFromStringIndexUsingLineHeight, true);
		}

		public void Copy()
		{
			bool flag = this.selectIndex == this.cursorIndex;
			if (!flag)
			{
				GUIUtility.systemCopyBuffer = this.selectedText;
			}
		}

		private TextSelectingUtilities.CharacterType ClassifyChar(int index)
		{
			char character = this.m_TextElementInfos[index].character;
			bool flag = character == '\n';
			TextSelectingUtilities.CharacterType characterType;
			if (flag)
			{
				characterType = TextSelectingUtilities.CharacterType.NewLine;
			}
			else
			{
				bool flag2 = char.IsWhiteSpace(character);
				if (flag2)
				{
					characterType = TextSelectingUtilities.CharacterType.WhiteSpace;
				}
				else
				{
					bool flag3 = char.IsLetterOrDigit(character) || this.m_TextElementInfos[index].character == '\'';
					if (flag3)
					{
						characterType = TextSelectingUtilities.CharacterType.LetterLike;
					}
					else
					{
						characterType = TextSelectingUtilities.CharacterType.Symbol;
					}
				}
			}
			return characterType;
		}

		public TextEditor.DblClickSnapping dblClickSnap = TextEditor.DblClickSnapping.WORDS;

		public int iAltCursorPos = -1;

		public bool hasHorizontalCursorPos = false;

		private bool m_bJustSelected = false;

		private bool m_MouseDragSelectsWholeWords = false;

		private int m_DblClickInitPosStart = 0;

		private int m_DblClickInitPosEnd = 0;

		private TextHandle m_TextHandle;

		private const int kMoveDownHeight = 5;

		private const char kNewLineChar = '\n';

		private bool m_RevealCursor;

		private int m_CursorIndex = 0;

		internal int m_SelectIndex = 0;

		private static Dictionary<Event, TextSelectOp> s_KeySelectOps;

		internal Action OnCursorIndexChange;

		internal Action OnSelectIndexChange;

		internal Action OnRevealCursorChange;

		private enum CharacterType
		{
			LetterLike,
			Symbol,
			Symbol2,
			WhiteSpace,
			NewLine
		}

		private enum Direction
		{
			Forward,
			Backward
		}
	}
}
