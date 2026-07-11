using System;
using UnityEngine.Experimental.UIElements.StyleSheets;

namespace UnityEngine.Experimental.UIElements
{
	public abstract class TextInputFieldBase<T> : BaseField<T>, ITextInputField, IEventHandler, ITextElement
	{
		public TextInputFieldBase(int maxLength, char maskChar)
		{
			base.requireMeasureFunction = true;
			this.m_Text = "";
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

		public string text
		{
			get
			{
				return this.m_Text;
			}
			protected set
			{
				if (!(this.m_Text == value))
				{
					this.m_Text = value;
					this.editorEngine.text = value;
					base.IncrementVersion(VersionChangeType.Layout);
				}
			}
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
					this.SendEvent(pooled);
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

		public int selectIndex
		{
			get
			{
				return this.editorEngine.selectIndex;
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

		private DropdownMenu.MenuAction.StatusFlags CutCopyActionStatus(DropdownMenu.MenuAction a)
		{
			return (!this.editorEngine.hasSelection || this.isPasswordField) ? DropdownMenu.MenuAction.StatusFlags.Disabled : DropdownMenu.MenuAction.StatusFlags.Normal;
		}

		private DropdownMenu.MenuAction.StatusFlags PasteActionStatus(DropdownMenu.MenuAction a)
		{
			return (!this.editorEngine.CanPaste()) ? DropdownMenu.MenuAction.StatusFlags.Disabled : DropdownMenu.MenuAction.StatusFlags.Normal;
		}

		private void Cut(DropdownMenu.MenuAction a)
		{
			this.editorEngine.Cut();
			this.editorEngine.text = this.CullString(this.editorEngine.text);
			this.UpdateText(this.editorEngine.text);
		}

		private void Copy(DropdownMenu.MenuAction a)
		{
			this.editorEngine.Copy();
		}

		private void Paste(DropdownMenu.MenuAction a)
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

		protected override void DoRepaint(IStylePainter painter)
		{
			IStylePainterInternal stylePainterInternal = (IStylePainterInternal)painter;
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
				this.text = text;
			}
			else if (!this.hasFocus)
			{
				stylePainterInternal.DrawText(this.text);
			}
			else
			{
				this.DrawWithTextSelectionAndCursor(stylePainterInternal, this.text);
			}
		}

		internal void DrawWithTextSelectionAndCursor(IStylePainterInternal painter, string newText)
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
				float num = TextNative.ComputeTextScaling(base.worldTransform, GUIUtility.pixelsPerPoint);
				TextStylePainterParameters textStylePainterParameters = TextStylePainterParameters.GetDefault(this, this.text);
				textStylePainterParameters.text = " ";
				textStylePainterParameters.wordWrapWidth = 0f;
				textStylePainterParameters.wordWrap = false;
				TextNativeSettings textNativeSettings = textStylePainterParameters.GetTextNativeSettings(num);
				float num2 = TextNative.ComputeTextHeight(textNativeSettings);
				float num3 = ((!this.editorEngine.multiline) ? 0f : base.contentRect.width);
				Input.compositionCursorPos = this.editorEngine.graphicalCursorPos - scrollOffset + new Vector2(localPosition.x, localPosition.y + num2);
				Color specifiedValueOrDefault = this.m_CursorColor.GetSpecifiedValueOrDefault(Color.grey);
				int num4 = ((!string.IsNullOrEmpty(Input.compositionString)) ? (cursorIndex + Input.compositionString.Length) : selectIndex);
				if (cursorIndex != num4)
				{
					RectStylePainterParameters @default = RectStylePainterParameters.GetDefault(this);
					@default.color = this.selectionColor;
					@default.border.SetWidth(0f);
					@default.border.SetRadius(0f);
					int num5 = ((cursorIndex >= num4) ? num4 : cursorIndex);
					int num6 = ((cursorIndex <= num4) ? num4 : cursorIndex);
					CursorPositionStylePainterParameters cursorPositionStylePainterParameters = CursorPositionStylePainterParameters.GetDefault(this, this.text);
					cursorPositionStylePainterParameters.text = this.editorEngine.text;
					cursorPositionStylePainterParameters.wordWrapWidth = num3;
					cursorPositionStylePainterParameters.cursorIndex = num5;
					textNativeSettings = cursorPositionStylePainterParameters.GetTextNativeSettings(num);
					Vector2 vector = TextNative.GetCursorPosition(textNativeSettings, cursorPositionStylePainterParameters.rect, num5);
					Vector2 vector2 = TextNative.GetCursorPosition(textNativeSettings, cursorPositionStylePainterParameters.rect, num6);
					vector -= scrollOffset;
					vector2 -= scrollOffset;
					if (Mathf.Approximately(vector.y, vector2.y))
					{
						@default.rect = new Rect(vector.x, vector.y, vector2.x - vector.x, num2);
						painter.DrawRect(@default);
					}
					else
					{
						@default.rect = new Rect(vector.x, vector.y, base.contentRect.xMax - vector.x, num2);
						painter.DrawRect(@default);
						float num7 = vector2.y - vector.y - num2;
						if (num7 > 0f)
						{
							@default.rect = new Rect(base.contentRect.x, vector.y + num2, num3, num7);
							painter.DrawRect(@default);
						}
						if (vector2.x != base.contentRect.x)
						{
							@default.rect = new Rect(base.contentRect.x, vector2.y, vector2.x, num2);
							painter.DrawRect(@default);
						}
					}
				}
				if (!string.IsNullOrEmpty(this.editorEngine.text) && base.contentRect.width > 0f && base.contentRect.height > 0f)
				{
					textStylePainterParameters = TextStylePainterParameters.GetDefault(this, this.text);
					textStylePainterParameters.rect = new Rect(base.contentRect.x - scrollOffset.x, base.contentRect.y - scrollOffset.y, base.contentRect.width, base.contentRect.height);
					textStylePainterParameters.text = this.editorEngine.text;
					painter.DrawText(textStylePainterParameters);
				}
				if (cursorIndex == num4 && style.font != null)
				{
					CursorPositionStylePainterParameters cursorPositionStylePainterParameters = CursorPositionStylePainterParameters.GetDefault(this, this.text);
					cursorPositionStylePainterParameters.text = this.editorEngine.text;
					cursorPositionStylePainterParameters.wordWrapWidth = num3;
					cursorPositionStylePainterParameters.cursorIndex = cursorIndex;
					textNativeSettings = cursorPositionStylePainterParameters.GetTextNativeSettings(num);
					Vector2 vector3 = TextNative.GetCursorPosition(textNativeSettings, cursorPositionStylePainterParameters.rect, cursorPositionStylePainterParameters.cursorIndex);
					vector3 -= scrollOffset;
					RectStylePainterParameters rectStylePainterParameters = new RectStylePainterParameters
					{
						rect = new Rect(vector3.x, vector3.y, 1f, num2),
						color = specifiedValueOrDefault
					};
					painter.DrawRect(rectStylePainterParameters);
				}
				if (this.editorEngine.altCursorPosition != -1)
				{
					CursorPositionStylePainterParameters cursorPositionStylePainterParameters = CursorPositionStylePainterParameters.GetDefault(this, this.text);
					cursorPositionStylePainterParameters.text = this.editorEngine.text.Substring(0, this.editorEngine.altCursorPosition);
					cursorPositionStylePainterParameters.wordWrapWidth = num3;
					cursorPositionStylePainterParameters.cursorIndex = this.editorEngine.altCursorPosition;
					textNativeSettings = cursorPositionStylePainterParameters.GetTextNativeSettings(num);
					Vector2 vector4 = TextNative.GetCursorPosition(textNativeSettings, cursorPositionStylePainterParameters.rect, cursorPositionStylePainterParameters.cursorIndex);
					vector4 -= scrollOffset;
					RectStylePainterParameters rectStylePainterParameters2 = new RectStylePainterParameters
					{
						rect = new Rect(vector4.x, vector4.y, 1f, num2),
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
				evt.menu.AppendAction("Cut", new Action<DropdownMenu.MenuAction>(this.Cut), new Func<DropdownMenu.MenuAction, DropdownMenu.MenuAction.StatusFlags>(this.CutCopyActionStatus), null);
				evt.menu.AppendAction("Copy", new Action<DropdownMenu.MenuAction>(this.Copy), new Func<DropdownMenu.MenuAction, DropdownMenu.MenuAction.StatusFlags>(this.CutCopyActionStatus), null);
				evt.menu.AppendAction("Paste", new Action<DropdownMenu.MenuAction>(this.Paste), new Func<DropdownMenu.MenuAction, DropdownMenu.MenuAction.StatusFlags>(this.PasteActionStatus), null);
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
			base.IncrementVersion(VersionChangeType.Repaint);
		}

		protected internal override Vector2 DoMeasure(float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode)
		{
			return TextElement.MeasureVisualElementTextSize(this, this.m_Text, width, widthMode, height, heightMode);
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

		string ITextElement.text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				this.m_Text = value;
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

		private string m_Text;

		internal const int kMaxLengthNone = -1;

		public new class UxmlTraits : BaseField<T>.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				TextInputFieldBase<T> textInputFieldBase = (TextInputFieldBase<T>)ve;
				textInputFieldBase.maxLength = this.m_MaxLength.GetValueFromBag(bag, cc);
				textInputFieldBase.isPasswordField = this.m_Password.GetValueFromBag(bag, cc);
				string valueFromBag = this.m_MaskCharacter.GetValueFromBag(bag, cc);
				if (valueFromBag != null && valueFromBag.Length > 0)
				{
					textInputFieldBase.maskChar = valueFromBag[0];
				}
				((ITextElement)ve).text = this.m_Text.GetValueFromBag(bag, cc);
			}

			private UxmlIntAttributeDescription m_MaxLength = new UxmlIntAttributeDescription
			{
				name = "max-length",
				obsoleteNames = new string[] { "maxLength" },
				defaultValue = -1
			};

			private UxmlBoolAttributeDescription m_Password = new UxmlBoolAttributeDescription
			{
				name = "password"
			};

			private UxmlStringAttributeDescription m_MaskCharacter = new UxmlStringAttributeDescription
			{
				name = "mask-character",
				obsoleteNames = new string[] { "maskCharacter" },
				defaultValue = "*"
			};

			private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
			{
				name = "text"
			};
		}
	}
}
