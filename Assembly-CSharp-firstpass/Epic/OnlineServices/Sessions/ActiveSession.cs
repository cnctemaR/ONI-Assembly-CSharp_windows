using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	public sealed class ActiveSession : Handle
	{
		public ActiveSession(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result CopyInfo(ActiveSessionCopyInfoOptions options, out ActiveSessionInfo outActiveSessionInfo)
		{
			ActiveSessionCopyInfoOptionsInternal activeSessionCopyInfoOptionsInternal = Helper.CopyProperties<ActiveSessionCopyInfoOptionsInternal>(options);
			outActiveSessionInfo = Helper.GetDefault<ActiveSessionInfo>();
			IntPtr zero = IntPtr.Zero;
			Result result = ActiveSession.EOS_ActiveSession_CopyInfo(base.InnerHandle, ref activeSessionCopyInfoOptionsInternal, ref zero);
			Helper.TryMarshalDispose<ActiveSessionCopyInfoOptionsInternal>(ref activeSessionCopyInfoOptionsInternal);
			if (Helper.TryMarshalGet<ActiveSessionInfoInternal, ActiveSessionInfo>(zero, out outActiveSessionInfo))
			{
				ActiveSession.EOS_ActiveSession_Info_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetRegisteredPlayerCount(ActiveSessionGetRegisteredPlayerCountOptions options)
		{
			ActiveSessionGetRegisteredPlayerCountOptionsInternal activeSessionGetRegisteredPlayerCountOptionsInternal = Helper.CopyProperties<ActiveSessionGetRegisteredPlayerCountOptionsInternal>(options);
			uint num = ActiveSession.EOS_ActiveSession_GetRegisteredPlayerCount(base.InnerHandle, ref activeSessionGetRegisteredPlayerCountOptionsInternal);
			Helper.TryMarshalDispose<ActiveSessionGetRegisteredPlayerCountOptionsInternal>(ref activeSessionGetRegisteredPlayerCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public ProductUserId GetRegisteredPlayerByIndex(ActiveSessionGetRegisteredPlayerByIndexOptions options)
		{
			ActiveSessionGetRegisteredPlayerByIndexOptionsInternal activeSessionGetRegisteredPlayerByIndexOptionsInternal = Helper.CopyProperties<ActiveSessionGetRegisteredPlayerByIndexOptionsInternal>(options);
			IntPtr intPtr = ActiveSession.EOS_ActiveSession_GetRegisteredPlayerByIndex(base.InnerHandle, ref activeSessionGetRegisteredPlayerByIndexOptionsInternal);
			Helper.TryMarshalDispose<ActiveSessionGetRegisteredPlayerByIndexOptionsInternal>(ref activeSessionGetRegisteredPlayerByIndexOptionsInternal);
			ProductUserId @default = Helper.GetDefault<ProductUserId>();
			Helper.TryMarshalGet<ProductUserId>(intPtr, out @default);
			return @default;
		}

		public void Release()
		{
			ActiveSession.EOS_ActiveSession_Release(base.InnerHandle);
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_ActiveSession_Info_Release(IntPtr activeSessionInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_ActiveSession_Release(IntPtr activeSessionHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern IntPtr EOS_ActiveSession_GetRegisteredPlayerByIndex(IntPtr handle, ref ActiveSessionGetRegisteredPlayerByIndexOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_ActiveSession_GetRegisteredPlayerCount(IntPtr handle, ref ActiveSessionGetRegisteredPlayerCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_ActiveSession_CopyInfo(IntPtr handle, ref ActiveSessionCopyInfoOptionsInternal options, ref IntPtr outActiveSessionInfo);
	}
}
