using System;
using System.Security.Cryptography;
using Unity;

namespace System.Data.SqlClient
{
	public abstract class SqlColumnEncryptionEnclaveProvider
	{
		protected SqlColumnEncryptionEnclaveProvider()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public abstract void CreateEnclaveSession(byte[] enclaveAttestationInfo, ECDiffieHellmanCng clientDiffieHellmanKey, string attestationUrl, string servername, out SqlEnclaveSession sqlEnclaveSession, out long counter);

		public abstract SqlEnclaveAttestationParameters GetAttestationParameters();

		public abstract void GetEnclaveSession(string serverName, string attestationUrl, out SqlEnclaveSession sqlEnclaveSession, out long counter);

		public abstract void InvalidateEnclaveSession(string serverName, string enclaveAttestationUrl, SqlEnclaveSession enclaveSession);
	}
}
