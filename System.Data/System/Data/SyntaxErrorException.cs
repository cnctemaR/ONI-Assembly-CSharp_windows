using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class SyntaxErrorException : InvalidExpressionException
	{
		protected SyntaxErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			throw new PlatformNotSupportedException();
		}

		public SyntaxErrorException()
		{
		}

		public SyntaxErrorException(string s)
			: base(s)
		{
		}

		public SyntaxErrorException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
