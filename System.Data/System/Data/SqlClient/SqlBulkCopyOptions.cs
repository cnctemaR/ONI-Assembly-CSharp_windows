using System;

namespace System.Data.SqlClient
{
	[Flags]
	public enum SqlBulkCopyOptions
	{
		Default = 0,
		KeepIdentity = 1,
		CheckConstraints = 2,
		TableLock = 4,
		KeepNulls = 8,
		FireTriggers = 16,
		UseInternalTransaction = 32,
		AllowEncryptedValueModifications = 64
	}
}
