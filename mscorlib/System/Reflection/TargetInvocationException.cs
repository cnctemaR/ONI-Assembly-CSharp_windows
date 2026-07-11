using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public sealed class TargetInvocationException : ApplicationException
	{
		public TargetInvocationException(Exception inner)
			: base("Exception has been thrown by the target of an invocation.", inner)
		{
		}

		public TargetInvocationException(string message, Exception inner)
			: base(message, inner)
		{
		}

		internal TargetInvocationException(SerializationInfo info, StreamingContext sc)
			: base(info, sc)
		{
		}
	}
}
