using System;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;

namespace System.Net
{
	internal class SecurityBuffer
	{
		public SecurityBuffer(byte[] data, int offset, int size, BufferType tokentype)
		{
			this.offset = ((data == null || offset < 0) ? 0 : Math.Min(offset, data.Length));
			this.size = ((data == null || size < 0) ? 0 : Math.Min(size, data.Length - this.offset));
			this.type = tokentype;
			this.token = ((size == 0) ? null : data);
		}

		public SecurityBuffer(byte[] data, BufferType tokentype)
		{
			this.size = ((data == null) ? 0 : data.Length);
			this.type = tokentype;
			this.token = ((this.size == 0) ? null : data);
		}

		public SecurityBuffer(int size, BufferType tokentype)
		{
			this.size = size;
			this.type = tokentype;
			this.token = ((size == 0) ? null : new byte[size]);
		}

		public SecurityBuffer(ChannelBinding binding)
		{
			this.size = ((binding == null) ? 0 : binding.Size);
			this.type = BufferType.ChannelBindings;
			this.unmanagedToken = binding;
		}

		public int size;

		public BufferType type;

		public byte[] token;

		public SafeHandle unmanagedToken;

		public int offset;
	}
}
