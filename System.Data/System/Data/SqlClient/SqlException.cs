using System;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.Serialization;
using System.Text;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlClient
{
	[Serializable]
	public sealed class SqlException : DbException
	{
		internal SqlException()
			: this("SQL Exception has occured.", null, null)
		{
		}

		internal SqlException(string message, Exception inner)
			: this(message, inner, null)
		{
		}

		internal SqlException(string message, Exception inner, SqlError sqlError)
			: base((message != null) ? message : "SQL Exception has occured.", inner)
		{
			base.HResult = -2146232060;
			this.errors = new SqlErrorCollection();
			if (sqlError != null)
			{
				this.errors.Add(sqlError);
			}
		}

		internal SqlException(byte theClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
			: this(null, null, new SqlError(theClass, lineNumber, message, number, procedure, server, source, state))
		{
		}

		private SqlException(SerializationInfo si, StreamingContext sc)
		{
			base.HResult = -2146232060;
			this.errors = (SqlErrorCollection)si.GetValue("Errors", typeof(SqlErrorCollection));
		}

		public byte Class
		{
			get
			{
				return this.Errors[0].Class;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public SqlErrorCollection Errors
		{
			get
			{
				return this.errors;
			}
		}

		public int LineNumber
		{
			get
			{
				return this.Errors[0].LineNumber;
			}
		}

		public override string Message
		{
			get
			{
				if (this.Errors.Count == 0)
				{
					return base.Message;
				}
				StringBuilder stringBuilder = new StringBuilder();
				if (base.Message != "SQL Exception has occured.")
				{
					stringBuilder.Append(base.Message);
					stringBuilder.Append("\n");
				}
				for (int i = 0; i < this.Errors.Count - 1; i++)
				{
					stringBuilder.Append(this.Errors[i].Message);
					stringBuilder.Append("\n");
				}
				stringBuilder.Append(this.Errors[this.Errors.Count - 1].Message);
				return stringBuilder.ToString();
			}
		}

		public int Number
		{
			get
			{
				return this.Errors[0].Number;
			}
		}

		public string Procedure
		{
			get
			{
				return this.Errors[0].Procedure;
			}
		}

		public string Server
		{
			get
			{
				return this.Errors[0].Server;
			}
		}

		public override string Source
		{
			get
			{
				return this.Errors[0].Source;
			}
		}

		public byte State
		{
			get
			{
				return this.Errors[0].State;
			}
		}

		internal static SqlException FromTdsInternalException(TdsInternalException e)
		{
			SqlError sqlError = new SqlError(e.Class, e.LineNumber, e.Message, e.Number, e.Procedure, e.Server, "Mono SqlClient Data Provider", e.State);
			return new SqlException(null, e, sqlError);
		}

		public override void GetObjectData(SerializationInfo si, StreamingContext context)
		{
			if (si == null)
			{
				throw new ArgumentNullException("si");
			}
			si.AddValue("Errors", this.errors, typeof(SqlErrorCollection));
			base.GetObjectData(si, context);
		}

		private const string DEF_MESSAGE = "SQL Exception has occured.";

		private readonly SqlErrorCollection errors;
	}
}
