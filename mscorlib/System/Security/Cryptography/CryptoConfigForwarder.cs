using System;

namespace System.Security.Cryptography
{
	internal static class CryptoConfigForwarder
	{
		internal static object CreateFromName(string name)
		{
			return CryptoConfig.CreateFromName(name);
		}

		internal static HashAlgorithm CreateDefaultHashAlgorithm()
		{
			return (HashAlgorithm)CryptoConfigForwarder.CreateFromName("System.Security.Cryptography.HashAlgorithm");
		}
	}
}
