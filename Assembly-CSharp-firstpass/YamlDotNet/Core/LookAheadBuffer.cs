using System;
using System.IO;

namespace YamlDotNet.Core
{
	[Serializable]
	public class LookAheadBuffer : ILookAheadBuffer
	{
		public LookAheadBuffer(TextReader input, int capacity)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (capacity < 1)
			{
				throw new ArgumentOutOfRangeException("capacity", "The capacity must be positive.");
			}
			this.input = input;
			this.buffer = new char[capacity];
		}

		public bool EndOfInput
		{
			get
			{
				return this.endOfInput && this.count == 0;
			}
		}

		private int GetIndexForOffset(int offset)
		{
			int num = this.firstIndex + offset;
			if (num >= this.buffer.Length)
			{
				num -= this.buffer.Length;
			}
			return num;
		}

		public char Peek(int offset)
		{
			if (offset < 0 || offset >= this.buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset", "The offset must be betwwen zero and the capacity of the buffer.");
			}
			this.Cache(offset);
			if (offset < this.count)
			{
				return this.buffer[this.GetIndexForOffset(offset)];
			}
			return '\0';
		}

		public void Cache(int length)
		{
			while (length >= this.count)
			{
				int num = this.input.Read();
				if (num < 0)
				{
					this.endOfInput = true;
					return;
				}
				int indexForOffset = this.GetIndexForOffset(this.count);
				this.buffer[indexForOffset] = (char)num;
				this.count++;
			}
		}

		public void Skip(int length)
		{
			if (length < 1 || length > this.count)
			{
				throw new ArgumentOutOfRangeException("length", "The length must be between 1 and the number of characters in the buffer. Use the Peek() and / or Cache() methods to fill the buffer.");
			}
			this.firstIndex = this.GetIndexForOffset(length);
			this.count -= length;
		}

		private readonly TextReader input;

		private readonly char[] buffer;

		private int firstIndex;

		private int count;

		private bool endOfInput;
	}
}
