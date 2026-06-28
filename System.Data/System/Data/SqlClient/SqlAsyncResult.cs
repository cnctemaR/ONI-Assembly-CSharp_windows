using System;
using System.Threading;

namespace System.Data.SqlClient
{
	internal class SqlAsyncResult : IAsyncResult
	{
		public SqlAsyncResult(AsyncCallback userCallback, SqlAsyncState sqlState)
		{
			this._sqlState = sqlState;
			this._userCallback = userCallback;
			this._waitHandle = new ManualResetEvent(false);
		}

		public SqlAsyncResult(AsyncCallback userCallback, object state)
		{
			this._sqlState = new SqlAsyncState(state);
			this._userCallback = userCallback;
			this._waitHandle = new ManualResetEvent(false);
		}

		public object AsyncState
		{
			get
			{
				return this._sqlState.UserState;
			}
		}

		internal SqlAsyncState SqlAsyncState
		{
			get
			{
				return this._sqlState;
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

		public string EndMethod
		{
			get
			{
				return this._endMethod;
			}
			set
			{
				this._endMethod = value;
			}
		}

		public bool Ended
		{
			get
			{
				return this._ended;
			}
			set
			{
				this._ended = value;
			}
		}

		internal IAsyncResult InternalResult
		{
			get
			{
				return this._internal;
			}
			set
			{
				this._internal = value;
			}
		}

		public AsyncCallback BubbleCallback
		{
			get
			{
				return new AsyncCallback(this.Bubbleback);
			}
		}

		internal void MarkComplete()
		{
			this._completed = true;
			((ManualResetEvent)this._waitHandle).Set();
			if (this._userCallback != null)
			{
				this._userCallback(this);
			}
		}

		public void Bubbleback(IAsyncResult ar)
		{
			this.MarkComplete();
		}

		private SqlAsyncState _sqlState;

		private WaitHandle _waitHandle;

		private bool _completed;

		private bool _completedSyncly;

		private bool _ended;

		private AsyncCallback _userCallback;

		private object _retValue;

		private string _endMethod;

		private IAsyncResult _internal;
	}
}
