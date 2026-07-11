using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class DuplicateNameException : DataException
	{
		public DuplicateNameException()
		{
		}

		protected DuplicateNameException(SerializationInfo info, StreamingContext context)
		{
		}

		public DuplicateNameException(string s)
		{
		}

		public DuplicateNameException(string message, Exception innerException)
		{
		}
	}
}
