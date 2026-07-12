using System;

namespace System.ComponentModel
{
	public class AddingNewEventArgs : EventArgs
	{
		public AddingNewEventArgs()
		{
		}

		public AddingNewEventArgs(object newObject)
		{
			this.NewObject = newObject;
		}

		public object NewObject { get; set; }
	}
}
