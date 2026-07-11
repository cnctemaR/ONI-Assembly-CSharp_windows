using System;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class AddingNewEventArgs : EventArgs
	{
		public AddingNewEventArgs()
		{
		}

		public AddingNewEventArgs(object newObject)
		{
			this.newObject = newObject;
		}

		public object NewObject
		{
			get
			{
				return this.newObject;
			}
			set
			{
				this.newObject = value;
			}
		}

		private object newObject;
	}
}
