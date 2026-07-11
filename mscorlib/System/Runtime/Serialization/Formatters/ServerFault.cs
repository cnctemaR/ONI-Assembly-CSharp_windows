using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Metadata;

namespace System.Runtime.Serialization.Formatters
{
	[SoapType]
	[ComVisible(true)]
	[Serializable]
	public sealed class ServerFault
	{
		public ServerFault(string exceptionType, string message, string stackTrace)
		{
			this.exceptionType = exceptionType;
			this.message = message;
			this.stackTrace = stackTrace;
		}

		public string ExceptionType
		{
			get
			{
				return this.exceptionType;
			}
			set
			{
				this.exceptionType = value;
			}
		}

		public string ExceptionMessage
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

		public string StackTrace
		{
			get
			{
				return this.stackTrace;
			}
			set
			{
				this.stackTrace = value;
			}
		}

		private string exceptionType;

		private string message;

		private string stackTrace;

		private Exception exception;
	}
}
