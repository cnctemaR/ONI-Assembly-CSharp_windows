using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class Publisher : EvidenceBase, IIdentityPermissionFactory, IBuiltInEvidence
	{
		public Publisher(X509Certificate cert)
		{
			if (cert == null)
			{
				throw new ArgumentNullException("cert");
			}
			if (cert.GetHashCode() == 0)
			{
				throw new ArgumentException("cert");
			}
			this.m_cert = cert;
		}

		public X509Certificate Certificate
		{
			get
			{
				if (this.m_cert.GetHashCode() == 0)
				{
					throw new ArgumentException("m_cert");
				}
				return this.m_cert;
			}
		}

		public object Copy()
		{
			return new Publisher(this.m_cert);
		}

		public IPermission CreateIdentityPermission(Evidence evidence)
		{
			return new PublisherIdentityPermission(this.m_cert);
		}

		public override bool Equals(object o)
		{
			Publisher publisher = o as Publisher;
			if (publisher == null)
			{
				throw new ArgumentException("o", Locale.GetText("not a Publisher instance."));
			}
			return this.m_cert.Equals(publisher.Certificate);
		}

		public override int GetHashCode()
		{
			return this.m_cert.GetHashCode();
		}

		public override string ToString()
		{
			SecurityElement securityElement = new SecurityElement("System.Security.Policy.Publisher");
			securityElement.AddAttribute("version", "1");
			SecurityElement securityElement2 = new SecurityElement("X509v3Certificate");
			string rawCertDataString = this.m_cert.GetRawCertDataString();
			if (rawCertDataString != null)
			{
				securityElement2.Text = rawCertDataString;
			}
			securityElement.AddChild(securityElement2);
			return securityElement.ToString();
		}

		int IBuiltInEvidence.GetRequiredSize(bool verbose)
		{
			return (verbose ? 3 : 1) + this.m_cert.GetRawCertData().Length;
		}

		[MonoTODO("IBuiltInEvidence")]
		int IBuiltInEvidence.InitFromBuffer(char[] buffer, int position)
		{
			return 0;
		}

		[MonoTODO("IBuiltInEvidence")]
		int IBuiltInEvidence.OutputToBuffer(char[] buffer, int position, bool verbose)
		{
			return 0;
		}

		private X509Certificate m_cert;
	}
}
