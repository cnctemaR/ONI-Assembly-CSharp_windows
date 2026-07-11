using System;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class RefreshEventArgs : EventArgs
	{
		public RefreshEventArgs(object componentChanged)
		{
			this.componentChanged = componentChanged;
			this.typeChanged = componentChanged.GetType();
		}

		public RefreshEventArgs(Type typeChanged)
		{
			this.typeChanged = typeChanged;
		}

		public object ComponentChanged
		{
			get
			{
				return this.componentChanged;
			}
		}

		public Type TypeChanged
		{
			get
			{
				return this.typeChanged;
			}
		}

		private object componentChanged;

		private Type typeChanged;
	}
}
