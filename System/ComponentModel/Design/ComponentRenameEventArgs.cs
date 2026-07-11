using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class ComponentRenameEventArgs : EventArgs
	{
		public object Component
		{
			get
			{
				return this.component;
			}
		}

		public virtual string OldName
		{
			get
			{
				return this.oldName;
			}
		}

		public virtual string NewName
		{
			get
			{
				return this.newName;
			}
		}

		public ComponentRenameEventArgs(object component, string oldName, string newName)
		{
			this.oldName = oldName;
			this.newName = newName;
			this.component = component;
		}

		private object component;

		private string oldName;

		private string newName;
	}
}
