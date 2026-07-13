using System;

namespace UnityEngine.UIElements.Internal
{
	internal class MultiColumnHeaderColumn : VisualElement
	{
		public Clickable clickable { get; private set; }

		public ColumnMover mover { get; private set; }

		public string sortOrderLabel
		{
			get
			{
				return this.m_SortIndicatorContainer.sortOrderLabel;
			}
			set
			{
				this.m_SortIndicatorContainer.sortOrderLabel = value;
			}
		}

		public Column column { get; private set; }

		internal Label title
		{
			get
			{
				VisualElement content = this.content;
				return (content != null) ? content.Q<Label>(MultiColumnHeaderColumn.titleElementName, null) : null;
			}
		}

		public VisualElement content
		{
			get
			{
				return this.m_Content;
			}
			set
			{
				bool flag = this.m_Content != null;
				if (flag)
				{
					bool flag2 = this.m_Content.parent == this.m_ContentContainer;
					if (flag2)
					{
						this.m_Content.RemoveFromHierarchy();
					}
					this.DestroyHeaderContent();
					this.m_Content = null;
				}
				this.m_Content = value;
				bool flag3 = this.m_Content != null;
				if (flag3)
				{
					this.m_Content.AddToClassList(MultiColumnHeaderColumn.contentUssClassName);
					this.m_ContentContainer.Add(this.m_Content);
				}
			}
		}

		private bool isContentBound
		{
			get
			{
				return this.m_Content != null && (bool)this.m_Content.GetProperty(MultiColumnHeaderColumn.s_BoundVEPropertyName);
			}
			set
			{
				VisualElement content = this.m_Content;
				if (content != null)
				{
					content.SetProperty(MultiColumnHeaderColumn.s_BoundVEPropertyName, value);
				}
			}
		}

		public MultiColumnHeaderColumn()
			: this(new Column())
		{
		}

		public MultiColumnHeaderColumn(Column column)
		{
			this.column = column;
			this.column.changed += this.OnColumnChanged;
			this.column.resized += this.OnColumnResized;
			base.AddToClassList(MultiColumnHeaderColumn.ussClassName);
			base.style.marginLeft = 0f;
			base.style.marginTop = 0f;
			base.style.marginRight = 0f;
			base.style.marginBottom = 0f;
			base.style.paddingLeft = 0f;
			base.style.paddingTop = 0f;
			base.style.paddingRight = 0f;
			base.style.paddingBottom = 0f;
			base.Add(this.m_SortIndicatorContainer = new MultiColumnHeaderColumnSortIndicator());
			this.m_ContentContainer = new VisualElement();
			this.m_ContentContainer.style.flexGrow = 1f;
			this.m_ContentContainer.style.flexShrink = 1f;
			this.m_ContentContainer.AddToClassList(MultiColumnHeaderColumn.contentContainerUssClassName);
			base.Add(this.m_ContentContainer);
			this.UpdateHeaderTemplate();
			this.UpdateGeometryFromColumn();
			this.InitManipulators();
		}

		private void OnColumnChanged(Column c, ColumnDataType role)
		{
			bool flag = this.column != c;
			if (!flag)
			{
				bool flag2 = role == ColumnDataType.HeaderTemplate;
				if (flag2)
				{
					IVisualElementScheduledItem scheduledHeaderTemplateUpdate = this.m_ScheduledHeaderTemplateUpdate;
					if (scheduledHeaderTemplateUpdate != null)
					{
						scheduledHeaderTemplateUpdate.Pause();
					}
					this.m_ScheduledHeaderTemplateUpdate = base.schedule.Execute(new Action(this.UpdateHeaderTemplate));
				}
				else
				{
					this.UpdateDataFromColumn();
				}
			}
		}

		private void OnColumnResized(Column c)
		{
			this.UpdateGeometryFromColumn();
		}

