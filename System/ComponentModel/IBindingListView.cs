using System;
using System.Collections;

namespace System.ComponentModel
{
	public interface IBindingListView : IBindingList, IList, ICollection, IEnumerable
	{
		void ApplySort(ListSortDescriptionCollection sorts);

		string Filter { get; set; }

		ListSortDescriptionCollection SortDescriptions { get; }

		void RemoveFilter();

		bool SupportsAdvancedSorting { get; }

		bool SupportsFiltering { get; }
	}
}
