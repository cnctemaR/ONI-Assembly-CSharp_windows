using System;
using System.Collections;

namespace System.ComponentModel
{
	public interface IBindingListView : IList, ICollection, IEnumerable, IBindingList
	{
		string Filter { get; set; }

		ListSortDescriptionCollection SortDescriptions { get; }

		bool SupportsAdvancedSorting { get; }

		bool SupportsFiltering { get; }

		void ApplySort(ListSortDescriptionCollection sorts);

		void RemoveFilter();
	}
}
