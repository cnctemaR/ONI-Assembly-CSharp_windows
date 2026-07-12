using System;

namespace System.ComponentModel
{
	public class ListChangedEventArgs : EventArgs
	{
		public ListChangedEventArgs(ListChangedType listChangedType, int newIndex)
			: this(listChangedType, newIndex, -1)
		{
		}

		public ListChangedEventArgs(ListChangedType listChangedType, int newIndex, PropertyDescriptor propDesc)
			: this(listChangedType, newIndex)
		{
			this.PropertyDescriptor = propDesc;
			this.OldIndex = newIndex;
		}

		public ListChangedEventArgs(ListChangedType listChangedType, PropertyDescriptor propDesc)
		{
			this.ListChangedType = listChangedType;
			this.PropertyDescriptor = propDesc;
		}

		public ListChangedEventArgs(ListChangedType listChangedType, int newIndex, int oldIndex)
		{
			this.ListChangedType = listChangedType;
			this.NewIndex = newIndex;
			this.OldIndex = oldIndex;
		}

		public ListChangedType ListChangedType { get; }

		public int NewIndex { get; }

		public int OldIndex { get; }

		public PropertyDescriptor PropertyDescriptor { get; }
	}
}
