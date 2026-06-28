using System;
using System.Runtime.Serialization;
using System.Threading;

namespace System.Transactions
{
	[Serializable]
	public sealed class CommittableTransaction : Transaction, IDisposable, IAsyncResult, ISerializable
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

		public IAsyncResult BeginCommit(AsyncCallback callback, object user_defined_state)
		{
			this.callback = callback;
			this.user_defined_state = user_defined_state;
			AsyncCallback asyncCallback = null;
			if (callback != null)
			{
				asyncCallback = new AsyncCallback(this.CommitCallback);
			}
			this.asyncResult = base.BeginCommitInternal(asyncCallback);
			return this;
		}

		public void EndCommit(IAsyncResult ar)
		{
			if (ar != this)
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

		private TransactionOptions options;

		private AsyncCallback callback;

		private object user_defined_state;

		private IAsyncResult asyncResult;
	}
}
