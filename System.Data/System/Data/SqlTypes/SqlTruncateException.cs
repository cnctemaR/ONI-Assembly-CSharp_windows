using System;
using System.Runtime.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	public sealed class SqlTruncateException : SqlTypeException, ISerializable
	{
		public SqlTruncateException()
		{
		}

		public SqlTruncateException(string message)
		{
		}

		public SqlTruncateException(string message, Exception e)
		{
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
		}
	}
}
