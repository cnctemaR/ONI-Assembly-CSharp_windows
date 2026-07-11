using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Presence
{
	public sealed class PresenceModification : Handle
	{
		public PresenceModification(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result SetStatus(PresenceModificationSetStatusOptions options)
		{
			PresenceModificationSetStatusOptionsInternal presenceModificationSetStatusOptionsInternal = Helper.CopyProperties<PresenceModificationSetStatusOptionsInternal>(options);
			Result result = PresenceModification.EOS_PresenceModification_SetStatus(base.InnerHandle, ref presenceModificationSetStatusOptionsInternal);
			Helper.TryMarshalDispose<PresenceModificationSetStatusOptionsInternal>(ref presenceModificationSetStatusOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetRawRichText(PresenceModificationSetRawRichTextOptions options)
		{
			PresenceModificationSetRawRichTextOptionsInternal presenceModificationSetRawRichTextOptionsInternal = Helper.CopyProperties<PresenceModificationSetRawRichTextOptionsInternal>(options);
			Result result = PresenceModification.EOS_PresenceModification_SetRawRichText(base.InnerHandle, ref presenceModificationSetRawRichTextOptionsInternal);
			Helper.TryMarshalDispose<PresenceModificationSetRawRichTextOptionsInternal>(ref presenceModificationSetRawRichTextOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetData(PresenceModificationSetDataOptions options)
		{
			PresenceModificationSetDataOptionsInternal presenceModificationSetDataOptionsInternal = Helper.CopyProperties<PresenceModificationSetDataOptionsInternal>(options);
			Result result = PresenceModification.EOS_PresenceModification_SetData(base.InnerHandle, ref presenceModificationSetDataOptionsInternal);
			Helper.TryMarshalDispose<PresenceModificationSetDataOptionsInternal>(ref presenceModificationSetDataOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result DeleteData(PresenceModificationDeleteDataOptions options)
		{
			PresenceModificationDeleteDataOptionsInternal presenceModificationDeleteDataOptionsInternal = Helper.CopyProperties<PresenceModificationDeleteDataOptionsInternal>(options);
			Result result = PresenceModification.EOS_PresenceModification_DeleteData(base.InnerHandle, ref presenceModificationDeleteDataOptionsInternal);
			Helper.TryMarshalDispose<PresenceModificationDeleteDataOptionsInternal>(ref presenceModificationDeleteDataOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result SetJoinInfo(PresenceModificationSetJoinInfoOptions options)
		{
			PresenceModificationSetJoinInfoOptionsInternal presenceModificationSetJoinInfoOptionsInternal = Helper.CopyProperties<PresenceModificationSetJoinInfoOptionsInternal>(options);
			Result result = PresenceModification.EOS_PresenceModification_SetJoinInfo(base.InnerHandle, ref presenceModificationSetJoinInfoOptionsInternal);
			Helper.TryMarshalDispose<PresenceModificationSetJoinInfoOptionsInternal>(ref presenceModificationSetJoinInfoOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void Release()
		{
			PresenceModification.EOS_PresenceModification_Release(base.InnerHandle);
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_PresenceModification_Release(IntPtr presenceModificationHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_PresenceModification_SetJoinInfo(IntPtr handle, ref PresenceModificationSetJoinInfoOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_PresenceModification_DeleteData(IntPtr handle, ref PresenceModificationDeleteDataOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_PresenceModification_SetData(IntPtr handle, ref PresenceModificationSetDataOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_PresenceModification_SetRawRichText(IntPtr handle, ref PresenceModificationSetRawRichTextOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_PresenceModification_SetStatus(IntPtr handle, ref PresenceModificationSetStatusOptionsInternal options);
	}
}
