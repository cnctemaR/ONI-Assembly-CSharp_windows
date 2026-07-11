using System;
using System.Runtime.InteropServices;

namespace Epic.OnlineServices.Metrics
{
	public sealed class MetricsInterface : Handle
	{
		public MetricsInterface(IntPtr innerHandle)
			: base(innerHandle)
		{
		}

		public Result BeginPlayerSession(BeginPlayerSessionOptions options)
		{
			BeginPlayerSessionOptionsInternal beginPlayerSessionOptionsInternal = Helper.CopyProperties<BeginPlayerSessionOptionsInternal>(options);
			Result result = MetricsInterface.EOS_Metrics_BeginPlayerSession(base.InnerHandle, ref beginPlayerSessionOptionsInternal);
			Helper.TryMarshalDispose<BeginPlayerSessionOptionsInternal>(ref beginPlayerSessionOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		public Result EndPlayerSession(EndPlayerSessionOptions options)
		{
			EndPlayerSessionOptionsInternal endPlayerSessionOptionsInternal = Helper.CopyProperties<EndPlayerSessionOptionsInternal>(options);
			Result result = MetricsInterface.EOS_Metrics_EndPlayerSession(base.InnerHandle, ref endPlayerSessionOptionsInternal);
			Helper.TryMarshalDispose<EndPlayerSessionOptionsInternal>(ref endPlayerSessionOptionsInternal);
			Result @default = Helper.GetDefault<Result>();
			Helper.TryMarshalGet<Result>(result, out @default);
			return @default;
		}

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Metrics_EndPlayerSession(IntPtr handle, ref EndPlayerSessionOptionsInternal options);

		[DllImport("EOSSDK-Win64-Shipping")]
		private static extern Result EOS_Metrics_BeginPlayerSession(IntPtr handle, ref BeginPlayerSessionOptionsInternal options);

		public const int EndplayersessionApiLatest = 1;

		public const int BeginplayersessionApiLatest = 1;
	}
}
