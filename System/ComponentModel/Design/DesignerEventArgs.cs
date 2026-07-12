using System;

namespace System.ComponentModel.Design
{
	public class DesignerEventArgs : EventArgs
	{
		public DesignerEventArgs(IDesignerHost host)
		{
			this.Designer = host;
		}

		public IDesignerHost Designer { get; }
	}
}
