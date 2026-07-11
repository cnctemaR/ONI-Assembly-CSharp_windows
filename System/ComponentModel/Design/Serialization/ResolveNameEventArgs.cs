using System;
using System.Security.Permissions;

namespace System.ComponentModel.Design.Serialization
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public class ResolveNameEventArgs : EventArgs
	{
		public ResolveNameEventArgs(string name)
		{
			this.name = name;
			this.value = null;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		private string name;

		private object value;
	}
}
