using System;
using System.Runtime.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	public sealed class SqlNullValueException : SqlTypeException, ISerializable
	{
		public SqlNullValueException()
		{
		}

		public SqlNullValueException(string message)
		{
		}

		public SqlNullValueException(string message, Exception e)
		{
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
		}
	}
}
