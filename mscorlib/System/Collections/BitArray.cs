using System;
using System.Runtime.InteropServices;

namespace System.Collections
{
	[ComVisible(true)]
	[Serializable]
	public sealed class BitArray : IEnumerable, ICloneable, ICollection
	{
		public BitArray(BitArray bits)
		{
			if (bits == null)
			{
				throw new ArgumentNullException("bits");
			}
			this.m_length = bits.m_length;
			this.m_array = new int[(this.m_length + 31) / 32];
			if (this.m_array.Length == 1)
			{
				this.m_array[0] = bits.m_array[0];
			}
			else
			{
				Array.Copy(bits.m_array, this.m_array, this.m_array.Length);
			}
		}

		public BitArray(bool[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			this.m_length = values.Length;
			this.m_array = new int[(this.m_length + 31) / 32];
			for (int i = 0; i < values.Length; i++)
			{
				this[i] = values[i];
			}
		}

		public BitArray(byte[] bytes)
		{
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			this.m_length = bytes.Length * 8;
			this.m_array = new int[(this.m_length + 31) / 32];
			for (int i = 0; i < bytes.Length; i++)
			{
				this.setByte(i, bytes[i]);
			}
		}

		public BitArray(int[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			int num = values.Length;
			this.m_length = num * 32;
			this.m_array = new int[num];
			Array.Copy(values, this.m_array, num);
		}

		public BitArray(int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length");
			}
			this.m_length = length;
			this.m_array = new int[(this.m_length + 31) / 32];
		}

		public BitArray(int length, bool defaultValue)
			: this(length)
		{
			if (defaultValue)
			{
				for (int i = 0; i < this.m_array.Length; i++)
				{
					this.m_array[i] = -1;
				}
			}
		}

		private BitArray(int[] array, int length)
		{
			this.m_array = array;
			this.m_length = length;
		}

		private byte getByte(int byteIndex)
		{
			int num = byteIndex / 4;
			int num2 = byteIndex % 4 * 8;
			int num3 = this.m_array[num] & (255 << num2);
			return (byte)((num3 >> num2) & 255);
		}

		private void setByte(int byteIndex, byte value)
		{
			int num = byteIndex / 4;
			int num2 = byteIndex % 4 * 8;
			this.m_array[num] &= ~(255 << num2);
			this.m_array[num] |= (int)value << num2;
			this._version++;
		}

		private void checkOperand(BitArray operand)
		{
			if (operand == null)
			{
				throw new ArgumentNullException();
			}
			if (operand.m_length != this.m_length)
			{
				throw new ArgumentException();
			}
		}

		public int Count
		{
			get
			{
				return this.m_length;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public bool this[int index]
		{
			get
			{
				return this.Get(index);
			}
			set
			{
				this.Set(index, value);
			}
		}

		public int Length
		{
			get
			{
				return this.m_length;
			}
			set
			{
				if (this.m_length == value)
				{
					return;
				}
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (value > this.m_length)
				{
					int num = (value + 31) / 32;
					int num2 = (this.m_length + 31) / 32;
					if (num > this.m_array.Length)
					{
						int[] array = new int[num];
						Array.Copy(this.m_array, array, this.m_array.Length);
						this.m_array = array;
					}
					else
					{
						Array.Clear(this.m_array, num2, num - num2);
					}
					int num3 = this.m_length % 32;
					if (num3 > 0)
					{
						this.m_array[num2 - 1] &= (1 << num3) - 1;
					}
				}
				this.m_length = value;
				this._version++;
			}
		}

		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		public object Clone()
		{
			return new BitArray(this);
		}

		public void CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("array", "Array rank must be 1");
			}
			if (index >= array.Length && this.m_length > 0)
			{
				throw new ArgumentException("index", "index is greater than array.Length");
			}
			if (array is bool[])
			{
				if (array.Length - index < this.m_length)
				{
					throw new ArgumentException();
				}
				bool[] array2 = (bool[])array;
				for (int i = 0; i < this.m_length; i++)
				{
					array2[index + i] = this[i];
				}
			}
			else if (array is byte[])
			{
				int num = (this.m_length + 7) / 8;
				if (array.Length - index < num)
				{
					throw new ArgumentException();
				}
				byte[] array3 = (byte[])array;
				for (int j = 0; j < num; j++)
				{
					array3[index + j] = this.getByte(j);
				}
			}
			else
			{
				if (!(array is int[]))
				{
					throw new ArgumentException("array", "Unsupported type");
				}
				Array.Copy(this.m_array, 0, array, index, (this.m_length + 31) / 32);
			}
		}

		public BitArray Not()
		{
			int num = (this.m_length + 31) / 32;
			for (int i = 0; i < num; i++)
			{
				this.m_array[i] = ~this.m_array[i];
			}
			this._version++;
			return this;
		}

		public BitArray And(BitArray value)
		{
			this.checkOperand(value);
			int num = (this.m_length + 31) / 32;
			for (int i = 0; i < num; i++)
			{
				this.m_array[i] &= value.m_array[i];
			}
			this._version++;
			return this;
		}

		public BitArray Or(BitArray value)
		{
			this.checkOperand(value);
			int num = (this.m_length + 31) / 32;
			for (int i = 0; i < num; i++)
			{
				this.m_array[i] |= value.m_array[i];
			}
			this._version++;
			return this;
		}

		public BitArray Xor(BitArray value)
		{
			this.checkOperand(value);
			int num = (this.m_length + 31) / 32;
			for (int i = 0; i < num; i++)
			{
				this.m_array[i] ^= value.m_array[i];
			}
			this._version++;
			return this;
		}

		public bool Get(int index)
		{
			if (index < 0 || index >= this.m_length)
			{
				throw new ArgumentOutOfRangeException();
			}
			return (this.m_array[index >> 5] & (1 << index)) != 0;
		}

		public void Set(int index, bool value)
		{
			if (index < 0 || index >= this.m_length)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (value)
			{
				this.m_array[index >> 5] |= 1 << index;
			}
			else
			{
				this.m_array[index >> 5] &= ~(1 << index);
			}
			this._version++;
		}

		public void SetAll(bool value)
		{
			if (value)
			{
				for (int i = 0; i < this.m_array.Length; i++)
				{
					this.m_array[i] = -1;
				}
			}
			else
			{
				Array.Clear(this.m_array, 0, this.m_array.Length);
			}
			this._version++;
		}

		public IEnumerator GetEnumerator()
		{
			return new BitArray.BitArrayEnumerator(this);
		}

		private int[] m_array;

		private int m_length;

		private int _version;

		[Serializable]
		private class BitArrayEnumerator : IEnumerator, ICloneable
		{
			public BitArrayEnumerator(BitArray ba)
			{
				this._index = -1;
				this._bitArray = ba;
				this._version = ba._version;
			}

			public object Clone()
			{
				return base.MemberwiseClone();
			}

			public object Current
			{
				get
				{
					if (this._index == -1)
					{
						throw new InvalidOperationException("Enum not started");
					}
					if (this._index >= this._bitArray.Count)
					{
						throw new InvalidOperationException("Enum Ended");
					}
					return this._current;
				}
			}

			public bool MoveNext()
			{
				this.checkVersion();
				if (this._index < this._bitArray.Count - 1)
				{
					this._current = this._bitArray[++this._index];
					return true;
				}
				this._index = this._bitArray.Count;
				return false;
			}

			public void Reset()
			{
				this.checkVersion();
				this._index = -1;
			}

			private void checkVersion()
			{
				if (this._version != this._bitArray._version)
				{
					throw new InvalidOperationException();
				}
			}

			private BitArray _bitArray;

			private bool _current;

			private int _index;

			private int _version;
		}
	}
}
