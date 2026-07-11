using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[Map("struct cmsghdr")]
	[CLSCompliant(false)]
	public struct Cmsghdr
	{
		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_Cmsghdr_getsize", SetLastError = true)]
		private static extern int getsize();

		public static int Size
		{
			get
			{
				return Cmsghdr.size;
			}
		}

		public unsafe static Cmsghdr ReadFromBuffer(Msghdr msgh, long cmsg)
		{
			if (msgh == null)
			{
				throw new ArgumentNullException("msgh");
			}
			if (msgh.msg_control == null || msgh.msg_controllen > (long)msgh.msg_control.Length)
			{
				throw new ArgumentException("msgh.msg_control == null || msgh.msg_controllen > msgh.msg_control.Length", "msgh");
			}
			if (cmsg < 0L || cmsg + (long)Cmsghdr.Size > msgh.msg_controllen)
			{
				throw new ArgumentException("cmsg offset pointing out of buffer", "cmsg");
			}
			byte[] array;
			byte* ptr;
			if ((array = msgh.msg_control) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			Cmsghdr cmsghdr;
			if (!NativeConvert.TryCopy((IntPtr)((void*)(ptr + cmsg)), out cmsghdr))
			{
				throw new ArgumentException("Failed to convert from native struct", "buffer");
			}
			array = null;
			if (NativeConvert.FromUnixSocketProtocol(cmsghdr.cmsg_level) == NativeConvert.FromUnixSocketProtocol(UnixSocketProtocol.SOL_SOCKET))
			{
				cmsghdr.cmsg_level = UnixSocketProtocol.SOL_SOCKET;
			}
			return cmsghdr;
		}

		public unsafe void WriteToBuffer(Msghdr msgh, long cmsg)
		{
			if (msgh == null)
			{
				throw new ArgumentNullException("msgh");
			}
			if (msgh.msg_control == null || msgh.msg_controllen > (long)msgh.msg_control.Length)
			{
				throw new ArgumentException("msgh.msg_control == null || msgh.msg_controllen > msgh.msg_control.Length", "msgh");
			}
			if (cmsg < 0L || cmsg + (long)Cmsghdr.Size > msgh.msg_controllen)
			{
				throw new ArgumentException("cmsg offset pointing out of buffer", "cmsg");
			}
			byte[] array;
			byte* ptr;
			if ((array = msgh.msg_control) == null || array.Length == 0)
			{
				ptr = null;
			}
			else
			{
				ptr = &array[0];
			}
			if (!NativeConvert.TryCopy(ref this, (IntPtr)((void*)(ptr + cmsg))))
			{
				throw new ArgumentException("Failed to convert to native struct", "buffer");
			}
			array = null;
		}

		public long cmsg_len;

		public UnixSocketProtocol cmsg_level;

		public UnixSocketControlMessage cmsg_type;

		private static readonly int size = Cmsghdr.getsize();
	}
}
