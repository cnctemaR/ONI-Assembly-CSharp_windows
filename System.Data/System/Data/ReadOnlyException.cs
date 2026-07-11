using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class ReadOnlyException : DataException
	{
		public ReadOnlyException()
		{
		}

		protected ReadOnlyException(SerializationInfo info, StreamingContext context)
		{
		}

		public ReadOnlyException(string s)
		{
		}

		public ReadOnlyException(string message, Exception innerException)
		{
		}
	}
}
