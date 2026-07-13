using System;
using Unity.Scripting.LifecycleManagement;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Unity.Hierarchy
{
	internal class HierarchyViewItemContainer : VisualElement
	{
		public HierarchyView View
		{
			get
			{
				return this.m_View;
			}
		}

		public HierarchyViewItem ViewItem
		{
			get
			{
				return this.m_ViewItem;
			}
		}

		public HierarchyNodeTypeHandler ViewItemNodeTypeHandler
		{
			get
			{
				return this.m_ViewItemNodeTypeHandler;
			}
		}

		public void Bind(in HierarchyNode node, HierarchyView view)
		{
			bool flag = (in node) == HierarchyNode.Null;
			if (flag)
			{
				throw new ArgumentNullException("node");
			}
			bool flag2 = view == null;
			if (flag2)
			{
				throw new ArgumentNullException("view");
			}
			HierarchyNodeTypeHandler nodeTypeHandler = view.Source.GetNodeTypeHandler(in node);
			bool flag3 = this.m_ViewItem == null || this.m_ViewItemNodeTypeHandler != nodeTypeHandler;
			if (flag3)
			{
				this.ReleaseViewItem();
				this.m_ViewItem = ((nodeTypeHandler != null) ? nodeTypeHandler.ViewItemPool.Get() : HierarchyViewItemContainer.s_ViewItemPool.Get());
				bool flag4 = this.m_ViewItem == null;
				if (flag4)
				{
					throw new NullReferenceException("Failed to get a view item from the pool");
				}
				base.Add(this.m_ViewItem);
				this.m_ViewItemNodeTypeHandler = nodeTypeHandler;
			}
			this.m_View = view;
			this.m_ViewItem.Bind(in node, this.m_View);
		}

		public void Unbind()
		{
			HierarchyViewItem viewItem = this.m_ViewItem;
			if (viewItem != null)
			{
				viewItem.Unbind();
			}
		}

		public void ReleaseViewItem()
		{
			bool flag = this.m_ViewItem != null;
			if (flag)
			{
				bool bound = this.m_ViewItem.Bound;
				if (bound)
				{
					this.m_ViewItem.Unbind();
				}
				base.Remove(this.m_ViewItem);
				bool flag2 = this.m_ViewItemNodeTypeHandler != null;
				if (flag2)
				{
					this.m_ViewItemNodeTypeHandler.ViewItemPool.Release(this.m_ViewItem);
				}
				else
				{
					HierarchyViewItemContainer.s_ViewItemPool.Release(this.m_ViewItem);
				}
				this.m_ViewItem = null;
			}
			this.m_View = null;
			this.m_ViewItemNodeTypeHandler = null;
		}

		[AutoStaticsCleanupOnCodeReload(CleanupStrategy = CleanupStrategy.Clear)]
		private static readonly UnityEngine.Pool.ObjectPool<HierarchyViewItem> s_ViewItemPool = new UnityEngine.Pool.ObjectPool<HierarchyViewItem>(() => new HierarchyViewItem(), null, null, null, true, 10, 10000);

		private HierarchyView m_View;

		private HierarchyViewItem m_ViewItem;

		private HierarchyNodeTypeHandler m_ViewItemNodeTypeHandler;
	}
}
