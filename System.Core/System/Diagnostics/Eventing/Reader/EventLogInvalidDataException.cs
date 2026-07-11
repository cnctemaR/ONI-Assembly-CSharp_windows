using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public class EventLogInvalidDataException : EventLogException
	{
		public EventLogInvalidDataException()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		protected EventLogInvalidDataException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogInvalidDataException(string message)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogInvalidDataException(string message, Exception innerException)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
