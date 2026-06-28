using System;

namespace System.ComponentModel.Design
{
	public class ActiveDesignerEventArgs : EventArgs
	{
		public ActiveDesignerEventArgs(IDesignerHost oldDesigner, IDesignerHost newDesigner)
		{
			this.oldDesigner = oldDesigner;
			this.newDesigner = newDesigner;
		}

		public IDesignerHost NewDesigner
		{
			get
			{
				return this.newDesigner;
			}
		}

		public IDesignerHost OldDesigner
		{
			get
			{
				return this.oldDesigner;
			}
		}

		private IDesignerHost oldDesigner;

		private IDesignerHost newDesigner;
	}
}
