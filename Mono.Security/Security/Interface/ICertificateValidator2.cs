using System;
using System.Security.Cryptography.X509Certificates;

namespace Mono.Security.Interface
{
	internal interface ICertificateValidator2 : ICertificateValidator
	{
		ValidationResult ValidateCertificate(string targetHost, bool serverMode, X509Certificate leaf, X509Chain chain);
	}
}
