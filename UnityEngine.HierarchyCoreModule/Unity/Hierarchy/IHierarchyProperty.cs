using System;

namespace Unity.Hierarchy
{
	public interface IHierarchyProperty<T>
	{
		bool IsCreated { get; }

		T GetValue(in HierarchyNode node);

		void SetValue(in HierarchyNode node, T value);

		void ClearValue(in HierarchyNode node);
	}
}
