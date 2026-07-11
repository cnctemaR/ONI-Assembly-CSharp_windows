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
			: base(Environment.GetResourceString("Object synchronization method was called from an unsynchronized block of code."))
		{
			base.SetErrorCode(-2146233064);
		}

		public SynchronizationLockException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233064);
		}

		public SynchronizationLockException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.SetErrorCode(-2146233064);
		}

		protected SynchronizationLockException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
