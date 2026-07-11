using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[Map("struct sockaddr_in6")]
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class SockaddrIn6 : Sockaddr, IEquatable<SockaddrIn6>
	{
		public UnixAddressFamily sin6_family
		{
			get
			{
				return base.sa_family;
			}
			set
			{
				base.sa_family = value;
			}
		}

		public SockaddrIn6()
			: base(SockaddrType.SockaddrIn6, UnixAddressFamily.AF_INET6)
		{
		}

		public override string ToString()
		{
			return string.Format("{{sin6_family={0}, sin6_port=htons({1}), sin6_flowinfo={2}, sin6_addr={3}, sin6_scope_id={4}}}", new object[]
			{
				base.sa_family,
				Syscall.ntohs(this.sin6_port),
				this.sin6_flowinfo,
				this.sin6_addr,
				this.sin6_scope_id
			});
		}

		public new static SockaddrIn6 FromSockaddrStorage(SockaddrStorage storage)
		{
			SockaddrIn6 sockaddrIn = new SockaddrIn6();
			storage.CopyTo(sockaddrIn);
			return sockaddrIn;
		}

		public override int GetHashCode()
		{
			return this.sin6_family.GetHashCode() ^ this.sin6_port.GetHashCode() ^ this.sin6_flowinfo.GetHashCode() ^ this.sin6_addr.GetHashCode() ^ this.sin6_scope_id.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is SockaddrIn6 && this.Equals((SockaddrIn6)obj);
		}

		public bool Equals(SockaddrIn6 value)
		{
			return value != null && (this.sin6_family == value.sin6_family && this.sin6_port == value.sin6_port && this.sin6_flowinfo == value.sin6_flowinfo && this.sin6_addr.Equals(value.sin6_addr)) && this.sin6_scope_id == value.sin6_scope_id;
		}

		public ushort sin6_port;

		public uint sin6_flowinfo;

		public In6Addr sin6_addr;

		public uint sin6_scope_id;
	}
}
