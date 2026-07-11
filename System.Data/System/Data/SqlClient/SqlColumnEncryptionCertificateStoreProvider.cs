using System;
using Unity;

namespace System.Data.SqlClient
{
	public class SqlColumnEncryptionCertificateStoreProvider : SqlColumnEncryptionKeyStoreProvider
	{
		public SqlColumnEncryptionCertificateStoreProvider()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		public override byte[] DecryptColumnEncryptionKey(string masterKeyPath, string encryptionAlgorithm, byte[] encryptedColumnEncryptionKey)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public override byte[] EncryptColumnEncryptionKey(string masterKeyPath, string encryptionAlgorithm, byte[] columnEncryptionKey)
		{
			ThrowStub.ThrowNotSupportedException();
			return null;
		}

		public const string ProviderName = "MSSQL_CERTIFICATE_STORE";
	}
}
