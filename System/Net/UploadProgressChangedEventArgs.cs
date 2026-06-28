using System;
using System.ComponentModel;

namespace System.Net
{
	public class UploadProgressChangedEventArgs : global::System.ComponentModel.ProgressChangedEventArgs
	{
		internal UploadProgressChangedEventArgs(long bytesReceived, long totalBytesToReceive, long bytesSent, long totalBytesToSend, int progressPercentage, object userState)
			: base(progressPercentage, userState)
		{
			this.received = bytesReceived;
			this.total_recv = totalBytesToReceive;
			this.sent = bytesSent;
			this.total_send = totalBytesToSend;
		}

		public long BytesReceived
		{
			get
			{
				return this.received;
			}
		}

		public long TotalBytesToReceive
		{
			get
			{
				return this.total_recv;
			}
		}

		public long BytesSent
		{
			get
			{
				return this.sent;
			}
		}

		public long TotalBytesToSend
		{
			get
			{
				return this.total_send;
			}
		}

		private long received;

		private long sent;

		private long total_recv;

		private long total_send;
	}
}
