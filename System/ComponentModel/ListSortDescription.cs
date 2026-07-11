using System;

namespace System.ComponentModel
{
	public class ListSortDescription
	{
		public ListSortDescription(PropertyDescriptor propertyDescriptor, ListSortDirection sortDirection)
		{
			this.propertyDescriptor = propertyDescriptor;
			this.sortDirection = sortDirection;
		}

		public PropertyDescriptor PropertyDescriptor
		{
			get
			{
				return this.propertyDescriptor;
			}
			set
			{
				this.propertyDescriptor = value;
			}
		}

		public ListSortDirection SortDirection
		{
			get
			{
				return this.sortDirection;
			}
			set
			{
				this.sortDirection = value;
			}
		}

		private PropertyDescriptor propertyDescriptor;

		private ListSortDirection sortDirection;
	}
}
