using System;
using Unity;

namespace System.Drawing.Drawing2D
{
	public sealed class GraphicsState : MarshalByRefObject
	{
		internal GraphicsState(int nativeState)
		{
			this.nativeState = nativeState;
		}

		internal GraphicsState()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		internal int nativeState;
	}
}
