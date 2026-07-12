using System;
using System.Diagnostics;

namespace UnityEngine.UIElements
{
	public abstract class TextInputBaseField<TValueType> : BaseField<TValueType>
	{
		protected TextInputBaseField<TValueType>.TextInputBase textInputBase
		{
			get
			{
				return this.m_TextInputBase;
			}
		}

		internal TextHandle textHandle { get; private set; } = TextHandle.New();

		public string text
		{
			get
			{
				return this.m_TextInputBase.text;
			}
			protected set
			{
				this.m_TextInputBase.text = value;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		protected event Action<bool> onIsReadOnlyChanged;

		public bool isReadOnly
		{
			get
			{
				return this.m_TextInputBase.isReadOnly;
			}
			set
			{
				this.m_TextInputBase.isReadOnly = value;
				Action<bool> action = this.onIsReadOnlyChanged;
				if (action != null)
				{
					action(value);
				}
			}
		}

		public bool isPasswordField
		{
			get
			{
				return this.m_TextInputBase.isPasswordField;
			}
			set
			{
				bool flag = this.m_TextInputBase.isPasswordField == value;
				if (!flag)
				{
					this.m_TextInputBase.isPasswordField = value;
					this.m_TextInputBase.IncrementVersion(VersionChangeType.Repaint);
				}
			}
		}

		public Color selectionColor
		{
			get
			{
				return this.m_TextInputBase.selectionColor;
			}
		}

		public Color cursorColor
		{
			get
			{
				return this.m_TextInputBase.cursorColor;
			}
		}

		public int cursorIndex
		{
			get
			{
				return this.m_TextInputBase.cursorIndex;
			}
		}

		public int selectIndex
		{
			get
			{
				return this.m_TextInputBase.selectIndex;
			}
		}

		public int maxLength
		{
			get
			{
				return this.m_TextInputBase.maxLength;
			}
			set
			{
				this.m_TextInputBase.maxLength = value;
			}
		}

		public bool doubleClickSelectsWord
		{
			get
			{
				return this.m_TextInputBase.doubleClickSelectsWord;
			}
			set
			{
				this.m_TextInputBase.doubleClickSelectsWord = value;
			}
		}

		public bool tripleClickSelectsLine
		{
			get
			{
				return this.m_TextInputBase.tripleClickSelectsLine;
			}
			set
			{
				this.m_TextInputBase.tripleClickSelectsLine = value;
			}
		}

		public bool isDelayed
		{
			get
			{
				return this.m_TextInputBase.isDelayed;
			}
			set
			{
				this.m_TextInputBase.isDelayed = value;
			}
		}

		public char maskChar
		{
			get
			{
				return this.m_TextInputBase.maskChar;
			}
			set
			{
				this.m_TextInputBase.maskChar = value;
			}
		}

		internal Vector2 MeasureTextSize(string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode)
		{
			return TextElement.MeasureVisualElementTextSize(this, textToMeasure, width, widthMode, height, heightMode, this.textHandle);
		}

		internal TextEditorEventHandler editorEventHandler
		{
			get
			{
				return this.m_TextInputBase.editorEventHandler;
			}
		}

		internal TextEditorEngine editorEngine
		{
			get
			{
				return this.m_TextInputBase.editorEngine;
			}
		}

		internal bool hasFocus
		{
			get
			{
				return this.m_TextInputBase.hasFocus;
			}
		}

		public void SelectAll()
		{
			this.m_TextInputBase.SelectAll();
		}

		internal void SyncTextEngine()
		{
			this.m_TextInputBase.SyncTextEngine();
		}

		internal void DrawWithTextSelectionAndCursor(MeshGenerationContext mgc, string newText)
		{
			this.m_TextInputBase.DrawWithTextSelectionAndCursor(mgc, newText, base.scaledPixelsPerPoint);
		}

		protected TextInputBaseField(int maxLength, char maskChar, TextInputBaseField<TValueType>.TextInputBase textInputBase)
			: this(null, maxLength, maskChar, textInputBase)
		{
		}

		protected TextInputBaseField(string label, int maxLength, char maskChar, TextInputBaseField<TValueType>.TextInputBase textInputBase)
			: base(label, textInputBase)
		{
			base.tabIndex = 0;
			base.delegatesFocus = true;
			base.labelElement.tabIndex = -1;
			base.AddToClassList(TextInputBaseField<TValueType>.ussClassName);
			base.labelElement.AddToClassList(TextInputBaseField<TValueType>.labelUssClassName);
			base.visualInput.AddToClassList(TextInputBaseField<TValueType>.inputUssClassName);
			this.m_TextInputBase = textInputBase;
			this.m_TextInputBase.maxLength = maxLength;
			this.m_TextInputBase.maskChar = maskChar;
			base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.NoTrickleDown);
		}

		private void OnAttachToPanel(AttachToPanelEvent e)
		{
			TextHandle textHandle = this.textHandle;
			textHandle.useLegacy = e.destinationPanel.contextType == ContextType.Editor;
			this.textHandle = textHandle;
		}

		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);
			bool flag = evt == null;
			if (!flag)
			{
				bool flag2 = evt.eventTypeId == EventBase<KeyDownEvent>.TypeId();
				if (flag2)
				{
					KeyDownEvent keyDownEvent = evt as KeyDownEvent;
					char? c = ((keyDownEvent != null) ? new char?(keyDownEvent.character) : null);
					int? num = ((c != null) ? new int?((int)c.GetValueOrDefault()) : null);
					int num2 = 3;
					bool flag3;
					if (!((num.GetValueOrDefault() == num2) & (num != null)))
					{
						c = ((keyDownEvent != null) ? new char?(keyDownEvent.character) : null);
						num = ((c != null) ? new int?((int)c.GetValueOrDefault()) : null);
						num2 = 10;
						flag3 = (num.GetValueOrDefault() == num2) & (num != null);
					}
					else
					{
						flag3 = true;
					}
					bool flag4 = flag3;
					if (flag4)
					{
						VisualElement visualInput = base.visualInput;
						if (visualInput != null)
						{
							visualInput.Focus();
						}
					}
				}
				else
				{
					bool flag5 = evt.eventTypeId == EventBase<FocusInEvent>.TypeId();
					if (flag5)
					{
						bool flag6 = evt.leafTarget == this || evt.leafTarget == base.labelElement;
						if (flag6)
						{
							this.m_VisualInputTabIndex = base.visualInput.tabIndex;
							base.visualInput.tabIndex = -1;
						}
					}
					else
					{
						bool flag7 = evt.eventTypeId == EventBase<FocusEvent>.TypeId();
						if (flag7)
						{
							base.delegatesFocus = false;
						}
						else
						{
							bool flag8 = evt.eventTypeId == EventBase<BlurEvent>.TypeId();
							if (flag8)
							{
								base.delegatesFocus = true;
								bool flag9 = evt.leafTarget == this || evt.leafTarget == base.labelElement;
								if (flag9)
								{
									base.visualInput.tabIndex = this.m_VisualInputTabIndex;
								}
							}
						}
					}
				}
			}
		}

