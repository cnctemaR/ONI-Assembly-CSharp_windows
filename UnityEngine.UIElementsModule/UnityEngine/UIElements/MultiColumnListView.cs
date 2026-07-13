using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class MultiColumnListView : BaseListView
	{
		public new MultiColumnListViewController viewController
		{
			get
			{
				return base.viewController as MultiColumnListViewController;
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action columnSortingChanged;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<ContextualMenuPopulateEvent, Column> headerContextMenuPopulateEvent;

		public IEnumerable<SortColumnDescription> sortedColumns
		{
			get
			{
				return this.m_SortedColumns;
			}
		}

		[CreateProperty]
		public Columns columns
		{
			get
			{
				return this.m_Columns;
			}
			private set
			{
				bool flag = this.m_Columns != null;
				if (flag)
				{
					this.m_Columns.propertyChanged -= this.ColumnsChanged;
				}
				bool flag2 = value == null;
				if (flag2)
				{
					this.m_Columns.Clear();
				}
				else
				{
					this.m_Columns = value;
					this.m_Columns.propertyChanged += this.ColumnsChanged;
					bool flag3 = this.m_Columns.Count > 0;
					if (flag3)
					{
						base.GetOrCreateViewController();
					}
					base.NotifyPropertyChanged(in MultiColumnListView.columnsProperty);
				}
			}
		}

		[CreateProperty]
		public SortColumnDescriptions sortColumnDescriptions
		{
			get
			{
				return this.m_SortColumnDescriptions;
			}
			private set
			{
				bool flag = value == null;
				if (flag)
				{
					this.m_SortColumnDescriptions.Clear();
				}
				else
				{
					this.m_SortColumnDescriptions = value;
					bool flag2 = this.viewController != null;
					if (flag2)
					{
						this.viewController.columnController.header.sortDescriptions = value;
						this.RaiseColumnSortingChanged();
					}
					base.NotifyPropertyChanged(in MultiColumnListView.sortColumnDescriptionsProperty);
				}
			}
		}

		[Obsolete("sortingEnabled has been deprecated. Use sortingMode instead.", false)]
		public bool sortingEnabled
		{
			get
			{
				return this.sortingMode == ColumnSortingMode.Custom;
			}
			set
			{
				this.sortingMode = (value ? ColumnSortingMode.Custom : ColumnSortingMode.None);
			}
		}

		[CreateProperty]
		public ColumnSortingMode sortingMode
		{
			get
			{
				return this.m_SortingMode;
			}
			set
			{
				bool flag = this.sortingMode == value;
				if (!flag)
				{
					this.m_SortingMode = value;
					bool flag2 = this.viewController != null;
					if (flag2)
					{
						this.viewController.columnController.sortingMode = value;
					}
					base.NotifyPropertyChanged(in MultiColumnListView.sortingModeProperty);
				}
			}
		}

		public MultiColumnListView()
			: this(new Columns())
		{
		}

		public MultiColumnListView(Columns columns)
		{
			base.scrollView.viewDataKey = "unity-multi-column-scroll-view";
			this.columns = columns ?? new Columns();
		}

		protected override CollectionViewController CreateViewController()
		{
			return new MultiColumnListViewController(this.columns, this.sortColumnDescriptions, this.m_SortedColumns);
		}

		public override void SetViewController(CollectionViewController controller)
		{
			bool flag = this.viewController != null;
			if (flag)
			{
				this.viewController.columnController.columnSortingChanged -= this.RaiseColumnSortingChanged;
				this.viewController.columnController.headerContextMenuPopulateEvent -= this.RaiseHeaderContextMenuPopulate;
			}
			base.SetViewController(controller);
			bool flag2 = this.viewController != null;
			if (flag2)
			{
				this.viewController.columnController.sortingMode = this.m_SortingMode;
				this.viewController.columnController.columnSortingChanged += this.RaiseColumnSortingChanged;
				this.viewController.columnController.headerContextMenuPopulateEvent += this.RaiseHeaderContextMenuPopulate;
			}
		}

		private protected override void CreateVirtualizationController()
		{
			base.CreateVirtualizationController<ReusableMultiColumnListViewItem>();
		}

		private void RaiseColumnSortingChanged()
		{
			Action action = this.columnSortingChanged;
			if (action != null)
			{
				action();
			}
		}

		private void ColumnsChanged(object sender, BindablePropertyChangedEventArgs args)
		{
			BindingId propertyName = args.propertyName;
			base.NotifyPropertyChanged(in propertyName);
		}

		private void RaiseHeaderContextMenuPopulate(ContextualMenuPopulateEvent evt, Column column)
		{
			Action<ContextualMenuPopulateEvent, Column> action = this.headerContextMenuPopulateEvent;
			if (action != null)
			{
				action(evt, column);
			}
		}

		private static readonly BindingId columnsProperty = "columns";

		private static readonly BindingId sortColumnDescriptionsProperty = "sortColumnDescriptions";

		private static readonly BindingId sortingModeProperty = "sortingMode";

		private Columns m_Columns;

		private ColumnSortingMode m_SortingMode;

		private SortColumnDescriptions m_SortColumnDescriptions = new SortColumnDescriptions();

		private List<SortColumnDescription> m_SortedColumns = new List<SortColumnDescription>();

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseListView.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(MultiColumnListView.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("sortingEnabled", "sorting-enabled", null, Array.Empty<string>()),
					new UxmlAttributeNames("sortingMode", "sorting-mode", null, Array.Empty<string>()),
					new UxmlAttributeNames("columns", "columns", null, Array.Empty<string>()),
					new UxmlAttributeNames("sortColumnDescriptions", "sort-column-descriptions", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new MultiColumnListView();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				MultiColumnListView multiColumnListView = (MultiColumnListView)obj;
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.sortingMode_UxmlAttributeFlags);
				if (flag)
				{
					multiColumnListView.sortingMode = this.sortingMode;
				}
				else
				{
					bool flag2 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.sortingEnabled_UxmlAttributeFlags);
					if (flag2)
					{
						multiColumnListView.sortingEnabled = this.sortingEnabled;
					}
				}
				bool flag3 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.sortColumnDescriptions_UxmlAttributeFlags) && this.sortColumnDescriptions != null;
				if (flag3)
				{
					SortColumnDescriptions sortColumnDescriptions = new SortColumnDescriptions();
					this.sortColumnDescriptions.Deserialize(sortColumnDescriptions);
					multiColumnListView.sortColumnDescriptions = sortColumnDescriptions;
				}
				bool flag4 = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.columns_UxmlAttributeFlags) && this.columns != null;
				if (flag4)
				{
					Columns columns = new Columns();
					this.columns.Deserialize(columns);
					multiColumnListView.columns = columns;
				}
			}

			[UxmlObjectReference]
			[SerializeReference]
			private Columns.UxmlSerializedData columns;

			[UxmlObjectReference]
			[SerializeReference]
			private SortColumnDescriptions.UxmlSerializedData sortColumnDescriptions;

			[SerializeField]
			private ColumnSortingMode sortingMode;

			[SerializeField]
			[HideInInspector]
			private bool sortingEnabled;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags sortingEnabled_UxmlAttributeFlags;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags sortingMode_UxmlAttributeFlags;

			[SerializeField]
			[UxmlIgnore]
			[HideInInspector]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags columns_UxmlAttributeFlags;

			[HideInInspector]
			[UxmlIgnore]
			[SerializeField]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags sortColumnDescriptions_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<MultiColumnListView, MultiColumnListView.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseListView.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				MultiColumnListView multiColumnListView = (MultiColumnListView)ve;
				string text;
				bool flag = this.m_SortingMode.TryGetValueFromBagAsString(bag, cc, out text);
				if (flag)
				{
					bool flag3;
					bool flag2 = bool.TryParse(text, out flag3);
					if (flag2)
					{
						multiColumnListView.sortingMode = (flag3 ? ColumnSortingMode.Custom : ColumnSortingMode.None);
					}
					else
					{
						multiColumnListView.sortingMode = this.m_SortingMode.GetValueFromBag(bag, cc);
					}
				}
				multiColumnListView.sortColumnDescriptions = this.m_SortColumnDescriptions.GetValueFromBag(bag, cc);
				multiColumnListView.columns = this.m_Columns.GetValueFromBag(bag, cc);
			}

			private readonly UxmlEnumAttributeDescription<ColumnSortingMode> m_SortingMode = new UxmlEnumAttributeDescription<ColumnSortingMode>
			{
				name = "sorting-mode",
				obsoleteNames = new string[] { "sorting-enabled" }
			};

			private readonly UxmlObjectAttributeDescription<Columns> m_Columns = new UxmlObjectAttributeDescription<Columns>();

			private readonly UxmlObjectAttributeDescription<SortColumnDescriptions> m_SortColumnDescriptions = new UxmlObjectAttributeDescription<SortColumnDescriptions>();
		}
	}
}
