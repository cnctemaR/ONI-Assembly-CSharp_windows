using System;

namespace System.ComponentModel
{
	public class ListSortDescription
	{
		public ListSortDescription(PropertyDescriptor property, ListSortDirection direction)
		{
			this.PropertyDescriptor = property;
			this.SortDirection = direction;
		}

		public PropertyDescriptor PropertyDescriptor { get; set; }

		public ListSortDirection SortDirection { get; set; }
	}
}
