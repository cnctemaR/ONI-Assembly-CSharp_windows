using System;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Enum)]
	public sealed class InspectorOrderAttribute : PropertyAttribute
	{
		internal InspectorSort m_inspectorSort { get; private set; }

		internal InspectorSortDirection m_sortDirection { get; private set; }

		public InspectorOrderAttribute(InspectorSort inspectorSort = InspectorSort.ByName, InspectorSortDirection sortDirection = InspectorSortDirection.Ascending)
		{
			this.m_inspectorSort = inspectorSort;
			this.m_sortDirection = sortDirection;
		}
	}
}
