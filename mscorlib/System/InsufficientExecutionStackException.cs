using System;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public sealed class InsufficientExecutionStackException : SystemException
	{
		public InsufficientExecutionStackException()
			: base(Environment.GetResourceString("Insufficient stack to continue executing the program safely. This can happen from having too many functions on the call stack or function on the stack using too much stack space."))
		{
			base.SetErrorCode(-2146232968);
		}

		public InsufficientExecutionStackException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146232968);
		}

		public InsufficientExecutionStackException(string message, Exception innerException)
			: base(message, innerException)
		{
			base.SetErrorCode(-2146232968);
		}

		private InsufficientExecutionStackException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
