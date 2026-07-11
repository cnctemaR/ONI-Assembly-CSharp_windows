using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class RowNotInTableException : DataException
	{
		public RowNotInTableException()
		{
		}

		protected RowNotInTableException(SerializationInfo info, StreamingContext context)
		{
		}

		public RowNotInTableException(string s)
		{
		}

		public RowNotInTableException(string message, Exception innerException)
		{
		}
	}
}
