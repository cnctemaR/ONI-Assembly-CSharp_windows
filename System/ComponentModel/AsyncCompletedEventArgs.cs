using System;
using System.Reflection;
using System.Security.Permissions;

namespace System.ComponentModel
{
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public class AsyncCompletedEventArgs : EventArgs
	{
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("This API supports the .NET Framework infrastructure and is not intended to be used directly from your code.", true)]
		public AsyncCompletedEventArgs()
		{
		}

		public AsyncCompletedEventArgs(Exception error, bool cancelled, object userState)
		{
			this.error = error;
			this.cancelled = cancelled;
			this.userState = userState;
		}

		[SRDescription("True if operation was cancelled.")]
		public bool Cancelled
		{
			get
			{
				return this.cancelled;
			}
		}

		[SRDescription("Exception that occurred during operation.  Null if no error.")]
		public Exception Error
		{
			get
			{
				return this.error;
			}
		}

		[SRDescription("User-supplied state to identify operation.")]
		public object UserState
		{
			get
			{
				return this.userState;
			}
		}

		protected void RaiseExceptionIfNecessary()
		{
			if (this.Error != null)
			{
				throw new TargetInvocationException(SR.GetString("An exception occurred during the operation, making the result invalid.  Check InnerException for exception details."), this.Error);
			}
			if (this.Cancelled)
			{
				throw new InvalidOperationException(SR.GetString("Operation has been cancelled."));
			}
		}

		private readonly Exception error;

		private readonly bool cancelled;

		private readonly object userState;
	}
}
