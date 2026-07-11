using System;

namespace System.ComponentModel
{
	public class AddingNewEventArgs : EventArgs
	{
		public AddingNewEventArgs()
			: this(null)
		{
		}

		public AddingNewEventArgs(object newObject)
		{
			this.obj = newObject;
		}

		public object NewObject
		{
			get
			{
				return this.obj;
			}
			set
			{
				this.obj = value;
			}
		}

		private object obj;
	}
}
