using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class NoNullAllowedException : DataException
	{
		public NoNullAllowedException()
		{
		}

		protected NoNullAllowedException(SerializationInfo info, StreamingContext context)
		{
		}

		public NoNullAllowedException(string s)
		{
		}

		public NoNullAllowedException(string message, Exception innerException)
		{
		}
	}
}
