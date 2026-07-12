using System;

namespace System.ComponentModel.Design
{
	public abstract class DesignerTransaction : IDisposable
	{
		protected DesignerTransaction()
			: this("")
		{
		}

		protected DesignerTransaction(string description)
		{
			this.Description = description;
		}

		public bool Canceled { get; private set; }

		public bool Committed { get; private set; }

		public string Description { get; }

		public void Cancel()
		{
			if (!this.Canceled && !this.Committed)
			{
				this.Canceled = true;
				GC.SuppressFinalize(this);
				this._suppressedFinalization = true;
				this.OnCancel();
			}
		}

		public void Commit()
		{
			if (!this.Committed && !this.Canceled)
			{
				this.Committed = true;
				GC.SuppressFinalize(this);
				this._suppressedFinalization = true;
				this.OnCommit();
			}
		}

		protected abstract void OnCancel();

		protected abstract void OnCommit();

		~DesignerTransaction()
		{
			this.Dispose(false);
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			if (!this._suppressedFinalization)
			{
				GC.SuppressFinalize(this);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			this.Cancel();
		}

		private bool _suppressedFinalization;
	}
}
