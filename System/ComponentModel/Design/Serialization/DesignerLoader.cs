using System;
using System.Runtime.InteropServices;

namespace System.ComponentModel.Design.Serialization
{
	[ComVisible(true)]
	public abstract class DesignerLoader
	{
		public virtual bool Loading
		{
			get
			{
				return false;
			}
		}

		public abstract void BeginLoad(IDesignerLoaderHost host);

		public abstract void Dispose();

		public virtual void Flush()
		{
		}
	}
}
