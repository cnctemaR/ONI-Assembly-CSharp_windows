using System;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;

namespace UnityEngine.UIElements
{
	public class TextElement : BindableElement, ITextElement, INotifyValueChanged<string>, ITextEdition, ITextElementExperimentalFeatures, IExperimentalFeatures, ITextSelection
	{
		public TextElement()
		{
			base.requireMeasureFunction = true;
			base.tabIndex = -1;
			this.uitkTextHandle = new UITKTextHandle(this);
			base.AddToClassList(TextElement.ussClassName);
			base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(this.OnGenerateVisualContent));
			base.RegisterCallback<GeometryChangedEvent>(new EventCallback<GeometryChangedEvent>(this.OnGeometryChanged), TrickleDown.NoTrickleDown);
		}

		internal UITKTextHandle uitkTextHandle { get; set; }

		private void OnGeometryChanged(GeometryChangedEvent e)
		{
			this.UpdateVisibleText();
		}

		public virtual string text
		{
			get
			{
				return ((INotifyValueChanged<string>)this).value;
			}
			set
			{
				((INotifyValueChanged<string>)this).value = value;
			}
		}

		public bool enableRichText
		{
			get
			{
				return this.m_EnableRichText;
			}
			set
			{
				bool flag = this.m_EnableRichText == value;
				if (!flag)
				{
					this.m_EnableRichText = value;
					base.MarkDirtyRepaint();
				}
			}
		}

		public bool parseEscapeSequences
		{
			get
			{
				return this.m_ParseEscapeSequences;
			}
			set
			{
				bool flag = this.m_ParseEscapeSequences == value;
				if (!flag)
				{
					this.m_ParseEscapeSequences = value;
					base.MarkDirtyRepaint();
				}
			}
		}

		public bool displayTooltipWhenElided
		{
			get
			{
				return this.m_DisplayTooltipWhenElided;
			}
			set
			{
				bool flag = this.m_DisplayTooltipWhenElided != value;
				if (flag)
				{
					this.m_DisplayTooltipWhenElided = value;
					this.UpdateVisibleText();
					base.MarkDirtyRepaint();
				}
			}
		}

		public bool isElided { get; private set; }

		internal void OnGenerateVisualContent(MeshGenerationContext mgc)
		{
			this.UpdateVisibleText();
			mgc.Text(this);
			bool flag = this.ShouldElide() && this.uitkTextHandle.TextLibraryCanElide();
			if (flag)
			{
				this.isElided = this.uitkTextHandle.IsElided();
			}
			this.UpdateTooltip();
			bool flag2 = this.selection.HasSelection() && this.selectingManipulator.HasFocus();
			if (flag2)
			{
				this.DrawHighlighting(mgc);
			}
			else
			{
				bool flag3 = !this.edition.isReadOnly && this.selection.isSelectable && this.selectingManipulator.RevealCursor();
				if (flag3)
				{
					this.DrawCaret(mgc);
				}
			}
		}

