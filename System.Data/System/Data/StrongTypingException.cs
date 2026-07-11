using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class StrongTypingException : DataException
	{
		public StrongTypingException()
		{
		}

		protected StrongTypingException(SerializationInfo info, StreamingContext context)
		{
		}

		public StrongTypingException(string message)
		{
		}

		public StrongTypingException(string s, Exception innerException)
		{
		}
	}
}
