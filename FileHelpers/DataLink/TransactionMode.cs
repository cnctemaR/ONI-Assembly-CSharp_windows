using System;

namespace FileHelpers.DataLink
{
	public enum TransactionMode
	{
		NoTransaction,
		UseDefault,
		UseChaosLevel,
		UseReadCommitted,
		UseReadUnCommitted,
		UseRepeatableRead,
		UseSerializable
	}
}
