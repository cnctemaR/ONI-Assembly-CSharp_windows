using System;
using Unity;

namespace System.Data.SqlClient
{
	public abstract class SqlColumnEncryptionKeyStoreProvider
	{
		protected SqlColumnEncryptionKeyStoreProvider()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		public abstract byte[] DecryptColumnEncryptionKey(string masterKeyPath, string encryptionAlgorithm, byte[] encryptedColumnEncryptionKey);

		public abstract byte[] EncryptColumnEncryptionKey(string masterKeyPath, string encryptionAlgorithm, byte[] columnEncryptionKey);
	}
}
