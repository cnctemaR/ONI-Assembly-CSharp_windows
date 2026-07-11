using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Unity;

namespace System.Diagnostics.Eventing.Reader
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public class EventLogException : Exception
	{
		public EventLogException()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		protected EventLogException(int errorCode)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		protected EventLogException(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogException(string message)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public EventLogException(string message, Exception innerException)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
