using System;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class DataErrorsChangedEventArgs : EventArgs
	{
		public DataErrorsChangedEventArgs(string propertyName)
		{
			this.propertyName = propertyName;
		}

		public virtual string PropertyName
		{
			get
			{
				return this.propertyName;
			}
		}

		private readonly string propertyName;
	}
}
