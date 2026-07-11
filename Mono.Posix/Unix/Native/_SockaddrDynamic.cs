using System;

namespace Mono.Unix.Native
{
	[Map]
	internal struct _SockaddrDynamic
	{
		public unsafe _SockaddrDynamic(Sockaddr address, byte* data, bool useMaxLength)
		{
			if (data == null)
			{
				this = default(_SockaddrDynamic);
				return;
			}
			byte[] array = address.DynamicData();
			this.type = address.type & (SockaddrType)(-32769);
			this.sa_family = address.sa_family;
			this.data = data;
			if (useMaxLength)
			{
				this.len = (long)array.Length;
				return;
			}
			this.len = address.GetDynamicLength();
			if (this.len < 0L || this.len > (long)array.Length)
			{
				throw new ArgumentException("len < 0 || len > dynData.Length", "address");
			}
		}

		public void Update(Sockaddr address)
		{
			if (this.data == null)
			{
				return;
			}
			address.sa_family = this.sa_family;
			address.SetDynamicLength(this.len);
		}

		public SockaddrType type;

		public UnixAddressFamily sa_family;

		public unsafe byte* data;

		public long len;
	}
}
