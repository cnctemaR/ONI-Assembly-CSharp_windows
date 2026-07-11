using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class InRowChangingEventException : DataException
	{
		public InRowChangingEventException()
		{
		}

		protected InRowChangingEventException(SerializationInfo info, StreamingContext context)
		{
		}

		public InRowChangingEventException(string s)
		{
		}

		public InRowChangingEventException(string message, Exception innerException)
		{
		}
	}
}
