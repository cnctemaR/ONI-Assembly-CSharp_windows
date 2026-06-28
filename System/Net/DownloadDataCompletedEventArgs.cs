using System;
using System.ComponentModel;

namespace System.Net
{
	public class DownloadDataCompletedEventArgs : global::System.ComponentModel.AsyncCompletedEventArgs
	{
		internal DownloadDataCompletedEventArgs(byte[] result, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState)
		{
			this.result = result;
		}

		public byte[] Result
		{
			get
			{
				return this.result;
			}
		}

		private byte[] result;
	}
}
