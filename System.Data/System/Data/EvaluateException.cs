using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class EvaluateException : InvalidExpressionException
	{
		protected EvaluateException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			throw new PlatformNotSupportedException();
		}

		public EvaluateException()
		{
		}

		public EvaluateException(string s)
			: base(s)
		{
		}

		public EvaluateException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
