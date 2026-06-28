using System;
using System.Runtime.Serialization;

namespace Mono.Data.Tds.Protocol
{
	public class TdsInternalException : SystemException
	{
		internal TdsInternalException()
			: base("a TDS Exception has occurred.")
		{
		}

		internal TdsInternalException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		internal TdsInternalException(byte theClass, int lineNumber, string message, int number, string procedure, string server, string source, byte state)
			: base(message)
		{
			this.theClass = theClass;
			this.lineNumber = lineNumber;
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
		}

		public int LineNumber
		{
			get
			{
				return this.lineNumber;
			}
		}

		public override string Message
		{
			get
			{
				return base.Message;
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

		public override string Source
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

		[MonoTODO]
		public override void GetObjectData(SerializationInfo si, StreamingContext context)
		{
			throw new NotImplementedException();
		}

		private byte theClass;

		private int lineNumber;

		private int number;

		private string procedure;

		private string server;

		private string source;

		private byte state;
	}
}
