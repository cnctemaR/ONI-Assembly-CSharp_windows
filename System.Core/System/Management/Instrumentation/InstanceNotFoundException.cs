using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Unity;

namespace System.Management.Instrumentation
{
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	[Serializable]
	public class InstanceNotFoundException : InstrumentationException
	{
		public InstanceNotFoundException()
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		protected InstanceNotFoundException(SerializationInfo info, StreamingContext context)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public InstanceNotFoundException(string message)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}

		public InstanceNotFoundException(string message, Exception innerException)
		{
			global::Unity.ThrowStub.ThrowNotSupportedException();
		}
	}
}
