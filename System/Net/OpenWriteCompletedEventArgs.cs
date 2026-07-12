using System;
using System.ComponentModel;
using System.IO;
using Unity;

namespace System.Net
{
	public class OpenWriteCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal OpenWriteCompletedEventArgs(Stream result, Exception exception, bool cancelled, object userToken)
			: base(exception, cancelled, userToken)
		{
			this._result = result;
		}

		public Stream Result
		{
			get
			{
				base.RaiseExceptionIfNecessary();
				return this._result;
			}
		}

		internal OpenWriteCompletedEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly Stream _result;
	}
}
