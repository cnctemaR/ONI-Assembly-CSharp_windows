using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class UnauthorizedAccessException : SystemException
	{
		public UnauthorizedAccessException()
			: base(Environment.GetResourceString("Attempted to perform an unauthorized operation."))
		{
			base.SetErrorCode(-2147024891);
		}

		public UnauthorizedAccessException(string message)
			: base(message)
		{
			base.SetErrorCode(-2147024891);
		}

		public UnauthorizedAccessException(string message, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(-2147024891);
		}

		protected UnauthorizedAccessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
