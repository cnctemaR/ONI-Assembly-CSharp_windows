using System;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.Serialization;

namespace System.Data.SqlClient
{
	[Serializable]
	public sealed class SqlException : DbException
	{
		internal SqlException()
		{
		}

		public byte Class
		{
			get
			{
				throw null;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SqlErrorCollection Errors
		{
			get
			{
				throw null;
			}
		}

		public int LineNumber
		{
			get
			{
				throw null;
			}
		}

		public override string Message
		{
			get
			{
				throw null;
			}
		}

		public int Number
		{
			get
			{
				throw null;
			}
		}

		public string Procedure
		{
			get
			{
				throw null;
			}
		}

		public string Server
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

		public byte State
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