		internal string ElideText(string drawText, string ellipsisText, float width, TextOverflowPosition textOverflowPosition)
		{
			float num = base.resolvedStyle.paddingRight;
			bool flag = float.IsNaN(num);
			if (flag)
			{
				num = 0f;
			}
			float num2 = Mathf.Clamp(num, 1f / base.scaledPixelsPerPoint, 1f);
			Vector2 vector = this.MeasureTextSize(drawText, 0f, VisualElement.MeasureMode.Undefined, 0f, VisualElement.MeasureMode.Undefined);
			bool flag2 = vector.x <= width + num2 || string.IsNullOrEmpty(ellipsisText);
			string text;
			if (flag2)
			{
				text = drawText;
			}
			else
			{
				string text2 = ((drawText.Length > 1) ? ellipsisText : drawText);
				Vector2 vector2 = this.MeasureTextSize(text2, 0f, VisualElement.MeasureMode.Undefined, 0f, VisualElement.MeasureMode.Undefined);
				bool flag3 = vector2.x >= width;
				if (flag3)
				{
					text = text2;
				}
				else
				{
					int num3 = drawText.Length - 1;
					int num4 = -1;
					string text3 = drawText;
					int i = ((textOverflowPosition == TextOverflowPosition.Start) ? 1 : 0);
					int num5 = ((textOverflowPosition == TextOverflowPosition.Start || textOverflowPosition == TextOverflowPosition.Middle) ? num3 : (num3 - 1));
					int num6 = (i + num5) / 2;
					while (i <= num5)
					{
						bool flag4 = textOverflowPosition == TextOverflowPosition.Start;
						if (flag4)
						{
							text3 = ellipsisText + drawText.Substring(num6, num3 - (num6 - 1));
						}
						else
						{
							bool flag5 = textOverflowPosition == TextOverflowPosition.End;
							if (flag5)
							{
								text3 = drawText.Substring(0, num6) + ellipsisText;
							}
							else
							{
								bool flag6 = textOverflowPosition == TextOverflowPosition.Middle;
								if (flag6)
								{
									text3 = ((num6 - 1 <= 0) ? "" : drawText.Substring(0, num6 - 1)) + ellipsisText + ((num3 - (num6 - 1) <= 0) ? "" : drawText.Substring(num3 - (num6 - 1)));
								}
							}
						}
						vector = this.MeasureTextSize(text3, 0f, VisualElement.MeasureMode.Undefined, 0f, VisualElement.MeasureMode.Undefined);
						bool flag7 = Math.Abs(vector.x - width) < 1E-30f;
						if (flag7)
						{
							return text3;
						}
						bool flag8 = textOverflowPosition == TextOverflowPosition.Start;
						if (flag8)
						{
							bool flag9 = vector.x > width;
							if (flag9)
							{
								bool flag10 = num4 == num6 - 1;
								if (flag10)
								{
									return ellipsisText + drawText.Substring(num4, num3 - (num4 - 1));
								}
								i = num6 + 1;
							}
							else
							{
								num5 = num6 - 1;
								num4 = num6;
							}
						}
						else
						{
							bool flag11 = textOverflowPosition == TextOverflowPosition.End || textOverflowPosition == TextOverflowPosition.Middle;
							if (flag11)
							{
								bool flag12 = vector.x > width;
								if (flag12)
								{
									bool flag13 = num4 == num6 - 1;
									if (flag13)
									{
										bool flag14 = textOverflowPosition == TextOverflowPosition.End;
										if (flag14)
										{
											return drawText.Substring(0, num4) + ellipsisText;
										}
										return drawText.Substring(0, Mathf.Max(num4 - 1, 0)) + ellipsisText + drawText.Substring(num3 - Mathf.Max(num4 - 1, 0));
									}
									else
									{
										num5 = num6 - 1;
									}
								}
								else
								{
									i = num6 + 1;
									num4 = num6;
								}
							}
						}
						num6 = (i + num5) / 2;
					}
					text = text3;
				}
			}
			return text;
		}

		private void UpdateTooltip()
		{
			bool flag = this.displayTooltipWhenElided && this.isElided;
			bool flag2 = flag;
			if (flag2)
			{
				base.tooltip = this.text;
				this.m_WasElided = true;
			}
			else
			{
				bool wasElided = this.m_WasElided;
				if (wasElided)
				{
					base.tooltip = null;
					this.m_WasElided = false;
				}
			}
		}

		private void UpdateVisibleText()
		{
			bool flag = this.ShouldElide();
			bool flag2 = flag && this.uitkTextHandle.TextLibraryCanElide();
			if (!flag2)
			{
				bool flag3 = flag;
				if (flag3)
				{
					this.elidedText = this.ElideText(this.text, TextElement.k_EllipsisText, base.contentRect.width, base.computedStyle.unityTextOverflowPosition);
					this.isElided = flag && this.elidedText != this.text;
				}
				else
				{
					this.isElided = false;
				}
			}
		}

		private bool ShouldElide()
		{
			return base.computedStyle.textOverflow == TextOverflow.Ellipsis && base.computedStyle.overflow == OverflowInternal.Hidden;
		}

