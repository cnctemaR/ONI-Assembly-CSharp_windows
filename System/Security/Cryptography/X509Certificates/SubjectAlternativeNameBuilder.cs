using System;
using System.Collections.Generic;
using System.Net;

namespace System.Security.Cryptography.X509Certificates
{
	public sealed class SubjectAlternativeNameBuilder
	{
		public void AddEmailAddress(string emailAddress)
		{
			if (string.IsNullOrEmpty(emailAddress))
			{
				throw new ArgumentOutOfRangeException("emailAddress", "String cannot be empty or null.");
			}
			this._encodedTlvs.Add(this._generalNameEncoder.EncodeEmailAddress(emailAddress));
		}

		public void AddDnsName(string dnsName)
		{
			if (string.IsNullOrEmpty(dnsName))
			{
				throw new ArgumentOutOfRangeException("dnsName", "String cannot be empty or null.");
			}
			this._encodedTlvs.Add(this._generalNameEncoder.EncodeDnsName(dnsName));
		}

		public void AddUri(Uri uri)
		{
			if (uri == null)
			{
				throw new ArgumentNullException("uri");
			}
			this._encodedTlvs.Add(this._generalNameEncoder.EncodeUri(uri));
		}

		public void AddIpAddress(IPAddress ipAddress)
		{
			if (ipAddress == null)
			{
				throw new ArgumentNullException("ipAddress");
			}
			this._encodedTlvs.Add(this._generalNameEncoder.EncodeIpAddress(ipAddress));
		}

		public void AddUserPrincipalName(string upn)
		{
			if (string.IsNullOrEmpty(upn))
			{
				throw new ArgumentOutOfRangeException("upn", "String cannot be empty or null.");
			}
			this._encodedTlvs.Add(this._generalNameEncoder.EncodeUserPrincipalName(upn));
		}

		public X509Extension Build(bool critical = false)
		{
			return new X509Extension("2.5.29.17", DerEncoder.ConstructSequence(this._encodedTlvs), critical);
		}

		private readonly List<byte[][]> _encodedTlvs = new List<byte[][]>();

		private readonly GeneralNameEncoder _generalNameEncoder = new GeneralNameEncoder();
	}
}
