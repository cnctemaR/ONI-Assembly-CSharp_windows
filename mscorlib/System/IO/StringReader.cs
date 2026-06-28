using System;
using System.Runtime.InteropServices;

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
			this.source = s;
			this.nextChar = 0;
			this.sourceLength = s.Length;
		}

		public override void Close()
		{
			this.Dispose(true);
		}

		protected override void Dispose(bool disposing)
		{
			this.source = null;
			base.Dispose(disposing);
		}

		public override int Peek()
		{
			this.CheckObjectDisposedException();
			if (this.nextChar >= this.sourceLength)
			{
				return -1;
			}
			return (int)this.source[this.nextChar];
		}

		public override int Read()
		{
			this.CheckObjectDisposedException();
			if (this.nextChar >= this.sourceLength)
			{
				return -1;
			}
			return (int)this.source[this.nextChar++];
		}

		public override int Read([In] [Out] char[] buffer, int index, int count)
		{
			this.CheckObjectDisposedException();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException();
			}
			if (index < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			int num;
			if (this.nextChar > this.sourceLength - count)
			{
				num = this.sourceLength - this.nextChar;
			}
			else
			{
				num = count;
			}
			this.source.CopyTo(this.nextChar, buffer, index, num);
			this.nextChar += num;
			return num;
		}

		public override string ReadLine()
		{
			this.CheckObjectDisposedException();
			int i;
			for (i = this.nextChar; i < this.sourceLength; i++)
			{
				char c = this.source[i];
				if (c == '\r' || c == '\n')
				{
					string text = this.source.Substring(this.nextChar, i - this.nextChar);
					this.nextChar = i + 1;
					if (c == '\r' && this.nextChar < this.sourceLength && this.source[this.nextChar] == '\n')
					{
						this.nextChar++;
					}
					return text;
				}
			}
			if (i > this.nextChar)
			{
				string text2 = this.source.Substring(this.nextChar, i - this.nextChar);
				this.nextChar = i;
				return text2;
			}
			return null;
		}

		public override string ReadToEnd()
		{
			this.CheckObjectDisposedException();
			string text = this.source.Substring(this.nextChar, this.sourceLength - this.nextChar);
			this.nextChar = this.sourceLength;
			return text;
		}

		private void CheckObjectDisposedException()
		{
			if (this.source == null)
			{
				throw new ObjectDisposedException("StringReader", Locale.GetText("Cannot read from a closed StringReader"));
			}
		}

		private string source;

		private int nextChar;

		private int sourceLength;
	}
}
