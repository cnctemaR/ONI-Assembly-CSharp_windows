using System;

namespace System.ComponentModel.Design
{
	public sealed class ComponentChangingEventArgs : EventArgs
	{
		public object Component { get; }

		public MemberDescriptor Member { get; }

		public ComponentChangingEventArgs(object component, MemberDescriptor member)
		{
			this.Component = component;
			this.Member = member;
		}
	}
}
