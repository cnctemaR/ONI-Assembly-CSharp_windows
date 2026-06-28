using System;
using System.IO;
using System.Runtime.InteropServices;

namespace FileHelpers
{
	[Serializable]
	internal sealed class InternalStringReader : TextReader
	{
		public InternalStringReader(string s)
		{
			if (s == null)
			{
				throw new ArgumentNullException("s");
			}
			this.mS = s;
			this.Length = s.Length;
		}

		internal int Length { get; private set; }

		internal int Position { get; private set; }

		public override void Close()
		{
			this.Dispose(true);
		}

		protected override void Dispose(bool disposing)
		{
			this.mS = null;
			this.Position = 0;
			this.Length = 0;
			base.Dispose(disposing);
		}

		public override int Peek()
		{
			if (this.mS == null)
			{
				throw new ObjectDisposedException(null, "The Reader is Closed");
			}
			if (this.Position == this.Length)
			{
				return -1;
			}
			return (int)this.mS[this.Position];
		}

		public override int Read()
		{
			if (this.mS == null)
			{
				throw new ObjectDisposedException(null, "The Reader is Closed");
			}
			if (this.Position == this.Length)
			{
				return -1;
			}
			return (int)this.mS[++this.Position];
		}

		public override int Read([In] [Out] char[] buffer, int index, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (buffer.Length - index < count)
			{
				throw new ArgumentException("offset");
			}
			if (this.mS == null)
			{
				throw new ObjectDisposedException(null, "The Reader is Closed");
			}
			int num = this.Length - this.Position;
			if (num > 0)
			{
				if (num > count)
				{
					num = count;
				}
				this.mS.CopyTo(this.Position, buffer, index, num);
				this.Position += num;
			}
			return num;
		}

		public override string ReadLine()
		{
			if (this.mS == null)
			{
				throw new ObjectDisposedException(null, "The Reader is Closed");
			}
			int i;
			for (i = this.Position; i < this.Length; i++)
			{
				char c = this.mS[i];
				char c2 = c;
				if (c2 == '\n' || c2 == '\r')
				{
					string text = this.mS.Substring(this.Position, i - this.Position);
					this.Position = i + 1;
					if (c == '\r' && this.Position < this.Length && this.mS[this.Position] == '\n')
					{
						this.Position++;
					}
					return text;
				}
			}
			if (i > this.Position)
			{
				string text2 = this.mS.Substring(this.Position, i - this.Position);
				this.Position = i;
				return text2;
			}
			return null;
		}

		public override string ReadToEnd()
		{
			if (this.mS == null)
			{
				throw new ObjectDisposedException(null, "The Reader is Closed");
			}
			string text;
			if (this.Position == 0)
			{
				text = this.mS;
			}
			else
			{
				text = this.mS.Substring(this.Position, this.Length - this.Position);
			}
			this.Position = this.Length;
			return text;
		}

		private string mS;
	}
}
