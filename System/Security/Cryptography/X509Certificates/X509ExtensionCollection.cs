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

		internal X509ExtensionCollection(X509Certificate cert)
		{
			this._list = new ArrayList(cert.Extensions.Count);
			if (cert.Extensions.Count == 0)
			{
				return;
			}
			object[] array = new object[2];
			foreach (object obj in cert.Extensions)
			{
				X509Extension x509Extension = (X509Extension)obj;
				bool critical = x509Extension.Critical;
				string oid = x509Extension.Oid;
				byte[] array2 = null;
				ASN1 value = x509Extension.Value;
				if (value.Tag == 4 && value.Count > 0)
				{
					array2 = value[0].GetBytes();
				}
				array[0] = new AsnEncodedData(oid, array2);
				array[1] = critical;
				X509Extension x509Extension2 = (X509Extension)CryptoConfig.CreateFromName(oid, array);
				if (x509Extension2 == null)
				{
					x509Extension2 = new X509Extension(oid, array2, critical);
				}
				this._list.Add(x509Extension2);
			}
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

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new X509ExtensionEnumerator(this._list);
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

		public X509Extension this[int index]
		{
			get
			{
				if (index < 0)
				{
					throw new InvalidOperationException("index");
				}
				return (X509Extension)this._list[index];
			}
		}

		public X509Extension this[string oid]
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
					X509Extension x509Extension = (X509Extension)obj;
					if (x509Extension.Oid.Value.Equals(oid))
					{
						return x509Extension;
					}
				}
				return null;
			}
		}

		public int Add(X509Extension extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			return this._list.Add(extension);
		}

		public void CopyTo(X509Extension[] array, int index)
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

		private ArrayList _list;
	}
}
