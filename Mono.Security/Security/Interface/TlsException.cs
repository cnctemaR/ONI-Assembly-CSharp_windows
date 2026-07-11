using System;

namespace Mono.Security.Interface
{
	public sealed class TlsException : Exception
	{
		public Alert Alert
		{
			get
			{
				return this.alert;
			}
		}

		public TlsException(Alert alert)
			: this(alert, alert.Description.ToString())
		{
		}

		public TlsException(Alert alert, string message)
			: base(message)
		{
			this.alert = alert;
		}

		public TlsException(AlertLevel level, AlertDescription description)
			: this(new Alert(level, description))
		{
		}

		public TlsException(AlertDescription description)
			: this(new Alert(description))
		{
		}

		public TlsException(AlertDescription description, string message)
			: this(new Alert(description), message)
		{
		}

		public TlsException(AlertDescription description, string format, params object[] args)
			: this(new Alert(description), string.Format(format, args))
		{
		}

		private Alert alert;
	}
}
