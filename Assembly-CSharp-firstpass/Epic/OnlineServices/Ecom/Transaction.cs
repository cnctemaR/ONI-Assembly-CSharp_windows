using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices.Ecom
{
	public sealed class Transaction : Handle
	{
		public Transaction(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result GetTransactionId(StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result result = Transaction.EOS_Ecom_Transaction_GetTransactionId(base.InnerHandle, outBuffer, ref inOutBufferLength);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetEntitlementsCount(TransactionGetEntitlementsCountOptions options)
		{
			TransactionGetEntitlementsCountOptionsInternal transactionGetEntitlementsCountOptionsInternal = Helper.CopyProperties<TransactionGetEntitlementsCountOptionsInternal>(options);
			uint num = Transaction.EOS_Ecom_Transaction_GetEntitlementsCount(base.InnerHandle, ref transactionGetEntitlementsCountOptionsInternal);
			Helper.TryMarshalDispose<TransactionGetEntitlementsCountOptionsInternal>(ref transactionGetEntitlementsCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopyEntitlementByIndex(TransactionCopyEntitlementByIndexOptions options, out Entitlement outEntitlement)
		{
			TransactionCopyEntitlementByIndexOptionsInternal transactionCopyEntitlementByIndexOptionsInternal = Helper.CopyProperties<TransactionCopyEntitlementByIndexOptionsInternal>(options);
			outEntitlement = Helper.GetDefault<Entitlement>();
			IntPtr zero = IntPtr.Zero;
			Result result = Transaction.EOS_Ecom_Transaction_CopyEntitlementByIndex(base.InnerHandle, ref transactionCopyEntitlementByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<TransactionCopyEntitlementByIndexOptionsInternal>(ref transactionCopyEntitlementByIndexOptionsInternal);
			if (Helper.TryMarshalGet<EntitlementInternal, Entitlement>(zero, out outEntitlement))
			{
				Transaction.EOS_Ecom_Entitlement_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void Release()
		{
			Transaction.EOS_Ecom_Transaction_Release(base.InnerHandle);
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Ecom_Entitlement_Release(IntPtr entitlement);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_Ecom_Transaction_Release(IntPtr transaction);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Ecom_Transaction_CopyEntitlementByIndex(IntPtr handle, ref TransactionCopyEntitlementByIndexOptionsInternal options, ref IntPtr outEntitlement);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_Ecom_Transaction_GetEntitlementsCount(IntPtr handle, ref TransactionGetEntitlementsCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Ecom_Transaction_GetTransactionId(IntPtr handle, StringBuilder outBuffer, ref int inOutBufferLength);
	}
}
