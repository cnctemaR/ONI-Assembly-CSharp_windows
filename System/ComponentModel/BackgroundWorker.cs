using System;
using System.Security.Permissions;
using System.Threading;

namespace System.ComponentModel
{
	[SRDescription("Executes an operation on a separate thread.")]
	[DefaultEvent("DoWork")]
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class BackgroundWorker : Component
	{
		public BackgroundWorker()
		{
			this.threadStart = new BackgroundWorker.WorkerThreadStartDelegate(this.WorkerThreadStart);
			this.operationCompleted = new SendOrPostCallback(this.AsyncOperationCompleted);
			this.progressReporter = new SendOrPostCallback(this.ProgressReporter);
		}

		private void AsyncOperationCompleted(object arg)
		{
			this.isRunning = false;
			this.cancellationPending = false;
			this.OnRunWorkerCompleted((RunWorkerCompletedEventArgs)arg);
		}

		[SRDescription("Has the user attempted to cancel the operation? To be accessed from DoWork event handler.")]
		[Browsable(false)]
		public bool CancellationPending
		{
			get
			{
				return this.cancellationPending;
			}
		}

		public void CancelAsync()
		{
			if (!this.WorkerSupportsCancellation)
			{
				throw new InvalidOperationException(global::SR.GetString("This BackgroundWorker states that it doesn't support cancellation. Modify WorkerSupportsCancellation to state that it does support cancellation."));
			}
			this.cancellationPending = true;
		}

		[SRDescription("Event handler to be run on a different thread when the operation begins.")]
		[SRCategory("Asynchronous")]
		public event DoWorkEventHandler DoWork
		{
			add
			{
				base.Events.AddHandler(BackgroundWorker.doWorkKey, value);
			}
			remove
			{
				base.Events.RemoveHandler(BackgroundWorker.doWorkKey, value);
			}
		}

		[SRDescription("Is the worker still currently working on a background operation?")]
		[Browsable(false)]
		public bool IsBusy
		{
			get
			{
				return this.isRunning;
			}
		}

		protected virtual void OnDoWork(DoWorkEventArgs e)
		{
			DoWorkEventHandler doWorkEventHandler = (DoWorkEventHandler)base.Events[BackgroundWorker.doWorkKey];
			if (doWorkEventHandler != null)
			{
				doWorkEventHandler(this, e);
			}
		}

		protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
		{
			RunWorkerCompletedEventHandler runWorkerCompletedEventHandler = (RunWorkerCompletedEventHandler)base.Events[BackgroundWorker.runWorkerCompletedKey];
			if (runWorkerCompletedEventHandler != null)
			{
				runWorkerCompletedEventHandler(this, e);
			}
		}

		protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
		{
			ProgressChangedEventHandler progressChangedEventHandler = (ProgressChangedEventHandler)base.Events[BackgroundWorker.progressChangedKey];
			if (progressChangedEventHandler != null)
			{
				progressChangedEventHandler(this, e);
			}
		}

		[SRCategory("Asynchronous")]
		[SRDescription("Raised when the worker thread indicates that some progress has been made.")]
		public event ProgressChangedEventHandler ProgressChanged
		{
			add
			{
				base.Events.AddHandler(BackgroundWorker.progressChangedKey, value);
			}
			remove
			{
				base.Events.RemoveHandler(BackgroundWorker.progressChangedKey, value);
			}
		}

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
				throw new InvalidOperationException(global::SR.GetString("This BackgroundWorker states that it doesn't report progress. Modify WorkerReportsProgress to state that it does report progress."));
			}
			ProgressChangedEventArgs e = new ProgressChangedEventArgs(percentProgress, userState);
			if (this.asyncOperation != null)
			{
				this.asyncOperation.Post(this.progressReporter, e);
				return;
			}
			this.progressReporter(e);
		}

		public void RunWorkerAsync()
		{
			this.RunWorkerAsync(null);
		}

		public void RunWorkerAsync(object argument)
		{
			if (this.isRunning)
			{
				throw new InvalidOperationException(global::SR.GetString("This BackgroundWorker is currently busy and cannot run multiple tasks concurrently."));
			}
			this.isRunning = true;
			this.cancellationPending = false;
			this.asyncOperation = AsyncOperationManager.CreateOperation(null);
			this.threadStart.BeginInvoke(argument, null, null);
		}

		[SRDescription("Raised when the worker has completed (either through success, failure, or cancellation).")]
		[SRCategory("Asynchronous")]
		public event RunWorkerCompletedEventHandler RunWorkerCompleted
		{
			add
			{
				base.Events.AddHandler(BackgroundWorker.runWorkerCompletedKey, value);
			}
			remove
			{
				base.Events.RemoveHandler(BackgroundWorker.runWorkerCompletedKey, value);
			}
		}

		[SRCategory("Asynchronous")]
		[SRDescription("Whether the worker will report progress.")]
		[DefaultValue(false)]
		public bool WorkerReportsProgress
		{
			get
			{
				return this.workerReportsProgress;
			}
			set
			{
				this.workerReportsProgress = value;
			}
		}

		[DefaultValue(false)]
		[SRDescription("Whether the worker supports cancellation.")]
		[SRCategory("Asynchronous")]
		public bool WorkerSupportsCancellation
		{
			get
			{
				return this.canCancelWorker;
			}
			set
			{
				this.canCancelWorker = value;
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
			this.asyncOperation.PostOperationCompleted(this.operationCompleted, e2);
		}

		private static readonly object doWorkKey = new object();

		private static readonly object runWorkerCompletedKey = new object();

		private static readonly object progressChangedKey = new object();

		private bool canCancelWorker;

		private bool workerReportsProgress;

		private bool cancellationPending;

		private bool isRunning;

		private AsyncOperation asyncOperation;

		private readonly BackgroundWorker.WorkerThreadStartDelegate threadStart;

		private readonly SendOrPostCallback operationCompleted;

		private readonly SendOrPostCallback progressReporter;

		private delegate void WorkerThreadStartDelegate(object argument);
	}
}