		internal bool hasFocus
		{
			get
			{
				bool flag;
				if (base.elementPanel != null)
				{
					FocusController focusController = base.elementPanel.focusController;
					flag = ((focusController != null) ? focusController.GetLeafFocusedElement() : null) == this;
				}
				else
				{
					flag = false;
				}
				return flag;
			}
		}

		public Vector2 MeasureTextSize(string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode)
		{
			return TextUtilities.MeasureVisualElementTextSize(this, textToMeasure, width, widthMode, height, heightMode);
		}

		protected internal override Vector2 DoMeasure(float desiredWidth, VisualElement.MeasureMode widthMode, float desiredHeight, VisualElement.MeasureMode heightMode)
		{
			return this.MeasureTextSize(this.renderedText, desiredWidth, widthMode, desiredHeight, heightMode);
		}

		string INotifyValueChanged<string>.value
		{
			get
			{
				return this.m_Text ?? string.Empty;
			}
			set
			{
				bool flag = this.m_Text != value;
				if (flag)
				{
					bool flag2 = base.panel != null;
					if (flag2)
					{
						using (ChangeEvent<string> pooled = ChangeEvent<string>.GetPooled(this.text, value))
						{
							pooled.target = this;
							((INotifyValueChanged<string>)this).SetValueWithoutNotify(value);
							this.SendEvent(pooled);
						}
					}
					else
					{
						((INotifyValueChanged<string>)this).SetValueWithoutNotify(value);
					}
				}
			}
		}

		void INotifyValueChanged<string>.SetValueWithoutNotify(string newValue)
		{
			newValue = ((ITextEdition)this).CullString(newValue);
			bool flag = this.m_Text != newValue;
			if (flag)
			{
				this.renderedText = newValue;
				this.m_Text = newValue;
				base.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
				bool flag2 = !string.IsNullOrEmpty(base.viewDataKey);
				if (flag2)
				{
					base.SaveViewData();
				}
			}
			bool flag3 = this.editingManipulator != null;
			if (flag3)
			{
				this.editingManipulator.editingUtilities.text = newValue;
			}
		}

		internal ITextEdition edition
		{
			get
			{
				return this;
			}
		}

		bool ITextEdition.multiline
		{
			get
			{
				return this.m_Multiline;
			}
			set
			{
				bool flag = value != this.m_Multiline;
				if (flag)
				{
					bool flag2 = !this.edition.isReadOnly;
					if (flag2)
					{
						this.editingManipulator.editingUtilities.multiline = value;
					}
					this.m_Multiline = value;
				}
			}
		}

		TouchScreenKeyboard ITextEdition.touchScreenKeyboard
		{
			get
			{
				return this.m_TouchScreenKeyboard;
			}
		}

		TouchScreenKeyboardType ITextEdition.keyboardType
		{
			get
			{
				return this.m_KeyboardType;
			}
			set
			{
				this.m_KeyboardType = value;
			}
		}

