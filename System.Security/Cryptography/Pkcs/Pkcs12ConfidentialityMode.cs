using System;

namespace System.Security.Cryptography.Pkcs
{
	public enum Pkcs12ConfidentialityMode
	{
		None = 1,
		Password,
		PublicKey,
		Unknown = 0
	}
}
