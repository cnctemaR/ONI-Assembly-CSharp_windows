using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

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

		protected SmtpException(SerializationInfo serializationInfo, StreamingContext streamingContext)
			: base(serializationInfo, streamingContext)
		{
			try
			{
				this.statusCode = (SmtpStatusCode)serializationInfo.GetValue("Status", typeof(int));
			}
			catch (SerializationException)
			{
				this.statusCode = (SmtpStatusCode)serializationInfo.GetValue("statusCode", typeof(SmtpStatusCode));
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

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public override void GetObjectData(SerializationInfo serializationInfo, StreamingContext streamingContext)
		{
			if (serializationInfo == null)
			{
				throw new ArgumentNullException("serializationInfo");
			}
			base.GetObjectData(serializationInfo, streamingContext);
			serializationInfo.AddValue("Status", this.statusCode, typeof(int));
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.GetObjectData(info, context);
		}

		private SmtpStatusCode statusCode;
	}
}
