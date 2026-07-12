using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
	[AddComponentMenu("UI/Input Field", 31)]
	public class InputField : Selectable, IUpdateSelectedHandler, IEventSystemHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, ISubmitHandler, ICanvasElement, ILayoutElement
	{
		private BaseInput input
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
				if (!(this.input != null))
				{
					return Input.compositionString;
				}
				return this.input.compositionString;
			}
		}

		protected InputField()
		{
			this.EnforceTextHOverflow();
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

		protected TextGenerator cachedInputTextGenerator
		{
			get
			{
				if (this.m_InputTextCache == null)
				{
					this.m_InputTextCache = new TextGenerator();
				}
				return this.m_InputTextCache;
			}
		}

		public bool shouldHideMobileInput
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				return (platform != RuntimePlatform.IPhonePlayer && platform != RuntimePlatform.Android && platform != RuntimePlatform.tvOS) || this.m_HideMobileInput;
			}
			set
			{
				SetPropertyUtility.SetStruct<bool>(ref this.m_HideMobileInput, value);
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
			if (this.m_LineType == InputField.LineType.SingleLine)
			{
				value = value.Replace("\n", "").Replace("\t", "");
			}
			if (this.onValidateInput != null || this.characterValidation != InputField.CharacterValidation.None)
			{
				this.m_Text = "";
				InputField.OnValidateInput onValidateInput = this.onValidateInput ?? new InputField.OnValidateInput(this.Validate);
				this.m_CaretPosition = (this.m_CaretSelectPosition = value.Length);
				int num = ((this.characterLimit > 0) ? Math.Min(this.characterLimit, value.Length) : value.Length);
				for (int i = 0; i < num; i++)
				{
					char c = onValidateInput(this.m_Text, this.m_Text.Length, value[i]);
					if (c != '\0')
					{
						this.m_Text += c.ToString();
					}
				}
			}
			else
			{
				this.m_Text = ((this.characterLimit > 0 && value.Length > this.characterLimit) ? value.Substring(0, this.characterLimit) : value);
			}
			if (this.m_Keyboard != null)
			{
				this.m_Keyboard.text = this.m_Text;
			}
			if (this.m_CaretPosition > this.m_Text.Length)
			{
				this.m_CaretPosition = (this.m_CaretSelectPosition = this.m_Text.Length);
			}
			else if (this.m_CaretSelectPosition > this.m_Text.Length)
			{
				this.m_CaretSelectPosition = this.m_Text.Length;
			}
			if (sendCallback)
			{
				this.SendOnValueChanged();
			}
			this.UpdateLabel();
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

		public Text textComponent
		{
			get
			{
				return this.m_TextComponent;
			}
			set
			{
				if (this.m_TextComponent != null)
				{
					this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
					this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
					this.m_TextComponent.UnregisterDirtyMaterialCallback(new UnityAction(this.UpdateCaretMaterial));
				}
				if (SetPropertyUtility.SetClass<Text>(ref this.m_TextComponent, value))
				{
					this.EnforceTextHOverflow();
					if (this.m_TextComponent != null)
					{
						this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
						this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
						this.m_TextComponent.RegisterDirtyMaterialCallback(new UnityAction(this.UpdateCaretMaterial));
					}
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

		public InputField.SubmitEvent onEndEdit
		{
			get
			{
				return this.m_OnEndEdit;
			}
			set
			{
				SetPropertyUtility.SetClass<InputField.SubmitEvent>(ref this.m_OnEndEdit, value);
			}
		}

		[Obsolete("onValueChange has been renamed to onValueChanged")]
		public InputField.OnChangeEvent onValueChange
		{
			get
			{
				return this.onValueChanged;
			}
			set
			{
				this.onValueChanged = value;
			}
		}

		public InputField.OnChangeEvent onValueChanged
		{
			get
			{
				return this.m_OnValueChanged;
			}
			set
			{
				SetPropertyUtility.SetClass<InputField.OnChangeEvent>(ref this.m_OnValueChanged, value);
			}
		}

		public InputField.OnValidateInput onValidateInput
		{
			get
			{
				return this.m_OnValidateInput;
			}
			set
			{
				SetPropertyUtility.SetClass<InputField.OnValidateInput>(ref this.m_OnValidateInput, value);
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
					if (this.m_Keyboard != null)
					{
						this.m_Keyboard.characterLimit = value;
					}
				}
			}
		}

		public InputField.ContentType contentType
		{
			get
			{
				return this.m_ContentType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<InputField.ContentType>(ref this.m_ContentType, value))
				{
					this.EnforceContentType();
				}
			}
		}

		public InputField.LineType lineType
		{
			get
			{
				return this.m_LineType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<InputField.LineType>(ref this.m_LineType, value))
				{
					this.SetToCustomIfContentTypeIsNot(new InputField.ContentType[]
					{
						InputField.ContentType.Standard,
						InputField.ContentType.Autocorrected
					});
					this.EnforceTextHOverflow();
				}
			}
		}

		public InputField.InputType inputType
		{
			get
			{
				return this.m_InputType;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<InputField.InputType>(ref this.m_InputType, value))
				{
					this.SetToCustom();
				}
			}
		}

		public TouchScreenKeyboard touchScreenKeyboard
		{
			get
			{
				return this.m_Keyboard;
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

		public InputField.CharacterValidation characterValidation
		{
			get
			{
				return this.m_CharacterValidation;
			}
			set
			{
				if (SetPropertyUtility.SetStruct<InputField.CharacterValidation>(ref this.m_CharacterValidation, value))
				{
					this.SetToCustom();
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

		public bool multiLine
		{
			get
			{
				return this.m_LineType == InputField.LineType.MultiLineNewline || this.lineType == InputField.LineType.MultiLineSubmit;
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

		protected void ClampPos(ref int pos)
		{
			if (pos < 0)
			{
				pos = 0;
				return;
			}
			if (pos > this.text.Length)
			{
				pos = this.text.Length;
			}
		}

		protected int caretPositionInternal
		{
			get
			{
				return this.m_CaretPosition + this.compositionString.Length;
			}
			set
			{
				this.m_CaretPosition = value;
				this.ClampPos(ref this.m_CaretPosition);
			}
		}

		protected int caretSelectPositionInternal
		{
			get
			{
				return this.m_CaretSelectPosition + this.compositionString.Length;
			}
			set
			{
				this.m_CaretSelectPosition = value;
				this.ClampPos(ref this.m_CaretSelectPosition);
			}
		}

		private bool hasSelection
		{
			get
			{
				return this.caretPositionInternal != this.caretSelectPositionInternal;
			}
		}

		public int caretPosition
		{
			get
			{
				return this.m_CaretSelectPosition + this.compositionString.Length;
			}
			set
			{
				this.selectionAnchorPosition = value;
				this.selectionFocusPosition = value;
			}
		}

		public int selectionAnchorPosition
		{
			get
			{
				return this.m_CaretPosition + this.compositionString.Length;
			}
			set
			{
				if (this.compositionString.Length != 0)
				{
					return;
				}
				this.m_CaretPosition = value;
				this.ClampPos(ref this.m_CaretPosition);
			}
		}

		public int selectionFocusPosition
		{
			get
			{
				return this.m_CaretSelectPosition + this.compositionString.Length;
			}
			set
			{
				if (this.compositionString.Length != 0)
				{
					return;
				}
				this.m_CaretSelectPosition = value;
				this.ClampPos(ref this.m_CaretSelectPosition);
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.m_Text == null)
			{
				this.m_Text = string.Empty;
			}
			this.m_DrawStart = 0;
			this.m_DrawEnd = this.m_Text.Length;
			if (this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.SetMaterial(this.m_TextComponent.GetModifiedMaterial(Graphic.defaultGraphicMaterial), Texture2D.whiteTexture);
			}
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
				this.m_TextComponent.RegisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
				this.m_TextComponent.RegisterDirtyMaterialCallback(new UnityAction(this.UpdateCaretMaterial));
				this.UpdateLabel();
			}
		}

		protected override void OnDisable()
		{
			this.m_BlinkCoroutine = null;
			this.DeactivateInputField();
			if (this.m_TextComponent != null)
			{
				this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.MarkGeometryAsDirty));
				this.m_TextComponent.UnregisterDirtyVerticesCallback(new UnityAction(this.UpdateLabel));
				this.m_TextComponent.UnregisterDirtyMaterialCallback(new UnityAction(this.UpdateCaretMaterial));
			}
			CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);
			if (this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.Clear();
			}
			if (this.m_Mesh != null)
			{
				Object.DestroyImmediate(this.m_Mesh);
			}
			this.m_Mesh = null;
			base.OnDisable();
		}

		private IEnumerator CaretBlink()
		{
			this.m_CaretVisible = true;
			yield return null;
			while (this.isFocused && this.m_CaretBlinkRate > 0f)
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

		private void UpdateCaretMaterial()
		{
			if (this.m_TextComponent != null && this.m_CachedInputRenderer != null)
			{
				this.m_CachedInputRenderer.SetMaterial(this.m_TextComponent.GetModifiedMaterial(Graphic.defaultGraphicMaterial), Texture2D.whiteTexture);
			}
		}

		protected void OnFocus()
		{
			this.SelectAll();
		}

		protected void SelectAll()
		{
			this.caretPositionInternal = this.text.Length;
			this.caretSelectPositionInternal = 0;
		}

		public void MoveTextEnd(bool shift)
		{
			int length = this.text.Length;
			if (shift)
			{
				this.caretSelectPositionInternal = length;
			}
			else
			{
				this.caretPositionInternal = length;
				this.caretSelectPositionInternal = this.caretPositionInternal;
			}
			this.UpdateLabel();
		}

		public void MoveTextStart(bool shift)
		{
			int num = 0;
			if (shift)
			{
				this.caretSelectPositionInternal = num;
			}
			else
			{
				this.caretPositionInternal = num;
				this.caretSelectPositionInternal = this.caretPositionInternal;
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

		private bool TouchScreenKeyboardShouldBeUsed()
		{
			if (Application.platform == RuntimePlatform.Android)
			{
				return !TouchScreenKeyboard.isInPlaceEditingAllowed;
			}
			return TouchScreenKeyboard.isSupported;
		}

		private bool InPlaceEditing()
		{
			return !TouchScreenKeyboard.isSupported || this.m_TouchKeyboardAllowsInPlaceEditing;
		}

		private bool InPlaceEditingChanged()
		{
			return this.m_TouchKeyboardAllowsInPlaceEditing != TouchScreenKeyboard.isInPlaceEditingAllowed;
		}

		private void UpdateCaretFromKeyboard()
		{
			RangeInt selection = this.m_Keyboard.selection;
			int start = selection.start;
			int end = selection.end;
			bool flag = false;
			if (this.caretPositionInternal != start)
			{
				flag = true;
				this.caretPositionInternal = start;
			}
			if (this.caretSelectPositionInternal != end)
			{
				this.caretSelectPositionInternal = end;
				flag = true;
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
			this.AssignPositioningIfNeeded();
			if (this.isFocused && this.InPlaceEditingChanged())
			{
				if (this.m_CachedInputRenderer != null)
				{
					using (VertexHelper vertexHelper = new VertexHelper())
					{
						vertexHelper.FillMesh(this.mesh);
					}
					this.m_CachedInputRenderer.SetMesh(this.mesh);
				}
				this.DeactivateInputField();
			}
			if (!this.isFocused || this.InPlaceEditing())
			{
				return;
			}
			if (this.m_Keyboard == null || this.m_Keyboard.status != TouchScreenKeyboard.Status.Visible)
			{
				if (this.m_Keyboard != null)
				{
					if (!this.m_ReadOnly)
					{
						this.text = this.m_Keyboard.text;
					}
					if (this.m_Keyboard.status == TouchScreenKeyboard.Status.Canceled)
					{
						this.m_WasCanceled = true;
					}
				}
				this.OnDeselect(null);
				return;
			}
			string text = this.m_Keyboard.text;
			if (this.m_Text != text)
			{
				if (this.m_ReadOnly)
				{
					this.m_Keyboard.text = this.m_Text;
				}
				else
				{
					this.m_Text = "";
					foreach (char c in text)
					{
						if (c == '\r' || c == '\u0003')
						{
							c = '\n';
						}
						if (this.onValidateInput != null)
						{
							c = this.onValidateInput(this.m_Text, this.m_Text.Length, c);
						}
						else if (this.characterValidation != InputField.CharacterValidation.None)
						{
							c = this.Validate(this.m_Text, this.m_Text.Length, c);
						}
						if (this.lineType == InputField.LineType.MultiLineSubmit && c == '\n')
						{
							this.m_Keyboard.text = this.m_Text;
							this.OnDeselect(null);
							return;
						}
						if (c != '\0')
						{
							this.m_Text += c.ToString();
						}
					}
					if (this.characterLimit > 0 && this.m_Text.Length > this.characterLimit)
					{
						this.m_Text = this.m_Text.Substring(0, this.characterLimit);
					}
					if (this.m_Keyboard.canGetSelection)
					{
						this.UpdateCaretFromKeyboard();
					}
					else
					{
						this.caretPositionInternal = (this.caretSelectPositionInternal = this.m_Text.Length);
					}
					if (this.m_Text != text)
					{
						this.m_Keyboard.text = this.m_Text;
					}
					this.SendOnValueChangedAndUpdateLabel();
				}
			}
			else if (this.m_HideMobileInput && this.m_Keyboard.canSetSelection)
			{
				int num = Mathf.Min(this.caretSelectPositionInternal, this.caretPositionInternal);
				int num2 = Mathf.Abs(this.caretSelectPositionInternal - this.caretPositionInternal);
				this.m_Keyboard.selection = new RangeInt(num, num2);
			}
			else if (this.m_Keyboard.canGetSelection && !this.m_HideMobileInput)
			{
				this.UpdateCaretFromKeyboard();
			}
			if (this.m_Keyboard.status != TouchScreenKeyboard.Status.Visible)
			{
				if (this.m_Keyboard.status == TouchScreenKeyboard.Status.Canceled)
				{
					this.m_WasCanceled = true;
				}
				this.OnDeselect(null);
			}
		}

		[Obsolete("This function is no longer used. Please use RectTransformUtility.ScreenPointToLocalPointInRectangle() instead.")]
		public Vector2 ScreenToLocal(Vector2 screen)
		{
			Canvas canvas = this.m_TextComponent.canvas;
			if (canvas == null)
			{
				return screen;
			}
			Vector3 vector = Vector3.zero;
			if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				vector = this.m_TextComponent.transform.InverseTransformPoint(screen);
			}
			else if (canvas.worldCamera != null)
			{
				Ray ray = canvas.worldCamera.ScreenPointToRay(screen);
				Plane plane = new Plane(this.m_TextComponent.transform.forward, this.m_TextComponent.transform.position);
				float num;
				plane.Raycast(ray, out num);
				vector = this.m_TextComponent.transform.InverseTransformPoint(ray.GetPoint(num));
			}
			return new Vector2(vector.x, vector.y);
		}

		private int GetUnclampedCharacterLineFromPosition(Vector2 pos, TextGenerator generator)
		{
			if (!this.multiLine)
			{
				return 0;
			}
			float num = pos.y * this.m_TextComponent.pixelsPerUnit;
			float num2 = 0f;
			int i = 0;
			while (i < generator.lineCount)
			{
				float topY = generator.lines[i].topY;
				float num3 = topY - (float)generator.lines[i].height;
				if (num > topY)
				{
					float num4 = topY - num2;
					if (num > topY - 0.5f * num4)
					{
						return i - 1;
					}
					return i;
				}
				else
				{
					if (num > num3)
					{
						return i;
					}
					num2 = num3;
					i++;
				}
			}
			return generator.lineCount;
		}

		protected int GetCharacterIndexFromPosition(Vector2 pos)
		{
			TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
			if (cachedTextGenerator.lineCount == 0)
			{
				return 0;
			}
			int unclampedCharacterLineFromPosition = this.GetUnclampedCharacterLineFromPosition(pos, cachedTextGenerator);
			if (unclampedCharacterLineFromPosition < 0)
			{
				return 0;
			}
			if (unclampedCharacterLineFromPosition >= cachedTextGenerator.lineCount)
			{
				return cachedTextGenerator.characterCountVisible;
			}
			int startCharIdx = cachedTextGenerator.lines[unclampedCharacterLineFromPosition].startCharIdx;
			int lineEndPosition = InputField.GetLineEndPosition(cachedTextGenerator, unclampedCharacterLineFromPosition);
			int num = startCharIdx;
			while (num < lineEndPosition && num < cachedTextGenerator.characterCountVisible)
			{
				UICharInfo uicharInfo = cachedTextGenerator.characters[num];
				Vector2 vector = uicharInfo.cursorPos / this.m_TextComponent.pixelsPerUnit;
				float num2 = pos.x - vector.x;
				float num3 = vector.x + uicharInfo.charWidth / this.m_TextComponent.pixelsPerUnit - pos.x;
				if (num2 < num3)
				{
					return num;
				}
				num++;
			}
			return lineEndPosition;
		}

		private bool MayDrag(PointerEventData eventData)
		{
			return this.IsActive() && this.IsInteractable() && eventData.button == PointerEventData.InputButton.Left && this.m_TextComponent != null && (this.InPlaceEditing() || this.m_HideMobileInput);
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
			Vector2 zero = Vector2.zero;
			if (!MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref zero))
			{
				return;
			}
			Vector2 vector;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textComponent.rectTransform, zero, eventData.pressEventCamera, out vector);
			this.caretSelectPositionInternal = this.GetCharacterIndexFromPosition(vector) + this.m_DrawStart;
			this.MarkGeometryAsDirty();
			this.m_DragPositionOutOfBounds = !RectTransformUtility.RectangleContainsScreenPoint(this.textComponent.rectTransform, eventData.position, eventData.pressEventCamera);
			if (this.m_DragPositionOutOfBounds && this.m_DragCoroutine == null)
			{
				this.m_DragCoroutine = base.StartCoroutine(this.MouseDragOutsideRect(eventData));
			}
			eventData.Use();
		}

		private IEnumerator MouseDragOutsideRect(PointerEventData eventData)
		{
			while (this.m_UpdateDrag && this.m_DragPositionOutOfBounds)
			{
				Vector2 zero = Vector2.zero;
				if (!MultipleDisplayUtilities.GetRelativeMousePositionForDrag(eventData, ref zero))
				{
					break;
				}
				Vector2 vector;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textComponent.rectTransform, zero, eventData.pressEventCamera, out vector);
				Rect rect = this.textComponent.rectTransform.rect;
				if (this.multiLine)
				{
					if (vector.y > rect.yMax)
					{
						this.MoveUp(true, true);
					}
					else if (vector.y < rect.yMin)
					{
						this.MoveDown(true, true);
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
			if (!this.InPlaceEditing() && (this.m_Keyboard == null || !this.m_Keyboard.active))
			{
				this.OnSelect(eventData);
				return;
			}
			if (allowInput)
			{
				Vector2 vector;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(this.textComponent.rectTransform, eventData.pointerPressRaycast.screenPosition, eventData.pressEventCamera, out vector);
				this.caretSelectPositionInternal = (this.caretPositionInternal = this.GetCharacterIndexFromPosition(vector) + this.m_DrawStart);
			}
			this.UpdateLabel();
			eventData.Use();
		}

		protected InputField.EditState KeyPressed(Event evt)
		{
			EventModifiers modifiers = evt.modifiers;
			bool flag = ((SystemInfo.operatingSystemFamily == OperatingSystemFamily.MacOSX) ? ((modifiers & EventModifiers.Command) > EventModifiers.None) : ((modifiers & EventModifiers.Control) > EventModifiers.None));
			bool flag2 = (modifiers & EventModifiers.Shift) > EventModifiers.None;
			bool flag3 = (modifiers & EventModifiers.Alt) > EventModifiers.None;
			bool flag4 = flag && !flag3 && !flag2;
			bool flag5 = flag2 && !flag && !flag3;
			KeyCode keyCode = evt.keyCode;
			if (keyCode <= KeyCode.A)
			{
				if (keyCode <= KeyCode.Return)
				{
					if (keyCode == KeyCode.Backspace)
					{
						this.Backspace();
						return InputField.EditState.Continue;
					}
					if (keyCode != KeyCode.Return)
					{
						goto IL_0213;
					}
				}
				else
				{
					if (keyCode == KeyCode.Escape)
					{
						this.m_WasCanceled = true;
						return InputField.EditState.Finish;
					}
					if (keyCode != KeyCode.A)
					{
						goto IL_0213;
					}
					if (flag4)
					{
						this.SelectAll();
						return InputField.EditState.Continue;
					}
					goto IL_0213;
				}
			}
			else if (keyCode <= KeyCode.V)
			{
				if (keyCode != KeyCode.C)
				{
					if (keyCode != KeyCode.V)
					{
						goto IL_0213;
					}
					if (flag4)
					{
						this.Append(InputField.clipboard);
						this.UpdateLabel();
						return InputField.EditState.Continue;
					}
					goto IL_0213;
				}
				else
				{
					if (flag4)
					{
						if (this.inputType != InputField.InputType.Password)
						{
							InputField.clipboard = this.GetSelectedString();
						}
						else
						{
							InputField.clipboard = "";
						}
						return InputField.EditState.Continue;
					}
					goto IL_0213;
				}
			}
			else if (keyCode != KeyCode.X)
			{
				if (keyCode == KeyCode.Delete)
				{
					this.ForwardSpace();
					return InputField.EditState.Continue;
				}
				switch (keyCode)
				{
				case KeyCode.KeypadEnter:
					break;
				case KeyCode.KeypadEquals:
					goto IL_0213;
				case KeyCode.UpArrow:
					this.MoveUp(flag2);
					return InputField.EditState.Continue;
				case KeyCode.DownArrow:
					this.MoveDown(flag2);
					return InputField.EditState.Continue;
				case KeyCode.RightArrow:
					this.MoveRight(flag2, flag);
					return InputField.EditState.Continue;
				case KeyCode.LeftArrow:
					this.MoveLeft(flag2, flag);
					return InputField.EditState.Continue;
				case KeyCode.Insert:
					if (flag4)
					{
						if (this.inputType != InputField.InputType.Password)
						{
							InputField.clipboard = this.GetSelectedString();
						}
						else
						{
							InputField.clipboard = "";
						}
						return InputField.EditState.Continue;
					}
					if (flag5)
					{
						this.Append(InputField.clipboard);
						this.UpdateLabel();
						return InputField.EditState.Continue;
					}
					goto IL_0213;
				case KeyCode.Home:
					this.MoveTextStart(flag2);
					return InputField.EditState.Continue;
				case KeyCode.End:
					this.MoveTextEnd(flag2);
					return InputField.EditState.Continue;
				default:
					goto IL_0213;
				}
			}
			else
			{
				if (flag4)
				{
					if (this.inputType != InputField.InputType.Password)
					{
						InputField.clipboard = this.GetSelectedString();
					}
					else
					{
						InputField.clipboard = "";
					}
					this.Delete();
					this.UpdateTouchKeyboardFromEditChanges();
					this.SendOnValueChangedAndUpdateLabel();
					return InputField.EditState.Continue;
				}
				goto IL_0213;
			}
			if (this.lineType != InputField.LineType.MultiLineNewline)
			{
				return InputField.EditState.Finish;
			}
			IL_0213:
			char c = evt.character;
			if (!this.multiLine && (c == '\t' || c == '\r' || c == '\n'))
			{
				return InputField.EditState.Continue;
			}
			if (c == '\r' || c == '\u0003')
			{
				c = '\n';
			}
			if (this.IsValidChar(c))
			{
				this.Append(c);
			}
			if (c == '\0' && this.compositionString.Length > 0)
			{
				this.UpdateLabel();
			}
			return InputField.EditState.Continue;
		}

		private bool IsValidChar(char c)
		{
			return c != '\u007f' && (c == '\t' || c == '\n' || this.m_TextComponent.font.HasCharacter(c));
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
				if (this.m_ProcessingEvent.rawType == EventType.KeyDown)
				{
					flag = true;
					if (this.KeyPressed(this.m_ProcessingEvent) == InputField.EditState.Finish)
					{
						this.DeactivateInputField();
						break;
					}
				}
				EventType type = this.m_ProcessingEvent.type;
				if (type - EventType.ValidateCommand <= 1)
				{
					string commandName = this.m_ProcessingEvent.commandName;
					if (commandName != null && commandName == "SelectAll")
					{
						this.SelectAll();
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.UpdateLabel();
			}
			eventData.Use();
		}

		private string GetSelectedString()
		{
			if (!this.hasSelection)
			{
				return "";
			}
			int num = this.caretPositionInternal;
			int num2 = this.caretSelectPositionInternal;
			if (num > num2)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			return this.text.Substring(num, num2 - num);
		}

		private int FindtNextWordBegin()
		{
			if (this.caretSelectPositionInternal + 1 >= this.text.Length)
			{
				return this.text.Length;
			}
			int num = this.text.IndexOfAny(InputField.kSeparators, this.caretSelectPositionInternal + 1);
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
				this.caretPositionInternal = (this.caretSelectPositionInternal = Mathf.Max(this.caretPositionInternal, this.caretSelectPositionInternal));
				return;
			}
			int num;
			if (ctrl)
			{
				num = this.FindtNextWordBegin();
			}
			else
			{
				num = this.caretSelectPositionInternal + 1;
			}
			if (shift)
			{
				this.caretSelectPositionInternal = num;
				return;
			}
			this.caretSelectPositionInternal = (this.caretPositionInternal = num);
		}

		private int FindtPrevWordBegin()
		{
			if (this.caretSelectPositionInternal - 2 < 0)
			{
				return 0;
			}
			int num = this.text.LastIndexOfAny(InputField.kSeparators, this.caretSelectPositionInternal - 2);
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
				this.caretPositionInternal = (this.caretSelectPositionInternal = Mathf.Min(this.caretPositionInternal, this.caretSelectPositionInternal));
				return;
			}
			int num;
			if (ctrl)
			{
				num = this.FindtPrevWordBegin();
			}
			else
			{
				num = this.caretSelectPositionInternal - 1;
			}
			if (shift)
			{
				this.caretSelectPositionInternal = num;
				return;
			}
			this.caretSelectPositionInternal = (this.caretPositionInternal = num);
		}

		private int DetermineCharacterLine(int charPos, TextGenerator generator)
		{
			for (int i = 0; i < generator.lineCount - 1; i++)
			{
				if (generator.lines[i + 1].startCharIdx > charPos)
				{
					return i;
				}
			}
			return generator.lineCount - 1;
		}

		private int LineUpCharacterPosition(int originalPos, bool goToFirstChar)
		{
			if (originalPos >= this.cachedInputTextGenerator.characters.Count)
			{
				return 0;
			}
			UICharInfo uicharInfo = this.cachedInputTextGenerator.characters[originalPos];
			int num = this.DetermineCharacterLine(originalPos, this.cachedInputTextGenerator);
			if (num > 0)
			{
				int num2 = this.cachedInputTextGenerator.lines[num].startCharIdx - 1;
				for (int i = this.cachedInputTextGenerator.lines[num - 1].startCharIdx; i < num2; i++)
				{
					if (this.cachedInputTextGenerator.characters[i].cursorPos.x >= uicharInfo.cursorPos.x)
					{
						return i;
					}
				}
				return num2;
			}
			if (!goToFirstChar)
			{
				return originalPos;
			}
			return 0;
		}

		private int LineDownCharacterPosition(int originalPos, bool goToLastChar)
		{
			if (originalPos >= this.cachedInputTextGenerator.characterCountVisible)
			{
				return this.text.Length;
			}
			UICharInfo uicharInfo = this.cachedInputTextGenerator.characters[originalPos];
			int num = this.DetermineCharacterLine(originalPos, this.cachedInputTextGenerator);
			if (num + 1 < this.cachedInputTextGenerator.lineCount)
			{
				int lineEndPosition = InputField.GetLineEndPosition(this.cachedInputTextGenerator, num + 1);
				for (int i = this.cachedInputTextGenerator.lines[num + 1].startCharIdx; i < lineEndPosition; i++)
				{
					if (this.cachedInputTextGenerator.characters[i].cursorPos.x >= uicharInfo.cursorPos.x)
					{
						return i;
					}
				}
				return lineEndPosition;
			}
			if (!goToLastChar)
			{
				return originalPos;
			}
			return this.text.Length;
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
			int num = (this.multiLine ? this.LineDownCharacterPosition(this.caretSelectPositionInternal, goToLastChar) : this.text.Length);
			if (shift)
			{
				this.caretSelectPositionInternal = num;
				return;
			}
			this.caretPositionInternal = (this.caretSelectPositionInternal = num);
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
				return;
			}
			this.caretSelectPositionInternal = (this.caretPositionInternal = num);
		}

		private void Delete()
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (this.caretPositionInternal == this.caretSelectPositionInternal)
			{
				return;
			}
			if (this.caretPositionInternal < this.caretSelectPositionInternal)
			{
				this.m_Text = this.text.Substring(0, this.caretPositionInternal) + this.text.Substring(this.caretSelectPositionInternal, this.text.Length - this.caretSelectPositionInternal);
				this.caretSelectPositionInternal = this.caretPositionInternal;
				return;
			}
			this.m_Text = this.text.Substring(0, this.caretSelectPositionInternal) + this.text.Substring(this.caretPositionInternal, this.text.Length - this.caretPositionInternal);
			this.caretPositionInternal = this.caretSelectPositionInternal;
		}

		private void ForwardSpace()
		{
			if (this.m_ReadOnly)
			{
				return;
			}
			if (this.hasSelection)
			{
				this.Delete();
				this.UpdateTouchKeyboardFromEditChanges();
				this.SendOnValueChangedAndUpdateLabel();
				return;
			}
			if (this.caretPositionInternal < this.text.Length)
			{
				this.m_Text = this.text.Remove(this.caretPositionInternal, 1);
				this.UpdateTouchKeyboardFromEditChanges();
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
				this.Delete();
				this.UpdateTouchKeyboardFromEditChanges();
				this.SendOnValueChangedAndUpdateLabel();
				return;
			}
			if (this.caretPositionInternal > 0)
			{
				this.m_Text = this.text.Remove(this.caretPositionInternal - 1, 1);
				this.caretSelectPositionInternal = --this.caretPositionInternal;
				this.UpdateTouchKeyboardFromEditChanges();
				this.SendOnValueChangedAndUpdateLabel();
			}
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
			this.m_Text = this.text.Insert(this.m_CaretPosition, text);
			this.caretSelectPositionInternal = (this.caretPositionInternal += text.Length);
			this.UpdateTouchKeyboardFromEditChanges();
			this.SendOnValueChanged();
		}

		private void UpdateTouchKeyboardFromEditChanges()
		{
			if (this.m_Keyboard != null && this.InPlaceEditing())
			{
				this.m_Keyboard.text = this.m_Text;
			}
		}

		private void SendOnValueChangedAndUpdateLabel()
		{
			this.SendOnValueChanged();
			this.UpdateLabel();
		}

		private void SendOnValueChanged()
		{
			UISystemProfilerApi.AddMarker("InputField.value", this);
			if (this.onValueChanged != null)
			{
				this.onValueChanged.Invoke(this.text);
			}
		}

		protected void SendOnSubmit()
		{
			UISystemProfilerApi.AddMarker("InputField.onSubmit", this);
			if (this.onEndEdit != null)
			{
				this.onEndEdit.Invoke(this.m_Text);
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
				if (c >= ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\n')
				{
					this.Append(c);
				}
				i++;
			}
		}

		protected virtual void Append(char input)
		{
			if (char.IsSurrogate(input))
			{
				return;
			}
			if (this.m_ReadOnly || this.text.Length >= 16382)
			{
				return;
			}
			if (!this.InPlaceEditing())
			{
				return;
			}
			int num = Math.Min(this.selectionFocusPosition, this.selectionAnchorPosition);
			string text = this.text;
			if (this.selectionFocusPosition != this.selectionAnchorPosition)
			{
				if (this.caretPositionInternal < this.caretSelectPositionInternal)
				{
					text = this.text.Substring(0, this.caretPositionInternal) + this.text.Substring(this.caretSelectPositionInternal, this.text.Length - this.caretSelectPositionInternal);
				}
				else
				{
					text = this.text.Substring(0, this.caretSelectPositionInternal) + this.text.Substring(this.caretPositionInternal, this.text.Length - this.caretPositionInternal);
				}
			}
			if (this.onValidateInput != null)
			{
				input = this.onValidateInput(text, num, input);
			}
			else if (this.characterValidation != InputField.CharacterValidation.None)
			{
				input = this.Validate(text, num, input);
			}
			if (input == '\0')
			{
				return;
			}
			this.Insert(input);
		}

		protected void UpdateLabel()
		{
			if (this.m_TextComponent != null && this.m_TextComponent.font != null && !this.m_PreventFontCallback)
			{
				this.m_PreventFontCallback = true;
				string text;
				if (EventSystem.current != null && base.gameObject == EventSystem.current.currentSelectedGameObject && this.compositionString.Length > 0)
				{
					text = this.text.Substring(0, this.m_CaretPosition) + this.compositionString + this.text.Substring(this.m_CaretPosition);
				}
				else
				{
					text = this.text;
				}
				string text2;
				if (this.inputType == InputField.InputType.Password)
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
				if (!this.m_AllowInput)
				{
					this.m_DrawStart = 0;
					this.m_DrawEnd = this.m_Text.Length;
				}
				if (!flag)
				{
					Vector2 size = this.m_TextComponent.rectTransform.rect.size;
					TextGenerationSettings generationSettings = this.m_TextComponent.GetGenerationSettings(size);
					generationSettings.generateOutOfBounds = true;
					this.cachedInputTextGenerator.PopulateWithErrors(text2, generationSettings, base.gameObject);
					this.SetDrawRangeToContainCaretPosition(this.caretSelectPositionInternal);
					text2 = text2.Substring(this.m_DrawStart, Mathf.Min(this.m_DrawEnd, text2.Length) - this.m_DrawStart);
					this.SetCaretVisible();
				}
				this.m_TextComponent.text = text2;
				this.MarkGeometryAsDirty();
				this.m_PreventFontCallback = false;
			}
		}

		private bool IsSelectionVisible()
		{
			return this.m_DrawStart <= this.caretPositionInternal && this.m_DrawStart <= this.caretSelectPositionInternal && this.m_DrawEnd >= this.caretPositionInternal && this.m_DrawEnd >= this.caretSelectPositionInternal;
		}

		private static int GetLineStartPosition(TextGenerator gen, int line)
		{
			line = Mathf.Clamp(line, 0, gen.lines.Count - 1);
			return gen.lines[line].startCharIdx;
		}

		private static int GetLineEndPosition(TextGenerator gen, int line)
		{
			line = Mathf.Max(line, 0);
			if (line + 1 < gen.lines.Count)
			{
				return gen.lines[line + 1].startCharIdx - 1;
			}
			return gen.characterCountVisible;
		}

		private void SetDrawRangeToContainCaretPosition(int caretPos)
		{
			if (this.cachedInputTextGenerator.lineCount <= 0)
			{
				return;
			}
			Vector2 size = this.cachedInputTextGenerator.rectExtents.size;
			if (!this.multiLine)
			{
				IList<UICharInfo> characters = this.cachedInputTextGenerator.characters;
				if (this.m_DrawEnd > this.cachedInputTextGenerator.characterCountVisible)
				{
					this.m_DrawEnd = this.cachedInputTextGenerator.characterCountVisible;
				}
				float num = 0f;
				if (caretPos > this.m_DrawEnd || (caretPos == this.m_DrawEnd && this.m_DrawStart > 0))
				{
					this.m_DrawEnd = caretPos;
					this.m_DrawStart = this.m_DrawEnd - 1;
					while (this.m_DrawStart >= 0 && num + characters[this.m_DrawStart].charWidth <= size.x)
					{
						num += characters[this.m_DrawStart].charWidth;
						this.m_DrawStart--;
					}
					this.m_DrawStart++;
				}
				else
				{
					if (caretPos < this.m_DrawStart)
					{
						this.m_DrawStart = caretPos;
					}
					this.m_DrawEnd = this.m_DrawStart;
				}
				while (this.m_DrawEnd < this.cachedInputTextGenerator.characterCountVisible)
				{
					num += characters[this.m_DrawEnd].charWidth;
					if (num > size.x)
					{
						break;
					}
					this.m_DrawEnd++;
				}
				return;
			}
			IList<UILineInfo> lines = this.cachedInputTextGenerator.lines;
			int num2 = this.DetermineCharacterLine(caretPos, this.cachedInputTextGenerator);
			if (caretPos > this.m_DrawEnd)
			{
				this.m_DrawEnd = InputField.GetLineEndPosition(this.cachedInputTextGenerator, num2);
				float num3 = lines[num2].topY - (float)lines[num2].height;
				if (num2 == lines.Count - 1)
				{
					num3 += lines[num2].leading;
				}
				int num4 = num2;
				while (num4 > 0 && lines[num4 - 1].topY - num3 <= size.y)
				{
					num4--;
				}
				this.m_DrawStart = InputField.GetLineStartPosition(this.cachedInputTextGenerator, num4);
				return;
			}
			if (caretPos < this.m_DrawStart)
			{
				this.m_DrawStart = InputField.GetLineStartPosition(this.cachedInputTextGenerator, num2);
			}
			int i = this.DetermineCharacterLine(this.m_DrawStart, this.cachedInputTextGenerator);
			int j = i;
			float num5 = lines[i].topY;
			float num6 = lines[j].topY - (float)lines[j].height;
			if (j == lines.Count - 1)
			{
				num6 += lines[j].leading;
			}
			while (j < lines.Count - 1)
			{
				num6 = lines[j + 1].topY - (float)lines[j + 1].height;
				if (j + 1 == lines.Count - 1)
				{
					num6 += lines[j + 1].leading;
				}
				if (num5 - num6 > size.y)
				{
					break;
				}
				j++;
			}
			this.m_DrawEnd = InputField.GetLineEndPosition(this.cachedInputTextGenerator, j);
			while (i > 0)
			{
				num5 = lines[i - 1].topY;
				if (num5 - num6 > size.y)
				{
					break;
				}
				i--;
			}
			this.m_DrawStart = InputField.GetLineStartPosition(this.cachedInputTextGenerator, i);
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
			if (!this.InPlaceEditing() && !this.shouldHideMobileInput)
			{
				return;
			}
			if (this.m_CachedInputRenderer == null && this.m_TextComponent != null)
			{
				GameObject gameObject = new GameObject(base.transform.name + " Input Caret", new Type[]
				{
					typeof(RectTransform),
					typeof(CanvasRenderer)
				});
				gameObject.hideFlags = HideFlags.DontSave;
				gameObject.transform.SetParent(this.m_TextComponent.transform.parent);
				gameObject.transform.SetAsFirstSibling();
				gameObject.layer = base.gameObject.layer;
				this.caretRectTrans = gameObject.GetComponent<RectTransform>();
				this.m_CachedInputRenderer = gameObject.GetComponent<CanvasRenderer>();
				this.m_CachedInputRenderer.SetMaterial(this.m_TextComponent.GetModifiedMaterial(Graphic.defaultGraphicMaterial), Texture2D.whiteTexture);
				gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
				this.AssignPositioningIfNeeded();
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
				if (!this.isFocused)
				{
					vertexHelper.FillMesh(vbo);
				}
				else
				{
					Vector2 vector = this.m_TextComponent.PixelAdjustPoint(Vector2.zero);
					if (!this.hasSelection)
					{
						this.GenerateCaret(vertexHelper, vector);
					}
					else
					{
						this.GenerateHighlight(vertexHelper, vector);
					}
					vertexHelper.FillMesh(vbo);
				}
			}
		}

		private void GenerateCaret(VertexHelper vbo, Vector2 roundingOffset)
		{
			if (!this.m_CaretVisible)
			{
				return;
			}
			if (this.m_CursorVerts == null)
			{
				this.CreateCursorVerts();
			}
			float num = (float)this.m_CaretWidth;
			int num2 = Mathf.Max(0, this.caretPositionInternal - this.m_DrawStart);
			TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
			if (cachedTextGenerator == null)
			{
				return;
			}
			if (cachedTextGenerator.lineCount == 0)
			{
				return;
			}
			Vector2 zero = Vector2.zero;
			if (num2 < cachedTextGenerator.characters.Count)
			{
				UICharInfo uicharInfo = cachedTextGenerator.characters[num2];
				zero.x = uicharInfo.cursorPos.x;
			}
			zero.x /= this.m_TextComponent.pixelsPerUnit;
			if (zero.x > this.m_TextComponent.rectTransform.rect.xMax)
			{
				zero.x = this.m_TextComponent.rectTransform.rect.xMax;
			}
			int num3 = this.DetermineCharacterLine(num2, cachedTextGenerator);
			zero.y = cachedTextGenerator.lines[num3].topY / this.m_TextComponent.pixelsPerUnit;
			float num4 = (float)cachedTextGenerator.lines[num3].height / this.m_TextComponent.pixelsPerUnit;
			for (int i = 0; i < this.m_CursorVerts.Length; i++)
			{
				this.m_CursorVerts[i].color = this.caretColor;
			}
			this.m_CursorVerts[0].position = new Vector3(zero.x, zero.y - num4, 0f);
			this.m_CursorVerts[1].position = new Vector3(zero.x + num, zero.y - num4, 0f);
			this.m_CursorVerts[2].position = new Vector3(zero.x + num, zero.y, 0f);
			this.m_CursorVerts[3].position = new Vector3(zero.x, zero.y, 0f);
			if (roundingOffset != Vector2.zero)
			{
				for (int j = 0; j < this.m_CursorVerts.Length; j++)
				{
					UIVertex uivertex = this.m_CursorVerts[j];
					uivertex.position.x = uivertex.position.x + roundingOffset.x;
					uivertex.position.y = uivertex.position.y + roundingOffset.y;
				}
			}
			vbo.AddUIVertexQuad(this.m_CursorVerts);
			int num5 = Screen.height;
			int targetDisplay = this.m_TextComponent.canvas.targetDisplay;
			if (targetDisplay > 0 && targetDisplay < Display.displays.Length)
			{
				num5 = Display.displays[targetDisplay].renderingHeight;
			}
			Camera camera;
			if (this.m_TextComponent.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				camera = null;
			}
			else
			{
				camera = this.m_TextComponent.canvas.worldCamera;
			}
			Vector3 vector = this.m_CachedInputRenderer.gameObject.transform.TransformPoint(this.m_CursorVerts[0].position);
			Vector2 vector2 = RectTransformUtility.WorldToScreenPoint(camera, vector);
			vector2.y = (float)num5 - vector2.y;
			if (this.input != null)
			{
				this.input.compositionCursorPos = vector2;
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
			int num = Mathf.Max(0, this.caretPositionInternal - this.m_DrawStart);
			int num2 = Mathf.Max(0, this.caretSelectPositionInternal - this.m_DrawStart);
			if (num > num2)
			{
				int num3 = num;
				num = num2;
				num2 = num3;
			}
			num2--;
			TextGenerator cachedTextGenerator = this.m_TextComponent.cachedTextGenerator;
			if (cachedTextGenerator.lineCount <= 0)
			{
				return;
			}
			int num4 = this.DetermineCharacterLine(num, cachedTextGenerator);
			int num5 = InputField.GetLineEndPosition(cachedTextGenerator, num4);
			UIVertex simpleVert = UIVertex.simpleVert;
			simpleVert.uv0 = Vector2.zero;
			simpleVert.color = this.selectionColor;
			int num6 = num;
			while (num6 <= num2 && num6 < cachedTextGenerator.characterCount)
			{
				if (num6 == num5 || num6 == num2)
				{
					UICharInfo uicharInfo = cachedTextGenerator.characters[num];
					UICharInfo uicharInfo2 = cachedTextGenerator.characters[num6];
					Vector2 vector = new Vector2(uicharInfo.cursorPos.x / this.m_TextComponent.pixelsPerUnit, cachedTextGenerator.lines[num4].topY / this.m_TextComponent.pixelsPerUnit);
					Vector2 vector2 = new Vector2((uicharInfo2.cursorPos.x + uicharInfo2.charWidth) / this.m_TextComponent.pixelsPerUnit, vector.y - (float)cachedTextGenerator.lines[num4].height / this.m_TextComponent.pixelsPerUnit);
					if (vector2.x > this.m_TextComponent.rectTransform.rect.xMax || vector2.x < this.m_TextComponent.rectTransform.rect.xMin)
					{
						vector2.x = this.m_TextComponent.rectTransform.rect.xMax;
					}
					int currentVertCount = vbo.currentVertCount;
					simpleVert.position = new Vector3(vector.x, vector2.y, 0f) + roundingOffset;
					vbo.AddVert(simpleVert);
					simpleVert.position = new Vector3(vector2.x, vector2.y, 0f) + roundingOffset;
					vbo.AddVert(simpleVert);
					simpleVert.position = new Vector3(vector2.x, vector.y, 0f) + roundingOffset;
					vbo.AddVert(simpleVert);
					simpleVert.position = new Vector3(vector.x, vector.y, 0f) + roundingOffset;
					vbo.AddVert(simpleVert);
					vbo.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
					vbo.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
					num = num6 + 1;
					num4++;
					num5 = InputField.GetLineEndPosition(cachedTextGenerator, num4);
				}
				num6++;
			}
		}

		protected char Validate(string text, int pos, char ch)
		{
			if (this.characterValidation == InputField.CharacterValidation.None || !base.enabled)
			{
				return ch;
			}
			if (this.characterValidation == InputField.CharacterValidation.Integer || this.characterValidation == InputField.CharacterValidation.Decimal)
			{
				int num = ((pos == 0 && text.Length > 0 && text[0] == '-') ? 1 : 0);
				bool flag = text.Length > 0 && text[0] == '-' && ((this.caretPositionInternal == 0 && this.caretSelectPositionInternal > 0) || (this.caretSelectPositionInternal == 0 && this.caretPositionInternal > 0));
				bool flag2 = this.caretPositionInternal == 0 || this.caretSelectPositionInternal == 0;
				if (num == 0 || flag)
				{
					if (ch >= '0' && ch <= '9')
					{
						return ch;
					}
					if (ch == '-' && (pos == 0 || flag2))
					{
						return ch;
					}
					if ((ch == '.' || ch == ',') && this.characterValidation == InputField.CharacterValidation.Decimal && text.IndexOfAny(new char[] { '.', ',' }) == -1)
					{
						return ch;
					}
				}
			}
			else if (this.characterValidation == InputField.CharacterValidation.Alphanumeric)
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
			else if (this.characterValidation == InputField.CharacterValidation.Name)
			{
				if (char.IsLetter(ch))
				{
					if (char.IsLower(ch) && (pos == 0 || text[pos - 1] == ' '))
					{
						return char.ToUpper(ch);
					}
					if (char.IsUpper(ch) && pos > 0 && text[pos - 1] != ' ' && text[pos - 1] != '\'')
					{
						return char.ToLower(ch);
					}
					return ch;
				}
				else
				{
					if (ch == '\'' && !text.Contains("'") && (pos <= 0 || (text[pos - 1] != ' ' && text[pos - 1] != '\'')) && (pos >= text.Length || (text[pos] != ' ' && text[pos] != '\'')))
					{
						return ch;
					}
					if (ch == ' ' && pos != 0 && (pos <= 0 || (text[pos - 1] != ' ' && text[pos - 1] != '\'')) && (pos >= text.Length || (text[pos] != ' ' && text[pos] != '\'')))
					{
						return ch;
					}
				}
			}
			else if (this.characterValidation == InputField.CharacterValidation.EmailAddress)
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
					int num2 = (int)((text.Length > 0) ? text[Mathf.Clamp(pos, 0, text.Length - 1)] : ' ');
					char c = ((text.Length > 0) ? text[Mathf.Clamp(pos + 1, 0, text.Length - 1)] : '\n');
					if (num2 != 46 && c != '.')
					{
						return ch;
					}
				}
			}
			return '\0';
		}

		public void ActivateInputField()
		{
			if (this.m_TextComponent == null || this.m_TextComponent.font == null || !this.IsActive() || !this.IsInteractable())
			{
				return;
			}
			if (this.isFocused && this.m_Keyboard != null && !this.m_Keyboard.active)
			{
				this.m_Keyboard.active = true;
				this.m_Keyboard.text = this.m_Text;
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
			this.m_TouchKeyboardAllowsInPlaceEditing = TouchScreenKeyboard.isInPlaceEditingAllowed;
			if (this.TouchScreenKeyboardShouldBeUsed())
			{
				if (this.input != null && this.input.touchSupported)
				{
					TouchScreenKeyboard.hideInput = this.shouldHideMobileInput;
				}
				this.m_Keyboard = ((this.inputType == InputField.InputType.Password) ? TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, false, this.multiLine, true, false, "", this.characterLimit) : TouchScreenKeyboard.Open(this.m_Text, this.keyboardType, this.inputType == InputField.InputType.AutoCorrect, this.multiLine, false, false, "", this.characterLimit));
				if (!this.m_TouchKeyboardAllowsInPlaceEditing)
				{
					this.MoveTextEnd(false);
				}
			}
			if (!TouchScreenKeyboard.isSupported || this.m_TouchKeyboardAllowsInPlaceEditing)
			{
				if (this.input != null)
				{
					this.input.imeCompositionMode = IMECompositionMode.On;
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

		public void DeactivateInputField()
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
				if (this.m_WasCanceled)
				{
					this.text = this.m_OriginalText;
				}
				this.SendOnSubmit();
				if (this.m_Keyboard != null)
				{
					this.m_Keyboard.active = false;
					this.m_Keyboard = null;
				}
				this.m_CaretPosition = (this.m_CaretSelectPosition = 0);
				if (this.input != null)
				{
					this.input.imeCompositionMode = IMECompositionMode.Auto;
				}
			}
			this.MarkGeometryAsDirty();
		}

		public override void OnDeselect(BaseEventData eventData)
		{
			this.DeactivateInputField();
			base.OnDeselect(eventData);
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
		}

		private void EnforceContentType()
		{
			switch (this.contentType)
			{
			case InputField.ContentType.Standard:
				this.m_InputType = InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = InputField.CharacterValidation.None;
				break;
			case InputField.ContentType.Autocorrected:
				this.m_InputType = InputField.InputType.AutoCorrect;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = InputField.CharacterValidation.None;
				break;
			case InputField.ContentType.IntegerNumber:
				this.m_LineType = InputField.LineType.SingleLine;
				this.m_InputType = InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				this.m_CharacterValidation = InputField.CharacterValidation.Integer;
				break;
			case InputField.ContentType.DecimalNumber:
				this.m_LineType = InputField.LineType.SingleLine;
				this.m_InputType = InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.NumbersAndPunctuation;
				this.m_CharacterValidation = InputField.CharacterValidation.Decimal;
				break;
			case InputField.ContentType.Alphanumeric:
				this.m_LineType = InputField.LineType.SingleLine;
				this.m_InputType = InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.ASCIICapable;
				this.m_CharacterValidation = InputField.CharacterValidation.Alphanumeric;
				break;
			case InputField.ContentType.Name:
				this.m_LineType = InputField.LineType.SingleLine;
				this.m_InputType = InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.NamePhonePad;
				this.m_CharacterValidation = InputField.CharacterValidation.Name;
				break;
			case InputField.ContentType.EmailAddress:
				this.m_LineType = InputField.LineType.SingleLine;
				this.m_InputType = InputField.InputType.Standard;
				this.m_KeyboardType = TouchScreenKeyboardType.EmailAddress;
				this.m_CharacterValidation = InputField.CharacterValidation.EmailAddress;
				break;
			case InputField.ContentType.Password:
				this.m_LineType = InputField.LineType.SingleLine;
				this.m_InputType = InputField.InputType.Password;
				this.m_KeyboardType = TouchScreenKeyboardType.Default;
				this.m_CharacterValidation = InputField.CharacterValidation.None;
				break;
			case InputField.ContentType.Pin:
				this.m_LineType = InputField.LineType.SingleLine;
				this.m_InputType = InputField.InputType.Password;
				this.m_KeyboardType = TouchScreenKeyboardType.NumberPad;
				this.m_CharacterValidation = InputField.CharacterValidation.Integer;
				break;
			}
			this.EnforceTextHOverflow();
		}

		private void EnforceTextHOverflow()
		{
			if (this.m_TextComponent != null)
			{
				if (this.multiLine)
				{
					this.m_TextComponent.horizontalOverflow = HorizontalWrapMode.Wrap;
					return;
				}
				this.m_TextComponent.horizontalOverflow = HorizontalWrapMode.Overflow;
			}
		}

		private void SetToCustomIfContentTypeIsNot(params InputField.ContentType[] allowedContentTypes)
		{
			if (this.contentType == InputField.ContentType.Custom)
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
			this.contentType = InputField.ContentType.Custom;
		}

		private void SetToCustom()
		{
			if (this.contentType == InputField.ContentType.Custom)
			{
				return;
			}
			this.contentType = InputField.ContentType.Custom;
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
				TextGenerationSettings generationSettings = this.textComponent.GetGenerationSettings(Vector2.zero);
				return this.textComponent.cachedTextGeneratorForLayout.GetPreferredWidth(this.m_Text, generationSettings) / this.textComponent.pixelsPerUnit;
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
				TextGenerationSettings generationSettings = this.textComponent.GetGenerationSettings(new Vector2(this.textComponent.rectTransform.rect.size.x, 0f));
				return this.textComponent.cachedTextGeneratorForLayout.GetPreferredHeight(this.m_Text, generationSettings) / this.textComponent.pixelsPerUnit;
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

		Transform ICanvasElement.get_transform()
		{
			return base.transform;
		}

		protected TouchScreenKeyboard m_Keyboard;

		private static readonly char[] kSeparators = new char[] { ' ', '.', ',', '\t', '\r', '\n' };

		[SerializeField]
		[FormerlySerializedAs("text")]
		protected Text m_TextComponent;

		[SerializeField]
		protected Graphic m_Placeholder;

		[SerializeField]
		private InputField.ContentType m_ContentType;

		[FormerlySerializedAs("inputType")]
		[SerializeField]
		private InputField.InputType m_InputType;

		[FormerlySerializedAs("asteriskChar")]
		[SerializeField]
		private char m_AsteriskChar = '*';

		[FormerlySerializedAs("keyboardType")]
		[SerializeField]
		private TouchScreenKeyboardType m_KeyboardType;

		[SerializeField]
		private InputField.LineType m_LineType;

		[FormerlySerializedAs("hideMobileInput")]
		[SerializeField]
		private bool m_HideMobileInput;

		[FormerlySerializedAs("validation")]
		[SerializeField]
		private InputField.CharacterValidation m_CharacterValidation;

		[FormerlySerializedAs("characterLimit")]
		[SerializeField]
		private int m_CharacterLimit;

		[FormerlySerializedAs("onSubmit")]
		[FormerlySerializedAs("m_OnSubmit")]
		[FormerlySerializedAs("m_EndEdit")]
		[SerializeField]
		private InputField.SubmitEvent m_OnEndEdit = new InputField.SubmitEvent();

		[FormerlySerializedAs("onValueChange")]
		[FormerlySerializedAs("m_OnValueChange")]
		[SerializeField]
		private InputField.OnChangeEvent m_OnValueChanged = new InputField.OnChangeEvent();

		[FormerlySerializedAs("onValidateInput")]
		[SerializeField]
		private InputField.OnValidateInput m_OnValidateInput;

		[FormerlySerializedAs("selectionColor")]
		[SerializeField]
		private Color m_CaretColor = new Color(0.19607843f, 0.19607843f, 0.19607843f, 1f);

		[SerializeField]
		private bool m_CustomCaretColor;

		[SerializeField]
		private Color m_SelectionColor = new Color(0.65882355f, 0.80784315f, 1f, 0.7529412f);

		[SerializeField]
		[FormerlySerializedAs("mValue")]
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
		private bool m_ShouldActivateOnSelect = true;

		protected int m_CaretPosition;

		protected int m_CaretSelectPosition;

		private RectTransform caretRectTrans;

		protected UIVertex[] m_CursorVerts;

		private TextGenerator m_InputTextCache;

		private CanvasRenderer m_CachedInputRenderer;

		private bool m_PreventFontCallback;

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

		protected int m_DrawStart;

		protected int m_DrawEnd;

		private Coroutine m_DragCoroutine;

		private string m_OriginalText = "";

		private bool m_WasCanceled;

		private bool m_HasDoneFocusTransition;

		private WaitForSecondsRealtime m_WaitForSecondsRealtime;

		private bool m_TouchKeyboardAllowsInPlaceEditing;

		private const string kEmailSpecialCharacters = "!#$%&'*+-/=?^_`{|}~";

		private Event m_ProcessingEvent = new Event();

		private const int k_MaxTextLength = 16382;

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
			Integer,
			Decimal,
			Alphanumeric,
			Name,
			EmailAddress
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

		protected enum EditState
		{
			Continue,
			Finish
		}
	}
}
