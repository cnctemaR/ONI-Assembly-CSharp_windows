using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Threading
{
	[TypeForwardedFrom("System, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	[Serializable]
	public class SemaphoreFullException : SystemException
	{
		public SemaphoreFullException()
			: base("Adding the specified count to the semaphore would cause it to exceed its maximum count.")
		{
		}

		public SemaphoreFullException(string message)
			: base(message)
		{
		}

		public SemaphoreFullException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected SemaphoreFullException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
