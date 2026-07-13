using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TMPro
{
	[AddComponentMenu("UI (Canvas)/TextMeshPro - Input Field", 11)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.ugui@2.0/manual/TextMeshPro/index.html")]
	public class TMP_InputField : Selectable, IUpdateSelectedHandler, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, ISubmitHandler, ICancelHandler, ICanvasElement, ILayoutElement, IScrollHandler
	{
		[DllImport("user32.dll")]
		private static extern IntPtr GetFocus();

		[DllImport("user32.dll")]
		private static extern bool GetWindowRect(IntPtr hWnd, out TMP_InputField.RECT lpRect);

		[DllImport("user32.dll")]
		private static extern bool ClientToScreen(IntPtr hWnd, ref TMP_InputField.POINT lpPoint);

		private BaseInput inputSystem
		{
			get
			{
				if (EventSystem.current && EventSystem.current.currentInputModule)
				{
					return EventSystem.current.currentInputModule.input;
				}
				return null;
			}
		}

		private string compositionString
		{
			get
			{
				if (!(this.inputSystem != null))
				{
					return Input.compositionString;
				}
				return this.inputSystem.compositionString;
			}
		}

		private int compositionLength
		{
			get
			{
				if (this.m_ReadOnly)
				{
					return 0;
				}
				return this.compositionString.Length;
			}
		}

		protected TMP_InputField()
		{
			this.SetTextComponentWrapMode();
		}

		protected Mesh mesh
		{
			get
			{
				if (this.m_Mesh == null)
				{
					this.m_Mesh = new Mesh();
				}
				return this.m_Mesh;
			}
		}

		public virtual bool shouldActivateOnSelect
		{
			get
			{
				return this.m_ShouldActivateOnSelect && Application.platform != RuntimePlatform.tvOS;
			}
			set
			{
				this.m_ShouldActivateOnSelect = value;
			}
		}

		public bool shouldHideMobileInput
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				if (platform <= RuntimePlatform.Android)
				{
					if (platform != RuntimePlatform.IPhonePlayer && platform != RuntimePlatform.Android)
					{
						return true;
					}
				}
				else if (platform != RuntimePlatform.WebGLPlayer && platform != RuntimePlatform.tvOS)
				{
					return true;
				}
				return this.m_HideMobileInput;
			}
			set
			{
				RuntimePlatform platform = Application.platform;
				if (platform <= RuntimePlatform.Android)
				{
					if (platform != RuntimePlatform.IPhonePlayer && platform != RuntimePlatform.Android)
					{
						goto IL_002E;
					}
				}
				else if (platform != RuntimePlatform.WebGLPlayer && platform != RuntimePlatform.tvOS)
				{
					goto IL_002E;
				}
				SetPropertyUtility.SetStruct<bool>(ref this.m_HideMobileInput, value);
				return;
				IL_002E:
				this.m_HideMobileInput = true;
			}
		}

		public bool shouldHideSoftKeyboard
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				if (platform <= RuntimePlatform.MetroPlayerARM)
				{
					if (platform != RuntimePlatform.IPhonePlayer && platform != RuntimePlatform.Android && platform - RuntimePlatform.WebGLPlayer > 3)
					{
						return true;
					}
				}
				else if (platform <= RuntimePlatform.Switch)
				{
					if (platform != RuntimePlatform.PS4 && platform - RuntimePlatform.tvOS > 1)
					{
						return true;
					}
				}
				else if (platform - RuntimePlatform.GameCoreXboxSeries > 2 && platform - RuntimePlatform.VisionOS > 1)
				{
					return true;
				}
				return this.m_HideSoftKeyboard;
			}
			set
			{
				RuntimePlatform platform = Application.platform;
				if (platform <= RuntimePlatform.MetroPlayerARM)
				{
					if (platform != RuntimePlatform.IPhonePlayer && platform != RuntimePlatform.Android && platform - RuntimePlatform.WebGLPlayer > 3)
					{
						goto IL_004D;
					}
				}
				else if (platform <= RuntimePlatform.Switch)
				{
					if (platform != RuntimePlatform.PS4 && platform - RuntimePlatform.tvOS > 1)
					{
						goto IL_004D;
					}
				}
				else if (platform - RuntimePlatform.GameCoreXboxSeries > 2 && platform - RuntimePlatform.VisionOS > 1)
				{
					goto IL_004D;
				}
				SetPropertyUtility.SetStruct<bool>(ref this.m_HideSoftKeyboard, value);
				goto IL_0054;
				IL_004D:
				this.m_HideSoftKeyboard = true;
				IL_0054:
				if (this.m_HideSoftKeyboard && this.m_SoftKeyboard != null && TouchScreenKeyboard.isSupported && this.m_SoftKeyboard.active)
				{
					this.m_SoftKeyboard.active = false;
					this.m_SoftKeyboard = null;
				}
			}
		}

		private bool isKeyboardUsingEvents()
		{
			RuntimePlatform platform = Application.platform;
			if (platform > RuntimePlatform.WebGLPlayer)
			{
				if (platform <= RuntimePlatform.PS5)
				{
					if (platform != RuntimePlatform.PS4)
					{
						switch (platform)
						{
						case RuntimePlatform.tvOS:
							goto IL_006D;
						case RuntimePlatform.Switch:
						case RuntimePlatform.GameCoreXboxSeries:
						case RuntimePlatform.GameCoreXboxOne:
						case RuntimePlatform.PS5:
							break;
						case RuntimePlatform.Lumin:
						case RuntimePlatform.Stadia:
						case RuntimePlatform.LinuxHeadlessSimulation:
							return true;
						default:
							return true;
						}
					}
				}
				else
				{
					if (platform == RuntimePlatform.VisionOS)
					{
						goto IL_006D;
					}
					if (platform != RuntimePlatform.Switch2)
					{
						return true;
					}
				}
				return false;
			}
			if (platform != RuntimePlatform.IPhonePlayer)
			{
				if (platform == RuntimePlatform.Android)
				{
					return this.InPlaceEditing() && this.m_HideSoftKeyboard;
				}
				if (platform != RuntimePlatform.WebGLPlayer)
				{
					return true;
				}
				return this.m_SoftKeyboard == null || !this.m_SoftKeyboard.active;
			}
			IL_006D:
			return this.m_HideSoftKeyboard;
		}

		private bool isUWP()
		{
			return Application.platform == RuntimePlatform.MetroPlayerX86 || Application.platform == RuntimePlatform.MetroPlayerX64 || Application.platform == RuntimePlatform.MetroPlayerARM;
		}

		public string text
		{
			get
			{
				return this.m_Text;
			}
			set
			{
				this.SetText(value, true);
			}
		}

		public void SetTextWithoutNotify(string input)
		{
			this.SetText(input, false);
		}

		private void SetText(string value, bool sendCallback = true)
		{
			if (this.text == value)
			{
				return;
			}
			if (value == null)
			{
				value = "";
			}
			value = value.Replace("\0", string.Empty);
			this.m_Text = value;
			if (this.m_SoftKeyboard != null)
			{
				this.m_SoftKeyboard.text = this.m_Text;
			}
			if (this.m_StringPosition > this.m_Text.Length)
			{
				this.m_StringPosition = (this.m_StringSelectPosition = this.m_Text.Length);
			}
			else if (this.m_StringSelectPosition > this.m_Text.Length)
			{
				this.m_StringSelectPosition = this.m_Text.Length;
			}
			this.m_forceRectTransformAdjustment = true;
			this.m_IsTextComponentUpdateRequired = true;
			this.UpdateLabel();
			if (sendCallback)
			{
				this.SendOnValueChanged();
			}
		}

		public bool isFocused
		{
			get
			{
				return this.m_AllowInput;
			}
		}

		public float caretBlinkRate
		{
			get
			{
				return this.m_CaretBlinkRate;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<float>(ref this.m_CaretBlinkRate, value) && this.m_AllowInput)
				{
					this.SetCaretActive();
				}
			}
		}

		public int caretWidth
		{
			get
			{
				return this.m_CaretWidth;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<int>(ref this.m_CaretWidth, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		public RectTransform textViewport
		{
			get
			{
				return this.m_TextViewport;
			}
			set
			{
				SetPropertyUtility.SetClass<RectTransform>(ref this.m_TextViewport, value);
			}
		}

		public TMP_Text textComponent
		{
			get
			{
				return this.m_TextComponent;
			}
			set
			{
				if (SetPropertyUtility.SetClass<TMP_Text>(ref this.m_TextComponent, value))
				{
					this.SetTextComponentWrapMode();
				}
			}
		}

		public Graphic placeholder
		{
			get
			{
				return this.m_Placeholder;
			}
			set
			{
				SetPropertyUtility.SetClass<Graphic>(ref this.m_Placeholder, value);
			}
		}

		public Scrollbar verticalScrollbar
		{
			get
			{
				return this.m_VerticalScrollbar;
			}
			set
			{
				if (this.m_VerticalScrollbar != null)
				{
					this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.OnScrollbarValueChange));
				}
				SetPropertyUtility.SetClass<Scrollbar>(ref this.m_VerticalScrollbar, value);
				if (this.m_VerticalScrollbar)
				{
					this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.OnScrollbarValueChange));
				}
			}
		}

		public float scrollSensitivity
		{
			get
			{
				return this.m_ScrollSensitivity;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<float>(ref this.m_ScrollSensitivity, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		public Color caretColor
		{
			get
			{
				if (!this.customCaretColor)
				{
					return this.textComponent.color;
				}
				return this.m_CaretColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref this.m_CaretColor, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		public bool customCaretColor
		{
			get
			{
				return this.m_CustomCaretColor;
			}
			set
			{
				if (this.m_CustomCaretColor != value)
				{
					this.m_CustomCaretColor = value;
					this.MarkGeometryAsDirty();
				}
			}
		}

		public Color selectionColor
		{
			get
			{
				return this.m_SelectionColor;
			}
			set
			{
				if (SetPropertyUtility.SetColor(ref this.m_SelectionColor, value))
				{
					this.MarkGeometryAsDirty();
				}
			}
		}

		public TMP_InputField.SubmitEvent onEndEdit
		{
			get
			{
				return this.m_OnEndEdit;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.SubmitEvent>(ref this.m_OnEndEdit, value);
			}
		}

		public TMP_InputField.SubmitEvent onSubmit
		{
			get
			{
				return this.m_OnSubmit;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.SubmitEvent>(ref this.m_OnSubmit, value);
			}
		}

		public TMP_InputField.SelectionEvent onSelect
		{
			get
			{
				return this.m_OnSelect;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.SelectionEvent>(ref this.m_OnSelect, value);
			}
		}

		public TMP_InputField.SelectionEvent onDeselect
		{
			get
			{
				return this.m_OnDeselect;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.SelectionEvent>(ref this.m_OnDeselect, value);
			}
		}

		public TMP_InputField.TextSelectionEvent onTextSelection
		{
			get
			{
				return this.m_OnTextSelection;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.TextSelectionEvent>(ref this.m_OnTextSelection, value);
			}
		}

		public TMP_InputField.TextSelectionEvent onEndTextSelection
		{
			get
			{
				return this.m_OnEndTextSelection;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.TextSelectionEvent>(ref this.m_OnEndTextSelection, value);
			}
		}

		public TMP_InputField.OnChangeEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.OnChangeEvent>(ref this.m_OnValueChanged, value);
			}
		}

		public TMP_InputField.TouchScreenKeyboardEvent onTouchScreenKeyboardStatusChanged
		{
			get
			{
				return this.m_OnTouchScreenKeyboardStatusChanged;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.TouchScreenKeyboardEvent>(ref this.m_OnTouchScreenKeyboardStatusChanged, value);
			}
		}

		public TMP_InputField.OnValidateInput onValidateInput
		{
			get
			{
				return this.m_OnValidateInput;
			}
			set
			{
				SetPropertyUtility.SetClass<TMP_InputField.OnValidateInput>(ref this.m_OnValidateInput, value);
			}
		}

		public int characterLimit
		{
			get
			{
				return this.m_CharacterLimit;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<int>(ref this.m_CharacterLimit, Math.Max(0, value)))
				{
					this.UpdateLabel();
					if (this.m_SoftKeyboard != null)
					{
						this.m_SoftKeyboard.characterLimit = value;
					}
				}
			}
		}

		public float pointSize
		{
			get
			{
				return this.m_GlobalPointSize;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<float>(ref this.m_GlobalPointSize, Math.Max(0f, value)))
				{
					this.SetGlobalPointSize(this.m_GlobalPointSize);
					this.UpdateLabel();
				}
			}
		}

		public TMP_FontAsset fontAsset
		{
			get
			{
				return this.m_GlobalFontAsset;
			}
			set
			{
				if (SetPropertyUtility.SetClass<TMP_FontAsset>(ref this.m_GlobalFontAsset, value))
				{
					this.SetGlobalFontAsset(this.m_GlobalFontAsset);
					this.UpdateLabel();
				}
			}
		}

		public bool onFocusSelectAll
		{
			get
			{
				return this.m_OnFocusSelectAll;
			}
			set
			{
				this.m_OnFocusSelectAll = value;
			}
		}

		public bool resetOnDeActivation
		{
			get
			{
				return this.m_ResetOnDeActivation;
			}
			set
			{
				this.m_ResetOnDeActivation = value;
			}
		}

		public bool keepTextSelectionVisible
		{
			get
			{
				return this.m_KeepTextSelectionVisible;
			}
			set
			{
				this.m_KeepTextSelectionVisible = value;
			}
		}

		public bool restoreOriginalTextOnEscape
		{
			get
			{
				return this.m_RestoreOriginalTextOnEscape;
			}
			set
			{
				this.m_RestoreOriginalTextOnEscape = value;
			}
		}

		public bool isRichTextEditingAllowed
		{
			get
			{
				return this.m_isRichTextEditingAllowed;
			}
			set
			{
				this.m_isRichTextEditingAllowed = value;
			}
		}

		public TMP_InputField.ContentType contentType
		{
			get
			{
				return this.m_ContentType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TMP_InputField.ContentType>(ref this.m_ContentType, value))
				{
					this.EnforceContentType();
				}
			}
		}

		public TMP_InputField.LineType lineType
		{
			get
			{
				return this.m_LineType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TMP_InputField.LineType>(ref this.m_LineType, value))
				{
					this.SetToCustomIfContentTypeIsNot(new TMP_InputField.ContentType[]
					{
						TMP_InputField.ContentType.Standard,
						TMP_InputField.ContentType.Autocorrected
					});
					this.SetTextComponentWrapMode();
				}
			}
		}

		public int lineLimit
		{
			get
			{
				return this.m_LineLimit;
			}
			set
			{
				if (this.m_LineType == TMP_InputField.LineType.SingleLine)
				{
					this.m_LineLimit = 1;
					return;
				}
				SetPropertyUtility.SetStruct<int>(ref this.m_LineLimit, value);
			}
		}

		public TMP_InputField.InputType inputType
		{
			get
			{
				return this.m_InputType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TMP_InputField.InputType>(ref this.m_InputType, value))
				{
					this.SetToCustom();
				}
			}
		}

		public TouchScreenKeyboard touchScreenKeyboard
		{
			get
			{
				return this.m_SoftKeyboard;
			}
		}

		public TouchScreenKeyboardType keyboardType
		{
			get
			{
				return this.m_KeyboardType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TouchScreenKeyboardType>(ref this.m_KeyboardType, value))
				{
					this.SetToCustom();
				}
			}
		}

		public TMP_InputField.CharacterValidation characterValidation
		{
			get
			{
				return this.m_CharacterValidation;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<TMP_InputField.CharacterValidation>(ref this.m_CharacterValidation, value))
				{
					this.SetToCustom();
				}
			}
		}

		public TMP_InputValidator inputValidator
		{
			get
			{
				return this.m_InputValidator;
			}
			set
			{
				if (SetPropertyUtility.SetClass<TMP_InputValidator>(ref this.m_InputValidator, value))
				{
					this.SetToCustom(TMP_InputField.CharacterValidation.CustomValidator);
				}
			}
		}

		public bool readOnly
		{
			get
			{
				return this.m_ReadOnly;
			}
			set
			{
				this.m_ReadOnly = value;
			}
		}

		public bool richText
		{
			get
			{
				return this.m_RichText;
			}
			set
			{
				this.m_RichText = value;
				this.SetTextComponentRichTextMode();
			}
		}

		public bool multiLine
		{
			get
			{
				return this.m_LineType == TMP_InputField.LineType.MultiLineNewline || this.lineType == TMP_InputField.LineType.MultiLineSubmit;
			}
		}

		public char asteriskChar
		{
			get
			{
				return this.m_AsteriskChar;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<char>(ref this.m_AsteriskChar, value))
				{
					this.UpdateLabel();
				}
			}
		}

		public bool wasCanceled
		{
			get
			{
				return this.m_WasCanceled;
			}
		}

		protected void ClampStringPos(ref int pos)
		{
			if (pos <= 0)
			{
				pos = 0;
				return;
			}
			if (pos > this.text.Length)
			{
				pos = this.text.Length;
			}
		}

		protected void ClampCaretPos(ref int pos)
		{
			if (pos > this.m_TextComponent.textInfo.characterCount - 1)
			{
				pos = this.m_TextComponent.textInfo.characterCount - 1;
			}
			if (pos <= 0)
			{
				pos = 0;
			}
		}

		private int ClampArrayIndex(int index)
		{
			if (index < 0)
			{
				return 0;
			}
			return index;
		}

		protected int caretPositionInternal
		{
			get
			{
				return this.m_CaretPosition + this.compositionLength;
			}
			set
			{
				this.m_CaretPosition = value;
				this.ClampCaretPos(ref this.m_CaretPosition);
			}
		}

		protected int stringPositionInternal
		{
			get
			{
				return this.m_StringPosition + this.compositionLength;
			}
			set
			{
				this.m_StringPosition = value;
				this.ClampStringPos(ref this.m_StringPosition);
			}
		}

		protected int caretSelectPositionInternal
		{
			get
			{
				return this.m_CaretSelectPosition + this.compositionLength;
			}
			set
			{
				this.m_CaretSelectPosition = value;
				this.ClampCaretPos(ref this.m_CaretSelectPosition);
			}
		}

		protected int stringSelectPositionInternal
		{
			get
			{
				return this.m_StringSelectPosition + this.compositionLength;
			}
			set
			{
				this.m_StringSelectPosition = value;
				this.ClampStringPos(ref this.m_StringSelectPosition);
			}
		}

		private bool hasSelection
		{
			get
			{
				return this.stringPositionInternal != this.stringSelectPositionInternal;
			}
		}

		public int caretPosition
		{
			get
			{
				return this.caretSelectPositionInternal;
			}
			set
			{
				this.selectionAnchorPosition = value;
				this.selectionFocusPosition = value;
				this.UpdateStringIndexFromCaretPosition();
			}
		}

		public int selectionAnchorPosition
		{
			get
			{
				return this.caretPositionInternal;
			}
			set
			{
				if (this.compositionLength != 0)
				{
					return;
				}
				this.caretPositionInternal = value;
				this.m_IsStringPositionDirty = true;
			}
		}

		public int selectionFocusPosition
		{
			get
			{
				return this.caretSelectPositionInternal;
			}
			set
			{
				if (this.compositionLength != 0)
				{
					return;
				}
				this.caretSelectPositionInternal = value;
				this.m_IsStringPositionDirty = true;
			}
		}

		public int stringPosition
		{
			get
			{
				return this.stringSelectPositionInternal;
			}
			set
			{
				this.selectionStringAnchorPosition = value;
				this.selectionStringFocusPosition = value;
				this.UpdateCaretPositionFromStringIndex();
			}
		}

		public int selectionStringAnchorPosition
		{
			get
			{
				return this.stringPositionInternal;
			}
			set
			{
				if (this.compositionLength != 0)
				{
					return;
				}
				this.stringPositionInternal = value;
				this.m_IsCaretPositionDirty = true;
			}
		}

		public int selectionStringFocusPosition
		{
			get
			{
				return this.stringSelectPositionInternal;
			}
			set
			{
				if (this.compositionLength != 0)
				{
					return;
				}
				this.stringSelectPositionInternal = value;
				this.m_IsCaretPositionDirty = true;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_Text == null)
			{
				this.m_Text = string.Empty;
			}
			this.m_IsApplePlatform = SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX || SystemInfo.operatingSystem.Contains("iOS") || SystemInfo.operatingSystem.Contains("tvOS");
			if (base.GetComponent<ILayoutController>() != null)
			{
				this.m_IsDrivenByLayoutComponents = true;
				this.m_LayoutGroup = base.GetComponent<LayoutGroup>();
			}
			else
			{
				this.m_IsDrivenByLayoutComponents = false;
			}
			if (Application.isPlaying && this.m_CachedInputRenderer == null && this.m_TextComponent != null)
			{
				GameObject gameObject = new GameObject("Caret", new Type[] { typeof(TMP_SelectionCaret) });
				gameObject.hideFlags = HideFlags.DontSave;
				gameObject.transform.SetParent(this.m_TextComponent.transform.parent);
				gameObject.transform.SetAsFirstSibling();
				gameObject.layer = base.gameObject.layer;
				this.caretRectTrans = gameObject.GetComponent<RectTransform>();
				this.m_CachedInputRenderer = gameObject.GetComponent<CanvasRenderer>();
				this.m_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);
				gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
				this.AssignPositioningIfNeeded();
			}
			this.m_RectTransform = base.GetComponent<RectTransform>();
			IScrollHandler[] componentsInParent = base.GetComponentsInParent<IScrollHandler>();
			if (componentsInParent.Length > 1)
			{
				this.m_IScrollHandlerParent = componentsInParent[1] as ScrollRect;
			}
			if (this.m_TextViewport != null)
			{
				this.m_TextViewportRectMask = this.m_TextViewport.GetComponent<RectMask2D>();
				this.UpdateMaskRegions();
			}
			if (this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.SetMaterial(Graphic.defaultGraphicMaterial, Texture2D.whiteTexture);
			}
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
				this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
				if (this.m_VerticalScrollbar != null)
				{
					this.m_VerticalScrollbar.onValueChanged.AddListener(new UnityAction<float>(this.OnScrollbarValueChange));
				}
				this.UpdateLabel();
			}
			this.m_TouchKeyboardAllowsInPlaceEditing = TouchScreenKeyboard.isInPlaceEditingAllowed;
			TMPro_EventManager.TEXT_CHANGED_EVENT.Add(new Action<global::UnityEngine.Object>(this.ON_TEXT_CHANGED));
		}

		protected override void OnDisable()
		{
			this.m_BlinkCoroutine = null;
			this.DeactivateInputField();
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
				this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
				if (this.m_VerticalScrollbar != null)
				{
					this.m_VerticalScrollbar.onValueChanged.RemoveListener(new UnityAction<float>(this.OnScrollbarValueChange));
				}
			}
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.Clear();
			}
			if (this.m_Mesh != null)
			{
				global::UnityEngine.Object.DestroyImmediate(this.m_Mesh);
			}
			this.m_Mesh = null;
			TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(new Action<global::UnityEngine.Object>(this.ON_TEXT_CHANGED));
			base.OnDisable();
		}

		private void ON_TEXT_CHANGED(global::UnityEngine.Object obj)
		{
			if (obj == this.m_TextComponent && !this.m_IsStringPositionDirty)
			{
				if (Application.isPlaying && this.compositionLength == 0)
				{
					this.UpdateCaretPositionFromStringIndex();
				}
				if (this.m_VerticalScrollbar)
				{
					this.UpdateScrollbar();
				}
			}
		}

		private IEnumerator CaretBlink()
		{
			this.m_CaretVisible = true;
			yield return null;
			while ((this.isFocused || this.m_SelectionStillActive) && this.m_CaretBlinkRate > 0f)
			{
				float num = 1f / this.m_CaretBlinkRate;
				bool flag = (Time.unscaledTime - this.m_BlinkStartTime) % num < num / 2f;
				if (this.m_CaretVisible != flag)
				{
					this.m_CaretVisible = flag;
					if (!this.hasSelection)
					{
						this.MarkGeometryAsDirty();
					}
				}
				yield return null;
			}
			this.m_BlinkCoroutine = null;
			yield break;
		}

		private void SetCaretVisible()
		{
			if (!this.m_AllowInput)
			{
				return;
			}
			this.m_CaretVisible = true;
			this.m_BlinkStartTime = Time.unscaledTime;
			this.SetCaretActive();
		}

		private void SetCaretActive()
		{
			if (!this.m_AllowInput)
			{
				return;
			}
			if (this.m_CaretBlinkRate > 0f)
			{
				if (this.m_BlinkCoroutine == null)
				{
					this.m_BlinkCoroutine = base.StartCoroutine(this.CaretBlink());
					return;
				}
			}
			else
			{
				this.m_CaretVisible = true;
			}
		}

		protected void OnFocus()
		{
			if (this.m_OnFocusSelectAll)
			{
				this.SelectAll();
			}
			if (this.onFocus != null)
			{
				this.onFocus();
			}
		}

		protected void SelectAll()
		{
			this.m_isSelectAll = true;
			this.stringPositionInternal = this.text.Length;
			this.stringSelectPositionInternal = 0;
		}

		public void MoveTextEnd(bool shift)
		{
			if (this.m_isRichTextEditingAllowed)
			{
				int length = this.text.Length;
				if (shift)
				{
					this.stringSelectPositionInternal = length;
				}
				else
				{
					this.stringPositionInternal = length;
					this.stringSelectPositionInternal = this.stringPositionInternal;
				}
			}
			else
			{
				int num = this.m_TextComponent.textInfo.characterCount - 1;
				if (shift)
				{
					this.caretSelectPositionInternal = num;
					this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(num);
				}
				else
				{
					this.caretPositionInternal = (this.caretSelectPositionInternal = num);
					this.stringSelectPositionInternal = (this.stringPositionInternal = this.GetStringIndexFromCaretPosition(num));
				}
			}
			this.UpdateLabel();
		}

		public void MoveTextStart(bool shift)
		{
			if (this.m_isRichTextEditingAllowed)
			{
				int num = 0;
				if (shift)
				{
					this.stringSelectPositionInternal = num;
				}
				else
				{
					this.stringPositionInternal = num;
					this.stringSelectPositionInternal = this.stringPositionInternal;
				}
			}
			else
			{
				int num2 = 0;
				if (shift)
				{
					this.caretSelectPositionInternal = num2;
					this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(num2);
				}
				else
				{
					this.caretPositionInternal = (this.caretSelectPositionInternal = num2);
					this.stringSelectPositionInternal = (this.stringPositionInternal = this.GetStringIndexFromCaretPosition(num2));
				}
			}
			this.UpdateLabel();
		}

		public void MoveToEndOfLine(bool shift, bool ctrl)
		{
			int lineNumber = this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal].lineNumber;
			int num = (ctrl ? (this.m_TextComponent.textInfo.characterCount - 1) : this.m_TextComponent.textInfo.lineInfo[lineNumber].lastCharacterIndex);
			int index = this.m_TextComponent.textInfo.characterInfo[num].index;
			if (shift)
			{
				this.stringSelectPositionInternal = index;
				this.caretSelectPositionInternal = num;
			}
			else
			{
				this.stringPositionInternal = index;
				this.stringSelectPositionInternal = this.stringPositionInternal;
				this.caretSelectPositionInternal = (this.caretPositionInternal = num);
			}
			this.UpdateLabel();
		}

		public void MoveToStartOfLine(bool shift, bool ctrl)
		{
			int lineNumber = this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal].lineNumber;
			int num = (ctrl ? 0 : this.m_TextComponent.textInfo.lineInfo[lineNumber].firstCharacterIndex);
			int num2 = 0;
			if (num > 0)
			{
				num2 = this.m_TextComponent.textInfo.characterInfo[num - 1].index + this.m_TextComponent.textInfo.characterInfo[num - 1].stringLength;
			}
			if (shift)
			{
				this.stringSelectPositionInternal = num2;
				this.caretSelectPositionInternal = num;
			}
			else
			{
				this.stringPositionInternal = num2;
				this.stringSelectPositionInternal = this.stringPositionInternal;
				this.caretSelectPositionInternal = (this.caretPositionInternal = num);
			}
			this.UpdateLabel();
		}

		private static string clipboard
		{
			get
			{
				return GUIUtility.systemCopyBuffer;
			}
			set
			{
				GUIUtility.systemCopyBuffer = value;
			}
		}

		private bool InPlaceEditing()
		{
			if (this.m_TouchKeyboardAllowsInPlaceEditing)
			{
				return true;
			}
			if (this.isUWP())
			{
				return !TouchScreenKeyboard.isSupported;
			}
			return (TouchScreenKeyboard.isSupported && this.shouldHideSoftKeyboard) || !TouchScreenKeyboard.isSupported || this.shouldHideSoftKeyboard || this.shouldHideMobileInput;
		}

		private bool InPlaceEditingChanged()
		{
			return !TMP_InputField.s_IsQuestDevice && this.m_TouchKeyboardAllowsInPlaceEditing != TouchScreenKeyboard.isInPlaceEditingAllowed;
		}

		private bool TouchScreenKeyboardShouldBeUsed()
		{
			RuntimePlatform platform = Application.platform;
			if (platform != RuntimePlatform.Android && platform != RuntimePlatform.WebGLPlayer)
			{
				return TouchScreenKeyboard.isSupported;
			}
			if (TMP_InputField.s_IsQuestDevice)
			{
				return TouchScreenKeyboard.isSupported;
			}
			return !TouchScreenKeyboard.isInPlaceEditingAllowed;
		}

		private void UpdateKeyboardStringPosition()
		{
			if (this.m_HideMobileInput && this.m_SoftKeyboard != null && this.m_SoftKeyboard.canSetSelection && (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS))
			{
				int num = Mathf.Min(this.stringSelectPositionInternal, this.stringPositionInternal);
				int num2 = Mathf.Abs(this.stringSelectPositionInternal - this.stringPositionInternal);
				this.m_SoftKeyboard.selection = new RangeInt(num, num2);
			}
		}

		private void UpdateStringPositionFromKeyboard()
		{
			RangeInt selection = this.m_SoftKeyboard.selection;
			int start = selection.start;
			int end = selection.end;
			bool flag = false;
			if (this.stringPositionInternal != start)
			{
				flag = true;
				this.stringPositionInternal = start;
				this.caretPositionInternal = this.GetCaretPositionFromStringIndex(this.stringPositionInternal);
			}
			if (this.stringSelectPositionInternal != end)
			{
				this.stringSelectPositionInternal = end;
				flag = true;
				this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
			}
			if (flag)
			{
				this.m_BlinkStartTime = Time.unscaledTime;
				this.UpdateLabel();
			}
		}

		protected virtual void LateUpdate()
		{
			if (this.m_ShouldActivateNextUpdate)
			{
				if (!this.isFocused)
				{
					this.ActivateInputFieldInternal();
					this.m_ShouldActivateNextUpdate = false;
					return;
				}
				this.m_ShouldActivateNextUpdate = false;
			}
			if (this.isFocused && this.InPlaceEditingChanged())
			{
				this.DeactivateInputField();
			}
			if (!this.isFocused && this.m_SelectionStillActive)
			{
				GameObject gameObject = ((EventSystem.current != null) ? EventSystem.current.currentSelectedGameObject : null);
				if (gameObject == null && this.m_ResetOnDeActivation)
				{
					this.ReleaseSelection();
					return;
				}
				if (gameObject != null && gameObject != base.gameObject)
				{
					if (gameObject == this.m_PreviouslySelectedObject)
					{
						return;
					}
					this.m_PreviouslySelectedObject = gameObject;
					if (this.m_VerticalScrollbar && gameObject == this.m_VerticalScrollbar.gameObject)
					{
						return;
					}
					if (this.m_ResetOnDeActivation)
					{
						this.ReleaseSelection();
						return;
					}
					if (!this.m_KeepTextSelectionVisible && gameObject.GetComponent<TMP_InputField>() != null)
					{
						this.ReleaseSelection();
					}
					return;
				}
				else if (Input.GetKeyDown(KeyCode.Mouse0))
				{
					bool flag = false;
					float unscaledTime = Time.unscaledTime;
					if (this.m_KeyDownStartTime + this.m_DoubleClickDelay > unscaledTime)
					{
						flag = true;
					}
					this.m_KeyDownStartTime = unscaledTime;
					if (flag)
					{
						this.ReleaseSelection();
						return;
					}
				}
			}
			this.UpdateMaskRegions();
			if ((this.InPlaceEditing() && this.isKeyboardUsingEvents()) || !this.isFocused)
			{
				return;
			}
			this.AssignPositioningIfNeeded();
			if (this.m_SoftKeyboard == null || this.m_SoftKeyboard.status != TouchScreenKeyboard.Status.Visible)
			{
				if (this.m_SoftKeyboard != null)
				{
					if (!this.m_ReadOnly)
					{
						this.text = this.m_SoftKeyboard.text;
					}
					TouchScreenKeyboard.Status status = this.m_SoftKeyboard.status;
					if (this.m_LastKeyCode != KeyCode.Return && status == TouchScreenKeyboard.Status.Done && this.isUWP())
					{
						status = TouchScreenKeyboard.Status.Canceled;
						this.m_IsKeyboardBeingClosedInHoloLens = true;
					}
					switch (status)
					{
					case TouchScreenKeyboard.Status.Done:
						this.m_ReleaseSelection = true;
						this.SendTouchScreenKeyboardStatusChanged();
						this.OnSubmit(null);
						break;
					case TouchScreenKeyboard.Status.Canceled:
						this.m_ReleaseSelection = true;
						this.m_WasCanceled = true;
						this.SendTouchScreenKeyboardStatusChanged();
						return;
					case TouchScreenKeyboard.Status.LostFocus:
						this.SendTouchScreenKeyboardStatusChanged();
						return;
					default:
						return;
					}
				}
				return;
			}
			string text = this.m_SoftKeyboard.text;
			if (this.m_Text != text)
			{
				if (this.m_ReadOnly)
				{
					this.m_SoftKeyboard.text = this.m_Text;
				}
				else
				{
					this.m_Text = "";
					foreach (char c in text)
					{
						bool flag2 = false;
						if (c == '\r' || c == '\u0003')
						{
							c = '\n';
						}
						if (this.onValidateInput != null)
						{
							c = this.onValidateInput(this.m_Text, this.m_Text.Length, c);
						}
						else if (this.characterValidation != TMP_InputField.CharacterValidation.None)
						{
							string text2 = this.m_Text;
							c = this.Validate(this.m_Text, this.m_Text.Length, c);
							flag2 = text2 != this.m_Text;
						}
						if (this.lineType != TMP_InputField.LineType.MultiLineNewline && c == '\n')
						{
							this.UpdateLabel();
							this.OnSubmit(null);
							this.OnDeselect(null);
							return;
						}
						if (c != '\0' && (this.characterValidation != TMP_InputField.CharacterValidation.CustomValidator || !flag2))
						{
							this.m_Text += c.ToString();
						}
					}
					if (this.characterLimit > 0 && this.m_Text.Length > this.characterLimit)
					{
						this.m_Text = this.m_Text.Substring(0, this.characterLimit);
					}
					this.UpdateStringPositionFromKeyboard();
					if (this.m_Text != text)
					{
						this.m_SoftKeyboard.text = this.m_Text;
					}
					this.SendOnValueChangedAndUpdateLabel();
				}
			}
			else if (this.m_HideMobileInput && this.m_SoftKeyboard != null && this.m_SoftKeyboard.canSetSelection && Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.tvOS)
			{
				int num = Mathf.Min(this.stringSelectPositionInternal, this.stringPositionInternal);
				int num2 = Mathf.Abs(this.stringSelectPositionInternal - this.stringPositionInternal);
				this.m_SoftKeyboard.selection = new RangeInt(num, num2);
			}
			else if ((this.m_HideMobileInput && Application.platform == RuntimePlatform.Android) || (this.m_SoftKeyboard.canSetSelection && (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.tvOS)))
			{
				this.UpdateStringPositionFromKeyboard();
			}
			if (this.m_SoftKeyboard != null && this.m_SoftKeyboard.status != TouchScreenKeyboard.Status.Visible)
			{
				if (this.m_SoftKeyboard.status == TouchScreenKeyboard.Status.Canceled)
				{
					this.m_WasCanceled = true;
				}
				this.OnDeselect(null);
			}
		}

		private bool MayDrag(PointerEventData eventData)
		{
			return this.IsActive() && this.IsInteractable() && eventData.button == PointerEventData.InputButton.Left && this.m_TextComponent != null && (this.m_SoftKeyboard == null || this.shouldHideSoftKeyboard || this.shouldHideMobileInput);
		}

		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			this.m_UpdateDrag = true;
		}

		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			CaretPosition caretPosition;
			int cursorIndexFromPosition = TMP_TextUtilities.GetCursorIndexFromPosition(this.m_TextComponent, eventData.position, eventData.pressEventCamera, out caretPosition);
			if (this.m_isRichTextEditingAllowed)
			{
				if (caretPosition == CaretPosition.Left)
				{
					this.stringSelectPositionInternal = this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].index;
				}
				else if (caretPosition == CaretPosition.Right)
				{
					this.stringSelectPositionInternal = this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].index + this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].stringLength;
				}
			}
			else if (caretPosition == CaretPosition.Left)
			{
				this.stringSelectPositionInternal = ((cursorIndexFromPosition == 0) ? this.m_TextComponent.textInfo.characterInfo[0].index : (this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition - 1].index + this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition - 1].stringLength));
			}
			else if (caretPosition == CaretPosition.Right)
			{
				this.stringSelectPositionInternal = this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].index + this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].stringLength;
			}
			this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
			this.MarkGeometryAsDirty();
			this.m_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(this.textViewport, eventData.position, eventData.pressEventCamera);
			if (this.m_DragPositionOutOfBounds && this.m_DragCoroutine == null)
			{
				this.m_DragCoroutine = base.StartCoroutine(this.MouseDragOutsideRect(eventData));
			}
			this.UpdateKeyboardStringPosition();
			eventData.Use();
		}

		private IEnumerator MouseDragOutsideRect(PointerEventData eventData)
		{
			while (this.m_UpdateDrag && this.m_DragPositionOutOfBounds)
			{
				Vector2 vector;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textViewport, eventData.position, eventData.pressEventCamera, out vector);
				Rect rect = this.textViewport.rect;
				if (this.multiLine)
				{
					if (vector.y > rect.yMax)
					{
						this.MoveUp(true, false);
					}
					else if (vector.y < rect.yMin)
					{
						this.MoveDown(true, false);
					}
				}
				else if (vector.x < rect.xMin)
				{
					this.MoveLeft(true, false);
				}
				else if (vector.x > rect.xMax)
				{
					this.MoveRight(true, false);
				}
				this.UpdateLabel();
				float num = (this.multiLine ? 0.1f : 0.05f);
				if (this.m_WaitForSecondsRealtime == null)
				{
					this.m_WaitForSecondsRealtime = new WaitForSecondsRealtime(num);
				}
				else
				{
					this.m_WaitForSecondsRealtime.waitTime = num;
				}
				yield return this.m_WaitForSecondsRealtime;
			}
			this.m_DragCoroutine = null;
			yield break;
		}

		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			this.m_UpdateDrag = false;
		}

		public override void OnPointerDown(PointerEventData eventData)
		{
			if (!this.MayDrag(eventData))
			{
				return;
			}
			EventSystem.current.SetSelectedGameObject(base.gameObject, eventData);
			bool allowInput = this.m_AllowInput;
			base.OnPointerDown(eventData);
			if (!this.InPlaceEditing() && (this.m_SoftKeyboard == null || !this.m_SoftKeyboard.active))
			{
				this.OnSelect(eventData);
				return;
			}
			bool flag = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			bool flag2 = false;
			float unscaledTime = Time.unscaledTime;
			if (this.m_PointerDownClickStartTime + this.m_DoubleClickDelay > unscaledTime)
			{
				flag2 = true;
			}
			this.m_PointerDownClickStartTime = unscaledTime;
			if (allowInput || !this.m_OnFocusSelectAll)
			{
				CaretPosition caretPosition;
				int cursorIndexFromPosition = TMP_TextUtilities.GetCursorIndexFromPosition(this.m_TextComponent, eventData.position, eventData.pressEventCamera, out caretPosition);
				if (flag)
				{
					if (this.m_isRichTextEditingAllowed)
					{
						if (caretPosition == CaretPosition.Left)
						{
							this.stringSelectPositionInternal = this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].index;
						}
						else if (caretPosition == CaretPosition.Right)
						{
							this.stringSelectPositionInternal = this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].index + this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].stringLength;
						}
					}
					else if (caretPosition == CaretPosition.Left)
					{
						this.stringSelectPositionInternal = ((cursorIndexFromPosition == 0) ? this.m_TextComponent.textInfo.characterInfo[0].index : (this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition - 1].index + this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition - 1].stringLength));
					}
					else if (caretPosition == CaretPosition.Right)
					{
						this.stringSelectPositionInternal = this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].index + this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].stringLength;
					}
				}
				else if (this.m_isRichTextEditingAllowed)
				{
					if (caretPosition == CaretPosition.Left)
					{
						this.stringPositionInternal = (this.stringSelectPositionInternal = this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].index);
					}
					else if (caretPosition == CaretPosition.Right)
					{
						this.stringPositionInternal = (this.stringSelectPositionInternal = this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].index + this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].stringLength);
					}
				}
				else if (caretPosition == CaretPosition.Left)
				{
					this.stringPositionInternal = (this.stringSelectPositionInternal = ((cursorIndexFromPosition == 0) ? this.m_TextComponent.textInfo.characterInfo[0].index : (this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition - 1].index + this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition - 1].stringLength)));
				}
				else if (caretPosition == CaretPosition.Right)
				{
					this.stringPositionInternal = (this.stringSelectPositionInternal = this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].index + this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].stringLength);
				}
				if (flag2)
				{
					int num = TMP_TextUtilities.FindIntersectingWord(this.m_TextComponent, eventData.position, eventData.pressEventCamera);
					if (num != -1)
					{
						this.caretPositionInternal = this.m_TextComponent.textInfo.wordInfo[num].firstCharacterIndex;
						this.caretSelectPositionInternal = this.m_TextComponent.textInfo.wordInfo[num].lastCharacterIndex + 1;
						this.stringPositionInternal = this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal].index;
						this.stringSelectPositionInternal = this.m_TextComponent.textInfo.characterInfo[this.caretSelectPositionInternal - 1].index + this.m_TextComponent.textInfo.characterInfo[this.caretSelectPositionInternal - 1].stringLength;
					}
					else
					{
						this.caretPositionInternal = cursorIndexFromPosition;
						this.caretSelectPositionInternal = this.caretPositionInternal + 1;
						this.stringPositionInternal = this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].index;
						this.stringSelectPositionInternal = this.stringPositionInternal + this.m_TextComponent.textInfo.characterInfo[cursorIndexFromPosition].stringLength;
					}
				}
				else
				{
					this.caretPositionInternal = (this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringPositionInternal));
				}
				this.m_isSelectAll = false;
			}
			this.UpdateLabel();
			this.UpdateKeyboardStringPosition();
			eventData.Use();
		}

		protected TMP_InputField.EditState KeyPressed(Event evt)
		{
			EventModifiers modifiers = evt.modifiers;
			bool flag = (this.m_IsApplePlatform ? ((modifiers & EventModifiers.Command) > EventModifiers.None) : ((modifiers & EventModifiers.Control) > EventModifiers.None));
			bool flag2 = (modifiers & EventModifiers.Shift) > EventModifiers.None;
			bool flag3 = (modifiers & EventModifiers.Alt) > EventModifiers.None;
			bool flag4 = flag && !flag3 && !flag2;
			this.m_LastKeyCode = evt.keyCode;
			KeyCode keyCode = evt.keyCode;
			if (keyCode <= KeyCode.A)
			{
				if (keyCode <= KeyCode.Return)
				{
					if (keyCode == KeyCode.Backspace)
					{
						this.Backspace();
						return TMP_InputField.EditState.Continue;
					}
					if (keyCode != KeyCode.Return)
					{
						goto IL_0229;
					}
				}
				else
				{
					if (keyCode == KeyCode.Escape)
					{
						this.m_ReleaseSelection = true;
						this.m_WasCanceled = true;
						return TMP_InputField.EditState.Finish;
					}
					if (keyCode != KeyCode.A)
					{
						goto IL_0229;
					}
					if (flag4)
					{
						this.SelectAll();
						return TMP_InputField.EditState.Continue;
					}
					goto IL_0229;
				}
			}
			else if (keyCode <= KeyCode.V)
			{
				if (keyCode != KeyCode.C)
				{
					if (keyCode != KeyCode.V)
					{
						goto IL_0229;
					}
					if (flag4)
					{
						this.Append(TMP_InputField.clipboard);
						return TMP_InputField.EditState.Continue;
					}
					goto IL_0229;
				}
				else
				{
					if (flag4)
					{
						if (this.inputType != TMP_InputField.InputType.Password)
						{
							TMP_InputField.clipboard = this.GetSelectedString();
						}
						else
						{
							TMP_InputField.clipboard = "";
						}
						return TMP_InputField.EditState.Continue;
					}
					goto IL_0229;
				}
			}
			else if (keyCode != KeyCode.X)
			{
				if (keyCode == KeyCode.Delete)
				{
					this.DeleteKey();
					return TMP_InputField.EditState.Continue;
				}
				switch (keyCode)
				{
				case KeyCode.KeypadEnter:
					break;
				case KeyCode.KeypadEquals:
				case KeyCode.Insert:
					goto IL_0229;
				case KeyCode.UpArrow:
					this.MoveUp(flag2);
					return TMP_InputField.EditState.Continue;
				case KeyCode.DownArrow:
					this.MoveDown(flag2);
					return TMP_InputField.EditState.Continue;
				case KeyCode.RightArrow:
					this.MoveRight(flag2, flag);
					return TMP_InputField.EditState.Continue;
				case KeyCode.LeftArrow:
					this.MoveLeft(flag2, flag);
					return TMP_InputField.EditState.Continue;
				case KeyCode.Home:
					this.MoveToStartOfLine(flag2, flag);
					return TMP_InputField.EditState.Continue;
				case KeyCode.End:
					this.MoveToEndOfLine(flag2, flag);
					return TMP_InputField.EditState.Continue;
				case KeyCode.PageUp:
					this.MovePageUp(flag2);
					return TMP_InputField.EditState.Continue;
				case KeyCode.PageDown:
					this.MovePageDown(flag2);
					return TMP_InputField.EditState.Continue;
				default:
					goto IL_0229;
				}
			}
			else
			{
				if (flag4)
				{
					if (this.inputType != TMP_InputField.InputType.Password)
					{
						TMP_InputField.clipboard = this.GetSelectedString();
					}
					else
					{
						TMP_InputField.clipboard = "";
					}
					this.Delete();
					this.UpdateTouchKeyboardFromEditChanges();
					this.SendOnValueChangedAndUpdateLabel();
					return TMP_InputField.EditState.Continue;
				}
				goto IL_0229;
			}
			if (this.lineType != TMP_InputField.LineType.MultiLineNewline)
			{
				this.m_ReleaseSelection = true;
				return TMP_InputField.EditState.Finish;
			}
			TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
			if (this.m_LineLimit > 0 && textInfo != null && textInfo.lineCount >= this.m_LineLimit)
			{
				this.m_ReleaseSelection = true;
				return TMP_InputField.EditState.Finish;
			}
			IL_0229:
			char c = evt.character;
			if (!this.multiLine && (c == '\t' || c == '\r' || c == '\n'))
			{
				return TMP_InputField.EditState.Continue;
			}
			if (c == '\r' || c == '\u0003')
			{
				c = '\n';
			}
			if (flag2 && c == '\n')
			{
				c = '\v';
			}
			if (this.IsValidChar(c))
			{
				this.Append(c);
			}
			if (c == '\0' && this.compositionLength > 0)
			{
				this.UpdateLabel();
			}
			return TMP_InputField.EditState.Continue;
		}

		protected virtual bool IsValidChar(char c)
		{
			return c != '\u007f' && (c == '\t' || c == '\n' || c >= ' ');
		}

		public void ProcessEvent(Event e)
		{
			this.KeyPressed(e);
		}

		public virtual void OnUpdateSelected(BaseEventData eventData)
		{
			if (!this.isFocused)
			{
				return;
			}
			bool flag = false;
			while (Event.PopEvent(this.m_ProcessingEvent))
			{
				EventType rawType = this.m_ProcessingEvent.rawType;
				if (rawType != EventType.KeyUp)
				{
					if (rawType == EventType.KeyDown)
					{
						flag = true;
						if (!this.m_IsCompositionActive || this.compositionLength != 0 || this.m_ProcessingEvent.character != '\0' || this.m_ProcessingEvent.modifiers != EventModifiers.None)
						{
							if (this.KeyPressed(this.m_ProcessingEvent) == TMP_InputField.EditState.Finish)
							{
								if (!this.m_WasCanceled)
								{
									this.SendOnSubmit();
								}
								this.DeactivateInputField();
								break;
							}
							this.m_IsTextComponentUpdateRequired = true;
							this.UpdateLabel();
						}
					}
					else if (rawType - EventType.ValidateCommand <= 1 && this.m_ProcessingEvent.commandName == "SelectAll")
					{
						this.SelectAll();
						flag = true;
					}
				}
			}
			if (flag || (this.m_IsCompositionActive && this.compositionLength > 0))
			{
				this.UpdateLabel();
				eventData.Use();
			}
		}

		public virtual void OnScroll(PointerEventData eventData)
		{
			if (this.m_LineType == TMP_InputField.LineType.SingleLine)
			{
				if (this.m_IScrollHandlerParent != null)
				{
					this.m_IScrollHandlerParent.OnScroll(eventData);
				}
				return;
			}
			if (this.m_TextComponent.preferredHeight < this.m_TextViewport.rect.height)
			{
				return;
			}
			float num = -eventData.scrollDelta.y;
			this.m_ScrollPosition = this.GetScrollPositionRelativeToViewport();
			this.m_ScrollPosition += 1f / (float)this.m_TextComponent.textInfo.lineCount * num * this.m_ScrollSensitivity;
			this.m_ScrollPosition = Mathf.Clamp01(this.m_ScrollPosition);
			this.AdjustTextPositionRelativeToViewport(this.m_ScrollPosition);
			if (this.m_VerticalScrollbar)
			{
				this.m_VerticalScrollbar.value = this.m_ScrollPosition;
			}
		}

		private float GetScrollPositionRelativeToViewport()
		{
			Rect rect = this.m_TextViewport.rect;
			return (float)((int)((this.m_TextComponent.textInfo.lineInfo[0].ascender + this.m_TextComponent.margin.y + this.m_TextComponent.margin.w - rect.yMax + this.m_TextComponent.rectTransform.anchoredPosition.y) / (this.m_TextComponent.preferredHeight - rect.height) * 1000f + 0.5f)) / 1000f;
		}

		private string GetSelectedString()
		{
			if (!this.hasSelection)
			{
				return "";
			}
			int num = this.stringPositionInternal;
			int num2 = this.stringSelectPositionInternal;
			if (num > num2)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			return this.text.Substring(num, num2 - num);
		}

		private int FindNextWordBegin()
		{
			if (this.stringSelectPositionInternal + 1 >= this.text.Length)
			{
				return this.text.Length;
			}
			int num = this.text.IndexOfAny(TMP_InputField.kSeparators, this.stringSelectPositionInternal + 1);
			if (num == -1)
			{
				num = this.text.Length;
			}
			else
			{
				num++;
			}
			return num;
		}

		private void MoveRight(bool shift, bool ctrl)
		{
			if (this.hasSelection && !shift)
			{
				this.stringPositionInternal = (this.stringSelectPositionInternal = Mathf.Max(this.stringPositionInternal, this.stringSelectPositionInternal));
				this.caretPositionInternal = (this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal));
				return;
			}
			int num;
			if (ctrl)
			{
				num = this.FindNextWordBegin();
			}
			else if (this.m_isRichTextEditingAllowed)
			{
				if (this.stringSelectPositionInternal < this.text.Length && char.IsHighSurrogate(this.text[this.stringSelectPositionInternal]))
				{
					num = this.stringSelectPositionInternal + 2;
				}
				else
				{
					num = this.stringSelectPositionInternal + 1;
				}
			}
			else if (this.m_TextComponent.textInfo.characterInfo[this.caretSelectPositionInternal].character == '\r' && this.m_TextComponent.textInfo.characterInfo[this.caretSelectPositionInternal + 1].character == '\n')
			{
				num = this.m_TextComponent.textInfo.characterInfo[this.caretSelectPositionInternal + 1].index + this.m_TextComponent.textInfo.characterInfo[this.caretSelectPositionInternal + 1].stringLength;
			}
			else
			{
				num = this.m_TextComponent.textInfo.characterInfo[this.caretSelectPositionInternal].index + this.m_TextComponent.textInfo.characterInfo[this.caretSelectPositionInternal].stringLength;
			}
			if (shift)
			{
				this.stringSelectPositionInternal = num;
				this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
				return;
			}
			this.stringSelectPositionInternal = (this.stringPositionInternal = num);
			if (this.stringPositionInternal >= this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal].index + this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal].stringLength)
			{
				this.caretSelectPositionInternal = (this.caretPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal));
			}
		}

		private int FindPrevWordBegin()
		{
			if (this.stringSelectPositionInternal - 2 < 0)
			{
				return 0;
			}
			int num = this.text.LastIndexOfAny(TMP_InputField.kSeparators, this.stringSelectPositionInternal - 2);
			if (num == -1)
			{
				num = 0;
			}
			else
			{
				num++;
			}
			return num;
		}

		private void MoveLeft(bool shift, bool ctrl)
		{
			if (this.hasSelection && !shift)
			{
				this.stringPositionInternal = (this.stringSelectPositionInternal = Mathf.Min(this.stringPositionInternal, this.stringSelectPositionInternal));
				this.caretPositionInternal = (this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal));
				return;
			}
			int num;
			if (ctrl)
			{
				num = this.FindPrevWordBegin();
			}
			else if (this.m_isRichTextEditingAllowed)
			{
				if (this.stringSelectPositionInternal > 0 && char.IsLowSurrogate(this.text[this.stringSelectPositionInternal - 1]))
				{
					num = this.stringSelectPositionInternal - 2;
				}
				else
				{
					num = this.stringSelectPositionInternal - 1;
				}
			}
			else
			{
				num = ((this.caretSelectPositionInternal < 1) ? this.m_TextComponent.textInfo.characterInfo[0].index : this.m_TextComponent.textInfo.characterInfo[this.caretSelectPositionInternal - 1].index);
				if (num > 0 && this.m_TextComponent.textInfo.characterInfo[this.caretSelectPositionInternal - 1].character == '\n' && this.m_TextComponent.textInfo.characterInfo[this.caretSelectPositionInternal - 2].character == '\r')
				{
					num = this.m_TextComponent.textInfo.characterInfo[this.caretSelectPositionInternal - 2].index;
				}
			}
			if (shift)
			{
				this.stringSelectPositionInternal = num;
				this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
				return;
			}
			this.stringSelectPositionInternal = (this.stringPositionInternal = num);
			if (this.caretPositionInternal > 0 && this.stringPositionInternal <= this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal - 1].index)
			{
				this.caretSelectPositionInternal = (this.caretPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal));
			}
		}

		private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
		{
			if (originalPos >= this.m_TextComponent.textInfo.characterCount)
			{
				originalPos--;
			}
			TMP_CharacterInfo tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[originalPos];
			int lineNumber = tmp_CharacterInfo.lineNumber;
			if (lineNumber - 1 < 0)
			{
				if (!goToFirstChar)
				{
					return originalPos;
				}
				return 0;
			}
			else
			{
				int num = this.m_TextComponent.textInfo.lineInfo[lineNumber].firstCharacterIndex - 1;
				int num2 = -1;
				float num3 = 32767f;
				float num4 = 0f;
				int i = this.m_TextComponent.textInfo.lineInfo[lineNumber - 1].firstCharacterIndex;
				while (i < num)
				{
					TMP_CharacterInfo tmp_CharacterInfo2 = this.m_TextComponent.textInfo.characterInfo[i];
					float num5 = tmp_CharacterInfo.origin - tmp_CharacterInfo2.origin;
					float num6 = num5 / (tmp_CharacterInfo2.xAdvance - tmp_CharacterInfo2.origin);
					if (num6 >= 0f && num6 <= 1f)
					{
						if (num6 < 0.5f)
						{
							return i;
						}
						return i + 1;
					}
					else
					{
						num5 = Mathf.Abs(num5);
						if (num5 < num3)
						{
							num2 = i;
							num3 = num5;
							num4 = num6;
						}
						i++;
					}
				}
				if (num2 == -1)
				{
					return num;
				}
				if (num4 < 0.5f)
				{
					return num2;
				}
				return num2 + 1;
			}
		}

		private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
		{
			if (originalPos >= this.m_TextComponent.textInfo.characterCount)
			{
				return this.m_TextComponent.textInfo.characterCount - 1;
			}
			TMP_CharacterInfo tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[originalPos];
			int lineNumber = tmp_CharacterInfo.lineNumber;
			if (lineNumber + 1 >= this.m_TextComponent.textInfo.lineCount)
			{
				if (!goToLastChar)
				{
					return originalPos;
				}
				return this.m_TextComponent.textInfo.characterCount - 1;
			}
			else
			{
				int lastCharacterIndex = this.m_TextComponent.textInfo.lineInfo[lineNumber + 1].lastCharacterIndex;
				int num = -1;
				float num2 = 32767f;
				float num3 = 0f;
				int i = this.m_TextComponent.textInfo.lineInfo[lineNumber + 1].firstCharacterIndex;
				while (i < lastCharacterIndex)
				{
					TMP_CharacterInfo tmp_CharacterInfo2 = this.m_TextComponent.textInfo.characterInfo[i];
					float num4 = tmp_CharacterInfo.origin - tmp_CharacterInfo2.origin;
					float num5 = num4 / (tmp_CharacterInfo2.xAdvance - tmp_CharacterInfo2.origin);
					if (num5 >= 0f && num5 <= 1f)
					{
						if (num5 < 0.5f)
						{
							return i;
						}
						return i + 1;
					}
					else
					{
						num4 = Mathf.Abs(num4);
						if (num4 < num2)
						{
							num = i;
							num2 = num4;
							num3 = num5;
						}
						i++;
					}
				}
				if (num == -1)
				{
					return lastCharacterIndex;
				}
				if (num3 < 0.5f)
				{
					return num;
				}
				return num + 1;
			}
		}

		private int PageUpCharacterPosition(int originalPos, bool goToFirstChar)
		{
			if (originalPos >= this.m_TextComponent.textInfo.characterCount)
			{
				originalPos--;
			}
			TMP_CharacterInfo tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[originalPos];
			int lineNumber = tmp_CharacterInfo.lineNumber;
			if (lineNumber - 1 < 0)
			{
				if (!goToFirstChar)
				{
					return originalPos;
				}
				return 0;
			}
			else
			{
				float height = this.m_TextViewport.rect.height;
				int num = lineNumber - 1;
				while (num > 0 && this.m_TextComponent.textInfo.lineInfo[num].baseline <= this.m_TextComponent.textInfo.lineInfo[lineNumber].baseline + height)
				{
					num--;
				}
				int lastCharacterIndex = this.m_TextComponent.textInfo.lineInfo[num].lastCharacterIndex;
				int num2 = -1;
				float num3 = 32767f;
				float num4 = 0f;
				int i = this.m_TextComponent.textInfo.lineInfo[num].firstCharacterIndex;
				while (i < lastCharacterIndex)
				{
					TMP_CharacterInfo tmp_CharacterInfo2 = this.m_TextComponent.textInfo.characterInfo[i];
					float num5 = tmp_CharacterInfo.origin - tmp_CharacterInfo2.origin;
					float num6 = num5 / (tmp_CharacterInfo2.xAdvance - tmp_CharacterInfo2.origin);
					if (num6 >= 0f && num6 <= 1f)
					{
						if (num6 < 0.5f)
						{
							return i;
						}
						return i + 1;
					}
					else
					{
						num5 = Mathf.Abs(num5);
						if (num5 < num3)
						{
							num2 = i;
							num3 = num5;
							num4 = num6;
						}
						i++;
					}
				}
				if (num2 == -1)
				{
					return lastCharacterIndex;
				}
				if (num4 < 0.5f)
				{
					return num2;
				}
				return num2 + 1;
			}
		}

		private int PageDownCharacterPosition(int originalPos, bool goToLastChar)
		{
			if (originalPos >= this.m_TextComponent.textInfo.characterCount)
			{
				return this.m_TextComponent.textInfo.characterCount - 1;
			}
			TMP_CharacterInfo tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[originalPos];
			int lineNumber = tmp_CharacterInfo.lineNumber;
			if (lineNumber + 1 >= this.m_TextComponent.textInfo.lineCount)
			{
				if (!goToLastChar)
				{
					return originalPos;
				}
				return this.m_TextComponent.textInfo.characterCount - 1;
			}
			else
			{
				float height = this.m_TextViewport.rect.height;
				int num = lineNumber + 1;
				while (num < this.m_TextComponent.textInfo.lineCount - 1 && this.m_TextComponent.textInfo.lineInfo[num].baseline >= this.m_TextComponent.textInfo.lineInfo[lineNumber].baseline - height)
				{
					num++;
				}
				int lastCharacterIndex = this.m_TextComponent.textInfo.lineInfo[num].lastCharacterIndex;
				int num2 = -1;
				float num3 = 32767f;
				float num4 = 0f;
				int i = this.m_TextComponent.textInfo.lineInfo[num].firstCharacterIndex;
				while (i < lastCharacterIndex)
				{
					TMP_CharacterInfo tmp_CharacterInfo2 = this.m_TextComponent.textInfo.characterInfo[i];
					float num5 = tmp_CharacterInfo.origin - tmp_CharacterInfo2.origin;
					float num6 = num5 / (tmp_CharacterInfo2.xAdvance - tmp_CharacterInfo2.origin);
					if (num6 >= 0f && num6 <= 1f)
					{
						if (num6 < 0.5f)
						{
							return i;
						}
						return i + 1;
					}
					else
					{
						num5 = Mathf.Abs(num5);
						if (num5 < num3)
						{
							num2 = i;
							num3 = num5;
							num4 = num6;
						}
						i++;
					}
				}
				if (num2 == -1)
				{
					return lastCharacterIndex;
				}
				if (num4 < 0.5f)
				{
					return num2;
				}
				return num2 + 1;
			}
		}

		private void MoveDown(bool shift)
		{
			this.MoveDown(shift, true);
		}

		private void MoveDown(bool shift, bool goToLastChar)
		{
			if (this.hasSelection && !shift)
			{
				this.caretPositionInternal = (this.caretSelectPositionInternal = Mathf.Max(this.caretPositionInternal, this.caretSelectPositionInternal));
			}
			int num = (this.multiLine ? this.LineDownCharacterPosition(this.caretSelectPositionInternal, goToLastChar) : (this.m_TextComponent.textInfo.characterCount - 1));
			if (shift)
			{
				this.caretSelectPositionInternal = num;
				this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal);
				return;
			}
			this.caretSelectPositionInternal = (this.caretPositionInternal = num);
			this.stringSelectPositionInternal = (this.stringPositionInternal = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal));
		}

		private void MoveUp(bool shift)
		{
			this.MoveUp(shift, true);
		}

		private void MoveUp(bool shift, bool goToFirstChar)
		{
			if (this.hasSelection && !shift)
			{
				this.caretPositionInternal = (this.caretSelectPositionInternal = Mathf.Min(this.caretPositionInternal, this.caretSelectPositionInternal));
			}
			int num = (this.multiLine ? this.LineUpCharacterPosition(this.caretSelectPositionInternal, goToFirstChar) : 0);
			if (shift)
			{
				this.caretSelectPositionInternal = num;
				this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal);
				return;
			}
			this.caretSelectPositionInternal = (this.caretPositionInternal = num);
			this.stringSelectPositionInternal = (this.stringPositionInternal = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal));
		}

		private void MovePageUp(bool shift)
		{
			this.MovePageUp(shift, true);
		}

		private void MovePageUp(bool shift, bool goToFirstChar)
		{
			if (this.hasSelection && !shift)
			{
				this.caretPositionInternal = (this.caretSelectPositionInternal = Mathf.Min(this.caretPositionInternal, this.caretSelectPositionInternal));
			}
			int num = (this.multiLine ? this.PageUpCharacterPosition(this.caretSelectPositionInternal, goToFirstChar) : 0);
			if (shift)
			{
				this.caretSelectPositionInternal = num;
				this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal);
			}
			else
			{
				this.caretSelectPositionInternal = (this.caretPositionInternal = num);
				this.stringSelectPositionInternal = (this.stringPositionInternal = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal));
			}
			if (this.m_LineType != TMP_InputField.LineType.SingleLine)
			{
				float num2 = this.m_TextViewport.rect.height;
				float num3 = this.m_TextComponent.rectTransform.position.y + this.m_TextComponent.textBounds.max.y;
				float num4 = this.m_TextViewport.position.y + this.m_TextViewport.rect.yMax;
				num2 = ((num4 > num3 + num2) ? num2 : (num4 - num3));
				this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(0f, num2);
				this.AssignPositioningIfNeeded();
			}
		}

		private void MovePageDown(bool shift)
		{
			this.MovePageDown(shift, true);
		}

		private void MovePageDown(bool shift, bool goToLastChar)
		{
			if (this.hasSelection && !shift)
			{
				this.caretPositionInternal = (this.caretSelectPositionInternal = Mathf.Max(this.caretPositionInternal, this.caretSelectPositionInternal));
			}
			int num = (this.multiLine ? this.PageDownCharacterPosition(this.caretSelectPositionInternal, goToLastChar) : (this.m_TextComponent.textInfo.characterCount - 1));
			if (shift)
			{
				this.caretSelectPositionInternal = num;
				this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal);
			}
			else
			{
				this.caretSelectPositionInternal = (this.caretPositionInternal = num);
				this.stringSelectPositionInternal = (this.stringPositionInternal = this.GetStringIndexFromCaretPosition(this.caretSelectPositionInternal));
			}
			if (this.m_LineType != TMP_InputField.LineType.SingleLine)
			{
				float num2 = this.m_TextViewport.rect.height;
				float num3 = this.m_TextComponent.rectTransform.position.y + this.m_TextComponent.textBounds.min.y;
				float num4 = this.m_TextViewport.position.y + this.m_TextViewport.rect.yMin;
				num2 = ((num4 > num3 + num2) ? num2 : (num4 - num3));
				this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(0f, num2);
				this.AssignPositioningIfNeeded();
			}
		}

		private void Delete()
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (this.m_StringPosition == this.m_StringSelectPosition)
			{
				return;
			}
			if (this.m_isRichTextEditingAllowed || this.m_isSelectAll)
			{
				if (this.m_StringPosition < this.m_StringSelectPosition)
				{
					this.m_Text = this.text.Remove(this.m_StringPosition, this.m_StringSelectPosition - this.m_StringPosition);
					this.m_StringSelectPosition = this.m_StringPosition;
				}
				else
				{
					this.m_Text = this.text.Remove(this.m_StringSelectPosition, this.m_StringPosition - this.m_StringSelectPosition);
					this.m_StringPosition = this.m_StringSelectPosition;
				}
				if (this.m_isSelectAll)
				{
					this.m_CaretPosition = (this.m_CaretSelectPosition = 0);
					this.m_isSelectAll = false;
					return;
				}
			}
			else
			{
				if (this.m_CaretPosition < this.m_CaretSelectPosition)
				{
					int num = this.ClampArrayIndex(this.m_CaretSelectPosition - 1);
					this.m_StringPosition = this.m_TextComponent.textInfo.characterInfo[this.m_CaretPosition].index;
					this.m_StringSelectPosition = this.m_TextComponent.textInfo.characterInfo[num].index + this.m_TextComponent.textInfo.characterInfo[num].stringLength;
					this.m_Text = this.text.Remove(this.m_StringPosition, this.m_StringSelectPosition - this.m_StringPosition);
					this.m_StringSelectPosition = this.m_StringPosition;
					this.m_CaretSelectPosition = this.m_CaretPosition;
					return;
				}
				int num2 = this.ClampArrayIndex(this.m_CaretPosition - 1);
				this.m_StringPosition = this.m_TextComponent.textInfo.characterInfo[num2].index + this.m_TextComponent.textInfo.characterInfo[num2].stringLength;
				this.m_StringSelectPosition = this.m_TextComponent.textInfo.characterInfo[this.m_CaretSelectPosition].index;
				this.m_Text = this.text.Remove(this.m_StringSelectPosition, this.m_StringPosition - this.m_StringSelectPosition);
				this.m_StringPosition = this.m_StringSelectPosition;
				this.m_CaretPosition = this.m_CaretSelectPosition;
			}
		}

		private void DeleteKey()
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (this.hasSelection)
			{
				this.m_HasTextBeenRemoved = true;
				this.Delete();
				this.UpdateTouchKeyboardFromEditChanges();
				this.SendOnValueChangedAndUpdateLabel();
				return;
			}
			if (this.m_isRichTextEditingAllowed)
			{
				if (this.stringPositionInternal < this.text.Length)
				{
					if (char.IsHighSurrogate(this.text[this.stringPositionInternal]))
					{
						this.m_Text = this.text.Remove(this.stringPositionInternal, 2);
					}
					else
					{
						this.m_Text = this.text.Remove(this.stringPositionInternal, 1);
					}
					this.m_HasTextBeenRemoved = true;
					this.UpdateTouchKeyboardFromEditChanges();
					this.SendOnValueChangedAndUpdateLabel();
					return;
				}
			}
			else if (this.caretPositionInternal < this.m_TextComponent.textInfo.characterCount - 1)
			{
				int num = this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal].stringLength;
				if (this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal].character == '\r' && this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal + 1].character == '\n')
				{
					num += this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal + 1].stringLength;
				}
				int index = this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal].index;
				this.m_Text = this.text.Remove(index, num);
				this.m_HasTextBeenRemoved = true;
				this.SendOnValueChangedAndUpdateLabel();
			}
		}

		private void Backspace()
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (this.hasSelection)
			{
				this.m_HasTextBeenRemoved = true;
				this.Delete();
				this.UpdateTouchKeyboardFromEditChanges();
				this.SendOnValueChangedAndUpdateLabel();
				return;
			}
			if (this.m_isRichTextEditingAllowed)
			{
				if (this.stringPositionInternal > 0 && this.stringPositionInternal <= this.text.Length)
				{
					int num = 1;
					if (char.IsLowSurrogate(this.text[this.stringPositionInternal - 1]))
					{
						num = 2;
					}
					this.stringSelectPositionInternal = (this.stringPositionInternal -= num);
					this.m_Text = this.text.Remove(this.stringPositionInternal, num);
					this.caretSelectPositionInternal = --this.caretPositionInternal;
					this.m_HasTextBeenRemoved = true;
					this.UpdateTouchKeyboardFromEditChanges();
					this.SendOnValueChangedAndUpdateLabel();
					return;
				}
			}
			else
			{
				if (this.caretPositionInternal > 0)
				{
					int num2 = this.caretPositionInternal - 1;
					int num3 = this.m_TextComponent.textInfo.characterInfo[num2].stringLength;
					if (num2 > 0 && this.m_TextComponent.textInfo.characterInfo[num2].character == '\n' && this.m_TextComponent.textInfo.characterInfo[num2 - 1].character == '\r')
					{
						num3 += this.m_TextComponent.textInfo.characterInfo[num2 - 1].stringLength;
						num2--;
					}
					this.m_Text = this.text.Remove(this.m_TextComponent.textInfo.characterInfo[num2].index, num3);
					this.stringSelectPositionInternal = (this.stringPositionInternal = ((this.caretPositionInternal < 1) ? this.m_TextComponent.textInfo.characterInfo[0].index : this.m_TextComponent.textInfo.characterInfo[num2].index));
					this.caretSelectPositionInternal = (this.caretPositionInternal = num2);
				}
				this.m_HasTextBeenRemoved = true;
				this.UpdateTouchKeyboardFromEditChanges();
				this.SendOnValueChangedAndUpdateLabel();
			}
		}

		protected virtual void Append(string input)
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (!this.InPlaceEditing())
			{
				return;
			}
			int i = 0;
			int length = input.Length;
			while (i < length)
			{
				char c = input[i];
				if (c >= ' ' || c == '\t' || c == '\r' || c == '\n')
				{
					this.Append(c);
				}
				i++;
			}
		}

		protected virtual void Append(char input)
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (!this.InPlaceEditing())
			{
				return;
			}
			int num = Mathf.Min(this.stringPositionInternal, this.stringSelectPositionInternal);
			string text = this.text;
			if (this.selectionFocusPosition != this.selectionAnchorPosition)
			{
				this.m_HasTextBeenRemoved = true;
				if (this.m_isRichTextEditingAllowed || this.m_isSelectAll)
				{
					if (this.m_StringPosition < this.m_StringSelectPosition)
					{
						text = this.text.Remove(this.m_StringPosition, this.m_StringSelectPosition - this.m_StringPosition);
					}
					else
					{
						text = this.text.Remove(this.m_StringSelectPosition, this.m_StringPosition - this.m_StringSelectPosition);
					}
				}
				else if (this.m_CaretPosition < this.m_CaretSelectPosition)
				{
					this.m_StringPosition = this.m_TextComponent.textInfo.characterInfo[this.m_CaretPosition].index;
					this.m_StringSelectPosition = this.m_TextComponent.textInfo.characterInfo[this.m_CaretSelectPosition - 1].index + this.m_TextComponent.textInfo.characterInfo[this.m_CaretSelectPosition - 1].stringLength;
					text = this.text.Remove(this.m_StringPosition, this.m_StringSelectPosition - this.m_StringPosition);
				}
				else
				{
					this.m_StringPosition = this.m_TextComponent.textInfo.characterInfo[this.m_CaretPosition - 1].index + this.m_TextComponent.textInfo.characterInfo[this.m_CaretPosition - 1].stringLength;
					this.m_StringSelectPosition = this.m_TextComponent.textInfo.characterInfo[this.m_CaretSelectPosition].index;
					text = this.text.Remove(this.m_StringSelectPosition, this.m_StringPosition - this.m_StringSelectPosition);
				}
			}
			if (this.onValidateInput != null)
			{
				input = this.onValidateInput(text, num, input);
			}
			else if (this.characterValidation == TMP_InputField.CharacterValidation.CustomValidator)
			{
				input = this.Validate(text, num, input);
				if (input == '\0')
				{
					return;
				}
				if (!char.IsHighSurrogate(input))
				{
					this.m_CaretSelectPosition = ++this.m_CaretPosition;
				}
				this.SendOnValueChanged();
				this.UpdateLabel();
				return;
			}
			else if (this.characterValidation != TMP_InputField.CharacterValidation.None)
			{
				input = this.Validate(text, num, input);
			}
			if (input == '\0')
			{
				return;
			}
			this.Insert(input);
		}

		private void Insert(char c)
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			string text = c.ToString();
			this.Delete();
			if (this.characterLimit > 0 && this.text.Length >= this.characterLimit)
			{
				return;
			}
			this.m_Text = this.text.Insert(this.m_StringPosition, text);
			if (!char.IsHighSurrogate(c))
			{
				this.m_CaretSelectPosition = ++this.m_CaretPosition;
			}
			this.m_StringSelectPosition = ++this.m_StringPosition;
			this.UpdateTouchKeyboardFromEditChanges();
			this.SendOnValueChanged();
		}

		private void UpdateTouchKeyboardFromEditChanges()
		{
			if (this.m_SoftKeyboard != null && this.InPlaceEditing())
			{
				this.m_SoftKeyboard.text = this.m_Text;
			}
		}

		private void SendOnValueChangedAndUpdateLabel()
		{
			this.UpdateLabel();
			this.SendOnValueChanged();
		}

		private void SendOnValueChanged()
		{
			if (this.onValueChanged != null)
			{
				this.onValueChanged.Invoke(this.text);
			}
		}

		protected void SendOnEndEdit()
		{
			if (this.onEndEdit != null)
			{
				this.onEndEdit.Invoke(this.m_Text);
			}
		}

		protected void SendOnSubmit()
		{
			if (this.onSubmit != null)
			{
				this.onSubmit.Invoke(this.m_Text);
			}
		}

		protected void SendOnFocus()
		{
			if (this.onSelect != null)
			{
				this.onSelect.Invoke(this.m_Text);
			}
		}

		protected void SendOnFocusLost()
		{
			if (this.onDeselect != null)
			{
				this.onDeselect.Invoke(this.m_Text);
			}
		}

		protected void SendOnTextSelection()
		{
			this.m_isSelected = true;
			if (this.onTextSelection != null)
			{
				this.onTextSelection.Invoke(this.m_Text, this.stringPositionInternal, this.stringSelectPositionInternal);
			}
		}

		protected void SendOnEndTextSelection()
		{
			if (!this.m_isSelected)
			{
				return;
			}
			if (this.onEndTextSelection != null)
			{
				this.onEndTextSelection.Invoke(this.m_Text, this.stringPositionInternal, this.stringSelectPositionInternal);
			}
			this.m_isSelected = false;
		}

		protected void SendTouchScreenKeyboardStatusChanged()
		{
			if (this.m_SoftKeyboard != null && this.onTouchScreenKeyboardStatusChanged != null)
			{
				this.onTouchScreenKeyboardStatusChanged.Invoke(this.m_SoftKeyboard.status);
			}
		}

		protected void UpdateLabel()
		{
			if (this.m_TextComponent != null && this.m_TextComponent.font != null && !this.m_PreventCallback)
			{
				this.m_PreventCallback = true;
				string text;
				if (this.compositionLength > 0 && !this.m_ReadOnly)
				{
					this.Delete();
					if (this.m_RichText)
					{
						text = string.Concat(new string[]
						{
							this.text.Substring(0, this.m_StringPosition),
							"<u>",
							this.compositionString,
							"</u>",
							this.text.Substring(this.m_StringPosition)
						});
					}
					else
					{
						text = this.text.Substring(0, this.m_StringPosition) + this.compositionString + this.text.Substring(this.m_StringPosition);
					}
					this.m_IsCompositionActive = true;
				}
				else
				{
					text = this.text;
					this.m_IsCompositionActive = false;
					this.m_ShouldUpdateIMEWindowPosition = true;
				}
				string text2;
				if (this.inputType == TMP_InputField.InputType.Password)
				{
					text2 = new string(this.asteriskChar, text.Length);
				}
				else
				{
					text2 = text;
				}
				bool flag = string.IsNullOrEmpty(text);
				if (this.m_Placeholder != null)
				{
					this.m_Placeholder.enabled = flag;
				}
				if (!flag && !this.m_ReadOnly)
				{
					this.SetCaretVisible();
				}
				this.m_TextComponent.text = text2 + "\u200b";
				if (this.m_IsDrivenByLayoutComponents)
				{
					LayoutRebuilder.MarkLayoutForRebuild(this.m_RectTransform);
				}
				if (this.m_LineLimit > 0)
				{
					this.m_TextComponent.ForceMeshUpdate(false, false);
					TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
					if (textInfo != null && textInfo.lineCount > this.m_LineLimit)
					{
						int lastCharacterIndex = textInfo.lineInfo[this.m_LineLimit - 1].lastCharacterIndex;
						int num = textInfo.characterInfo[lastCharacterIndex].index + textInfo.characterInfo[lastCharacterIndex].stringLength;
						this.text = text2.Remove(num, text2.Length - num);
						this.m_TextComponent.text = this.text + "\u200b";
					}
				}
				if (this.m_IsTextComponentUpdateRequired || (this.m_VerticalScrollbar && (!this.m_IsCaretPositionDirty || !this.m_IsStringPositionDirty)))
				{
					this.m_IsTextComponentUpdateRequired = false;
					this.m_TextComponent.ForceMeshUpdate(false, false);
				}
				this.MarkGeometryAsDirty();
				this.m_PreventCallback = false;
			}
		}

		private void UpdateScrollbar()
		{
			if (this.m_VerticalScrollbar)
			{
				float num = this.m_TextViewport.rect.height / this.m_TextComponent.preferredHeight;
				this.m_VerticalScrollbar.size = num;
				this.m_VerticalScrollbar.value = this.GetScrollPositionRelativeToViewport();
			}
		}

		private void OnScrollbarValueChange(float value)
		{
			if (value < 0f || value > 1f)
			{
				return;
			}
			this.AdjustTextPositionRelativeToViewport(value);
			this.m_ScrollPosition = value;
		}

		private void UpdateMaskRegions()
		{
		}

		private void AdjustTextPositionRelativeToViewport(float relativePosition)
		{
			if (this.m_TextViewport == null)
			{
				return;
			}
			TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
			if (textInfo == null || textInfo.lineInfo == null || textInfo.lineCount == 0 || textInfo.lineCount > textInfo.lineInfo.Length)
			{
				return;
			}
			float num = 0f;
			float num2 = this.m_TextComponent.preferredHeight;
			VerticalAlignmentOptions verticalAlignment = this.m_TextComponent.verticalAlignment;
			if (verticalAlignment <= VerticalAlignmentOptions.Bottom)
			{
				if (verticalAlignment != VerticalAlignmentOptions.Top)
				{
					if (verticalAlignment != VerticalAlignmentOptions.Middle)
					{
						if (verticalAlignment == VerticalAlignmentOptions.Bottom)
						{
							num = 1f;
						}
					}
					else
					{
						num = 0.5f;
					}
				}
				else
				{
					num = 0f;
				}
			}
			else if (verticalAlignment != VerticalAlignmentOptions.Baseline)
			{
				if (verticalAlignment != VerticalAlignmentOptions.Geometry)
				{
					if (verticalAlignment == VerticalAlignmentOptions.Capline)
					{
						num = 0.5f;
					}
				}
				else
				{
					num = 0.5f;
					num2 = this.m_TextComponent.bounds.size.y;
				}
			}
			this.m_TextComponent.rectTransform.anchoredPosition = new Vector2(this.m_TextComponent.rectTransform.anchoredPosition.x, (num2 - this.m_TextViewport.rect.height) * (relativePosition - num));
			this.AssignPositioningIfNeeded();
		}

		private int GetCaretPositionFromStringIndex(int stringIndex)
		{
			int characterCount = this.m_TextComponent.textInfo.characterCount;
			for (int i = 0; i < characterCount; i++)
			{
				if (this.m_TextComponent.textInfo.characterInfo[i].index >= stringIndex)
				{
					return i;
				}
			}
			return characterCount;
		}

		private int GetMinCaretPositionFromStringIndex(int stringIndex)
		{
			int characterCount = this.m_TextComponent.textInfo.characterCount;
			for (int i = 0; i < characterCount; i++)
			{
				if (stringIndex < this.m_TextComponent.textInfo.characterInfo[i].index + this.m_TextComponent.textInfo.characterInfo[i].stringLength)
				{
					return i;
				}
			}
			return characterCount;
		}

		private int GetMaxCaretPositionFromStringIndex(int stringIndex)
		{
			int characterCount = this.m_TextComponent.textInfo.characterCount;
			for (int i = 0; i < characterCount; i++)
			{
				if (this.m_TextComponent.textInfo.characterInfo[i].index >= stringIndex)
				{
					return i;
				}
			}
			return characterCount;
		}

		private int GetStringIndexFromCaretPosition(int caretPosition)
		{
			this.ClampCaretPos(ref caretPosition);
			return this.m_TextComponent.textInfo.characterInfo[caretPosition].index;
		}

		private void UpdateStringIndexFromCaretPosition()
		{
			this.stringPositionInternal = this.GetStringIndexFromCaretPosition(this.m_CaretPosition);
			this.stringSelectPositionInternal = this.GetStringIndexFromCaretPosition(this.m_CaretSelectPosition);
			this.m_IsStringPositionDirty = false;
		}

		private void UpdateCaretPositionFromStringIndex()
		{
			this.caretPositionInternal = this.GetCaretPositionFromStringIndex(this.stringPositionInternal);
			this.caretSelectPositionInternal = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
			this.m_IsCaretPositionDirty = false;
		}

		public void ForceLabelUpdate()
		{
			this.UpdateLabel();
		}

		private void MarkGeometryAsDirty()
		{
			CanvasUpdateRegistry.RegisterCanvasElementForGraphicRebuild(this);
		}

		public virtual void Rebuild(CanvasUpdate update)
		{
			if (update == CanvasUpdate.LatePreRender)
			{
				this.UpdateGeometry();
			}
		}

		public virtual void LayoutComplete()
		{
		}

		public virtual void GraphicUpdateComplete()
		{
		}

		private void UpdateGeometry()
		{
			if (!this.InPlaceEditing() && !this.isUWP())
			{
				return;
			}
			if (this.m_CachedInputRenderer == null)
			{
				return;
			}
			this.OnFillVBO(this.mesh);
			this.m_CachedInputRenderer.SetMesh(this.mesh);
		}

		private void AssignPositioningIfNeeded()
		{
			if (this.m_TextComponent != null && this.caretRectTrans != null && (this.caretRectTrans.localPosition != this.m_TextComponent.rectTransform.localPosition || this.caretRectTrans.localRotation != this.m_TextComponent.rectTransform.localRotation || this.caretRectTrans.localScale != this.m_TextComponent.rectTransform.localScale || this.caretRectTrans.anchorMin != this.m_TextComponent.rectTransform.anchorMin || this.caretRectTrans.anchorMax != this.m_TextComponent.rectTransform.anchorMax || this.caretRectTrans.anchoredPosition != this.m_TextComponent.rectTransform.anchoredPosition || this.caretRectTrans.sizeDelta != this.m_TextComponent.rectTransform.sizeDelta || this.caretRectTrans.pivot != this.m_TextComponent.rectTransform.pivot))
			{
				this.caretRectTrans.localPosition = this.m_TextComponent.rectTransform.localPosition;
				this.caretRectTrans.localRotation = this.m_TextComponent.rectTransform.localRotation;
				this.caretRectTrans.localScale = this.m_TextComponent.rectTransform.localScale;
				this.caretRectTrans.anchorMin = this.m_TextComponent.rectTransform.anchorMin;
				this.caretRectTrans.anchorMax = this.m_TextComponent.rectTransform.anchorMax;
				this.caretRectTrans.anchoredPosition = this.m_TextComponent.rectTransform.anchoredPosition;
				this.caretRectTrans.sizeDelta = this.m_TextComponent.rectTransform.sizeDelta;
				this.caretRectTrans.pivot = this.m_TextComponent.rectTransform.pivot;
			}
		}

		private void OnFillVBO(Mesh vbo)
		{
			using (VertexHelper vertexHelper = new VertexHelper())
			{
				if (!this.isFocused && !this.m_SelectionStillActive)
				{
					vertexHelper.FillMesh(vbo);
				}
				else
				{
					if (this.m_IsStringPositionDirty)
					{
						this.UpdateStringIndexFromCaretPosition();
					}
					if (this.m_IsCaretPositionDirty)
					{
						this.UpdateCaretPositionFromStringIndex();
					}
					if (!this.hasSelection)
					{
						this.GenerateCaret(vertexHelper, Vector2.zero);
						this.SendOnEndTextSelection();
					}
					else
					{
						this.GenerateHighlight(vertexHelper, Vector2.zero);
						this.SendOnTextSelection();
					}
					vertexHelper.FillMesh(vbo);
				}
			}
		}

		private void GenerateCaret(VertexHelper vbo, Vector2 roundingOffset)
		{
			if (!this.m_CaretVisible || this.m_TextComponent.canvas == null || this.m_ReadOnly)
			{
				return;
			}
			if (this.m_CursorVerts == null)
			{
				this.CreateCursorVerts();
			}
			Vector2 zero = Vector2.zero;
			if (this.caretPositionInternal >= this.m_TextComponent.textInfo.characterInfo.Length || this.caretPositionInternal < 0)
			{
				return;
			}
			int lineNumber = this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal].lineNumber;
			TMP_CharacterInfo tmp_CharacterInfo;
			float num;
			if (this.caretPositionInternal == this.m_TextComponent.textInfo.lineInfo[lineNumber].firstCharacterIndex)
			{
				tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal];
				num = tmp_CharacterInfo.ascender - tmp_CharacterInfo.descender;
				if (this.m_TextComponent.verticalAlignment == VerticalAlignmentOptions.Geometry)
				{
					zero = new Vector2(tmp_CharacterInfo.origin, 0f - num / 2f);
				}
				else
				{
					zero = new Vector2(tmp_CharacterInfo.origin, tmp_CharacterInfo.descender);
				}
			}
			else
			{
				tmp_CharacterInfo = this.m_TextComponent.textInfo.characterInfo[this.caretPositionInternal - 1];
				num = tmp_CharacterInfo.ascender - tmp_CharacterInfo.descender;
				if (this.m_TextComponent.verticalAlignment == VerticalAlignmentOptions.Geometry)
				{
					zero = new Vector2(tmp_CharacterInfo.xAdvance, 0f - num / 2f);
				}
				else
				{
					zero = new Vector2(tmp_CharacterInfo.xAdvance, tmp_CharacterInfo.descender);
				}
			}
			if (this.m_SoftKeyboard != null && this.compositionLength == 0)
			{
				int num2 = this.m_StringPosition;
				int num3 = ((this.m_SoftKeyboard.text == null) ? 0 : this.m_SoftKeyboard.text.Length);
				if (num2 < 0)
				{
					num2 = 0;
				}
				if (num2 > num3)
				{
					num2 = num3;
				}
				this.m_SoftKeyboard.selection = new RangeInt(num2, 0);
			}
			if ((this.isFocused && zero != this.m_LastPosition) || this.m_forceRectTransformAdjustment || this.m_HasTextBeenRemoved)
			{
				this.AdjustRectTransformRelativeToViewport(zero, num, tmp_CharacterInfo.isVisible);
			}
			this.m_LastPosition = zero;
			float num4 = zero.y + num;
			float num5 = num4 - num;
			TMP_FontAsset font = this.m_TextComponent.font;
			float num6 = this.m_TextComponent.fontSize / font.m_FaceInfo.pointSize * font.m_FaceInfo.scale;
			float num7 = (float)this.m_CaretWidth * font.faceInfo.lineHeight * num6 * 0.05f;
			num7 = Mathf.Max(num7, 1f);
			this.m_CursorVerts[0].position = new Vector3(zero.x, num5, 0f);
			this.m_CursorVerts[1].position = new Vector3(zero.x, num4, 0f);
			this.m_CursorVerts[2].position = new Vector3(zero.x + num7, num4, 0f);
			this.m_CursorVerts[3].position = new Vector3(zero.x + num7, num5, 0f);
			this.m_CursorVerts[0].color = this.caretColor;
			this.m_CursorVerts[1].color = this.caretColor;
			this.m_CursorVerts[2].color = this.caretColor;
			this.m_CursorVerts[3].color = this.caretColor;
			vbo.AddUIVertexQuad(this.m_CursorVerts);
			if (this.m_ShouldUpdateIMEWindowPosition || lineNumber != this.m_PreviousIMEInsertionLine)
			{
				this.m_ShouldUpdateIMEWindowPosition = false;
				this.m_PreviousIMEInsertionLine = lineNumber;
				Camera camera;
				if (this.m_TextComponent.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
				{
					camera = null;
				}
				else
				{
					camera = this.m_TextComponent.canvas.worldCamera;
					if (camera == null)
					{
						camera = Camera.current;
					}
				}
				Vector3 vector = this.m_CachedInputRenderer.gameObject.transform.TransformPoint(this.m_CursorVerts[0].position);
				Vector2 vector2 = RectTransformUtility.WorldToScreenPoint(camera, vector);
				vector2.y = (float)Screen.height - vector2.y;
				IntPtr focus = TMP_InputField.GetFocus();
				if (focus != IntPtr.Zero)
				{
					TMP_InputField.POINT point = default(TMP_InputField.POINT);
					TMP_InputField.RECT rect;
					if (TMP_InputField.GetWindowRect(focus, out rect) && TMP_InputField.ClientToScreen(focus, ref point))
					{
						vector2.x += (float)(point.x - rect.left);
						vector2.y += (float)(point.y - rect.top);
					}
				}
				if (this.inputSystem != null)
				{
					this.inputSystem.compositionCursorPos = vector2;
				}
			}
		}

		private void CreateCursorVerts()
		{
			this.m_CursorVerts = new UIVertex[4];
			for (int i = 0; i < this.m_CursorVerts.Length; i++)
			{
				this.m_CursorVerts[i] = UIVertex.simpleVert;
				this.m_CursorVerts[i].uv0 = Vector2.zero;
			}
		}

		private void GenerateHighlight(VertexHelper vbo, Vector2 roundingOffset)
		{
			this.UpdateMaskRegions();
			TMP_TextInfo textInfo = this.m_TextComponent.textInfo;
			if (textInfo.characterCount == 0)
			{
				return;
			}
			this.m_CaretPosition = this.GetCaretPositionFromStringIndex(this.stringPositionInternal);
			this.m_CaretSelectPosition = this.GetCaretPositionFromStringIndex(this.stringSelectPositionInternal);
			if (this.m_SoftKeyboard != null && this.compositionLength == 0)
			{
				int num = ((this.m_CaretPosition < this.m_CaretSelectPosition) ? textInfo.characterInfo[this.m_CaretPosition].index : textInfo.characterInfo[this.m_CaretSelectPosition].index);
				int num2 = ((this.m_CaretPosition < this.m_CaretSelectPosition) ? (this.stringSelectPositionInternal - num) : (this.stringPositionInternal - num));
				this.m_SoftKeyboard.selection = new RangeInt(num, num2);
			}
			Vector2 vector;
			float num3;
			if (this.m_CaretSelectPosition < textInfo.characterCount)
			{
				vector = new Vector2(textInfo.characterInfo[this.m_CaretSelectPosition].origin, textInfo.characterInfo[this.m_CaretSelectPosition].descender);
				num3 = textInfo.characterInfo[this.m_CaretSelectPosition].ascender - textInfo.characterInfo[this.m_CaretSelectPosition].descender;
			}
			else
			{
				vector = new Vector2(textInfo.characterInfo[this.m_CaretSelectPosition - 1].xAdvance, textInfo.characterInfo[this.m_CaretSelectPosition - 1].descender);
				num3 = textInfo.characterInfo[this.m_CaretSelectPosition - 1].ascender - textInfo.characterInfo[this.m_CaretSelectPosition - 1].descender;
			}
			this.AdjustRectTransformRelativeToViewport(vector, num3, true);
			int num4 = Mathf.Max(0, this.m_CaretPosition);
			int num5 = Mathf.Max(0, this.m_CaretSelectPosition);
			if (num4 > num5)
			{
				int num6 = num4;
				num4 = num5;
				num5 = num6;
			}
			num5--;
			num4 = Math.Min(textInfo.characterInfo.Length - 1, num4);
			int num7 = textInfo.characterInfo[num4].lineNumber;
			int num8 = textInfo.lineInfo[num7].lastCharacterIndex;
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.uv0 = Vector2.zero;
			simpleVert.color = this.selectionColor;
			int num9 = num4;
			while (num9 <= num5 && num9 < textInfo.characterCount)
			{
				if (num9 == num8 || num9 == num5)
				{
					TMP_CharacterInfo tmp_CharacterInfo = textInfo.characterInfo[num4];
					TMP_CharacterInfo tmp_CharacterInfo2 = textInfo.characterInfo[num9];
					if (num9 > 0 && tmp_CharacterInfo2.character == '\n' && textInfo.characterInfo[num9 - 1].character == '\r')
					{
						tmp_CharacterInfo2 = textInfo.characterInfo[num9 - 1];
					}
					Vector2 vector2 = new Vector2(tmp_CharacterInfo.origin, textInfo.lineInfo[num7].ascender);
					Vector2 vector3 = new Vector2(tmp_CharacterInfo2.xAdvance, textInfo.lineInfo[num7].descender);
					int currentVertCount = vbo.currentVertCount;
					simpleVert.position = new Vector3(vector2.x, vector3.y, 0f);
					vbo.AddVert(simpleVert);
					simpleVert.position = new Vector3(vector3.x, vector3.y, 0f);
					vbo.AddVert(simpleVert);
					simpleVert.position = new Vector3(vector3.x, vector2.y, 0f);
					vbo.AddVert(simpleVert);
					simpleVert.position = new Vector3(vector2.x, vector2.y, 0f);
					vbo.AddVert(simpleVert);
					vbo.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
					vbo.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
					num4 = num9 + 1;
					num7++;
					if (num7 < textInfo.lineCount)
					{
						num8 = textInfo.lineInfo[num7].lastCharacterIndex;
					}
				}
				num9++;
			}
		}

		private void AdjustRectTransformRelativeToViewport(Vector2 startPosition, float height, bool isCharVisible)
		{
			if (this.m_TextViewport == null)
			{
				return;
			}
			Vector3 localPosition = base.transform.localPosition;
			Vector3 localPosition2 = this.m_TextComponent.rectTransform.localPosition;
			Vector3 localPosition3 = this.m_TextViewport.localPosition;
			Rect rect = this.m_TextViewport.rect;
			Vector2 vector = new Vector2(startPosition.x + localPosition2.x + localPosition3.x + localPosition.x, startPosition.y + localPosition2.y + localPosition3.y + localPosition.y);
			Rect rect2 = new Rect(localPosition.x + localPosition3.x + rect.x, localPosition.y + localPosition3.y + rect.y, rect.width, rect.height);
			float num = rect2.xMax - (vector.x + this.m_TextComponent.margin.z + (float)this.m_CaretWidth);
			if (num < 0f && (!this.multiLine || (this.multiLine && isCharVisible)))
			{
				this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(num, 0f);
				this.AssignPositioningIfNeeded();
			}
			float num2 = vector.x - this.m_TextComponent.margin.x - rect2.xMin;
			if (num2 < 0f)
			{
				this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(-num2, 0f);
				this.AssignPositioningIfNeeded();
			}
			if (this.m_LineType != TMP_InputField.LineType.SingleLine)
			{
				float num3 = rect2.yMax - (vector.y + height);
				if (num3 < -0.0001f)
				{
					this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(0f, num3);
					this.AssignPositioningIfNeeded();
				}
				float num4 = vector.y - rect2.yMin;
				if (num4 < 0f)
				{
					this.m_TextComponent.rectTransform.anchoredPosition -= new Vector2(0f, num4);
					this.AssignPositioningIfNeeded();
				}
			}
			if (this.m_HasTextBeenRemoved)
			{
				float x = this.m_TextComponent.rectTransform.anchoredPosition.x;
				float num5 = localPosition.x + localPosition3.x + localPosition2.x + this.m_TextComponent.textInfo.characterInfo[0].origin - this.m_TextComponent.margin.x;
				int num6 = this.ClampArrayIndex(this.m_TextComponent.textInfo.characterCount - 1);
				float num7 = localPosition.x + localPosition3.x + localPosition2.x + this.m_TextComponent.textInfo.characterInfo[num6].origin + this.m_TextComponent.margin.z + (float)this.m_CaretWidth;
				if (x > 0.0001f && num5 > rect2.xMin)
				{
					float num8 = rect2.xMin - num5;
					if (x < -num8)
					{
						num8 = -x;
					}
					this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(num8, 0f);
					this.AssignPositioningIfNeeded();
				}
				else if (x < -0.0001f && num7 < rect2.xMax)
				{
					float num9 = rect2.xMax - num7;
					if (-x < num9)
					{
						num9 = -x;
					}
					this.m_TextComponent.rectTransform.anchoredPosition += new Vector2(num9, 0f);
					this.AssignPositioningIfNeeded();
				}
				this.m_HasTextBeenRemoved = false;
			}
			this.m_forceRectTransformAdjustment = false;
		}

		protected char Validate(string text, int pos, char ch)
		{
			if (this.characterValidation == TMP_InputField.CharacterValidation.None || !base.enabled)
			{
				return ch;
			}
			if (this.characterValidation == TMP_InputField.CharacterValidation.Integer || this.characterValidation == TMP_InputField.CharacterValidation.Decimal)
			{
				bool flag = pos == 0 && text.Length > 0 && text[0] == '-';
				bool flag2 = this.stringPositionInternal == 0 || this.stringSelectPositionInternal == 0;
				if (!flag)
				{
					if (ch >= '0' && ch <= '9')
					{
						return ch;
					}
					if (ch == '-' && (pos == 0 || flag2) && !text.Contains('-'))
					{
						return ch;
					}
					string numberDecimalSeparator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
					if (ch == Convert.ToChar(numberDecimalSeparator) && this.characterValidation == TMP_InputField.CharacterValidation.Decimal && !text.Contains(numberDecimalSeparator))
					{
						return ch;
					}
					if (this.characterValidation == TMP_InputField.CharacterValidation.Integer && ch == '.' && (pos == 0 || flag2) && !text.Contains('-'))
					{
						return '-';
					}
				}
			}
			else if (this.characterValidation == TMP_InputField.CharacterValidation.Digit)
			{
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
			}
			else if (this.characterValidation == TMP_InputField.CharacterValidation.Alphanumeric)
			{
				if (ch >= 'A' && ch <= 'Z')
				{
					return ch;
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
			}
			else if (this.characterValidation == TMP_InputField.CharacterValidation.Name)
			{
				char c = ((text.Length > 0) ? text[Mathf.Clamp(pos - 1, 0, text.Length - 1)] : ' ');
				char c2 = ((text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ');
				char c3 = ((text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n');
				if (char.IsLetter(ch))
				{
					if (char.IsLower(ch) && pos == 0)
					{
						return char.ToUpper(ch);
					}
					if (char.IsLower(ch) && (c == ' ' || c == '-'))
					{
						return char.ToUpper(ch);
					}
					if (char.IsUpper(ch) && pos > 0 && c != ' ' && c != '\'' && c != '-' && !char.IsLower(c))
					{
						return char.ToLower(ch);
					}
					if (char.IsUpper(ch) && char.IsUpper(c2))
					{
						return '\0';
					}
					return ch;
				}
				else
				{
					if (ch == '\'' && c2 != ' ' && c2 != '\'' && c3 != '\'' && !text.Contains("'"))
					{
						return ch;
					}
					if (char.IsLetter(c) && ch == '-' && c2 != '-')
					{
						return ch;
					}
					if ((ch == ' ' || ch == '-') && pos != 0 && c != ' ' && c != '\'' && c != '-' && c2 != ' ' && c2 != '\'' && c2 != '-' && c3 != ' ' && c3 != '\'' && c3 != '-')
					{
						return ch;
					}
				}
			}
			else if (this.characterValidation == TMP_InputField.CharacterValidation.EmailAddress)
			{
				if (ch >= 'A' && ch <= 'Z')
				{
					return ch;
				}
				if (ch >= 'a' && ch <= 'z')
				{
					return ch;
				}
				if (ch >= '0' && ch <= '9')
				{
					return ch;
				}
				if (ch == '@' && text.IndexOf('@') == -1)
				{
					return ch;
				}
				if ("!#$%&'*+-/=?^_`{|}~".IndexOf(ch) != -1)
				{
					return ch;
				}
				if (ch == '.')
				{
					int num = (int)((text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ');
					char c4 = ((text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n');
					if (num != 46 && c4 != '.')
					{
						return ch;
					}
				}
			}
			else if (this.characterValidation == TMP_InputField.CharacterValidation.Regex)
			{
				if (Regex.IsMatch(ch.ToString(), this.m_RegexValue))
				{
					return ch;
				}
			}
			else if (this.characterValidation == TMP_InputField.CharacterValidation.CustomValidator && this.m_InputValidator != null)
			{
				char c5 = this.m_InputValidator.Validate(ref text, ref pos, ch);
				this.m_Text = text;
				this.stringSelectPositionInternal = (this.stringPositionInternal = pos);
				return c5;
			}
			return '\0';
		}

		public void ActivateInputField()
		{
			if (this.m_TextComponent == null || this.m_TextComponent.font == null || !this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			if (this.isFocused && this.m_SoftKeyboard != null && !this.m_SoftKeyboard.active)
			{
				this.m_SoftKeyboard.active = true;
				this.m_SoftKeyboard.text = this.m_Text;
			}
			this.m_ShouldActivateNextUpdate = true;
		}

		private void ActivateInputFieldInternal()
		{
			if (EventSystem.current == null)
			{
				return;
			}
			if (EventSystem.current.currentSelectedGameObject != base.gameObject)
			{
				EventSystem.current.SetSelectedGameObject(base.gameObject);
			}
			this.m_TouchKeyboardAllowsInPlaceEditing = !TMP_InputField.s_IsQuestDevice && TouchScreenKeyboard.isInPlaceEditingAllowed;
			if (this.TouchScreenKeyboardShouldBeUsed() && !this.shouldHideSoftKeyboard)
			{
				if (this.inputSystem != null && this.inputSystem.touchSupported)
				{
					TouchScreenKeyboard.hideInput = this.shouldHideMobileInput;
				}
				if (!this.shouldHideSoftKeyboard && !this.m_ReadOnly)
				{
					this.m_SoftKeyboard = ((this.inputType == TMP_InputField.InputType.Password) ? TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, false, this.multiLine, true, this.isAlert, "", this.characterLimit) : TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, this.inputType == TMP_InputField.InputType.AutoCorrect, this.multiLine, false, this.isAlert, "", this.characterLimit));
					this.OnFocus();
					if (this.m_SoftKeyboard != null)
					{
						int num = ((this.stringPositionInternal < this.stringSelectPositionInternal) ? (this.stringSelectPositionInternal - this.stringPositionInternal) : (this.stringPositionInternal - this.stringSelectPositionInternal));
						this.m_SoftKeyboard.selection = new RangeInt((this.stringPositionInternal < this.stringSelectPositionInternal) ? this.stringPositionInternal : this.stringSelectPositionInternal, num);
					}
				}
			}
			else
			{
				if (!this.TouchScreenKeyboardShouldBeUsed() && !this.m_ReadOnly && this.inputSystem != null)
				{
					this.inputSystem.imeCompositionMode = IMECompositionMode.On;
				}
				this.OnFocus();
			}
			this.m_AllowInput = true;
			this.m_OriginalText = this.text;
			this.m_WasCanceled = false;
			this.SetCaretVisible();
			this.UpdateLabel();
		}

		public override void OnSelect(BaseEventData eventData)
		{
			base.OnSelect(eventData);
			this.SendOnFocus();
			if (this.shouldActivateOnSelect)
			{
				this.ActivateInputField();
			}
		}

		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}
			this.ActivateInputField();
		}

		public void OnControlClick()
		{
		}

		public void ReleaseSelection()
		{
			this.m_SelectionStillActive = false;
			this.m_ReleaseSelection = false;
			this.m_PreviouslySelectedObject = null;
			this.MarkGeometryAsDirty();
			this.SendOnEndEdit();
			this.SendOnEndTextSelection();
		}

		public void DeactivateInputField()
		{
			this.DeactivateInputField(false);
		}

		public void DeactivateInputField(bool clearSelection = false)
		{
			if (!this.m_AllowInput)
			{
				return;
			}
			this.m_HasDoneFocusTransition = false;
			this.m_AllowInput = false;
			if (this.m_Placeholder != null)
			{
				this.m_Placeholder.enabled = string.IsNullOrEmpty(this.m_Text);
			}
			if (this.m_TextComponent != null && this.IsInteractable())
			{
				if (this.m_WasCanceled && this.m_RestoreOriginalTextOnEscape && !this.m_IsKeyboardBeingClosedInHoloLens)
				{
					this.text = this.m_OriginalText;
				}
				if (this.m_SoftKeyboard != null)
				{
					this.m_SoftKeyboard.active = false;
					this.m_SoftKeyboard = null;
				}
				this.m_SelectionStillActive = true;
				if ((this.m_ResetOnDeActivation || this.m_ReleaseSelection || clearSelection) && this.m_VerticalScrollbar == null)
				{
					this.ReleaseSelection();
				}
				if (this.inputSystem != null)
				{
					this.inputSystem.imeCompositionMode = IMECompositionMode.Auto;
				}
				this.m_IsKeyboardBeingClosedInHoloLens = false;
				if (!EventSystem.current.alreadySelecting && EventSystem.current.currentSelectedGameObject == base.gameObject)
				{
					EventSystem.current.SetSelectedGameObject(null);
				}
			}
			this.MarkGeometryAsDirty();
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			this.DeactivateInputField();
			base.OnDeselect(eventData);
			this.SendOnFocusLost();
		}

		public virtual void OnSubmit(BaseEventData eventData)
		{
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			if (!this.isFocused)
			{
				this.m_ShouldActivateNextUpdate = true;
			}
			this.SendOnSubmit();
			this.DeactivateInputField();
			if (eventData != null)
			{
				eventData.Use();
			}
		}

		public virtual void OnCancel(BaseEventData eventData)
		{
			if (!this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			if (!this.isFocused)
			{
				this.m_ShouldActivateNextUpdate = true;
			}
			this.m_WasCanceled = true;
			this.DeactivateInputField();
			eventData.Use();
		}

		public override void OnMove(AxisEventData eventData)
		{
			if (!this.m_AllowInput)
			{
				base.OnMove(eventData);
			}
		}

		private void EnforceContentType()
		{
			switch (this.contentType)
			{
			case TMP_InputField.ContentType.Standard:
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.None;
				break;
			case TMP_InputField.ContentType.Autocorrected:
				this.m_InputType = TMP_InputField.InputType.AutoCorrect;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.None;
				break;
			case TMP_InputField.ContentType.IntegerNumber:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Integer;
				break;
			case TMP_InputField.ContentType.DecimalNumber:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Decimal;
				break;
			case TMP_InputField.ContentType.Alphanumeric:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.ASCIICapable;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Alphanumeric;
				break;
			case TMP_InputField.ContentType.Name:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Name;
				break;
			case TMP_InputField.ContentType.EmailAddress:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_InputType = TMP_InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.EmailAddress;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.EmailAddress;
				break;
			case TMP_InputField.ContentType.Password:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_InputType = TMP_InputField.InputType.Password;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.None;
				break;
			case TMP_InputField.ContentType.Pin:
				this.m_LineType = TMP_InputField.LineType.SingleLine;
				this.m_InputType = TMP_InputField.InputType.Password;
				this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				this.m_CharacterValidation = TMP_InputField.CharacterValidation.Digit;
				break;
			}
			this.SetTextComponentWrapMode();
		}

		private void SetTextComponentWrapMode()
		{
			if (this.m_TextComponent == null)
			{
				return;
			}
			if (this.multiLine)
			{
				this.m_TextComponent.textWrappingMode = TextWrappingModes.Normal;
				return;
			}
			this.m_TextComponent.textWrappingMode = TextWrappingModes.PreserveWhitespaceNoWrap;
		}

		private void SetTextComponentRichTextMode()
		{
			if (this.m_TextComponent == null)
			{
				return;
			}
			this.m_TextComponent.richText = this.m_RichText;
		}

		private void SetToCustomIfContentTypeIsNot(params TMP_InputField.ContentType[] allowedContentTypes)
		{
			if (this.contentType == TMP_InputField.ContentType.Custom)
			{
				return;
			}
			for (int i = 0; i < allowedContentTypes.Length; i++)
			{
				if (this.contentType == allowedContentTypes[i])
				{
					return;
				}
			}
			this.contentType = TMP_InputField.ContentType.Custom;
		}

		private void SetToCustom()
		{
			if (this.contentType == TMP_InputField.ContentType.Custom)
			{
				return;
			}
			this.contentType = TMP_InputField.ContentType.Custom;
		}

		private void SetToCustom(TMP_InputField.CharacterValidation characterValidation)
		{
			if (this.contentType == TMP_InputField.ContentType.Custom)
			{
				return;
			}
			this.contentType = TMP_InputField.ContentType.Custom;
		}

		protected override void DoStateTransition(Selectable.SelectionState state, bool instant)
		{
			if (this.m_HasDoneFocusTransition)
			{
				state = Selectable.SelectionState.Selected;
			}
			else if (state == Selectable.SelectionState.Pressed)
			{
				this.m_HasDoneFocusTransition = true;
			}
			base.DoStateTransition(state, instant);
		}

		public virtual void CalculateLayoutInputHorizontal()
		{
		}

		public virtual void CalculateLayoutInputVertical()
		{
		}

		public virtual float minWidth
		{
			get
			{
				return 0f;
			}
		}

		public virtual float preferredWidth
		{
			get
			{
				if (this.textComponent == null)
				{
					return 0f;
				}
				float num = 0f;
				if (this.m_LayoutGroup != null)
				{
					num = (float)this.m_LayoutGroup.padding.horizontal;
				}
				if (this.m_TextViewport != null)
				{
					num += this.m_TextViewport.offsetMin.x - this.m_TextViewport.offsetMax.x;
				}
				return this.m_TextComponent.preferredWidth + num;
			}
		}

		public virtual float flexibleWidth
		{
			get
			{
				return -1f;
			}
		}

		public virtual float minHeight
		{
			get
			{
				return 0f;
			}
		}

		public virtual float preferredHeight
		{
			get
			{
				if (this.textComponent == null)
				{
					return 0f;
				}
				float num = 0f;
				if (this.m_LayoutGroup != null)
				{
					num = (float)this.m_LayoutGroup.padding.vertical;
				}
				if (this.m_TextViewport != null)
				{
					num += this.m_TextViewport.offsetMin.y - this.m_TextViewport.offsetMax.y;
				}
				return this.m_TextComponent.preferredHeight + num;
			}
		}

		public virtual float flexibleHeight
		{
			get
			{
				return -1f;
			}
		}

		public virtual int layoutPriority
		{
			get
			{
				return 1;
			}
		}

		public void SetGlobalPointSize(float pointSize)
		{
			TMP_Text tmp_Text = this.m_Placeholder as TMP_Text;
			if (tmp_Text != null)
			{
				tmp_Text.fontSize = pointSize;
			}
			this.textComponent.fontSize = pointSize;
		}

		public void SetGlobalFontAsset(TMP_FontAsset fontAsset)
		{
			TMP_Text tmp_Text = this.m_Placeholder as TMP_Text;
			if (tmp_Text != null)
			{
				tmp_Text.font = fontAsset;
			}
			this.textComponent.font = fontAsset;
		}

		Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}

		public Action onFocus;

		protected TouchScreenKeyboard m_SoftKeyboard;

		private static readonly char[] kSeparators = new char[] { ' ', '.', ',', '\t', '\r', '\n' };

		private static bool s_IsQuestDevice = false;

		protected RectTransform m_RectTransform;

		[SerializeField]
		protected RectTransform m_TextViewport;

		protected RectMask2D m_TextComponentRectMask;

		protected RectMask2D m_TextViewportRectMask;

		[SerializeField]
		protected TMP_Text m_TextComponent;

		protected RectTransform m_TextComponentRectTransform;

		[SerializeField]
		protected Graphic m_Placeholder;

		[SerializeField]
		protected Scrollbar m_VerticalScrollbar;

		[SerializeField]
		protected TMP_ScrollbarEventHandler m_VerticalScrollbarEventHandler;

		private bool m_IsDrivenByLayoutComponents;

		[SerializeField]
		private LayoutGroup m_LayoutGroup;

		private IScrollHandler m_IScrollHandlerParent;

		private float m_ScrollPosition;

		[SerializeField]
		protected float m_ScrollSensitivity = 1f;

		[SerializeField]
		private TMP_InputField.ContentType m_ContentType;

		[SerializeField]
		private TMP_InputField.InputType m_InputType;

		[SerializeField]
		private char m_AsteriskChar = '*';

		[SerializeField]
		private TouchScreenKeyboardType m_KeyboardType;

		[SerializeField]
		private TMP_InputField.LineType m_LineType;

		[SerializeField]
		private bool m_HideMobileInput;

		[SerializeField]
		private bool m_HideSoftKeyboard;

		[SerializeField]
		private TMP_InputField.CharacterValidation m_CharacterValidation;

		[SerializeField]
		private string m_RegexValue = string.Empty;

		[SerializeField]
		private float m_GlobalPointSize = 14f;

		[SerializeField]
		private int m_CharacterLimit;

		[SerializeField]
		private TMP_InputField.SubmitEvent m_OnEndEdit = new TMP_InputField.SubmitEvent();

		[SerializeField]
		private TMP_InputField.SubmitEvent m_OnSubmit = new TMP_InputField.SubmitEvent();

		[SerializeField]
		private TMP_InputField.SelectionEvent m_OnSelect = new TMP_InputField.SelectionEvent();

		[SerializeField]
		private TMP_InputField.SelectionEvent m_OnDeselect = new TMP_InputField.SelectionEvent();

		[SerializeField]
		private TMP_InputField.TextSelectionEvent m_OnTextSelection = new TMP_InputField.TextSelectionEvent();

		[SerializeField]
		private TMP_InputField.TextSelectionEvent m_OnEndTextSelection = new TMP_InputField.TextSelectionEvent();

		[SerializeField]
		private TMP_InputField.OnChangeEvent m_OnValueChanged = new TMP_InputField.OnChangeEvent();

		[SerializeField]
		private TMP_InputField.TouchScreenKeyboardEvent m_OnTouchScreenKeyboardStatusChanged = new TMP_InputField.TouchScreenKeyboardEvent();

		[SerializeField]
		private TMP_InputField.OnValidateInput m_OnValidateInput;

		[SerializeField]
		private Color m_CaretColor = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);

		[SerializeField]
		private bool m_CustomCaretColor;

		[SerializeField]
		private Color m_SelectionColor = new Color(0.65882355f, 0.80784315f, 1f, 0.7529412f);

		[SerializeField]
		[TextArea(5, 10)]
		protected string m_Text = string.Empty;

		[SerializeField]
		[Range(0f, 4f)]
		private float m_CaretBlinkRate = 0.85f;

		[SerializeField]
		[Range(1f, 5f)]
		private int m_CaretWidth = 1;

		[SerializeField]
		private bool m_ReadOnly;

		[SerializeField]
		private bool m_RichText = true;

		protected int m_StringPosition;

		protected int m_StringSelectPosition;

		protected int m_CaretPosition;

		protected int m_CaretSelectPosition;

		private RectTransform caretRectTrans;

		protected UIVertex[] m_CursorVerts;

		private CanvasRenderer m_CachedInputRenderer;

		private Vector2 m_LastPosition;

		[NonSerialized]
		protected Mesh m_Mesh;

		private bool m_AllowInput;

		private bool m_ShouldActivateNextUpdate;

		private bool m_UpdateDrag;

		private bool m_DragPositionOutOfBounds;

		private const float kHScrollSpeed = 0.05f;

		private const float kVScrollSpeed = 0.1f;

		protected bool m_CaretVisible;

		private Coroutine m_BlinkCoroutine;

		private float m_BlinkStartTime;

		private Coroutine m_DragCoroutine;

		private string m_OriginalText = "";

		private bool m_WasCanceled;

		private bool m_HasDoneFocusTransition;

		private WaitForSecondsRealtime m_WaitForSecondsRealtime;

		private bool m_PreventCallback;

		private bool m_TouchKeyboardAllowsInPlaceEditing;

		private bool m_IsTextComponentUpdateRequired;

		private bool m_HasTextBeenRemoved;

		private float m_PointerDownClickStartTime;

		private float m_KeyDownStartTime;

		private float m_DoubleClickDelay = 0.5f;

		private bool m_IsApplePlatform;

		private const string kEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";

		private const string kOculusQuestDeviceModel = "Oculus Quest";

		private bool m_IsCompositionActive;

		private bool m_ShouldUpdateIMEWindowPosition;

		private int m_PreviousIMEInsertionLine;

		[SerializeField]
		protected TMP_FontAsset m_GlobalFontAsset;

		[SerializeField]
		protected bool m_OnFocusSelectAll = true;

		protected bool m_isSelectAll;

		[SerializeField]
		protected bool m_ResetOnDeActivation = true;

		private bool m_SelectionStillActive;

		private bool m_ReleaseSelection;

		private KeyCode m_LastKeyCode;

		private GameObject m_PreviouslySelectedObject;

		[SerializeField]
		private bool m_KeepTextSelectionVisible;

		[SerializeField]
		private bool m_RestoreOriginalTextOnEscape = true;

		[SerializeField]
		protected bool m_isRichTextEditingAllowed;

		[SerializeField]
		protected int m_LineLimit;

		public bool isAlert;

		[SerializeField]
		protected TMP_InputValidator m_InputValidator;

		[SerializeField]
		private bool m_ShouldActivateOnSelect = true;

		private bool m_isSelected;

		private bool m_IsStringPositionDirty;

		private bool m_IsCaretPositionDirty;

		private bool m_forceRectTransformAdjustment;

		private bool m_IsKeyboardBeingClosedInHoloLens;

		private Event m_ProcessingEvent = new Event();

		public enum ContentType
		{
			Standard,
			Autocorrected,
			IntegerNumber,
			DecimalNumber,
			Alphanumeric,
			Name,
			EmailAddress,
			Password,
			Pin,
			Custom
		}

		public enum InputType
		{
			Standard,
			AutoCorrect,
			Password
		}

		public enum CharacterValidation
		{
			None,
			Digit,
			Integer,
			Decimal,
			Alphanumeric,
			Name,
			Regex,
			EmailAddress,
			CustomValidator
		}

		public enum LineType
		{
			SingleLine,
			MultiLineSubmit,
			MultiLineNewline
		}

		public delegate char OnValidateInput(string text, int charIndex, char addedChar);

		[Serializable]
		public class SubmitEvent : UnityEvent<string>
		{
		}

		[Serializable]
		public class OnChangeEvent : UnityEvent<string>
		{
		}

		[Serializable]
		public class SelectionEvent : UnityEvent<string>
		{
		}

		[Serializable]
		public class TextSelectionEvent : UnityEvent<string, int, int>
		{
		}

		[Serializable]
		public class TouchScreenKeyboardEvent : UnityEvent<TouchScreenKeyboard.Status>
		{
		}

		private struct RECT
		{
			public int left;

			public int top;

			public int right;

			public int bottom;
		}

		private struct POINT
		{
			public int x;

			public int y;
		}

		protected enum EditState
		{
			Continue,
			Finish
		}
	}
}
