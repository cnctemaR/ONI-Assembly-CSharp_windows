using System;
using System.Runtime.CompilerServices;

namespace Iced.Intel
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class ByteArrayCodeReader : CodeReader
	{
		public int Position
		{
			get
			{
				return this.currentPosition - this.startPosition;
			}
			set
			{
				if (value > this.Count)
				{
					ThrowHelper.ThrowArgumentOutOfRangeException_value();
				}
				this.currentPosition = this.startPosition + value;
			}
		}

		public int Count
		{
			get
			{
				return this.endPosition - this.startPosition;
			}
		}

		public bool CanReadByte
		{
			get
			{
				return this.currentPosition < this.endPosition;
			}
		}

		public ByteArrayCodeReader(string hexData)
			: this(HexUtils.ToByteArray(hexData))
		{
		}

		public ByteArrayCodeReader(byte[] data)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			this.data = data;
			this.currentPosition = 0;
			this.startPosition = 0;
			this.endPosition = data.Length;
		}

		public ByteArrayCodeReader(byte[] data, int index, int count)
		{
			if (data == null)
			{
				ThrowHelper.ThrowArgumentNullException_data();
			}
			this.data = data;
			if (index < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_index();
			}
			if (count < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_count();
			}
			if (index + count > data.Length)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException_count();
			}
			this.currentPosition = index;
			this.startPosition = index;
			this.endPosition = index + count;
		}

		[NullableContext(0)]
		public ByteArrayCodeReader(ArraySegment<byte> data)
		{
			if (data.Array == null)
			{
				ThrowHelper.ThrowArgumentException();
			}
			this.data = data.Array;
			int offset = data.Offset;
			this.currentPosition = offset;
			this.startPosition = offset;
			this.endPosition = offset + data.Count;
		}

		public override int ReadByte()
		{
			if (this.currentPosition >= this.endPosition)
			{
				return -1;
			}
			byte[] array = this.data;
			int num = this.currentPosition;
			this.currentPosition = num + 1;
			return array[num];
		}

		private readonly byte[] data;

		private int currentPosition;

		private readonly int startPosition;

		private readonly int endPosition;
	}
}
