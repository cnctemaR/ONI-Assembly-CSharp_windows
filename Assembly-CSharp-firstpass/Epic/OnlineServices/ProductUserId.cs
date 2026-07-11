using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices
{
	public sealed class ProductUserId : Handle
	{
		public ProductUserId(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public bool IsValid()
		{
			int num = ProductUserId.EOS_ProductUserId_IsValid(base.InnerHandle);
			bool @default = Helper.GetDefault<bool>();
			Helper.TryMarshalGet(num, out @default);
			return @default;
		}

		public Result ToString(StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result result = ProductUserId.EOS_ProductUserId_ToString(base.InnerHandle, outBuffer, ref inOutBufferLength);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public static ProductUserId FromString(string accountIdString)
		{
			IntPtr intPtr = ProductUserId.EOS_ProductUserId_FromString(accountIdString);
			ProductUserId @default = Helper.GetDefault<ProductUserId>();
			Helper.TryMarshalGet<ProductUserId>(intPtr, out @default);
			return @default;
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_ProductUserId_FromString([MarshalAs(UnmanagedType.LPStr)] string accountIdString);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_ProductUserId_ToString(IntPtr accountId, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern int EOS_ProductUserId_IsValid(IntPtr accountId);
	}
}
