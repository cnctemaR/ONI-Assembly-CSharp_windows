using System;
using System.Collections.Specialized;
using System.Net.Mime;
using System.Text;

namespace System.Net.Mail
{
	public class MailMessage : IDisposable
	{
		public MailMessage()
		{
			this.to = new MailAddressCollection();
			this.alternateViews = new AlternateViewCollection();
			this.attachments = new AttachmentCollection();
			this.bcc = new MailAddressCollection();
			this.cc = new MailAddressCollection();
			this.replyTo = new MailAddressCollection();
			this.headers = new global::System.Collections.Specialized.NameValueCollection();
			this.headers.Add("MIME-Version", "1.0");
		}

		public MailMessage(MailAddress from, MailAddress to)
			: this()
		{
			if (from == null || to == null)
			{
				throw new ArgumentNullException();
			}
			this.From = from;
			this.to.Add(to);
		}

		public MailMessage(string from, string to)
			: this()
		{
			if (from == null || from == string.Empty)
			{
				throw new ArgumentNullException("from");
			}
			if (to == null || to == string.Empty)
			{
				throw new ArgumentNullException("to");
			}
			this.from = new MailAddress(from);
			foreach (string text in to.Split(new char[] { ',' }))
			{
				this.to.Add(new MailAddress(text.Trim()));
			}
		}

		public MailMessage(string from, string to, string subject, string body)
			: this()
		{
			if (from == null || from == string.Empty)
			{
				throw new ArgumentNullException("from");
			}
			if (to == null || to == string.Empty)
			{
				throw new ArgumentNullException("to");
			}
			this.from = new MailAddress(from);
			foreach (string text in to.Split(new char[] { ',' }))
			{
				this.to.Add(new MailAddress(text.Trim()));
			}
			this.Body = body;
			this.Subject = subject;
		}

		public AlternateViewCollection AlternateViews
		{
			get
			{
				return this.alternateViews;
			}
		}

		public AttachmentCollection Attachments
		{
			get
			{
				return this.attachments;
			}
		}

		public MailAddressCollection Bcc
		{
			get
			{
				return this.bcc;
			}
		}

		public string Body
		{
			get
			{
				return this.body;
			}
			set
			{
				if (value != null && this.bodyEncoding == null)
				{
					this.bodyEncoding = this.GuessEncoding(value) ?? Encoding.ASCII;
				}
				this.body = value;
			}
		}

		internal global::System.Net.Mime.ContentType BodyContentType
		{
			get
			{
				return new global::System.Net.Mime.ContentType((!this.isHtml) ? "text/plain" : "text/html")
				{
					CharSet = (this.BodyEncoding ?? Encoding.ASCII).HeaderName
				};
			}
		}

		internal global::System.Net.Mime.TransferEncoding ContentTransferEncoding
		{
			get
			{
				return global::System.Net.Mime.ContentType.GuessTransferEncoding(this.BodyEncoding);
			}
		}

		public Encoding BodyEncoding
		{
			get
			{
				return this.bodyEncoding;
			}
			set
			{
				this.bodyEncoding = value;
			}
		}

		public MailAddressCollection CC
		{
			get
			{
				return this.cc;
			}
		}

		public DeliveryNotificationOptions DeliveryNotificationOptions
		{
			get
			{
				return this.deliveryNotificationOptions;
			}
			set
			{
				this.deliveryNotificationOptions = value;
			}
		}

		public MailAddress From
		{
			get
			{
				return this.from;
			}
			set
			{
				this.from = value;
			}
		}

		public global::System.Collections.Specialized.NameValueCollection Headers
		{
			get
			{
				return this.headers;
			}
		}

		public bool IsBodyHtml
		{
			get
			{
				return this.isHtml;
			}
			set
			{
				this.isHtml = value;
			}
		}

		public MailPriority Priority
		{
			get
			{
				return this.priority;
			}
			set
			{
				this.priority = value;
			}
		}

		internal Encoding HeadersEncoding
		{
			get
			{
				return this.headersEncoding;
			}
			set
			{
				this.headersEncoding = value;
			}
		}

		internal MailAddressCollection ReplyToList
		{
			get
			{
				return this.replyTo;
			}
		}

		public MailAddress ReplyTo
		{
			get
			{
				if (this.replyTo.Count == 0)
				{
					return null;
				}
				return this.replyTo[0];
			}
			set
			{
				this.replyTo.Clear();
				this.replyTo.Add(value);
			}
		}

		public MailAddress Sender
		{
			get
			{
				return this.sender;
			}
			set
			{
				this.sender = value;
			}
		}

		public string Subject
		{
			get
			{
				return this.subject;
			}
			set
			{
				if (value != null && this.subjectEncoding == null)
				{
					this.subjectEncoding = this.GuessEncoding(value);
				}
				this.subject = value;
			}
		}

		public Encoding SubjectEncoding
		{
			get
			{
				return this.subjectEncoding;
			}
			set
			{
				this.subjectEncoding = value;
			}
		}

		public MailAddressCollection To
		{
			get
			{
				return this.to;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
		}

		private Encoding GuessEncoding(string s)
		{
			return global::System.Net.Mime.ContentType.GuessEncoding(s);
		}

		private AlternateViewCollection alternateViews;

		private AttachmentCollection attachments;

		private MailAddressCollection bcc;

		private MailAddressCollection replyTo;

		private string body;

		private MailPriority priority;

		private MailAddress sender;

		private DeliveryNotificationOptions deliveryNotificationOptions;

		private MailAddressCollection cc;

		private MailAddress from;

		private global::System.Collections.Specialized.NameValueCollection headers;

		private MailAddressCollection to;

		private string subject;

		private Encoding subjectEncoding;

		private Encoding bodyEncoding;

		private Encoding headersEncoding = Encoding.UTF8;

		private bool isHtml;
	}
}
