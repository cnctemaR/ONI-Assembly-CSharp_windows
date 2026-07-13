using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;

namespace UnityEngine
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEditor.UIBuilderModule" })]
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
				return this.textHandle.characterCount;
			}
		}

		private int characterCount
		{
			get
			{
				return (!this.textHandle.useAdvancedText && this.m_CharacterCount > 0 && this.textHandle.textInfo.textElementInfo[this.m_CharacterCount - 1].character == 8203U) ? (this.m_CharacterCount - 1) : this.m_CharacterCount;
			}
		}

		private TextElementInfo[] m_TextElementInfos
		{
			get
			{
				return this.textHandle.textInfo.textElementInfo;
			}
		}

		public int cursorIndex
		{
			get
			{
				return this.textHandle.IsPlaceholder ? 0 : this.ClampTextIndex(this.m_CursorIndex);
			}
			set
			{
				bool flag = this.m_CursorIndex != value;
				if (flag)
				{
					this.m_CursorIndex = value;
					Action onCursorIndexChange = this.OnCursorIndexChange;
					if (onCursorIndexChange != null)
					{
						onCursorIndexChange();
					}
				}
			}
		}

		internal int cursorIndexNoValidation
		{
			get
			{
				return this.m_CursorIndex;
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
				return this.textHandle.IsPlaceholder ? 0 : this.ClampTextIndex(this.m_SelectIndex);
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

		internal int selectIndexNoValidation
		{
			get
			{
				return this.m_SelectIndex;
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
						text = this.textHandle.Substring(this.cursorIndex, this.selectIndex - this.cursorIndex);
					}
					else
					{
						text = this.textHandle.Substring(this.selectIndex, this.cursorIndex - this.selectIndex);
					}
				}
				return text;
			}
		}

		public TextSelectingUtilities(TextHandle textHandle)
		{
			this.textHandle = textHandle;
		}

		internal bool HandleKeyEvent(Event e)
		{
			return this.HandleKeyEvent(e.keyCode, e.modifiers);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal bool HandleKeyEvent(KeyCode key, EventModifiers modifiers)
		{
			TextSelectOp? textSelectOp = TextSelectingUtilities.TextSelectOpFromEnum(key, modifiers, SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX);
			bool flag = textSelectOp != null;
			bool flag2;
			if (flag)
			{
				this.PerformOperation(textSelectOp.Value);
				flag2 = true;
			}
			else
			{
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

		internal static TextSelectOp? TextSelectOpFromEnum(KeyCode key, EventModifiers modifiers, bool IsMacOsFamily)
		{
			modifiers &= ~EventModifiers.CapsLock;
			TextEditingUtilities.KeyEvent keyEvent = new TextEditingUtilities.KeyEvent(key, modifiers);
			foreach (ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp> valueTuple in TextSelectingUtilities.s_GlobalKeyMappings)
			{
				bool flag = valueTuple.Item1 == keyEvent;
				if (flag)
				{
					return new TextSelectOp?(valueTuple.Item2);
				}
			}
			foreach (ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp> valueTuple2 in (IsMacOsFamily ? TextSelectingUtilities.s_MacKeyMappings : TextSelectingUtilities.s_WindowsLinuxKeyMappings))
			{
				bool flag2 = valueTuple2.Item1 == keyEvent;
				if (flag2)
				{
					return new TextSelectOp?(valueTuple2.Item2);
				}
			}
			return null;
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
			this.cursorIndex = this.textHandle.LineUpCharacterPosition(this.cursorIndex);
		}

		public void SelectDown()
		{
			this.cursorIndex = this.textHandle.LineDownCharacterPosition(this.cursorIndex);
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
			bool useAdvancedText = this.textHandle.useAdvancedText;
			if (useAdvancedText)
			{
				int cursorIndex = this.cursorIndex;
				this.textHandle.SelectToNextParagraph(ref cursorIndex);
				this.cursorIndex = cursorIndex;
			}
			else
			{
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
		}

		public void SelectParagraphBackward()
		{
			this.ClearCursorPos();
			bool flag = this.cursorIndex > this.selectIndex;
			bool useAdvancedText = this.textHandle.useAdvancedText;
			if (useAdvancedText)
			{
				int cursorIndex = this.cursorIndex;
				this.textHandle.SelectToPreviousParagraph(ref cursorIndex);
				this.cursorIndex = cursorIndex;
			}
			else
			{
				bool flag2 = this.cursorIndex > 1;
				if (flag2)
				{
					this.cursorIndex = this.textHandle.LastIndexOf('\n', this.cursorIndex - 2) + 1;
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
		}

		public void SelectCurrentWord()
		{
			int cursorIndex = this.cursorIndex;
			bool useAdvancedText = this.textHandle.useAdvancedText;
			if (useAdvancedText)
			{
				int num = 0;
				int num2 = 0;
				this.textHandle.SelectCurrentWord(cursorIndex, ref num, ref num2);
				bool flag = this.cursorIndex < this.selectIndex;
				if (flag)
				{
					this.cursorIndex = num;
					this.selectIndex = num2;
				}
				else
				{
					this.cursorIndex = num2;
					this.selectIndex = num;
				}
			}
			else
			{
				bool flag2 = this.cursorIndex < this.selectIndex;
				if (flag2)
				{
					this.cursorIndex = this.FindEndOfClassification(cursorIndex, TextSelectingUtilities.Direction.Backward);
					this.selectIndex = this.FindEndOfClassification(cursorIndex, TextSelectingUtilities.Direction.Forward);
				}
				else
				{
					this.cursorIndex = this.FindEndOfClassification(cursorIndex, TextSelectingUtilities.Direction.Forward);
					this.selectIndex = this.FindEndOfClassification(cursorIndex, TextSelectingUtilities.Direction.Backward);
				}
			}
			this.ClearCursorPos();
			this.m_bJustSelected = true;
		}

		public void SelectCurrentParagraph()
		{
			this.ClearCursorPos();
			int characterCount = this.characterCount;
			bool useAdvancedText = this.textHandle.useAdvancedText;
			if (useAdvancedText)
			{
				int cursorIndex = this.cursorIndex;
				int selectIndex = this.selectIndex;
				this.textHandle.SelectCurrentParagraph(ref cursorIndex, ref selectIndex);
				this.cursorIndex = cursorIndex;
				this.selectIndex = selectIndex;
			}
			else
			{
				bool flag = this.cursorIndex < characterCount;
				if (flag)
				{
					this.cursorIndex = this.IndexOfEndOfLine(this.cursorIndex);
				}
				bool flag2 = this.selectIndex != 0;
				if (flag2)
				{
					this.selectIndex = this.textHandle.LastIndexOf('\n', this.selectIndex - 1) + 1;
				}
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
			this.cursorIndex = (this.selectIndex = this.textHandle.LineUpCharacterPosition(this.cursorIndex));
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
			this.cursorIndex = (this.selectIndex = this.textHandle.LineDownCharacterPosition(this.cursorIndex));
			bool flag2 = this.cursorIndex == this.characterCount;
			if (flag2)
			{
				this.ClearCursorPos();
			}
		}

		public void MoveLineStart()
		{
			bool useAdvancedText = this.textHandle.useAdvancedText;
			if (useAdvancedText)
			{
				int cursorIndex = this.cursorIndex;
				this.textHandle.SelectToPreviousParagraph(ref cursorIndex);
				this.cursorIndex = (this.selectIndex = cursorIndex);
			}
			else
			{
				int num = ((this.selectIndex < this.cursorIndex) ? this.selectIndex : this.cursorIndex);
				int num2 = num;
				while (num2-- != 0)
				{
					bool flag = this.m_TextElementInfos[num2].character == 10U;
					if (flag)
					{
						this.selectIndex = (this.cursorIndex = num2 + 1);
						return;
					}
				}
				this.selectIndex = (this.cursorIndex = 0);
			}
		}

		public void MoveLineEnd()
		{
			bool useAdvancedText = this.textHandle.useAdvancedText;
			if (useAdvancedText)
			{
				int cursorIndex = this.cursorIndex;
				this.textHandle.SelectToNextParagraph(ref cursorIndex);
				this.cursorIndex = (this.selectIndex = cursorIndex);
			}
			else
			{
				int num = ((this.selectIndex > this.cursorIndex) ? this.selectIndex : this.cursorIndex);
				int i = num;
				int characterCount = this.characterCount;
				while (i < characterCount)
				{
					bool flag = this.m_TextElementInfos[i].character == 10U;
					if (flag)
					{
						this.selectIndex = (this.cursorIndex = i);
						return;
					}
					i++;
				}
				this.selectIndex = (this.cursorIndex = characterCount);
			}
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
			bool useAdvancedText = this.textHandle.useAdvancedText;
			if (useAdvancedText)
			{
				int cursorIndex = this.cursorIndex;
				this.textHandle.SelectToNextParagraph(ref cursorIndex);
				this.cursorIndex = (this.selectIndex = cursorIndex);
			}
			else
			{
				this.cursorIndex = ((this.cursorIndex > this.selectIndex) ? this.cursorIndex : this.selectIndex);
				bool flag = this.cursorIndex < this.characterCount;
				if (flag)
				{
					this.selectIndex = (this.cursorIndex = this.IndexOfEndOfLine(this.cursorIndex + 1));
				}
			}
		}

		public void MoveParagraphBackward()
		{
			bool useAdvancedText = this.textHandle.useAdvancedText;
			if (useAdvancedText)
			{
				int cursorIndex = this.cursorIndex;
				this.textHandle.SelectToPreviousParagraph(ref cursorIndex);
				this.cursorIndex = (this.selectIndex = cursorIndex);
			}
			else
			{
				this.cursorIndex = ((this.cursorIndex < this.selectIndex) ? this.cursorIndex : this.selectIndex);
				bool flag = this.cursorIndex > 1;
				if (flag)
				{
					this.selectIndex = (this.cursorIndex = this.textHandle.LastIndexOf('\n', this.cursorIndex - 2) + 1);
				}
				else
				{
					this.selectIndex = (this.cursorIndex = 0);
				}
			}
		}

		public void MoveWordRight()
		{
			this.cursorIndex = ((this.cursorIndex > this.selectIndex) ? this.cursorIndex : this.selectIndex);
			bool useAdvancedText = this.textHandle.useAdvancedText;
			if (useAdvancedText)
			{
				this.cursorIndex = (this.selectIndex = this.FindStartOfNextWord(this.cursorIndex));
			}
			else
			{
				this.cursorIndex = (this.selectIndex = this.FindNextSeperator(this.cursorIndex));
			}
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
			bool useAdvancedText = this.textHandle.useAdvancedText;
			if (useAdvancedText)
			{
				this.cursorIndex = this.FindEndOfPreviousWord(this.cursorIndex);
			}
			else
			{
				this.cursorIndex = this.FindPrevSeperator(this.cursorIndex);
			}
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
			this.selectIndex = this.textHandle.GetCursorIndexFromPosition(cursorPosition, true);
			bool flag = !shift;
			if (flag)
			{
				this.cursorIndex = this.selectIndex;
			}
		}

		protected internal void MoveAltCursorToPosition(Vector2 cursorPosition)
		{
			bool flag = this.cursorIndex == 0 && this.selectIndex == this.characterCount;
			if (flag)
			{
				this.iAltCursorPos = -1;
			}
			else
			{
				int cursorIndexFromPosition = this.textHandle.GetCursorIndexFromPosition(cursorPosition, true);
				this.iAltCursorPos = Mathf.Min(this.characterCount, cursorIndexFromPosition);
			}
		}

		protected internal bool IsOverSelection(Vector2 cursorPosition)
		{
			int cursorIndexFromPosition = this.textHandle.GetCursorIndexFromPosition(cursorPosition, true);
			return cursorIndexFromPosition < Mathf.Max(this.cursorIndex, this.selectIndex) && cursorIndexFromPosition > Mathf.Min(this.cursorIndex, this.selectIndex);
		}

		public void SelectToPosition(Vector2 cursorPosition)
		{
			bool flag = this.characterCount == 0;
			if (!flag)
			{
				bool flag2 = !this.m_MouseDragSelectsWholeWords;
				if (flag2)
				{
					this.cursorIndex = this.textHandle.GetCursorIndexFromPosition(cursorPosition, true);
				}
				else
				{
					int cursorIndexFromPosition = this.textHandle.GetCursorIndexFromPosition(cursorPosition, true);
					bool flag3 = this.dblClickSnap == TextEditor.DblClickSnapping.WORDS;
					if (flag3)
					{
						bool flag4 = cursorIndexFromPosition <= this.m_DblClickInitPosStart;
						if (flag4)
						{
							bool useAdvancedText = this.textHandle.useAdvancedText;
							if (useAdvancedText)
							{
								this.selectIndex = Mathf.Max(this.selectIndex, this.cursorIndex);
								this.cursorIndex = this.textHandle.GetEndOfPreviousWord(cursorIndexFromPosition);
							}
							else
							{
								this.cursorIndex = this.FindEndOfClassification(cursorIndexFromPosition, TextSelectingUtilities.Direction.Backward);
								this.selectIndex = this.FindEndOfClassification(this.m_DblClickInitPosEnd - 1, TextSelectingUtilities.Direction.Forward);
							}
						}
						else
						{
							bool flag5 = cursorIndexFromPosition >= this.m_DblClickInitPosEnd;
							if (flag5)
							{
								bool useAdvancedText2 = this.textHandle.useAdvancedText;
								if (useAdvancedText2)
								{
									this.selectIndex = Mathf.Min(this.selectIndex, this.cursorIndex);
									this.cursorIndex = this.textHandle.GetStartOfNextWord(cursorIndexFromPosition - 1);
								}
								else
								{
									this.cursorIndex = this.FindEndOfClassification(cursorIndexFromPosition - 1, TextSelectingUtilities.Direction.Forward);
									this.selectIndex = this.FindEndOfClassification(this.m_DblClickInitPosStart + 1, TextSelectingUtilities.Direction.Backward);
								}
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
						bool flag6 = (!this.textHandle.useAdvancedText && cursorIndexFromPosition <= this.m_DblClickInitPosStart) || (this.textHandle.useAdvancedText && cursorIndexFromPosition < this.m_DblClickInitPosStart);
						if (flag6)
						{
							bool useAdvancedText3 = this.textHandle.useAdvancedText;
							if (useAdvancedText3)
							{
								int num = cursorIndexFromPosition;
								this.textHandle.SelectToStartOfParagraph(ref num);
								this.selectIndex = num;
							}
							else
							{
								bool flag7 = cursorIndexFromPosition > 0;
								if (flag7)
								{
									this.cursorIndex = this.textHandle.LastIndexOf('\n', Mathf.Max(0, cursorIndexFromPosition - 1)) + 1;
								}
								else
								{
									this.cursorIndex = 0;
								}
								this.selectIndex = this.textHandle.LastIndexOf('\n', Mathf.Min(this.characterCount - 1, this.m_DblClickInitPosEnd + 1));
							}
						}
						else
						{
							bool flag8 = cursorIndexFromPosition >= this.m_DblClickInitPosEnd;
							if (flag8)
							{
								bool useAdvancedText4 = this.textHandle.useAdvancedText;
								if (useAdvancedText4)
								{
									int num2 = cursorIndexFromPosition;
									this.textHandle.SelectToEndOfParagraph(ref num2);
									this.cursorIndex = num2;
								}
								else
								{
									bool flag9 = cursorIndexFromPosition < this.characterCount;
									if (flag9)
									{
										this.cursorIndex = this.IndexOfEndOfLine(cursorIndexFromPosition);
									}
									else
									{
										this.cursorIndex = this.characterCount;
									}
									this.selectIndex = this.textHandle.LastIndexOf('\n', Mathf.Max(0, this.m_DblClickInitPosEnd - 2)) + 1;
								}
							}
							else
							{
								bool useAdvancedText5 = this.textHandle.useAdvancedText;
								if (useAdvancedText5)
								{
									this.cursorIndex = this.m_DblClickInitPosEnd;
									this.selectIndex = this.m_DblClickInitPosStart;
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
			bool useAdvancedText = this.textHandle.useAdvancedText;
			int num;
			if (useAdvancedText)
			{
				num = this.textHandle.GetStartOfNextWord(p);
			}
			else
			{
				int characterCount = this.characterCount;
				bool flag = p == characterCount;
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
						bool flag3 = this.m_TextElementInfos[p].character == 9U || this.m_TextElementInfos[p].character == 10U;
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
						bool flag5 = this.m_TextElementInfos[p].character == 32U;
						if (flag5)
						{
							while (p < characterCount && this.ClassifyChar(p) == TextSelectingUtilities.CharacterType.WhiteSpace)
							{
								p = this.NextCodePointIndex(p);
							}
						}
						else
						{
							bool flag6 = this.m_TextElementInfos[p].character == 9U || this.m_TextElementInfos[p].character == 10U;
							if (flag6)
							{
								return p;
							}
						}
						num = p;
					}
				}
			}
			return num;
		}

		public int FindEndOfPreviousWord(int p)
		{
			bool useAdvancedText = this.textHandle.useAdvancedText;
			int num;
			if (useAdvancedText)
			{
				num = this.textHandle.GetEndOfPreviousWord(p);
			}
			else
			{
				bool flag = p == 0;
				if (flag)
				{
					num = p;
				}
				else
				{
					p = this.PreviousCodePointIndex(p);
					while (p > 0 && this.m_TextElementInfos[p].character == 32U)
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
				bool flag2 = p >= this.characterCount;
				if (flag2)
				{
					p = this.characterCount - 1;
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
							bool flag5 = p >= this.characterCount;
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

		private int IndexOfEndOfLine(int startIndex)
		{
			int num = this.textHandle.IndexOf('\n', startIndex);
			return (num != -1) ? num : this.characterCount;
		}

		public int PreviousCodePointIndex(int index)
		{
			bool useAdvancedText = this.textHandle.useAdvancedText;
			int num;
			if (useAdvancedText)
			{
				num = this.textHandle.PreviousCodePointIndex(index);
			}
			else
			{
				bool flag = index > 0;
				if (flag)
				{
					index--;
				}
				num = index;
			}
			return num;
		}

		public int NextCodePointIndex(int index)
		{
			bool useAdvancedText = this.textHandle.useAdvancedText;
			int num;
			if (useAdvancedText)
			{
				num = this.textHandle.NextCodePointIndex(index);
			}
			else
			{
				bool flag = index < this.characterCount;
				if (flag)
				{
					index++;
				}
				num = index;
			}
			return num;
		}

		private int GetGraphicalLineStart(int p)
		{
			return this.textHandle.GetFirstCharacterIndexOnLine(p);
		}

		private int GetGraphicalLineEnd(int p)
		{
			return this.textHandle.GetLastCharacterIndexOnLine(p);
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
			char c = (char)this.m_TextElementInfos[index].character;
			bool flag = c == '\n';
			TextSelectingUtilities.CharacterType characterType;
			if (flag)
			{
				characterType = TextSelectingUtilities.CharacterType.NewLine;
			}
			else
			{
				bool flag2 = char.IsWhiteSpace(c);
				if (flag2)
				{
					characterType = TextSelectingUtilities.CharacterType.WhiteSpace;
				}
				else
				{
					bool flag3 = char.IsLetterOrDigit(c) || this.m_TextElementInfos[index].character == 39U;
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

		public TextHandle textHandle;

		private const int kMoveDownHeight = 5;

		private const char kNewLineChar = '\n';

		private bool m_RevealCursor;

		private int m_CursorIndex = 0;

		internal int m_SelectIndex = 0;

		[TupleElementNames(new string[] { "keyEvent", "operation" })]
		internal static readonly List<ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>> s_GlobalKeyMappings = new List<ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>>
		{
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.LeftArrow, EventModifiers.Shift | EventModifiers.FunctionKey), TextSelectOp.SelectLeft),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.RightArrow, EventModifiers.Shift | EventModifiers.FunctionKey), TextSelectOp.SelectRight),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.UpArrow, EventModifiers.Shift | EventModifiers.FunctionKey), TextSelectOp.SelectUp),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.DownArrow, EventModifiers.Shift | EventModifiers.FunctionKey), TextSelectOp.SelectDown)
		};

		[TupleElementNames(new string[] { "keyEvent", "operation" })]
		internal static readonly List<ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>> s_MacKeyMappings = new List<ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>>
		{
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.Home, EventModifiers.Shift | EventModifiers.FunctionKey), TextSelectOp.SelectTextStart),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.End, EventModifiers.Shift | EventModifiers.FunctionKey), TextSelectOp.SelectTextEnd),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.LeftArrow, EventModifiers.Shift | EventModifiers.Control | EventModifiers.FunctionKey), TextSelectOp.ExpandSelectGraphicalLineStart),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.RightArrow, EventModifiers.Shift | EventModifiers.Control | EventModifiers.FunctionKey), TextSelectOp.ExpandSelectGraphicalLineEnd),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.UpArrow, EventModifiers.Shift | EventModifiers.Control | EventModifiers.FunctionKey), TextSelectOp.SelectParagraphBackward),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.DownArrow, EventModifiers.Shift | EventModifiers.Control | EventModifiers.FunctionKey), TextSelectOp.SelectParagraphForward),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.LeftArrow, EventModifiers.Shift | EventModifiers.Alt | EventModifiers.FunctionKey), TextSelectOp.SelectWordLeft),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.RightArrow, EventModifiers.Shift | EventModifiers.Alt | EventModifiers.FunctionKey), TextSelectOp.SelectWordRight),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.UpArrow, EventModifiers.Shift | EventModifiers.Alt | EventModifiers.FunctionKey), TextSelectOp.SelectParagraphBackward),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.DownArrow, EventModifiers.Shift | EventModifiers.Alt | EventModifiers.FunctionKey), TextSelectOp.SelectParagraphForward),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.LeftArrow, EventModifiers.Shift | EventModifiers.Command | EventModifiers.FunctionKey), TextSelectOp.ExpandSelectGraphicalLineStart),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.RightArrow, EventModifiers.Shift | EventModifiers.Command | EventModifiers.FunctionKey), TextSelectOp.ExpandSelectGraphicalLineEnd),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.UpArrow, EventModifiers.Shift | EventModifiers.Command | EventModifiers.FunctionKey), TextSelectOp.SelectTextStart),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.DownArrow, EventModifiers.Shift | EventModifiers.Command | EventModifiers.FunctionKey), TextSelectOp.SelectTextEnd),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.A, EventModifiers.Command), TextSelectOp.SelectAll),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.C, EventModifiers.Command), TextSelectOp.Copy)
		};

		[TupleElementNames(new string[] { "keyEvent", "operation" })]
		internal static readonly List<ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>> s_WindowsLinuxKeyMappings = new List<ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>>
		{
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.LeftArrow, EventModifiers.Shift | EventModifiers.Control | EventModifiers.FunctionKey), TextSelectOp.SelectToEndOfPreviousWord),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.RightArrow, EventModifiers.Shift | EventModifiers.Control | EventModifiers.FunctionKey), TextSelectOp.SelectToStartOfNextWord),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.UpArrow, EventModifiers.Shift | EventModifiers.Control | EventModifiers.FunctionKey), TextSelectOp.SelectParagraphBackward),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.DownArrow, EventModifiers.Shift | EventModifiers.Control | EventModifiers.FunctionKey), TextSelectOp.SelectParagraphForward),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.Home, EventModifiers.Shift | EventModifiers.FunctionKey), TextSelectOp.SelectGraphicalLineStart),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.End, EventModifiers.Shift | EventModifiers.FunctionKey), TextSelectOp.SelectGraphicalLineEnd),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.A, EventModifiers.Control), TextSelectOp.SelectAll),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.C, EventModifiers.Control), TextSelectOp.Copy),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextSelectOp>(new TextEditingUtilities.KeyEvent(KeyCode.Insert, EventModifiers.Control | EventModifiers.FunctionKey), TextSelectOp.Copy)
		};

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal Action OnCursorIndexChange;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal Action OnSelectIndexChange;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
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
