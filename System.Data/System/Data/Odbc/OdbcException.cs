using System;
using System.Data.Common;
using System.Runtime.Serialization;

namespace System.Data.Odbc
{
	[Serializable]
	public sealed class OdbcException : DbException
	{
		internal OdbcException()
		{
		}

		public OdbcErrorCollection Errors
		{
			get
			{
				throw null;
			}
		}

		public override string Source
		{
			get
			{
				throw null;
			}
		}

		public override void GetObjectData(SerializationInfo si, StreamingContext context)
		{
		}
	}
}
