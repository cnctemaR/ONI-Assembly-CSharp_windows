using System;
using System.Diagnostics;

namespace UnityEngine.UIElements
{
	public abstract class TextInputBaseField<TValueType> : BaseField<TValueType>
	{
		protected internal TextInputBaseField<TValueType>.TextInputBase textInputBase
		{
			get
			{
				return this.m_TextInputBase;
			}
		}

		public string text
		{
			get
			{
				return this.m_TextInputBase.text;
			}
			protected internal set
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
				return this.textEdition.isReadOnly;
			}
			set
			{
				this.textEdition.isReadOnly = value;
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

		public bool autoCorrection
		{
			get
			{
				return this.textEdition.autoCorrection;
			}
			set
			{
				this.textEdition.autoCorrection = value;
			}
		}

		public bool hideMobileInput
		{
			get
			{
				return this.textEdition.hideMobileInput;
			}
			set
			{
				this.textEdition.hideMobileInput = value;
			}
		}

		public TouchScreenKeyboardType keyboardType
		{
			get
			{
				return this.textEdition.keyboardType;
			}
			set
			{
				this.textEdition.keyboardType = value;
			}
		}

		public TouchScreenKeyboard touchScreenKeyboard
		{
			get
			{
				return this.textEdition.touchScreenKeyboard;
			}
		}

		public ITextSelection textSelection
		{
			get
			{
				return this.m_TextInputBase.textElement.selection;
			}
		}

		public ITextEdition textEdition
		{
			get
			{
				return this.m_TextInputBase.textElement.edition;
			}
		}

		public Color selectionColor
		{
			get
			{
				return this.textSelection.selectionColor;
			}
		}

		public Color cursorColor
		{
			get
			{
				return this.textSelection.cursorColor;
			}
		}

		public int cursorIndex
		{
			get
			{
				return this.textSelection.cursorIndex;
			}
			set
			{
				this.textSelection.cursorIndex = value;
			}
		}

		public Vector2 cursorPosition
		{
			get
			{
				return this.textSelection.cursorPosition;
			}
		}

		public int selectIndex
		{
			get
			{
				return this.textSelection.selectIndex;
			}
			set
			{
				this.textSelection.selectIndex = value;
			}
		}

		public void SelectAll()
		{
			this.textSelection.SelectAll();
		}

		public void SelectNone()
		{
			this.textSelection.SelectNone();
		}

		public void SelectRange(int cursorIndex, int selectionIndex)
		{
			this.textSelection.SelectRange(cursorIndex, selectionIndex);
		}

		public bool selectAllOnFocus
		{
			get
			{
				return this.textSelection.selectAllOnFocus;
			}
			set
			{
				this.textSelection.selectAllOnFocus = value;
			}
		}

		public bool selectAllOnMouseUp
		{
			get
			{
				return this.textSelection.selectAllOnMouseUp;
			}
			set
			{
				this.textSelection.selectAllOnMouseUp = value;
			}
		}

		public int maxLength
		{
			get
			{
				return this.textEdition.maxLength;
			}
			set
			{
				this.textEdition.maxLength = value;
			}
		}

		public bool doubleClickSelectsWord
		{
			get
			{
				return this.textSelection.doubleClickSelectsWord;
			}
			set
			{
				this.textSelection.doubleClickSelectsWord = value;
			}
		}

		public bool tripleClickSelectsLine
		{
			get
			{
				return this.textSelection.tripleClickSelectsLine;
			}
			set
			{
				this.textSelection.tripleClickSelectsLine = value;
			}
		}

		public bool isDelayed
		{
			get
			{
				return this.textEdition.isDelayed;
			}
			set
			{
				this.textEdition.isDelayed = value;
			}
		}

		public char maskChar
		{
			get
			{
				return this.textEdition.maskChar;
			}
			set
			{
				this.textEdition.maskChar = value;
			}
		}

		public bool SetVerticalScrollerVisibility(ScrollerVisibility sv)
		{
			return this.textInputBase.SetVerticalScrollerVisibility(sv);
		}

		public Vector2 MeasureTextSize(string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode)
		{
			return TextUtilities.MeasureVisualElementTextSize(this.m_TextInputBase.textElement, textToMeasure, width, widthMode, height, heightMode);
		}

		internal bool hasFocus
		{
			get
			{
				return this.textInputBase.textElement.hasFocus;
			}
		}

		protected abstract string ValueToString(TValueType value);

		protected abstract TValueType StringToValue(string str);

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
			base.visualInput.AddToClassList(TextInputBaseField<TValueType>.singleLineInputUssClassName);
			this.m_TextInputBase = textInputBase;
			this.m_TextInputBase.maxLength = maxLength;
			this.m_TextInputBase.maskChar = maskChar;
			base.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnFieldCustomStyleResolved), TrickleDown.NoTrickleDown);
		}

		private void OnFieldCustomStyleResolved(CustomStyleResolvedEvent e)
		{
			this.m_TextInputBase.OnInputCustomStyleResolved(e);
		}

		[EventInterest(new Type[]
		{
			typeof(NavigationSubmitEvent),
			typeof(FocusInEvent),
			typeof(FocusEvent),
			typeof(BlurEvent)
		})]
		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			base.ExecuteDefaultActionAtTarget(evt);
			bool isReadOnly = this.textEdition.isReadOnly;
			if (!isReadOnly)
			{
				bool flag = evt.eventTypeId == EventBase<NavigationSubmitEvent>.TypeId() && evt.leafTarget != this.textInputBase.textElement;
				if (flag)
				{
					this.textInputBase.textElement.Focus();
				}
				else
				{
					bool flag2 = evt.eventTypeId == EventBase<FocusInEvent>.TypeId();
					if (flag2)
					{
						bool showMixedValue = base.showMixedValue;
						if (showMixedValue)
						{
							((INotifyValueChanged<string>)this.textInputBase.textElement).SetValueWithoutNotify(null);
						}
						bool flag3 = evt.leafTarget == this || evt.leafTarget == base.labelElement;
						if (flag3)
						{
							this.m_VisualInputTabIndex = this.textInputBase.textElement.tabIndex;
							this.textInputBase.textElement.tabIndex = -1;
						}
					}
					else
					{
						bool flag4 = evt.eventTypeId == EventBase<FocusEvent>.TypeId();
						if (flag4)
						{
							base.delegatesFocus = false;
						}
						else
						{
							bool flag5 = evt.eventTypeId == EventBase<BlurEvent>.TypeId();
							if (flag5)
							{
								bool showMixedValue2 = base.showMixedValue;
								if (showMixedValue2)
								{
									this.UpdateMixedValueContent();
								}
								base.delegatesFocus = true;
								bool flag6 = evt.leafTarget == this || evt.leafTarget == base.labelElement;
								if (flag6)
								{
									this.textInputBase.textElement.tabIndex = this.m_VisualInputTabIndex;
								}
							}
						}
					}
				}
			}
		}

		protected override void UpdateMixedValueContent()
		{
			bool showMixedValue = base.showMixedValue;
			if (showMixedValue)
			{
				((INotifyValueChanged<string>)this.textInputBase.textElement).SetValueWithoutNotify(BaseField<TValueType>.mixedValueString);
				base.AddToClassList(BaseField<TValueType>.mixedValueLabelUssClassName);
				VisualElement visualInput = base.visualInput;
				if (visualInput != null)
				{
					visualInput.AddToClassList(BaseField<TValueType>.mixedValueLabelUssClassName);
				}
			}
			else
			{
				this.UpdateTextFromValue();
				VisualElement visualInput2 = base.visualInput;
				if (visualInput2 != null)
				{
					visualInput2.RemoveFromClassList(BaseField<TValueType>.mixedValueLabelUssClassName);
				}
				base.RemoveFromClassList(BaseField<TValueType>.mixedValueLabelUssClassName);
			}
		}

		internal virtual void UpdateValueFromText()
		{
			this.value = this.StringToValue(this.text);
		}

		internal virtual void UpdateTextFromValue()
		{
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

		internal static readonly string multilineContainerClassName = TextInputBaseField<TValueType>.ussClassName + "__multiline-container";

		public static readonly string singleLineInputUssClassName = TextInputBaseField<TValueType>.inputUssClassName + "--single-line";

		public static readonly string multilineInputUssClassName = TextInputBaseField<TValueType>.inputUssClassName + "--multiline";

		internal static readonly string multilineInputWithScrollViewUssClassName = TextInputBaseField<TValueType>.multilineInputUssClassName + "--scroll-view";

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
				textInputBaseField.isDelayed = this.m_IsDelayed.GetValueFromBag(bag, cc);
				textInputBaseField.hideMobileInput = this.m_HideMobileInput.GetValueFromBag(bag, cc);
				textInputBaseField.keyboardType = this.m_KeyboardType.GetValueFromBag(bag, cc);
				textInputBaseField.autoCorrection = this.m_AutoCorrection.GetValueFromBag(bag, cc);
				string valueFromBag = this.m_MaskCharacter.GetValueFromBag(bag, cc);
				textInputBaseField.maskChar = (string.IsNullOrEmpty(valueFromBag) ? '*' : valueFromBag[0]);
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

			private UxmlBoolAttributeDescription m_IsReadOnly = new UxmlBoolAttributeDescription
			{
				name = "readonly"
			};

			private UxmlBoolAttributeDescription m_IsDelayed = new UxmlBoolAttributeDescription
			{
				name = "is-delayed"
			};

			private UxmlBoolAttributeDescription m_HideMobileInput = new UxmlBoolAttributeDescription
			{
				name = "hide-mobile-input"
			};

			private UxmlEnumAttributeDescription<TouchScreenKeyboardType> m_KeyboardType = new UxmlEnumAttributeDescription<TouchScreenKeyboardType>
			{
				name = "keyboard-type"
			};

			private UxmlBoolAttributeDescription m_AutoCorrection = new UxmlBoolAttributeDescription
			{
				name = "auto-correction"
			};
		}

		protected internal abstract class TextInputBase : VisualElement
		{
			internal TextElement textElement { get; private set; }

			public ITextSelection textSelection
			{
				get
				{
					return this.textElement.selection;
				}
			}

			public ITextEdition textEdition
			{
				get
				{
					return this.textElement.edition;
				}
			}

			public void SelectAll()
			{
				this.textSelection.SelectAll();
			}

			internal void SelectNone()
			{
				this.textSelection.SelectNone();
			}

			internal string originalText
			{
				get
				{
					return this.textElement.originalText;
				}
			}

			protected virtual TValueType StringToValue(string str)
			{
				throw new NotSupportedException();
			}

			internal void UpdateValueFromText()
			{
				TextInputBaseField<TValueType> textInputBaseField = (TextInputBaseField<TValueType>)base.parent;
				textInputBaseField.UpdateValueFromText();
			}

			internal void UpdateTextFromValue()
			{
				TextInputBaseField<TValueType> textInputBaseField = (TextInputBaseField<TValueType>)base.parent;
				textInputBaseField.UpdateTextFromValue();
			}

			internal void MoveFocusToCompositeRoot()
			{
				TextInputBaseField<TValueType> textInputBaseField = (TextInputBaseField<TValueType>)base.parent;
				textInputBaseField.Focus();
			}

			public void ResetValueAndText()
			{
				this.textEdition.ResetValueAndText();
			}

			public bool isReadOnly
			{
				get
				{
					return this.textEdition.isReadOnly;
				}
				set
				{
					this.textEdition.isReadOnly = value;
				}
			}

			public int maxLength
			{
				get
				{
					return this.textEdition.maxLength;
				}
				set
				{
					this.textEdition.maxLength = value;
				}
			}

			public char maskChar
			{
				get
				{
					return this.textEdition.maskChar;
				}
				set
				{
					this.textEdition.maskChar = value;
				}
			}

			public virtual bool isPasswordField
			{
				get
				{
					return this.textEdition.isPassword;
				}
				set
				{
					this.textEdition.isPassword = value;
				}
			}

			internal bool isDelayed
			{
				get
				{
					return this.textEdition.isDelayed;
				}
				set
				{
					this.textEdition.isDelayed = value;
				}
			}

			internal bool isDragging { get; set; }

			public Color selectionColor
			{
				get
				{
					return this.textSelection.selectionColor;
				}
				set
				{
					this.textSelection.selectionColor = value;
				}
			}

			public Color cursorColor
			{
				get
				{
					return this.textSelection.cursorColor;
				}
				set
				{
					this.textSelection.cursorColor = value;
				}
			}

			public int cursorIndex
			{
				get
				{
					return this.textSelection.cursorIndex;
				}
			}

			public int selectIndex
			{
				get
				{
					return this.textSelection.selectIndex;
				}
			}

			public bool doubleClickSelectsWord
			{
				get
				{
					return this.textSelection.doubleClickSelectsWord;
				}
				set
				{
					this.textSelection.doubleClickSelectsWord = value;
				}
			}

			public bool tripleClickSelectsLine
			{
				get
				{
					return this.textSelection.tripleClickSelectsLine;
				}
				set
				{
					this.textSelection.tripleClickSelectsLine = value;
				}
			}

			public string text
			{
				get
				{
					return this.textElement.text;
				}
				set
				{
					bool flag = this.textElement.text == value;
					if (!flag)
					{
						this.textElement.text = value;
					}
				}
			}

			internal TextInputBase()
			{
				base.delegatesFocus = true;
				this.textElement = new TextElement();
				this.textElement.parseEscapeSequences = false;
				this.textElement.selection.isSelectable = true;
				this.textEdition.isReadOnly = false;
				this.textEdition.keyboardType = TouchScreenKeyboardType.Default;
				this.textEdition.autoCorrection = false;
				this.textSelection.isSelectable = true;
				this.textElement.enableRichText = false;
				this.textSelection.selectAllOnFocus = true;
				this.textSelection.selectAllOnMouseUp = true;
				this.textElement.tabIndex = 0;
				ITextEdition textEdition = this.textEdition;
				textEdition.AcceptCharacter = (Func<char, bool>)Delegate.Combine(textEdition.AcceptCharacter, new Func<char, bool>(this.AcceptCharacter));
				ITextEdition textEdition2 = this.textEdition;
				textEdition2.UpdateScrollOffset = (Action<bool>)Delegate.Combine(textEdition2.UpdateScrollOffset, new Action<bool>(this.UpdateScrollOffset));
				ITextEdition textEdition3 = this.textEdition;
				textEdition3.UpdateValueFromText = (Action)Delegate.Combine(textEdition3.UpdateValueFromText, new Action(this.UpdateValueFromText));
				ITextEdition textEdition4 = this.textEdition;
				textEdition4.UpdateTextFromValue = (Action)Delegate.Combine(textEdition4.UpdateTextFromValue, new Action(this.UpdateTextFromValue));
				ITextEdition textEdition5 = this.textEdition;
				textEdition5.MoveFocusToCompositeRoot = (Action)Delegate.Combine(textEdition5.MoveFocusToCompositeRoot, new Action(this.MoveFocusToCompositeRoot));
				base.AddToClassList(TextInputBaseField<TValueType>.inputUssClassName);
				base.name = TextInputBaseField<string>.textInputUssName;
				this.SetSingleLine();
				base.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnInputCustomStyleResolved), TrickleDown.NoTrickleDown);
				base.tabIndex = -1;
			}

			private void MakeSureScrollViewDoesNotLeakEvents(ChangeEvent<float> evt)
			{
				evt.StopPropagation();
			}

			internal void SetSingleLine()
			{
				base.hierarchy.Clear();
				this.RemoveMultilineComponents();
				base.Add(this.textElement);
				base.AddToClassList(TextInputBaseField<TValueType>.singleLineInputUssClassName);
				this.textElement.AddToClassList(TextInputBaseField<TValueType>.TextInputBase.innerTextElementUssClassName);
				this.textElement.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.TextElementOnGeometryChangedEvent), TrickleDown.NoTrickleDown);
				bool flag = this.scrollOffset != Vector2.zero;
				if (flag)
				{
					this.scrollOffset.y = 0f;
					this.UpdateScrollOffset(false);
				}
			}

			internal void SetMultiline()
			{
				bool flag = !this.textEdition.multiline;
				if (!flag)
				{
					this.RemoveSingleLineComponents();
					this.RemoveMultilineComponents();
					bool flag2 = this.m_VerticalScrollerVisibility != ScrollerVisibility.Hidden && this.scrollView == null;
					if (flag2)
					{
						this.scrollView = new ScrollView();
						this.scrollView.Add(this.textElement);
						base.Add(this.scrollView);
						this.SetScrollViewMode();
						this.scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
						this.scrollView.verticalScrollerVisibility = this.m_VerticalScrollerVisibility;
						this.scrollView.AddToClassList(TextInputBaseField<TValueType>.TextInputBase.innerScrollviewUssClassName);
						this.scrollView.contentViewport.AddToClassList(TextInputBaseField<TValueType>.TextInputBase.innerViewportUssClassName);
						this.scrollView.contentContainer.AddToClassList(TextInputBaseField<TValueType>.TextInputBase.innerContentContainerUssClassName);
						this.scrollView.contentContainer.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.ScrollViewOnGeometryChangedEvent), TrickleDown.NoTrickleDown);
						this.scrollView.verticalScroller.slider.RegisterValueChangedCallback<float>(new EventCallback<ChangeEvent<float>>(this.MakeSureScrollViewDoesNotLeakEvents));
						this.scrollView.verticalScroller.slider.focusable = false;
						this.scrollView.horizontalScroller.slider.RegisterValueChangedCallback<float>(new EventCallback<ChangeEvent<float>>(this.MakeSureScrollViewDoesNotLeakEvents));
						this.scrollView.horizontalScroller.slider.focusable = false;
						base.AddToClassList(TextInputBaseField<TValueType>.multilineInputWithScrollViewUssClassName);
						this.textElement.AddToClassList(TextInputBaseField<TValueType>.TextInputBase.innerTextElementWithScrollViewUssClassName);
					}
					else
					{
						bool flag3 = this.multilineContainer == null;
						if (flag3)
						{
							this.textElement.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.TextElementOnGeometryChangedEvent), TrickleDown.NoTrickleDown);
							this.multilineContainer = new VisualElement
							{
								classList = { TextInputBaseField<TValueType>.multilineContainerClassName }
							};
							this.multilineContainer.Add(this.textElement);
							base.Add(this.multilineContainer);
							this.SetMultilineContainerStyle();
							base.AddToClassList(TextInputBaseField<TValueType>.multilineInputUssClassName);
							this.textElement.AddToClassList(TextInputBaseField<TValueType>.TextInputBase.innerTextElementUssClassName);
						}
					}
				}
			}

			private void ScrollViewOnGeometryChangedEvent(GeometryChangedEvent e)
			{
				bool flag = e.oldRect.size == e.newRect.size;
				if (!flag)
				{
					this.UpdateScrollOffset(false);
				}
			}

			private void TextElementOnGeometryChangedEvent(GeometryChangedEvent e)
			{
				bool flag = e.oldRect.size == e.newRect.size;
				if (!flag)
				{
					bool flag2 = Math.Abs(e.oldRect.size.x - e.newRect.size.x) > 1E-30f;
					this.UpdateScrollOffset(false, flag2);
				}
			}

			internal void OnInputCustomStyleResolved(CustomStyleResolvedEvent e)
			{
				ICustomStyle customStyle = e.customStyle;
				Color color;
				bool flag = customStyle.TryGetValue(TextInputBaseField<TValueType>.s_SelectionColorProperty, out color);
				if (flag)
				{
					this.textSelection.selectionColor = color;
				}
				Color color2;
				bool flag2 = customStyle.TryGetValue(TextInputBaseField<TValueType>.s_CursorColorProperty, out color2);
				if (flag2)
				{
					this.textSelection.cursorColor = color2;
				}
				this.SetScrollViewMode();
				this.SetMultilineContainerStyle();
			}

			internal virtual bool AcceptCharacter(char c)
			{
				return !this.isReadOnly && base.enabledInHierarchy;
			}

			internal void UpdateScrollOffset(bool isBackspace = false)
			{
				this.UpdateScrollOffset(isBackspace, false);
			}

			internal void UpdateScrollOffset(bool isBackspace, bool widthChanged)
			{
				ITextSelection textSelection = this.textSelection;
				bool flag = textSelection.cursorIndex < 0;
				if (!flag)
				{
					bool flag2 = this.scrollView != null;
					if (flag2)
					{
						this.scrollOffset = this.GetScrollOffset(this.scrollView.scrollOffset.x, this.scrollView.scrollOffset.y, this.scrollView.contentViewport.layout.width, isBackspace, widthChanged);
						this.scrollView.scrollOffset = this.scrollOffset;
						this.m_ScrollViewWasClamped = this.scrollOffset.x > this.scrollView.scrollOffset.x || this.scrollOffset.y > this.scrollView.scrollOffset.y;
					}
					else
					{
						Vector3 position = this.textElement.transform.position;
						this.scrollOffset = this.GetScrollOffset(this.scrollOffset.x, this.scrollOffset.y, base.contentRect.width, isBackspace, widthChanged);
						position.y = -Mathf.Min(this.scrollOffset.y, Math.Abs(this.textElement.contentRect.height - base.contentRect.height));
						position.x = -this.scrollOffset.x;
						bool flag3 = !position.Equals(this.textElement.transform.position);
						if (flag3)
						{
							this.textElement.transform.position = position;
						}
					}
				}
			}

			private Vector2 GetScrollOffset(float xOffset, float yOffset, float contentViewportWidth, bool isBackspace, bool widthChanged)
			{
				Vector2 cursorPosition = this.textSelection.cursorPosition;
				float cursorWidth = this.textSelection.cursorWidth;
				float num = xOffset;
				float num2 = yOffset;
				bool flag = Math.Abs(this.lastCursorPos.x - cursorPosition.x) > 0.05f || this.m_ScrollViewWasClamped || widthChanged;
				if (flag)
				{
					bool flag2 = cursorPosition.x > xOffset + contentViewportWidth - cursorWidth || (xOffset > 0f && widthChanged);
					if (flag2)
					{
						float num3 = Mathf.Ceil(cursorPosition.x + cursorWidth - contentViewportWidth);
						num = Mathf.Max(num3, 0f);
					}
					else
					{
						bool flag3 = cursorPosition.x < xOffset + 5f;
						if (flag3)
						{
							num = Mathf.Max(cursorPosition.x - 5f, 0f);
						}
					}
				}
				bool flag4 = this.textEdition.multiline && (Math.Abs(this.lastCursorPos.y - cursorPosition.y) > 0.05f || this.m_ScrollViewWasClamped);
				if (flag4)
				{
					bool flag5 = cursorPosition.y > base.contentRect.height + yOffset;
					if (flag5)
					{
						num2 = cursorPosition.y - base.contentRect.height;
					}
					else
					{
						bool flag6 = cursorPosition.y < this.textSelection.lineHeightAtCursorPosition + yOffset + 0.05f;
						if (flag6)
						{
							num2 = cursorPosition.y - this.textSelection.lineHeightAtCursorPosition;
						}
					}
				}
				this.lastCursorPos = cursorPosition;
				bool flag7 = Math.Abs(xOffset - num) > 0.05f || Math.Abs(yOffset - num2) > 0.05f;
				Vector2 vector;
				if (flag7)
				{
					vector = new Vector2(num, num2);
				}
				else
				{
					vector = ((this.scrollView != null) ? this.scrollView.scrollOffset : this.scrollOffset);
				}
				return vector;
			}

			internal void SetScrollViewMode()
			{
				bool flag = this.scrollView == null;
				if (!flag)
				{
					this.textElement.RemoveFromClassList(TextInputBaseField<TValueType>.TextInputBase.verticalVariantInnerTextElementUssClassName);
					this.textElement.RemoveFromClassList(TextInputBaseField<TValueType>.TextInputBase.verticalHorizontalVariantInnerTextElementUssClassName);
					this.textElement.RemoveFromClassList(TextInputBaseField<TValueType>.TextInputBase.horizontalVariantInnerTextElementUssClassName);
					bool flag2 = this.textEdition.multiline && base.computedStyle.whiteSpace == WhiteSpace.Normal;
					if (flag2)
					{
						this.textElement.AddToClassList(TextInputBaseField<TValueType>.TextInputBase.verticalVariantInnerTextElementUssClassName);
						this.scrollView.mode = ScrollViewMode.Vertical;
					}
					else
					{
						bool multiline = this.textEdition.multiline;
						if (multiline)
						{
							this.textElement.AddToClassList(TextInputBaseField<TValueType>.TextInputBase.verticalHorizontalVariantInnerTextElementUssClassName);
							this.scrollView.mode = ScrollViewMode.VerticalAndHorizontal;
						}
						else
						{
							this.textElement.AddToClassList(TextInputBaseField<TValueType>.TextInputBase.horizontalVariantInnerTextElementUssClassName);
							this.scrollView.mode = ScrollViewMode.Horizontal;
						}
					}
				}
			}

			private void SetMultilineContainerStyle()
			{
				bool flag = this.multilineContainer != null;
				if (flag)
				{
					bool flag2 = base.computedStyle.whiteSpace == WhiteSpace.Normal;
					if (flag2)
					{
						base.style.overflow = Overflow.Hidden;
					}
					else
					{
						base.style.overflow = (Overflow)2;
					}
				}
			}

			private void RemoveSingleLineComponents()
			{
				base.RemoveFromClassList(TextInputBaseField<TValueType>.singleLineInputUssClassName);
				this.textElement.RemoveFromClassList(TextInputBaseField<TValueType>.TextInputBase.innerTextElementUssClassName);
				this.textElement.RemoveFromHierarchy();
				this.textElement.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.TextElementOnGeometryChangedEvent), TrickleDown.NoTrickleDown);
			}

			private void RemoveMultilineComponents()
			{
				bool flag = this.scrollView != null;
				if (flag)
				{
					this.scrollView.RemoveFromHierarchy();
					this.scrollView.contentContainer.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.ScrollViewOnGeometryChangedEvent), TrickleDown.NoTrickleDown);
					this.scrollView.verticalScroller.slider.UnregisterValueChangedCallback<float>(new EventCallback<ChangeEvent<float>>(this.MakeSureScrollViewDoesNotLeakEvents));
					this.scrollView.horizontalScroller.slider.UnregisterValueChangedCallback<float>(new EventCallback<ChangeEvent<float>>(this.MakeSureScrollViewDoesNotLeakEvents));
					this.scrollView = null;
					this.textElement.RemoveFromClassList(TextInputBaseField<TValueType>.TextInputBase.verticalVariantInnerTextElementUssClassName);
					this.textElement.RemoveFromClassList(TextInputBaseField<TValueType>.TextInputBase.verticalHorizontalVariantInnerTextElementUssClassName);
					this.textElement.RemoveFromClassList(TextInputBaseField<TValueType>.TextInputBase.horizontalVariantInnerTextElementUssClassName);
					base.RemoveFromClassList(TextInputBaseField<TValueType>.multilineInputWithScrollViewUssClassName);
					this.textElement.RemoveFromClassList(TextInputBaseField<TValueType>.TextInputBase.innerTextElementWithScrollViewUssClassName);
				}
				bool flag2 = this.multilineContainer != null;
				if (flag2)
				{
					this.textElement.transform.position = Vector3.zero;
					this.multilineContainer.RemoveFromHierarchy();
					this.textElement.UnregisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.TextElementOnGeometryChangedEvent), TrickleDown.NoTrickleDown);
					this.multilineContainer = null;
					base.RemoveFromClassList(TextInputBaseField<TValueType>.multilineInputUssClassName);
				}
			}

			internal bool SetVerticalScrollerVisibility(ScrollerVisibility sv)
			{
				bool multiline = this.textEdition.multiline;
				bool flag2;
				if (multiline)
				{
					this.m_VerticalScrollerVisibility = sv;
					bool flag = this.scrollView == null;
					if (flag)
					{
						this.SetMultiline();
					}
					else
					{
						this.scrollView.verticalScrollerVisibility = this.m_VerticalScrollerVisibility;
					}
					flag2 = true;
				}
				else
				{
					Debug.LogWarning("Can't SetVerticalScrollerVisibility as the field isn't multiline.");
					flag2 = false;
				}
				return flag2;
			}

			internal ScrollView scrollView;

			internal VisualElement multilineContainer;

			public static readonly string innerComponentsModifierName = "--inner-input-field-component";

			public static readonly string innerTextElementUssClassName = TextElement.ussClassName + TextInputBaseField<TValueType>.TextInputBase.innerComponentsModifierName;

			internal static readonly string innerTextElementWithScrollViewUssClassName = TextElement.ussClassName + TextInputBaseField<TValueType>.TextInputBase.innerComponentsModifierName + "--scroll-view";

			public static readonly string horizontalVariantInnerTextElementUssClassName = TextElement.ussClassName + TextInputBaseField<TValueType>.TextInputBase.innerComponentsModifierName + "--horizontal";

			public static readonly string verticalVariantInnerTextElementUssClassName = TextElement.ussClassName + TextInputBaseField<TValueType>.TextInputBase.innerComponentsModifierName + "--vertical";

			public static readonly string verticalHorizontalVariantInnerTextElementUssClassName = TextElement.ussClassName + TextInputBaseField<TValueType>.TextInputBase.innerComponentsModifierName + "--vertical-horizontal";

			public static readonly string innerScrollviewUssClassName = ScrollView.ussClassName + TextInputBaseField<TValueType>.TextInputBase.innerComponentsModifierName;

			public static readonly string innerViewportUssClassName = ScrollView.viewportUssClassName + TextInputBaseField<TValueType>.TextInputBase.innerComponentsModifierName;

			public static readonly string innerContentContainerUssClassName = ScrollView.contentUssClassName + TextInputBaseField<TValueType>.TextInputBase.innerComponentsModifierName;

			internal Vector2 scrollOffset = Vector2.zero;

			private bool m_ScrollViewWasClamped;

			private Vector2 lastCursorPos = Vector2.zero;

			private ScrollerVisibility m_VerticalScrollerVisibility = ScrollerVisibility.Hidden;
		}
	}
}
