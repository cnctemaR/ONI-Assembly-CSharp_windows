using System;
using System.Threading;

namespace System.Net
{
	internal class CallbackClosure
	{
		internal CallbackClosure(ExecutionContext context, AsyncCallback callback)
		{
			if (callback != null)
			{
				this._savedCallback = callback;
				this._savedContext = context;
			}
		}

		internal bool IsCompatible(AsyncCallback callback)
		{
			return callback != null && this._savedCallback != null && object.Equals(this._savedCallback, callback);
		}

		internal AsyncCallback AsyncCallback
		{
			get
			{
				return this._savedCallback;
			}
		}

		internal ExecutionContext Context
		{
			get
			{
				return this._savedContext;
			}
		}

		private AsyncCallback _savedCallback;

		private ExecutionContext _savedContext;
	}
}
