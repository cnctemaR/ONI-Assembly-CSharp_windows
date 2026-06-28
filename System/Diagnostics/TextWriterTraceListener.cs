using System;
using System.IO;

namespace System.Diagnostics
{
	public class TextWriterTraceListener : TraceListener
	{
		public TextWriterTraceListener()
			: base("TextWriter")
		{
		}

		public TextWriterTraceListener(Stream stream)
			: this(stream, string.Empty)
		{
		}

		public TextWriterTraceListener(string fileName)
			: this(fileName, string.Empty)
		{
		}

		public TextWriterTraceListener(TextWriter writer)
			: this(writer, string.Empty)
		{
		}

		public TextWriterTraceListener(Stream stream, string name)
			: base((name == null) ? string.Empty : name)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.writer = new StreamWriter(stream);
		}

		public TextWriterTraceListener(string fileName, string name)
			: base((name == null) ? string.Empty : name)
		{
			if (fileName == null)
			{
				throw new ArgumentNullException("fileName");
			}
			this.writer = new StreamWriter(new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite));
		}

		public TextWriterTraceListener(TextWriter writer, string name)
			: base((name == null) ? string.Empty : name)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			this.writer = writer;
		}

		public TextWriter Writer
		{
			get
			{
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
				this.writer.Flush();
				this.writer.Close();
				this.writer = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Close();
			}
			base.Dispose(disposing);
		}

		public override void Flush()
		{
			if (this.writer != null)
			{
				this.writer.Flush();
			}
		}

		public override void Write(string message)
		{
			if (this.writer != null)
			{
				if (base.NeedIndent)
				{
					this.WriteIndent();
				}
				this.writer.Write(message);
			}
		}

		public override void WriteLine(string message)
		{
			if (this.writer != null)
			{
				if (base.NeedIndent)
				{
					this.WriteIndent();
				}
				this.writer.WriteLine(message);
				base.NeedIndent = true;
			}
		}

		private TextWriter writer;
	}
}
