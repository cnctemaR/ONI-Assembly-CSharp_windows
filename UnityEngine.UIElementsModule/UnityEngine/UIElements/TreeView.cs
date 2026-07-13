using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class TreeView : BaseTreeView
	{
		[CreateProperty]
		public new Func<VisualElement> makeItem
		{
			get
			{
				return this.m_MakeItem;
			}
			set
			{
				bool flag = value != this.m_MakeItem;
				if (flag)
				{
					this.m_MakeItem = value;
					base.Rebuild();
					base.NotifyPropertyChanged(in TreeView.makeItemProperty);
				}
			}
		}

		[CreateProperty]
		public VisualTreeAsset itemTemplate
		{
			get
			{
				return this.m_ItemTemplate;
			}
			set
			{
				bool flag = this.m_ItemTemplate == value;
				if (!flag)
				{
					this.m_ItemTemplate = value;
					bool flag2 = this.makeItem != this.m_TemplateMakeItem;
					if (flag2)
					{
						this.makeItem = this.m_TemplateMakeItem;
					}
					else
					{
						base.Rebuild();
					}
					base.NotifyPropertyChanged(in TreeView.itemTemplateProperty);
				}
			}
		}

		private VisualElement TemplateMakeItem()
		{
			bool flag = this.m_ItemTemplate != null;
			VisualElement visualElement;
			if (flag)
			{
				visualElement = this.m_ItemTemplate.Instantiate();
			}
			else
			{
				visualElement = new Label(BaseVerticalCollectionView.k_InvalidTemplateError);
			}
			return visualElement;
		}

		[CreateProperty]
		public new Action<VisualElement, int> bindItem
		{
			get
			{
				return this.m_BindItem;
			}
			set
			{
				bool flag = value != this.m_BindItem;
				if (flag)
				{
					this.m_BindItem = value;
					base.RefreshItems();
					base.NotifyPropertyChanged(in TreeView.bindItemProperty);
				}
			}
		}

		[CreateProperty]
		public new Action<VisualElement, int> unbindItem
		{
			get
			{
				return this.m_UnbindItem;
			}
			set
			{
				bool flag = value != this.m_UnbindItem;
				if (flag)
				{
					this.m_UnbindItem = value;
					base.NotifyPropertyChanged(in TreeView.unbindItemProperty);
				}
			}
		}

		[CreateProperty]
		public new Action<VisualElement> destroyItem
		{
			get
			{
				return this.m_DestroyItem;
			}
			set
			{
				bool flag = value != this.m_DestroyItem;
				if (flag)
				{
					this.m_DestroyItem = value;
					base.NotifyPropertyChanged(in TreeView.destroyItemProperty);
				}
			}
		}

		internal override void SetRootItemsInternal<T>(IList<TreeViewItemData<T>> rootItems)
		{
			TreeViewHelpers<T, DefaultTreeViewController<T>>.SetRootItems(this, rootItems, () => new DefaultTreeViewController<T>());
		}

		internal override bool HasValidDataAndBindings()
		{
			return base.HasValidDataAndBindings() && this.makeItem != null == (this.bindItem != null);
		}

		public new TreeViewController viewController
		{
			get
			{
				return base.viewController as TreeViewController;
			}
		}

		protected override CollectionViewController CreateViewController()
		{
			return new DefaultTreeViewController<object>();
		}

		public TreeView()
			: this(null, null)
		{
		}

		public TreeView(Func<VisualElement> makeItem, Action<VisualElement, int> bindItem)
			: base(-1)
		{
			this.makeItem = makeItem;
			this.bindItem = bindItem;
			this.m_TemplateMakeItem = new Func<VisualElement>(this.TemplateMakeItem);
		}

		public TreeView(int itemHeight, Func<VisualElement> makeItem, Action<VisualElement, int> bindItem)
			: this(makeItem, bindItem)
		{
			base.fixedItemHeight = (float)itemHeight;
		}

		private protected override IEnumerable<TreeViewItemData<T>> GetSelectedItemsInternal<T>()
		{
			return TreeViewHelpers<T, DefaultTreeViewController<T>>.GetSelectedItems(this);
		}

		private protected override T GetItemDataForIndexInternal<T>(int index)
		{
			return TreeViewHelpers<T, DefaultTreeViewController<T>>.GetItemDataForIndex(this, index);
		}

		private protected override T GetItemDataForIdInternal<T>(int id)
		{
			return TreeViewHelpers<T, DefaultTreeViewController<T>>.GetItemDataForId(this, id);
		}

		private protected override void AddItemInternal<T>(TreeViewItemData<T> item, int parentId, int childIndex, bool rebuildTree)
		{
			TreeViewHelpers<T, DefaultTreeViewController<T>>.AddItem(this, item, parentId, childIndex, rebuildTree);
		}

		internal static readonly BindingId itemTemplateProperty = "itemTemplate";

		internal static readonly BindingId makeItemProperty = "makeItem";

		internal static readonly BindingId bindItemProperty = "bindItem";

		internal static readonly BindingId unbindItemProperty = "unbindItem";

		internal static readonly BindingId destroyItemProperty = "destroyItem";

		private Func<VisualElement> m_MakeItem;

		private Func<VisualElement> m_TemplateMakeItem;

		private VisualTreeAsset m_ItemTemplate;

		private Action<VisualElement, int> m_BindItem;

		private Action<VisualElement, int> m_UnbindItem;

		private Action<VisualElement> m_DestroyItem;

		[ExcludeFromDocs]
		[Serializable]
		public new class UxmlSerializedData : BaseTreeView.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(TreeView.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("itemTemplate", "item-template", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new TreeView();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.itemTemplate_UxmlAttributeFlags);
				if (flag)
				{
					TreeView treeView = (TreeView)obj;
					treeView.itemTemplate = this.itemTemplate;
				}
			}

			[SerializeField]
			private VisualTreeAsset itemTemplate;

			[SerializeField]
			[HideInInspector]
			[UxmlIgnore]
			private UnityEngine.UIElements.UxmlSerializedData.UxmlAttributeFlags itemTemplate_UxmlAttributeFlags;
		}

		[Obsolete("UxmlFactory is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlFactory : UxmlFactory<TreeView, TreeView.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseTreeView.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				TreeView treeView = ve as TreeView;
				VisualTreeAsset visualTreeAsset;
				bool flag = this.m_ItemTemplate.TryGetValueFromBag(bag, cc, out visualTreeAsset);
				if (flag)
				{
					treeView.itemTemplate = visualTreeAsset;
				}
			}

			private UxmlAssetAttributeDescription<VisualTreeAsset> m_ItemTemplate = new UxmlAssetAttributeDescription<VisualTreeAsset>
			{
				name = "item-template"
			};
		}
	}
}
