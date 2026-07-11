using System;

namespace System.Security.Cryptography
{
	internal enum AsymmetricPaddingMode
	{
		None = 1,
		Pkcs1,
		Oaep = 4,
		Pss = 8
	}
}
