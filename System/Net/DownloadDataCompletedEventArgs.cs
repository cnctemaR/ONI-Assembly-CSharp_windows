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
			this._result = result;
		}

		public byte[] Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return this._result;
			}
		}

		internal DownloadDataCompletedEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly byte[] _result;
	}
}
