using System;
using System.Collections;
using System.Diagnostics;
using Unity.Properties;
using UnityEngine.Internal;

namespace UnityEngine.UIElements
{
	public class ListView : BaseListView
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
					base.NotifyPropertyChanged(in ListView.makeItemProperty);
				}
			}
		}

		internal void SetMakeItemWithoutNotify(Func<VisualElement> func)
		{
			this.m_MakeItem = func;
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
					bool flag2 = this.m_TemplateMakeItem != this.makeItem;
					if (flag2)
					{
						this.makeItem = this.m_TemplateMakeItem;
					}
					else
					{
						base.Rebuild();
					}
					base.NotifyPropertyChanged(in ListView.itemTemplateProperty);
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
					base.NotifyPropertyChanged(in ListView.bindItemProperty);
				}
			}
		}

		internal void SetBindItemWithoutNotify(Action<VisualElement, int> callback)
		{
			this.m_BindItem = callback;
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
				bool flag = value == this.m_UnbindItem;
				if (!flag)
				{
					this.m_UnbindItem = value;
					base.NotifyPropertyChanged(in ListView.unbindItemProperty);
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
				bool flag = value == this.m_DestroyItem;
				if (!flag)
				{
					this.m_DestroyItem = value;
					base.NotifyPropertyChanged(in ListView.destroyItemProperty);
				}
			}
		}

		internal override bool HasValidDataAndBindings()
		{
			return base.HasValidDataAndBindings() && ((base.autoAssignSource && this.makeItem != null) || this.makeItem != null == (this.bindItem != null));
		}

		protected override CollectionViewController CreateViewController()
		{
			return new ListViewController();
		}

		public ListView()
		{
			base.AddToClassList(BaseListView.ussClassName);
			this.m_TemplateMakeItem = new Func<VisualElement>(this.TemplateMakeItem);
		}

		public ListView(IList itemsSource, float itemHeight = -1f, Func<VisualElement> makeItem = null, Action<VisualElement, int> bindItem = null)
			: base(itemsSource, itemHeight)
		{
			base.AddToClassList(BaseListView.ussClassName);
			this.m_TemplateMakeItem = new Func<VisualElement>(this.TemplateMakeItem);
			this.makeItem = makeItem;
			this.bindItem = bindItem;
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
		public new class UxmlSerializedData : BaseListView.UxmlSerializedData
		{
			[Conditional("UNITY_EDITOR")]
			public new static void Register()
			{
				UxmlDescriptionCache.RegisterType(typeof(ListView.UxmlSerializedData), new UxmlAttributeNames[]
				{
					new UxmlAttributeNames("itemTemplate", "item-template", null, Array.Empty<string>())
				}, false);
			}

			public override object CreateInstance()
			{
				return new ListView();
			}

			public override void Deserialize(object obj)
			{
				base.Deserialize(obj);
				bool flag = UnityEngine.UIElements.UxmlSerializedData.ShouldWriteAttributeValue(this.itemTemplate_UxmlAttributeFlags);
				if (flag)
				{
					ListView listView = (ListView)obj;
					listView.itemTemplate = this.itemTemplate;
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
		public new class UxmlFactory : UxmlFactory<ListView, ListView.UxmlTraits>
		{
		}

		[Obsolete("UxmlTraits is deprecated and will be removed. Use UxmlElementAttribute instead.", false)]
		public new class UxmlTraits : BaseListView.UxmlTraits
		{
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				ListView listView = ve as ListView;
				VisualTreeAsset visualTreeAsset;
				bool flag = this.m_ItemTemplate.TryGetValueFromBag(bag, cc, out visualTreeAsset);
				if (flag)
				{
					listView.itemTemplate = visualTreeAsset;
				}
			}

			private UxmlAssetAttributeDescription<VisualTreeAsset> m_ItemTemplate = new UxmlAssetAttributeDescription<VisualTreeAsset>
			{
				name = "item-template"
			};
		}
	}
}
