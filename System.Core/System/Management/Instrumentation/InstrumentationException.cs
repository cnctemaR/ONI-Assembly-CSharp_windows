using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public class InstrumentationException : InstrumentationBaseException
	{
		public InstrumentationException()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public InstrumentationException(Exception innerException)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		protected InstrumentationException(SerializationInfo info, StreamingContext context)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public InstrumentationException(string message)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public InstrumentationException(string message, Exception innerException)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
