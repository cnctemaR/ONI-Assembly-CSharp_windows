using System;

namespace System.Transactions
{
	[MonoTODO]
	public static class TransactionInterop
	{
		[MonoTODO]
		public static IDtcTransaction GetDtcTransaction(Transaction transaction)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static byte[] GetExportCookie(Transaction transaction, byte[] whereabouts)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static Transaction GetTransactionFromDtcTransaction(IDtcTransaction transactionNative)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static Transaction GetTransactionFromExportCookie(byte[] cookie)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static Transaction GetTransactionFromTransmitterPropagationToken(byte[] propagationToken)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static byte[] GetTransmitterPropagationToken(Transaction transaction)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public static byte[] GetWhereabouts()
		{
			throw new NotImplementedException();
		}

		public static readonly Guid PromoterTypeDtc = new Guid("14229753-FFE1-428D-82B7-DF73045CB8DA");
	}
}
