using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class EvaluateException : InvalidExpressionException
	{
		public EvaluateException()
		{
		}

		protected EvaluateException(SerializationInfo info, StreamingContext context)
		{
		}

		public EvaluateException(string s)
		{
		}

		public EvaluateException(string message, Exception innerException)
		{
		}
	}
}
