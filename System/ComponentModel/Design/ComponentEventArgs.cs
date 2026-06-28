using System;
using System.Runtime.InteropServices;

namespace System.ComponentModel.Design
{
	[ComVisible(true)]
	public class ComponentEventArgs : EventArgs
	{
		public ComponentEventArgs(IComponent component)
		{
			this.icomp = component;
		}

		public virtual IComponent Component
		{
			get
			{
				return this.icomp;
			}
		}

		private IComponent icomp;
	}
}
