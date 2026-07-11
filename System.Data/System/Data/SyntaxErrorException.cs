using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class SyntaxErrorException : InvalidExpressionException
	{
		public SyntaxErrorException()
		{
		}

		protected SyntaxErrorException(SerializationInfo info, StreamingContext context)
		{
		}

		public SyntaxErrorException(string s)
		{
		}

		public SyntaxErrorException(string message, Exception innerException)
		{
		}
	}
}
