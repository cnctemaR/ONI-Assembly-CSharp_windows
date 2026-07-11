using System;
using System.ComponentModel;
using System.IO;

namespace System.Net
{
	public class OpenWriteCompletedEventArgs : global::System.ComponentModel.AsyncCompletedEventArgs
	{
		internal OpenWriteCompletedEventArgs(Stream result, Exception error, bool cancelled, object userState)
			: base(error, cancelled, userState)
		{
			this.result = result;
		}

		public Stream Result
		{
			get
			{
				return this.result;
			}
		}

		private Stream result;
	}
}
