using System;
using System.Threading;
using System.Threading.Tasks;

namespace System.ComponentModel
{
	[DefaultEvent("DoWork")]
	public class BackgroundWorker : Component
	{
		public BackgroundWorker()
		{
			this._operationCompleted = new SendOrPostCallback(this.AsyncOperationCompleted);
			this._progressReporter = new SendOrPostCallback(this.ProgressReporter);
		}

		private void AsyncOperationCompleted(object arg)
		{
			this._isRunning = false;
			this._cancellationPending = false;
			this.OnRunWorkerCompleted((RunWorkerCompletedEventArgs)arg);
		}

		public bool CancellationPending
		{
			get
			{
				return this._cancellationPending;
			}
		}

		public void CancelAsync()
		{
			if (!this.WorkerSupportsCancellation)
			{
				throw new InvalidOperationException("This BackgroundWorker states that it doesn't support cancellation. Modify WorkerSupportsCancellation to state that it does support cancellation.");
			}
			this._cancellationPending = true;
		}

		public event DoWorkEventHandler DoWork;

		public bool IsBusy
		{
			get
			{
				return this._isRunning;
			}
		}

		protected virtual void OnDoWork(DoWorkEventArgs e)
		{
			DoWorkEventHandler doWork = this.DoWork;
			if (doWork != null)
			{
				doWork(this, e);
			}
		}

		protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
		{
			RunWorkerCompletedEventHandler runWorkerCompleted = this.RunWorkerCompleted;
			if (runWorkerCompleted != null)
			{
				runWorkerCompleted(this, e);
			}
		}

		protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
		{
			ProgressChangedEventHandler progressChanged = this.ProgressChanged;
			if (progressChanged != null)
			{
				progressChanged(this, e);
			}
		}

		public event ProgressChangedEventHandler ProgressChanged;

		private void ProgressReporter(object arg)
		{
			this.OnProgressChanged((ProgressChangedEventArgs)arg);
		}

		public void ReportProgress(int percentProgress)
		{
			this.ReportProgress(percentProgress, null);
		}

		public void ReportProgress(int percentProgress, object userState)
		{
			if (!this.WorkerReportsProgress)
			{
				throw new InvalidOperationException("This BackgroundWorker states that it doesn't report progress. Modify WorkerReportsProgress to state that it does report progress.");
			}
			ProgressChangedEventArgs e = new ProgressChangedEventArgs(percentProgress, userState);
			if (this._asyncOperation != null)
			{
				this._asyncOperation.Post(this._progressReporter, e);
				return;
			}
			this._progressReporter(e);
		}

		public void RunWorkerAsync()
		{
			this.RunWorkerAsync(null);
		}

		public void RunWorkerAsync(object argument)
		{
			if (this._isRunning)
			{
				throw new InvalidOperationException("This BackgroundWorker is currently busy and cannot run multiple tasks concurrently.");
			}
			this._isRunning = true;
			this._cancellationPending = false;
			this._asyncOperation = AsyncOperationManager.CreateOperation(null);
			Task.Factory.StartNew(delegate(object arg)
			{
				this.WorkerThreadStart(arg);
			}, argument, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
		}

		public event RunWorkerCompletedEventHandler RunWorkerCompleted;

		public bool WorkerReportsProgress
		{
			get
			{
				return this._workerReportsProgress;
			}
			set
			{
				this._workerReportsProgress = value;
			}
		}

		public bool WorkerSupportsCancellation
		{
			get
			{
				return this._canCancelWorker;
			}
			set
			{
				this._canCancelWorker = value;
			}
		}

		private void WorkerThreadStart(object argument)
		{
			object obj = null;
			Exception ex = null;
			bool flag = false;
			try
			{
				DoWorkEventArgs e = new DoWorkEventArgs(argument);
				this.OnDoWork(e);
				if (e.Cancel)
				{
					flag = true;
				}
				else
				{
					obj = e.Result;
				}
			}
			catch (Exception ex)
			{
			}
			RunWorkerCompletedEventArgs e2 = new RunWorkerCompletedEventArgs(obj, ex, flag);
			this._asyncOperation.PostOperationCompleted(this._operationCompleted, e2);
		}

		protected override void Dispose(bool disposing)
		{
		}

		private bool _canCancelWorker;

		private bool _workerReportsProgress;

		private bool _cancellationPending;

		private bool _isRunning;

		private AsyncOperation _asyncOperation;

		private readonly SendOrPostCallback _operationCompleted;

		private readonly SendOrPostCallback _progressReporter;
	}
}
