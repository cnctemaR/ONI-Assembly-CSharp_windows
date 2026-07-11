using System;
using System.Collections.Generic;

namespace System.Security.Cryptography.X509Certificates
{
	internal class X509CertificateImplCollection : IDisposable
	{
		public X509CertificateImplCollection()
		{
			this.list = new List<X509CertificateImpl>();
		}

		private X509CertificateImplCollection(X509CertificateImplCollection other)
		{
			this.list = new List<X509CertificateImpl>();
			foreach (X509CertificateImpl x509CertificateImpl in other.list)
			{
				this.list.Add(x509CertificateImpl.Clone());
			}
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public X509CertificateImpl this[int index]
		{
			get
			{
				return this.list[index];
			}
		}

		public void Add(X509CertificateImpl impl, bool takeOwnership)
		{
			if (!takeOwnership)
			{
				impl = impl.Clone();
			}
			this.list.Add(impl);
		}

		public X509CertificateImplCollection Clone()
		{
			return new X509CertificateImplCollection(this);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			foreach (X509CertificateImpl x509CertificateImpl in this.list)
			{
				try
				{
					x509CertificateImpl.Dispose();
				}
				catch
				{
				}
			}
			this.list.Clear();
		}

		~X509CertificateImplCollection()
		{
			this.Dispose(false);
		}

		private List<X509CertificateImpl> list;
	}
}
