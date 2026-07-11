using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
	public sealed class ComponentChangedEventArgs : EventArgs
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

		public object NewValue
		{
			get
			{
				return this.newValue;
			}
		}

		public object OldValue
		{
			get
			{
				return this.oldValue;
			}
		}

		public ComponentChangedEventArgs(object component, MemberDescriptor member, object oldValue, object newValue)
		{
			this.component = component;
			this.member = member;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}

		private object component;

		private MemberDescriptor member;

		private object oldValue;

		private object newValue;
	}
}
