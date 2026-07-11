using System;
using System.Runtime.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	public sealed class SqlNotFilledException : SqlTypeException, ISerializable
	{
		public SqlNotFilledException()
		{
		}

		public SqlNotFilledException(string message)
		{
		}

		public SqlNotFilledException(string message, Exception e)
		{
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
		}
	}
}
