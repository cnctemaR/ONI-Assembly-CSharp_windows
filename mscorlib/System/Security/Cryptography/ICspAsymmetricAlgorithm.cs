using System;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography
{
	[ComVisible(true)]
	public interface ICspAsymmetricAlgorithm
	{
		byte[] ExportCspBlob(bool includePrivateParameters);

		void ImportCspBlob(byte[] rawData);

		CspKeyContainerInfo CspKeyContainerInfo { get; }
	}
}
