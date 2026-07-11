using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class ComponentEventArgs : EventArgs
	{
		public virtual IComponent Component
		{
			get
			{
				return this.component;
			}
		}

		public ComponentEventArgs(IComponent component)
		{
			this.component = component;
		}

		private IComponent component;
	}
}
