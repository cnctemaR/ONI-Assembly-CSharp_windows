using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class InvalidExpressionException : DataException
	{
		public InvalidExpressionException()
		{
		}

		protected InvalidExpressionException(SerializationInfo info, StreamingContext context)
		{
		}

		public InvalidExpressionException(string s)
		{
		}

		public InvalidExpressionException(string message, Exception innerException)
		{
		}
	}
}
