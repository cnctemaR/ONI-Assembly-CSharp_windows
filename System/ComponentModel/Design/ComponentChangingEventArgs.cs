using System;
using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public sealed class ComponentChangingEventArgs : EventArgs
	{
		public ComponentChangingEventArgs(object component, MemberDescriptor member)
		{
			this.component = component;
			this.member = member;
		}

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

		private object component;

		private MemberDescriptor member;
	}
}
