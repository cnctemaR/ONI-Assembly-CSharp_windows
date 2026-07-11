using System;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Security
{
	public delegate X509Certificate LocalCertificateSelectionCallback(object sender, string targetHost, global::System.Security.Cryptography.X509Certificates.X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers);
}