		private void InitManipulators()
		{
			this.AddManipulator(this.mover = new ColumnMover());
			this.mover.movingChanged += this.OnMoverChanged;
			this.AddManipulator(this.clickable = new Clickable(null));
			this.clickable.activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.LeftMouse,
				modifiers = EventModifiers.Shift
			});
			EventModifiers eventModifiers = EventModifiers.Control;
			RuntimePlatform platform = Application.platform;
			bool flag = platform == RuntimePlatform.OSXEditor || platform == RuntimePlatform.OSXPlayer;
			if (flag)
			{
				eventModifiers = EventModifiers.Command;
			}
			this.clickable.activators.Add(new ManipulatorActivationFilter
			{
				button = MouseButton.LeftMouse,
				modifiers = eventModifiers
			});
		}

		private void OnMoverChanged(ColumnMover mv)
		{
			bool moving = this.mover.moving;
			if (moving)
			{
				base.AddToClassList(MultiColumnHeaderColumn.movingUssClassName);
			}
			else
			{
				base.RemoveFromClassList(MultiColumnHeaderColumn.movingUssClassName);
			}
		}

		private void UpdateDataFromColumn()
		{
			bool flag = this.column == null;
			if (!flag)
			{
				base.name = this.column.name;
				this.UnbindHeaderContent();
				this.BindHeaderContent();
			}
		}

		private void BindHeaderContent()
		{
			bool flag = !this.isContentBound;
			if (flag)
			{
				Action<VisualElement> action = this.content.GetProperty(MultiColumnHeaderColumn.s_BindingCallbackVEPropertyName) as Action<VisualElement>;
				if (action != null)
				{
					action(this.content);
				}
				this.isContentBound = true;
			}
		}

		private void UnbindHeaderContent()
		{
			bool isContentBound = this.isContentBound;
			if (isContentBound)
			{
				Action<VisualElement> action = this.content.GetProperty(MultiColumnHeaderColumn.s_UnbindingCallbackVEPropertyName) as Action<VisualElement>;
				if (action != null)
				{
					action(this.content);
				}
				this.isContentBound = false;
			}
		}

		private void DestroyHeaderContent()
		{
			this.UnbindHeaderContent();
			Action<VisualElement> action = this.content.GetProperty(MultiColumnHeaderColumn.s_DestroyCallbackVEPropertyName) as Action<VisualElement>;
			this.content.ClearProperty(MultiColumnHeaderColumn.s_BindingCallbackVEPropertyName);
			this.content.ClearProperty(MultiColumnHeaderColumn.s_UnbindingCallbackVEPropertyName);
			this.content.ClearProperty(MultiColumnHeaderColumn.s_DestroyCallbackVEPropertyName);
			this.content.ClearProperty(MultiColumnHeaderColumn.s_BoundVEPropertyName);
			if (action != null)
			{
				action(this.content);
			}
		}

		private VisualElement CreateDefaultHeaderContent()
		{
			VisualElement visualElement = new VisualElement
			{
				pickingMode = PickingMode.Ignore
			};
			visualElement.AddToClassList(MultiColumnHeaderColumn.defaultContentUssClassName);
			MultiColumnHeaderColumnIcon multiColumnHeaderColumnIcon = new MultiColumnHeaderColumnIcon
			{
				name = MultiColumnHeaderColumn.iconElementName,
				pickingMode = PickingMode.Ignore
			};
			Label label = new Label
			{
				name = MultiColumnHeaderColumn.titleElementName,
				pickingMode = PickingMode.Ignore
			};
			label.AddToClassList(MultiColumnHeaderColumn.titleUssClassName);
			visualElement.Add(multiColumnHeaderColumnIcon);
			visualElement.Add(label);
			return visualElement;
		}

		private void DefaultBindHeaderContent(VisualElement ve)
		{
			Label label = ve.Q<Label>(MultiColumnHeaderColumn.titleElementName, null);
			MultiColumnHeaderColumnIcon multiColumnHeaderColumnIcon = ve.Q<MultiColumnHeaderColumnIcon>(null, null);
			ve.RemoveFromClassList(MultiColumnHeaderColumn.hasTitleUssClassName);
			bool flag = label != null;
			if (flag)
			{
				label.text = this.column.title;
			}
			bool flag2 = !string.IsNullOrEmpty(this.column.title);
			if (flag2)
			{
				ve.AddToClassList(MultiColumnHeaderColumn.hasTitleUssClassName);
			}
			bool flag3 = multiColumnHeaderColumnIcon != null;
			if (flag3)
			{
				bool flag4 = this.column.icon.texture != null || this.column.icon.sprite != null || this.column.icon.vectorImage != null;
				if (flag4)
				{
					multiColumnHeaderColumnIcon.isImageInline = true;
					multiColumnHeaderColumnIcon.image = this.column.icon.texture;
					multiColumnHeaderColumnIcon.sprite = this.column.icon.sprite;
					multiColumnHeaderColumnIcon.vectorImage = this.column.icon.vectorImage;
				}
				else
				{
					bool isImageInline = multiColumnHeaderColumnIcon.isImageInline;
					if (isImageInline)
					{
						multiColumnHeaderColumnIcon.image = null;
						multiColumnHeaderColumnIcon.sprite = null;
						multiColumnHeaderColumnIcon.vectorImage = null;
					}
				}
				multiColumnHeaderColumnIcon.UpdateClassList();
			}
		}

		private void UpdateHeaderTemplate()
		{
			bool flag = this.column == null;
			if (!flag)
			{
				Func<VisualElement> func = this.column.makeHeader;
				Action<VisualElement> action = this.column.bindHeader;
				Action<VisualElement> action2 = this.column.unbindHeader;
				Action<VisualElement> action3 = this.column.destroyHeader;
				bool flag2 = func == null;
				if (flag2)
				{
					func = new Func<VisualElement>(this.CreateDefaultHeaderContent);
					action = new Action<VisualElement>(this.DefaultBindHeaderContent);
					action2 = null;
					action3 = null;
				}
				this.content = func();
				this.content.SetProperty(MultiColumnHeaderColumn.s_BindingCallbackVEPropertyName, action);
				this.content.SetProperty(MultiColumnHeaderColumn.s_UnbindingCallbackVEPropertyName, action2);
				this.content.SetProperty(MultiColumnHeaderColumn.s_DestroyCallbackVEPropertyName, action3);
				this.isContentBound = false;
				this.m_ScheduledHeaderTemplateUpdate = null;
				this.UpdateDataFromColumn();
			}
		}

		private void UpdateGeometryFromColumn()
		{
			bool flag = float.IsNaN(this.column.desiredWidth);
			if (!flag)
			{
				base.style.width = this.column.desiredWidth;
			}
		}

		public void Dispose()
		{
			this.mover.movingChanged -= this.OnMoverChanged;
			this.column.changed -= this.OnColumnChanged;
			this.column.resized -= this.OnColumnResized;
			this.RemoveManipulator(this.mover);
			this.RemoveManipulator(this.clickable);
			this.mover = null;
			this.column = null;
			this.content = null;
		}

		public static readonly string ussClassName = MultiColumnCollectionHeader.ussClassName + "__column";

		public static readonly string sortableUssClassName = MultiColumnHeaderColumn.ussClassName + "--sortable";

		public static readonly string sortedAscendingUssClassName = MultiColumnHeaderColumn.ussClassName + "--sorted-ascending";

		public static readonly string sortedDescendingUssClassName = MultiColumnHeaderColumn.ussClassName + "--sorted-descending";

		public static readonly string movingUssClassName = MultiColumnHeaderColumn.ussClassName + "--moving";

		public static readonly string contentContainerUssClassName = MultiColumnHeaderColumn.ussClassName + "__content-container";

		public static readonly string contentUssClassName = MultiColumnHeaderColumn.ussClassName + "__content";

		public static readonly string defaultContentUssClassName = MultiColumnHeaderColumn.ussClassName + "__default-content";

		public static readonly string hasIconUssClassName = MultiColumnHeaderColumn.contentUssClassName + "--has-icon";

		public static readonly string hasTitleUssClassName = MultiColumnHeaderColumn.contentUssClassName + "--has-title";

		public static readonly string titleUssClassName = MultiColumnHeaderColumn.ussClassName + "__title";

		public static readonly string iconElementName = "unity-multi-column-header-column-icon";

		public static readonly string titleElementName = "unity-multi-column-header-column-title";

		private static readonly string s_BoundVEPropertyName = "__bound";

		private static readonly string s_BindingCallbackVEPropertyName = "__binding-callback";

		private static readonly string s_UnbindingCallbackVEPropertyName = "__unbinding-callback";

		private static readonly string s_DestroyCallbackVEPropertyName = "__destroy-callback";

		private VisualElement m_ContentContainer;

		private VisualElement m_Content;

		private MultiColumnHeaderColumnSortIndicator m_SortIndicatorContainer;

		private IVisualElementScheduledItem m_ScheduledHeaderTemplateUpdate;
	}
}
