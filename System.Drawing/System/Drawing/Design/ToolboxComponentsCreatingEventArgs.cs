using System;
using System.ComponentModel.Design;

namespace System.Drawing.Design
{
	public class ToolboxComponentsCreatingEventArgs : EventArgs
	{
		public ToolboxComponentsCreatingEventArgs(IDesignerHost host)
		{
			this.host = host;
		}

		public IDesignerHost DesignerHost
		{
			get
			{
				return this.host;
			}
		}

		private readonly IDesignerHost host;
	}
}
