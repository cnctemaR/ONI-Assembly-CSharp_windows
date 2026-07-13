using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MonoMod.Utils.Interop
{
	internal static class OSX
	{
		[DllImport("libSystem", CallingConvention = CallingConvention.Cdecl, EntryPoint = "uname", SetLastError = true)]
		public unsafe static extern int Uname(byte* buf);

		[Nullable(1)]
		public const string LibSystem = "libSystem";
	}
}
