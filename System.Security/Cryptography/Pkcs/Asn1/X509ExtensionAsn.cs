using System;
using System.Security.Cryptography.Asn1;
using System.Security.Cryptography.X509Certificates;
using Internal.Cryptography;

namespace System.Security.Cryptography.Pkcs.Asn1
{
	internal struct X509ExtensionAsn
	{
		public X509ExtensionAsn(X509Extension extension, bool copyValue = true)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			this.ExtnId = extension.Oid.Value;
			this.Critical = extension.Critical;
			this.ExtnValue = (copyValue ? extension.RawData.CloneByteArray() : extension.RawData);
		}

		[ObjectIdentifier]
		internal string ExtnId;

		[DefaultValue(new byte[] { 1, 1, 0 })]
		internal bool Critical;

		[OctetString]
		internal ReadOnlyMemory<byte> ExtnValue;
	}
}
