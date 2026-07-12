using System;
using System.Text;

namespace System.Net.Sockets
{
	public sealed class UnixDomainSocketEndPoint : EndPoint
	{
		private SocketAddress CreateSocketAddressForSerialize()
		{
			return new SocketAddress(AddressFamily.Unix, UnixDomainSocketEndPoint.s_nativeAddressSize);
		}

		public UnixDomainSocketEndPoint(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			bool flag = UnixDomainSocketEndPoint.IsAbstract(path);
			int num = UnixDomainSocketEndPoint.s_pathEncoding.GetByteCount(path);
			if (!flag)
			{
				num++;
			}
			if (path.Length == 0 || num > UnixDomainSocketEndPoint.s_nativePathLength)
			{
				throw new ArgumentOutOfRangeException("path", path, global::SR.Format("The path '{0}' is of an invalid length for use with domain sockets on this platform.  The length must be between 1 and {1} characters, inclusive.", path, UnixDomainSocketEndPoint.s_nativePathLength));
			}
			this._path = path;
			this._encodedPath = new byte[num];
			UnixDomainSocketEndPoint.s_pathEncoding.GetBytes(path, 0, path.Length, this._encodedPath, 0);
			if (!UnixDomainSocketEndPoint.s_udsSupported.Value)
			{
				throw new PlatformNotSupportedException();
			}
		}

		internal UnixDomainSocketEndPoint(SocketAddress socketAddress)
		{
			if (socketAddress == null)
			{
				throw new ArgumentNullException("socketAddress");
			}
			if (socketAddress.Family != AddressFamily.Unix || socketAddress.Size > UnixDomainSocketEndPoint.s_nativeAddressSize)
			{
				throw new ArgumentOutOfRangeException("socketAddress");
			}
			if (socketAddress.Size > UnixDomainSocketEndPoint.s_nativePathOffset)
			{
				this._encodedPath = new byte[socketAddress.Size - UnixDomainSocketEndPoint.s_nativePathOffset];
				for (int i = 0; i < this._encodedPath.Length; i++)
				{
					this._encodedPath[i] = socketAddress[UnixDomainSocketEndPoint.s_nativePathOffset + i];
				}
				int num = this._encodedPath.Length;
				if (!UnixDomainSocketEndPoint.IsAbstract(this._encodedPath))
				{
					while (this._encodedPath[num - 1] == 0)
					{
						num--;
					}
				}
				this._path = UnixDomainSocketEndPoint.s_pathEncoding.GetString(this._encodedPath, 0, num);
				return;
			}
			this._encodedPath = Array.Empty<byte>();
			this._path = string.Empty;
		}

		public override SocketAddress Serialize()
		{
			SocketAddress socketAddress = this.CreateSocketAddressForSerialize();
			for (int i = 0; i < this._encodedPath.Length; i++)
			{
				socketAddress[UnixDomainSocketEndPoint.s_nativePathOffset + i] = this._encodedPath[i];
			}
			return socketAddress;
		}

		public override EndPoint Create(SocketAddress socketAddress)
		{
			return new UnixDomainSocketEndPoint(socketAddress);
		}

		public override AddressFamily AddressFamily
		{
			get
			{
				return AddressFamily.Unix;
			}
		}

		public override string ToString()
		{
			if (UnixDomainSocketEndPoint.IsAbstract(this._path))
			{
				return "@" + this._path.Substring(1);
			}
			return this._path;
		}

		private static bool IsAbstract(string path)
		{
			return path.Length > 0 && path[0] == '\0';
		}

		private static bool IsAbstract(byte[] encodedPath)
		{
			return encodedPath.Length != 0 && encodedPath[0] == 0;
		}

		private static readonly int s_nativePathOffset = 2;

		private static readonly int s_nativePathLength = 108;

		private static readonly int s_nativeAddressSize = UnixDomainSocketEndPoint.s_nativePathOffset + UnixDomainSocketEndPoint.s_nativePathLength;

		private const AddressFamily EndPointAddressFamily = AddressFamily.Unix;

		private static readonly Encoding s_pathEncoding = Encoding.UTF8;

		private static readonly Lazy<bool> s_udsSupported = new Lazy<bool>(delegate
		{
			bool flag;
			try
			{
				new Socket(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP).Dispose();
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		});

		private readonly string _path;

		private readonly byte[] _encodedPath;
	}
}
