using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public sealed class TargetInvocationException : ApplicationException
	{
		private TargetInvocationException()
			: base(Environment.GetResourceString("Exception has been thrown by the target of an invocation."))
		{
			base.SetErrorCode(-2146232828);
		}

		private TargetInvocationException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146232828);
		}

		public TargetInvocationException(Exception inner)
			: base(Environment.GetResourceString("Exception has been thrown by the target of an invocation."), inner)
		{
			base.SetErrorCode(-2146232828);
		}

		public TargetInvocationException(string message, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(-2146232828);
		}

		internal TargetInvocationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
