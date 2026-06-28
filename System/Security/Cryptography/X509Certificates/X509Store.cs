using System;
using System.Collections.Generic;
using System.Security.Permissions;
using Mono.Security.X509;

namespace System.Security.Cryptography.X509Certificates
{
	public sealed class X509Store
	{
		public X509Store()
			: this("MY", StoreLocation.CurrentUser)
		{
		}

		public X509Store(string storeName)
			: this(storeName, StoreLocation.CurrentUser)
		{
		}

		public X509Store(StoreName storeName)
			: this(storeName, StoreLocation.CurrentUser)
		{
		}

		public X509Store(StoreLocation storeLocation)
			: this("MY", storeLocation)
		{
		}

		public X509Store(StoreName storeName, StoreLocation storeLocation)
		{
			if (storeName < StoreName.AddressBook || storeName > StoreName.TrustedPublisher)
			{
				throw new ArgumentException("storeName");
			}
			if (storeLocation < StoreLocation.CurrentUser || storeLocation > StoreLocation.LocalMachine)
			{
				throw new ArgumentException("storeLocation");
			}
			if (storeName != StoreName.CertificateAuthority)
			{
				this._name = storeName.ToString();
			}
			else
			{
				this._name = "CA";
			}
			this._location = storeLocation;
		}

		[global::System.MonoTODO("Mono's stores are fully managed. All handles are invalid.")]
		[PermissionSet(SecurityAction.LinkDemand, XML = "<PermissionSet class=\"System.Security.PermissionSet\"\nversion=\"1\">\n<IPermission class=\"System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089\"\nversion=\"1\"\nFlags=\"UnmanagedCode\"/>\n</PermissionSet>\n")]
		public X509Store(IntPtr storeHandle)
		{
			if (storeHandle == IntPtr.Zero)
			{
				throw new ArgumentNullException("storeHandle");
			}
			throw new CryptographicException("Invalid handle.");
		}

		public X509Store(string storeName, StoreLocation storeLocation)
		{
			if (storeLocation < StoreLocation.CurrentUser || storeLocation > StoreLocation.LocalMachine)
			{
				throw new ArgumentException("storeLocation");
			}
			this._name = storeName;
			this._location = storeLocation;
		}

		public X509Certificate2Collection Certificates
		{
			get
			{
				if (this.list == null)
				{
					this.list = new X509Certificate2Collection();
				}
				else if (this.store == null)
				{
					this.list.Clear();
				}
				return this.list;
			}
		}

