using System;
using System.Runtime.Serialization;

namespace System.Net.Mail
{
	[Serializable]
	public class SmtpException : Exception, ISerializable
	{
		public SmtpException()
			: this(SmtpStatusCode.GeneralFailure)
		{
		}

		public SmtpException(SmtpStatusCode statusCode)
			: this(statusCode, "Syntax error, command unrecognized.")
		{
		}

		public SmtpException(string message)
			: this(SmtpStatusCode.GeneralFailure, message)
		{
		}

		protected SmtpException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			try
			{
				this.statusCode = (SmtpStatusCode)((int)info.GetValue("Status", typeof(int)));
			}
			catch (SerializationException)
			{
				this.statusCode = (SmtpStatusCode)((int)info.GetValue("statusCode", typeof(SmtpStatusCode)));
			}
		}

		public SmtpException(SmtpStatusCode statusCode, string message)
			: base(message)
		{
			this.statusCode = statusCode;
		}

		public SmtpException(string message, Exception innerException)
			: base(message, innerException)
		{
			this.statusCode = SmtpStatusCode.GeneralFailure;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.GetObjectData(info, context);
		}

		public SmtpStatusCode StatusCode
		{
			get
			{
				return this.statusCode;
			}
			set
			{
				this.statusCode = value;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			info.AddValue("Status", this.statusCode, typeof(int));
		}

		private SmtpStatusCode statusCode;
	}
}
