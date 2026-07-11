using System;
using System.ComponentModel;
using System.IO;

namespace System.Net
{
	public class OpenReadCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal OpenReadCompletedEventArgs(Stream result, Exception exception, bool cancelled, object userToken)
			: base(exception, cancelled, userToken)
		{
			this.m_Result = result;
		}

		public Stream Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return this.m_Result;
			}
		}

		private Stream m_Result;
	}
}
