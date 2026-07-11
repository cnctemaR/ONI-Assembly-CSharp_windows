using System;

namespace System.ComponentModel
{
	public class ListChangedEventArgs : EventArgs
	{
		public ListChangedEventArgs(ListChangedType listChangedType, int newIndex)
			: this(listChangedType, newIndex, -1)
		{
		}

		public ListChangedEventArgs(ListChangedType listChangedType, PropertyDescriptor propDesc)
		{
			this.changedType = listChangedType;
			this.propDesc = propDesc;
		}

		public ListChangedEventArgs(ListChangedType listChangedType, int newIndex, int oldIndex)
		{
			this.changedType = listChangedType;
			this.newIndex = newIndex;
			this.oldIndex = oldIndex;
		}

		public ListChangedEventArgs(ListChangedType listChangedType, int newIndex, PropertyDescriptor propDesc)
		{
			this.changedType = listChangedType;
			this.newIndex = newIndex;
			this.oldIndex = newIndex;
			this.propDesc = propDesc;
		}

		public ListChangedType ListChangedType
		{
			get
			{
				return this.changedType;
			}
		}

		public int OldIndex
		{
			get
			{
				return this.oldIndex;
			}
		}

		public int NewIndex
		{
			get
			{
				return this.newIndex;
			}
		}

		public PropertyDescriptor PropertyDescriptor
		{
			get
			{
				return this.propDesc;
			}
		}

		private ListChangedType changedType;

		private int oldIndex;

		private int newIndex;

		private PropertyDescriptor propDesc;
	}
}
