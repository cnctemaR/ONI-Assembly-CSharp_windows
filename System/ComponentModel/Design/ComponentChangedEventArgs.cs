using System;
using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public sealed class ComponentChangedEventArgs : EventArgs
	{
		public ComponentChangedEventArgs(object component, MemberDescriptor member, object oldValue, object newValue)
		{
			this.component = component;
			this.member = member;
			this.oldValue = oldValue;
			this.newValue = newValue;
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

		public object NewValue
		{
			get
			{
				return this.oldValue;
			}
		}

		public object OldValue
		{
			get
			{
				return this.newValue;
			}
		}

		private object component;

		private MemberDescriptor member;

		private object oldValue;

		private object newValue;
	}
}
