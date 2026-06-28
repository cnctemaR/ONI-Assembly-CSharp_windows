using System;

namespace System.ComponentModel.Design
{
	public class DesignerEventArgs : EventArgs
	{
		public DesignerEventArgs(IDesignerHost host)
		{
			this.host = host;
		}

		public IDesignerHost Designer
		{
			get
			{
				return this.host;
			}
		}

		private IDesignerHost host;
	}
}
