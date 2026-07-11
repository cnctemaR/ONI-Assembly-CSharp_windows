using System;
using System.Runtime.Serialization;

namespace System.Data
{
	[Serializable]
	public class DeletedRowInaccessibleException : DataException
	{
		public DeletedRowInaccessibleException()
		{
		}

		protected DeletedRowInaccessibleException(SerializationInfo info, StreamingContext context)
		{
		}

		public DeletedRowInaccessibleException(string s)
		{
		}

		public DeletedRowInaccessibleException(string message, Exception innerException)
		{
		}
	}
}