		public StoreLocation Location
		{
			get
			{
				return this._location;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		private X509Stores Factory
		{
			get
			{
				if (this._location == StoreLocation.CurrentUser)
				{
					return X509StoreManager.CurrentUser;
				}
				return X509StoreManager.LocalMachine;
			}
		}

		private bool IsOpen
		{
			get
			{
				return this.store != null;
			}
		}

		private bool IsReadOnly
		{
			get
			{
				return Environment.UnityWebSecurityEnabled || (this._flags & OpenFlags.ReadWrite) == OpenFlags.ReadOnly;
			}
		}

		internal X509Store Store
		{
			get
			{
				return this.store;
			}
		}

		[global::System.MonoTODO("Mono's stores are fully managed. Always returns IntPtr.Zero.")]
		public IntPtr StoreHandle
		{
			get
			{
				return IntPtr.Zero;
			}
		}

		public void Add(X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (!this.IsOpen)
			{
				throw new CryptographicException(global::Locale.GetText("Store isn't opened."));
			}
			if (this.IsReadOnly)
			{
				throw new CryptographicException(global::Locale.GetText("Store is read-only."));
			}
			if (!this.Exists(certificate))
			{
				try
				{
					this.store.Import(new X509Certificate(certificate.RawData));
				}
				finally
				{
					this.Certificates.Add(certificate);
				}
			}
		}

		[global::System.MonoTODO("Method isn't transactional (like documented)")]
		public void AddRange(X509Certificate2Collection certificates)
		{
			if (certificates == null)
			{
				throw new ArgumentNullException("certificates");
			}
			if (certificates.Count == 0)
			{
				return;
			}
			if (!this.IsOpen)
			{
				throw new CryptographicException(global::Locale.GetText("Store isn't opened."));
			}
			if (this.IsReadOnly)
			{
				throw new CryptographicException(global::Locale.GetText("Store is read-only."));
			}
			foreach (X509Certificate2 x509Certificate in certificates)
			{
				if (!this.Exists(x509Certificate))
				{
					try
					{
						this.store.Import(new X509Certificate(x509Certificate.RawData));
					}
					finally
					{
						this.Certificates.Add(x509Certificate);
					}
				}
			}
		}

		public void Close()
		{
			this.store = null;
			if (this.list != null)
			{
				this.list.Clear();
			}
		}

		public void Open(OpenFlags flags)
		{
			if (string.IsNullOrEmpty(this._name))
			{
				throw new CryptographicException(global::Locale.GetText("Invalid store name (null or empty)."));
			}
			string name = this._name;
			string text;
			if (name != null)
			{
				if (X509Store.<>f__switch$map1B == null)
				{
					X509Store.<>f__switch$map1B = new Dictionary<string, int>(1) { { "Root", 0 } };
				}
				int num;
				if (X509Store.<>f__switch$map1B.TryGetValue(name, out num))
				{
					if (num == 0)
					{
						text = "Trust";
						goto IL_008B;
					}
				}
			}
			text = this._name;
			IL_008B:
			bool flag = (flags & OpenFlags.OpenExistingOnly) != OpenFlags.OpenExistingOnly;
			this.store = this.Factory.Open(text, flag);
			if (this.store == null)
			{
				throw new CryptographicException(global::Locale.GetText("Store {0} doesn't exists.", new object[] { this._name }));
			}
			this._flags = flags;
			foreach (X509Certificate x509Certificate in this.store.Certificates)
			{
				this.Certificates.Add(new X509Certificate2(x509Certificate.RawData));
			}
		}

		public void Remove(X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (!this.IsOpen)
			{
				throw new CryptographicException(global::Locale.GetText("Store isn't opened."));
			}
			if (!this.Exists(certificate))
			{
				return;
			}
			if (this.IsReadOnly)
			{
				throw new CryptographicException(global::Locale.GetText("Store is read-only."));
			}
			try
			{
				this.store.Remove(new X509Certificate(certificate.RawData));
			}
			finally
			{
				this.Certificates.Remove(certificate);
			}
		}

		[global::System.MonoTODO("Method isn't transactional (like documented)")]
		public void RemoveRange(X509Certificate2Collection certificates)
		{
			if (certificates == null)
			{
				throw new ArgumentNullException("certificates");
			}
			if (certificates.Count == 0)
			{
				return;
			}
			if (!this.IsOpen)
			{
				throw new CryptographicException(global::Locale.GetText("Store isn't opened."));
			}
			bool flag = false;
			foreach (X509Certificate2 x509Certificate in certificates)
			{
				if (this.Exists(x509Certificate))
				{
					flag = true;
				}
			}
			if (!flag)
			{
				return;
			}
			if (this.IsReadOnly)
			{
				throw new CryptographicException(global::Locale.GetText("Store is read-only."));
			}
			try
			{
				foreach (X509Certificate2 x509Certificate2 in certificates)
				{
					this.store.Remove(new X509Certificate(x509Certificate2.RawData));
				}
			}
			finally
			{
				this.Certificates.RemoveRange(certificates);
			}
		}

		private bool Exists(X509Certificate2 certificate)
		{
			if (this.store == null || this.list == null || certificate == null)
			{
				return false;
			}
			foreach (X509Certificate2 x509Certificate in this.list)
			{
				if (certificate.Equals(x509Certificate))
				{
					return true;
				}
			}
			return false;
		}

		private string _name;

		private StoreLocation _location;

		private X509Certificate2Collection list;

		private OpenFlags _flags;

		private X509Store store;
	}
}
