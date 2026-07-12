using System;

namespace System.ComponentModel.Design
{
	public class ActiveDesignerEventArgs : EventArgs
	{
		public ActiveDesignerEventArgs(IDesignerHost oldDesigner, IDesignerHost newDesigner)
		{
			this.OldDesigner = oldDesigner;
			this.NewDesigner = newDesigner;
		}

		public IDesignerHost OldDesigner { get; }

		public IDesignerHost NewDesigner { get; }
	}
}
