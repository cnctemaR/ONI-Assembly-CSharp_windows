using System;
using System.ComponentModel;
using Unity;

namespace System.Net
{
	public class UploadFileCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal UploadFileCompletedEventArgs(byte[] result, Exception exception, bool cancelled, object userToken)
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

		internal UploadFileCompletedEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly byte[] _result;
	}
}
