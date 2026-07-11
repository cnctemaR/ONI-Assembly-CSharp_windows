using System;
using System.Security.Permissions;

namespace System.Security.Cryptography
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public sealed class CngKeyBlobFormat : IEquatable<CngKeyBlobFormat>
	{
		public CngKeyBlobFormat(string format)
		{
			if (format == null)
			{
				throw new ArgumentNullException("format");
			}
			if (format.Length == 0)
			{
				throw new ArgumentException(global::SR.GetString("The key blob format '{0}' is invalid.", new object[] { format }), "format");
			}
			this.m_format = format;
		}

		public string Format
		{
			get
			{
				return this.m_format;
			}
		}

		public static bool operator ==(CngKeyBlobFormat left, CngKeyBlobFormat right)
		{
			if (left == null)
			{
				return right == null;
			}
			return left.Equals(right);
		}

		public static bool operator !=(CngKeyBlobFormat left, CngKeyBlobFormat right)
		{
			if (left == null)
			{
				return right != null;
			}
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as CngKeyBlobFormat);
		}

		public bool Equals(CngKeyBlobFormat other)
		{
			return other != null && this.m_format.Equals(other.Format);
		}

		public override int GetHashCode()
		{
			return this.m_format.GetHashCode();
		}

		public override string ToString()
		{
			return this.m_format;
		}

		public static CngKeyBlobFormat EccPrivateBlob
		{
			get
			{
				if (CngKeyBlobFormat.s_eccPrivate == null)
				{
					CngKeyBlobFormat.s_eccPrivate = new CngKeyBlobFormat("ECCPRIVATEBLOB");
				}
				return CngKeyBlobFormat.s_eccPrivate;
			}
		}

		public static CngKeyBlobFormat EccPublicBlob
		{
			get
			{
				if (CngKeyBlobFormat.s_eccPublic == null)
				{
					CngKeyBlobFormat.s_eccPublic = new CngKeyBlobFormat("ECCPUBLICBLOB");
				}
				return CngKeyBlobFormat.s_eccPublic;
			}
		}

		public static CngKeyBlobFormat EccFullPrivateBlob
		{
			get
			{
				if (CngKeyBlobFormat.s_eccFullPrivate == null)
				{
					CngKeyBlobFormat.s_eccFullPrivate = new CngKeyBlobFormat("ECCFULLPRIVATEBLOB");
				}
				return CngKeyBlobFormat.s_eccFullPrivate;
			}
		}

		public static CngKeyBlobFormat EccFullPublicBlob
		{
			get
			{
				if (CngKeyBlobFormat.s_eccFullPublic == null)
				{
					CngKeyBlobFormat.s_eccFullPublic = new CngKeyBlobFormat("ECCFULLPUBLICBLOB");
				}
				return CngKeyBlobFormat.s_eccFullPublic;
			}
		}

		public static CngKeyBlobFormat GenericPrivateBlob
		{
			get
			{
				if (CngKeyBlobFormat.s_genericPrivate == null)
				{
					CngKeyBlobFormat.s_genericPrivate = new CngKeyBlobFormat("PRIVATEBLOB");
				}
				return CngKeyBlobFormat.s_genericPrivate;
			}
		}

		public static CngKeyBlobFormat GenericPublicBlob
		{
			get
			{
				if (CngKeyBlobFormat.s_genericPublic == null)
				{
					CngKeyBlobFormat.s_genericPublic = new CngKeyBlobFormat("PUBLICBLOB");
				}
				return CngKeyBlobFormat.s_genericPublic;
			}
		}

		public static CngKeyBlobFormat OpaqueTransportBlob
		{
			get
			{
				if (CngKeyBlobFormat.s_opaqueTransport == null)
				{
					CngKeyBlobFormat.s_opaqueTransport = new CngKeyBlobFormat("OpaqueTransport");
				}
				return CngKeyBlobFormat.s_opaqueTransport;
			}
		}

		public static CngKeyBlobFormat Pkcs8PrivateBlob
		{
			get
			{
				if (CngKeyBlobFormat.s_pkcs8Private == null)
				{
					CngKeyBlobFormat.s_pkcs8Private = new CngKeyBlobFormat("PKCS8_PRIVATEKEY");
				}
				return CngKeyBlobFormat.s_pkcs8Private;
			}
		}

		private static volatile CngKeyBlobFormat s_eccPrivate;

		private static volatile CngKeyBlobFormat s_eccPublic;

		private static volatile CngKeyBlobFormat s_eccFullPrivate;

		private static volatile CngKeyBlobFormat s_eccFullPublic;

		private static volatile CngKeyBlobFormat s_genericPrivate;

		private static volatile CngKeyBlobFormat s_genericPublic;

		private static volatile CngKeyBlobFormat s_opaqueTransport;

		private static volatile CngKeyBlobFormat s_pkcs8Private;

		private string m_format;
	}
}
