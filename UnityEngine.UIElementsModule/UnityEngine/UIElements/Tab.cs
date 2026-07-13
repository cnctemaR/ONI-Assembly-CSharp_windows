using System;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class Tab : VisualElement
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Tab> selected;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Func<bool> closing;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<Tab> closed;

		private Object iconImageReference
		{
			get
			{
				return this.iconImage.GetSelectedImage();
			}
			set
			{
				this.iconImage = Background.FromObject(value);
			}
		}

		internal Label headerLabel
		{
			[VisibleToOtherModules(new string[] { "UnityEditor.UIBuilderModule" })]
			get
			{
				return this.m_TabHeaderLabel;
			}
		}

		public VisualElement tabHeader
		{
			get
			{
				return this.m_TabHeader;
			}
		}

		internal TabDragger dragger { get; }

		internal int index { get; set; }

		[CreateProperty]
		public string label
		{
			get
			{
				return this.m_Label;
			}
			set
			{
				bool flag = string.CompareOrdinal(value, this.m_Label) == 0;
				if (!flag)
				{
					this.m_TabHeaderLabel.text = value;
					this.m_TabHeaderLabel.EnableInClassList(Tab.tabHeaderEmptyLabeUssClassName, string.IsNullOrEmpty(value));
					this.m_TabHeaderImage.EnableInClassList(Tab.tabHeaderStandaloneImageUssClassName, string.IsNullOrEmpty(value));
					this.m_Label = value;
					base.NotifyPropertyChanged(in Tab.labelProperty);
				}
			}
		}

		[CreateProperty]
		public Background iconImage
		{
			get
			{
				return this.m_IconImage;
			}
			set
			{
				bool flag = value == this.m_IconImage;
				if (!flag)
				{
					bool flag2 = value.IsEmpty();
					if (flag2)
					{
						this.m_TabHeaderImage.image = null;
						this.m_TabHeaderImage.sprite = null;
						this.m_TabHeaderImage.vectorImage = null;
						this.m_TabHeaderImage.AddToClassList(Tab.tabHeaderEmptyImageUssClassName);
						this.m_TabHeaderImage.RemoveFromClassList(Tab.tabHeaderStandaloneImageUssClassName);
						this.m_IconImage = value;
						base.NotifyPropertyChanged(in Tab.iconImageProperty);
					}
					else
					{
						bool flag3 = value.texture;
						if (flag3)
						{
							this.m_TabHeaderImage.image = value.texture;
						}
						else
						{
							bool flag4 = value.sprite;
							if (flag4)
							{
								this.m_TabHeaderImage.sprite = value.sprite;
							}
							else
							{
								bool flag5 = value.renderTexture;
								if (flag5)
								{
									this.m_TabHeaderImage.image = value.renderTexture;
								}
								else
								{
									this.m_TabHeaderImage.vectorImage = value.vectorImage;
								}
							}
						}
						this.m_TabHeaderImage.RemoveFromClassList(Tab.tabHeaderEmptyImageUssClassName);
						this.m_TabHeaderImage.EnableInClassList(Tab.tabHeaderStandaloneImageUssClassName, string.IsNullOrEmpty(this.m_Label));
						this.m_IconImage = value;
						base.NotifyPropertyChanged(in Tab.iconImageProperty);
					}
				}
			}
		}

		[CreateProperty]
		public bool closeable
		{
			get
			{
				return this.m_Closeable;
			}
			set
			{
				bool flag = this.m_Closeable == value;
				if (!flag)
				{
					this.m_Closeable = value;
					this.m_TabHeader.EnableInClassList(Tab.closeableUssClassName, value);
					this.EnableTabCloseButton(value);
					base.NotifyPropertyChanged(in Tab.closeableProperty);
				}
			}
		}

		public override VisualElement contentContainer
		{
			get
			{
				return this.m_ContentContainer;
			}
		}

		public Tab()
			: this(null, null)
		{
		}

		public Tab(string label)
			: this(label, null)
		{
		}

		public Tab(Background iconImage)
			: this(null, iconImage)
		{
		}

		public Tab(string label, Background iconImage)
		{
			base.AddToClassList(Tab.ussClassName);
			this.m_TabHeader = new VisualElement
			{
				classList = { Tab.tabHeaderUssClassName },
				name = Tab.tabHeaderUssClassName
			};
			this.m_DragHandle = new VisualElement
			{
				name = Tab.reorderableItemHandleUssClassName,
				classList = { Tab.reorderableItemHandleUssClassName }
			};
			this.m_DragHandle.AddToClassList(Tab.reorderableItemHandleUssClassName);
			this.m_DragHandle.Add(new VisualElement
			{
				name = Tab.reorderableItemHandleBarUssClassName,
				classList = 
				{
					Tab.reorderableItemHandleBarUssClassName,
					Tab.reorderableItemHandleBarUssClassName + "--left"
				}
			});
			this.m_DragHandle.Add(new VisualElement
			{
				name = Tab.reorderableItemHandleBarUssClassName,
				classList = { Tab.reorderableItemHandleBarUssClassName }
			});
			this.m_TabHeaderImage = new Image
			{
				name = Tab.tabHeaderImageUssClassName,
				classList = 
				{
					Tab.tabHeaderImageUssClassName,
					Tab.tabHeaderEmptyImageUssClassName
				}
			};
			this.m_TabHeader.Add(this.m_TabHeaderImage);
			this.m_TabHeaderLabel = new Label
			{
				name = Tab.tabHeaderLabelUssClassName,
				classList = { Tab.tabHeaderLabelUssClassName }
			};
			this.m_TabHeader.Add(this.m_TabHeaderLabel);
			this.m_TabHeader.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnTabClicked), TrickleDown.NoTrickleDown);
			this.m_TabHeader.Add(new VisualElement
			{
				name = Tab.tabHeaderUnderlineUssClassName,
				classList = { Tab.tabHeaderUnderlineUssClassName }
			});
			this.m_CloseButton = new VisualElement
			{
				name = Tab.closeButtonUssClassName,
				classList = { Tab.closeButtonUssClassName }
			};
			this.m_CloseButton.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnCloseButtonClicked), TrickleDown.NoTrickleDown);
			base.hierarchy.Add(this.m_TabHeader);
			this.m_ContentContainer = new VisualElement
			{
				name = Tab.contentUssClassName,
				classList = { Tab.contentUssClassName },
				userData = this.m_TabHeader
			};
			base.hierarchy.Add(this.m_ContentContainer);
			this.label = label;
			this.iconImage = iconImage;
			this.m_DragHandle.AddManipulator(this.dragger = new TabDragger());
			this.m_TabHeader.RegisterCallback<TooltipEvent>(new EventCallback<TooltipEvent>(this.UpdateTooltip), TrickleDown.NoTrickleDown);
			base.RegisterCallback<TooltipEvent>(delegate(TooltipEvent evt)
			{
				evt.StopImmediatePropagation();
			}, TrickleDown.NoTrickleDown);
		}

		private void UpdateTooltip(TooltipEvent evt)
		{
			VisualElement visualElement = evt.currentTarget as VisualElement;
			bool flag = visualElement != null && !string.IsNullOrEmpty(base.tooltip);
			if (flag)
			{
				evt.rect = visualElement.worldBound;
				evt.tooltip = base.tooltip;
				evt.StopImmediatePropagation();
			}
		}

		private void AddDragHandles()
		{
			this.m_TabHeader.Insert(0, this.m_DragHandle);
		}

		private void RemoveDragHandles()
		{
			bool flag = this.m_TabHeader.Contains(this.m_DragHandle);
			if (flag)
			{
				this.m_TabHeader.Remove(this.m_DragHandle);
			}
		}

		internal void EnableTabDragHandles(bool enable)
		{
			if (enable)
			{
				this.AddDragHandles();
			}
			else
			{
				this.RemoveDragHandles();
			}
		}

		private void AddCloseButton()
		{
			this.m_TabHeader.Add(this.m_CloseButton);
		}

		private void RemoveCloseButton()
		{
			bool flag = this.m_TabHeader.Contains(this.m_CloseButton);
			if (flag)
			{
				this.m_TabHeader.Remove(this.m_CloseButton);
			}
		}

		internal void EnableTabCloseButton(bool enable)
		{
			if (enable)
			{
				this.AddCloseButton();
			}
			else
			{
				this.RemoveCloseButton();
			}
		}

		internal void SetActive()
		{
			this.m_TabHeader.SetCheckedPseudoState(true);
			base.SetCheckedPseudoState(true);
		}

		internal void SetInactive()
		{
			this.m_TabHeader.SetCheckedPseudoState(false);
			base.SetCheckedPseudoState(false);
		}

		private void OnTabClicked(PointerDownEvent _)
		{
			Action<Tab> action = this.selected;
			if (action != null)
			{
				action(this);
			}
		}

		private void OnCloseButtonClicked(PointerDownEvent evt)
		{
			Func<bool> func = this.closing;
			bool flag = func == null || func();
			bool flag2 = flag;
			if (flag2)
			{
				base.RemoveFromHierarchy();
				Action<Tab> action = this.closed;
				if (action != null)
				{
					action(this);
				}
			}
			evt.StopPropagation();
		}

		internal static readonly BindingId labelProperty = "label";

		internal static readonly BindingId iconImageProperty = "iconImage";

		internal static readonly BindingId closeableProperty = "closeable";

		public static readonly string ussClassName = "unity-tab";

		public static readonly string tabHeaderUssClassName = Tab.ussClassName + "__header";

		public static readonly string tabHeaderImageUssClassName = Tab.tabHeaderUssClassName + "-image";

		public static readonly string tabHeaderEmptyImageUssClassName = Tab.tabHeaderImageUssClassName + "--empty";

		public static readonly string tabHeaderStandaloneImageUssClassName = Tab.tabHeaderImageUssClassName + "--standalone";

		public static readonly string tabHeaderLabelUssClassName = Tab.tabHeaderUssClassName + "-label";

		public static readonly string tabHeaderEmptyLabeUssClassName = Tab.tabHeaderLabelUssClassName + "--empty";

		public static readonly string tabHeaderUnderlineUssClassName = Tab.tabHeaderUssClassName + "-underline";

		public static readonly string contentUssClassName = Tab.ussClassName + "__content-container";

		public static readonly string draggingUssClassName = Tab.ussClassName + "--dragging";

		public static readonly string reorderableUssClassName = Tab.ussClassName + "__reorderable";

		public static readonly string reorderableItemHandleUssClassName = Tab.reorderableUssClassName + "-handle";

		public static readonly string reorderableItemHandleBarUssClassName = Tab.reorderableItemHandleUssClassName + "-bar";

		public static readonly string closeableUssClassName = Tab.tabHeaderUssClassName + "__closeable";

		public static readonly string closeButtonUssClassName = Tab.ussClassName + "__close-button";

		private string m_Label;

		private Background m_IconImage;

		private bool m_Closeable;

		private VisualElement m_ContentContainer;

		private VisualElement m_DragHandle;

		private VisualElement m_CloseButton;

		private VisualElement m_TabHeader;

		private Image m_TabHeaderImage;

		private Label m_TabHeaderLabel;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : VisualElement.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(Tab.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("label", "label", null, Array.Empty<string>()),
					new UxmlAttributeNames("iconImageReference", "icon-image", null, Array.Empty<string>()),
					new UxmlAttributeNames("closeable", "closeable", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new Tab();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				Tab tab = (Tab)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.label_UxmlAttributeFlags);
				if (flag)
				{
					tab.label = this.label;
				}
				bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.iconImageReference_UxmlAttributeFlags);
				if (flag2)
				{
					tab.iconImageReference = this.iconImageReference;
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.closeable_UxmlAttributeFlags);
				if (flag3)
				{
					tab.closeable = this.closeable;
				}
			}

			[SerializeField]
			[MultilineTextField]
			private string label;

			[ImageFieldValueDecorator("Icon Image")]
			[SerializeField]
			[UxmlAttribute("icon-image")]
			[UxmlAttributeBindingPath("iconImage")]
			private Object iconImageReference;

			[SerializeField]
			private bool closeable;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags label_UxmlAttributeFlags;

			[UxmlIgnore]
			[HideInInspector]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags iconImageReference_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags closeable_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<Tab, Tab.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				Tab tab = (Tab)ve;
				tab.label = this.m_Label.GetValueFromBag(bag, cc);
				tab.iconImage = this.m_IconImage.GetValueFromBag(bag, cc);
				tab.closeable = this.m_Closeable.GetValueFromBag(bag, cc);
			}

			private readonly UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
			{
				name = "label"
			};

			private readonly UxmlImageAttributeDescription m_IconImage = new UxmlImageAttributeDescription
			{
				name = "icon-image"
			};

			private readonly UxmlBoolAttributeDescription m_Closeable = new UxmlBoolAttributeDescription
			{
				name = "closeable",
				defaultValue = false
			};
		}
	}
}
