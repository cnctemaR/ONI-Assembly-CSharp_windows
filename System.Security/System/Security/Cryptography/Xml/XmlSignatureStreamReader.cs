using System;
using System.IO;
using System.Runtime.InteropServices;

namespace System.Security.Cryptography.Xml
{
	internal class XmlSignatureStreamReader : TextReader
	{
		public XmlSignatureStreamReader(TextReader input)
		{
			this.source = input;
		}

		public override void Close()
		{
			this.source.Close();
		}

		public override int Peek()
		{
			if (this.source.Peek() == -1)
			{
				return -1;
			}
			if (this.cache != -2147483648)
			{
				return this.cache;
			}
			this.cache = this.source.Read();
			if (this.cache != 13)
			{
				return this.cache;
			}
			if (this.source.Peek() != 10)
			{
				return 13;
			}
			this.cache = int.MinValue;
			return 10;
		}

		public override int Read()
		{
			if (this.cache != -2147483648)
			{
				int num = this.cache;
				this.cache = int.MinValue;
				return num;
			}
			int num2 = this.source.Read();
			if (num2 != 13)
			{
				return num2;
			}
			this.cache = this.source.Read();
			if (this.cache != 10)
			{
				return 13;
			}
			this.cache = int.MinValue;
			return 10;
		}

		public override int ReadBlock([In] [Out] char[] buffer, int index, int count)
		{
			char[] array = new char[count];
			this.source.ReadBlock(array, 0, count);
			int i = index;
			int j = 0;
			while (j < count)
			{
				if (array[j] == '\r')
				{
					if (++j < array.Length && array[j] == '\n')
					{
						buffer[i] = array[j++];
					}
					else
					{
						buffer[i] = '\r';
					}
				}
				else
				{
					buffer[i] = array[j];
				}
				i++;
			}
			while (i < count)
			{
				int num = this.Read();
				if (num < 0)
				{
					break;
				}
				buffer[i++] = (char)num;
			}
			return i;
		}

		public override string ReadLine()
		{
			return this.source.ReadLine();
		}

		public override string ReadToEnd()
		{
			return this.source.ReadToEnd().Replace("\r\n", "\n");
		}

		private TextReader source;

		private int cache = int.MinValue;
	}
}
