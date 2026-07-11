using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class ConstraintException : DataException
	{
		public ConstraintException()
		{
		}

		protected ConstraintException(SerializationInfo info, StreamingContext context)
		{
		}

		public ConstraintException(string s)
		{
		}

		public ConstraintException(string message, Exception innerException)
		{
		}
	}
}
