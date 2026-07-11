using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[Map("struct sockaddr_in")]
	[CLSCompliant(false)]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class SockaddrIn : Sockaddr, IEquatable<SockaddrIn>
	{
		public UnixAddressFamily sin_family
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

		public SockaddrIn()
			: base(SockaddrType.SockaddrIn, UnixAddressFamily.AF_INET)
		{
		}

		public override string ToString()
		{
			return string.Format("{{sin_family={0}, sin_port=htons({1}), sin_addr={2}}}", base.sa_family, Syscall.ntohs(this.sin_port), this.sin_addr);
		}

		public new static SockaddrIn FromSockaddrStorage(SockaddrStorage storage)
		{
			SockaddrIn sockaddrIn = new SockaddrIn();
			storage.CopyTo(sockaddrIn);
			return sockaddrIn;
		}

		public override int GetHashCode()
		{
			return this.sin_family.GetHashCode() ^ this.sin_port.GetHashCode() ^ this.sin_addr.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is SockaddrIn && this.Equals((SockaddrIn)obj);
		}

		public bool Equals(SockaddrIn value)
		{
			return value != null && (this.sin_family == value.sin_family && this.sin_port == value.sin_port) && this.sin_addr.Equals(value.sin_addr);
		}

		public ushort sin_port;

		public InAddr sin_addr;
	}
}
