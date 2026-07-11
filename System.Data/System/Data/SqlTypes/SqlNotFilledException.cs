using System;

namespace System.Data.SqlTypes
{
	[Serializable]
	public sealed class SqlNotFilledException : SqlTypeException
	{
		public SqlNotFilledException()
			: this(SQLResource.NotFilledMessage, null)
		{
		}

		public SqlNotFilledException(string message)
			: this(message, null)
		{
		}

		public SqlNotFilledException(string message, Exception e)
			: base(message, e)
		{
			base.HResult = -2146232015;
		}
	}
}
