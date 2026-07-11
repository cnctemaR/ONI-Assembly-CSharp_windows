using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public class EventLogReadingException : EventLogException
	{
		public EventLogReadingException()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		protected EventLogReadingException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogReadingException(string message)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogReadingException(string message, Exception innerException)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
