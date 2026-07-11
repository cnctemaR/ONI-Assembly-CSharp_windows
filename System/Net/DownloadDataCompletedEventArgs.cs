using System;
using System.ComponentModel;
using Unity;

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

		internal DownloadDataCompletedEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private byte[] m_Result;
	}
}
