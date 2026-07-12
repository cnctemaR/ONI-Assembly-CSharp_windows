using System;

namespace System.ComponentModel.Design
{
	public class ComponentEventArgs : EventArgs
	{
		public virtual IComponent Component { get; }

		public ComponentEventArgs(IComponent component)
		{
			this.Component = component;
		}
	}
}
