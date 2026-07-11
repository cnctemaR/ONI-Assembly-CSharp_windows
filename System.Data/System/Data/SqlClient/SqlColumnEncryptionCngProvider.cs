using System;
using Unity;

namespace System.Data.SqlClient
{
	public class SqlColumnEncryptionCngProvider : SqlColumnEncryptionKeyStoreProvider
	{
		public SqlColumnEncryptionCngProvider()
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

		public const string ProviderName = "MSSQL_CNG_STORE";
	}
}
