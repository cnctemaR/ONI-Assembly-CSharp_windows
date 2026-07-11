using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class MissingPrimaryKeyException : DataException
	{
		public MissingPrimaryKeyException()
		{
		}

		protected MissingPrimaryKeyException(SerializationInfo info, StreamingContext context)
		{
		}

		public MissingPrimaryKeyException(string s)
		{
		}

		public MissingPrimaryKeyException(string message, Exception innerException)
		{
		}
	}
}
