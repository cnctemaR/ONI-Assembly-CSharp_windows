using System;
using Unity;

namespace System.Drawing.Drawing2D
{
	public sealed class GraphicsContainer : MarshalByRefObject
	{
		internal GraphicsContainer(uint state)
		{
			this.nativeState = state;
		}

		internal uint NativeObject
		{
			get
			{
				return this.nativeState;
			}
		}

		internal GraphicsContainer()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private uint nativeState;
	}
}
