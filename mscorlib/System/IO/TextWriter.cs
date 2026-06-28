using System;
using System.Runtime.InteropServices;
using System.Text;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public abstract class TextWriter : MarshalByRefObject, IDisposable
	{
		protected TextWriter()
		{
			this.CoreNewLine = Environment.NewLine.ToCharArray();
		}

		protected TextWriter(IFormatProvider formatProvider)
		{
			this.CoreNewLine = Environment.NewLine.ToCharArray();
			this.internalFormatProvider = formatProvider;
		}

		public abstract Encoding Encoding { get; }

		public virtual IFormatProvider FormatProvider
		{
			get
			{
				return this.internalFormatProvider;
			}
		}

		public virtual string NewLine
		{
			get
			{
				return new string(this.CoreNewLine);
			}
			set
			{
				if (value == null)
				{
					value = Environment.NewLine;
				}
				this.CoreNewLine = value.ToCharArray();
			}
		}

		public virtual void Close()
		{
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				GC.SuppressFinalize(this);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public virtual void Flush()
		{
		}

		public static TextWriter Synchronized(TextWriter writer)
		{
			return TextWriter.Synchronized(writer, false);
		}

		internal static TextWriter Synchronized(TextWriter writer, bool neverClose)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer is null");
			}
			if (writer is SynchronizedWriter)
			{
				return writer;
			}
			return new SynchronizedWriter(writer, neverClose);
		}

		public virtual void Write(bool value)
		{
			this.Write(value.ToString());
		}

		public virtual void Write(char value)
		{
		}

		public virtual void Write(char[] buffer)
		{
			if (buffer == null)
			{
				return;
			}
			this.Write(buffer, 0, buffer.Length);
		}

		public virtual void Write(decimal value)
		{
			this.Write(value.ToString(this.internalFormatProvider));
		}

		public virtual void Write(double value)
		{
			this.Write(value.ToString(this.internalFormatProvider));
		}

		public virtual void Write(int value)
		{
			this.Write(value.ToString(this.internalFormatProvider));
		}

		public virtual void Write(long value)
		{
			this.Write(value.ToString(this.internalFormatProvider));
		}

		public virtual void Write(object value)
		{
			if (value != null)
			{
				this.Write(value.ToString());
			}
		}

		public virtual void Write(float value)
		{
			this.Write(value.ToString(this.internalFormatProvider));
		}

		public virtual void Write(string value)
		{
			if (value != null)
			{
				this.Write(value.ToCharArray());
			}
		}

		[CLSCompliant(false)]
		public virtual void Write(uint value)
		{
			this.Write(value.ToString(this.internalFormatProvider));
		}

		[CLSCompliant(false)]
		public virtual void Write(ulong value)
		{
			this.Write(value.ToString(this.internalFormatProvider));
		}

		public virtual void Write(string format, object arg0)
		{
			this.Write(string.Format(format, arg0));
		}

		public virtual void Write(string format, params object[] arg)
		{
			this.Write(string.Format(format, arg));
		}

		public virtual void Write(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0 || index > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0 || index > buffer.Length - count)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			while (count > 0)
			{
				this.Write(buffer[index]);
				count--;
				index++;
			}
		}

		public virtual void Write(string format, object arg0, object arg1)
		{
			this.Write(string.Format(format, arg0, arg1));
		}

		public virtual void Write(string format, object arg0, object arg1, object arg2)
		{
			this.Write(string.Format(format, arg0, arg1, arg2));
		}

		public virtual void WriteLine()
		{
			this.Write(this.CoreNewLine);
		}

		public virtual void WriteLine(bool value)
		{
			this.Write(value);
			this.WriteLine();
		}

		public virtual void WriteLine(char value)
		{
			this.Write(value);
			this.WriteLine();
		}

		public virtual void WriteLine(char[] buffer)
		{
			this.Write(buffer);
			this.WriteLine();
		}

		public virtual void WriteLine(decimal value)
		{
			this.Write(value);
			this.WriteLine();
		}

		public virtual void WriteLine(double value)
		{
			this.Write(value);
			this.WriteLine();
		}

		public virtual void WriteLine(int value)
		{
			this.Write(value);
			this.WriteLine();
		}

		public virtual void WriteLine(long value)
		{
			this.Write(value);
			this.WriteLine();
		}

		public virtual void WriteLine(object value)
		{
			this.Write(value);
			this.WriteLine();
		}

		public virtual void WriteLine(float value)
		{
			this.Write(value);
			this.WriteLine();
		}

		public virtual void WriteLine(string value)
		{
			this.Write(value);
			this.WriteLine();
		}

		[CLSCompliant(false)]
		public virtual void WriteLine(uint value)
		{
			this.Write(value);
			this.WriteLine();
		}

		[CLSCompliant(false)]
		public virtual void WriteLine(ulong value)
		{
			this.Write(value);
			this.WriteLine();
		}

		public virtual void WriteLine(string format, object arg0)
		{
			this.Write(format, arg0);
			this.WriteLine();
		}

		public virtual void WriteLine(string format, params object[] arg)
		{
			this.Write(format, arg);
			this.WriteLine();
		}

		public virtual void WriteLine(char[] buffer, int index, int count)
		{
			this.Write(buffer, index, count);
			this.WriteLine();
		}

		public virtual void WriteLine(string format, object arg0, object arg1)
		{
			this.Write(format, arg0, arg1);
			this.WriteLine();
		}

		public virtual void WriteLine(string format, object arg0, object arg1, object arg2)
		{
			this.Write(format, arg0, arg1, arg2);
			this.WriteLine();
		}

		protected char[] CoreNewLine;

		internal IFormatProvider internalFormatProvider;

		public static readonly TextWriter Null = new TextWriter.NullTextWriter();

		private sealed class NullTextWriter : TextWriter
		{
			public override Encoding Encoding
			{
				get
				{
					return Encoding.Default;
				}
			}

			public override void Write(string s)
			{
			}

			public override void Write(char value)
			{
			}

			public override void Write(char[] value, int index, int count)
			{
			}
		}
	}
}
