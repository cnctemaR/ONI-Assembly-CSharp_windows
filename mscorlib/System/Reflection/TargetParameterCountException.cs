using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[ComVisible(true)]
	[Serializable]
	public sealed class TargetParameterCountException : ApplicationException
	{
		public TargetParameterCountException()
			: base(Environment.GetResourceString("Number of parameters specified does not match the expected number."))
		{
			base.SetErrorCode(-2147352562);
		}

		public TargetParameterCountException(string message)
			: base(message)
		{
			base.SetErrorCode(-2147352562);
		}

		public TargetParameterCountException(string message, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(-2147352562);
		}

		internal TargetParameterCountException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
