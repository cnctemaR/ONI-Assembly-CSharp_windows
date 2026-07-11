using System;
using System.IO;
using System.Security.Permissions;
using System.Text;

namespace System.Diagnostics
{
	[HostProtection(SecurityAction.LinkDemand, Synchronization = true)]
	public class TextWriterTraceListener : TraceListener
	{
		public TextWriterTraceListener()
		{
		}

		public TextWriterTraceListener(Stream stream)
			: this(stream, string.Empty)
		{
		}

		public TextWriterTraceListener(Stream stream, string name)
			: base(name)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.writer = new StreamWriter(stream);
		}

		public TextWriterTraceListener(TextWriter writer)
			: this(writer, string.Empty)
		{
		}

		public TextWriterTraceListener(TextWriter writer, string name)
			: base(name)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
		}

		public TextWriterTraceListener(string fileName)
		{
			this.fileName = fileName;
		}

		public TextWriterTraceListener(string fileName, string name)
			: base(name)
		{
			this.fileName = fileName;
		}

		public TextWriter Writer
		{
			get
			{
				this.EnsureWriter();
				return this.writer;
			}
			set
			{
				this.writer = value;
			}
		}

		public override void Close()
		{
			if (this.writer != null)
			{
				try
				{
					this.writer.Close();
				}
				catch (ObjectDisposedException)
				{
				}
			}
			this.writer = null;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					this.Close();
				}
				else
				{
					if (this.writer != null)
					{
						try
						{
							this.writer.Close();
						}
						catch (ObjectDisposedException)
						{
						}
					}
					this.writer = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		public override void Flush()
		{
			if (!this.EnsureWriter())
			{
				return;
			}
			try
			{
				this.writer.Flush();
			}
			catch (ObjectDisposedException)
			{
			}
		}

		public override void Write(string message)
		{
			if (!this.EnsureWriter())
			{
				return;
			}
			if (base.NeedIndent)
			{
				this.WriteIndent();
			}
			try
			{
				this.writer.Write(message);
			}
			catch (ObjectDisposedException)
			{
			}
		}

		public override void WriteLine(string message)
		{
			if (!this.EnsureWriter())
			{
				return;
			}
			if (base.NeedIndent)
			{
				this.WriteIndent();
			}
			try
			{
				this.writer.WriteLine(message);
				base.NeedIndent = true;
			}
			catch (ObjectDisposedException)
			{
			}
		}

		private static Encoding GetEncodingWithFallback(Encoding encoding)
		{
			Encoding encoding2 = (Encoding)encoding.Clone();
			encoding2.EncoderFallback = EncoderFallback.ReplacementFallback;
			encoding2.DecoderFallback = DecoderFallback.ReplacementFallback;
			return encoding2;
		}

		internal bool EnsureWriter()
		{
			bool flag = true;
			if (this.writer == null)
			{
				flag = false;
				if (this.fileName == null)
				{
					return flag;
				}
				Encoding encodingWithFallback = TextWriterTraceListener.GetEncodingWithFallback(new UTF8Encoding(false));
				string text = Path.GetFullPath(this.fileName);
				string directoryName = Path.GetDirectoryName(text);
				string text2 = Path.GetFileName(text);
				for (int i = 0; i < 2; i++)
				{
					try
					{
						this.writer = new StreamWriter(text, true, encodingWithFallback, 4096);
						flag = true;
						break;
					}
					catch (IOException)
					{
						text2 = Guid.NewGuid().ToString() + text2;
						text = Path.Combine(directoryName, text2);
					}
					catch (UnauthorizedAccessException)
					{
						break;
					}
					catch (Exception)
					{
						break;
					}
				}
				if (!flag)
				{
					this.fileName = null;
				}
			}
			return flag;
		}

		internal TextWriter writer;

		private string fileName;
	}
}
