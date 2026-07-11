using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class DataException : SystemException
	{
		public DataException()
		{
		}

		protected DataException(SerializationInfo info, StreamingContext context)
		{
		}

		public DataException(string s)
		{
		}

		public DataException(string s, Exception innerException)
		{
		}
	}
}
