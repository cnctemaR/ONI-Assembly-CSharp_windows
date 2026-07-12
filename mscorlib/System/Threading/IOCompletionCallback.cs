using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Threading
{
	[ComVisible(true)]
	[SecurityCritical]
	[CLSCompliant(false)]
	public unsafe delegate void IOCompletionCallback(uint errorCode, uint numBytes, NativeOverlapped* pOVERLAP);
}
