using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Threading
{
	[ComVisible(true)]
	[Serializable]
	public class SynchronizationLockException : SystemException
	{
		public SynchronizationLockException()
			: base("Synchronization Error")
		{
		}

		public SynchronizationLockException(string message)
			: base(message)
		{
		}

		protected SynchronizationLockException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public SynchronizationLockException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
