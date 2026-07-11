using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	public sealed class ComponentChangingEventArgs : EventArgs
	{
		public object Component
		{
			get
			{
				return this.component;
			}
		}

		public MemberDescriptor Member
		{
			get
			{
				return this.member;
			}
		}

		public ComponentChangingEventArgs(object component, MemberDescriptor member)
		{
			this.component = component;
			this.member = member;
		}

		private object component;

		private MemberDescriptor member;
	}
}
