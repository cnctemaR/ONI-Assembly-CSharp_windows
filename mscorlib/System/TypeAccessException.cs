using System;
using System.Runtime.Serialization;

namespace System
{
	[Serializable]
	public class TypeAccessException : TypeLoadException
	{
		public TypeAccessException()
			: base(Environment.GetResourceString("Attempt to access the type failed."))
		{
			base.SetErrorCode(-2146233021);
		}

		public TypeAccessException(string message)
			: base(message)
		{
			base.SetErrorCode(-2146233021);
		}

		public TypeAccessException(string message, Exception inner)
			: base(message, inner)
		{
			base.SetErrorCode(-2146233021);
		}

		protected TypeAccessException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			base.SetErrorCode(-2146233021);
		}
	}
}
