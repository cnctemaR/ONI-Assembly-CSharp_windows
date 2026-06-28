using System;

namespace System.Data.SqlClient
{
	[Serializable]
	public sealed class SqlError
	{
		internal SqlError(byte errorClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
		{
			this.errorClass = errorClass;
			this.lineNumber = lineNumber;
			this.message = message;
			this.number = number;
			this.procedure = procedure;
			this.server = server;
			this.source = source;
			this.state = state;
		}

		public byte Class
		{
			get
			{
				return this.errorClass;
			}
		}

		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		public string Message
		{
			get
			{
				return this.message;
			}
		}

		public int Number
		{
			get
			{
				return this.number;
			}
		}

		public string Procedure
		{
			get
			{
				return this.procedure;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
		}

		public string Source
		{
			get
			{
				return this.source;
			}
		}

		public byte State
		{
			get
			{
				return this.state;
			}
		}

		public override string ToString()
		{
			return this.Message;
		}

		private byte errorClass;

		private int lineNumber;

		private string message = string.Empty;

		private int number;

		private string procedure = string.Empty;

		private string source = string.Empty;

		private byte state;

		[NonSerialized]
		private string server = string.Empty;
	}
}
