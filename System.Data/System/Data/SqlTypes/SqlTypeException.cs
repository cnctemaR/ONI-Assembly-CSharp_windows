using System;
using System.Runtime.Serialization;

namespace System.Data.SqlTypes
{
	[Serializable]
	public class SqlTypeException : SystemException, ISerializable
	{
		public SqlTypeException()
			: base(Locale.GetText("A sql exception has occured."))
		{
		}

		public SqlTypeException(string message)
			: base(message)
		{
		}

		public SqlTypeException(string message, Exception e)
			: base(message, e)
		{
		}

		protected SqlTypeException(SerializationInfo si, StreamingContext sc)
			: base(si.GetString("SqlTypeExceptionMessage"))
		{
		}

		void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
		{
			si.AddValue("SqlTypeExceptionMessage", this.Message, typeof(string));
		}
	}
}
