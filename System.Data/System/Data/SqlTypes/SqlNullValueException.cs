using System;
using System.Runtime.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	public sealed class SqlNullValueException : SqlTypeException, ISerializable
	{
		public SqlNullValueException()
			: base(Locale.GetText("Data is Null. This method or property cannot be called on Null values."))
		{
		}

		public SqlNullValueException(string message)
			: base(message)
		{
		}

		public SqlNullValueException(string message, Exception e)
			: base(message, e)
		{
		}

		private SqlNullValueException(SerializationInfo si, StreamingContext sc)
			: base(si.GetString("SqlNullValueExceptionMessage"))
		{
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			si.AddValue("SqlNullValueExceptionMessage", this.Message, typeof(string));
		}
	}
}
