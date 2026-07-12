using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Authentication.ExtendedProtection;

namespace System.Net.Security
{
	internal class SecurityBuffer
	{
		public SecurityBuffer(byte[] data, int offset, int size, SecurityBufferType tokentype)
		{
			if (offset < 0 || offset > ((data == null) ? 0 : data.Length))
			{
				NetEventSource.Fail(this, FormattableStringFactory.Create("'offset' out of range.  [{0}]", new object[] { offset }), ".ctor");
			}
			if (size < 0 || size > ((data == null) ? 0 : (data.Length - offset)))
			{
				NetEventSource.Fail(this, FormattableStringFactory.Create("'size' out of range.  [{0}]", new object[] { size }), ".ctor");
			}
			this.offset = ((data == null || offset < 0) ? 0 : Math.Min(offset, data.Length));
			this.size = ((data == null || size < 0) ? 0 : Math.Min(size, data.Length - this.offset));
			this.type = tokentype;
			this.token = ((size == 0) ? null : data);
		}

		public SecurityBuffer(byte[] data, SecurityBufferType tokentype)
		{
			this.size = ((data == null) ? 0 : data.Length);
			this.type = tokentype;
			this.token = ((this.size == 0) ? null : data);
		}

		public SecurityBuffer(int size, SecurityBufferType tokentype)
		{
			if (size < 0)
			{
				NetEventSource.Fail(this, FormattableStringFactory.Create("'size' out of range.  [{0}]", new object[] { size }), ".ctor");
			}
			this.size = size;
			this.type = tokentype;
			this.token = ((size == 0) ? null : new byte[size]);
		}

		public SecurityBuffer(ChannelBinding binding)
		{
			this.size = ((binding == null) ? 0 : binding.Size);
			this.type = SecurityBufferType.SECBUFFER_CHANNEL_BINDINGS;
			this.unmanagedToken = binding;
		}

		public int size;

		public SecurityBufferType type;

		public byte[] token;

		public SafeHandle unmanagedToken;

		public int offset;
	}
}
