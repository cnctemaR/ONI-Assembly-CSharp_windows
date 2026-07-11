using System;
using System.Runtime.InteropServices;

namespace System.Net.NetworkInformation.MacOsStructs
{
	internal struct sockaddr_dl
	{
		internal void Read(IntPtr ptr)
		{
			this.sdl_len = Marshal.ReadByte(ptr, 0);
			this.sdl_family = Marshal.ReadByte(ptr, 1);
			this.sdl_index = (ushort)Marshal.ReadInt16(ptr, 2);
			this.sdl_type = Marshal.ReadByte(ptr, 4);
			this.sdl_nlen = Marshal.ReadByte(ptr, 5);
			this.sdl_alen = Marshal.ReadByte(ptr, 6);
			this.sdl_slen = Marshal.ReadByte(ptr, 7);
			this.sdl_data = new byte[Math.Max(12, (int)(this.sdl_len - 8))];
			Marshal.Copy(new IntPtr(ptr.ToInt64() + 8L), this.sdl_data, 0, this.sdl_data.Length);
		}

		public byte sdl_len;

		public byte sdl_family;

		public ushort sdl_index;

		public byte sdl_type;

		public byte sdl_nlen;

		public byte sdl_alen;

		public byte sdl_slen;

		public byte[] sdl_data;
	}
}
