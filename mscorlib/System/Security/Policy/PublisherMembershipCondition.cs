using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using Mono.Security.Cryptography;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public sealed class PublisherMembershipCondition : ISecurityEncodable, ISecurityPolicyEncodable, IConstantMembershipCondition, IMembershipCondition
	{
		internal PublisherMembershipCondition()
		{
		}

		public PublisherMembershipCondition(X509Certificate certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (certificate.GetHashCode() == 0)
			{
				throw new ArgumentException("certificate");
			}
			this.x509 = certificate;
		}

		public X509Certificate Certificate
		{
			get
			{
				return this.x509;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.x509 = value;
			}
		}

		public bool Check(Evidence evidence)
		{
			if (evidence == null)
			{
				return false;
			}
			IEnumerator hostEnumerator = evidence.GetHostEnumerator();
			while (hostEnumerator.MoveNext())
			{
				if (hostEnumerator.Current is Publisher && this.x509.Equals((hostEnumerator.Current as Publisher).Certificate))
				{
					return true;
				}
			}
			return false;
		}

		public IMembershipCondition Copy()
		{
			return new PublisherMembershipCondition(this.x509);
		}

		public override bool Equals(object o)
		{
			PublisherMembershipCondition publisherMembershipCondition = o as PublisherMembershipCondition;
			return publisherMembershipCondition != null && this.x509.Equals(publisherMembershipCondition.Certificate);
		}

		public void FromXml(SecurityElement e)
		{
			this.FromXml(e, null);
		}

		public void FromXml(SecurityElement e, PolicyLevel level)
		{
			MembershipConditionHelper.CheckSecurityElement(e, "e", this.version, this.version);
			string text = e.Attribute("X509Certificate");
			if (text != null)
			{
				byte[] array = CryptoConvert.FromHex(text);
				this.x509 = new X509Certificate(array);
			}
		}

		public override int GetHashCode()
		{
			return this.x509.GetHashCode();
		}

		public override string ToString()
		{
			return "Publisher - " + this.x509.GetPublicKeyString();
		}

		public SecurityElement ToXml()
		{
			return this.ToXml(null);
		}

		public SecurityElement ToXml(PolicyLevel level)
		{
			SecurityElement securityElement = MembershipConditionHelper.Element(typeof(PublisherMembershipCondition), this.version);
			securityElement.AddAttribute("X509Certificate", this.x509.GetRawCertDataString());
			return securityElement;
		}

		private readonly int version = 1;

		private X509Certificate x509;
	}
}
