using System;
using System.Text;

namespace System.Net.Security
{
	public readonly struct SslApplicationProtocol : IEquatable<SslApplicationProtocol>
	{
		internal SslApplicationProtocol(byte[] protocol, bool copy)
		{
			if (protocol == null)
			{
				throw new ArgumentNullException("protocol");
			}
			if (protocol.Length == 0 || protocol.Length > 255)
			{
				throw new ArgumentException("The application protocol value is invalid.", "protocol");
			}
			if (copy)
			{
				byte[] array = new byte[protocol.Length];
				Array.Copy(protocol, 0, array, 0, protocol.Length);
				this._readOnlyProtocol = new ReadOnlyMemory<byte>(array);
				return;
			}
			this._readOnlyProtocol = new ReadOnlyMemory<byte>(protocol);
		}

		public SslApplicationProtocol(byte[] protocol)
		{
			this = new SslApplicationProtocol(protocol, true);
		}

		public SslApplicationProtocol(string protocol)
		{
			this = new SslApplicationProtocol(SslApplicationProtocol.s_utf8.GetBytes(protocol), false);
		}

		public ReadOnlyMemory<byte> Protocol
		{
			get
			{
				return this._readOnlyProtocol;
			}
		}

		public bool Equals(SslApplicationProtocol other)
		{
			return this._readOnlyProtocol.Length == other._readOnlyProtocol.Length && ((this._readOnlyProtocol.IsEmpty && other._readOnlyProtocol.IsEmpty) || this._readOnlyProtocol.Span.SequenceEqual<byte>(other._readOnlyProtocol.Span));
		}

		public override bool Equals(object obj)
		{
			if (obj is SslApplicationProtocol)
			{
				SslApplicationProtocol sslApplicationProtocol = (SslApplicationProtocol)obj;
				return this.Equals(sslApplicationProtocol);
			}
			return false;
		}

		public unsafe override int GetHashCode()
		{
			if (this._readOnlyProtocol.Length == 0)
			{
				return 0;
			}
			int num = 0;
			ReadOnlySpan<byte> span = this._readOnlyProtocol.Span;
			for (int i = 0; i < this._readOnlyProtocol.Length; i++)
			{
				num = ((num << 5) + num) ^ (int)(*span[i]);
			}
			return num;
		}

		public unsafe override string ToString()
		{
			string text;
			try
			{
				if (this._readOnlyProtocol.Length == 0)
				{
					text = null;
				}
				else
				{
					text = SslApplicationProtocol.s_utf8.GetString(this._readOnlyProtocol.Span);
				}
			}
			catch
			{
				int num = this._readOnlyProtocol.Length * 5;
				char[] array = new char[num];
				int num2 = 0;
				ReadOnlySpan<byte> span = this._readOnlyProtocol.Span;
				for (int i = 0; i < num; i += 5)
				{
					byte b = *span[num2++];
					array[i] = '0';
					array[i + 1] = 'x';
					int num3;
					array[i + 2] = SslApplicationProtocol.GetHexValue(Math.DivRem((int)b, 16, out num3));
					array[i + 3] = SslApplicationProtocol.GetHexValue(num3);
					array[i + 4] = ' ';
				}
				text = new string(array, 0, num - 1);
			}
			return text;
		}

		private static char GetHexValue(int i)
		{
			if (i < 10)
			{
				return (char)(i + 48);
			}
			return (char)(i - 10 + 97);
		}

		public static bool operator ==(SslApplicationProtocol left, SslApplicationProtocol right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(SslApplicationProtocol left, SslApplicationProtocol right)
		{
			return !(left == right);
		}

		private readonly ReadOnlyMemory<byte> _readOnlyProtocol;

		private static readonly Encoding s_utf8 = Encoding.GetEncoding(Encoding.UTF8.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);

		public static readonly SslApplicationProtocol Http2 = new SslApplicationProtocol(new byte[] { 104, 50 }, false);

		public static readonly SslApplicationProtocol Http11 = new SslApplicationProtocol(new byte[] { 104, 116, 116, 112, 47, 49, 46, 49 }, false);
	}
}
