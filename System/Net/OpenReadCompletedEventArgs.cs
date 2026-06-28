using System;
using System.ComponentModel;
using System.IO;

namespace System.Net
{
	public class OpenReadCompletedEventArgs : global::System.ComponentModel.AsyncCompletedEventArgs
	{
		internal OpenReadCompletedEventArgs(Stream result, Exception error, bool cancelled, object userState)
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
