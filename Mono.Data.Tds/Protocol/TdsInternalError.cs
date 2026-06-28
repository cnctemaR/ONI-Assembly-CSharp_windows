using System;

namespace Mono.Data.Tds.Protocol
{
	public sealed class TdsInternalError
	{
		public TdsInternalError(byte theClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
		{
			this.theClass = theClass;
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
				return this.theClass;
			}
			set
			{
				this.theClass = value;
			}
		}

		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
			set
			{
				this.lineNumber = value;
			}
		}

		public string Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
			}
		}

		public int Number
		{
			get
			{
				return this.number;
			}
			set
			{
				this.number = value;
			}
		}

		public string Procedure
		{
			get
			{
				return this.procedure;
			}
			set
			{
				this.procedure = value;
			}
		}

		public string Server
		{
			get
			{
				return this.server;
			}
			set
			{
				this.server = value;
			}
		}

		public string Source
		{
			get
			{
				return this.source;
			}
			set
			{
				this.source = value;
			}
		}

		public byte State
		{
			get
			{
				return this.state;
			}
			set
			{
				this.state = value;
			}
		}

		private byte theClass;

		private int lineNumber;

		private string message;

		private int number;

		private string procedure;

		private string server;

		private string source;

		private byte state;
	}
}
