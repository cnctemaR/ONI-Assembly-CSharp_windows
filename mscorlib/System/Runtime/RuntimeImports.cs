using System;
using System.Runtime.CompilerServices;

namespace System.Runtime
{
	public static class RuntimeImports
	{
		internal unsafe static void RhZeroMemory(ref byte b, ulong byteLength)
		{
			fixed (byte* ptr = &b)
			{
				RuntimeImports.ZeroMemory((void*)ptr, (uint)byteLength);
			}
		}

		internal unsafe static void RhZeroMemory(IntPtr p, UIntPtr byteLength)
		{
			RuntimeImports.ZeroMemory((void*)p, (uint)byteLength);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ZeroMemory(void* p, uint byteLength);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void Memmove(byte* dest, byte* src, uint len);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void Memmove_wbarrier(byte* dest, byte* src, uint len, IntPtr type_handle);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal unsafe static extern void _ecvt_s(byte* buffer, int sizeInBytes, double value, int count, int* dec, int* sign);
	}
}
