using System;
using System.Security.Permissions;

namespace System.ComponentModel.Design
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class ActiveDesignerEventArgs : EventArgs
	{
		public ActiveDesignerEventArgs(IDesignerHost oldDesigner, IDesignerHost newDesigner)
		{
			this.oldDesigner = oldDesigner;
			this.newDesigner = newDesigner;
		}

		public IDesignerHost OldDesigner
		{
			get
			{
				return this.oldDesigner;
			}
		}

		public IDesignerHost NewDesigner
		{
			get
			{
				return this.newDesigner;
			}
		}

		private readonly IDesignerHost oldDesigner;

		private readonly IDesignerHost newDesigner;
	}
}
