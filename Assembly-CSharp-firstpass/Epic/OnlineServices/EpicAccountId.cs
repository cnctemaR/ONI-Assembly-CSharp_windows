using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices
{
	public sealed class EpicAccountId : Handle
	{
		public EpicAccountId(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public bool IsValid()
		{
			int num = EpicAccountId.EOS_EpicAccountId_IsValid(base.InnerHandle);
			bool @default = Helper.GetDefault<bool>();
			Helper.TryMarshalGet(num, out @default);
			return @default;
		}

		public Result ToString(StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result result = EpicAccountId.EOS_EpicAccountId_ToString(base.InnerHandle, outBuffer, ref inOutBufferLength);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public static EpicAccountId FromString(string accountIdString)
		{
			IntPtr intPtr = EpicAccountId.EOS_EpicAccountId_FromString(accountIdString);
			EpicAccountId @default = Helper.GetDefault<EpicAccountId>();
			Helper.TryMarshalGet<EpicAccountId>(intPtr, out @default);
			return @default;
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_EpicAccountId_FromString([MarshalAs(UnmanagedType.LPStr)] string accountIdString);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_EpicAccountId_ToString(IntPtr accountId, StringBuilder outBuffer, ref int inOutBufferLength);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern int EOS_EpicAccountId_IsValid(IntPtr accountId);
	}
}
