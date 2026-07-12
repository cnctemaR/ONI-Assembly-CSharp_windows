using System;

namespace System.ComponentModel.Design
{
	public sealed class ComponentChangedEventArgs : EventArgs
	{
		public object Component { get; }

		public MemberDescriptor Member { get; }

		public object NewValue { get; }

		public object OldValue { get; }

		public ComponentChangedEventArgs(object component, MemberDescriptor member, object oldValue, object newValue)
		{
			this.Component = component;
			this.Member = member;
			this.OldValue = oldValue;
			this.NewValue = newValue;
		}
	}
}
