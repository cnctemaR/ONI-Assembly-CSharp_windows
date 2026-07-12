using System;

namespace System.Security.Cryptography
{
	public sealed class PbeParameters
	{
		public PbeEncryptionAlgorithm EncryptionAlgorithm { get; }

		public HashAlgorithmName HashAlgorithm { get; }

		public int IterationCount { get; }

		public PbeParameters(PbeEncryptionAlgorithm encryptionAlgorithm, HashAlgorithmName hashAlgorithm, int iterationCount)
		{
			if (iterationCount < 1)
			{
				throw new ArgumentOutOfRangeException("iterationCount", iterationCount, "Positive number required.");
			}
			this.EncryptionAlgorithm = encryptionAlgorithm;
			this.HashAlgorithm = hashAlgorithm;
			this.IterationCount = iterationCount;
		}
	}
}
