using System;
using System.ComponentModel;
using Unity;

namespace System.Net
{
	public class DownloadProgressChangedEventArgs : ProgressChangedEventArgs
	{
		internal DownloadProgressChangedEventArgs(int progressPercentage, object userToken, long bytesReceived, long totalBytesToReceive)
			: base(progressPercentage, userToken)
		{
			this.BytesReceived = bytesReceived;
			this.TotalBytesToReceive = totalBytesToReceive;
		}

		public long BytesReceived { get; }

		public long TotalBytesToReceive { get; }

		internal DownloadProgressChangedEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
