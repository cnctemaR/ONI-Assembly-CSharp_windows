using System;
using System.Runtime.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	public class SqlTypeException : SystemException, ISerializable
	{
		public SqlTypeException()
		{
		}

		protected SqlTypeException(SerializationInfo si, StreamingContext sc)
		{
		}

		public SqlTypeException(string message)
		{
		}

		public SqlTypeException(string message, Exception e)
		{
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
		}
	}
}
