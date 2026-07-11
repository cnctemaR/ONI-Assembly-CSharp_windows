using System;

namespace Mono.Net.Security
{
	internal class AsyncShutdownRequest : AsyncProtocolRequest
	{
		public AsyncShutdownRequest(MobileAuthenticatedStream parent)
			: base(parent, false)
		{
		}

		protected override AsyncOperationStatus Run(AsyncOperationStatus status)
		{
			return base.Parent.ProcessShutdown(status);
		}
	}
}
