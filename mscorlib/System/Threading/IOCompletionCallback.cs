using System;
using System.Runtime.InteropServices;

namespace System.Threading
{
	[ComVisible(true)]
	[CLSCompliant(false)]
	public unsafe delegate void IOCompletionCallback(uint errorCode, uint numBytes, NativeOverlapped* pOVERLAP);
}
