using System;

namespace System.Threading.Tasks
{
	public class UnobservedTaskExceptionEventArgs : EventArgs
	{
		public UnobservedTaskExceptionEventArgs(AggregateException exception)
		{
			this.m_exception = exception;
		}

		public void SetObserved()
		{
			this.m_observed = true;
		}

		public bool Observed
		{
			get
			{
				return this.m_observed;
			}
		}

		public AggregateException Exception
		{
			get
			{
				return this.m_exception;
			}
		}

		private AggregateException m_exception;

		internal bool m_observed;
	}
}
