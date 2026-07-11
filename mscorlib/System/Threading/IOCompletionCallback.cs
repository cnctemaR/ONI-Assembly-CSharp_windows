using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Threading
{
	[CLSCompliant(false)]
	[SecurityCritical]
	[ComVisible(true)]
	public unsafe delegate void IOCompletionCallback(uint errorCode, uint numBytes, NativeOverlapped* pOVERLAP);
}
