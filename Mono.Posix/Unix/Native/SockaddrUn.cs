using System;
using System.Runtime.InteropServices;

namespace Mono.Unix.Native
{
	[CLSCompliant(false)]
	public sealed class SockaddrUn : Sockaddr, IEquatable<SockaddrUn>
	{
		public UnixAddressFamily sun_family
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

		public byte[] sun_path { get; set; }

		public long sun_path_len { get; set; }

		internal override byte[] DynamicData()
		{
			return this.sun_path;
		}

		internal override long GetDynamicLength()
		{
			return this.sun_path_len;
		}

		internal override void SetDynamicLength(long value)
		{
			this.sun_path_len = value;
		}

		[DllImport("MonoPosixHelper", EntryPoint = "Mono_Posix_SockaddrUn_get_sizeof_sun_path", SetLastError = true)]
		private static extern int get_sizeof_sun_path();

		public SockaddrUn()
			: base((SockaddrType)32770, UnixAddressFamily.AF_UNIX)
		{
			this.sun_path = new byte[SockaddrUn.sizeof_sun_path];
			this.sun_path_len = 0L;
		}

		public SockaddrUn(int size)
			: base((SockaddrType)32770, UnixAddressFamily.AF_UNIX)
		{
			this.sun_path = new byte[size];
			this.sun_path_len = 0L;
		}

		public SockaddrUn(string path, bool linuxAbstractNamespace = false)
			: base((SockaddrType)32770, UnixAddressFamily.AF_UNIX)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			byte[] bytes = UnixEncoding.Instance.GetBytes(path);
			if (linuxAbstractNamespace)
			{
				this.sun_path = new byte[1 + bytes.Length];
				Array.Copy(bytes, 0, this.sun_path, 1, bytes.Length);
			}
			else
			{
				this.sun_path = bytes;
			}
			this.sun_path_len = (long)this.sun_path.Length;
		}

		public bool IsLinuxAbstractNamespace
		{
			get
			{
				return this.sun_path_len > 0L && this.sun_path[0] == 0;
			}
		}

		public string Path
		{
			get
			{
				int num = (this.IsLinuxAbstractNamespace ? 1 : 0);
				int num2 = 0;
				while ((long)(num + num2) < this.sun_path_len && this.sun_path[num + num2] != 0)
				{
					num2++;
				}
				return UnixEncoding.Instance.GetString(this.sun_path, num, num2);
			}
		}

		public override string ToString()
		{
			return string.Format("{{sa_family={0}, sun_path=\"{1}{2}\"}}", base.sa_family, this.IsLinuxAbstractNamespace ? "\\0" : "", this.Path);
		}

		public new static SockaddrUn FromSockaddrStorage(SockaddrStorage storage)
		{
			SockaddrUn sockaddrUn = new SockaddrUn((int)storage.data_len);
			storage.CopyTo(sockaddrUn);
			return sockaddrUn;
		}

		public override int GetHashCode()
		{
			return this.sun_family.GetHashCode() ^ this.IsLinuxAbstractNamespace.GetHashCode() ^ this.Path.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return obj is SockaddrUn && this.Equals((SockaddrUn)obj);
		}

		public bool Equals(SockaddrUn value)
		{
			return value != null && (this.sun_family == value.sun_family && this.IsLinuxAbstractNamespace == value.IsLinuxAbstractNamespace) && this.Path == value.Path;
		}

		private static readonly int sizeof_sun_path = SockaddrUn.get_sizeof_sun_path();
	}
}
