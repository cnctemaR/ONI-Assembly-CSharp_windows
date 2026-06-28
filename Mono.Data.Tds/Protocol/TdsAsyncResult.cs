using System;
using System.Threading;

namespace Mono.Data.Tds.Protocol
{
	internal class TdsAsyncResult : IAsyncResult
	{
		public TdsAsyncResult(AsyncCallback userCallback, TdsAsyncState tdsState)
		{
			this._tdsState = tdsState;
			this._userCallback = userCallback;
			this._waitHandle = new ManualResetEvent(false);
		}

		public TdsAsyncResult(AsyncCallback userCallback, object state)
		{
			this._tdsState = new TdsAsyncState(state);
			this._userCallback = userCallback;
			this._waitHandle = new ManualResetEvent(false);
		}

		public object AsyncState
		{
			get
			{
				return this._tdsState.UserState;
			}
		}

		internal TdsAsyncState TdsAsyncState
		{
			get
			{
				return this._tdsState;
			}
		}

		public WaitHandle AsyncWaitHandle
		{
			get
			{
				return this._waitHandle;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this._completed;
			}
		}

		public bool IsCompletedWithException
		{
			get
			{
				return this._exception != null;
			}
		}

		public Exception Exception
		{
			get
			{
				return this._exception;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return this._completedSyncly;
			}
		}

		internal object ReturnValue
		{
			get
			{
				return this._retValue;
			}
			set
			{
				this._retValue = value;
			}
		}

		internal void MarkComplete()
		{
			this._completed = true;
			this._exception = null;
			((ManualResetEvent)this._waitHandle).Set();
			if (this._userCallback != null)
			{
				this._userCallback(this);
			}
		}

		internal void MarkComplete(Exception e)
		{
			this._completed = true;
			this._exception = e;
			((ManualResetEvent)this._waitHandle).Set();
			if (this._userCallback != null)
			{
				this._userCallback(this);
			}
		}

		private TdsAsyncState _tdsState;

		private WaitHandle _waitHandle;

		private bool _completed;

		private bool _completedSyncly;

		private AsyncCallback _userCallback;

		private object _retValue;

		private Exception _exception;
	}
}
