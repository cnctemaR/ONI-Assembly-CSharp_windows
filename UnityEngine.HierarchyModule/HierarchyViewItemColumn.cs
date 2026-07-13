using System;
using System.Diagnostics;
using Unity.Scripting.LifecycleManagement;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.UIElements;

namespace Unity.Hierarchy
{
	[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
	internal class HierarchyViewItemColumn : Column
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<HierarchyViewItem> OnBindItem;

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<HierarchyViewItem> OnUnbindItem;

		public HierarchyViewItemColumn(HierarchyView view)
		{
			this.m_View = view;
			this.m_ViewItemContainerPool = new UnityEngine.Pool.ObjectPool<HierarchyViewItemContainer>(() => new HierarchyViewItemContainer(), null, null, null, true, 10, 10000);
			base.title = "Name";
			base.name = "HierarchyViewColumn Name";
			this.ApplyDefaultColumnProperties();
			base.makeCell = new Func<VisualElement>(this.MakeCell);
			base.destroyCell = new Action<VisualElement>(this.DestroyCell);
			base.bindCell = new Action<VisualElement, int>(this.BindCell);
			base.unbindCell = new Action<VisualElement, int>(this.UnbindCell);
			base.propertyChanged += delegate(object _, BindablePropertyChangedEventArgs args)
			{
				BindingId propertyName = args.propertyName;
				bool flag = (in propertyName) == (in HierarchyViewItemColumn.k_ColumnStretchableProperty);
				if (flag)
				{
					base.minWidth = (base.stretchable ? HierarchyViewItemColumn.k_MinimumWidth : HierarchyViewItemColumn.k_DefaultMinimumWidth);
					bool flag2 = base.width.value < base.minWidth.value;
					if (flag2)
					{
						base.width = base.minWidth;
					}
				}
			};
		}

		private VisualElement MakeCell()
		{
			return this.m_ViewItemContainerPool.Get();
		}

		internal void ApplyDefaultColumnProperties()
		{
			base.width = HierarchyViewItemColumn.k_DefaultWidth;
			base.minWidth = (base.stretchable ? HierarchyViewItemColumn.k_MinimumWidth : HierarchyViewItemColumn.k_DefaultMinimumWidth);
			base.visible = true;
			base.optional = false;
			base.resizable = true;
			base.sortable = false;
		}

		private void DestroyCell(VisualElement element)
		{
			bool flag = element == null;
			if (flag)
			{
				throw new ArgumentNullException("element");
			}
			HierarchyViewItemContainer hierarchyViewItemContainer = element as HierarchyViewItemContainer;
			bool flag2 = hierarchyViewItemContainer == null;
			if (flag2)
			{
				throw new ArgumentException("Expected element to be a HierarchyViewItemContainer");
			}
			hierarchyViewItemContainer.ReleaseViewItem();
			this.m_ViewItemContainerPool.Release(hierarchyViewItemContainer);
		}

		private unsafe void BindCell(VisualElement element, int index)
		{
			bool flag = element == null;
			if (flag)
			{
				throw new ArgumentNullException("element");
			}
			HierarchyViewItemContainer hierarchyViewItemContainer = element as HierarchyViewItemContainer;
			bool flag2 = hierarchyViewItemContainer == null;
			if (flag2)
			{
				throw new ArgumentException("Expected element to be a HierarchyViewItemContainer");
			}
			HierarchyNode hierarchyNode = *this.m_View.ViewModel[index];
			bool flag3 = (in hierarchyNode) == HierarchyNode.Null;
			if (flag3)
			{
				throw new InvalidOperationException("Expected node to be valid");
			}
			bool flag4 = !this.m_View.Source.Exists(in hierarchyNode);
			if (!flag4)
			{
				hierarchyViewItemContainer.Bind(in hierarchyNode, this.m_View);
				Action<HierarchyViewItem> onBindItem = this.OnBindItem;
				if (onBindItem != null)
				{
					onBindItem(hierarchyViewItemContainer.ViewItem);
				}
			}
		}

		private void UnbindCell(VisualElement element, int index)
		{
			bool flag = element == null;
			if (flag)
			{
				throw new ArgumentNullException("element");
			}
			HierarchyViewItemContainer hierarchyViewItemContainer = element as HierarchyViewItemContainer;
			bool flag2 = hierarchyViewItemContainer == null;
			if (flag2)
			{
				throw new ArgumentException("Expected element to be a HierarchyViewItemContainer");
			}
			bool flag3 = hierarchyViewItemContainer.ViewItem != null;
			if (flag3)
			{
				Action<HierarchyViewItem> onUnbindItem = this.OnUnbindItem;
				if (onUnbindItem != null)
				{
					onUnbindItem(hierarchyViewItemContainer.ViewItem);
				}
			}
			hierarchyViewItemContainer.Unbind();
		}

		internal const string k_HierarchyNameColumnName = "HierarchyViewColumn Name";

		[NoAutoStaticsCleanup]
		private static readonly BindingId k_ColumnStretchableProperty = "stretchable";

		private static readonly Length k_DefaultMinimumWidth = new Length(35f, LengthUnit.Pixel);

		private static readonly Length k_MinimumWidth = new Length(200f, LengthUnit.Pixel);

		private static readonly Length k_DefaultWidth = new Length(300f, LengthUnit.Pixel);

		private readonly HierarchyView m_View;

		private readonly UnityEngine.Pool.ObjectPool<HierarchyViewItemContainer> m_ViewItemContainerPool;
	}
}