		bool ITextEdition.hideMobileInput
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				RuntimePlatform runtimePlatform = platform;
				if (runtimePlatform <= RuntimePlatform.Android)
				{
					if (runtimePlatform != RuntimePlatform.IPhonePlayer && runtimePlatform != RuntimePlatform.Android)
					{
						goto IL_0032;
					}
				}
				else if (runtimePlatform != RuntimePlatform.WebGLPlayer && runtimePlatform != RuntimePlatform.tvOS)
				{
					goto IL_0032;
				}
				return this.m_HideMobileInput;
				IL_0032:
				return true;
			}
			set
			{
				RuntimePlatform platform = Application.platform;
				RuntimePlatform runtimePlatform = platform;
				if (runtimePlatform <= RuntimePlatform.Android)
				{
					if (runtimePlatform != RuntimePlatform.IPhonePlayer && runtimePlatform != RuntimePlatform.Android)
					{
						goto IL_0032;
					}
				}
				else if (runtimePlatform != RuntimePlatform.WebGLPlayer && runtimePlatform != RuntimePlatform.tvOS)
				{
					goto IL_0032;
				}
				this.m_HideMobileInput = value;
				return;
				IL_0032:
				this.m_HideMobileInput = true;
			}
		}

		bool ITextEdition.isReadOnly
		{
			get
			{
				return this.m_IsReadOnly || !base.enabledInHierarchy;
			}
			set
			{
				bool flag = value == this.m_IsReadOnly;
				if (!flag)
				{
					this.editingManipulator = (value ? null : new TextEditingManipulator(this));
					this.m_IsReadOnly = value;
				}
			}
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

		private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
		{
			bool flag = ((evt != null) ? evt.target : null) is TextElement;
			if (flag)
			{
				bool flag2 = !this.edition.isReadOnly;
				if (flag2)
				{
					evt.menu.AppendAction("Cut", new Action<DropdownMenuAction>(this.Cut), new Func<DropdownMenuAction, DropdownMenuAction.Status>(this.CutActionStatus), null);
					evt.menu.AppendAction("Copy", new Action<DropdownMenuAction>(this.Copy), new Func<DropdownMenuAction, DropdownMenuAction.Status>(this.CopyActionStatus), null);
					evt.menu.AppendAction("Paste", new Action<DropdownMenuAction>(this.Paste), new Func<DropdownMenuAction, DropdownMenuAction.Status>(this.PasteActionStatus), null);
				}
				else
				{
					evt.menu.AppendAction("Copy", new Action<DropdownMenuAction>(this.Copy), new Func<DropdownMenuAction, DropdownMenuAction.Status>(this.CopyActionStatus), null);
				}
			}
		}

		private DropdownMenuAction.Status CutActionStatus(DropdownMenuAction a)
		{
			return (base.enabledInHierarchy && this.selection.HasSelection() && !this.edition.isPassword) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
		}

		private DropdownMenuAction.Status CopyActionStatus(DropdownMenuAction a)
		{
			return ((!base.enabledInHierarchy || this.selection.HasSelection()) && !this.edition.isPassword) ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
		}

		private DropdownMenuAction.Status PasteActionStatus(DropdownMenuAction a)
		{
			bool flag = this.editingManipulator.editingUtilities.CanPaste();
			return base.enabledInHierarchy ? (flag ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled) : DropdownMenuAction.Status.Hidden;
		}

		[EventInterest(new Type[]
		{
			typeof(ContextualMenuPopulateEvent),
			typeof(FocusInEvent),
			typeof(FocusOutEvent),
			typeof(KeyDownEvent),
			typeof(KeyUpEvent),
			typeof(FocusEvent),
			typeof(BlurEvent),
			typeof(ValidateCommandEvent),
			typeof(ExecuteCommandEvent),
			typeof(PointerDownEvent),
			typeof(PointerUpEvent),
			typeof(PointerMoveEvent),
			typeof(NavigationMoveEvent),
			typeof(NavigationSubmitEvent),
			typeof(NavigationCancelEvent)
		})]
		protected override void ExecuteDefaultActionAtTarget(EventBase evt)
		{
			bool isSelectable = this.selection.isSelectable;
			if (isSelectable)
			{
				TextEditingManipulator textEditingManipulator = this.editingManipulator;
				bool flag = textEditingManipulator != null && textEditingManipulator.editingUtilities.TouchScreenKeyboardShouldBeUsed();
				bool flag2 = !flag || (flag && this.edition.hideMobileInput);
				if (flag2)
				{
					TextSelectingManipulator selectingManipulator = this.selectingManipulator;
					if (selectingManipulator != null)
					{
						selectingManipulator.ExecuteDefaultActionAtTarget(evt);
					}
				}
				bool flag3 = !this.edition.isReadOnly;
				if (flag3)
				{
					TextEditingManipulator textEditingManipulator2 = this.editingManipulator;
					if (textEditingManipulator2 != null)
					{
						textEditingManipulator2.ExecuteDefaultActionAtTarget(evt);
					}
				}
				BaseVisualElementPanel elementPanel = base.elementPanel;
				if (elementPanel != null)
				{
					ContextualMenuManager contextualMenuManager = elementPanel.contextualMenuManager;
					if (contextualMenuManager != null)
					{
						contextualMenuManager.DisplayMenuIfEventMatches(evt, this);
					}
				}
				long? num = ((evt != null) ? new long?(evt.eventTypeId) : null);
				long num2 = EventBase<ContextualMenuPopulateEvent>.TypeId();
				bool flag4 = (num.GetValueOrDefault() == num2) & (num != null);
				if (flag4)
				{
					ContextualMenuPopulateEvent contextualMenuPopulateEvent = evt as ContextualMenuPopulateEvent;
					int count = contextualMenuPopulateEvent.menu.MenuItems().Count;
					this.BuildContextualMenu(contextualMenuPopulateEvent);
					bool flag5 = count > 0 && contextualMenuPopulateEvent.menu.MenuItems().Count > count;
					if (flag5)
					{
						contextualMenuPopulateEvent.menu.InsertSeparator(null, count);
					}
				}
			}
		}

		int ITextEdition.maxLength
		{
			get
			{
				return this.m_MaxLength;
			}
			set
			{
				this.m_MaxLength = value;
				this.text = this.edition.CullString(this.text);
			}
		}

		bool ITextEdition.isDelayed { get; set; }

		void ITextEdition.ResetValueAndText()
		{
			this.m_OriginalText = (this.text = null);
		}

		void ITextEdition.SaveValueAndText()
		{
			this.m_OriginalText = this.text;
		}

		void ITextEdition.RestoreValueAndText()
		{
			this.text = this.m_OriginalText;
		}

		Func<char, bool> ITextEdition.AcceptCharacter { get; set; }

		Action<bool> ITextEdition.UpdateScrollOffset { get; set; }

		Action ITextEdition.UpdateValueFromText { get; set; }

		Action ITextEdition.UpdateTextFromValue { get; set; }

		Action ITextEdition.MoveFocusToCompositeRoot { get; set; }

		void ITextEdition.UpdateText(string value)
		{
			bool flag = this.m_TouchScreenKeyboard != null && this.m_TouchScreenKeyboard.text != value;
			if (flag)
			{
				this.m_TouchScreenKeyboard.text = value;
			}
			bool flag2 = this.text != value;
			if (flag2)
			{
				using (InputEvent pooled = InputEvent.GetPooled(this.text, value))
				{
					pooled.target = base.parent;
					((INotifyValueChanged<string>)this).SetValueWithoutNotify(value);
					VisualElement parent = base.parent;
					if (parent != null)
					{
						parent.SendEvent(pooled);
					}
				}
			}
		}

		string ITextEdition.CullString(string s)
		{
			int maxLength = this.edition.maxLength;
			bool flag = maxLength >= 0 && s != null && s.Length > maxLength;
			string text;
			if (flag)
			{
				text = s.Substring(0, maxLength);
			}
			else
			{
				text = s;
			}
			return text;
		}

		char ITextEdition.maskChar
		{
			get
			{
				return this.m_MaskChar;
			}
			set
			{
				bool flag = this.m_MaskChar != value;
				if (flag)
				{
					this.m_MaskChar = value;
					bool isPassword = this.edition.isPassword;
					if (isPassword)
					{
						base.IncrementVersion(VersionChangeType.Repaint);
					}
				}
			}
		}

		private char effectiveMaskChar
		{
			get
			{
				return this.edition.isPassword ? this.m_MaskChar : '\0';
			}
		}

		bool ITextEdition.isPassword
		{
			get
			{
				return this.m_IsPassword;
			}
			set
			{
				bool flag = this.m_IsPassword != value;
				if (flag)
				{
					this.m_IsPassword = value;
					base.IncrementVersion(VersionChangeType.Repaint);
				}
			}
		}

		bool ITextEdition.autoCorrection
		{
			get
			{
				return this.m_AutoCorrection;
			}
			set
			{
				this.m_AutoCorrection = value;
			}
		}

		internal string renderedText
		{
			get
			{
				bool flag = this.effectiveMaskChar > '\0';
				string text;
				if (flag)
				{
					text = "".PadLeft(this.text.Length, this.effectiveMaskChar) + "\u200b";
				}
				else
				{
					text = (string.IsNullOrEmpty(this.m_RenderedText) ? "\u200b" : this.m_RenderedText);
				}
				return text;
			}
			set
			{
				this.m_RenderedText = value + "\u200b";
			}
		}

		internal string originalText
		{
			get
			{
				return this.m_OriginalText;
			}
		}

		public new ITextElementExperimentalFeatures experimental
		{
			get
			{
				return this;
			}
		}

		void ITextElementExperimentalFeatures.SetRenderedText(string renderedText)
		{
			this.renderedText = renderedText;
		}

		public ITextSelection selection
		{
			get
			{
				return this;
			}
		}

		bool ITextSelection.isSelectable
		{
			get
			{
				return this.m_IsSelectable && base.focusable;
			}
			set
			{
				bool flag = value == this.m_IsSelectable;
				if (!flag)
				{
					base.focusable = value;
					this.m_IsSelectable = value;
				}
			}
		}

		int ITextSelection.cursorIndex
		{
			get
			{
				return this.selection.isSelectable ? this.selectingManipulator.cursorIndex : (-1);
			}
			set
			{
				bool isSelectable = this.selection.isSelectable;
				if (isSelectable)
				{
					this.selectingManipulator.cursorIndex = value;
				}
			}
		}

		int ITextSelection.selectIndex
		{
			get
			{
				return this.selection.isSelectable ? this.selectingManipulator.selectIndex : (-1);
			}
			set
			{
				bool isSelectable = this.selection.isSelectable;
				if (isSelectable)
				{
					this.selectingManipulator.selectIndex = value;
				}
			}
		}

		void ITextSelection.SelectAll()
		{
			bool isSelectable = this.selection.isSelectable;
			if (isSelectable)
			{
				this.selectingManipulator.m_SelectingUtilities.SelectAll();
			}
		}

		void ITextSelection.SelectNone()
		{
			bool isSelectable = this.selection.isSelectable;
			if (isSelectable)
			{
				this.selectingManipulator.m_SelectingUtilities.SelectNone();
			}
		}

		void ITextSelection.SelectRange(int cursorIndex, int selectionIndex)
		{
			bool isSelectable = this.selection.isSelectable;
			if (isSelectable)
			{
				this.selectingManipulator.m_SelectingUtilities.cursorIndex = cursorIndex;
				this.selectingManipulator.m_SelectingUtilities.selectIndex = selectionIndex;
			}
		}

		bool ITextSelection.HasSelection()
		{
			return this.selection.isSelectable && this.selectingManipulator.HasSelection();
		}

		bool ITextSelection.doubleClickSelectsWord { get; set; } = true;

		bool ITextSelection.tripleClickSelectsLine { get; set; } = true;

		bool ITextSelection.selectAllOnFocus { get; set; } = false;

		bool ITextSelection.selectAllOnMouseUp { get; set; } = false;

		Vector2 ITextSelection.cursorPosition
		{
			get
			{
				return this.uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(this.selection.cursorIndex, false, true) + base.contentRect.min;
			}
		}

		float ITextSelection.lineHeightAtCursorPosition
		{
			get
			{
				return this.uitkTextHandle.GetLineHeightFromCharacterIndex(this.selection.cursorIndex);
			}
		}

		void ITextSelection.MoveTextEnd()
		{
			bool isSelectable = this.selection.isSelectable;
			if (isSelectable)
			{
				this.selectingManipulator.m_SelectingUtilities.MoveTextEnd();
			}
		}

		Color ITextSelection.selectionColor
		{
			get
			{
				return this.m_SelectionColor;
			}
			set
			{
				bool flag = this.m_SelectionColor == value;
				if (!flag)
				{
					this.m_SelectionColor = value;
					base.MarkDirtyRepaint();
				}
			}
		}

		Color ITextSelection.cursorColor
		{
			get
			{
				return this.m_CursorColor;
			}
			set
			{
				bool flag = this.m_CursorColor == value;
				if (!flag)
				{
					this.m_CursorColor = value;
					base.MarkDirtyRepaint();
				}
			}
		}

		private Color cursorColor
		{
			get
			{
				return this.selection.cursorColor;
			}
			set
			{
				this.selection.cursorColor = value;
			}
		}

		float ITextSelection.cursorWidth
		{
			get
			{
				return this.m_CursorWidth;
			}
			set
			{
				bool flag = Mathf.Approximately(this.m_CursorWidth, value);
				if (!flag)
				{
					this.m_CursorWidth = value;
					base.MarkDirtyRepaint();
				}
			}
		}

		internal TextSelectingManipulator selectingManipulator
		{
			get
			{
				TextSelectingManipulator textSelectingManipulator;
				if ((textSelectingManipulator = this.m_SelectingManipulator) == null)
				{
					textSelectingManipulator = (this.m_SelectingManipulator = new TextSelectingManipulator(this));
				}
				return textSelectingManipulator;
			}
		}

		private void DrawHighlighting(MeshGenerationContext mgc)
		{
			Color color = ((base.panel.contextType == ContextType.Editor) ? UIElementsUtility.editorPlayModeTintColor : Color.white);
			int num = Math.Min(this.selection.cursorIndex, this.selection.selectIndex);
			int num2 = Math.Max(this.selection.cursorIndex, this.selection.selectIndex);
			Vector2 vector = this.uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(num, false, true);
			Vector2 vector2 = this.uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(num2, false, true);
			int lineNumber = this.uitkTextHandle.GetLineNumber(num);
			int lineNumber2 = this.uitkTextHandle.GetLineNumber(num2);
			float lineHeight = this.uitkTextHandle.GetLineHeight(lineNumber);
			Vector2 min = base.contentRect.min;
			bool flag = this.m_TouchScreenKeyboard != null && this.m_HideMobileInput;
			if (flag)
			{
				TextInfo textInfo = this.uitkTextHandle.textInfo;
				int num3 = ((this.selection.selectIndex < this.selection.cursorIndex) ? textInfo.textElementInfo[this.selection.selectIndex].index : textInfo.textElementInfo[this.selection.cursorIndex].index);
				int num4 = ((this.selection.selectIndex < this.selection.cursorIndex) ? (this.selection.cursorIndex - num3) : (this.selection.selectIndex - num3));
				this.m_TouchScreenKeyboard.selection = new RangeInt(num3, num4);
			}
			bool flag2 = lineNumber == lineNumber2;
			if (flag2)
			{
				vector += min;
				vector2 += min;
				mgc.Rectangle(new MeshGenerationContextUtils.RectangleParams
				{
					rect = new Rect(vector.x, vector.y - lineHeight, vector2.x - vector.x, lineHeight),
					color = this.selection.selectionColor,
					playmodeTintColor = color
				});
			}
			else
			{
				for (int i = lineNumber; i <= lineNumber2; i++)
				{
					bool flag3 = i == lineNumber;
					if (flag3)
					{
						int num5 = this.GetLastCharacterAt(i);
						vector2 = this.uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(num5, true, true);
					}
					else
					{
						bool flag4 = i == lineNumber2;
						if (flag4)
						{
							int num6 = this.uitkTextHandle.textInfo.lineInfo[i].firstCharacterIndex;
							vector = this.uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(num6, false, true);
							vector2 = this.uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(num2, true, true);
						}
						else
						{
							bool flag5 = i != lineNumber && i != lineNumber2;
							if (flag5)
							{
								int num6 = this.uitkTextHandle.textInfo.lineInfo[i].firstCharacterIndex;
								vector = this.uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(num6, false, true);
								int num5 = this.GetLastCharacterAt(i);
								vector2 = this.uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(num5, true, true);
							}
						}
					}
					vector += min;
					vector2 += min;
					mgc.Rectangle(new MeshGenerationContextUtils.RectangleParams
					{
						rect = new Rect(vector.x, vector.y - lineHeight, vector2.x - vector.x, lineHeight),
						color = this.selection.selectionColor,
						playmodeTintColor = color
					});
				}
			}
		}

		internal void DrawCaret(MeshGenerationContext mgc)
		{
			Color color = ((base.panel.contextType == ContextType.Editor) ? UIElementsUtility.editorPlayModeTintColor : Color.white);
			float characterHeightFromIndex = this.uitkTextHandle.GetCharacterHeightFromIndex(this.selection.cursorIndex);
			float num = AlignmentUtils.CeilToPixelGrid(this.selection.cursorWidth, base.scaledPixelsPerPoint, -0.02f);
			mgc.Rectangle(new MeshGenerationContextUtils.RectangleParams
			{
				rect = new Rect(this.selection.cursorPosition.x, this.selection.cursorPosition.y - characterHeightFromIndex, num, characterHeightFromIndex),
				color = this.selection.cursorColor,
				playmodeTintColor = color
			});
		}

		private int GetLastCharacterAt(int lineIndex)
		{
			int num = this.uitkTextHandle.textInfo.lineInfo[lineIndex].lastCharacterIndex;
			int firstCharacterIndex = this.uitkTextHandle.textInfo.lineInfo[lineIndex].firstCharacterIndex;
			TextElementInfo textElementInfo = this.uitkTextHandle.textInfo.textElementInfo[num];
			while (textElementInfo.character == '\n' || textElementInfo.character == '\r')
			{
				bool flag = num > firstCharacterIndex;
				if (!flag)
				{
					break;
				}
				num--;
				textElementInfo = this.uitkTextHandle.textInfo.textElementInfo[num];
			}
			return num;
		}

		public static readonly string ussClassName = "unity-text-element";

		private string m_Text = string.Empty;

		private bool m_EnableRichText = true;

		private bool m_ParseEscapeSequences = true;

		private bool m_DisplayTooltipWhenElided = true;

		internal static readonly string k_EllipsisText = "...";

		internal string elidedText;

		private bool m_WasElided;

		internal TextEditingManipulator editingManipulator;

		private bool m_Multiline;

		internal TouchScreenKeyboard m_TouchScreenKeyboard;

		internal TouchScreenKeyboardType m_KeyboardType = TouchScreenKeyboardType.Default;

		private bool m_HideMobileInput;

		private bool m_IsReadOnly = true;

		private int m_MaxLength = -1;

		private string m_RenderedText;

		private string m_OriginalText;

		private char m_MaskChar;

		private bool m_IsPassword;

		private bool m_AutoCorrection;

		private TextSelectingManipulator m_SelectingManipulator;

		private bool m_IsSelectable;

		private Color m_SelectionColor = new Color(0.239f, 0.502f, 0.875f, 0.65f);

		private Color m_CursorColor = new Color(0.706f, 0.706f, 0.706f, 1f);

		private float m_CursorWidth = 1f;

		public new class UxmlFactory : UxmlFactory<TextElement, TextElement.UxmlTraits>
		{
		}

		public new class UxmlTraits : BindableElement.UxmlTraits
		{
			public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
			{
				get
				{
					yield break;
				}
			}

			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				TextElement textElement = (TextElement)ve;
				textElement.text = this.m_Text.GetValueFromBag(bag, cc);
				textElement.enableRichText = this.m_EnableRichText.GetValueFromBag(bag, cc);
				textElement.parseEscapeSequences = this.m_ParseEscapeSequences.GetValueFromBag(bag, cc);
				textElement.displayTooltipWhenElided = this.m_DisplayTooltipWhenElided.GetValueFromBag(bag, cc);
			}

			private UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
			{
				name = "text"
			};

			private UxmlBoolAttributeDescription m_EnableRichText = new UxmlBoolAttributeDescription
			{
				name = "enable-rich-text",
				defaultValue = true
			};

			private UxmlBoolAttributeDescription m_ParseEscapeSequences = new UxmlBoolAttributeDescription
			{
				name = "parse-escape-sequences",
				defaultValue = false
			};

			private UxmlBoolAttributeDescription m_DisplayTooltipWhenElided = new UxmlBoolAttributeDescription
			{
				name = "display-tooltip-when-elided"
			};
		}
	}
}
