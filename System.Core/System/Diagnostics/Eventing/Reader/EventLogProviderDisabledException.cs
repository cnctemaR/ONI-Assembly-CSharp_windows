using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public class EventLogProviderDisabledException : EventLogException
	{
		public EventLogProviderDisabledException()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		protected EventLogProviderDisabledException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogProviderDisabledException(string message)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogProviderDisabledException(string message, Exception innerException)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
