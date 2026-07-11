using System;

namespace YamlDotNet.Core
{
	[Serializable]
	internal class StringLookAheadBuffer : ILookAheadBuffer
	{
		public StringLookAheadBuffer(string value)
		{
			this.value = value;
		}

		public int Position { get; private set; }

		public int Length
		{
			get
			{
				return this.value.Length;
			}
		}

		public bool EndOfInput
		{
			get
			{
				return this.IsOutside(this.Position);
			}
		}

		public char Peek(int offset)
		{
			int num = this.Position + offset;
			return (!this.IsOutside(num)) ? this.value[num] : '\0';
		}

		private bool IsOutside(int index)
		{
			return index >= this.value.Length;
		}

		public void Skip(int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", "The length must be positive.");
			}
			this.Position += length;
		}

		private readonly string value;
	}
}
