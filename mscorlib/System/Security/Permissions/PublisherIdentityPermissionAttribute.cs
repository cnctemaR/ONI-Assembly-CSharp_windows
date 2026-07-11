using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Cryptography;

namespace System.Security.Permissions
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	[ComVisible(true)]
	[Serializable]
	public sealed class PublisherIdentityPermissionAttribute : CodeAccessSecurityAttribute
	{
		public PublisherIdentityPermissionAttribute(SecurityAction action)
			: base(action)
		{
		}

		public string CertFile
		{
			get
			{
				return this.certFile;
			}
			set
			{
				this.certFile = value;
			}
		}

		public string SignedFile
		{
			get
			{
				return this.signedFile;
			}
			set
			{
				this.signedFile = value;
			}
		}

		public string X509Certificate
		{
			get
			{
				return this.x509data;
			}
			set
			{
				this.x509data = value;
			}
		}

		public override IPermission CreatePermission()
		{
			if (base.Unrestricted)
			{
				return new PublisherIdentityPermission(PermissionState.Unrestricted);
			}
			if (this.x509data != null)
			{
				return new PublisherIdentityPermission(new X509Certificate(CryptoConvert.FromHex(this.x509data)));
			}
			if (this.certFile != null)
			{
				return new PublisherIdentityPermission(global::System.Security.Cryptography.X509Certificates.X509Certificate.CreateFromCertFile(this.certFile));
			}
			if (this.signedFile != null)
			{
				return new PublisherIdentityPermission(global::System.Security.Cryptography.X509Certificates.X509Certificate.CreateFromSignedFile(this.signedFile));
			}
			return new PublisherIdentityPermission(PermissionState.None);
		}

		private string certFile;

		private string signedFile;

		private string x509data;
	}
}
