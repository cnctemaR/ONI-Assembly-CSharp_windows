using System;
using System.Security.Permissions;

namespace System.ComponentModel.Design
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class DesignerEventArgs : EventArgs
	{
		public DesignerEventArgs(IDesignerHost host)
		{
			this.host = host;
		}

		public IDesignerHost Designer
		{
			get
			{
				return this.host;
			}
		}

		private readonly IDesignerHost host;
	}
}
