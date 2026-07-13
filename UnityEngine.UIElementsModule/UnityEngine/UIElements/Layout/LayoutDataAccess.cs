using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.UIElements.Layout
{
	[RequiredByNativeCode]
	internal readonly struct LayoutDataAccess
	{
		public bool IsValid
		{
			get
			{
				return this.m_Nodes.IsValid && this.m_Configs.IsValid;
			}
		}

		internal LayoutDataAccess(int manager, LayoutDataStore nodes, LayoutDataStore configs)
		{
			this.m_Manager = manager;
			this.m_Nodes = nodes;
			this.m_Configs = configs;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe ref T GetTypedNodeDataRef<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(LayoutHandle handle, LayoutNodeDataType type) where T : struct, ValueType
		{
			return ref *(T*)this.m_Nodes.GetComponentDataPtr(handle.Index, (int)type);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe ref T GetTypedConfigDataRef<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(LayoutHandle handle, LayoutConfigDataType type) where T : struct, ValueType
		{
			return ref *(T*)this.m_Configs.GetComponentDataPtr(handle.Index, (int)type);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref LayoutNodeData GetNodeData(LayoutHandle handle)
		{
			return this.GetTypedNodeDataRef<LayoutNodeData>(handle, LayoutNodeDataType.Node);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref LayoutStyleData GetStyleData(LayoutHandle handle)
		{
			return this.GetTypedNodeDataRef<LayoutStyleData>(handle, LayoutNodeDataType.Style);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref LayoutComputedData GetComputedData(LayoutHandle handle)
		{
			return this.GetTypedNodeDataRef<LayoutComputedData>(handle, LayoutNodeDataType.Computed);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref LayoutCacheData GetCacheData(LayoutHandle handle)
		{
			return this.GetTypedNodeDataRef<LayoutCacheData>(handle, LayoutNodeDataType.Cache);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref LayoutConfigData GetConfigData(LayoutHandle handle)
		{
			return this.GetTypedConfigDataRef<LayoutConfigData>(handle, LayoutConfigDataType.Config);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public LayoutMeasureFunction GetMeasureFunction(LayoutHandle handle)
		{
			return LayoutManager.GetManager(this.m_Manager).GetMeasureFunction(handle);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetMeasureFunction(LayoutHandle handle, LayoutMeasureFunction value)
		{
			LayoutManager.GetManager(this.m_Manager).SetMeasureFunction(handle, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public VisualElement GetOwner(LayoutHandle handle)
		{
			return LayoutManager.GetManager(this.m_Manager).GetOwner(handle);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetOwner(LayoutHandle handle, VisualElement value)
		{
			LayoutManager.GetManager(this.m_Manager).SetOwner(handle, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public LayoutBaselineFunction GetBaselineFunction(LayoutHandle handle)
		{
			return LayoutManager.GetManager(this.m_Manager).GetBaselineFunction(handle);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetBaselineFunction(LayoutHandle handle, LayoutBaselineFunction value)
		{
			LayoutManager.GetManager(this.m_Manager).SetBaselineFunction(handle, value);
		}

		private readonly int m_Manager;

		private readonly LayoutDataStore m_Nodes;

		private readonly LayoutDataStore m_Configs;
	}
}
