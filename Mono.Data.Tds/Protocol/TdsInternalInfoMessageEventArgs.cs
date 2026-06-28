using System;

namespace Mono.Data.Tds.Protocol
{
	public class TdsInternalInfoMessageEventArgs : EventArgs
	{
		public TdsInternalInfoMessageEventArgs(TdsInternalErrorCollection errors)
		{
			this.errors = errors;
		}

		public TdsInternalInfoMessageEventArgs(TdsInternalError error)
		{
			this.errors = new TdsInternalErrorCollection();
			this.errors.Add(error);
		}

		public TdsInternalErrorCollection Errors
		{
			get
			{
				return this.errors;
			}
		}

		public byte Class
		{
			get
			{
				return this.errors[0].Class;
			}
		}

		public int LineNumber
		{
			get
			{
				return this.errors[0].LineNumber;
			}
		}

		public string Message
		{
			get
			{
				return this.errors[0].Message;
			}
		}

		public int Number
		{
			get
			{
				return this.errors[0].Number;
			}
		}

		public string Procedure
		{
			get
			{
				return this.errors[0].Procedure;
			}
		}

		public string Server
		{
			get
			{
				return this.errors[0].Server;
			}
		}

		public string Source
		{
			get
			{
				return this.errors[0].Source;
			}
		}

		public byte State
		{
			get
			{
				return this.errors[0].State;
			}
		}

		public int Add(byte theClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
		{
			return this.errors.Add(new TdsInternalError(theClass, lineNumber, message, number, procedure, server, source, state));
		}

		private TdsInternalErrorCollection errors;
	}
}
