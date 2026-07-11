using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography.X509Certificates
{
	[ComVisible(true)]
	public enum X509ContentType
	{
		Unknown,
		Cert,
		SerializedCert,
		Pfx,
		SerializedStore,
		Pkcs7,
		Authenticode,
		Pkcs12 = 3
	}
}
