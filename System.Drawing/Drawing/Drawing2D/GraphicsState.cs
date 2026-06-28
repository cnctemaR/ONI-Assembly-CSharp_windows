using System;

namespace System.Drawing.Drawing2D
{
	public sealed class GraphicsState : MarshalByRefObject
	{
		internal GraphicsState()
		{
		}

		internal uint nativeState;
	}
}
