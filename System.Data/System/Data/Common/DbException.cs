using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Data.Common
{
	[Serializable]
	public abstract class DbException : ExternalException
	{
		protected DbException()
		{
		}

		protected DbException(SerializationInfo info, StreamingContext context)
		{
		}

		protected DbException(string message)
		{
		}

		protected DbException(string message, Exception innerException)
		{
		}

		protected DbException(string message, int errorCode)
		{
		}
	}
}
