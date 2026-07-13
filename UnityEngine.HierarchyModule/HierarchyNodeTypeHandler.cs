using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.Scripting;

namespace Unity.Hierarchy
{
	[RequiredByNativeCode(Optional = true)]
	[VisibleToOtherModules(new string[] { "UnityEditor.UIToolkitAuthoringModule" })]
	[StructLayout(LayoutKind.Sequential)]
	internal abstract class HierarchyNodeTypeHandler : HierarchyNodeTypeHandlerBase
	{
		internal ObjectPool<HierarchyViewItem> ViewItemPool
		{
			get
			{
				return this.m_ViewItemPool.Value;
			}
		}

		protected HierarchyNodeTypeHandler()
		{
			this.m_ViewItemPool = new Lazy<ObjectPool<HierarchyViewItem>>(() => new ObjectPool<HierarchyViewItem>(() => new HierarchyViewItem(), null, null, null, true, 0, 10000));
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.HierarchyModule" })]
		internal HierarchyNodeTypeHandler(IntPtr nativePtr, Hierarchy hierarchy, HierarchyCommandList cmdList)
			: base(nativePtr, hierarchy, cmdList)
		{
			this.m_ViewItemPool = new Lazy<ObjectPool<HierarchyViewItem>>(() => new ObjectPool<HierarchyViewItem>(() => new HierarchyViewItem(), null, null, null, true, 0, 10000));
		}

		protected virtual void OnBindView(HierarchyView view)
		{
		}

		protected virtual void OnUnbindView(HierarchyView view)
		{
		}

		protected virtual void OnBindItem(HierarchyViewItem item)
		{
		}

		protected virtual void OnUnbindItem(HierarchyViewItem item)
		{
		}

		internal void Internal_BindView(HierarchyView view)
		{
			this.OnBindView(view);
		}

		internal void Internal_UnbindView(HierarchyView view)
		{
			this.OnUnbindView(view);
		}

		internal void Internal_BindItem(HierarchyViewItem item)
		{
			this.OnBindItem(item);
		}

		internal void Internal_UnbindItem(HierarchyViewItem item)
		{
			this.OnUnbindItem(item);
		}

		private readonly Lazy<ObjectPool<HierarchyViewItem>> m_ViewItemPool;
	}
}
