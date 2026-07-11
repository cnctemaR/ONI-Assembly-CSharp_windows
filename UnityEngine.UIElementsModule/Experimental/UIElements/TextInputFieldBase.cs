using System;
using System.Collections.Generic;
using UnityEngine.Experimental.UIElements.StyleSheets;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Abstract base class used for all text-based fields.</para>
	/// </summary>
	public abstract class TextInputFieldBase<T> : BaseTextControl<T>, ITextInputField, IEventHandler
	{
		public TextInputFieldBase(int maxLength, char maskChar)
		{
			this.maxLength = maxLength;
			this.maskChar = maskChar;
			this.editorEngine = new TextEditorEngine(new TextEditorEngine.OnDetectFocusChangeFunction(this.OnDetectFocusChange), new TextEditorEngine.OnIndexChangeFunction(this.OnCursorIndexChange));
			if (this.touchScreenTextField)
			{
				this.editorEventHandler = new TouchScreenTextEditorEventHandler(this.editorEngine, this);
			}
			else
			{
				this.doubleClickSelectsWord = true;
				this.tripleClickSelectsLine = true;
				this.editorEventHandler = new KeyboardTextEditorEventHandler(this.editorEngine, this);
			}
			this.editorEngine.style = new GUIStyle(this.editorEngine.style);
		}

		public void SelectAll()
		{
			if (this.editorEngine != null)
			{
				this.editorEngine.SelectAll();
			}
		}

		private void UpdateText(string value)
		{
			if (this.text != value)
			{
				using (InputEvent pooled = InputEvent.GetPooled(this.text, value))
				{
					pooled.target = this;
					this.text = value;
					UIElementsUtility.eventDispatcher.DispatchEvent(pooled, base.panel);
				}
			}
		}

		public virtual bool isPasswordField { get; set; }

		public Color selectionColor
		{
			get
			{
				return this.m_SelectionColor.GetSpecifiedValueOrDefault(Color.clear);
			}
		}

		public Color cursorColor
		{
			get
			{
				return this.m_CursorColor.GetSpecifiedValueOrDefault(Color.clear);
			}
		}

		public int cursorIndex
		{
			get
			{
				return this.editorEngine.cursorIndex;
			}
		}

		public int maxLength { get; set; }

		public bool doubleClickSelectsWord { get; set; }

		public bool tripleClickSelectsLine { get; set; }

		public bool isDelayed { get; set; }

		private bool touchScreenTextField
		{
			get
			{
				return TouchScreenKeyboard.isSupported;
			}
		}

		internal bool hasFocus
		{
			get
			{
				return base.elementPanel != null && base.elementPanel.focusController.focusedElement == this;
			}
		}

		internal TextEditorEventHandler editorEventHandler { get; private set; }

		internal TextEditorEngine editorEngine { get; private set; }

		public char maskChar { get; set; }

		public override string text
		{
			protected set
			{
				base.text = value;
				this.editorEngine.text = value;
			}
		}

		private ContextualMenu.MenuAction.StatusFlags CutCopyActionStatus(ContextualMenu.MenuAction a)
		{
			return (!this.editorEngine.hasSelection || this.isPasswordField) ? ContextualMenu.MenuAction.StatusFlags.Disabled : ContextualMenu.MenuAction.StatusFlags.Normal;
		}

		private ContextualMenu.MenuAction.StatusFlags PasteActionStatus(ContextualMenu.MenuAction a)
		{
			return (!this.editorEngine.CanPaste()) ? ContextualMenu.MenuAction.StatusFlags.Disabled : ContextualMenu.MenuAction.StatusFlags.Normal;
		}

		private void Cut(ContextualMenu.MenuAction a)
		{
			this.editorEngine.Cut();
			this.editorEngine.text = this.CullString(this.editorEngine.text);
			this.UpdateText(this.editorEngine.text);
		}

		private void Copy(ContextualMenu.MenuAction a)
		{
			this.editorEngine.Copy();
		}

		private void Paste(ContextualMenu.MenuAction a)
		{
			this.editorEngine.Paste();
			this.editorEngine.text = this.CullString(this.editorEngine.text);
			this.UpdateText(this.editorEngine.text);
		}

		protected override void OnStyleResolved(ICustomStyle style)
		{
			base.OnStyleResolved(style);
			base.effectiveStyle.ApplyCustomProperty("selection-color", ref this.m_SelectionColor);
			base.effectiveStyle.ApplyCustomProperty("cursor-color", ref this.m_CursorColor);
			base.effectiveStyle.WriteToGUIStyle(this.editorEngine.style);
		}

		internal virtual void SyncTextEngine()
		{
			this.editorEngine.text = this.CullString(this.text);
			this.editorEngine.SaveBackup();
			this.editorEngine.position = base.layout;
			this.editorEngine.DetectFocusChange();
		}

		internal string CullString(string s)
		{
			string text;
			if (this.maxLength >= 0 && s != null && s.Length > this.maxLength)
			{
				text = s.Substring(0, this.maxLength);
			}
			else
			{
				text = s;
			}
			return text;
		}

		internal override void DoRepaint(IStylePainter painter)
		{
			if (this.touchScreenTextField)
			{
				TouchScreenTextEditorEventHandler touchScreenTextEditorEventHandler = this.editorEventHandler as TouchScreenTextEditorEventHandler;
				if (touchScreenTextEditorEventHandler != null && this.editorEngine.keyboardOnScreen != null)
				{
					this.UpdateText(this.CullString(this.editorEngine.keyboardOnScreen.text));
					if (this.editorEngine.keyboardOnScreen.status != TouchScreenKeyboard.Status.Visible)
					{
						this.editorEngine.keyboardOnScreen = null;
						GUI.changed = true;
					}
				}
				string text = this.text;
				if (touchScreenTextEditorEventHandler != null && !string.IsNullOrEmpty(touchScreenTextEditorEventHandler.secureText))
				{
					text = "".PadRight(touchScreenTextEditorEventHandler.secureText.Length, this.maskChar);
				}
				base.DoRepaint(painter);
				this.text = text;
			}
			else if (!this.hasFocus)
			{
				base.DoRepaint(painter);
			}
			else
			{
				this.DrawWithTextSelectionAndCursor(painter, this.text);
			}
		}

		internal void DrawWithTextSelectionAndCursor(IStylePainter painter, string newText)
		{
			KeyboardTextEditorEventHandler keyboardTextEditorEventHandler = this.editorEventHandler as KeyboardTextEditorEventHandler;
			if (keyboardTextEditorEventHandler != null)
			{
				keyboardTextEditorEventHandler.PreDrawCursor(newText);
				int cursorIndex = this.editorEngine.cursorIndex;
				int selectIndex = this.editorEngine.selectIndex;
				Rect localPosition = this.editorEngine.localPosition;
				Vector2 scrollOffset = this.editorEngine.scrollOffset;
				IStyle style = base.style;
				TextStylePainterParameters textStylePainterParameters = painter.GetDefaultTextParameters(this);
				textStylePainterParameters.text = " ";
				textStylePainterParameters.wordWrapWidth = 0f;
				textStylePainterParameters.wordWrap = false;
				float num = painter.ComputeTextHeight(textStylePainterParameters);
				float width = base.contentRect.width;
				Input.compositionCursorPos = this.editorEngine.graphicalCursorPos - scrollOffset + new Vector2(localPosition.x, localPosition.y + num);
				Color specifiedValueOrDefault = this.m_CursorColor.GetSpecifiedValueOrDefault(Color.grey);
				int num2 = ((!string.IsNullOrEmpty(Input.compositionString)) ? (cursorIndex + Input.compositionString.Length) : selectIndex);
				painter.DrawBackground(this);
				if (cursorIndex != num2)
				{
					RectStylePainterParameters defaultRectParameters = painter.GetDefaultRectParameters(this);
					defaultRectParameters.color = this.selectionColor;
					defaultRectParameters.border.SetWidth(0f);
					defaultRectParameters.border.SetRadius(0f);
					int num3 = ((cursorIndex >= num2) ? num2 : cursorIndex);
					int num4 = ((cursorIndex <= num2) ? num2 : cursorIndex);
					CursorPositionStylePainterParameters cursorPositionStylePainterParameters = painter.GetDefaultCursorPositionParameters(this);
					cursorPositionStylePainterParameters.text = this.editorEngine.text;
					cursorPositionStylePainterParameters.wordWrapWidth = width;
					cursorPositionStylePainterParameters.cursorIndex = num3;
					Vector2 vector = painter.GetCursorPosition(cursorPositionStylePainterParameters);
					cursorPositionStylePainterParameters.cursorIndex = num4;
					Vector2 vector2 = painter.GetCursorPosition(cursorPositionStylePainterParameters);
					vector -= scrollOffset;
					vector2 -= scrollOffset;
					if (Mathf.Approximately(vector.y, vector2.y))
					{
						defaultRectParameters.rect = new Rect(vector.x, vector.y, vector2.x - vector.x, num);
						painter.DrawRect(defaultRectParameters);
					}
					else
					{
						defaultRectParameters.rect = new Rect(vector.x, vector.y, base.contentRect.xMax - vector.x, num);
						painter.DrawRect(defaultRectParameters);
						float num5 = vector2.y - vector.y - num;
						if (num5 > 0f)
						{
							defaultRectParameters.rect = new Rect(base.contentRect.x, vector.y + num, width, num5);
							painter.DrawRect(defaultRectParameters);
						}
						if (vector2.x != base.contentRect.x)
						{
							defaultRectParameters.rect = new Rect(base.contentRect.x, vector2.y, vector2.x, num);
							painter.DrawRect(defaultRectParameters);
						}
					}
				}
				painter.DrawBorder(this);
				if (!string.IsNullOrEmpty(this.editorEngine.text) && base.contentRect.width > 0f && base.contentRect.height > 0f)
				{
					textStylePainterParameters = painter.GetDefaultTextParameters(this);
					textStylePainterParameters.rect = new Rect(base.contentRect.x - scrollOffset.x, base.contentRect.y - scrollOffset.y, base.contentRect.width, base.contentRect.height);
					textStylePainterParameters.text = this.editorEngine.text;
					painter.DrawText(textStylePainterParameters);
				}
				if (cursorIndex == num2 && style.font != null)
				{
					CursorPositionStylePainterParameters cursorPositionStylePainterParameters = painter.GetDefaultCursorPositionParameters(this);
					cursorPositionStylePainterParameters.text = this.editorEngine.text;
					cursorPositionStylePainterParameters.wordWrapWidth = width;
					cursorPositionStylePainterParameters.cursorIndex = cursorIndex;
					Vector2 vector3 = painter.GetCursorPosition(cursorPositionStylePainterParameters);
					vector3 -= scrollOffset;
					RectStylePainterParameters rectStylePainterParameters = new RectStylePainterParameters
					{
						rect = new Rect(vector3.x, vector3.y, 1f, num),
						color = specifiedValueOrDefault
					};
					painter.DrawRect(rectStylePainterParameters);
				}
				if (this.editorEngine.altCursorPosition != -1)
				{
					CursorPositionStylePainterParameters cursorPositionStylePainterParameters = painter.GetDefaultCursorPositionParameters(this);
					cursorPositionStylePainterParameters.text = this.editorEngine.text.Substring(0, this.editorEngine.altCursorPosition);
					cursorPositionStylePainterParameters.wordWrapWidth = width;
					cursorPositionStylePainterParameters.cursorIndex = this.editorEngine.altCursorPosition;
					Vector2 vector4 = painter.GetCursorPosition(cursorPositionStylePainterParameters);
					vector4 -= scrollOffset;
					RectStylePainterParameters rectStylePainterParameters2 = new RectStylePainterParameters
					{
						rect = new Rect(vector4.x, vector4.y, 1f, num),
						color = specifiedValueOrDefault
					};
					painter.DrawRect(rectStylePainterParameters2);
				}
				keyboardTextEditorEventHandler.PostDrawCursor();
			}
		}

		internal virtual bool AcceptCharacter(char c)
		{
			return true;
		}

		protected virtual void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			if (evt.target is TextInputFieldBase<T>)
			{
				evt.menu.AppendAction("Cut", new Action<ContextualMenu.MenuAction>(this.Cut), new Func<ContextualMenu.MenuAction, ContextualMenu.MenuAction.StatusFlags>(this.CutCopyActionStatus), null);
				evt.menu.AppendAction("Copy", new Action<ContextualMenu.MenuAction>(this.Copy), new Func<ContextualMenu.MenuAction, ContextualMenu.MenuAction.StatusFlags>(this.CutCopyActionStatus), null);
				evt.menu.AppendAction("Paste", new Action<ContextualMenu.MenuAction>(this.Paste), new Func<ContextualMenu.MenuAction, ContextualMenu.MenuAction.StatusFlags>(this.PasteActionStatus), null);
			}
		}

		private void OnDetectFocusChange()
		{
			if (this.editorEngine.m_HasFocus && !this.hasFocus)
			{
				this.editorEngine.OnFocus();
			}
			if (!this.editorEngine.m_HasFocus && this.hasFocus)
			{
				this.editorEngine.OnLostFocus();
			}
		}

		private void OnCursorIndexChange()
		{
			base.Dirty(ChangeType.Repaint);
		}

		protected internal override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);
			if (base.elementPanel != null && base.elementPanel.contextualMenuManager != null)
			{
				base.elementPanel.contextualMenuManager.DisplayMenuIfEventMatches(evt, this);
			}
			if (evt.GetEventTypeId() == EventBase<ContextualMenuPopulateEvent>.TypeId())
			{
				ContextualMenuPopulateEvent contextualMenuPopulateEvent = evt as ContextualMenuPopulateEvent;
				int count = contextualMenuPopulateEvent.menu.MenuItems().Count;
				this.BuildContextualMenu(contextualMenuPopulateEvent);
				if (count > 0 && contextualMenuPopulateEvent.menu.MenuItems().Count > count)
				{
					contextualMenuPopulateEvent.menu.InsertSeparator(null, count);
				}
			}
			this.editorEventHandler.ExecuteDefaultActionAtTarget(evt);
		}

		protected internal override void ExecuteDefaultAction(EventBase evt)
		{
			base.ExecuteDefaultAction(evt);
			this.editorEventHandler.ExecuteDefaultAction(evt);
		}

		bool ITextInputField.hasFocus
		{
			get
			{
				return this.hasFocus;
			}
		}

		string ITextInputField.text
		{
			get
			{
				return this.text;
			}
		}

		void ITextInputField.SyncTextEngine()
		{
			this.SyncTextEngine();
		}

		bool ITextInputField.AcceptCharacter(char c)
		{
			return this.AcceptCharacter(c);
		}

		string ITextInputField.CullString(string s)
		{
			return this.CullString(s);
		}

		void ITextInputField.UpdateText(string value)
		{
			this.UpdateText(value);
		}

		private const string SelectionColorProperty = "selection-color";

		private const string CursorColorProperty = "cursor-color";

		private StyleValue<Color> m_SelectionColor;

		private StyleValue<Color> m_CursorColor;

		internal const int kMaxLengthNone = -1;

		/// <summary>
		///   <para>UxmlTraits for the TextInputFieldBase.</para>
		/// </summary>
		public class TextInputFieldBaseUxmlTraits : BaseTextControl<T>.BaseTextControlUxmlTraits
		{
			protected TextInputFieldBaseUxmlTraits()
			{
				this.m_MaxLength = new UxmlIntAttributeDescription
				{
					name = "maxLength",
					defaultValue = -1
				};
				this.m_Password = new UxmlBoolAttributeDescription
				{
					name = "password"
				};
				this.m_MaskCharacter = new UxmlStringAttributeDescription
				{
					name = "maskCharacter",
					defaultValue = "*"
				};
			}

			public override IEnumerable<UxmlAttributeDescription> uxmlAttributesDescription
			{
				get
				{
					foreach (UxmlAttributeDescription attr in this.<get_uxmlAttributesDescription>__BaseCallProxy0())
					{
						yield return attr;
					}
					yield return this.m_MaxLength;
					yield return this.m_Password;
					yield return this.m_MaskCharacter;
					yield break;
				}
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				TextInputFieldBase<T> textInputFieldBase = (TextInputFieldBase<T>)ve;
				textInputFieldBase.maxLength = this.m_MaxLength.GetValueFromBag(bag);
				textInputFieldBase.isPasswordField = this.m_Password.GetValueFromBag(bag);
				string valueFromBag = this.m_MaskCharacter.GetValueFromBag(bag);
				if (valueFromBag != null && valueFromBag.Length > 0)
				{
					textInputFieldBase.maskChar = valueFromBag[0];
				}
			}

			private UxmlIntAttributeDescription m_MaxLength;

			private UxmlBoolAttributeDescription m_Password;

			private UxmlStringAttributeDescription m_MaskCharacter;
		}
	}
}
