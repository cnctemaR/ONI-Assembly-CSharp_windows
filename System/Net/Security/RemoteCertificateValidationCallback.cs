using System;
using System.Security.Cryptography.X509Certificates;

namespace System.Net.Security
{
	public delegate bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, global::System.Security.Cryptography.X509Certificates.X509Chain chain, SslPolicyErrors sslPolicyErrors);
}
