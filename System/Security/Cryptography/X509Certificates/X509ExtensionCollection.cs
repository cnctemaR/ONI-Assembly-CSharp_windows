using System;
using System.Collections;
using Mono.Security;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	public sealed class X509ExtensionCollection : ICollection, IEnumerable
	{
		public X509ExtensionCollection()
		{
			this._list = new ArrayList();
		}

		internal X509ExtensionCollection(Mono.Security.X509.X509Certificate cert)
		{
			this._list = new ArrayList(cert.Extensions.Count);
			if (cert.Extensions.Count == 0)
			{
				return;
			}
			foreach (object obj in cert.Extensions)
			{
				Mono.Security.X509.X509Extension x509Extension = (Mono.Security.X509.X509Extension)obj;
				bool critical = x509Extension.Critical;
				string oid = x509Extension.Oid;
				byte[] array = null;
				Mono.Security.ASN1 value = x509Extension.Value;
				if (value.Tag == 4 && value.Count > 0)
				{
					array = value[0].GetBytes();
				}
				global::System.Security.Cryptography.X509Certificates.X509Extension x509Extension2 = (global::System.Security.Cryptography.X509Certificates.X509Extension)CryptoConfig.CreateFromName(oid, new object[]
				{
					new AsnEncodedData(oid, array ?? global::System.Security.Cryptography.X509Certificates.X509ExtensionCollection.Empty),
					critical
				});
				if (x509Extension2 == null)
				{
					x509Extension2 = new global::System.Security.Cryptography.X509Certificates.X509Extension(oid, array ?? global::System.Security.Cryptography.X509Certificates.X509ExtensionCollection.Empty, critical);
				}
				this._list.Add(x509Extension2);
			}
		}

		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return this._list.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public global::System.Security.Cryptography.X509Certificates.X509Extension this[int index]
		{
			get
			{
				if (index < 0)
				{
					throw new InvalidOperationException("index");
				}
				return (global::System.Security.Cryptography.X509Certificates.X509Extension)this._list[index];
			}
		}

		public global::System.Security.Cryptography.X509Certificates.X509Extension this[string oid]
		{
			get
			{
				if (oid == null)
				{
					throw new ArgumentNullException("oid");
				}
				if (this._list.Count == 0 || oid.Length == 0)
				{
					return null;
				}
				foreach (object obj in this._list)
				{
					global::System.Security.Cryptography.X509Certificates.X509Extension x509Extension = (global::System.Security.Cryptography.X509Certificates.X509Extension)obj;
					if (x509Extension.Oid.Value.Equals(oid))
					{
						return x509Extension;
					}
				}
				return null;
			}
		}

		public int Add(global::System.Security.Cryptography.X509Certificates.X509Extension extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			return this._list.Add(extension);
		}

		public void CopyTo(global::System.Security.Cryptography.X509Certificates.X509Extension[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("negative index");
			}
			if (index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index >= array.Length");
			}
			this._list.CopyTo(array, index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("negative index");
			}
			if (index >= array.Length)
			{
				throw new ArgumentOutOfRangeException("index >= array.Length");
			}
			this._list.CopyTo(array, index);
		}

		public X509ExtensionEnumerator GetEnumerator()
		{
			return new X509ExtensionEnumerator(this._list);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new X509ExtensionEnumerator(this._list);
		}

		private static byte[] Empty = new byte[0];

		private ArrayList _list;
	}
}
