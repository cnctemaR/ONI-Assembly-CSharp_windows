using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices
{
	public sealed class ContinuanceToken : Handle
	{
		public ContinuanceToken(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result ToString(StringBuilder outBuffer, ref int inOutBufferLength)
		{
			Result result = ContinuanceToken.EOS_ContinuanceToken_ToString(base.InnerHandle, outBuffer, ref inOutBufferLength);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_ContinuanceToken_ToString(IntPtr continuanceToken, StringBuilder outBuffer, ref int inOutBufferLength);
	}
}
