using System;
using System.ComponentModel;
using System.IO;
using Unity;

namespace System.Net
{
	public class OpenReadCompletedEventArgs : AsyncCompletedEventArgs
	{
		internal OpenReadCompletedEventArgs(Stream result, Exception exception, bool cancelled, object userToken)
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

		internal OpenReadCompletedEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		private readonly Stream _result;
	}
}
