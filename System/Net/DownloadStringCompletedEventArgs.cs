using System;
using System.ComponentModel;

namespace System.Net
{
	public class DownloadStringCompletedEventArgs : global::System.ComponentModel.AsyncCompletedEventArgs
	{
		internal DownloadStringCompletedEventArgs(string result, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState)
		{
			this.result = result;
		}

		public string Result
		{
			get
			{
				return this.result;
			}
		}

		private string result;
	}
}
