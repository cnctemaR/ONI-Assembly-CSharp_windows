using System;
using System.ComponentModel;
using Unity;

namespace System.Net
{
	public class UploadStringCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal UploadStringCompletedEventArgs(string result, Exception exception, bool cancelled, object userToken)
			: base(exception, cancelled, userToken)
		{
			this._result = result;
		}

		public string Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return this._result;
			}
		}

		internal UploadStringCompletedEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly string _result;
	}
}
