using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class InvalidConstraintException : DataException
	{
		public InvalidConstraintException()
		{
		}

		protected InvalidConstraintException(SerializationInfo info, StreamingContext context)
		{
		}

		public InvalidConstraintException(string s)
		{
		}

		public InvalidConstraintException(string message, Exception innerException)
		{
		}
	}
}
