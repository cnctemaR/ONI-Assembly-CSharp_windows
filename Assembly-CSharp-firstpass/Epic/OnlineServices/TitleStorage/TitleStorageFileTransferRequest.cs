using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Epic.OnlineServices.TitleStorage
{
	public sealed class TitleStorageFileTransferRequest : Handle
	{
		public TitleStorageFileTransferRequest(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result GetFileRequestState()
		{
			Result result = TitleStorageFileTransferRequest.EOS_TitleStorageFileTransferRequest_GetFileRequestState(base.InnerHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result GetFilename(uint filenameStringBufferSizeBytes, StringBuilder outStringBuffer, out int outStringLength)
		{
			outStringLength = Helper.GetDefault<int>();
			Result result = TitleStorageFileTransferRequest.EOS_TitleStorageFileTransferRequest_GetFilename(base.InnerHandle, filenameStringBufferSizeBytes, outStringBuffer, ref outStringLength);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CancelRequest()
		{
			Result result = TitleStorageFileTransferRequest.EOS_TitleStorageFileTransferRequest_CancelRequest(base.InnerHandle);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void Release()
		{
			TitleStorageFileTransferRequest.EOS_TitleStorageFileTransferRequest_Release(base.InnerHandle);
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_TitleStorageFileTransferRequest_Release(IntPtr titleStorageFileTransferHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_TitleStorageFileTransferRequest_CancelRequest(IntPtr handle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_TitleStorageFileTransferRequest_GetFilename(IntPtr handle, uint filenameStringBufferSizeBytes, StringBuilder outStringBuffer, ref int outStringLength);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_TitleStorageFileTransferRequest_GetFileRequestState(IntPtr handle);
	}
}
