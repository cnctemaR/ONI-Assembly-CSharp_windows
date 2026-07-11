using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public class InstrumentationBaseException : Exception
	{
		public InstrumentationBaseException()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		protected InstrumentationBaseException(SerializationInfo info, StreamingContext context)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public InstrumentationBaseException(string message)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public InstrumentationBaseException(string message, Exception innerException)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
