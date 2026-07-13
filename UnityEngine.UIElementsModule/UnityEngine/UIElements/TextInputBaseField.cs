using System;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public abstract class TextInputBaseField<TValueType> : BaseField<TValueType>, IDelayedField
	{
		internal bool password
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

		internal bool selectWordByDoubleClick
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

		internal bool selectLineByTripleClick
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

		internal bool readOnly
		{
			get
			{
				return this.isReadOnly;
			}
			set
			{
				this.isReadOnly = value;
			}
		}

		[CreateProperty]
		internal string placeholderText
		{
			get
			{
				return this.textEdition.placeholder;
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			set
			{
				bool flag = this.textEdition.placeholder == value;
				if (!flag)
				{
					this.textEdition.placeholder = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.placeholderTextProperty);
				}
			}
		}

		[CreateProperty]
		internal bool hidePlaceholderOnFocus
		{
			get
			{
				return this.textEdition.hidePlaceholderOnFocus;
			}
			set
			{
				bool flag = this.textEdition.hidePlaceholderOnFocus == value;
				if (!flag)
				{
					this.textEdition.hidePlaceholderOnFocus = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.hidePlaceholderOnFocusProperty);
				}
			}
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
			base.visualInput.AddToClassList(TextInputBaseField<TValueType>.singleLineInputUssClassName);
			this.m_TextInputBase = textInputBase;
			this.m_TextInputBase.textEdition.maxLength = maxLength;
			this.m_TextInputBase.textEdition.maskChar = maskChar;
			base.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnFieldCustomStyleResolved), TrickleDown.NoTrickleDown);
			TextElement textElement = textInputBase.textElement;
			textElement.OnPlaceholderChanged = (Action)Delegate.Combine(textElement.OnPlaceholderChanged, new Action(this.OnPlaceholderChanged));
			this.m_UpdateTextFromValue = true;
		}

		protected internal TextInputBaseField<TValueType>.TextInputBase textInputBase
		{
			get
			{
				return this.m_TextInputBase;
			}
		}

		[CreateProperty(ReadOnly = true)]
		public ITextSelection textSelection
		{
			get
			{
				return this.m_TextInputBase.textElement.selection;
			}
		}

		[CreateProperty(ReadOnly = true)]
		public ITextEdition textEdition
		{
			get
			{
				return this.m_TextInputBase.textElement.edition;
			}
		}

		protected Action<bool> onIsReadOnlyChanged
		{
			get
			{
				return this.m_TextInputBase.textElement.onIsReadOnlyChanged;
			}
			set
			{
				this.m_TextInputBase.textElement.onIsReadOnlyChanged = value;
			}
		}

		[CreateProperty]
		public bool isReadOnly
		{
			get
			{
				return this.textEdition.isReadOnly;
			}
			set
			{
				bool flag = this.textEdition.isReadOnly == value;
				if (!flag)
				{
					this.textEdition.isReadOnly = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.isReadOnlyProperty);
				}
			}
		}

		[CreateProperty]
		public bool isPasswordField
		{
			get
			{
				return this.textEdition.isPassword;
			}
			set
			{
				bool flag = this.textEdition.isPassword == value;
				if (!flag)
				{
					this.textEdition.isPassword = value;
					this.m_TextInputBase.IncrementVersion(VersionChangeType.Repaint);
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.isPasswordFieldProperty);
				}
			}
		}

		[CreateProperty]
		public bool autoCorrection
		{
			get
			{
				return this.textEdition.autoCorrection;
			}
			set
			{
				bool flag = this.textEdition.autoCorrection == value;
				if (!flag)
				{
					this.textEdition.autoCorrection = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.autoCorrectionProperty);
				}
			}
		}

		[CreateProperty]
		public bool hideMobileInput
		{
			get
			{
				return this.textEdition.hideMobileInput;
			}
			set
			{
				bool flag = this.textEdition.hideMobileInput == value;
				if (!flag)
				{
					this.textEdition.hideMobileInput = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.hideMobileInputProperty);
				}
			}
		}

		[CreateProperty]
		public TouchScreenKeyboardType keyboardType
		{
			get
			{
				return this.textEdition.keyboardType;
			}
			set
			{
				bool flag = this.textEdition.keyboardType == value;
				if (!flag)
				{
					this.textEdition.keyboardType = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.keyboardTypeProperty);
				}
			}
		}

		public TouchScreenKeyboard touchScreenKeyboard
		{
			get
			{
				return this.textEdition.touchScreenKeyboard;
			}
		}

		[CreateProperty]
		public int maxLength
		{
			get
			{
				return this.textEdition.maxLength;
			}
			set
			{
				bool flag = this.textEdition.maxLength == value;
				if (!flag)
				{
					this.textEdition.maxLength = value;
					this.textEdition.UpdateText(this.ValueToString(this.value));
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.maxLengthProperty);
				}
			}
		}

		[CreateProperty]
		public bool isDelayed
		{
			get
			{
				return this.textEdition.isDelayed;
			}
			set
			{
				bool flag = this.textEdition.isDelayed == value;
				if (!flag)
				{
					this.textEdition.isDelayed = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.isDelayedProperty);
				}
			}
		}

		[CreateProperty]
		public char maskChar
		{
			get
			{
				return this.textEdition.maskChar;
			}
			set
			{
				bool flag = this.textEdition.maskChar == value;
				if (!flag)
				{
					this.textEdition.maskChar = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.maskCharProperty);
				}
			}
		}

		[Obsolete("cursorColor is deprecated. Please use the corresponding USS property (--unity-cursor-color) instead.")]
		public Color selectionColor
		{
			get
			{
				return this.textSelection.selectionColor;
			}
		}

		[Obsolete("cursorColor is deprecated. Please use the corresponding USS property (--unity-cursor-color) instead.")]
		public Color cursorColor
		{
			get
			{
				return this.textSelection.cursorColor;
			}
		}

		[CreateProperty]
		public int cursorIndex
		{
			get
			{
				return this.textSelection.cursorIndex;
			}
			set
			{
				bool flag = this.textSelection.cursorIndex == value;
				if (!flag)
				{
					this.textSelection.cursorIndex = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.cursorIndexProperty);
				}
			}
		}

		[CreateProperty(ReadOnly = true)]
		public Vector2 cursorPosition
		{
			get
			{
				return this.textSelection.cursorPosition;
			}
		}

		[CreateProperty]
		public int selectIndex
		{
			get
			{
				return this.textSelection.selectIndex;
			}
			set
			{
				bool flag = this.textSelection.selectIndex == value;
				if (!flag)
				{
					this.textSelection.selectIndex = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.selectIndexProperty);
				}
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

		[CreateProperty]
		public bool selectAllOnFocus
		{
			get
			{
				return this.textSelection.selectAllOnFocus;
			}
			set
			{
				bool flag = this.textSelection.selectAllOnFocus == value;
				if (!flag)
				{
					this.textSelection.selectAllOnFocus = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.selectAllOnFocusProperty);
				}
			}
		}

		[CreateProperty]
		public bool selectAllOnMouseUp
		{
			get
			{
				return this.textSelection.selectAllOnMouseUp;
			}
			set
			{
				bool flag = this.textSelection.selectAllOnMouseUp == value;
				if (!flag)
				{
					this.textSelection.selectAllOnMouseUp = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.selectAllOnMouseUpProperty);
				}
			}
		}

		[CreateProperty]
		public bool doubleClickSelectsWord
		{
			get
			{
				return this.textSelection.doubleClickSelectsWord;
			}
			set
			{
				bool flag = this.textSelection.doubleClickSelectsWord == value;
				if (!flag)
				{
					this.textSelection.doubleClickSelectsWord = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.doubleClickSelectsWordProperty);
				}
			}
		}

		[CreateProperty]
		public bool tripleClickSelectsLine
		{
			get
			{
				return this.textSelection.tripleClickSelectsLine;
			}
			set
			{
				bool flag = this.textSelection.tripleClickSelectsLine == value;
				if (!flag)
				{
					this.textSelection.tripleClickSelectsLine = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.tripleClickSelectsLineProperty);
				}
			}
		}

		[Obsolete("SetVerticalScrollerVisibility is deprecated. Use TextField.verticalScrollerVisibility instead.")]
		public bool SetVerticalScrollerVisibility(ScrollerVisibility sv)
		{
			return this.textInputBase.SetVerticalScrollerVisibility(sv);
		}

		public string text
		{
			get
			{
				return this.m_TextInputBase.text;
			}
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			protected internal set
			{
				this.m_TextInputBase.text = value;
			}
		}

		[CreateProperty]
		public bool emojiFallbackSupport
		{
			get
			{
				return this.m_TextInputBase.textElement.emojiFallbackSupport;
			}
			set
			{
				bool flag = this.m_TextInputBase.textElement.emojiFallbackSupport == value;
				if (!flag)
				{
					base.labelElement.emojiFallbackSupport = value;
					this.m_TextInputBase.textElement.emojiFallbackSupport = value;
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.emojiFallbackSupportProperty);
				}
			}
		}

		[CreateProperty]
		public ScrollerVisibility verticalScrollerVisibility
		{
			get
			{
				return this.textInputBase.verticalScrollerVisibility;
			}
			set
			{
				bool flag = this.textInputBase.verticalScrollerVisibility == value;
				if (!flag)
				{
					this.textInputBase.SetVerticalScrollerVisibility(value);
					base.NotifyPropertyChanged(in TextInputBaseField<TValueType>.verticalScrollerVisibilityProperty);
				}
			}
		}

		public Vector2 MeasureTextSize(string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode)
		{
			return TextUtilities.MeasureVisualElementTextSize(this.m_TextInputBase.textElement, textToMeasure, width, widthMode, height, heightMode, null);
		}

		[EventInterest(new Type[]
		{
			typeof(NavigationSubmitEvent),
			typeof(FocusInEvent),
			typeof(FocusEvent),
			typeof(FocusOutEvent),
			typeof(BlurEvent)
		})]
		protected override void HandleEventBubbleUp(EventBase evt)
		{
			base.HandleEventBubbleUp(evt);
			bool isReadOnly = this.textEdition.isReadOnly;
			if (!isReadOnly)
			{
				bool flag = evt.eventTypeId == EventBase<NavigationSubmitEvent>.TypeId() && evt.target != this.textInputBase.textElement;
				if (flag)
				{
					this.textInputBase.textElement.Focus();
				}
				else
				{
					bool flag2 = evt.eventTypeId == EventBase<NavigationMoveEvent>.TypeId() && evt.target != this.textInputBase.textElement;
					if (flag2)
					{
						this.focusController.SwitchFocusOnEvent(this.textInputBase.textElement, evt);
					}
					else
					{
						bool flag3 = evt.eventTypeId == EventBase<FocusInEvent>.TypeId();
						if (flag3)
						{
							bool showMixedValue = base.showMixedValue;
							if (showMixedValue)
							{
								((INotifyValueChanged<string>)this.textInputBase.textElement).SetValueWithoutNotify(null);
							}
						}
						else
						{
							bool flag4 = evt.eventTypeId == EventBase<FocusEvent>.TypeId();
							if (flag4)
							{
								this.UpdatePlaceholderClassList(null);
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
									this.UpdatePlaceholderClassList(null);
									this.textInputBase.UpdateScrollOffset(false);
								}
							}
						}
					}
				}
			}
		}

		public override void SetValueWithoutNotify(TValueType newValue)
		{
			base.SetValueWithoutNotify(newValue);
			bool flag = this.textInputBase.textElement.needsPlaceholderIfTextIsEmpty && string.IsNullOrEmpty(this.ValueToString(newValue));
			if (flag)
			{
				base.visualInput.AddToClassList(TextInputBaseField<TValueType>.placeholderUssClassName);
			}
		}

		protected abstract string ValueToString(TValueType value);

		protected abstract TValueType StringToValue(string str);

		private protected override bool canSwitchToMixedValue
		{
			get
			{
				return !this.textInputBase.textElement.hasFocus;
			}
		}

		protected override void UpdateMixedValueContent()
		{
			bool showMixedValue = base.showMixedValue;
			if (showMixedValue)
			{
				bool updateTextFromValue = this.m_UpdateTextFromValue;
				if (updateTextFromValue)
				{
					((INotifyValueChanged<string>)this.textInputBase.textElement).SetValueWithoutNotify(BaseField<TValueType>.mixedValueString);
				}
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

		internal bool hasFocus
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return this.textInputBase.textElement.hasFocus;
			}
		}

		internal void OnPlaceholderChanged()
		{
			bool flag = !string.IsNullOrEmpty(this.textEdition.placeholder);
			if (flag)
			{
				base.RegisterCallback<ChangeEvent<TValueType>>(new EventCallback<ChangeEvent<TValueType>>(this.UpdatePlaceholderClassList), TrickleDown.NoTrickleDown);
			}
			else
			{
				base.UnregisterCallback<ChangeEvent<TValueType>>(new EventCallback<ChangeEvent<TValueType>>(this.UpdatePlaceholderClassList), TrickleDown.NoTrickleDown);
			}
			this.UpdatePlaceholderClassList(null);
		}

		internal void UpdatePlaceholderClassList(ChangeEvent<TValueType> evt = null)
		{
			bool showPlaceholderText = this.textInputBase.textElement.showPlaceholderText;
			if (showPlaceholderText)
			{
				base.visualInput.AddToClassList(TextInputBaseField<TValueType>.placeholderUssClassName);
			}
			else
			{
				base.visualInput.RemoveFromClassList(TextInputBaseField<TValueType>.placeholderUssClassName);
			}
		}

		internal virtual void UpdateValueFromText()
		{
			this.value = this.StringToValue(this.text);
		}

		internal virtual void UpdateTextFromValue()
		{
		}

		private void OnFieldCustomStyleResolved(CustomStyleResolvedEvent e)
		{
			this.m_TextInputBase.OnInputCustomStyleResolved(e);
		}

		internal static readonly BindingId autoCorrectionProperty = "autoCorrection";

		internal static readonly BindingId hideMobileInputProperty = "hideMobileInput";

		internal static readonly BindingId hidePlaceholderOnFocusProperty = "hidePlaceholderOnFocus";

		internal static readonly BindingId keyboardTypeProperty = "keyboardType";

		internal static readonly BindingId isReadOnlyProperty = "isReadOnly";

		internal static readonly BindingId isPasswordFieldProperty = "isPasswordField";

		internal static readonly BindingId textSelectionProperty = "textSelection";

		internal static readonly BindingId textEditionProperty = "textEdition";

		internal static readonly BindingId placeholderTextProperty = "placeholderText";

		internal static readonly BindingId cursorIndexProperty = "cursorIndex";

		internal static readonly BindingId cursorPositionProperty = "cursorPosition";

		internal static readonly BindingId selectIndexProperty = "selectIndex";

		internal static readonly BindingId selectAllOnFocusProperty = "selectAllOnFocus";

		internal static readonly BindingId selectAllOnMouseUpProperty = "selectAllOnMouseUp";

		internal static readonly BindingId maxLengthProperty = "maxLength";

		internal static readonly BindingId doubleClickSelectsWordProperty = "doubleClickSelectsWord";

		internal static readonly BindingId tripleClickSelectsLineProperty = "tripleClickSelectsLine";

		internal static readonly BindingId emojiFallbackSupportProperty = "emojiFallbackSupport";

		internal static readonly BindingId isDelayedProperty = "isDelayed";

		internal static readonly BindingId maskCharProperty = "maskChar";

		internal static readonly BindingId verticalScrollerVisibilityProperty = "verticalScrollerVisibility";

		private static CustomStyleProperty<Color> s_SelectionColorProperty = new CustomStyleProperty<Color>("--unity-selection-color");

		private static CustomStyleProperty<Color> s_CursorColorProperty = new CustomStyleProperty<Color>("--unity-cursor-color");

		internal const int kMaxLengthNone = -1;

		internal const char kMaskCharDefault = '*';

		public new static readonly string ussClassName = "unity-base-text-field";

		public new static readonly string labelUssClassName = TextInputBaseField<TValueType>.ussClassName + "__label";

		public new static readonly string inputUssClassName = TextInputBaseField<TValueType>.ussClassName + "__input";

		internal static readonly string multilineContainerClassName = TextInputBaseField<TValueType>.ussClassName + "__multiline-container";

		public static readonly string singleLineInputUssClassName = TextInputBaseField<TValueType>.inputUssClassName + "--single-line";

		public static readonly string multilineInputUssClassName = TextInputBaseField<TValueType>.inputUssClassName + "--multiline";

		public static readonly string placeholderUssClassName = TextInputBaseField<TValueType>.inputUssClassName + "--placeholder";

		internal static readonly string multilineInputWithScrollViewUssClassName = TextInputBaseField<TValueType>.multilineInputUssClassName + "--scroll-view";

		public static readonly string textInputUssName = "unity-text-input";

		private TextInputBaseField<TValueType>.TextInputBase m_TextInputBase;

		internal bool m_UpdateTextFromValue;

		[ExcludeFromDocs]
		[Serializable]
		public new abstract class UxmlSerializedData : BaseField<TValueType>.UxmlSerializedData
		{
			public new static void Register()
			{
				BaseField<TValueType>.UxmlSerializedData.Register();
				UxmlDescriptionCache.RegisterType(typeof(TextInputBaseField<TValueType>.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("maxLength", "max-length", null, new string[] { "maxLength" }),
					new UxmlAttributeNames("isPasswordField", "password", null, Array.Empty<string>()),
					new UxmlAttributeNames("maskChar", "mask-character", null, new string[] { "maskCharacter" }),
					new UxmlAttributeNames("placeholderText", "placeholder-text", null, Array.Empty<string>()),
					new UxmlAttributeNames("hidePlaceholderOnFocus", "hide-placeholder-on-focus", null, Array.Empty<string>()),
					new UxmlAttributeNames("isReadOnly", "readonly", null, Array.Empty<string>()),
					new UxmlAttributeNames("isDelayed", "is-delayed", null, Array.Empty<string>()),
					new UxmlAttributeNames("verticalScrollerVisibility", "vertical-scroller-visibility", null, Array.Empty<string>()),
					new UxmlAttributeNames("selectAllOnMouseUp", "select-all-on-mouse-up", null, Array.Empty<string>()),
					new UxmlAttributeNames("selectAllOnFocus", "select-all-on-focus", null, Array.Empty<string>()),
					new UxmlAttributeNames("doubleClickSelectsWord", "select-word-by-double-click", null, Array.Empty<string>()),
					new UxmlAttributeNames("tripleClickSelectsLine", "select-line-by-triple-click", null, Array.Empty<string>()),
					new UxmlAttributeNames("emojiFallbackSupport", "emoji-fallback-support", null, Array.Empty<string>()),
					new UxmlAttributeNames("hideMobileInput", "hide-mobile-input", null, Array.Empty<string>()),
					new UxmlAttributeNames("keyboardType", "keyboard-type", null, Array.Empty<string>()),
					new UxmlAttributeNames("autoCorrection", "auto-correction", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				throw new MissingMethodException();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				TextInputBaseField<TValueType> textInputBaseField = (TextInputBaseField<TValueType>)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.maxLength_UxmlAttributeFlags);
				if (flag)
				{
					textInputBaseField.maxLength = this.maxLength;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.isPasswordField_UxmlAttributeFlags);
				if (flag2)
				{
					textInputBaseField.isPasswordField = this.isPasswordField;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.maskChar_UxmlAttributeFlags);
				if (flag3)
				{
					textInputBaseField.maskChar = this.maskChar;
				}
				bool flag4 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.placeholderText_UxmlAttributeFlags);
				if (flag4)
				{
					textInputBaseField.placeholderText = this.placeholderText;
				}
				bool flag5 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.hidePlaceholderOnFocus_UxmlAttributeFlags);
				if (flag5)
				{
					textInputBaseField.hidePlaceholderOnFocus = this.hidePlaceholderOnFocus;
				}
				bool flag6 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.isReadOnly_UxmlAttributeFlags);
				if (flag6)
				{
					textInputBaseField.isReadOnly = this.isReadOnly;
				}
				bool flag7 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.isDelayed_UxmlAttributeFlags);
				if (flag7)
				{
					textInputBaseField.isDelayed = this.isDelayed;
				}
				bool flag8 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.verticalScrollerVisibility_UxmlAttributeFlags);
				if (flag8)
				{
					textInputBaseField.verticalScrollerVisibility = this.verticalScrollerVisibility;
				}
				bool flag9 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.selectAllOnMouseUp_UxmlAttributeFlags);
				if (flag9)
				{
					textInputBaseField.textSelection.selectAllOnMouseUp = this.selectAllOnMouseUp;
				}
				bool flag10 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.selectAllOnFocus_UxmlAttributeFlags);
				if (flag10)
				{
					textInputBaseField.textSelection.selectAllOnFocus = this.selectAllOnFocus;
				}
				bool flag11 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.doubleClickSelectsWord_UxmlAttributeFlags);
				if (flag11)
				{
					textInputBaseField.doubleClickSelectsWord = this.doubleClickSelectsWord;
				}
				bool flag12 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.tripleClickSelectsLine_UxmlAttributeFlags);
				if (flag12)
				{
					textInputBaseField.tripleClickSelectsLine = this.tripleClickSelectsLine;
				}
				bool flag13 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.emojiFallbackSupport_UxmlAttributeFlags);
				if (flag13)
				{
					textInputBaseField.emojiFallbackSupport = this.emojiFallbackSupport;
				}
				bool flag14 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.hideMobileInput_UxmlAttributeFlags);
				if (flag14)
				{
					textInputBaseField.hideMobileInput = this.hideMobileInput;
				}
				bool flag15 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.keyboardType_UxmlAttributeFlags);
				if (flag15)
				{
					textInputBaseField.keyboardType = this.keyboardType;
				}
				bool flag16 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.autoCorrection_UxmlAttributeFlags);
				if (flag16)
				{
					textInputBaseField.autoCorrection = this.autoCorrection;
				}
			}

			[SerializeField]
			private string placeholderText;

			[SerializeField]
			[Delayed]
			[UxmlAttribute(obsoleteNames = new string[] { "maxLength" })]
			private int maxLength;

			[SerializeField]
			private TouchScreenKeyboardType keyboardType;

			[SerializeField]
			private protected ScrollerVisibility verticalScrollerVisibility;

			[UxmlAttribute("password")]
			[SerializeField]
			private bool isPasswordField;

			[SerializeField]
			[UxmlAttribute("mask-character", obsoleteNames = new string[] { "maskCharacter" })]
			private char maskChar;

			[SerializeField]
			private bool hidePlaceholderOnFocus;

			[UxmlAttribute("readonly")]
			[SerializeField]
			private bool isReadOnly;

			[SerializeField]
			private bool isDelayed;

			[SerializeField]
			private bool selectAllOnMouseUp;

			[SerializeField]
			private bool selectAllOnFocus;

			[UxmlAttribute("select-word-by-double-click")]
			[SerializeField]
			private bool doubleClickSelectsWord;

			[UxmlAttribute("select-line-by-triple-click")]
			[SerializeField]
			private bool tripleClickSelectsLine;

			[SerializeField]
			private bool emojiFallbackSupport;

			[SerializeField]
			private bool hideMobileInput;

			[SerializeField]
			private bool autoCorrection;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags maxLength_UxmlAttributeFlags;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags isPasswordField_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags maskChar_UxmlAttributeFlags;

			[UxmlIgnore]
			[SerializeField]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags placeholderText_UxmlAttributeFlags;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags hidePlaceholderOnFocus_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags isReadOnly_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags isDelayed_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private protected UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags verticalScrollerVisibility_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags selectAllOnMouseUp_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags selectAllOnFocus_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags doubleClickSelectsWord_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags tripleClickSelectsLine_UxmlAttributeFlags;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags emojiFallbackSupport_UxmlAttributeFlags;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags hideMobileInput_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags keyboardType_UxmlAttributeFlags;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags autoCorrection_UxmlAttributeFlags;
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseFieldTraits<string, UxmlStringAttributeDescription>
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				TextInputBaseField<TValueType> textInputBaseField = (TextInputBaseField<TValueType>)ve;
				textInputBaseField.maxLength = this.m_MaxLength.GetValueFromBag(bag, cc);
				textInputBaseField.password = this.m_Password.GetValueFromBag(bag, cc);
				textInputBaseField.readOnly = this.m_IsReadOnly.GetValueFromBag(bag, cc);
				textInputBaseField.isDelayed = this.m_IsDelayed.GetValueFromBag(bag, cc);
				textInputBaseField.textSelection.selectAllOnFocus = this.m_SelectAllOnFocus.GetValueFromBag(bag, cc);
				textInputBaseField.textSelection.selectAllOnMouseUp = this.m_SelectAllOnMouseUp.GetValueFromBag(bag, cc);
				textInputBaseField.doubleClickSelectsWord = this.m_SelectWordByDoubleClick.GetValueFromBag(bag, cc);
				textInputBaseField.tripleClickSelectsLine = this.m_SelectLineByTripleClick.GetValueFromBag(bag, cc);
				textInputBaseField.emojiFallbackSupport = this.m_EmojiFallbackSupport.GetValueFromBag(bag, cc);
				ScrollerVisibility scrollerVisibility = ScrollerVisibility.Hidden;
				this.m_VerticalScrollerVisibility.TryGetValueFromBag(bag, cc, ref scrollerVisibility);
				textInputBaseField.verticalScrollerVisibility = scrollerVisibility;
				textInputBaseField.hideMobileInput = this.m_HideMobileInput.GetValueFromBag(bag, cc);
				textInputBaseField.keyboardType = this.m_KeyboardType.GetValueFromBag(bag, cc);
				textInputBaseField.autoCorrection = this.m_AutoCorrection.GetValueFromBag(bag, cc);
				string valueFromBag = this.m_MaskCharacter.GetValueFromBag(bag, cc);
				textInputBaseField.maskChar = (string.IsNullOrEmpty(valueFromBag) ? '*' : valueFromBag[0]);
				textInputBaseField.placeholderText = this.m_PlaceholderText.GetValueFromBag(bag, cc);
				textInputBaseField.hidePlaceholderOnFocus = this.m_HidePlaceholderOnFocus.GetValueFromBag(bag, cc);
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

			private UxmlStringAttributeDescription m_PlaceholderText = new UxmlStringAttributeDescription
			{
				name = "placeholder-text"
			};

			private UxmlBoolAttributeDescription m_HidePlaceholderOnFocus = new UxmlBoolAttributeDescription
			{
				name = "hide-placeholder-on-focus"
			};

			private UxmlBoolAttributeDescription m_IsReadOnly = new UxmlBoolAttributeDescription
			{
				name = "readonly"
			};

			private UxmlBoolAttributeDescription m_IsDelayed = new UxmlBoolAttributeDescription
			{
				name = "is-delayed"
			};

			private UxmlEnumAttributeDescription<ScrollerVisibility> m_VerticalScrollerVisibility = new UxmlEnumAttributeDescription<ScrollerVisibility>
			{
				name = "vertical-scroller-visibility",
				defaultValue = ScrollerVisibility.Hidden
			};

			private UxmlBoolAttributeDescription m_SelectAllOnMouseUp = new UxmlBoolAttributeDescription
			{
				name = "select-all-on-mouse-up",
				defaultValue = true
			};

			private UxmlBoolAttributeDescription m_SelectAllOnFocus = new UxmlBoolAttributeDescription
			{
				name = "select-all-on-focus",
				defaultValue = true
			};

			private UxmlBoolAttributeDescription m_SelectWordByDoubleClick = new UxmlBoolAttributeDescription
			{
				name = "select-word-by-double-click",
				defaultValue = true
			};

			private UxmlBoolAttributeDescription m_SelectLineByTripleClick = new UxmlBoolAttributeDescription
			{
				name = "select-line-by-triple-click",
				defaultValue = true
			};

			private UxmlBoolAttributeDescription m_EmojiFallbackSupport = new UxmlBoolAttributeDescription
			{
				name = "emoji-fallback-support",
				defaultValue = true
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

			internal TextInputBase()
			{
				base.delegatesFocus = true;
				this.textElement = new TextElement();
				this.textElement.isInputField = true;
				this.textElement.selection.isSelectable = true;
				this.textEdition.isReadOnly = false;
				this.textSelection.isSelectable = true;
				this.textSelection.selectAllOnFocus = true;
				this.textSelection.selectAllOnMouseUp = true;
				this.textElement.enableRichText = false;
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
				this.textEdition.GetDefaultValueType = new Func<string>(this.GetDefaultValueType);
				base.AddToClassList(TextInputBaseField<TValueType>.inputUssClassName);
				base.name = TextInputBaseField<string>.textInputUssName;
				this.SetSingleLine();
				base.RegisterCallback<CustomStyleResolvedEvent>(new EventCallback<CustomStyleResolvedEvent>(this.OnInputCustomStyleResolved), TrickleDown.NoTrickleDown);
				base.tabIndex = -1;
			}

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

			internal bool isDragging { get; set; }

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
				this.focusController.SwitchFocus(textInputBaseField, false, DispatchMode.Default);
				this.textEdition.keyboardType = TouchScreenKeyboardType.Default;
				this.textEdition.autoCorrection = false;
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
				bool hasFocus = this.textElement.hasFocus;
				if (hasFocus)
				{
					this.textElement.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
				}
			}

			internal void SetMultiline()
			{
				bool flag = !this.textEdition.multiline;
				if (!flag)
				{
					this.RemoveSingleLineComponents();
					this.RemoveMultilineComponents();
					bool flag2 = this.verticalScrollerVisibility != ScrollerVisibility.Hidden && this.scrollView == null;
					if (flag2)
					{
						this.scrollView = new ScrollView();
						this.scrollView.Add(this.textElement);
						base.Add(this.scrollView);
						this.SetScrollViewMode();
						this.scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
						this.scrollView.verticalScrollerVisibility = this.verticalScrollerVisibility;
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
					bool hasFocus = this.textElement.hasFocus;
					if (hasFocus)
					{
						this.textElement.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
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
					this.textElement.selectionColor = color;
				}
				Color color2;
				bool flag2 = customStyle.TryGetValue(TextInputBaseField<TValueType>.s_CursorColorProperty, out color2);
				if (flag2)
				{
					this.textElement.cursorColor = color2;
				}
				this.SetScrollViewMode();
				this.SetMultilineContainerStyle();
			}

			private string GetDefaultValueType()
			{
				TValueType tvalueType = default(TValueType);
				string text;
				if (tvalueType != null)
				{
					tvalueType = default(TValueType);
					text = tvalueType.ToString();
				}
				else
				{
					text = "";
				}
				return text;
			}

			internal virtual bool AcceptCharacter(char c)
			{
				return !this.textEdition.isReadOnly && base.enabledInHierarchy;
			}

			internal void UpdateScrollOffset(bool isBackspace = false)
			{
				this.UpdateScrollOffset(isBackspace, false);
			}

			internal void UpdateScrollOffset(bool isBackspace, bool widthChanged)
			{
				ITextSelection textSelection = this.textSelection;
				bool flag = textSelection.cursorIndex < 0 || (textSelection.cursorIndex <= 0 && textSelection.selectIndex <= 0 && this.scrollOffset == Vector2.zero);
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
						Vector3 translate = this.textElement.resolvedStyle.translate;
						this.scrollOffset = this.GetScrollOffset(this.scrollOffset.x, this.scrollOffset.y, base.contentRect.width, isBackspace, widthChanged);
						translate.y = -Mathf.Min(this.scrollOffset.y, Math.Abs(this.textElement.contentRect.height - base.contentRect.height));
						translate.x = -this.scrollOffset.x;
						bool flag3 = !translate.Equals(this.textElement.resolvedStyle.translate);
						if (flag3)
						{
							this.textElement.style.translate = translate;
						}
					}
				}
			}

			private Vector2 GetScrollOffset(float xOffset, float yOffset, float contentViewportWidth, bool isBackspace, bool widthChanged)
			{
				bool flag = !this.textElement.hasFocus;
				Vector2 vector;
				if (flag)
				{
					vector = Vector2.zero;
				}
				else
				{
					Vector2 cursorPosition = this.textSelection.cursorPosition;
					float cursorWidth = this.textSelection.cursorWidth;
					float num = xOffset;
					float num2 = yOffset;
					bool flag2 = Math.Abs(this.lastCursorPos.x - cursorPosition.x) > 0.05f || this.m_ScrollViewWasClamped || widthChanged;
					if (flag2)
					{
						bool flag3 = cursorPosition.x > xOffset + contentViewportWidth - cursorWidth || (xOffset > 0f && widthChanged);
						if (flag3)
						{
							float num3 = Mathf.Ceil(cursorPosition.x + cursorWidth - contentViewportWidth);
							num = Mathf.Max(num3, 0f);
						}
						else
						{
							bool flag4 = cursorPosition.x < xOffset + 5f;
							if (flag4)
							{
								num = Mathf.Max(cursorPosition.x - 5f, 0f);
							}
						}
					}
					bool flag5 = this.textEdition.multiline && (Math.Abs(this.lastCursorPos.y - cursorPosition.y) > 0.05f || this.m_ScrollViewWasClamped);
					if (flag5)
					{
						bool flag6 = cursorPosition.y > base.contentRect.height + yOffset;
						if (flag6)
						{
							num2 = cursorPosition.y - base.contentRect.height;
						}
						else
						{
							bool flag7 = cursorPosition.y < this.textSelection.lineHeightAtCursorPosition + yOffset + 0.05f;
							if (flag7)
							{
								num2 = cursorPosition.y - this.textSelection.lineHeightAtCursorPosition;
							}
						}
					}
					this.lastCursorPos = cursorPosition;
					bool flag8 = Math.Abs(xOffset - num) > 0.05f || Math.Abs(yOffset - num2) > 0.05f;
					if (flag8)
					{
						vector = new Vector2(num, num2);
					}
					else
					{
						vector = ((this.scrollView != null) ? this.scrollView.scrollOffset : this.scrollOffset);
					}
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
					bool flag2 = this.textEdition.multiline && (base.computedStyle.whiteSpace == WhiteSpace.Normal || base.computedStyle.whiteSpace == WhiteSpace.PreWrap);
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
					bool flag2 = base.computedStyle.whiteSpace == WhiteSpace.Normal || base.computedStyle.whiteSpace == WhiteSpace.PreWrap;
					if (flag2)
					{
						base.style.overflow = Overflow.Hidden;
						this.multilineContainer.style.alignSelf = Align.Auto;
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
					this.textElement.style.translate = Vector3.zero;
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
					this.verticalScrollerVisibility = sv;
					bool flag = this.scrollView == null;
					if (flag)
					{
						this.SetMultiline();
					}
					else
					{
						this.scrollView.verticalScrollerVisibility = this.verticalScrollerVisibility;
					}
					flag2 = true;
				}
				else
				{
					flag2 = false;
				}
				return flag2;
			}

			[Obsolete("SelectAll() is deprecated. Use textSelection.SelectAll() instead.")]
			public void SelectAll()
			{
				this.textSelection.SelectAll();
			}

			[Obsolete("isReadOnly is deprecated. Use textEdition.isReadOnly instead.")]
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

			[Obsolete("maxLength is deprecated. Use textEdition.maxLength instead.")]
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

			[Obsolete("maskChar is deprecated. Use textEdition.maskChar instead.")]
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

			[Obsolete("isPasswordField is deprecated. Use textEdition.isPassword instead.")]
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

			[Obsolete("selectionColor is deprecated. Use textSelection.selectionColor instead.")]
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

			[Obsolete("cursorColor is deprecated. Use textSelection.cursorColor instead.")]
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

			[Obsolete("cursorIndex is deprecated. Use textSelection.cursorIndex instead.")]
			public int cursorIndex
			{
				get
				{
					return this.textSelection.cursorIndex;
				}
			}

			[Obsolete("selectIndex is deprecated. Use textSelection.selectIndex instead.")]
			public int selectIndex
			{
				get
				{
					return this.textSelection.selectIndex;
				}
			}

			[Obsolete("doubleClickSelectsWord is deprecated. Use textSelection.doubleClickSelectsWord instead.")]
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

			[Obsolete("tripleClickSelectsLine is deprecated. Use textSelection.tripleClickSelectsLine instead.")]
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

			internal ScrollerVisibility verticalScrollerVisibility = ScrollerVisibility.Hidden;
		}
	}
}
