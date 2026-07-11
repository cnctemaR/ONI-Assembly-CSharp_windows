using System;
using System.Security.Permissions;

namespace System.ComponentModel.Design
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	[PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
	public abstract class DesignerTransaction : IDisposable
	{
		protected DesignerTransaction()
			: this("")
		{
		}

		protected DesignerTransaction(string description)
		{
			this.desc = description;
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
				return this.desc;
			}
		}

		public void Cancel()
		{
			if (!this.canceled && !this.committed)
			{
				this.canceled = true;
				GC.SuppressFinalize(this);
				this.suppressedFinalization = true;
				this.OnCancel();
			}
		}

		public void Commit()
		{
			if (!this.committed && !this.canceled)
			{
				this.committed = true;
				GC.SuppressFinalize(this);
				this.suppressedFinalization = true;
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
			if (!this.suppressedFinalization)
			{
				GC.SuppressFinalize(this);
			}
		}

		protected virtual void Dispose(bool disposing)
		{
			this.Cancel();
		}

		private bool committed;

		private bool canceled;

		private bool suppressedFinalization;

		private string desc;
	}
}
