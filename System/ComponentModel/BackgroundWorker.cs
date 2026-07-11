using System;
using System.Threading;

namespace System.ComponentModel
{
	[DefaultEvent("DoWork")]
	public class BackgroundWorker : Component
	{
		public event DoWorkEventHandler DoWork;

		public event ProgressChangedEventHandler ProgressChanged;

		public event RunWorkerCompletedEventHandler RunWorkerCompleted;

		[Browsable(false)]
		public bool CancellationPending
		{
			get
			{
				return this.cancel_pending;
			}
		}

		[Browsable(false)]
		public bool IsBusy
		{
			get
			{
				return this.async != null;
			}
		}

		[DefaultValue(false)]
		public bool WorkerReportsProgress
		{
			get
			{
				return this.report_progress;
			}
			set
			{
				this.report_progress = value;
			}
		}

		[DefaultValue(false)]
		public bool WorkerSupportsCancellation
		{
			get
			{
				return this.support_cancel;
			}
			set
			{
				this.support_cancel = value;
			}
		}

		public void CancelAsync()
		{
			if (!this.support_cancel)
			{
				throw new InvalidOperationException("This background worker does not support cancellation.");
			}
			if (!this.IsBusy)
			{
				return;
			}
			this.cancel_pending = true;
		}

		public void ReportProgress(int percentProgress)
		{
			this.ReportProgress(percentProgress, null);
		}

		public void ReportProgress(int percentProgress, object userState)
		{
			if (!this.WorkerReportsProgress)
			{
				throw new InvalidOperationException("This background worker does not report progress.");
			}
			if (!this.IsBusy)
			{
				return;
			}
			this.async.Post(delegate(object o)
			{
				ProgressChangedEventArgs e = o as ProgressChangedEventArgs;
				this.OnProgressChanged(e);
			}, new ProgressChangedEventArgs(percentProgress, userState));
		}

		public void RunWorkerAsync()
		{
			this.RunWorkerAsync(null);
		}

		private void ProcessWorker(object argument, AsyncOperation async, SendOrPostCallback callback)
		{
			Exception ex = null;
			DoWorkEventArgs e = new DoWorkEventArgs(argument);
			try
			{
				this.OnDoWork(e);
			}
			catch (Exception ex2)
			{
				ex = ex2;
				e.Cancel = false;
			}
			callback(new object[]
			{
				new RunWorkerCompletedEventArgs(e.Result, ex, e.Cancel),
				async
			});
		}

		private void CompleteWorker(object state)
		{
			object[] array = (object[])state;
			RunWorkerCompletedEventArgs e = array[0] as RunWorkerCompletedEventArgs;
			AsyncOperation asyncOperation = array[1] as AsyncOperation;
			SendOrPostCallback sendOrPostCallback = delegate(object darg)
			{
				this.async = null;
				this.OnRunWorkerCompleted(darg as RunWorkerCompletedEventArgs);
			};
			asyncOperation.PostOperationCompleted(sendOrPostCallback, e);
			this.cancel_pending = false;
		}

		public void RunWorkerAsync(object argument)
		{
			if (this.IsBusy)
			{
				throw new InvalidOperationException("The background worker is busy.");
			}
			this.async = AsyncOperationManager.CreateOperation(this);
			BackgroundWorker.ProcessWorkerEventHandler processWorkerEventHandler = new BackgroundWorker.ProcessWorkerEventHandler(this.ProcessWorker);
			processWorkerEventHandler.BeginInvoke(argument, this.async, new SendOrPostCallback(this.CompleteWorker), null, null);
		}

		protected virtual void OnDoWork(DoWorkEventArgs e)
		{
			if (this.DoWork != null)
			{
				this.DoWork(this, e);
			}
		}

		protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
		{
			if (this.ProgressChanged != null)
			{
				this.ProgressChanged(this, e);
			}
		}

		protected virtual void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
		{
			if (this.RunWorkerCompleted != null)
			{
				this.RunWorkerCompleted(this, e);
			}
		}

		private AsyncOperation async;

		private bool cancel_pending;

		private bool report_progress;

		private bool support_cancel;

		private delegate void ProcessWorkerEventHandler(object argument, AsyncOperation async, SendOrPostCallback callback);
	}
}
