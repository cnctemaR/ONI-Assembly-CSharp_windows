using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net.Configuration;
using System.Net.Mime;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace System.Net.Mail
{
	public class SmtpClient
	{
		public SmtpClient()
			: this(null, 0)
		{
		}

		public SmtpClient(string host)
			: this(host, 0)
		{
		}

		public SmtpClient(string host, int port)
		{
			global::System.Net.Configuration.SmtpSection smtpSection = (global::System.Net.Configuration.SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
			if (smtpSection != null)
			{
				this.host = smtpSection.Network.Host;
				this.port = smtpSection.Network.Port;
				if (smtpSection.Network.UserName != null)
				{
					string text = string.Empty;
					if (smtpSection.Network.Password != null)
					{
						text = smtpSection.Network.Password;
					}
					this.Credentials = new CCredentialsByHost(smtpSection.Network.UserName, text);
				}
				if (smtpSection.From != null)
				{
					this.defaultFrom = new MailAddress(smtpSection.From);
				}
			}
			if (!string.IsNullOrEmpty(host))
			{
				this.host = host;
			}
			if (port != 0)
			{
				this.port = port;
			}
		}

		public event SendCompletedEventHandler SendCompleted;

		[global::System.MonoTODO("Client certificates not used")]
		public global::System.Security.Cryptography.X509Certificates.X509CertificateCollection ClientCertificates
		{
			get
			{
				if (this.clientCertificates == null)
				{
					this.clientCertificates = new global::System.Security.Cryptography.X509Certificates.X509CertificateCollection();
				}
				return this.clientCertificates;
			}
		}

		private string TargetName { get; set; }

		public ICredentialsByHost Credentials
		{
			get
			{
				return this.credentials;
			}
			set
			{
				this.CheckState();
				this.credentials = value;
			}
		}

		public SmtpDeliveryMethod DeliveryMethod
		{
			get
			{
				return this.deliveryMethod;
			}
			set
			{
				this.CheckState();
				this.deliveryMethod = value;
			}
		}

		public bool EnableSsl
		{
			get
			{
				return this.enableSsl;
			}
			set
			{
				this.CheckState();
				this.enableSsl = value;
			}
		}

		public string Host
		{
			get
			{
				return this.host;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				if (value.Length == 0)
				{
					throw new ArgumentException("An empty string is not allowed.", "value");
				}
				this.CheckState();
				this.host = value;
			}
		}

		public string PickupDirectoryLocation
		{
			get
			{
				return this.pickupDirectoryLocation;
			}
			set
			{
				this.pickupDirectoryLocation = value;
			}
		}

		public int Port
		{
			get
			{
				return this.port;
			}
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.CheckState();
				this.port = value;
			}
		}

		[global::System.MonoTODO]
		public ServicePoint ServicePoint
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int Timeout
		{
			get
			{
				return this.timeout;
			}
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.CheckState();
				this.timeout = value;
			}
		}

		public bool UseDefaultCredentials
		{
			get
			{
				return false;
			}
			[global::System.MonoNotSupported("no DefaultCredential support in Mono")]
			set
			{
				if (value)
				{
					throw new NotImplementedException("Default credentials are not supported");
				}
				this.CheckState();
			}
		}

		private void CheckState()
		{
			if (this.messageInProcess != null)
			{
				throw new InvalidOperationException("Cannot set Timeout while Sending a message");
			}
		}

		private static string EncodeAddress(MailAddress address)
		{
			string text = global::System.Net.Mime.ContentType.EncodeSubjectRFC2047(address.DisplayName, Encoding.UTF8);
			return string.Concat(new string[] { "\"", text, "\" <", address.Address, ">" });
		}

		private static string EncodeAddresses(MailAddressCollection addresses)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = true;
			foreach (MailAddress mailAddress in addresses)
			{
				if (!flag)
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(SmtpClient.EncodeAddress(mailAddress));
				flag = false;
			}
			return stringBuilder.ToString();
		}

		private string EncodeSubjectRFC2047(MailMessage message)
		{
			return global::System.Net.Mime.ContentType.EncodeSubjectRFC2047(message.Subject, message.SubjectEncoding);
		}

		private string EncodeBody(MailMessage message)
		{
			string body = message.Body;
			Encoding bodyEncoding = message.BodyEncoding;
			global::System.Net.Mime.TransferEncoding contentTransferEncoding = message.ContentTransferEncoding;
			if (contentTransferEncoding == global::System.Net.Mime.TransferEncoding.Base64)
			{
				return Convert.ToBase64String(bodyEncoding.GetBytes(body), Base64FormattingOptions.InsertLineBreaks);
			}
			if (contentTransferEncoding != global::System.Net.Mime.TransferEncoding.SevenBit)
			{
				return this.ToQuotedPrintable(body, bodyEncoding);
			}
			return body;
		}

		private string EncodeBody(AlternateView av)
		{
			byte[] array = new byte[av.ContentStream.Length];
			av.ContentStream.Read(array, 0, array.Length);
			global::System.Net.Mime.TransferEncoding transferEncoding = av.TransferEncoding;
			if (transferEncoding == global::System.Net.Mime.TransferEncoding.Base64)
			{
				return Convert.ToBase64String(array, Base64FormattingOptions.InsertLineBreaks);
			}
			if (transferEncoding != global::System.Net.Mime.TransferEncoding.SevenBit)
			{
				return this.ToQuotedPrintable(array);
			}
			return Encoding.ASCII.GetString(array);
		}

		private void EndSection(string section)
		{
			this.SendData(string.Format("--{0}--", section));
			this.SendData(string.Empty);
		}

		private string GenerateBoundary()
		{
			string text = SmtpClient.GenerateBoundary(this.boundaryIndex);
			this.boundaryIndex++;
			return text;
		}

		private static string GenerateBoundary(int index)
		{
			return string.Format("--boundary_{0}_{1}", index, Guid.NewGuid().ToString("D"));
		}

		private bool IsError(SmtpClient.SmtpResponse status)
		{
			return status.StatusCode >= (SmtpStatusCode)400;
		}

		protected void OnSendCompleted(global::System.ComponentModel.AsyncCompletedEventArgs e)
		{
			try
			{
				if (this.SendCompleted != null)
				{
					this.SendCompleted(this, e);
				}
			}
			finally
			{
				this.worker = null;
				this.user_async_state = null;
			}
		}

		private void CheckCancellation()
		{
			if (this.worker != null && this.worker.CancellationPending)
			{
				throw new SmtpClient.CancellationException();
			}
		}

		private SmtpClient.SmtpResponse Read()
		{
			byte[] array = new byte[512];
			int num = 0;
			bool flag = false;
			do
			{
				this.CheckCancellation();
				int num2 = this.stream.Read(array, num, array.Length - num);
				if (num2 <= 0)
				{
					break;
				}
				int num3 = num + num2 - 1;
				if (num3 > 4 && (array[num3] == 10 || array[num3] == 13))
				{
					int num4 = num3 - 3;
					while (num4 >= 0 && array[num4] != 10 && array[num4] != 13)
					{
						num4--;
					}
					flag = array[num4 + 4] == 32;
				}
				num += num2;
				if (num == array.Length)
				{
					byte[] array2 = new byte[array.Length * 2];
					Array.Copy(array, 0, array2, 0, array.Length);
					array = array2;
				}
			}
			while (!flag);
			if (num > 0)
			{
				Encoding encoding = new ASCIIEncoding();
				string @string = encoding.GetString(array, 0, num - 1);
				return SmtpClient.SmtpResponse.Parse(@string);
			}
			throw new IOException("Connection closed");
		}

		private void ResetExtensions()
		{
			this.authMechs = SmtpClient.AuthMechs.None;
		}

		private void ParseExtensions(string extens)
		{
			char[] array = new char[] { ' ' };
			string[] array2 = extens.Split(new char[] { '\n' });
			foreach (string text in array2)
			{
				if (text.Length >= 4)
				{
					string text2 = text.Substring(4);
					if (text2.StartsWith("AUTH ", StringComparison.Ordinal))
					{
						string[] array4 = text2.Split(array);
						for (int j = 1; j < array4.Length; j++)
						{
							string text3 = array4[j].Trim();
							string text4 = text3;
							switch (text4)
							{
							case "CRAM-MD5":
								this.authMechs |= SmtpClient.AuthMechs.CramMD5;
								break;
							case "DIGEST-MD5":
								this.authMechs |= SmtpClient.AuthMechs.DigestMD5;
								break;
							case "GSSAPI":
								this.authMechs |= SmtpClient.AuthMechs.GssAPI;
								break;
							case "KERBEROS_V4":
								this.authMechs |= SmtpClient.AuthMechs.Kerberos4;
								break;
							case "LOGIN":
								this.authMechs |= SmtpClient.AuthMechs.Login;
								break;
							case "PLAIN":
								this.authMechs |= SmtpClient.AuthMechs.Plain;
								break;
							}
						}
					}
				}
			}
		}

		public void Send(MailMessage message)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (this.deliveryMethod == SmtpDeliveryMethod.Network && (this.Host == null || this.Host.Trim().Length == 0))
			{
				throw new InvalidOperationException("The SMTP host was not specified");
			}
			if (this.deliveryMethod == SmtpDeliveryMethod.PickupDirectoryFromIis)
			{
				throw new NotSupportedException("IIS delivery is not supported");
			}
			if (this.port == 0)
			{
				this.port = 25;
			}
			this.mutex.WaitOne();
			try
			{
				this.messageInProcess = message;
				if (this.deliveryMethod == SmtpDeliveryMethod.SpecifiedPickupDirectory)
				{
					this.SendToFile(message);
				}
				else
				{
					this.SendInternal(message);
				}
			}
			catch (SmtpClient.CancellationException)
			{
			}
			catch (SmtpException)
			{
				throw;
			}
			catch (Exception ex)
			{
				throw new SmtpException("Message could not be sent.", ex);
			}
			finally
			{
				this.mutex.ReleaseMutex();
				this.messageInProcess = null;
			}
		}

		private void SendInternal(MailMessage message)
		{
			this.CheckCancellation();
			try
			{
				this.client = new global::System.Net.Sockets.TcpClient(this.host, this.port);
				this.stream = this.client.GetStream();
				this.writer = new StreamWriter(this.stream);
				this.reader = new StreamReader(this.stream);
				this.SendCore(message);
			}
			finally
			{
				if (this.writer != null)
				{
					this.writer.Close();
				}
				if (this.reader != null)
				{
					this.reader.Close();
				}
				if (this.stream != null)
				{
					this.stream.Close();
				}
				if (this.client != null)
				{
					this.client.Close();
				}
			}
		}

		private void SendToFile(MailMessage message)
		{
			if (!Path.IsPathRooted(this.pickupDirectoryLocation))
			{
				throw new SmtpException("Only absolute directories are allowed for pickup directory.");
			}
			string text = Path.Combine(this.pickupDirectoryLocation, Guid.NewGuid() + ".eml");
			try
			{
				this.writer = new StreamWriter(text);
				MailAddress from = message.From;
				if (from == null)
				{
					from = this.defaultFrom;
				}
				this.SendHeader("Date", DateTime.Now.ToString("ddd, dd MMM yyyy HH':'mm':'ss zzz", DateTimeFormatInfo.InvariantInfo));
				this.SendHeader("From", from.ToString());
				this.SendHeader("To", message.To.ToString());
				if (message.CC.Count > 0)
				{
					this.SendHeader("Cc", message.CC.ToString());
				}
				this.SendHeader("Subject", this.EncodeSubjectRFC2047(message));
				foreach (string text2 in message.Headers.AllKeys)
				{
					this.SendHeader(text2, message.Headers[text2]);
				}
				this.AddPriorityHeader(message);
				this.boundaryIndex = 0;
				if (message.Attachments.Count > 0)
				{
					this.SendWithAttachments(message);
				}
				else
				{
					this.SendWithoutAttachments(message, null, false);
				}
			}
			finally
			{
				if (this.writer != null)
				{
					this.writer.Close();
				}
				this.writer = null;
			}
		}

		private void SendCore(MailMessage message)
		{
			SmtpClient.SmtpResponse smtpResponse = this.Read();
			if (this.IsError(smtpResponse))
			{
				throw new SmtpException(smtpResponse.StatusCode, smtpResponse.Description);
			}
			smtpResponse = this.SendCommand("EHLO " + Dns.GetHostName());
			if (this.IsError(smtpResponse))
			{
				smtpResponse = this.SendCommand("HELO " + Dns.GetHostName());
				if (this.IsError(smtpResponse))
				{
					throw new SmtpException(smtpResponse.StatusCode, smtpResponse.Description);
				}
			}
			else
			{
				string description = smtpResponse.Description;
				if (description != null)
				{
					this.ParseExtensions(description);
				}
			}
			if (this.enableSsl)
			{
				this.InitiateSecureConnection();
				this.ResetExtensions();
				this.writer = new StreamWriter(this.stream);
				this.reader = new StreamReader(this.stream);
				smtpResponse = this.SendCommand("EHLO " + Dns.GetHostName());
				if (this.IsError(smtpResponse))
				{
					smtpResponse = this.SendCommand("HELO " + Dns.GetHostName());
					if (this.IsError(smtpResponse))
					{
						throw new SmtpException(smtpResponse.StatusCode, smtpResponse.Description);
					}
				}
				else
				{
					string description2 = smtpResponse.Description;
					if (description2 != null)
					{
						this.ParseExtensions(description2);
					}
				}
			}
			if (this.authMechs != SmtpClient.AuthMechs.None)
			{
				this.Authenticate();
			}
			MailAddress from = message.From;
			if (from == null)
			{
				from = this.defaultFrom;
			}
			smtpResponse = this.SendCommand("MAIL FROM:<" + from.Address + '>');
			if (this.IsError(smtpResponse))
			{
				throw new SmtpException(smtpResponse.StatusCode, smtpResponse.Description);
			}
			List<SmtpFailedRecipientException> list = new List<SmtpFailedRecipientException>();
			for (int i = 0; i < message.To.Count; i++)
			{
				smtpResponse = this.SendCommand("RCPT TO:<" + message.To[i].Address + '>');
				if (this.IsError(smtpResponse))
				{
					list.Add(new SmtpFailedRecipientException(smtpResponse.StatusCode, message.To[i].Address));
				}
			}
			for (int j = 0; j < message.CC.Count; j++)
			{
				smtpResponse = this.SendCommand("RCPT TO:<" + message.CC[j].Address + '>');
				if (this.IsError(smtpResponse))
				{
					list.Add(new SmtpFailedRecipientException(smtpResponse.StatusCode, message.CC[j].Address));
				}
			}
			for (int k = 0; k < message.Bcc.Count; k++)
			{
				smtpResponse = this.SendCommand("RCPT TO:<" + message.Bcc[k].Address + '>');
				if (this.IsError(smtpResponse))
				{
					list.Add(new SmtpFailedRecipientException(smtpResponse.StatusCode, message.Bcc[k].Address));
				}
			}
			if (list.Count > 0)
			{
				throw new SmtpFailedRecipientsException("failed recipients", list.ToArray());
			}
			smtpResponse = this.SendCommand("DATA");
			if (this.IsError(smtpResponse))
			{
				throw new SmtpException(smtpResponse.StatusCode, smtpResponse.Description);
			}
			string text = DateTime.Now.ToString("ddd, dd MMM yyyy HH':'mm':'ss zzz", DateTimeFormatInfo.InvariantInfo);
			text = text.Remove(text.Length - 3, 1);
			this.SendHeader("Date", text);
			this.SendHeader("From", SmtpClient.EncodeAddress(from));
			this.SendHeader("To", SmtpClient.EncodeAddresses(message.To));
			if (message.CC.Count > 0)
			{
				this.SendHeader("Cc", SmtpClient.EncodeAddresses(message.CC));
			}
			this.SendHeader("Subject", this.EncodeSubjectRFC2047(message));
			string text2 = "normal";
			switch (message.Priority)
			{
			case MailPriority.Normal:
				text2 = "normal";
				break;
			case MailPriority.Low:
				text2 = "non-urgent";
				break;
			case MailPriority.High:
				text2 = "urgent";
				break;
			}
			this.SendHeader("Priority", text2);
			if (message.Sender != null)
			{
				this.SendHeader("Sender", SmtpClient.EncodeAddress(message.Sender));
			}
			if (message.ReplyToList.Count > 0)
			{
				this.SendHeader("Reply-To", SmtpClient.EncodeAddresses(message.ReplyToList));
			}
			foreach (string text3 in message.Headers.AllKeys)
			{
				this.SendHeader(text3, message.Headers[text3]);
			}
			this.AddPriorityHeader(message);
			this.boundaryIndex = 0;
			if (message.Attachments.Count > 0)
			{
				this.SendWithAttachments(message);
			}
			else
			{
				this.SendWithoutAttachments(message, null, false);
			}
			this.SendDot();
			smtpResponse = this.Read();
			if (this.IsError(smtpResponse))
			{
				throw new SmtpException(smtpResponse.StatusCode, smtpResponse.Description);
			}
			try
			{
				smtpResponse = this.SendCommand("QUIT");
			}
			catch (IOException)
			{
			}
		}

		public void Send(string from, string to, string subject, string body)
		{
			this.Send(new MailMessage(from, to, subject, body));
		}

		private void SendDot()
		{
			this.writer.Write(".\r\n");
			this.writer.Flush();
		}

		private void SendData(string data)
		{
			if (string.IsNullOrEmpty(data))
			{
				this.writer.Write("\r\n");
				this.writer.Flush();
				return;
			}
			StringReader stringReader = new StringReader(data);
			bool flag = this.deliveryMethod == SmtpDeliveryMethod.Network;
			string text;
			while ((text = stringReader.ReadLine()) != null)
			{
				this.CheckCancellation();
				if (flag)
				{
					int i;
					for (i = 0; i < text.Length; i++)
					{
						if (text[i] != '.')
						{
							break;
						}
					}
					if (i > 0 && i == text.Length)
					{
						text += ".";
					}
				}
				this.writer.Write(text);
				this.writer.Write("\r\n");
			}
			this.writer.Flush();
		}

		public void SendAsync(MailMessage message, object userToken)
		{
			if (this.worker != null)
			{
				throw new InvalidOperationException("Another SendAsync operation is in progress");
			}
			this.worker = new global::System.ComponentModel.BackgroundWorker();
			this.worker.DoWork += delegate(object o, global::System.ComponentModel.DoWorkEventArgs ea)
			{
				try
				{
					this.user_async_state = ea.Argument;
					this.Send(message);
				}
				catch (Exception ex)
				{
					ea.Result = ex;
					throw ex;
				}
			};
			this.worker.WorkerSupportsCancellation = true;
			this.worker.RunWorkerCompleted += delegate(object o, global::System.ComponentModel.RunWorkerCompletedEventArgs ea)
			{
				this.OnSendCompleted(new global::System.ComponentModel.AsyncCompletedEventArgs(ea.Error, ea.Cancelled, this.user_async_state));
			};
			this.worker.RunWorkerAsync(userToken);
		}

		public void SendAsync(string from, string to, string subject, string body, object userToken)
		{
			this.SendAsync(new MailMessage(from, to, subject, body), userToken);
		}

		public void SendAsyncCancel()
		{
			if (this.worker == null)
			{
				throw new InvalidOperationException("SendAsync operation is not in progress");
			}
			this.worker.CancelAsync();
		}

		private void AddPriorityHeader(MailMessage message)
		{
			MailPriority priority = message.Priority;
			if (priority != MailPriority.Low)
			{
				if (priority == MailPriority.High)
				{
					this.SendHeader("Priority", "Urgent");
					this.SendHeader("Importance", "high");
					this.SendHeader("X-Priority", "1");
				}
			}
			else
			{
				this.SendHeader("Priority", "Non-Urgent");
				this.SendHeader("Importance", "low");
				this.SendHeader("X-Priority", "5");
			}
		}

		private void SendSimpleBody(MailMessage message)
		{
			this.SendHeader("Content-Type", message.BodyContentType.ToString());
			if (message.ContentTransferEncoding != global::System.Net.Mime.TransferEncoding.SevenBit)
			{
				this.SendHeader("Content-Transfer-Encoding", SmtpClient.GetTransferEncodingName(message.ContentTransferEncoding));
			}
			this.SendData(string.Empty);
			this.SendData(this.EncodeBody(message));
		}

		private void SendBodylessSingleAlternate(AlternateView av)
		{
			this.SendHeader("Content-Type", av.ContentType.ToString());
			if (av.TransferEncoding != global::System.Net.Mime.TransferEncoding.SevenBit)
			{
				this.SendHeader("Content-Transfer-Encoding", SmtpClient.GetTransferEncodingName(av.TransferEncoding));
			}
			this.SendData(string.Empty);
			this.SendData(this.EncodeBody(av));
		}

		private void SendWithoutAttachments(MailMessage message, string boundary, bool attachmentExists)
		{
			if (message.Body == null && message.AlternateViews.Count == 1)
			{
				this.SendBodylessSingleAlternate(message.AlternateViews[0]);
			}
			else if (message.AlternateViews.Count > 0)
			{
				this.SendBodyWithAlternateViews(message, boundary, attachmentExists);
			}
			else
			{
				this.SendSimpleBody(message);
			}
		}

		private void SendWithAttachments(MailMessage message)
		{
			string text = this.GenerateBoundary();
			this.SendHeader("Content-Type", new global::System.Net.Mime.ContentType
			{
				Boundary = text,
				MediaType = "multipart/mixed",
				CharSet = null
			}.ToString());
			this.SendData(string.Empty);
			Attachment attachment = null;
			if (message.AlternateViews.Count > 0)
			{
				this.SendWithoutAttachments(message, text, true);
			}
			else
			{
				attachment = Attachment.CreateAttachmentFromString(message.Body, null, message.BodyEncoding, (!message.IsBodyHtml) ? "text/plain" : "text/html");
				message.Attachments.Insert(0, attachment);
			}
			try
			{
				this.SendAttachments(message, attachment, text);
			}
			finally
			{
				if (attachment != null)
				{
					message.Attachments.Remove(attachment);
				}
			}
			this.EndSection(text);
		}

		private void SendBodyWithAlternateViews(MailMessage message, string boundary, bool attachmentExists)
		{
			AlternateViewCollection alternateViews = message.AlternateViews;
			string text = this.GenerateBoundary();
			global::System.Net.Mime.ContentType contentType = new global::System.Net.Mime.ContentType();
			contentType.Boundary = text;
			contentType.MediaType = "multipart/alternative";
			if (!attachmentExists)
			{
				this.SendHeader("Content-Type", contentType.ToString());
				this.SendData(string.Empty);
			}
			AlternateView alternateView = null;
			if (message.Body != null)
			{
				alternateView = AlternateView.CreateAlternateViewFromString(message.Body, message.BodyEncoding, (!message.IsBodyHtml) ? "text/plain" : "text/html");
				alternateViews.Insert(0, alternateView);
				this.StartSection(boundary, contentType);
			}
			try
			{
				foreach (AlternateView alternateView2 in alternateViews)
				{
					string text2 = null;
					if (alternateView2.LinkedResources.Count > 0)
					{
						text2 = this.GenerateBoundary();
						global::System.Net.Mime.ContentType contentType2 = new global::System.Net.Mime.ContentType("multipart/related");
						contentType2.Boundary = text2;
						contentType2.Parameters["type"] = alternateView2.ContentType.ToString();
						this.StartSection(text, contentType2);
						this.StartSection(text2, alternateView2.ContentType, alternateView2.TransferEncoding);
					}
					else
					{
						global::System.Net.Mime.ContentType contentType2 = new global::System.Net.Mime.ContentType(alternateView2.ContentType.ToString());
						this.StartSection(text, contentType2, alternateView2.TransferEncoding);
					}
					global::System.Net.Mime.TransferEncoding transferEncoding = alternateView2.TransferEncoding;
					switch (transferEncoding + 1)
					{
					case global::System.Net.Mime.TransferEncoding.QuotedPrintable:
					case (global::System.Net.Mime.TransferEncoding)3:
					{
						byte[] array = new byte[alternateView2.ContentStream.Length];
						alternateView2.ContentStream.Read(array, 0, array.Length);
						this.SendData(Encoding.ASCII.GetString(array));
						break;
					}
					case global::System.Net.Mime.TransferEncoding.Base64:
					{
						byte[] array2 = new byte[alternateView2.ContentStream.Length];
						alternateView2.ContentStream.Read(array2, 0, array2.Length);
						this.SendData(this.ToQuotedPrintable(array2));
						break;
					}
					case global::System.Net.Mime.TransferEncoding.SevenBit:
					{
						byte[] array = new byte[alternateView2.ContentStream.Length];
						alternateView2.ContentStream.Read(array, 0, array.Length);
						this.SendData(Convert.ToBase64String(array, Base64FormattingOptions.InsertLineBreaks));
						break;
					}
					}
					if (alternateView2.LinkedResources.Count > 0)
					{
						this.SendLinkedResources(message, alternateView2.LinkedResources, text2);
						this.EndSection(text2);
					}
					if (!attachmentExists)
					{
						this.SendData(string.Empty);
					}
				}
			}
			finally
			{
				if (alternateView != null)
				{
					alternateViews.Remove(alternateView);
				}
			}
			this.EndSection(text);
		}

		private void SendLinkedResources(MailMessage message, LinkedResourceCollection resources, string boundary)
		{
			foreach (LinkedResource linkedResource in resources)
			{
				this.StartSection(boundary, linkedResource.ContentType, linkedResource.TransferEncoding, linkedResource);
				global::System.Net.Mime.TransferEncoding transferEncoding = linkedResource.TransferEncoding;
				switch (transferEncoding + 1)
				{
				case global::System.Net.Mime.TransferEncoding.QuotedPrintable:
				case (global::System.Net.Mime.TransferEncoding)3:
				{
					byte[] array = new byte[linkedResource.ContentStream.Length];
					linkedResource.ContentStream.Read(array, 0, array.Length);
					this.SendData(Encoding.ASCII.GetString(array));
					break;
				}
				case global::System.Net.Mime.TransferEncoding.Base64:
				{
					byte[] array2 = new byte[linkedResource.ContentStream.Length];
					linkedResource.ContentStream.Read(array2, 0, array2.Length);
					this.SendData(this.ToQuotedPrintable(array2));
					break;
				}
				case global::System.Net.Mime.TransferEncoding.SevenBit:
				{
					byte[] array = new byte[linkedResource.ContentStream.Length];
					linkedResource.ContentStream.Read(array, 0, array.Length);
					this.SendData(Convert.ToBase64String(array, Base64FormattingOptions.InsertLineBreaks));
					break;
				}
				}
			}
		}

		private void SendAttachments(MailMessage message, Attachment body, string boundary)
		{
			foreach (Attachment attachment in message.Attachments)
			{
				global::System.Net.Mime.ContentType contentType = new global::System.Net.Mime.ContentType(attachment.ContentType.ToString());
				if (attachment.Name != null)
				{
					contentType.Name = attachment.Name;
					if (attachment.NameEncoding != null)
					{
						contentType.CharSet = attachment.NameEncoding.HeaderName;
					}
					attachment.ContentDisposition.FileName = attachment.Name;
				}
				this.StartSection(boundary, contentType, attachment.TransferEncoding, (attachment != body) ? attachment.ContentDisposition : null);
				byte[] array = new byte[attachment.ContentStream.Length];
				attachment.ContentStream.Read(array, 0, array.Length);
				global::System.Net.Mime.TransferEncoding transferEncoding = attachment.TransferEncoding;
				switch (transferEncoding + 1)
				{
				case global::System.Net.Mime.TransferEncoding.QuotedPrintable:
				case (global::System.Net.Mime.TransferEncoding)3:
					this.SendData(Encoding.ASCII.GetString(array));
					break;
				case global::System.Net.Mime.TransferEncoding.Base64:
					this.SendData(this.ToQuotedPrintable(array));
					break;
				case global::System.Net.Mime.TransferEncoding.SevenBit:
					this.SendData(Convert.ToBase64String(array, Base64FormattingOptions.InsertLineBreaks));
					break;
				}
				this.SendData(string.Empty);
			}
		}

		private SmtpClient.SmtpResponse SendCommand(string command)
		{
			this.writer.Write(command);
			this.writer.Write("\r\n");
			this.writer.Flush();
			return this.Read();
		}

		private void SendHeader(string name, string value)
		{
			this.SendData(string.Format("{0}: {1}", name, value));
		}

		private void StartSection(string section, global::System.Net.Mime.ContentType sectionContentType)
		{
			this.SendData(string.Format("--{0}", section));
			this.SendHeader("content-type", sectionContentType.ToString());
			this.SendData(string.Empty);
		}

		private void StartSection(string section, global::System.Net.Mime.ContentType sectionContentType, global::System.Net.Mime.TransferEncoding transferEncoding)
		{
			this.SendData(string.Format("--{0}", section));
			this.SendHeader("content-type", sectionContentType.ToString());
			this.SendHeader("content-transfer-encoding", SmtpClient.GetTransferEncodingName(transferEncoding));
			this.SendData(string.Empty);
		}

		private void StartSection(string section, global::System.Net.Mime.ContentType sectionContentType, global::System.Net.Mime.TransferEncoding transferEncoding, LinkedResource lr)
		{
			this.SendData(string.Format("--{0}", section));
			this.SendHeader("content-type", sectionContentType.ToString());
			this.SendHeader("content-transfer-encoding", SmtpClient.GetTransferEncodingName(transferEncoding));
			if (lr.ContentId != null && lr.ContentId.Length > 0)
			{
				this.SendHeader("content-ID", "<" + lr.ContentId + ">");
			}
			this.SendData(string.Empty);
		}

		private void StartSection(string section, global::System.Net.Mime.ContentType sectionContentType, global::System.Net.Mime.TransferEncoding transferEncoding, global::System.Net.Mime.ContentDisposition contentDisposition)
		{
			this.SendData(string.Format("--{0}", section));
			this.SendHeader("content-type", sectionContentType.ToString());
			this.SendHeader("content-transfer-encoding", SmtpClient.GetTransferEncodingName(transferEncoding));
			if (contentDisposition != null)
			{
				this.SendHeader("content-disposition", contentDisposition.ToString());
			}
			this.SendData(string.Empty);
		}

		private string ToQuotedPrintable(string input, Encoding enc)
		{
			byte[] bytes = enc.GetBytes(input);
			return this.ToQuotedPrintable(bytes);
		}

		private string ToQuotedPrintable(byte[] bytes)
		{
			StringWriter stringWriter = new StringWriter();
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder("=", 3);
			byte b = 61;
			char c = '\0';
			int i = 0;
			while (i < bytes.Length)
			{
				byte b2 = bytes[i];
				int num2;
				if (b2 > 127 || b2 == b)
				{
					stringBuilder.Length = 1;
					stringBuilder.Append(Convert.ToString(b2, 16).ToUpperInvariant());
					num2 = 3;
					goto IL_008E;
				}
				c = Convert.ToChar(b2);
				if (c != '\r' && c != '\n')
				{
					num2 = 1;
					goto IL_008E;
				}
				stringWriter.Write(c);
				num = 0;
				IL_00C7:
				i++;
				continue;
				IL_008E:
				num += num2;
				if (num > 75)
				{
					stringWriter.Write("=\r\n");
					num = num2;
				}
				if (num2 == 1)
				{
					stringWriter.Write(c);
					goto IL_00C7;
				}
				stringWriter.Write(stringBuilder.ToString());
				goto IL_00C7;
			}
			return stringWriter.ToString();
		}

		private static string GetTransferEncodingName(global::System.Net.Mime.TransferEncoding encoding)
		{
			switch (encoding)
			{
			case global::System.Net.Mime.TransferEncoding.QuotedPrintable:
				return "quoted-printable";
			case global::System.Net.Mime.TransferEncoding.Base64:
				return "base64";
			case global::System.Net.Mime.TransferEncoding.SevenBit:
				return "7bit";
			default:
				return "unknown";
			}
		}

		private void InitiateSecureConnection()
		{
			SmtpClient.SmtpResponse smtpResponse = this.SendCommand("STARTTLS");
			if (this.IsError(smtpResponse))
			{
				throw new SmtpException(SmtpStatusCode.GeneralFailure, "Server does not support secure connections.");
			}
			global::System.Net.Security.SslStream sslStream = new global::System.Net.Security.SslStream(this.stream, false, this.callback, null);
			this.CheckCancellation();
			sslStream.AuthenticateAsClient(this.Host, this.ClientCertificates, global::System.Security.Authentication.SslProtocols.Default, false);
			this.stream = sslStream;
		}

		private void Authenticate()
		{
			string text;
			string text2;
			if (this.UseDefaultCredentials)
			{
				text = CredentialCache.DefaultCredentials.GetCredential(new global::System.Uri("smtp://" + this.host), "basic").UserName;
				text2 = CredentialCache.DefaultCredentials.GetCredential(new global::System.Uri("smtp://" + this.host), "basic").Password;
			}
			else
			{
				if (this.Credentials == null)
				{
					return;
				}
				text = this.Credentials.GetCredential(this.host, this.port, "smtp").UserName;
				text2 = this.Credentials.GetCredential(this.host, this.port, "smtp").Password;
			}
			this.Authenticate(text, text2);
		}

		private void Authenticate(string Username, string Password)
		{
			SmtpClient.SmtpResponse smtpResponse = this.SendCommand("AUTH LOGIN");
			if (smtpResponse.StatusCode != (SmtpStatusCode)334)
			{
				throw new SmtpException(smtpResponse.StatusCode, smtpResponse.Description);
			}
			smtpResponse = this.SendCommand(Convert.ToBase64String(Encoding.ASCII.GetBytes(Username)));
			if (smtpResponse.StatusCode != (SmtpStatusCode)334)
			{
				throw new SmtpException(smtpResponse.StatusCode, smtpResponse.Description);
			}
			smtpResponse = this.SendCommand(Convert.ToBase64String(Encoding.ASCII.GetBytes(Password)));
			if (this.IsError(smtpResponse))
			{
				throw new SmtpException(smtpResponse.StatusCode, smtpResponse.Description);
			}
		}

		private string host;

		private int port;

		private int timeout = 100000;

		private ICredentialsByHost credentials;

		private string pickupDirectoryLocation;

		private SmtpDeliveryMethod deliveryMethod;

		private bool enableSsl;

		private global::System.Security.Cryptography.X509Certificates.X509CertificateCollection clientCertificates;

		private global::System.Net.Sockets.TcpClient client;

		private Stream stream;

		private StreamWriter writer;

		private StreamReader reader;

		private int boundaryIndex;

		private MailAddress defaultFrom;

		private MailMessage messageInProcess;

		private global::System.ComponentModel.BackgroundWorker worker;

		private object user_async_state;

		private SmtpClient.AuthMechs authMechs;

		private Mutex mutex = new Mutex();

		private global::System.Net.Security.RemoteCertificateValidationCallback callback = delegate(object sender, X509Certificate certificate, global::System.Security.Cryptography.X509Certificates.X509Chain chain, global::System.Net.Security.SslPolicyErrors sslPolicyErrors)
		{
			if (ServicePointManager.ServerCertificateValidationCallback != null)
			{
				return ServicePointManager.ServerCertificateValidationCallback(sender, certificate, chain, sslPolicyErrors);
			}
			if (sslPolicyErrors != global::System.Net.Security.SslPolicyErrors.None)
			{
				throw new InvalidOperationException("SSL authentication error: " + sslPolicyErrors);
			}
			return true;
		};

		[Flags]
		private enum AuthMechs
		{
			None = 0,
			CramMD5 = 1,
			DigestMD5 = 2,
			GssAPI = 4,
			Kerberos4 = 8,
			Login = 16,
			Plain = 32
		}

		private class CancellationException : Exception
		{
		}

		private struct HeaderName
		{
			public const string ContentTransferEncoding = "Content-Transfer-Encoding";

			public const string ContentType = "Content-Type";

			public const string Bcc = "Bcc";

			public const string Cc = "Cc";

			public const string From = "From";

			public const string Subject = "Subject";

			public const string To = "To";

			public const string MimeVersion = "MIME-Version";

			public const string MessageId = "Message-ID";

			public const string Priority = "Priority";

			public const string Importance = "Importance";

			public const string XPriority = "X-Priority";

			public const string Date = "Date";
		}

		private struct SmtpResponse
		{
			public static SmtpClient.SmtpResponse Parse(string line)
			{
				SmtpClient.SmtpResponse smtpResponse = default(SmtpClient.SmtpResponse);
				if (line.Length < 4)
				{
					throw new SmtpException("Response is to short " + line.Length + ".");
				}
				if (line[3] != ' ' && line[3] != '-')
				{
					throw new SmtpException("Response format is wrong.(" + line + ")");
				}
				smtpResponse.StatusCode = (SmtpStatusCode)int.Parse(line.Substring(0, 3));
				smtpResponse.Description = line;
				return smtpResponse;
			}

			public SmtpStatusCode StatusCode;

			public string Description;
		}
	}
}
