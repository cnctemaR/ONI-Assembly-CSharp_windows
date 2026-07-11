using System;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class ListSortDescription
	{
		public ListSortDescription(PropertyDescriptor property, ListSortDirection direction)
		{
			this.property = property;
			this.sortDirection = direction;
		}

		public PropertyDescriptor PropertyDescriptor
		{
			get
			{
				return this.property;
			}
			set
			{
				this.property = value;
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

		private PropertyDescriptor property;

		private ListSortDirection sortDirection;
	}
}
