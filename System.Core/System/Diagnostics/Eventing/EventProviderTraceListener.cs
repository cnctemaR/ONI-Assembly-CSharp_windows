using System;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class EventProviderTraceListener : TraceListener
	{
		public EventProviderTraceListener(string providerId)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventProviderTraceListener(string providerId, string name)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventProviderTraceListener(string providerId, string name, string delimiter)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public string Delimiter
		{
			get
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
				return null;
			}
			set
			{
				global::Unity.ThrowStub.ThrowNotSupportedException();
			}
		}

		public sealed override void Write(string message)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public sealed override void WriteLine(string message)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
