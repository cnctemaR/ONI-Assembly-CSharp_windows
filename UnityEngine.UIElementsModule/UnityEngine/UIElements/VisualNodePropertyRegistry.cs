using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Hierarchy;

namespace UnityEngine.UIElements
{
	internal class VisualNodePropertyRegistry
	{
		public static void RegisterInternalProperty<TProperty>()
		{
			bool flag = VisualNodePropertyRegistry.TypeIndex<TProperty>.Index != 0;
			if (flag)
			{
				throw new InvalidOperationException("TProperty has already been registered");
			}
			VisualNodePropertyRegistry.TypeIndex<TProperty>.Index = -(++VisualNodePropertyRegistry.s_InternalPropertyCount);
		}

		public static void RegisterHierarchyProperty<TProperty>()
		{
			bool flag = VisualNodePropertyRegistry.TypeIndex<TProperty>.Index != 0;
			if (flag)
			{
				throw new InvalidOperationException("TProperty has already been registered");
			}
			VisualNodePropertyRegistry.TypeIndex<TProperty>.Index = ++VisualNodePropertyRegistry.s_HierarchyPropertyCount;
		}

		public unsafe VisualNodePropertyRegistry(VisualManager manager)
		{
			if (manager == null)
			{
				throw new ArgumentNullException("manager");
			}
			this.m_Manager = manager;
			this.m_InternalPropertyData = new VisualNodePropertyData*[VisualNodePropertyRegistry.s_InternalPropertyCount];
			for (int i = 0; i < VisualNodePropertyRegistry.s_InternalPropertyCount; i++)
			{
				this.m_InternalPropertyData[i] = (VisualNodePropertyData*)(void*)this.m_Manager.GetPropertyPtr(i);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsInternalProperty(int typeIndex)
		{
			return typeIndex < 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private VisualNodeProperty<T> GetInternalProperty<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(int typeIndex) where T : struct, ValueType
		{
			return new VisualNodeProperty<T>(this.m_InternalPropertyData[-typeIndex - 1]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private HierarchyPropertyUnmanaged<T> GetHierarchyProperty<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(int typeIndex) where T : struct, ValueType
		{
			throw new NotImplementedException();
		}

		public unsafe T GetProperty<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(VisualNodeHandle handle) where T : struct, ValueType
		{
			int index = VisualNodePropertyRegistry.TypeIndex<T>.Index;
			bool flag = index == 0;
			if (flag)
			{
				throw new InvalidOperationException("The property type has not been registered");
			}
			bool flag2 = this.IsInternalProperty(index);
			T t;
			if (flag2)
			{
				t = *this.GetInternalProperty<T>(index)[handle];
			}
			else
			{
				t = this.GetHierarchyProperty<T>(index).GetValue(UnsafeUtility.As<VisualNodeHandle, HierarchyNode>(ref handle));
			}
			return t;
		}

		public unsafe void SetProperty<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(VisualNodeHandle handle, in T value) where T : struct, ValueType
		{
			int index = VisualNodePropertyRegistry.TypeIndex<T>.Index;
			bool flag = index == 0;
			if (flag)
			{
				throw new InvalidOperationException("The property type has not been registered");
			}
			bool flag2 = this.IsInternalProperty(index);
			if (flag2)
			{
				*this.GetInternalProperty<T>(index)[handle] = value;
			}
			else
			{
				this.GetHierarchyProperty<T>(index).SetValue(UnsafeUtility.As<VisualNodeHandle, HierarchyNode>(ref handle), value);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetPropertyRef<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(VisualNodeHandle handle) where T : struct, ValueType
		{
			int index = VisualNodePropertyRegistry.TypeIndex<T>.Index;
			bool flag = index == 0;
			if (flag)
			{
				throw new InvalidOperationException("The property type has not been registered");
			}
			bool flag2 = index > 0;
			if (flag2)
			{
				throw new InvalidOperationException("The property type is not an internal property");
			}
			return this.GetInternalProperty<T>(index)[handle];
		}

		private static int s_InternalPropertyCount;

		private static int s_HierarchyPropertyCount;

		private readonly VisualManager m_Manager;

		private unsafe readonly VisualNodePropertyData*[] m_InternalPropertyData;

		private readonly ChunkAllocatingArray<object> m_Bindings = new ChunkAllocatingArray<object>();

		private struct TypeIndex<T>
		{
			public static int Index;
		}

		private class HierarchyPropertyBinding<[global::System.Runtime.CompilerServices.IsUnmanaged] TProperty> where TProperty : struct, ValueType
		{
			public HierarchyPropertyBinding(HierarchyPropertyUnmanaged<TProperty> property)
			{
				this.Property = property;
			}

			public readonly HierarchyPropertyUnmanaged<TProperty> Property;
		}
	}
}
