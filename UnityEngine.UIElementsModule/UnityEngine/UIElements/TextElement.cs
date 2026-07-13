using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Serialization;
using UnityEngine.TextCore;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements.UIR;

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
			base.generateVisualContent = (Action<MeshGenerationContext>)Delegate.Combine(base.generateVisualContent, new Action<MeshGenerationContext>(TextElement.OnGenerateVisualContent));
			this.edition.GetDefaultValueType = new Func<string>(this.GetDefaultValueType);
		}

		private string GetDefaultValueType()
		{
			return string.Empty;
		}

		public Action<TextElement.GlyphsEnumerable> PostProcessTextVertices { get; set; }

		internal UITKTextHandle uitkTextHandle { get; set; }

		[EventInterest(new Type[]
		{
			typeof(ContextualMenuPopulateEvent),
			typeof(KeyDownEvent),
			typeof(KeyUpEvent),
			typeof(ValidateCommandEvent),
			typeof(ExecuteCommandEvent),
			typeof(FocusEvent),
			typeof(BlurEvent),
			typeof(FocusInEvent),
			typeof(FocusOutEvent),
			typeof(PointerDownEvent),
			typeof(PointerUpEvent),
			typeof(PointerMoveEvent),
			typeof(NavigationMoveEvent),
			typeof(NavigationSubmitEvent),
			typeof(NavigationCancelEvent),
			typeof(IMEEvent),
			typeof(GeometryChangedEvent),
			typeof(AttachToPanelEvent),
			typeof(DetachFromPanelEvent)
		})]
		protected override void HandleEventBubbleUp(EventBase evt)
		{
			base.HandleEventBubbleUp(evt);
			bool flag = evt.target == this;
			if (flag)
			{
				if (evt is GeometryChangedEvent)
				{
					this.UpdateVisibleText();
					return;
				}
				AttachToPanelEvent attachToPanelEvent = evt as AttachToPanelEvent;
				if (attachToPanelEvent != null)
				{
					this.OnAttachToPanel(attachToPanelEvent);
					return;
				}
				DetachFromPanelEvent detachFromPanelEvent = evt as DetachFromPanelEvent;
				if (detachFromPanelEvent != null)
				{
					this.OnDetachFromPanel(detachFromPanelEvent);
					return;
				}
			}
			bool isSelectable = this.selection.isSelectable;
			if (isSelectable)
			{
				this.EditionHandleEvent(evt);
			}
		}

		private void OnAttachToPanel(AttachToPanelEvent attachEvent)
		{
			bool flag = TextUtilities.IsAdvancedTextEnabledForPanel(attachEvent.destinationPanel);
			if (flag)
			{
				BaseVisualElementPanel baseVisualElementPanel = attachEvent.destinationPanel as BaseVisualElementPanel;
				if (baseVisualElementPanel != null)
				{
					baseVisualElementPanel.textElementRegistry.Value.Add(this);
				}
			}
		}

		private void OnDetachFromPanel(DetachFromPanelEvent detachEvent)
		{
			this.uitkTextHandle.RemoveFromPermanentCache();
			this.uitkTextHandle.RemoveFromTemporaryCache();
			BaseVisualElementPanel baseVisualElementPanel = detachEvent.originPanel as BaseVisualElementPanel;
			Lazy<HashSet<TextElement>> lazy = ((baseVisualElementPanel != null) ? baseVisualElementPanel.textElementRegistry : null);
			bool flag = lazy != null && lazy.IsValueCreated;
			if (flag)
			{
				lazy.Value.Remove(this);
			}
		}

		[CreateProperty]
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

		[CreateProperty]
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
					base.NotifyPropertyChanged(in TextElement.enableRichTextProperty);
				}
			}
		}

		[CreateProperty]
		public bool emojiFallbackSupport
		{
			get
			{
				return this.m_EmojiFallbackSupport;
			}
			set
			{
				bool flag = this.m_EmojiFallbackSupport == value;
				if (!flag)
				{
					this.m_EmojiFallbackSupport = value;
					base.MarkDirtyRepaint();
					base.NotifyPropertyChanged(in TextElement.emojiFallbackSupportProperty);
				}
			}
		}

		[CreateProperty]
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
					base.NotifyPropertyChanged(in TextElement.parseEscapeSequencesProperty);
				}
			}
		}

		[CreateProperty]
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
					base.NotifyPropertyChanged(in TextElement.displayTooltipWhenElidedProperty);
				}
			}
		}

		[CreateProperty(ReadOnly = true)]
		public bool isElided { get; private set; }

		internal static void OnGenerateVisualContent(MeshGenerationContext mgc)
		{
			TextElement textElement = mgc.visualElement as TextElement;
			bool flag = textElement != null;
			if (flag)
			{
				textElement.UpdateVisibleText();
				bool flag2 = TextUtilities.IsFontAssigned(textElement);
				if (flag2)
				{
					textElement.uitkTextHandle.ReleaseResourcesIfPossible();
					mgc.meshGenerator.textJobSystem.GenerateText(mgc, textElement);
				}
			}
		}

		internal void OnGenerateTextOver(MeshGenerationContext mgc)
		{
			bool flag = this.selection.HasSelection() && this.selectingManipulator.HasFocus();
			if (flag)
			{
				this.DrawHighlighting(mgc);
			}
			else
			{
				bool flag2 = !this.edition.isReadOnly && this.selection.isSelectable && this.selectingManipulator.RevealCursor();
				if (flag2)
				{
					this.DrawCaret(mgc);
				}
			}
			bool flag3 = this.ShouldElide() && this.uitkTextHandle.TextLibraryCanElide();
			if (flag3)
			{
				this.isElided = this.uitkTextHandle.IsElided();
			}
			this.UpdateTooltip();
		}

		internal void OnGenerateTextOverNative(MeshGenerationContext mgc)
		{
			bool flag = this.selection.HasSelection() && this.selectingManipulator.HasFocus();
			if (flag)
			{
				this.DrawNativeHighlighting(mgc);
			}
			else
			{
				bool flag2 = !this.edition.isReadOnly && this.selection.isSelectable && this.selectingManipulator.RevealCursor();
				if (flag2)
				{
					this.DrawCaret(mgc);
				}
			}
			bool flag3 = this.ShouldElide() && this.uitkTextHandle.TextLibraryCanElide();
			if (flag3)
			{
				this.isElided = this.uitkTextHandle.IsElided();
			}
			this.UpdateTooltip();
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
			Vector2 vector = this.MeasureTextSize(drawText, float.NaN, VisualElement.MeasureMode.Undefined, float.NaN, VisualElement.MeasureMode.Undefined);
			bool flag2 = vector.x <= width + num2 || string.IsNullOrEmpty(ellipsisText);
			string text;
			if (flag2)
			{
				text = drawText;
			}
			else
			{
				string text2 = ((drawText.Length > 1) ? ellipsisText : drawText);
				Vector2 vector2 = this.MeasureTextSize(text2, float.NaN, VisualElement.MeasureMode.Undefined, float.NaN, VisualElement.MeasureMode.Undefined);
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
						vector = this.MeasureTextSize(text3, float.NaN, VisualElement.MeasureMode.Undefined, float.NaN, VisualElement.MeasureMode.Undefined);
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
					this.isElided = flag && !string.Equals(this.elidedText, this.text, StringComparison.Ordinal);
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
			return TextUtilities.MeasureVisualElementTextSize(this, textToMeasure, width, widthMode, height, heightMode, null);
		}

		public Vector2 MeasureTextSize(string textToMeasure, float width, VisualElement.MeasureMode widthMode, float height, VisualElement.MeasureMode heightMode, float? fontsize = null)
		{
			return TextUtilities.MeasureVisualElementTextSize(this, textToMeasure, width, widthMode, height, heightMode, fontsize);
		}

		protected internal override Vector2 DoMeasure(float desiredWidth, VisualElement.MeasureMode widthMode, float desiredHeight, VisualElement.MeasureMode heightMode)
		{
			bool flag = TextUtilities.IsAdvancedTextEnabledForElement(this);
			Vector2 vector;
			if (flag)
			{
				vector = TextUtilities.MeasureVisualElementTextSize(this, this.renderedTextString, desiredWidth, widthMode, desiredHeight, heightMode, null);
			}
			else
			{
				RenderedText renderedText = this.renderedText;
				vector = TextUtilities.MeasureVisualElementTextSize(this, in renderedText, desiredWidth, widthMode, desiredHeight, heightMode, null);
			}
			return vector;
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
							pooled.elementTarget = this;
							((INotifyValueChanged<string>)this).SetValueWithoutNotify(value);
							this.SendEvent(pooled);
							base.NotifyPropertyChanged(in TextElement.valueProperty);
							base.NotifyPropertyChanged(in TextElement.textProperty);
						}
					}
					else
					{
						((INotifyValueChanged<string>)this).SetValueWithoutNotify(value);
					}
				}
			}
		}

		[CreateProperty]
		private string value
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

		internal static bool AnySizeAutoOrNone(ComputedStyle computedStyle)
		{
			return computedStyle.height.IsAuto() || computedStyle.height.IsNone() || computedStyle.width.IsAuto() || computedStyle.width.IsNone();
		}

		unsafe void INotifyValueChanged<string>.SetValueWithoutNotify(string newValue)
		{
			newValue = ((ITextEdition)this).CullString(newValue);
			bool flag = this.m_Text != newValue;
			if (flag)
			{
				this.SetRenderedText(newValue);
				this.m_Text = newValue;
				bool flag2 = TextElement.AnySizeAutoOrNone(*base.computedStyle);
				if (flag2)
				{
					base.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
				}
				else
				{
					base.IncrementVersion(VersionChangeType.Repaint);
				}
				bool flag3 = !string.IsNullOrEmpty(base.viewDataKey);
				if (flag3)
				{
					base.SaveViewData();
				}
			}
			bool flag4 = this.editingManipulator != null;
			if (flag4)
			{
				this.editingManipulator.editingUtilities.text = newValue;
			}
		}

		public void MarkDirtyText()
		{
			base.IncrementVersion(VersionChangeType.Layout | VersionChangeType.Repaint);
			this.uitkTextHandle.SetDirty();
		}

		internal ITextEdition edition
		{
			get
			{
				return this;
			}
		}

		internal TextEditingManipulator editingManipulator { get; private set; }

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
				bool flag = this.m_KeyboardType == value;
				if (!flag)
				{
					this.m_KeyboardType = value;
					base.NotifyPropertyChanged(in TextElement.keyboardTypeProperty);
				}
			}
		}

		[CreateProperty]
		private TouchScreenKeyboardType keyboardType
		{
			get
			{
				return this.edition.keyboardType;
			}
			set
			{
				this.edition.keyboardType = value;
			}
		}

		bool ITextEdition.hideMobileInput
		{
			get
			{
				TouchScreenKeyboard.InputFieldAppearance inputFieldAppearance = TouchScreenKeyboard.inputFieldAppearance;
				if (!true)
				{
				}
				bool flag = inputFieldAppearance != TouchScreenKeyboard.InputFieldAppearance.AlwaysVisible && (inputFieldAppearance == TouchScreenKeyboard.InputFieldAppearance.AlwaysHidden || this.m_HideMobileInput);
				if (!true)
				{
				}
				return flag;
			}
			set
			{
				bool flag = TouchScreenKeyboard.inputFieldAppearance > TouchScreenKeyboard.InputFieldAppearance.Customizable;
				if (!flag)
				{
					bool flag2 = this.m_HideMobileInput == value;
					if (!flag2)
					{
						this.m_HideMobileInput = value;
						base.NotifyPropertyChanged(in TextElement.hideMobileInputProperty);
					}
				}
			}
		}

		[CreateProperty]
		private bool hideMobileInput
		{
			get
			{
				return this.edition.hideMobileInput;
			}
			set
			{
				this.edition.hideMobileInput = value;
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
					TextEditingManipulator editingManipulator = this.editingManipulator;
					if (editingManipulator != null)
					{
						editingManipulator.Reset();
					}
					this.editingManipulator = (value ? null : new TextEditingManipulator(this));
					this.m_IsReadOnly = value;
					Action<bool> action = this.onIsReadOnlyChanged;
					if (action != null)
					{
						action(value);
					}
					base.NotifyPropertyChanged(in TextElement.isReadOnlyProperty);
				}
			}
		}

		[CreateProperty]
		private bool isReadOnly
		{
			get
			{
				return this.edition.isReadOnly;
			}
			set
			{
				this.edition.isReadOnly = value;
			}
		}

		private void ProcessMenuCommand(string command)
		{
			this.Focus();
			using (ExecuteCommandEvent pooled = CommandEventBase<ExecuteCommandEvent>.GetPooled(command))
			{
				pooled.elementTarget = this;
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

		private void EditionHandleEvent(EventBase evt)
		{
			TextEditingManipulator editingManipulator = this.editingManipulator;
			bool flag = editingManipulator != null && editingManipulator.editingUtilities.TouchScreenKeyboardShouldBeUsed();
			bool flag2 = !flag || this.edition.hideMobileInput;
			if (flag2)
			{
				TextSelectingManipulator selectingManipulator = this.selectingManipulator;
				if (selectingManipulator != null)
				{
					selectingManipulator.HandleEventBubbleUp(evt);
				}
			}
			bool flag3 = !this.edition.isReadOnly;
			if (flag3)
			{
				TextEditingManipulator editingManipulator2 = this.editingManipulator;
				if (editingManipulator2 != null)
				{
					editingManipulator2.HandleEventBubbleUp(evt);
				}
			}
			BaseVisualElementPanel elementPanel = base.elementPanel;
			bool flag4;
			if (elementPanel == null)
			{
				flag4 = false;
			}
			else
			{
				ContextualMenuManager contextualMenuManager = elementPanel.contextualMenuManager;
				bool? flag5 = ((contextualMenuManager != null) ? new bool?(contextualMenuManager.CheckIfEventMatches(evt)) : null);
				bool flag6 = true;
				flag4 = (flag5.GetValueOrDefault() == flag6) & (flag5 != null);
			}
			bool flag7 = flag4;
			if (flag7)
			{
				bool flag8 = evt.eventTypeId == EventBase<PointerDownEvent>.TypeId() && !this.focusController.IsFocused(this);
				if (flag8)
				{
					long evtTimestamp = evt.timestamp;
					base.RegisterCallbackOnce<FocusEvent>(delegate(FocusEvent _)
					{
						bool flag11 = evt.timestamp != evtTimestamp;
						if (!flag11)
						{
							DropdownMenu dropdownMenu = new DropdownMenu
							{
								repaintPanelBeforeDisplay = true
							};
							BaseVisualElementPanel elementPanel2 = this.elementPanel;
							if (elementPanel2 != null)
							{
								ContextualMenuManager contextualMenuManager2 = elementPanel2.contextualMenuManager;
								if (contextualMenuManager2 != null)
								{
									contextualMenuManager2.DisplayMenu(evt, this, dropdownMenu);
								}
							}
						}
					}, TrickleDown.NoTrickleDown);
				}
				else
				{
					base.elementPanel.contextualMenuManager.DisplayMenu(evt, this);
				}
				evt.StopPropagation();
			}
			bool flag9 = evt.eventTypeId == EventBase<ContextualMenuPopulateEvent>.TypeId();
			if (flag9)
			{
				ContextualMenuPopulateEvent contextualMenuPopulateEvent = evt as ContextualMenuPopulateEvent;
				int count = contextualMenuPopulateEvent.menu.MenuItems().Count;
				this.BuildContextualMenu(contextualMenuPopulateEvent);
				bool flag10 = count > 0 && contextualMenuPopulateEvent.menu.MenuItems().Count > count;
				if (flag10)
				{
					contextualMenuPopulateEvent.menu.InsertSeparator(null, count);
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
				bool flag = this.m_MaxLength == value;
				if (!flag)
				{
					this.m_MaxLength = value;
					this.text = this.edition.CullString(this.text);
					base.NotifyPropertyChanged(in TextElement.maxLengthProperty);
				}
			}
		}

		[CreateProperty]
		private int maxLength
		{
			get
			{
				return this.edition.maxLength;
			}
			set
			{
				this.edition.maxLength = value;
			}
		}

		string ITextEdition.placeholder
		{
			get
			{
				return this.m_PlaceholderText;
			}
			set
			{
				bool flag = value == this.m_PlaceholderText;
				if (!flag)
				{
					bool flag2 = !string.IsNullOrEmpty(value) && (this.text == null || this.text.Equals(this.edition.GetDefaultValueType()));
					if (flag2)
					{
						this.text = "";
					}
					this.m_PlaceholderText = value;
					Action onPlaceholderChanged = this.OnPlaceholderChanged;
					if (onPlaceholderChanged != null)
					{
						onPlaceholderChanged();
					}
					base.MarkDirtyRepaint();
				}
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

		internal Action OnPlaceholderChanged { get; set; }

		Func<string> ITextEdition.GetDefaultValueType { get; set; }

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
					pooled.elementTarget = base.parent;
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
					base.NotifyPropertyChanged(in TextElement.maskCharProperty);
				}
			}
		}

		[CreateProperty]
		private char maskChar
		{
			get
			{
				return this.edition.maskChar;
			}
			set
			{
				this.edition.maskChar = value;
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
					base.NotifyPropertyChanged(in TextElement.isPasswordProperty);
				}
			}
		}

		[CreateProperty]
		private bool isPassword
		{
			get
			{
				return this.edition.isPassword;
			}
			set
			{
				this.edition.isPassword = value;
			}
		}

		bool ITextEdition.hidePlaceholderOnFocus
		{
			get
			{
				return this.m_HidePlaceholderTextOnFocus;
			}
			set
			{
				this.m_HidePlaceholderTextOnFocus = value;
			}
		}

		internal bool needsPlaceholderIfTextIsEmpty
		{
			get
			{
				bool flag = this.m_PlaceholderText.Length > 0;
				bool flag2 = this.edition.hidePlaceholderOnFocus && this.hasFocus;
				return flag && !flag2;
			}
		}

		internal bool showPlaceholderText
		{
			get
			{
				bool flag = !this.needsPlaceholderIfTextIsEmpty;
				bool flag2;
				if (flag)
				{
					flag2 = false;
				}
				else
				{
					bool flag3 = string.IsNullOrEmpty(this.text);
					flag2 = flag3;
				}
				return flag2;
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
				bool flag = this.m_AutoCorrection == value;
				if (!flag)
				{
					this.m_AutoCorrection = value;
					base.NotifyPropertyChanged(in TextElement.autoCorrectionProperty);
				}
			}
		}

		[CreateProperty]
		private bool autoCorrection
		{
			get
			{
				return this.edition.autoCorrection;
			}
			set
			{
				this.edition.autoCorrection = value;
			}
		}

		internal RenderedText renderedText
		{
			get
			{
				bool showPlaceholderText = this.showPlaceholderText;
				RenderedText renderedText;
				if (showPlaceholderText)
				{
					renderedText = (TextUtilities.IsAdvancedTextEnabledForElement(this) ? new RenderedText(this.m_PlaceholderText) : new RenderedText(this.m_PlaceholderText, "\u200b"));
				}
				else
				{
					bool flag = this.effectiveMaskChar > '\0';
					if (flag)
					{
						RenderedText renderedText3;
						if (!TextUtilities.IsAdvancedTextEnabledForElement(this))
						{
							char effectiveMaskChar = this.effectiveMaskChar;
							string renderedText2 = this.m_RenderedText;
							renderedText3 = new RenderedText(effectiveMaskChar, (renderedText2 != null) ? renderedText2.Length : 0, "\u200b");
						}
						else
						{
							char effectiveMaskChar2 = this.effectiveMaskChar;
							string renderedText4 = this.m_RenderedText;
							renderedText3 = new RenderedText(effectiveMaskChar2, (renderedText4 != null) ? renderedText4.Length : 0, null);
						}
						renderedText = renderedText3;
					}
					else
					{
						bool flag2 = !TextUtilities.IsAdvancedTextEnabledForElement(this) && (!this.isReadOnly || ((base.pseudoStates & PseudoStates.Disabled) != PseudoStates.None && this.isSelectable));
						if (flag2)
						{
							renderedText = new RenderedText(this.m_RenderedText, "\u200b");
						}
						else
						{
							renderedText = new RenderedText(this.m_RenderedText);
						}
					}
				}
				return renderedText;
			}
		}

		internal string renderedTextString
		{
			get
			{
				bool showPlaceholderText = this.showPlaceholderText;
				string text;
				if (showPlaceholderText)
				{
					text = this.m_PlaceholderText;
				}
				else
				{
					bool flag = this.effectiveMaskChar > '\0';
					if (flag)
					{
						string text2 = "";
						string renderedText = this.m_RenderedText;
						text = text2.PadLeft((renderedText != null) ? renderedText.Length : 0, this.effectiveMaskChar);
					}
					else
					{
						text = this.m_RenderedText;
					}
				}
				return text;
			}
		}

		private void SetRenderedText(string value)
		{
			this.m_RenderedText = value;
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
			this.SetRenderedText(renderedText);
		}

		[CreateProperty(ReadOnly = true)]
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
				return this.m_IsSelectable && this.focusable;
			}
			set
			{
				bool flag = value == this.m_IsSelectable;
				if (!flag)
				{
					this.focusable = value;
					this.m_IsSelectable = value;
					base.EnableInClassList(TextElement.selectableUssClassName, value);
					base.NotifyPropertyChanged(in TextElement.isSelectableProperty);
				}
			}
		}

		[CreateProperty]
		internal bool isSelectable
		{
			get
			{
				return this.selection.isSelectable;
			}
			set
			{
				this.selection.isSelectable = value;
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
				int cursorIndex = this.selection.cursorIndex;
				bool isSelectable = this.selection.isSelectable;
				if (isSelectable)
				{
					this.selectingManipulator.cursorIndex = value;
				}
				bool flag = cursorIndex != this.selection.cursorIndex;
				if (flag)
				{
					base.NotifyPropertyChanged(in TextElement.cursorIndexProperty);
				}
			}
		}

		[CreateProperty]
		private int cursorIndex
		{
			get
			{
				return this.selection.cursorIndex;
			}
			set
			{
				this.selection.cursorIndex = value;
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
				int selectIndex = this.selection.selectIndex;
				bool isSelectable = this.selection.isSelectable;
				if (isSelectable)
				{
					this.selectingManipulator.selectIndex = value;
				}
				bool flag = selectIndex != this.selection.selectIndex;
				if (flag)
				{
					base.NotifyPropertyChanged(in TextElement.selectIndexProperty);
				}
			}
		}

		[CreateProperty]
		private int selectIndex
		{
			get
			{
				return this.selection.selectIndex;
			}
			set
			{
				this.selection.selectIndex = value;
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
				bool flag = this.m_TouchScreenKeyboard != null;
				if (flag)
				{
					this.m_TouchScreenKeyboard.selection = new RangeInt(Mathf.Min(cursorIndex, selectionIndex), Mathf.Abs(selectionIndex - cursorIndex));
				}
			}
		}

		bool ITextSelection.HasSelection()
		{
			return this.selection.isSelectable && this.selectingManipulator.HasSelection();
		}

		bool ITextSelection.doubleClickSelectsWord
		{
			get
			{
				return this.m_DoubleClickSelectsWord;
			}
			set
			{
				bool flag = this.m_DoubleClickSelectsWord == value;
				if (!flag)
				{
					this.m_DoubleClickSelectsWord = value;
					base.NotifyPropertyChanged(in TextElement.doubleClickSelectsWordProperty);
				}
			}
		}

		[CreateProperty]
		internal bool doubleClickSelectsWord
		{
			get
			{
				return this.selection.doubleClickSelectsWord;
			}
			set
			{
				this.selection.doubleClickSelectsWord = value;
			}
		}

		bool ITextSelection.tripleClickSelectsLine
		{
			get
			{
				return this.m_TripleClickSelectsLine;
			}
			set
			{
				bool flag = this.m_TripleClickSelectsLine == value;
				if (!flag)
				{
					this.m_TripleClickSelectsLine = value;
					base.NotifyPropertyChanged(in TextElement.tripleClickSelectsLineProperty);
				}
			}
		}

		[CreateProperty]
		internal bool tripleClickSelectsLine
		{
			get
			{
				return this.selection.tripleClickSelectsLine;
			}
			set
			{
				this.selection.tripleClickSelectsLine = value;
			}
		}

		bool ITextSelection.selectAllOnFocus
		{
			get
			{
				return this.m_SelectAllOnFocus;
			}
			set
			{
				bool flag = this.m_SelectAllOnFocus == value;
				if (!flag)
				{
					this.m_SelectAllOnFocus = value;
					base.NotifyPropertyChanged(in TextElement.selectAllOnFocusProperty);
				}
			}
		}

		[CreateProperty]
		private bool selectAllOnFocus
		{
			get
			{
				return this.selection.selectAllOnFocus;
			}
			set
			{
				this.selection.selectAllOnFocus = value;
			}
		}

		bool ITextSelection.selectAllOnMouseUp
		{
			get
			{
				return this.m_SelectAllOnMouseUp;
			}
			set
			{
				bool flag = this.m_SelectAllOnMouseUp == value;
				if (!flag)
				{
					this.m_SelectAllOnMouseUp = value;
					base.NotifyPropertyChanged(in TextElement.selectAllOnMouseUpProperty);
				}
			}
		}

		[CreateProperty]
		private bool selectAllOnMouseUp
		{
			get
			{
				return this.selection.selectAllOnMouseUp;
			}
			set
			{
				this.selection.selectAllOnMouseUp = value;
			}
		}

		Vector2 ITextSelection.cursorPosition
		{
			get
			{
				this.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
				return this.uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(this.selection.cursorIndex, false, true) + base.contentRect.min;
			}
		}

		[CreateProperty(ReadOnly = true)]
		private Vector2 cursorPosition
		{
			get
			{
				return this.selection.cursorPosition;
			}
		}

		float ITextSelection.lineHeightAtCursorPosition
		{
			get
			{
				this.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
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

		Vector2 ITextSelection.GetCursorPositionFromStringIndex(int stringIndex)
		{
			this.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
			return this.uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(stringIndex, false, true) + base.contentRect.min;
		}

		void ITextSelection.MoveForward()
		{
			this.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
			bool flag = !TextUtilities.IsAdvancedTextEnabledForElement(this);
			if (flag)
			{
				this.uitkTextHandle.ComputeSettingsAndUpdate();
			}
			this.selectingManipulator.m_SelectingUtilities.MoveRight();
		}

		void ITextSelection.MoveBackward()
		{
			this.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
			bool flag = !TextUtilities.IsAdvancedTextEnabledForElement(this);
			if (flag)
			{
				this.uitkTextHandle.ComputeSettingsAndUpdate();
			}
			this.selectingManipulator.m_SelectingUtilities.MoveLeft();
		}

		void ITextSelection.MoveToParagraphEnd()
		{
			this.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
			bool flag = !TextUtilities.IsAdvancedTextEnabledForElement(this);
			if (flag)
			{
				this.uitkTextHandle.ComputeSettingsAndUpdate();
			}
			this.selectingManipulator.m_SelectingUtilities.MoveLineEnd();
		}

		void ITextSelection.MoveToParagraphStart()
		{
			this.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
			bool flag = !TextUtilities.IsAdvancedTextEnabledForElement(this);
			if (flag)
			{
				this.uitkTextHandle.ComputeSettingsAndUpdate();
			}
			this.selectingManipulator.m_SelectingUtilities.MoveLineStart();
		}

		void ITextSelection.MoveToEndOfPreviousWord()
		{
			this.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
			bool flag = !TextUtilities.IsAdvancedTextEnabledForElement(this);
			if (flag)
			{
				this.uitkTextHandle.ComputeSettingsAndUpdate();
			}
			this.selectingManipulator.m_SelectingUtilities.MoveToEndOfPreviousWord();
		}

		void ITextSelection.MoveToStartOfNextWord()
		{
			this.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
			bool flag = !TextUtilities.IsAdvancedTextEnabledForElement(this);
			if (flag)
			{
				this.uitkTextHandle.ComputeSettingsAndUpdate();
			}
			this.selectingManipulator.m_SelectingUtilities.MoveToStartOfNextWord();
		}

		void ITextSelection.MoveWordBackward()
		{
			this.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
			bool flag = !TextUtilities.IsAdvancedTextEnabledForElement(this);
			if (flag)
			{
				this.uitkTextHandle.ComputeSettingsAndUpdate();
			}
			this.selectingManipulator.m_SelectingUtilities.MoveWordLeft();
		}

		void ITextSelection.MoveWordForward()
		{
			this.uitkTextHandle.AddToPermanentCacheAndGenerateMesh();
			bool flag = !TextUtilities.IsAdvancedTextEnabledForElement(this);
			if (flag)
			{
				this.uitkTextHandle.ComputeSettingsAndUpdate();
			}
			this.selectingManipulator.m_SelectingUtilities.MoveWordRight();
		}

		event Action ITextSelection.OnCursorIndexChange
		{
			add
			{
				TextSelectingUtilities selectingUtilities = this.selectingManipulator.m_SelectingUtilities;
				selectingUtilities.OnCursorIndexChange = (Action)Delegate.Combine(selectingUtilities.OnCursorIndexChange, value);
			}
			remove
			{
				TextSelectingUtilities selectingUtilities = this.selectingManipulator.m_SelectingUtilities;
				selectingUtilities.OnCursorIndexChange = (Action)Delegate.Remove(selectingUtilities.OnCursorIndexChange, value);
			}
		}

		event Action ITextSelection.OnSelectIndexChange
		{
			add
			{
				TextSelectingUtilities selectingUtilities = this.selectingManipulator.m_SelectingUtilities;
				selectingUtilities.OnSelectIndexChange = (Action)Delegate.Combine(selectingUtilities.OnSelectIndexChange, value);
			}
			remove
			{
				TextSelectingUtilities selectingUtilities = this.selectingManipulator.m_SelectingUtilities;
				selectingUtilities.OnSelectIndexChange = (Action)Delegate.Remove(selectingUtilities.OnSelectIndexChange, value);
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

		internal Color selectionColor
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

		internal Color cursorColor
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
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
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
			VisualElement visualElement = mgc.visualElement;
			Color color = ((visualElement != null) ? visualElement.playModeTintColor : Color.white);
			int num = Math.Min(this.selection.cursorIndex, this.selection.selectIndex);
			int num2 = Math.Max(this.selection.cursorIndex, this.selection.selectIndex);
			Vector2 vector = this.uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(num, false, true);
			Vector2 vector2 = this.uitkTextHandle.GetCursorPositionFromStringIndexUsingLineHeight(num2, false, true);
			int lineNumber = this.uitkTextHandle.GetLineNumber(num);
			int lineNumber2 = this.uitkTextHandle.GetLineNumber(num2);
			float lineHeight = this.uitkTextHandle.GetLineHeight(lineNumber);
			Vector2 min = base.contentRect.min;
			bool flag = this.m_TouchScreenKeyboard != null && this.hideMobileInput;
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
				mgc.meshGenerator.DrawRectangle(new MeshGenerator.RectangleParams
				{
					rect = new Rect(vector.x, vector.y - lineHeight, vector2.x - vector.x, lineHeight),
					color = this.selectionColor,
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
					mgc.meshGenerator.DrawRectangle(new MeshGenerator.RectangleParams
					{
						rect = new Rect(vector.x, vector.y - lineHeight, vector2.x - vector.x, lineHeight),
						color = this.selectionColor,
						playmodeTintColor = color
					});
				}
			}
		}

		private void DrawNativeHighlighting(MeshGenerationContext mgc)
		{
			VisualElement visualElement = mgc.visualElement;
			Color color = ((visualElement != null) ? visualElement.playModeTintColor : Color.white);
			int num = Math.Min(this.selection.cursorIndex, this.selection.selectIndex);
			int num2 = Math.Max(this.selection.cursorIndex, this.selection.selectIndex);
			Rect[] highlightRectangles = this.uitkTextHandle.GetHighlightRectangles(num, num2);
			for (int i = 0; i < highlightRectangles.Length; i++)
			{
				mgc.meshGenerator.DrawRectangle(new MeshGenerator.RectangleParams
				{
					rect = new Rect(highlightRectangles[i].position + base.contentRect.min, highlightRectangles[i].size),
					color = this.selectionColor,
					playmodeTintColor = color
				});
			}
			bool flag = this.m_TouchScreenKeyboard != null && this.hideMobileInput;
			if (flag)
			{
				this.m_TouchScreenKeyboard.selection = new RangeInt(num, num2 - num);
			}
		}

		internal void DrawCaret(MeshGenerationContext mgc)
		{
			VisualElement visualElement = mgc.visualElement;
			Color color = ((visualElement != null) ? visualElement.playModeTintColor : Color.white);
			float characterHeightFromIndex = this.uitkTextHandle.GetCharacterHeightFromIndex(this.selection.cursorIndex);
			float num = AlignmentUtils.CeilToPixelGrid(this.selection.cursorWidth, base.scaledPixelsPerPoint, -0.02f);
			mgc.meshGenerator.DrawRectangle(new MeshGenerator.RectangleParams
			{
				rect = new Rect(this.selection.cursorPosition.x, this.selection.cursorPosition.y - characterHeightFromIndex, num, characterHeightFromIndex),
				color = this.cursorColor,
				playmodeTintColor = color
			});
		}

		private int GetLastCharacterAt(int lineIndex)
		{
			int num = this.uitkTextHandle.textInfo.lineInfo[lineIndex].lastCharacterIndex;
			int firstCharacterIndex = this.uitkTextHandle.textInfo.lineInfo[lineIndex].firstCharacterIndex;
			TextElementInfo textElementInfo = this.uitkTextHandle.textInfo.textElementInfo[num];
			for (;;)
			{
				uint character = textElementInfo.character;
				if ((character != 10U && character != 13U) || num <= firstCharacterIndex)
				{
					break;
				}
				textElementInfo = this.uitkTextHandle.textInfo.textElementInfo[--num];
			}
			return num;
		}

		internal static readonly BindingId displayTooltipWhenElidedProperty = "displayTooltipWhenElided";

		internal static readonly BindingId emojiFallbackSupportProperty = "emojiFallbackSupport";

		internal static readonly BindingId enableRichTextProperty = "enableRichText";

		internal static readonly BindingId isElidedProperty = "isElided";

		internal static readonly BindingId parseEscapeSequencesProperty = "parseEscapeSequences";

		internal static readonly BindingId textProperty = "text";

		internal static readonly BindingId valueProperty = "value";

		public static readonly string ussClassName = "unity-text-element";

		public static readonly string selectableUssClassName = TextElement.ussClassName + "__selectable";

		private string m_Text = string.Empty;

		private bool m_EnableRichText = true;

		private bool m_EmojiFallbackSupport = true;

		private bool m_ParseEscapeSequences;

		private bool m_DisplayTooltipWhenElided = true;

		internal static readonly string k_EllipsisText = "...";

		internal string elidedText;

		private bool m_WasElided;

		internal static readonly BindingId autoCorrectionProperty = "autoCorrection";

		internal static readonly BindingId hideMobileInputProperty = "hideMobileInput";

		internal static readonly BindingId keyboardTypeProperty = "keyboardType";

		internal static readonly BindingId isReadOnlyProperty = "isReadOnly";

		internal static readonly BindingId isPasswordProperty = "isPassword";

		internal static readonly BindingId maxLengthProperty = "maxLength";

		internal static readonly BindingId maskCharProperty = "maskChar";

		internal bool isInputField = false;

		private bool m_Multiline;

		internal TouchScreenKeyboard m_TouchScreenKeyboard;

		internal Action<bool> onIsReadOnlyChanged;

		internal TouchScreenKeyboardType m_KeyboardType = TouchScreenKeyboardType.Default;

		private bool m_HideMobileInput;

		private bool m_IsReadOnly = true;

		private int m_MaxLength = -1;

		private string m_PlaceholderText = "";

		private const string ZeroWidthSpace = "\u200b";

		private string m_RenderedText;

		private string m_OriginalText;

		private char m_MaskChar;

		private bool m_IsPassword;

		private bool m_HidePlaceholderTextOnFocus;

		private bool m_AutoCorrection;

		internal static readonly BindingId isSelectableProperty = "isSelectable";

		internal static readonly BindingId cursorIndexProperty = "cursorIndex";

		internal static readonly BindingId selectIndexProperty = "selectIndex";

		internal static readonly BindingId doubleClickSelectsWordProperty = "doubleClickSelectsWord";

		internal static readonly BindingId tripleClickSelectsLineProperty = "tripleClickSelectsLine";

		internal static readonly BindingId cursorPositionProperty = "cursorPosition";

		internal static readonly BindingId selectAllOnFocusProperty = "selectAllOnFocus";

		internal static readonly BindingId selectAllOnMouseUpProperty = "selectAllOnMouseUp";

		internal static readonly BindingId selectionProperty = "selection";

		private TextSelectingManipulator m_SelectingManipulator;

		private bool m_IsSelectable;

		private bool m_DoubleClickSelectsWord = true;

		private bool m_TripleClickSelectsLine = true;

		private bool m_SelectAllOnFocus = false;

		private bool m_SelectAllOnMouseUp = false;

		private Color m_SelectionColor = new Color(0.239f, 0.502f, 0.875f, 0.65f);

		private Color m_CursorColor = new Color(0.706f, 0.706f, 0.706f, 1f);

		private float m_CursorWidth = 1f;

		public readonly struct Glyph
		{
			internal Glyph(NativeSlice<Vertex> vertices)
			{
				this.vertices = vertices;
			}

			public readonly NativeSlice<Vertex> vertices;
		}

		public readonly struct GlyphsEnumerable
		{
			internal GlyphsEnumerable(TextElement te, List<NativeSlice<Vertex>> vertices)
			{
				this.m_TextElement = te;
				this.m_Vertices = vertices;
				this.Count = TextElement.GlyphsEnumerable.ComputeCount(vertices);
			}

			internal unsafe GlyphsEnumerable(TextElement te, List<NativeSlice<Vertex>> vertices, Span<ATGMeshInfo> meshInfos)
			{
				this.m_TextElement = te;
				this.m_Vertices = vertices;
				this.Count = TextElement.GlyphsEnumerable.ComputeCount(vertices);
				Span<ATGMeshInfo> span = meshInfos;
				for (int i = 0; i < span.Length; i++)
				{
					ATGMeshInfo atgmeshInfo = *span[i];
					TextAsset textAssetByID = TextAsset.GetTextAssetByID(atgmeshInfo.textAssetId);
					FontAsset fontAsset = textAssetByID as FontAsset;
					bool flag = fontAsset != null && fontAsset.atlasTextureCount > 1;
					if (flag)
					{
						Debug.LogWarning("PostProcessTextVertices with ATG does not support this Multi-Atlas.");
					}
				}
			}

			private static int ComputeCount(List<NativeSlice<Vertex>> verts)
			{
				int num = 0;
				for (int i = 0; i < verts.Count; i++)
				{
					num += verts[i].Length;
				}
				return num / 4;
			}

			public TextElement.GlyphsEnumerator GetEnumerator()
			{
				return new TextElement.GlyphsEnumerator(this.m_TextElement, this.m_Vertices);
			}

			public readonly int Count;

			private readonly List<NativeSlice<Vertex>> m_Vertices;

			private readonly TextElement m_TextElement;
		}

		public struct GlyphsEnumerator
		{
			public TextElement.Glyph Current { readonly get; private set; }

			internal GlyphsEnumerator(TextElement textElement, List<NativeSlice<Vertex>> vertices)
			{
				this.m_TextElement = textElement;
				this.m_Vertices = vertices;
				this.m_NextIndex = 0;
				this.Current = default(TextElement.Glyph);
			}

			public bool MoveNext()
			{
				bool flag = this.m_TextElement.computedStyle.unityTextGenerator == TextGeneratorType.Advanced;
				bool flag2;
				if (flag)
				{
					flag2 = this.MoveNextAdvanced();
				}
				else
				{
					flag2 = this.MoveNextStandard();
				}
				return flag2;
			}

			private bool MoveNextStandard()
			{
				TextInfo textInfo = this.m_TextElement.uitkTextHandle.textInfo;
				int characterCount = textInfo.characterCount;
				while (this.m_NextIndex < characterCount)
				{
					TextElementInfo[] textElementInfo = textInfo.textElementInfo;
					int nextIndex = this.m_NextIndex;
					this.m_NextIndex = nextIndex + 1;
					ref TextElementInfo ptr = ref textElementInfo[nextIndex];
					bool flag = !ptr.isVisible;
					if (!flag)
					{
						this.Current = new TextElement.Glyph(this.m_Vertices[ptr.materialReferenceIndex].Slice<Vertex>(ptr.vertexIndex, 4));
						return true;
					}
				}
				return false;
			}

			private bool MoveNextAdvanced()
			{
				IntPtr textGenerationInfo = this.m_TextElement.uitkTextHandle.textGenerationInfo;
				int glyphCount = TextGenerationInfo.GetGlyphCount(textGenerationInfo);
				while (this.m_NextIndex < glyphCount)
				{
					IntPtr intPtr = textGenerationInfo;
					int nextIndex = this.m_NextIndex;
					this.m_NextIndex = nextIndex + 1;
					TextRenderingIndices textRenderingIndices = TextGenerationInfo.GetTextRenderingIndices(intPtr, nextIndex);
					bool flag = textRenderingIndices.textElementInfoIndex < 0 || textRenderingIndices.meshIndex < 0;
					if (!flag)
					{
						this.Current = new TextElement.Glyph(this.m_Vertices[textRenderingIndices.meshIndex].Slice<Vertex>(textRenderingIndices.textElementInfoIndex * 4, 4));
						return true;
					}
				}
				return false;
			}

			public void Reset()
			{
				this.m_NextIndex = 0;
			}

			private readonly TextElement m_TextElement;

			private readonly List<NativeSlice<Vertex>> m_Vertices;

			private int m_NextIndex;
		}

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BindableElement.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(TextElement.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("text", "text", null, Array.Empty<string>()),
					new UxmlAttributeNames("enableRichText", "enable-rich-text", null, Array.Empty<string>()),
					new UxmlAttributeNames("emojiFallbackSupport", "emoji-fallback-support", null, Array.Empty<string>()),
					new UxmlAttributeNames("parseEscapeSequences", "parse-escape-sequences", null, Array.Empty<string>()),
					new UxmlAttributeNames("isSelectable", "selectable", null, new string[] { "selectable" }),
					new UxmlAttributeNames("doubleClickSelectsWord", "double-click-selects-word", null, new string[] { "selectWordByDoubleClick", "select-word-by-double-click" }),
					new UxmlAttributeNames("tripleClickSelectsLine", "triple-click-selects-line", null, new string[] { "selectLineByTripleClick", "select-line-by-triple-click" }),
					new UxmlAttributeNames("displayTooltipWhenElided", "display-tooltip-when-elided", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new TextElement();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				TextElement textElement = (TextElement)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.text_UxmlAttributeFlags);
				if (flag)
				{
					textElement.text = this.text;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.enableRichText_UxmlAttributeFlags);
				if (flag2)
				{
					textElement.enableRichText = this.enableRichText;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.emojiFallbackSupport_UxmlAttributeFlags);
				if (flag3)
				{
					textElement.emojiFallbackSupport = this.emojiFallbackSupport;
				}
				bool flag4 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.parseEscapeSequences_UxmlAttributeFlags);
				if (flag4)
				{
					textElement.parseEscapeSequences = this.parseEscapeSequences;
				}
				bool flag5 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.isSelectable_UxmlAttributeFlags);
				if (flag5)
				{
					textElement.isSelectable = this.isSelectable;
				}
				bool flag6 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.doubleClickSelectsWord_UxmlAttributeFlags);
				if (flag6)
				{
					textElement.doubleClickSelectsWord = this.doubleClickSelectsWord;
				}
				bool flag7 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.tripleClickSelectsLine_UxmlAttributeFlags);
				if (flag7)
				{
					textElement.tripleClickSelectsLine = this.tripleClickSelectsLine;
				}
				bool flag8 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.displayTooltipWhenElided_UxmlAttributeFlags);
				if (flag8)
				{
					textElement.displayTooltipWhenElided = this.displayTooltipWhenElided;
				}
			}

			[SerializeField]
			[MultilineTextField]
			private string text;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags text_UxmlAttributeFlags;

			[SerializeField]
			private bool enableRichText;

			[HideInInspector]
			[SerializeField]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags enableRichText_UxmlAttributeFlags;

			[SerializeField]
			private bool emojiFallbackSupport;

			[UxmlIgnore]
			[SerializeField]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags emojiFallbackSupport_UxmlAttributeFlags;

			[SerializeField]
			private bool parseEscapeSequences;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags parseEscapeSequences_UxmlAttributeFlags;

			[SelectableTextElement]
			[FormerlySerializedAs("selectable")]
			[SerializeField]
			[UxmlAttribute("selectable")]
			private bool isSelectable;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			[FormerlySerializedAs("selectable_UxmlAttributeFlags")]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags isSelectable_UxmlAttributeFlags;

			[FormerlySerializedAs("selectWordByDoubleClick")]
			[SerializeField]
			[UxmlAttribute("double-click-selects-word", new string[] { "select-word-by-double-click" })]
			private bool doubleClickSelectsWord;

			[FormerlySerializedAs("selectWordByDoubleClick_UxmlAttributeFlags")]
			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags doubleClickSelectsWord_UxmlAttributeFlags;

			[FormerlySerializedAs("selectLineByTripleClick")]
			[UxmlAttribute("triple-click-selects-line", new string[] { "select-line-by-triple-click" })]
			[SerializeField]
			private bool tripleClickSelectsLine;

			[FormerlySerializedAs("selectLineByTripleClick_UxmlAttributeFlags")]
			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags tripleClickSelectsLine_UxmlAttributeFlags;

			[SerializeField]
			private bool displayTooltipWhenElided;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags displayTooltipWhenElided_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<TextElement, TextElement.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
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
				textElement.emojiFallbackSupport = this.m_EmojiFallbackSupport.GetValueFromBag(bag, cc);
				textElement.isSelectable = this.m_Selectable.GetValueFromBag(bag, cc);
				textElement.parseEscapeSequences = this.m_ParseEscapeSequences.GetValueFromBag(bag, cc);
				textElement.selection.doubleClickSelectsWord = this.m_SelectWordByDoubleClick.GetValueFromBag(bag, cc);
				textElement.selection.tripleClickSelectsLine = this.m_SelectLineByTripleClick.GetValueFromBag(bag, cc);
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

			private UxmlBoolAttributeDescription m_EmojiFallbackSupport = new UxmlBoolAttributeDescription
			{
				name = "emoji-fallback-support",
				defaultValue = true
			};

			private UxmlBoolAttributeDescription m_ParseEscapeSequences = new UxmlBoolAttributeDescription
			{
				name = "parse-escape-sequences"
			};

			private UxmlBoolAttributeDescription m_Selectable = new UxmlBoolAttributeDescription
			{
				name = "selectable"
			};

			private UxmlBoolAttributeDescription m_SelectWordByDoubleClick = new UxmlBoolAttributeDescription
			{
				name = "double-click-selects-word"
			};

			private UxmlBoolAttributeDescription m_SelectLineByTripleClick = new UxmlBoolAttributeDescription
			{
				name = "triple-click-selects-line"
			};

			private UxmlBoolAttributeDescription m_DisplayTooltipWhenElided = new UxmlBoolAttributeDescription
			{
				name = "display-tooltip-when-elided"
			};
		}
	}
}
