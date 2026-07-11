using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Sessions
{
	public sealed class SessionDetails : Handle
	{
		public SessionDetails(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result CopyInfo(SessionDetailsCopyInfoOptions options, out SessionDetailsInfo outSessionInfo)
		{
			SessionDetailsCopyInfoOptionsInternal sessionDetailsCopyInfoOptionsInternal = Helper.CopyProperties<SessionDetailsCopyInfoOptionsInternal>(options);
			outSessionInfo = Helper.GetDefault<SessionDetailsInfo>();
			IntPtr zero = IntPtr.Zero;
			Result result = SessionDetails.EOS_SessionDetails_CopyInfo(base.InnerHandle, ref sessionDetailsCopyInfoOptionsInternal, ref zero);
			Helper.TryMarshalDispose<SessionDetailsCopyInfoOptionsInternal>(ref sessionDetailsCopyInfoOptionsInternal);
			if (Helper.TryMarshalGet<SessionDetailsInfoInternal, SessionDetailsInfo>(zero, out outSessionInfo))
			{
				SessionDetails.EOS_SessionDetails_Info_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public uint GetSessionAttributeCount(SessionDetailsGetSessionAttributeCountOptions options)
		{
			SessionDetailsGetSessionAttributeCountOptionsInternal sessionDetailsGetSessionAttributeCountOptionsInternal = Helper.CopyProperties<SessionDetailsGetSessionAttributeCountOptionsInternal>(options);
			uint num = SessionDetails.EOS_SessionDetails_GetSessionAttributeCount(base.InnerHandle, ref sessionDetailsGetSessionAttributeCountOptionsInternal);
			Helper.TryMarshalDispose<SessionDetailsGetSessionAttributeCountOptionsInternal>(ref sessionDetailsGetSessionAttributeCountOptionsInternal);
			uint @default = Helper.GetDefault<uint>();
			Helper.TryMarshalGet<uint>(num, out @default);
			return @default;
		}

		public Result CopySessionAttributeByIndex(SessionDetailsCopySessionAttributeByIndexOptions options, out SessionDetailsAttribute outSessionAttribute)
		{
			SessionDetailsCopySessionAttributeByIndexOptionsInternal sessionDetailsCopySessionAttributeByIndexOptionsInternal = Helper.CopyProperties<SessionDetailsCopySessionAttributeByIndexOptionsInternal>(options);
			outSessionAttribute = Helper.GetDefault<SessionDetailsAttribute>();
			IntPtr zero = IntPtr.Zero;
			Result result = SessionDetails.EOS_SessionDetails_CopySessionAttributeByIndex(base.InnerHandle, ref sessionDetailsCopySessionAttributeByIndexOptionsInternal, ref zero);
			Helper.TryMarshalDispose<SessionDetailsCopySessionAttributeByIndexOptionsInternal>(ref sessionDetailsCopySessionAttributeByIndexOptionsInternal);
			if (Helper.TryMarshalGet<SessionDetailsAttributeInternal, SessionDetailsAttribute>(zero, out outSessionAttribute))
			{
				SessionDetails.EOS_SessionDetails_Attribute_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result CopySessionAttributeByKey(SessionDetailsCopySessionAttributeByKeyOptions options, out SessionDetailsAttribute outSessionAttribute)
		{
			SessionDetailsCopySessionAttributeByKeyOptionsInternal sessionDetailsCopySessionAttributeByKeyOptionsInternal = Helper.CopyProperties<SessionDetailsCopySessionAttributeByKeyOptionsInternal>(options);
			outSessionAttribute = Helper.GetDefault<SessionDetailsAttribute>();
			IntPtr zero = IntPtr.Zero;
			Result result = SessionDetails.EOS_SessionDetails_CopySessionAttributeByKey(base.InnerHandle, ref sessionDetailsCopySessionAttributeByKeyOptionsInternal, ref zero);
			Helper.TryMarshalDispose<SessionDetailsCopySessionAttributeByKeyOptionsInternal>(ref sessionDetailsCopySessionAttributeByKeyOptionsInternal);
			if (Helper.TryMarshalGet<SessionDetailsAttributeInternal, SessionDetailsAttribute>(zero, out outSessionAttribute))
			{
				SessionDetails.EOS_SessionDetails_Attribute_Release(zero);
			}
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public void Release()
		{
			SessionDetails.EOS_SessionDetails_Release(base.InnerHandle);
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_SessionDetails_Info_Release(IntPtr sessionInfo);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_SessionDetails_Attribute_Release(IntPtr sessionAttribute);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern void EOS_SessionDetails_Release(IntPtr sessionHandle);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionDetails_CopySessionAttributeByKey(IntPtr handle, ref SessionDetailsCopySessionAttributeByKeyOptionsInternal options, ref IntPtr outSessionAttribute);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionDetails_CopySessionAttributeByIndex(IntPtr handle, ref SessionDetailsCopySessionAttributeByIndexOptionsInternal options, ref IntPtr outSessionAttribute);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern uint EOS_SessionDetails_GetSessionAttributeCount(IntPtr handle, ref SessionDetailsGetSessionAttributeCountOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_SessionDetails_CopyInfo(IntPtr handle, ref SessionDetailsCopyInfoOptionsInternal options, ref IntPtr outSessionInfo);
	}
}
