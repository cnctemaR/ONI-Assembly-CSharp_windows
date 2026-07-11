using System;
using System.ComponentModel;

namespace System.Net
{
	public class UploadStringCompletedEventArgs : global::System.ComponentModel.AsyncCompletedEventArgs
	{
		internal UploadStringCompletedEventArgs(string result, Exception error, bool cancelled, object userState)
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
