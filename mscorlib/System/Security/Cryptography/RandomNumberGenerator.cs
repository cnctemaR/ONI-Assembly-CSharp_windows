using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public abstract class RandomNumberGenerator
	{
		public static RandomNumberGenerator Create()
		{
			return RandomNumberGenerator.Create("System.Security.Cryptography.RandomNumberGenerator");
		}

		public static RandomNumberGenerator Create(string rngName)
		{
			return (RandomNumberGenerator)CryptoConfig.CreateFromName(rngName);
		}

		public abstract void GetBytes(byte[] data);

		public abstract void GetNonZeroBytes(byte[] data);
	}
}
