using System;
using System.ComponentModel;

namespace System.Drawing.Design
{
	public class ToolboxComponentsCreatedEventArgs : EventArgs
	{
		public ToolboxComponentsCreatedEventArgs(IComponent[] components)
		{
			this.comps = components;
		}

		public IComponent[] Components
		{
			get
			{
				return (IComponent[])this.comps.Clone();
			}
		}

		private readonly IComponent[] comps;
	}
}
