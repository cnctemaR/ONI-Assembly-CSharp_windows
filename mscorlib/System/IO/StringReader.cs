using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public class StringReader : TextReader
	{
		public StringReader(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			this._s = s;
			this._length = ((s == null) ? 0 : s.Length);
		}

		public override void Close()
		{
			this.Dispose(true);
		}

		protected override void Dispose(bool disposing)
		{
			this._s = null;
			this._pos = 0;
			this._length = 0;
			base.Dispose(disposing);
		}

		public override int Peek()
		{
			if (this._s == null)
			{
				__Error.ReaderClosed();
			}
			if (this._pos == this._length)
			{
				return -1;
			}
			return (int)this._s[this._pos];
		}

		public override int Read()
		{
			if (this._s == null)
			{
				__Error.ReaderClosed();
			}
			if (this._pos == this._length)
			{
				return -1;
			}
			string s = this._s;
			int pos = this._pos;
			this._pos = pos + 1;
			return (int)s[pos];
		}

		public override int Read([In] [Out] char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("Non-negative number required."));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			if (this._s == null)
			{
				__Error.ReaderClosed();
			}
			int num = this._length - this._pos;
			if (num > 0)
			{
				if (num > count)
				{
					num = count;
				}
				this._s.CopyTo(this._pos, buffer, index, num);
				this._pos += num;
			}
			return num;
		}

		public override string ReadToEnd()
		{
			if (this._s == null)
			{
				__Error.ReaderClosed();
			}
			string text;
			if (this._pos == 0)
			{
				text = this._s;
			}
			else
			{
				text = this._s.Substring(this._pos, this._length - this._pos);
			}
			this._pos = this._length;
			return text;
		}

		public override string ReadLine()
		{
			if (this._s == null)
			{
				__Error.ReaderClosed();
			}
			int i;
			for (i = this._pos; i < this._length; i++)
			{
				char c = this._s[i];
				if (c == '\r' || c == '\n')
				{
					string text = this._s.Substring(this._pos, i - this._pos);
					this._pos = i + 1;
					if (c == '\r' && this._pos < this._length && this._s[this._pos] == '\n')
					{
						this._pos++;
					}
					return text;
				}
			}
			if (i > this._pos)
			{
				string text2 = this._s.Substring(this._pos, i - this._pos);
				this._pos = i;
				return text2;
			}
			return null;
		}

		[ComVisible(false)]
		public override Task<string> ReadLineAsync()
		{
			return Task.FromResult<string>(this.ReadLine());
		}

		[ComVisible(false)]
		public override Task<string> ReadToEndAsync()
		{
			return Task.FromResult<string>(this.ReadToEnd());
		}

		[ComVisible(false)]
		public override Task<int> ReadBlockAsync(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			if (index < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			return Task.FromResult<int>(this.ReadBlock(buffer, index, count));
		}

		[ComVisible(false)]
		public override Task<int> ReadAsync(char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer", Environment.GetResourceString("Buffer cannot be null."));
			}
			if (index < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException((index < 0) ? "index" : "count", Environment.GetResourceString("Non-negative number required."));
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection."));
			}
			return Task.FromResult<int>(this.Read(buffer, index, count));
		}

		private string _s;

		private int _pos;

		private int _length;
	}
}
