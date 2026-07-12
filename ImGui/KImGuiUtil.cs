using System;
using System.Runtime.InteropServices;

public static class KImGuiUtil
{
	[DllImport("cimgui", CallingConvention = CallingConvention.Cdecl)]
	public static extern void SetKAssertCB(KImGuiUtil.KAssertCB cb);

	public static void KAssertHandler(IntPtr msg)
	{
		throw new Exception(Marshal.PtrToStringAnsi(msg));
	}

	public delegate void KAssertCB(IntPtr msg);
}
