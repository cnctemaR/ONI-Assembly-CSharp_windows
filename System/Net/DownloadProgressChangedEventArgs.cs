using System;
using System.ComponentModel;

namespace System.Net
{
	public class DownloadProgressChangedEventArgs : global::System.ComponentModel.ProgressChangedEventArgs
	{
		internal DownloadProgressChangedEventArgs(long bytesReceived, long totalBytesToReceive, object userState)
			: base((totalBytesToReceive == -1L) ? 0 : ((int)(bytesReceived * 100L / totalBytesToReceive)), userState)
		{
			this.received = bytesReceived;
			this.total = totalBytesToReceive;
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
				return this.total;
			}
		}

		private long received;

		private long total;
	}
}
