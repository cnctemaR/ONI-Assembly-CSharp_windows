using System;
using System.ComponentModel;
using Unity;

namespace System.Net
{
	public class UploadProgressChangedEventArgs : ProgressChangedEventArgs
	{
		internal UploadProgressChangedEventArgs(int progressPercentage, object userToken, long bytesSent, long totalBytesToSend, long bytesReceived, long totalBytesToReceive)
			: base(progressPercentage, userToken)
		{
			this.BytesReceived = bytesReceived;
			this.TotalBytesToReceive = totalBytesToReceive;
			this.BytesSent = bytesSent;
			this.TotalBytesToSend = totalBytesToSend;
		}

		public long BytesReceived { get; }

		public long TotalBytesToReceive { get; }

		public long BytesSent { get; }

		public long TotalBytesToSend { get; }

		internal UploadProgressChangedEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
