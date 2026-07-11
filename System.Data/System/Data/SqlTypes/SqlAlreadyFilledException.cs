using System;

namespace System.Data.SqlTypes
{
	[Serializable]
	public sealed class SqlAlreadyFilledException : SqlTypeException
	{
		public SqlAlreadyFilledException()
		{
		}

		public SqlAlreadyFilledException(string message)
		{
		}

		public SqlAlreadyFilledException(string message, Exception e)
		{
		}
	}
}