		private static CustomStyleProperty<Color> s_SelectionColorProperty = new CustomStyleProperty<Color>("--unity-selection-color");

		private static CustomStyleProperty<Color> s_CursorColorProperty = new CustomStyleProperty<Color>("--unity-cursor-color");

		private int m_VisualInputTabIndex;

		private TextInputBaseField<TValueType>.TextInputBase m_TextInputBase;

		internal const int kMaxLengthNone = -1;

		internal const char kMaskCharDefault = '*';

		public new static readonly string ussClassName = "unity-base-text-field";

		public new static readonly string labelUssClassName = TextInputBaseField<TValueType>.ussClassName + "__label";

		public new static readonly string inputUssClassName = TextInputBaseField<TValueType>.ussClassName + "__input";

		public static readonly string textInputUssName = "unity-text-input";

		public new class UxmlTraits : BaseFieldTraits<string, UxmlStringAttributeDescription>
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				TextInputBaseField<TValueType> textInputBaseField = (TextInputBaseField<TValueType>)ve;
				textInputBaseField.maxLength = this.m_MaxLength.GetValueFromBag(bag, cc);
				textInputBaseField.isPasswordField = this.m_Password.GetValueFromBag(bag, cc);
				textInputBaseField.isReadOnly = this.m_IsReadOnly.GetValueFromBag(bag, cc);
				string valueFromBag = this.m_MaskCharacter.GetValueFromBag(bag, cc);
				bool flag = !string.IsNullOrEmpty(valueFromBag);
				if (flag)
				{
					textInputBaseField.maskChar = valueFromBag[0];
				}
				textInputBaseField.text = this.m_Text.GetValueFromBag(bag, cc);
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
				defaultValue = '*'.ToString()
			};

			private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
			{
				name = "text"
			};

			private UxmlBoolAttributeDescription m_IsReadOnly = new UxmlBoolAttributeDescription
			{
				name = "readonly"
			};
		}

		protected abstract class TextInputBase : VisualElement, ITextInputField, IEventHandler, ITextElement
		{
			private void SaveValueAndText()
			{
				this.m_OriginalText = this.text;
			}

			private void RestoreValueAndText()
			{
				this.text = this.m_OriginalText;
			}

			public void SelectAll()
			{
				TextEditorEngine editorEngine = this.editorEngine;
				if (editorEngine != null)
				{
					editorEngine.SelectAll();
				}
			}

			internal void SelectNone()
			{
				TextEditorEngine editorEngine = this.editorEngine;
				if (editorEngine != null)
				{
					editorEngine.SelectNone();
				}
			}

			private void UpdateText(string value)
			{
				bool flag = this.text != value;
				if (flag)
				{
					using (InputEvent pooled = InputEvent.GetPooled(this.text, value))
					{
						pooled.target = base.parent;
						this.text = value;
						VisualElement parent = base.parent;
						if (parent != null)
						{
							parent.SendEvent(pooled);
						}
					}
				}
			}

			protected virtual TValueType StringToValue(string str)
			{
				throw new NotSupportedException();
			}

			internal void UpdateValueFromText()
			{
				TValueType tvalueType = this.StringToValue(this.text);
				TextInputBaseField<TValueType> textInputBaseField = (TextInputBaseField<TValueType>)base.parent;
				textInputBaseField.value = tvalueType;
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

			bool ITextInputField.isReadOnly
			{
				get
				{
					return this.isReadOnly || !base.enabledInHierarchy;
				}
			}

			public bool isReadOnly { get; set; }

			public int maxLength { get; set; }

			public char maskChar { get; set; }

			public virtual bool isPasswordField { get; set; }

			public bool doubleClickSelectsWord { get; set; }

			public bool tripleClickSelectsLine { get; set; }

			internal bool isDelayed { get; set; }

			internal bool isDragging { get; set; }

			private bool touchScreenTextField
			{
				get
				{
					return TouchScreenKeyboard.isSupported && !TouchScreenKeyboard.isInPlaceEditingAllowed;
				}
			}

			private bool touchScreenTextFieldChanged
			{
				get
				{
					return this.m_TouchScreenTextFieldInitialized != this.touchScreenTextField;
				}
			}

			public Color selectionColor
			{
				get
				{
					return this.m_SelectionColor;
				}
			}

			public Color cursorColor
			{
				get
				{
					return this.m_CursorColor;
				}
			}

			internal bool hasFocus
			{
				get
				{
					return base.elementPanel != null && base.elementPanel.focusController.GetLeafFocusedElement() == this;
				}
			}

			internal TextEditorEventHandler editorEventHandler { get; private set; }

			internal TextEditorEngine editorEngine { get; private set; }

			public string text
			{
				get
				{
					return this.m_Text;
				}
				set
				{
					bool flag = this.m_Text == value;
					if (!flag)
					{
						this.m_Text = value;
						this.editorEngine.text = value;
						base.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
					}
				}
			}

			internal TextInputBase()
			{
				this.isReadOnly = false;
				base.focusable = true;
				base.AddToClassList(TextInputBaseField<TValueType>.inputUssClassName);
				this.m_Text = string.Empty;
				base.name = TextInputBaseField<string>.textInputUssName;
				base.requireMeasureFunction = true;
				this.editorEngine = new TextEditorEngine(new TextEditorEngine.OnDetectFocusChangeFunction(this.OnDetectFocusChange), new TextEditorEngine.OnIndexChangeFunction(this.OnCursorIndexChange));
				this.editorEngine.style.richText = false;
				this.InitTextEditorEventHandler();
				this.editorEngine.style = new GUIStyle(this.editorEngine.style);
				base.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnCustomStyleResolved), TrickleDown.NoTrickleDown);
				base.RegisterCallback<AttachToPanelEvent>(new EventCallback<AttachToPanelEvent>(this.OnAttachToPanel), TrickleDown.NoTrickleDown);
				base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(this.OnGenerateVisualContent));
			}

			private void InitTextEditorEventHandler()
			{
				this.m_TouchScreenTextFieldInitialized = this.touchScreenTextField;
				bool touchScreenTextFieldInitialized = this.m_TouchScreenTextFieldInitialized;
				if (touchScreenTextFieldInitialized)
				{
					this.editorEventHandler = new TouchScreenTextEditorEventHandler(this.editorEngine, this);
				}
				else
				{
					this.doubleClickSelectsWord = true;
					this.tripleClickSelectsLine = true;
					this.editorEventHandler = new KeyboardTextEditorEventHandler(this.editorEngine, this);
				}
			}

			private DropdownMenuAction.Status CutCopyActionStatus(DropdownMenuAction a)
			{
				return (this.editorEngine.hasSelection && !this.isPasswordField) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
			}

			private DropdownMenuAction.Status PasteActionStatus(DropdownMenuAction a)
			{
				return this.editorEngine.CanPaste() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
			}

			private void ProcessMenuCommand(string command)
			{
				using (ExecuteCommandEvent pooled = CommandEventBase<ExecuteCommandEvent>.GetPooled(command))
				{
					pooled.target = this;
					this.SendEvent(pooled);
				}
			}

			private void Cut(DropdownMenuAction a)
			{
				this.ProcessMenuCommand("Cut");
			}

			private void Copy(DropdownMenuAction a)
			{
				this.ProcessMenuCommand("Copy");
			}

			private void Paste(DropdownMenuAction a)
			{
				this.ProcessMenuCommand("Paste");
			}

			private void OnCustomStyleResolved(CustomStyleResolvedEvent e)
			{
				Color clear = Color.clear;
				Color clear2 = Color.clear;
				ICustomStyle customStyle = e.customStyle;
				bool flag = customStyle.TryGetValue(TextInputBaseField<TValueType>.s_SelectionColorProperty, out clear);
				if (flag)
				{
					this.m_SelectionColor = clear;
				}
				bool flag2 = customStyle.TryGetValue(TextInputBaseField<TValueType>.s_CursorColorProperty, out clear2);
				if (flag2)
				{
					this.m_CursorColor = clear2;
				}
				TextInputBaseField<TValueType>.TextInputBase.SyncGUIStyle(this, this.editorEngine.style);
			}

			private void OnAttachToPanel(AttachToPanelEvent e)
			{
				this.m_TextHandle.useLegacy = e.destinationPanel.contextType == ContextType.Editor;
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
				bool flag = this.maxLength >= 0 && s != null && s.Length > this.maxLength;
				string text;
				if (flag)
				{
					text = s.Substring(0, this.maxLength);
				}
				else
				{
					text = s;
				}
				return text;
			}

			internal void OnGenerateVisualContent(MeshGenerationContext mgc)
			{
				string text = this.text;
				bool isPasswordField = this.isPasswordField;
				if (isPasswordField)
				{
					text = "".PadRight(this.text.Length, this.maskChar);
				}
				bool touchScreenTextFieldInitialized = this.m_TouchScreenTextFieldInitialized;
				if (touchScreenTextFieldInitialized)
				{
					TouchScreenTextEditorEventHandler touchScreenTextEditorEventHandler = this.editorEventHandler as TouchScreenTextEditorEventHandler;
					bool flag = touchScreenTextEditorEventHandler != null;
					if (flag)
					{
						mgc.Text(MeshGenerationContextUtils.TextParams.MakeStyleBased(this, text), this.m_TextHandle, base.scaledPixelsPerPoint);
					}
				}
				else
				{
					bool flag2 = !this.hasFocus;
					if (flag2)
					{
						mgc.Text(MeshGenerationContextUtils.TextParams.MakeStyleBased(this, text), this.m_TextHandle, base.scaledPixelsPerPoint);
					}
					else
					{
						this.DrawWithTextSelectionAndCursor(mgc, text, base.scaledPixelsPerPoint);
					}
				}
			}

			internal void DrawWithTextSelectionAndCursor(MeshGenerationContext mgc, string newText, float pixelsPerPoint)
			{
				Color color = ((base.panel.contextType == ContextType.Editor) ? UIElementsUtility.editorPlayModeTintColor : Color.white);
				KeyboardTextEditorEventHandler keyboardTextEditorEventHandler = this.editorEventHandler as KeyboardTextEditorEventHandler;
				bool flag = keyboardTextEditorEventHandler == null;
				if (!flag)
				{
					keyboardTextEditorEventHandler.PreDrawCursor(newText);
					int cursorIndex = this.editorEngine.cursorIndex;
					int selectIndex = this.editorEngine.selectIndex;
					Rect localPosition = this.editorEngine.localPosition;
					Vector2 scrollOffset = this.editorEngine.scrollOffset;
					float num = TextHandle.ComputeTextScaling(base.worldTransform, pixelsPerPoint);
					MeshGenerationContextUtils.TextParams textParams = MeshGenerationContextUtils.TextParams.MakeStyleBased(this, this.text);
					textParams.text = " ";
					textParams.wordWrapWidth = 0f;
					textParams.wordWrap = false;
					float num2 = this.m_TextHandle.ComputeTextHeight(textParams, num);
					float num3 = 0f;
					bool flag2 = this.editorEngine.multiline && base.resolvedStyle.whiteSpace == WhiteSpace.Normal;
					if (flag2)
					{
						num3 = base.contentRect.width;
					}
					Vector2 vector = this.editorEngine.graphicalCursorPos - scrollOffset;
					vector.y += num2;
					GUIUtility.compositionCursorPos = this.LocalToWorld(vector);
					Color cursorColor = this.cursorColor;
					int num4 = (string.IsNullOrEmpty(GUIUtility.compositionString) ? selectIndex : (cursorIndex + GUIUtility.compositionString.Length));
					bool flag3 = cursorIndex != num4 && !this.isDragging;
					if (flag3)
					{
						int num5 = ((cursorIndex < num4) ? cursorIndex : num4);
						int num6 = ((cursorIndex > num4) ? cursorIndex : num4);
						CursorPositionStylePainterParameters cursorPositionStylePainterParameters = CursorPositionStylePainterParameters.GetDefault(this, this.text);
						cursorPositionStylePainterParameters.text = this.editorEngine.text;
						cursorPositionStylePainterParameters.wordWrapWidth = num3;
						cursorPositionStylePainterParameters.cursorIndex = num5;
						Vector2 vector2 = this.m_TextHandle.GetCursorPosition(cursorPositionStylePainterParameters, num);
						cursorPositionStylePainterParameters.cursorIndex = num6;
						Vector2 vector3 = this.m_TextHandle.GetCursorPosition(cursorPositionStylePainterParameters, num);
						vector2 -= scrollOffset;
						vector3 -= scrollOffset;
						bool flag4 = Mathf.Approximately(vector2.y, vector3.y);
						if (flag4)
						{
							mgc.Rectangle(new MeshGenerationContextUtils.RectangleParams
							{
								rect = new Rect(vector2.x, vector2.y, vector3.x - vector2.x, num2),
								color = this.selectionColor,
								playmodeTintColor = color
							});
						}
						else
						{
							mgc.Rectangle(new MeshGenerationContextUtils.RectangleParams
							{
								rect = new Rect(vector2.x, vector2.y, base.contentRect.xMax - vector2.x, num2),
								color = this.selectionColor,
								playmodeTintColor = color
							});
							float num7 = vector3.y - vector2.y - num2;
							bool flag5 = num7 > 0f;
							if (flag5)
							{
								mgc.Rectangle(new MeshGenerationContextUtils.RectangleParams
								{
									rect = new Rect(base.contentRect.xMin, vector2.y + num2, base.contentRect.width, num7),
									color = this.selectionColor,
									playmodeTintColor = color
								});
							}
							bool flag6 = vector3.x != base.contentRect.x;
							if (flag6)
							{
								mgc.Rectangle(new MeshGenerationContextUtils.RectangleParams
								{
									rect = new Rect(base.contentRect.xMin, vector3.y, vector3.x, num2),
									color = this.selectionColor,
									playmodeTintColor = color
								});
							}
						}
					}
					bool flag7 = !string.IsNullOrEmpty(this.editorEngine.text) && base.contentRect.width > 0f && base.contentRect.height > 0f;
					if (flag7)
					{
						textParams = MeshGenerationContextUtils.TextParams.MakeStyleBased(this, this.text);
						textParams.rect = new Rect(base.contentRect.x - scrollOffset.x, base.contentRect.y - scrollOffset.y, base.contentRect.width + scrollOffset.x, base.contentRect.height + scrollOffset.y);
						textParams.text = this.editorEngine.text;
						mgc.Text(textParams, this.m_TextHandle, base.scaledPixelsPerPoint);
					}
					bool flag8 = !this.isReadOnly && !this.isDragging;
					if (flag8)
					{
						bool flag9 = cursorIndex == num4 && base.computedStyle.unityFont.value != null;
						if (flag9)
						{
							CursorPositionStylePainterParameters cursorPositionStylePainterParameters = CursorPositionStylePainterParameters.GetDefault(this, this.text);
							cursorPositionStylePainterParameters.text = this.editorEngine.text;
							cursorPositionStylePainterParameters.wordWrapWidth = num3;
							cursorPositionStylePainterParameters.cursorIndex = cursorIndex;
							Vector2 vector4 = this.m_TextHandle.GetCursorPosition(cursorPositionStylePainterParameters, num);
							vector4 -= scrollOffset;
							mgc.Rectangle(new MeshGenerationContextUtils.RectangleParams
							{
								rect = new Rect(vector4.x, vector4.y, 1f, num2),
								color = cursorColor,
								playmodeTintColor = color
							});
						}
						bool flag10 = this.editorEngine.altCursorPosition != -1;
						if (flag10)
						{
							CursorPositionStylePainterParameters cursorPositionStylePainterParameters = CursorPositionStylePainterParameters.GetDefault(this, this.text);
							cursorPositionStylePainterParameters.text = this.editorEngine.text.Substring(0, this.editorEngine.altCursorPosition);
							cursorPositionStylePainterParameters.wordWrapWidth = num3;
							cursorPositionStylePainterParameters.cursorIndex = this.editorEngine.altCursorPosition;
							Vector2 vector5 = this.m_TextHandle.GetCursorPosition(cursorPositionStylePainterParameters, num);
							vector5 -= scrollOffset;
							mgc.Rectangle(new MeshGenerationContextUtils.RectangleParams
							{
								rect = new Rect(vector5.x, vector5.y, 1f, num2),
								color = cursorColor,
								playmodeTintColor = color
							});
						}
					}
					keyboardTextEditorEventHandler.PostDrawCursor();
				}
			}

			internal virtual bool AcceptCharacter(char c)
			{
				return !this.isReadOnly && base.enabledInHierarchy;
			}

			protected virtual void BuildContextualMenu(ContextualMenuPopulateEvent evt)
			{
				bool flag = ((evt != null) ? evt.target : null) is TextInputBaseField<TValueType>.TextInputBase;
				if (flag)
				{
					bool flag2 = !this.isReadOnly;
					if (flag2)
					{
						evt.menu.AppendAction("Cut", new Action<DropdownMenuAction>(this.Cut), new Func<DropdownMenuAction, DropdownMenuAction.Status>(this.CutCopyActionStatus), null);
					}
					evt.menu.AppendAction("Copy", new Action<DropdownMenuAction>(this.Copy), new Func<DropdownMenuAction, DropdownMenuAction.Status>(this.CutCopyActionStatus), null);
					bool flag3 = !this.isReadOnly;
					if (flag3)
					{
						evt.menu.AppendAction("Paste", new Action<DropdownMenuAction>(this.Paste), new Func<DropdownMenuAction, DropdownMenuAction.Status>(this.PasteActionStatus), null);
					}
				}
			}

			private void OnDetectFocusChange()
			{
				bool flag = this.editorEngine.m_HasFocus && !this.hasFocus;
				if (flag)
				{
					this.editorEngine.OnFocus();
				}
				bool flag2 = !this.editorEngine.m_HasFocus && this.hasFocus;
				if (flag2)
				{
					this.editorEngine.OnLostFocus();
				}
			}

			private void OnCursorIndexChange()
			{
				base.IncrementVersion(VersionChangeType.Repaint);
			}

			protected internal override Vector2 DoMeasure(float desiredWidth, VisualElement.MeasureMode widthMode, float desiredHeight, VisualElement.MeasureMode heightMode)
			{
				string text = this.m_Text;
				bool flag = string.IsNullOrEmpty(text);
				if (flag)
				{
					text = " ";
				}
				return TextElement.MeasureVisualElementTextSize(this, text, desiredWidth, widthMode, desiredHeight, heightMode, this.m_TextHandle);
			}

			protected override void ExecuteDefaultActionAtTarget(EventBase evt)
			{
				base.ExecuteDefaultActionAtTarget(evt);
				bool flag = base.elementPanel != null && base.elementPanel.contextualMenuManager != null;
				if (flag)
				{
					base.elementPanel.contextualMenuManager.DisplayMenuIfEventMatches(evt, this);
				}
				long? num = ((evt != null) ? new long?(evt.eventTypeId) : null);
				long num2 = EventBase<ContextualMenuPopulateEvent>.TypeId();
				bool flag2 = (num.GetValueOrDefault() == num2) & (num != null);
				if (flag2)
				{
					ContextualMenuPopulateEvent contextualMenuPopulateEvent = evt as ContextualMenuPopulateEvent;
					int count = contextualMenuPopulateEvent.menu.MenuItems().Count;
					this.BuildContextualMenu(contextualMenuPopulateEvent);
					bool flag3 = count > 0 && contextualMenuPopulateEvent.menu.MenuItems().Count > count;
					if (flag3)
					{
						contextualMenuPopulateEvent.menu.InsertSeparator(null, count);
					}
				}
				else
				{
					bool flag4 = evt.eventTypeId == EventBase<FocusInEvent>.TypeId();
					if (flag4)
					{
						this.SaveValueAndText();
						bool touchScreenTextFieldChanged = this.touchScreenTextFieldChanged;
						if (touchScreenTextFieldChanged)
						{
							this.InitTextEditorEventHandler();
						}
						bool flag5 = this.m_HardwareKeyboardPoller == null;
						if (flag5)
						{
							this.m_HardwareKeyboardPoller = base.schedule.Execute(delegate
							{
								bool touchScreenTextFieldChanged2 = this.touchScreenTextFieldChanged;
								if (touchScreenTextFieldChanged2)
								{
									this.InitTextEditorEventHandler();
									this.Blur();
								}
							}).Every(250L);
						}
						else
						{
							this.m_HardwareKeyboardPoller.Resume();
						}
					}
					else
					{
						bool flag6 = evt.eventTypeId == EventBase<FocusOutEvent>.TypeId();
						if (flag6)
						{
							bool flag7 = this.m_HardwareKeyboardPoller != null;
							if (flag7)
							{
								this.m_HardwareKeyboardPoller.Pause();
							}
						}
						else
						{
							bool flag8 = evt.eventTypeId == EventBase<KeyDownEvent>.TypeId();
							if (flag8)
							{
								KeyDownEvent keyDownEvent = evt as KeyDownEvent;
								bool flag9 = keyDownEvent != null && keyDownEvent.keyCode == KeyCode.Escape;
								if (flag9)
								{
									this.RestoreValueAndText();
									base.parent.Focus();
								}
							}
						}
					}
				}
				this.editorEventHandler.ExecuteDefaultActionAtTarget(evt);
			}

			protected override void ExecuteDefaultAction(EventBase evt)
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

			TextEditorEngine ITextInputField.editorEngine
			{
				get
				{
					return this.editorEngine;
				}
			}

			bool ITextInputField.isDelayed
			{
				get
				{
					return this.isDelayed;
				}
			}

			void ITextInputField.UpdateValueFromText()
			{
				this.UpdateValueFromText();
			}

			private void DeferGUIStyleRectSync()
			{
				base.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnPercentResolved), TrickleDown.NoTrickleDown);
			}

			private void OnPercentResolved(GeometryChangedEvent evt)
			{
				base.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnPercentResolved), TrickleDown.NoTrickleDown);
				GUIStyle style = this.editorEngine.style;
				int num = (int)base.resolvedStyle.marginLeft;
				int num2 = (int)base.resolvedStyle.marginTop;
				int num3 = (int)base.resolvedStyle.marginRight;
				int num4 = (int)base.resolvedStyle.marginBottom;
				TextInputBaseField<TValueType>.TextInputBase.AssignRect(style.margin, num, num2, num3, num4);
				num = (int)base.resolvedStyle.paddingLeft;
				num2 = (int)base.resolvedStyle.paddingTop;
				num3 = (int)base.resolvedStyle.paddingRight;
				num4 = (int)base.resolvedStyle.paddingBottom;
				TextInputBaseField<TValueType>.TextInputBase.AssignRect(style.padding, num, num2, num3, num4);
			}

			private static void SyncGUIStyle(TextInputBaseField<TValueType>.TextInputBase textInput, GUIStyle style)
			{
				ComputedStyle computedStyle = textInput.computedStyle;
				style.alignment = computedStyle.unityTextAlign.value;
				style.wordWrap = computedStyle.whiteSpace.value == WhiteSpace.Normal;
				style.clipping = ((computedStyle.overflow.value == OverflowInternal.Visible) ? TextClipping.Overflow : TextClipping.Clip);
				bool flag = computedStyle.unityFont.value != null;
				if (flag)
				{
					style.font = computedStyle.unityFont.value;
				}
				style.fontSize = (int)computedStyle.fontSize.value.value;
				style.fontStyle = computedStyle.unityFontStyleAndWeight.value;
				int num = computedStyle.unitySliceLeft.value;
				int num2 = computedStyle.unitySliceTop.value;
				int num3 = computedStyle.unitySliceRight.value;
				int num4 = computedStyle.unitySliceBottom.value;
				TextInputBaseField<TValueType>.TextInputBase.AssignRect(style.border, num, num2, num3, num4);
				bool flag2 = TextInputBaseField<TValueType>.TextInputBase.IsLayoutUsingPercent(textInput);
				if (flag2)
				{
					textInput.DeferGUIStyleRectSync();
				}
				else
				{
					num = (int)computedStyle.marginLeft.value.value;
					num2 = (int)computedStyle.marginTop.value.value;
					num3 = (int)computedStyle.marginRight.value.value;
					num4 = (int)computedStyle.marginBottom.value.value;
					TextInputBaseField<TValueType>.TextInputBase.AssignRect(style.margin, num, num2, num3, num4);
					num = (int)computedStyle.paddingLeft.value.value;
					num2 = (int)computedStyle.paddingTop.value.value;
					num3 = (int)computedStyle.paddingRight.value.value;
					num4 = (int)computedStyle.paddingBottom.value.value;
					TextInputBaseField<TValueType>.TextInputBase.AssignRect(style.padding, num, num2, num3, num4);
				}
			}

			private static bool IsLayoutUsingPercent(VisualElement ve)
			{
				ComputedStyle computedStyle = ve.computedStyle;
				bool flag = computedStyle.marginLeft.value.unit == LengthUnit.Percent || computedStyle.marginTop.value.unit == LengthUnit.Percent || computedStyle.marginRight.value.unit == LengthUnit.Percent || computedStyle.marginBottom.value.unit == LengthUnit.Percent;
				bool flag2;
				if (flag)
				{
					flag2 = true;
				}
				else
				{
					bool flag3 = computedStyle.paddingLeft.value.unit == LengthUnit.Percent || computedStyle.paddingTop.value.unit == LengthUnit.Percent || computedStyle.paddingRight.value.unit == LengthUnit.Percent || computedStyle.paddingBottom.value.unit == LengthUnit.Percent;
					flag2 = flag3;
				}
				return flag2;
			}

			private static void AssignRect(RectOffset rect, int left, int top, int right, int bottom)
			{
				rect.left = left;
				rect.top = top;
				rect.right = right;
				rect.bottom = bottom;
			}

			private string m_OriginalText;

			private bool m_TouchScreenTextFieldInitialized;

			private IVisualElementScheduledItem m_HardwareKeyboardPoller = null;

			private Color m_SelectionColor = Color.clear;

			private Color m_CursorColor = Color.grey;

			private TextHandle m_TextHandle = TextHandle.New();

			private string m_Text;
		}
	}
}
