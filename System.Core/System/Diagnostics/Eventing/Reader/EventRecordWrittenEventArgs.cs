using System;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class EventRecordWrittenEventArgs : EventArgs
	{
		internal EventRecordWrittenEventArgs()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public Exception EventException
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}

		public EventRecord EventRecord
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
		}
	}
}
