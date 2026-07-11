using System;
using System.Runtime.Serialization;
using System.Threading;

namespace System.Transactions
{
	[Serializable]
	public sealed class CommittableTransaction : Transaction, ISerializable, IDisposable, IAsyncResult
	{
		public CommittableTransaction()
			: this(default(TransactionOptions))
		{
		}

		public CommittableTransaction(TimeSpan timeout)
		{
			this.options = default(TransactionOptions);
			this.options.Timeout = timeout;
		}

		public CommittableTransaction(TransactionOptions options)
		{
			this.options = options;
		}

		public IAsyncResult BeginCommit(AsyncCallback asyncCallback, object asyncState)
		{
			this.callback = asyncCallback;
			this.user_defined_state = asyncState;
			AsyncCallback asyncCallback2 = null;
			if (asyncCallback != null)
			{
				asyncCallback2 = new AsyncCallback(this.CommitCallback);
			}
			this.asyncResult = base.BeginCommitInternal(asyncCallback2);
			return this;
		}

		public void EndCommit(IAsyncResult asyncResult)
		{
			if (asyncResult != this)
			{
				throw new ArgumentException("The IAsyncResult parameter must be the same parameter as returned by BeginCommit.", "asyncResult");
			}
			base.EndCommitInternal(this.asyncResult);
		}

		private void CommitCallback(IAsyncResult ar)
		{
			if (this.asyncResult == null && ar.CompletedSynchronously)
			{
				this.asyncResult = ar;
			}
			this.callback(this);
		}

		public void Commit()
		{
			base.CommitInternal();
		}

		[MonoTODO("Not implemented")]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		object IAsyncResult.AsyncState
		{
			get
			{
				return this.user_defined_state;
			}
		}

		WaitHandle IAsyncResult.AsyncWaitHandle
		{
			get
			{
				return this.asyncResult.AsyncWaitHandle;
			}
		}

		bool IAsyncResult.CompletedSynchronously
		{
			get
			{
				return this.asyncResult.CompletedSynchronously;
			}
		}

		bool IAsyncResult.IsCompleted
		{
			get
			{
				return this.asyncResult.IsCompleted;
			}
		}

		private TransactionOptions options;

		private AsyncCallback callback;

		private object user_defined_state;

		private IAsyncResult asyncResult;
	}
}
