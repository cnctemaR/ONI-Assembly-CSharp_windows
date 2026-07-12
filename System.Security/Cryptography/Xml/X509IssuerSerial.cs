using System;

namespace System.Security.Cryptography.Xml
{
	public struct X509IssuerSerial
	{
		internal X509IssuerSerial(string issuerName, string serialNumber)
		{
			this = default(X509IssuerSerial);
			this.IssuerName = issuerName;
			this.SerialNumber = serialNumber;
		}

		public string IssuerName { readonly get; set; }

		public string SerialNumber { readonly get; set; }
	}
}
