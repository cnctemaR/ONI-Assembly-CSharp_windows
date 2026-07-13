using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Bindings;
using UnityEngine.TextCore.Text;

namespace UnityEngine
{
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal class TextEditingUtilities
	{
		private bool hasSelection
		{
			get
			{
				return this.m_TextSelectingUtility.hasSelection;
			}
		}

		private string SelectedText
		{
			get
			{
				return this.m_TextSelectingUtility.selectedText;
			}
		}

		private int m_iAltCursorPos
		{
			get
			{
				return this.m_TextSelectingUtility.iAltCursorPos;
			}
		}

		internal bool revealCursor
		{
			get
			{
				return this.m_TextSelectingUtility.revealCursor;
			}
			set
			{
				this.m_TextSelectingUtility.revealCursor = value;
			}
		}

		internal int stringCursorIndex
		{
			get
			{
				return this.textHandle.GetCorrespondingStringIndex(this.cursorIndex);
			}
			set
			{
				this.cursorIndex = this.textHandle.GetCorrespondingCodePointIndex(value);
			}
		}

		private int cursorIndex
		{
			get
			{
				return this.m_TextSelectingUtility.cursorIndex;
			}
			set
			{
				this.m_TextSelectingUtility.cursorIndex = value;
			}
		}

		private int cursorIndexNoValidation
		{
			get
			{
				return this.m_TextSelectingUtility.cursorIndexNoValidation;
			}
			set
			{
				this.m_TextSelectingUtility.cursorIndexNoValidation = value;
			}
		}

		private int selectIndexNoValidation
		{
			get
			{
				return this.m_TextSelectingUtility.selectIndexNoValidation;
			}
			set
			{
				this.m_TextSelectingUtility.selectIndexNoValidation = value;
			}
		}

		private int stringCursorIndexNoValidation
		{
			get
			{
				return this.textHandle.GetCorrespondingStringIndex(this.m_TextSelectingUtility.cursorIndexNoValidation);
			}
		}

		internal int stringSelectIndex
		{
			get
			{
				return this.textHandle.GetCorrespondingStringIndex(this.selectIndex);
			}
			set
			{
				this.selectIndex = this.textHandle.GetCorrespondingCodePointIndex(value);
			}
		}

		private int selectIndex
		{
			get
			{
				return this.m_TextSelectingUtility.selectIndex;
			}
			set
			{
				this.m_TextSelectingUtility.selectIndex = value;
			}
		}

		public string text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				bool flag = value == this.m_Text;
				if (!flag)
				{
					this.m_Text = value ?? string.Empty;
					Action onTextChanged = this.OnTextChanged;
					if (onTextChanged != null)
					{
						onTextChanged();
					}
				}
			}
		}

		internal void SetTextWithoutNotify(string value)
		{
			this.m_Text = value;
		}

		public TextEditingUtilities(TextSelectingUtilities selectingUtilities, TextHandle textHandle, string text)
		{
			this.m_TextSelectingUtility = selectingUtilities;
			this.textHandle = textHandle;
			this.m_Text = text;
		}

		public bool UpdateImeState()
		{
			bool flag = GUIUtility.compositionString.Length > 0;
			if (flag)
			{
				bool flag2 = !this.isCompositionActive;
				if (flag2)
				{
					this.m_UpdateImeWindowPosition = true;
					this.ReplaceSelection(string.Empty);
				}
				this.isCompositionActive = true;
			}
			else
			{
				this.isCompositionActive = false;
			}
			return this.isCompositionActive;
		}

		public bool ShouldUpdateImeWindowPosition()
		{
			return this.m_UpdateImeWindowPosition;
		}

		public void SetImeWindowPosition(Vector2 worldPosition)
		{
			Vector2 cursorPositionFromStringIndexUsingCharacterHeight = this.textHandle.GetCursorPositionFromStringIndexUsingCharacterHeight(this.cursorIndex, true);
			GUIUtility.compositionCursorPos = worldPosition + cursorPositionFromStringIndexUsingCharacterHeight;
		}

		public string GeneratePreviewString(bool richText)
		{
			this.RestoreCursorState();
			string compositionString = GUIUtility.compositionString;
			bool flag = this.isCompositionActive;
			string text;
			if (flag)
			{
				text = (richText ? this.text.Insert(this.stringCursorIndex, "<u>" + compositionString + "</u>") : this.text.Insert(this.stringCursorIndex, compositionString));
			}
			else
			{
				text = this.text;
			}
			return text;
		}

		public void EnableCursorPreviewState()
		{
			bool flag = this.m_CursorIndexSavedState != -1;
			if (!flag)
			{
				this.m_CursorIndexSavedState = this.m_TextSelectingUtility.cursorIndexNoValidation;
				this.cursorIndexNoValidation = (this.selectIndexNoValidation = this.m_CursorIndexSavedState + GUIUtility.compositionString.Length);
			}
		}

		public void RestoreCursorState()
		{
			bool flag = this.m_CursorIndexSavedState == -1;
			if (!flag)
			{
				this.cursorIndex = (this.selectIndex = this.m_CursorIndexSavedState);
				this.m_CursorIndexSavedState = -1;
			}
		}

		internal bool HandleKeyEvent(Event e)
		{
			return this.HandleKeyEvent(e.keyCode, e.modifiers);
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal bool HandleKeyEvent(KeyCode key, EventModifiers modifiers)
		{
			TextEditOp? textEditOp = TextEditingUtilities.TextEditOpFromEnum(key, modifiers, SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX);
			bool flag = textEditOp != null;
			bool flag2;
			if (flag)
			{
				this.PerformOperation(textEditOp.Value);
				flag2 = true;
			}
			else
			{
				flag2 = false;
			}
			return flag2;
		}

		internal static TextEditOp? TextEditOpFromEnum(KeyCode key, EventModifiers modifiers, bool IsMacOsFamily)
		{
			modifiers &= ~EventModifiers.CapsLock;
			TextEditingUtilities.KeyEvent keyEvent = new TextEditingUtilities.KeyEvent(key, modifiers);
			foreach (ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp> valueTuple in TextEditingUtilities.s_GlobalKeyMappings)
			{
				bool flag = valueTuple.Item1 == keyEvent;
				if (flag)
				{
					return new TextEditOp?(valueTuple.Item2);
				}
			}
			foreach (ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp> valueTuple2 in (IsMacOsFamily ? TextEditingUtilities.s_MacKeyMappings : TextEditingUtilities.s_WindowsLinuxKeyMappings))
			{
				bool flag2 = valueTuple2.Item1 == keyEvent;
				if (flag2)
				{
					return new TextEditOp?(valueTuple2.Item2);
				}
			}
			return null;
		}

		private void PerformOperation(TextEditOp operation)
		{
			this.revealCursor = true;
			switch (operation)
			{
			case TextEditOp.MoveLeft:
				this.m_TextSelectingUtility.MoveLeft();
				return;
			case TextEditOp.MoveRight:
				this.m_TextSelectingUtility.MoveRight();
				return;
			case TextEditOp.MoveUp:
				this.m_TextSelectingUtility.MoveUp();
				return;
			case TextEditOp.MoveDown:
				this.m_TextSelectingUtility.MoveDown();
				return;
			case TextEditOp.MoveLineStart:
				this.m_TextSelectingUtility.MoveLineStart();
				return;
			case TextEditOp.MoveLineEnd:
				this.m_TextSelectingUtility.MoveLineEnd();
				return;
			case TextEditOp.MoveTextStart:
				this.m_TextSelectingUtility.MoveTextStart();
				return;
			case TextEditOp.MoveTextEnd:
				this.m_TextSelectingUtility.MoveTextEnd();
				return;
			case TextEditOp.MoveGraphicalLineStart:
				this.m_TextSelectingUtility.MoveGraphicalLineStart();
				return;
			case TextEditOp.MoveGraphicalLineEnd:
				this.m_TextSelectingUtility.MoveGraphicalLineEnd();
				return;
			case TextEditOp.MoveWordLeft:
				this.m_TextSelectingUtility.MoveWordLeft();
				return;
			case TextEditOp.MoveWordRight:
				this.m_TextSelectingUtility.MoveWordRight();
				return;
			case TextEditOp.MoveParagraphForward:
				this.m_TextSelectingUtility.MoveParagraphForward();
				return;
			case TextEditOp.MoveParagraphBackward:
				this.m_TextSelectingUtility.MoveParagraphBackward();
				return;
			case TextEditOp.MoveToStartOfNextWord:
				this.m_TextSelectingUtility.MoveToStartOfNextWord();
				return;
			case TextEditOp.MoveToEndOfPreviousWord:
				this.m_TextSelectingUtility.MoveToEndOfPreviousWord();
				return;
			case TextEditOp.Delete:
				this.Delete();
				return;
			case TextEditOp.Backspace:
				this.Backspace();
				return;
			case TextEditOp.DeleteWordBack:
				this.DeleteWordBack();
				return;
			case TextEditOp.DeleteWordForward:
				this.DeleteWordForward();
				return;
			case TextEditOp.DeleteLineBack:
				this.DeleteLineBack();
				return;
			case TextEditOp.Cut:
				this.Cut();
				return;
			case TextEditOp.Paste:
				this.Paste();
				return;
			}
			Debug.Log("Unimplemented: " + operation.ToString());
		}

		public bool DeleteLineBack()
		{
			this.RestoreCursorState();
			bool hasSelection = this.hasSelection;
			bool flag;
			if (hasSelection)
			{
				this.DeleteSelection();
				flag = true;
			}
			else
			{
				bool useAdvancedText = this.textHandle.useAdvancedText;
				if (useAdvancedText)
				{
					int firstCharacterIndexOnLine = this.textHandle.GetFirstCharacterIndexOnLine(this.cursorIndex);
					bool flag2 = firstCharacterIndexOnLine != this.cursorIndex;
					if (flag2)
					{
						this.text = this.text.Remove(firstCharacterIndexOnLine, this.stringCursorIndex - firstCharacterIndexOnLine);
						this.cursorIndex = (this.selectIndex = firstCharacterIndexOnLine);
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
				else
				{
					LineInfo lineInfoFromCharacterIndex = this.textHandle.GetLineInfoFromCharacterIndex(this.cursorIndex);
					int firstCharacterIndex = lineInfoFromCharacterIndex.firstCharacterIndex;
					int correspondingStringIndex = this.textHandle.GetCorrespondingStringIndex(firstCharacterIndex);
					bool flag3 = firstCharacterIndex != this.cursorIndex;
					if (flag3)
					{
						this.text = this.text.Remove(correspondingStringIndex, this.stringCursorIndex - correspondingStringIndex);
						this.cursorIndex = (this.selectIndex = firstCharacterIndex);
						flag = true;
					}
					else
					{
						flag = false;
					}
				}
			}
			return flag;
		}

		public bool DeleteWordBack()
		{
			this.RestoreCursorState();
			bool hasSelection = this.hasSelection;
			bool flag;
			if (hasSelection)
			{
				this.DeleteSelection();
				flag = true;
			}
			else
			{
				int num = this.m_TextSelectingUtility.FindEndOfPreviousWord(this.cursorIndex);
				bool flag2 = this.cursorIndex != num;
				if (flag2)
				{
					int correspondingStringIndex = this.textHandle.GetCorrespondingStringIndex(num);
					this.text = this.text.Remove(correspondingStringIndex, this.stringCursorIndex - correspondingStringIndex);
					this.selectIndex = (this.cursorIndex = num);
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
			this.RestoreCursorState();
			bool hasSelection = this.hasSelection;
			bool flag;
			if (hasSelection)
			{
				this.DeleteSelection();
				flag = true;
			}
			else
			{
				int num = this.m_TextSelectingUtility.FindStartOfNextWord(this.cursorIndex);
				bool flag2 = this.cursorIndex < this.text.Length;
				if (flag2)
				{
					int correspondingStringIndex = this.textHandle.GetCorrespondingStringIndex(num);
					this.text = this.text.Remove(this.stringCursorIndex, correspondingStringIndex - this.stringCursorIndex);
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
			this.RestoreCursorState();
			bool hasSelection = this.hasSelection;
			bool flag;
			if (hasSelection)
			{
				this.DeleteSelection();
				flag = true;
			}
			else
			{
				bool flag2 = this.stringCursorIndex < this.text.Length;
				if (flag2)
				{
					bool useAdvancedText = this.textHandle.useAdvancedText;
					int num;
					if (useAdvancedText)
					{
						num = Mathf.Abs(this.textHandle.NextCodePointIndex(this.cursorIndex) - this.cursorIndex);
					}
					else
					{
						num = this.textHandle.textInfo.textElementInfo[this.cursorIndex].stringLength;
					}
					this.text = this.text.Remove(this.stringCursorIndex, num);
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		public bool Backspace()
		{
			this.RestoreCursorState();
			bool hasSelection = this.hasSelection;
			bool flag;
			if (hasSelection)
			{
				this.DeleteSelection();
				flag = true;
			}
			else
			{
				bool flag2 = this.cursorIndex > 0;
				if (flag2)
				{
					int num = this.m_TextSelectingUtility.PreviousCodePointIndex(this.cursorIndex);
					bool useAdvancedText = this.textHandle.useAdvancedText;
					int num2;
					if (useAdvancedText)
					{
						num2 = Mathf.Abs(this.cursorIndex - num);
					}
					else
					{
						num2 = this.textHandle.textInfo.textElementInfo[this.cursorIndex - 1].stringLength;
					}
					this.text = this.text.Remove(this.stringCursorIndex - num2, num2);
					this.cursorIndex = (this.textHandle.useAdvancedText ? Math.Max(0, this.cursorIndex - num2) : num);
					this.selectIndex = (this.textHandle.useAdvancedText ? Math.Max(0, this.selectIndex - num2) : num);
					this.m_TextSelectingUtility.ClearCursorPos();
					flag = true;
				}
				else
				{
					flag = false;
				}
			}
			return flag;
		}

		public bool DeleteSelection()
		{
			bool flag = this.cursorIndex == this.selectIndex;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = this.cursorIndex < this.selectIndex;
				if (flag3)
				{
					this.text = this.text.Substring(0, this.stringCursorIndex) + this.text.Substring(this.stringSelectIndex, this.text.Length - this.stringSelectIndex);
					this.selectIndex = this.cursorIndex;
				}
				else
				{
					this.text = this.text.Substring(0, this.stringSelectIndex) + this.text.Substring(this.stringCursorIndex, this.text.Length - this.stringCursorIndex);
					this.cursorIndex = this.selectIndex;
				}
				this.m_TextSelectingUtility.ClearCursorPos();
				flag2 = true;
			}
			return flag2;
		}

		public void ReplaceSelection(string replace)
		{
			this.RestoreCursorState();
			this.DeleteSelection();
			this.text = this.text.Insert(this.stringCursorIndex, replace);
			int num = (this.textHandle.useAdvancedText ? replace.Length : new StringInfo(replace).LengthInTextElements);
			int num2 = this.cursorIndexNoValidation + num;
			this.cursorIndexNoValidation = num2;
			this.selectIndexNoValidation = num2;
			this.m_TextSelectingUtility.ClearCursorPos();
		}

		public bool Insert(char c)
		{
			bool flag = char.IsHighSurrogate(c);
			bool flag2;
			if (flag)
			{
				this.m_HighSurrogate = c;
				flag2 = false;
			}
			else
			{
				bool flag3 = char.IsLowSurrogate(c);
				if (flag3)
				{
					char c2 = c;
					string text = new string(new char[] { this.m_HighSurrogate, c2 });
					this.ReplaceSelection(text.ToString());
					flag2 = true;
				}
				else
				{
					this.ReplaceSelection(c.ToString());
					flag2 = true;
				}
			}
			return flag2;
		}

		public void MoveSelectionToAltCursor()
		{
			this.RestoreCursorState();
			bool flag = this.m_iAltCursorPos == -1;
			if (!flag)
			{
				int iAltCursorPos = this.m_iAltCursorPos;
				string selectedText = this.SelectedText;
				this.text = this.text.Insert(iAltCursorPos, selectedText);
				bool flag2 = iAltCursorPos < this.cursorIndex;
				if (flag2)
				{
					this.cursorIndex += selectedText.Length;
					this.selectIndex += selectedText.Length;
				}
				this.DeleteSelection();
				this.selectIndex = (this.cursorIndex = iAltCursorPos);
				this.m_TextSelectingUtility.ClearCursorPos();
			}
		}

		public bool CanPaste()
		{
			return GUIUtility.systemCopyBuffer.Length != 0;
		}

		public bool Cut()
		{
			this.m_TextSelectingUtility.Copy();
			return this.DeleteSelection();
		}

		public bool Paste()
		{
			this.RestoreCursorState();
			string text = GUIUtility.systemCopyBuffer;
			bool flag = text != "";
			bool flag3;
			if (flag)
			{
				bool flag2 = !this.multiline;
				if (flag2)
				{
					text = TextEditingUtilities.ReplaceNewlinesWithSpaces(text);
				}
				this.ReplaceSelection(text);
				flag3 = true;
			}
			else
			{
				flag3 = false;
			}
			return flag3;
		}

		private static string ReplaceNewlinesWithSpaces(string value)
		{
			value = value.Replace("\r\n", " ");
			value = value.Replace('\n', ' ');
			value = value.Replace('\r', ' ');
			return value;
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal void OnBlur()
		{
			this.revealCursor = false;
			this.isCompositionActive = false;
			this.RestoreCursorState();
			this.m_TextSelectingUtility.SelectNone();
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal bool TouchScreenKeyboardShouldBeUsed()
		{
			RuntimePlatform platform = Application.platform;
			RuntimePlatform runtimePlatform = platform;
			RuntimePlatform runtimePlatform2 = runtimePlatform;
			bool flag;
			if (runtimePlatform2 != RuntimePlatform.Android && runtimePlatform2 - RuntimePlatform.WebGLPlayer > 3)
			{
				flag = TouchScreenKeyboard.isSupported;
			}
			else
			{
				flag = !TouchScreenKeyboard.isInPlaceEditingAllowed;
			}
			return flag;
		}

		private TextSelectingUtilities m_TextSelectingUtility;

		internal TextHandle textHandle;

		private int m_CursorIndexSavedState = -1;

		[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
		internal bool isCompositionActive;

		private bool m_UpdateImeWindowPosition;

		internal Action OnTextChanged;

		public bool multiline = false;

		private string m_Text;

		[TupleElementNames(new string[] { "keyEvent", "operation" })]
		internal static readonly List<ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>> s_GlobalKeyMappings = new List<ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>>
		{
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.LeftArrow, EventModifiers.FunctionKey), TextEditOp.MoveLeft),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.RightArrow, EventModifiers.FunctionKey), TextEditOp.MoveRight),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.UpArrow, EventModifiers.FunctionKey), TextEditOp.MoveUp),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.DownArrow, EventModifiers.FunctionKey), TextEditOp.MoveDown),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.Delete, EventModifiers.FunctionKey), TextEditOp.Delete),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.Backspace, EventModifiers.FunctionKey), TextEditOp.Backspace),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.Backspace, EventModifiers.Shift | EventModifiers.FunctionKey), TextEditOp.Backspace)
		};

		[TupleElementNames(new string[] { "keyEvent", "operation" })]
		internal static readonly List<ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>> s_MacKeyMappings = new List<ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>>
		{
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.LeftArrow, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.MoveGraphicalLineStart),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.RightArrow, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.MoveGraphicalLineEnd),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.LeftArrow, EventModifiers.Alt | EventModifiers.FunctionKey), TextEditOp.MoveWordLeft),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.RightArrow, EventModifiers.Alt | EventModifiers.FunctionKey), TextEditOp.MoveWordRight),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.UpArrow, EventModifiers.Alt | EventModifiers.FunctionKey), TextEditOp.MoveParagraphBackward),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.DownArrow, EventModifiers.Alt | EventModifiers.FunctionKey), TextEditOp.MoveParagraphForward),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.LeftArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveGraphicalLineStart),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.RightArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveGraphicalLineEnd),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.UpArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveTextStart),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.DownArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveTextEnd),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.X, EventModifiers.Command), TextEditOp.Cut),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.V, EventModifiers.Command), TextEditOp.Paste),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.D, EventModifiers.Control), TextEditOp.Delete),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.H, EventModifiers.Control), TextEditOp.Backspace),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.B, EventModifiers.Control), TextEditOp.MoveLeft),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.F, EventModifiers.Control), TextEditOp.MoveRight),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.A, EventModifiers.Control), TextEditOp.MoveLineStart),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.E, EventModifiers.Control), TextEditOp.MoveLineEnd),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.Delete, EventModifiers.Alt | EventModifiers.FunctionKey), TextEditOp.DeleteWordForward),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.Backspace, EventModifiers.Alt | EventModifiers.FunctionKey), TextEditOp.DeleteWordBack),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.Backspace, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.DeleteLineBack)
		};

		[TupleElementNames(new string[] { "keyEvent", "operation" })]
		internal static readonly List<ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>> s_WindowsLinuxKeyMappings = new List<ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>>
		{
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.Home, EventModifiers.FunctionKey), TextEditOp.MoveGraphicalLineStart),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.End, EventModifiers.FunctionKey), TextEditOp.MoveGraphicalLineEnd),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.LeftArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveWordLeft),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.RightArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveWordRight),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.UpArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveParagraphBackward),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.DownArrow, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.MoveParagraphForward),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.LeftArrow, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.MoveToEndOfPreviousWord),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.RightArrow, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.MoveToStartOfNextWord),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.UpArrow, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.MoveParagraphBackward),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.DownArrow, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.MoveParagraphForward),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.Delete, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.DeleteWordForward),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.Backspace, EventModifiers.Control | EventModifiers.FunctionKey), TextEditOp.DeleteWordBack),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.Backspace, EventModifiers.Command | EventModifiers.FunctionKey), TextEditOp.DeleteLineBack),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.X, EventModifiers.Control), TextEditOp.Cut),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.V, EventModifiers.Control), TextEditOp.Paste),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.Delete, EventModifiers.Shift | EventModifiers.FunctionKey), TextEditOp.Cut),
			new ValueTuple<TextEditingUtilities.KeyEvent, TextEditOp>(new TextEditingUtilities.KeyEvent(KeyCode.Insert, EventModifiers.Shift | EventModifiers.FunctionKey), TextEditOp.Paste)
		};

		private char m_HighSurrogate;

		internal struct KeyEvent : IEquatable<TextEditingUtilities.KeyEvent>
		{
			public KeyEvent(KeyCode key, EventModifiers modifiers)
			{
				this.key = key;
				this.modifiers = modifiers;
			}

			public KeyCode key { readonly get; set; }

			public EventModifiers modifiers { readonly get; set; }

			[CompilerGenerated]
			public override readonly string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("KeyEvent");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[CompilerGenerated]
			private readonly bool PrintMembers(StringBuilder builder)
			{
				builder.Append("key = ");
				builder.Append(this.key.ToString());
				builder.Append(", modifiers = ");
				builder.Append(this.modifiers.ToString());
				return true;
			}

			[CompilerGenerated]
			public static bool operator !=(TextEditingUtilities.KeyEvent left, TextEditingUtilities.KeyEvent right)
			{
				return !(left == right);
			}

			[CompilerGenerated]
			public static bool operator ==(TextEditingUtilities.KeyEvent left, TextEditingUtilities.KeyEvent right)
			{
				return left.Equals(right);
			}

			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return EqualityComparer<KeyCode>.Default.GetHashCode(this.<key>k__BackingField) * -1521134295 + EqualityComparer<EventModifiers>.Default.GetHashCode(this.<modifiers>k__BackingField);
			}

			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is TextEditingUtilities.KeyEvent && this.Equals((TextEditingUtilities.KeyEvent)obj);
			}

			[CompilerGenerated]
			public readonly bool Equals(TextEditingUtilities.KeyEvent other)
			{
				return EqualityComparer<KeyCode>.Default.Equals(this.<key>k__BackingField, other.<key>k__BackingField) && EqualityComparer<EventModifiers>.Default.Equals(this.<modifiers>k__BackingField, other.<modifiers>k__BackingField);
			}

			[CompilerGenerated]
			public readonly void Deconstruct(out KeyCode key, out EventModifiers modifiers)
			{
				key = this.key;
				modifiers = this.modifiers;
			}
		}
	}
}
