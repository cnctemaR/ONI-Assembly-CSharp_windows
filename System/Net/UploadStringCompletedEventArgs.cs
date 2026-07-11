using System;
using System.ComponentModel;

namespace System.Net
{
	public class UploadStringCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal UploadStringCompletedEventArgs(string result, Exception exception, bool cancelled, object userToken)
			: base(exception, cancelled, userToken)
		{
			this.m_Result = result;
		}

		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return this.m_Result;
			}
		}

		private string m_Result;
	}
}
