using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public class EventLogNotFoundException : EventLogException
	{
		public EventLogNotFoundException()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		protected EventLogNotFoundException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogNotFoundException(string message)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogNotFoundException(string message, Exception innerException)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
