using System;
using System.Collections;

namespace UnityEngine.UIElements
{
	public class ListView : BaseListView
	{
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
				}
			}
		}

		internal void SetMakeItemWithoutNotify(Func<VisualElement> func)
		{
			this.m_MakeItem = func;
		}

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
				}
			}
		}

		internal void SetBindItemWithoutNotify(Action<VisualElement, int> callback)
		{
			this.m_BindItem = callback;
		}

		public new Action<VisualElement, int> unbindItem { get; set; }

		public new Action<VisualElement> destroyItem { get; set; }

		internal override bool HasValidDataAndBindings()
		{
			return base.HasValidDataAndBindings() && this.makeItem != null == (this.bindItem != null);
		}

		protected override CollectionViewController CreateViewController()
		{
			return new ListViewController();
		}

		public ListView()
		{
			base.AddToClassList(BaseListView.ussClassName);
		}

		public ListView(IList itemsSource, float itemHeight = -1f, Func<VisualElement> makeItem = null, Action<VisualElement, int> bindItem = null)
			: base(itemsSource, itemHeight)
		{
			base.AddToClassList(BaseListView.ussClassName);
			this.makeItem = makeItem;
			this.bindItem = bindItem;
		}

		private Func<VisualElement> m_MakeItem;

		private Action<VisualElement, int> m_BindItem;

		public new class UxmlFactory : UxmlFactory<ListView, ListView.UxmlTraits>
		{
		}

		public new class UxmlTraits : BaseListView.UxmlTraits
		{
		}
	}
}
