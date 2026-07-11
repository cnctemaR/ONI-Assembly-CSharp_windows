using System;

namespace System.ComponentModel.Design
{
	public abstract class DesignerTransaction : IDisposable
	{
		protected DesignerTransaction()
			: this(string.Empty)
		{
		}

		protected DesignerTransaction(string description)
		{
			this.description = description;
			this.committed = false;
			this.canceled = false;
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			this.Cancel();
			if (disposing)
			{
				GC.SuppressFinalize(true);
			}
		}

		protected abstract void OnCancel();

		protected abstract void OnCommit();

		public void Cancel()
		{
			if (!this.Canceled && !this.Committed)
			{
				this.canceled = true;
				this.OnCancel();
			}
		}

		public void Commit()
		{
			if (!this.Canceled && !this.Committed)
			{
				this.committed = true;
				this.OnCommit();
			}
		}

		public bool Canceled
		{
			get
			{
				return this.canceled;
			}
		}

		public bool Committed
		{
			get
			{
				return this.committed;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
		}

		~DesignerTransaction()
		{
			this.Dispose(false);
		}

		private string description;

		private bool committed;

		private bool canceled;
	}
}
