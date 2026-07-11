using System;

namespace System.IO
{
	[Serializable]
	internal class SynchronizedReader : TextReader
	{
		public SynchronizedReader(TextReader reader)
		{
			this.reader = reader;
		}

		public override void Close()
		{
			lock (this)
			{
				this.reader.Close();
			}
		}

		public override int Peek()
		{
			int num;
			lock (this)
			{
				num = this.reader.Peek();
			}
			return num;
		}

		public override int ReadBlock(char[] buffer, int index, int count)
		{
			int num;
			lock (this)
			{
				num = this.reader.ReadBlock(buffer, index, count);
			}
			return num;
		}

		public override string ReadLine()
		{
			string text;
			lock (this)
			{
				text = this.reader.ReadLine();
			}
			return text;
		}

		public override string ReadToEnd()
		{
			string text;
			lock (this)
			{
				text = this.reader.ReadToEnd();
			}
			return text;
		}

		public override int Read()
		{
			int num;
			lock (this)
			{
				num = this.reader.Read();
			}
			return num;
		}

		public override int Read(char[] buffer, int index, int count)
		{
			int num;
			lock (this)
			{
				num = this.reader.Read(buffer, index, count);
			}
			return num;
		}

		private TextReader reader;
	}
}
