using System;

namespace System.Security.Cryptography.X509Certificates
{
	public sealed class X509ChainPolicy
	{
		public X509ChainPolicy()
		{
			this.Reset();
		}

		public OidCollection ApplicationPolicy
		{
			get
			{
				return this.apps;
			}
		}

		public OidCollection CertificatePolicy
		{
			get
			{
				return this.cert;
			}
		}

		public X509Certificate2Collection ExtraStore
		{
			get
			{
				return this.store;
			}
		}

		public X509RevocationFlag RevocationFlag
		{
			get
			{
				return this.rflag;
			}
			set
			{
				if (value < X509RevocationFlag.EndCertificateOnly || value > X509RevocationFlag.ExcludeRoot)
				{
					throw new ArgumentException("RevocationFlag");
				}
				this.rflag = value;
			}
		}

		public X509RevocationMode RevocationMode
		{
			get
			{
				return this.mode;
			}
			set
			{
				if (value < X509RevocationMode.NoCheck || value > X509RevocationMode.Offline)
				{
					throw new ArgumentException("RevocationMode");
				}
				this.mode = value;
			}
		}

		public TimeSpan UrlRetrievalTimeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				this.timeout = value;
			}
		}

		public X509VerificationFlags VerificationFlags
		{
			get
			{
				return this.vflags;
			}
			set
			{
				if ((value | X509VerificationFlags.AllFlags) != X509VerificationFlags.AllFlags)
				{
					throw new ArgumentException("VerificationFlags");
				}
				this.vflags = value;
			}
		}

		public DateTime VerificationTime
		{
			get
			{
				return this.vtime;
			}
			set
			{
				this.vtime = value;
			}
		}

		public void Reset()
		{
			this.apps = new OidCollection();
			this.cert = new OidCollection();
			this.store = new X509Certificate2Collection();
			this.rflag = X509RevocationFlag.ExcludeRoot;
			this.mode = X509RevocationMode.Online;
			this.timeout = TimeSpan.Zero;
			this.vflags = X509VerificationFlags.NoFlag;
			this.vtime = DateTime.Now;
		}

		private OidCollection apps;

		private OidCollection cert;

		private X509Certificate2Collection store;

		private X509RevocationFlag rflag;

		private X509RevocationMode mode;

		private TimeSpan timeout;

		private X509VerificationFlags vflags;

		private DateTime vtime;
	}
}
