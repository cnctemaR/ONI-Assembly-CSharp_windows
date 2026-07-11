using System;
using System.ComponentModel;

namespace System.Net
{
	public class DownloadDataCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal DownloadDataCompletedEventArgs(byte[] result, Exception exception, bool cancelled, object userToken)
			: base(exception, cancelled, userToken)
		{
			this.m_Result = result;
		}

		public byte[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return this.m_Result;
			}
		}

		private byte[] m_Result;
	}
}
