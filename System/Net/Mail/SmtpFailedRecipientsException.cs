using System;
using System.Runtime.Serialization;

namespace System.Net.Mail
{
	[Serializable]
	public class SmtpFailedRecipientsException : SmtpFailedRecipientException, ISerializable
	{
		public SmtpFailedRecipientsException()
		{
		}

		public SmtpFailedRecipientsException(string message)
			: base(message)
		{
		}

		public SmtpFailedRecipientsException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public SmtpFailedRecipientsException(string message, SmtpFailedRecipientException[] innerExceptions)
			: base(message)
		{
			this.innerExceptions = innerExceptions;
		}

		protected SmtpFailedRecipientsException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.innerExceptions = (SmtpFailedRecipientException[])info.GetValue("innerExceptions", typeof(SmtpFailedRecipientException[]));
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			this.GetObjectData(info, context);
		}

		public SmtpFailedRecipientException[] InnerExceptions
		{
			get
			{
				return this.innerExceptions;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			base.GetObjectData(info, context);
			info.AddValue("innerExceptions", this.innerExceptions);
		}

		private SmtpFailedRecipientException[] innerExceptions;
	}
}
