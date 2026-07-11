using System;
using System.Net;
using System.Threading;

namespace Mono.Net.Dns
{
	internal class SimpleResolverEventArgs : EventArgs
	{
		public event EventHandler<SimpleResolverEventArgs> Completed;

		public ResolverError ResolverError { get; set; }

		public string ErrorMessage { get; set; }

		public string HostName { get; set; }

		public IPHostEntry HostEntry { get; internal set; }

		public object UserToken { get; set; }

		internal void Reset(ResolverAsyncOperation op)
		{
			this.ResolverError = ResolverError.NoError;
			this.ErrorMessage = null;
			this.HostEntry = null;
			this.LastOperation = op;
			this.QueryID = 0;
			this.Retries = 0;
			this.PTRAddress = null;
		}

		protected internal void OnCompleted(object sender)
		{
			EventHandler<SimpleResolverEventArgs> completed = this.Completed;
			if (completed != null)
			{
				completed(sender, this);
			}
		}

		public ResolverAsyncOperation LastOperation;

		internal ushort QueryID;

		internal ushort Retries;

		internal Timer Timer;

		internal IPAddress PTRAddress;
	}
}
