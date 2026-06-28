using System;

namespace System.Security.Cryptography.Xml
{
	public struct X509IssuerSerial
	{
		internal X509IssuerSerial(string issuer, string serial)
		{
			this._issuerName = issuer;
			this._serialNumber = serial;
		}

		public string IssuerName
		{
			get
			{
				return this._issuerName;
			}
			set
			{
				this._issuerName = value;
			}
		}

		public string SerialNumber
		{
			get
			{
				return this._serialNumber;
			}
			set
			{
				this._serialNumber = value;
			}
		}

		private string _issuerName;

		private string _serialNumber;
	}
}
