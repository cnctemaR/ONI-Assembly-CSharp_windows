using System;
using System.IO;

namespace System.Data.SqlClient
{
	internal sealed class SqlXmlTextReader : TextReader, IDisposable
	{
		internal SqlXmlTextReader(SqlDataReader reader)
		{
			this.reader = reader;
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public override void Close()
		{
			this.reader.Close();
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.Close();
					((IDisposable)this.reader).Dispose();
				}
				this.disposed = true;
			}
		}

		private bool GetNextBuffer()
		{
			if (this.eof)
			{
				this.localBuffer = null;
				return false;
			}
			this.position = 0;
			if (this.reader.Read())
			{
				this.localBuffer = this.reader.GetString(0);
			}
			else if (this.reader.NextResult() && this.reader.Read())
			{
				this.localBuffer = this.reader.GetString(0);
			}
			else
			{
				this.eof = true;
				this.localBuffer = "</results>";
			}
			return true;
		}

		public override int Peek()
		{
			if ((this.localBuffer == null || this.localBuffer.Length == 0) && !this.GetNextBuffer())
			{
				return -1;
			}
			if (this.eof && this.position >= this.localBuffer.Length)
			{
				return -1;
			}
			return (int)this.localBuffer[this.position];
		}

		public override int Read()
		{
			int num = this.Peek();
			this.position++;
			if (!this.eof && this.position >= this.localBuffer.Length)
			{
				this.GetNextBuffer();
			}
			return num;
		}

		public override int Read(char[] buffer, int index, int count)
		{
			bool flag = true;
			int num = 0;
			if (this.localBuffer == null)
			{
				flag = this.GetNextBuffer();
			}
			while (flag && count - num > this.localBuffer.Length - this.position)
			{
				this.localBuffer.CopyTo(this.position, buffer, index + num, this.localBuffer.Length);
				num += this.localBuffer.Length;
				flag = this.GetNextBuffer();
			}
			if (flag && num < count)
			{
				this.localBuffer.CopyTo(this.position, buffer, index + num, count - num);
				this.position += count - num;
			}
			return num;
		}

		public override int ReadBlock(char[] buffer, int index, int count)
		{
			return this.Read(buffer, index, count);
		}

		public override string ReadLine()
		{
			bool flag = true;
			if (this.localBuffer == null)
			{
				flag = this.GetNextBuffer();
			}
			if (!flag)
			{
				return null;
			}
			string text = this.localBuffer;
			this.GetNextBuffer();
			return text;
		}

		public override string ReadToEnd()
		{
			string text = string.Empty;
			bool flag = true;
			if (this.localBuffer == null)
			{
				flag = this.GetNextBuffer();
			}
			while (flag)
			{
				text += this.localBuffer;
				flag = this.GetNextBuffer();
			}
			return text;
		}

		private bool disposed;

		private bool eof;

		private SqlDataReader reader;

		private string localBuffer = "<results>";

		private int position;
	}
}
