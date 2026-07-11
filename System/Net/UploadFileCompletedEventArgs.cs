using System;
using System.ComponentModel;

namespace System.Net
{
	public class UploadFileCompletedEventArgs : global::System.ComponentModel.AsyncCompletedEventArgs
	{
		internal UploadFileCompletedEventArgs(byte[] result, Exception error, bool cancelled, object userState)
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
