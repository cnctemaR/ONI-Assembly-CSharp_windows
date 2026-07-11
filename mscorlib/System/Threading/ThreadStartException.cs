using System;
using System.Runtime.Serialization;

namespace System.Threading
{
	[Serializable]
	public sealed class ThreadStartException : SystemException
	{
		private ThreadStartException()
			: base(Environment.GetResourceString("Thread failed to start."))
		{
			base.SetErrorCode(-2146233051);
		}

		private ThreadStartException(Exception reason)
			: base(Environment.GetResourceString("Thread failed to start."), reason)
		{
			base.SetErrorCode(-2146233051);
		}

		internal ThreadStartException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
