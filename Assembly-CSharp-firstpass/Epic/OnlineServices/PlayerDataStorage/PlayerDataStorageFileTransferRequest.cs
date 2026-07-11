using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices.PlayerDataStorage
{
	public sealed class PlayerDataStorageFileTransferRequest : Handle
	{
		public PlayerDataStorageFileTransferRequest(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result GetFileRequestState()
		{
			Result result = PlayerDataStorageFileTransferRequest.EOS_PlayerDataStorageFileTransferRequest_GetFileRequestState(base.InnerHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result GetFilename(uint filenameStringBufferSizeBytes, StringBuilder outStringBuffer, out int outStringLength)
		{
			outStringLength = Helper.GetDefault<int>();
			Result result = PlayerDataStorageFileTransferRequest.EOS_PlayerDataStorageFileTransferRequest_GetFilename(base.InnerHandle, filenameStringBufferSizeBytes, outStringBuffer, ref outStringLength);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CancelRequest()
		{
			Result result = PlayerDataStorageFileTransferRequest.EOS_PlayerDataStorageFileTransferRequest_CancelRequest(base.InnerHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void Release()
		{
			PlayerDataStorageFileTransferRequest.EOS_PlayerDataStorageFileTransferRequest_Release(base.InnerHandle);
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_PlayerDataStorageFileTransferRequest_Release(IntPtr playerDataStorageFileTransferHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_PlayerDataStorageFileTransferRequest_CancelRequest(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_PlayerDataStorageFileTransferRequest_GetFilename(IntPtr handle, uint filenameStringBufferSizeBytes, StringBuilder outStringBuffer, ref int outStringLength);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_PlayerDataStorageFileTransferRequest_GetFileRequestState(IntPtr handle);
	}
}
