using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.IO
{
	[MonoTODO("Serialization format not compatible with .NET")]
	[ComVisible(true)]
	[Serializable]
	public class StringWriter : TextWriter
	{
		public StringWriter()
			: this(new StringBuilder())
		{
		}

		public StringWriter(IFormatProvider formatProvider)
			: this(new StringBuilder(), formatProvider)
		{
		}

		public StringWriter(StringBuilder sb)
			: this(sb, null)
		{
		}

		public StringWriter(StringBuilder sb, IFormatProvider formatProvider)
		{
			if (sb == null)
			{
				throw new ArgumentNullException("sb");
			}
			this.internalString = sb;
			this.internalFormatProvider = formatProvider;
		}

		public override Encoding Encoding
		{
			get
			{
				return Encoding.Unicode;
			}
		}

		public override void Close()
		{
			this.Dispose(true);
			this.disposed = true;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			this.disposed = true;
		}

		public virtual StringBuilder GetStringBuilder()
		{
			return this.internalString;
		}

		public override string ToString()
		{
			return this.internalString.ToString();
		}

		public override void Write(char value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("StringReader", Locale.GetText("Cannot write to a closed StringWriter"));
			}
			this.internalString.Append(value);
		}

		public override void Write(string value)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("StringReader", Locale.GetText("Cannot write to a closed StringWriter"));
			}
			this.internalString.Append(value);
		}

		public override void Write(char[] buffer, int index, int count)
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("StringReader", Locale.GetText("Cannot write to a closed StringWriter"));
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", "< 0");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", "< 0");
			}
			if (index > buffer.Length - count)
			{
				throw new ArgumentException("index + count > buffer.Length");
			}
			this.internalString.Append(buffer, index, count);
		}

		private StringBuilder internalString;

		private bool disposed;
	}
}
