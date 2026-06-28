using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Text
{
	[MonoTODO("Serialization format not compatible with .NET")]
	[ComVisible(true)]
	[Serializable]
	public sealed class StringBuilder : ISerializable
	{
		public StringBuilder(string value, int startIndex, int length, int capacity)
			: this(value, startIndex, length, capacity, int.MaxValue)
		{
		}

		private StringBuilder(string value, int startIndex, int length, int capacity, int maxCapacity)
		{
			if (value == null)
			{
				value = string.Empty;
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex", startIndex, "StartIndex cannot be less than zero.");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", length, "Length cannot be less than zero.");
			}
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity", capacity, "capacity must be greater than zero.");
			}
			if (maxCapacity < 1)
			{
				throw new ArgumentOutOfRangeException("maxCapacity", "maxCapacity is less than one.");
			}
			if (capacity > maxCapacity)
			{
				throw new ArgumentOutOfRangeException("capacity", "Capacity exceeds maximum capacity.");
			}
			if (startIndex > value.Length - length)
			{
				throw new ArgumentOutOfRangeException("startIndex", startIndex, "StartIndex and length must refer to a location within the string.");
			}
			if (capacity == 0)
			{
				if (maxCapacity > 16)
				{
					capacity = 16;
				}
				else
				{
					this._str = (this._cached_str = string.Empty);
				}
			}
			this._maxCapacity = maxCapacity;
			if (this._str == null)
			{
				this._str = string.InternalAllocateStr((length <= capacity) ? capacity : length);
			}
			if (length > 0)
			{
				string.CharCopy(this._str, 0, value, startIndex, length);
			}
			this._length = length;
		}

		public StringBuilder()
			: this(null)
		{
		}

		public StringBuilder(int capacity)
			: this(string.Empty, 0, 0, capacity)
		{
		}

		public StringBuilder(int capacity, int maxCapacity)
			: this(string.Empty, 0, 0, capacity, maxCapacity)
		{
		}

		public StringBuilder(string value)
		{
			if (value == null)
			{
				value = string.Empty;
			}
			this._length = value.Length;
			this._str = (this._cached_str = value);
			this._maxCapacity = int.MaxValue;
		}

		public StringBuilder(string value, int capacity)
			: this((value != null) ? value : string.Empty, 0, (value != null) ? value.Length : 0, capacity)
		{
		}

		private StringBuilder(SerializationInfo info, StreamingContext context)
		{
			string text = info.GetString("m_StringValue");
			if (text == null)
			{
				text = string.Empty;
			}
			this._length = text.Length;
			this._str = (this._cached_str = text);
			this._maxCapacity = info.GetInt32("m_MaxCapacity");
			if (this._maxCapacity < 0)
			{
				this._maxCapacity = int.MaxValue;
			}
			this.Capacity = info.GetInt32("Capacity");
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("m_MaxCapacity", this._maxCapacity);
			info.AddValue("Capacity", this.Capacity);
			info.AddValue("m_StringValue", this.ToString());
			info.AddValue("m_currentThread", 0);
		}

		public int MaxCapacity
		{
			get
			{
				return this._maxCapacity;
			}
		}

		public int Capacity
		{
			get
			{
				if (this._str.Length == 0)
				{
					return Math.Min(this._maxCapacity, 16);
				}
				return this._str.Length;
			}
			set
			{
				if (value < this._length)
				{
					throw new ArgumentException("Capacity must be larger than length");
				}
				if (value > this._maxCapacity)
				{
					throw new ArgumentOutOfRangeException("value", "Should be less than or equal to MaxCapacity");
				}
				this.InternalEnsureCapacity(value);
			}
		}

		public int Length
		{
			get
			{
				return this._length;
			}
			set
			{
				if (value < 0 || value > this._maxCapacity)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (value == this._length)
				{
					return;
				}
				if (value < this._length)
				{
					this.InternalEnsureCapacity(value);
					this._length = value;
				}
				else
				{
					this.Append('\0', value - this._length);
				}
			}
		}

		[IndexerName("Chars")]
		public char this[int index]
		{
			get
			{
				if (index >= this._length || index < 0)
				{
					throw new IndexOutOfRangeException();
				}
				return this._str[index];
			}
			set
			{
				if (index >= this._length || index < 0)
				{
					throw new IndexOutOfRangeException();
				}
				if (this._cached_str != null)
				{
					this.InternalEnsureCapacity(this._length);
				}
				this._str.InternalSetChar(index, value);
			}
		}

		public override string ToString()
		{
			if (this._length == 0)
			{
				return string.Empty;
			}
			if (this._cached_str != null)
			{
				return this._cached_str;
			}
			if (this._length < this._str.Length >> 1)
			{
				this._cached_str = this._str.SubstringUnchecked(0, this._length);
				return this._cached_str;
			}
			this._cached_str = this._str;
			this._str.InternalSetLength(this._length);
			return this._str;
		}

		public string ToString(int startIndex, int length)
		{
			if (startIndex < 0 || length < 0 || startIndex > this._length - length)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (startIndex == 0 && length == this._length)
			{
				return this.ToString();
			}
			return this._str.SubstringUnchecked(startIndex, length);
		}

		public int EnsureCapacity(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("Capacity must be greater than 0.");
			}
			if (capacity <= this._str.Length)
			{
				return this._str.Length;
			}
			this.InternalEnsureCapacity(capacity);
			return this._str.Length;
		}

		public bool Equals(StringBuilder sb)
		{
			return sb != null && (this._length == sb.Length && this._str == sb._str);
		}

		public StringBuilder Remove(int startIndex, int length)
		{
			if (startIndex < 0 || length < 0 || startIndex > this._length - length)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (this._cached_str != null)
			{
				this.InternalEnsureCapacity(this._length);
			}
			if (this._length - (startIndex + length) > 0)
			{
				string.CharCopy(this._str, startIndex, this._str, startIndex + length, this._length - (startIndex + length));
			}
			this._length -= length;
			return this;
		}

		public StringBuilder Replace(char oldChar, char newChar)
		{
			return this.Replace(oldChar, newChar, 0, this._length);
		}

		public StringBuilder Replace(char oldChar, char newChar, int startIndex, int count)
		{
			if (startIndex > this._length - count || startIndex < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (this._cached_str != null)
			{
				this.InternalEnsureCapacity(this._str.Length);
			}
			for (int i = startIndex; i < startIndex + count; i++)
			{
				if (this._str[i] == oldChar)
				{
					this._str.InternalSetChar(i, newChar);
				}
			}
			return this;
		}

		public StringBuilder Replace(string oldValue, string newValue)
		{
			return this.Replace(oldValue, newValue, 0, this._length);
		}

		public StringBuilder Replace(string oldValue, string newValue, int startIndex, int count)
		{
			if (oldValue == null)
			{
				throw new ArgumentNullException("The old value cannot be null.");
			}
			if (startIndex < 0 || count < 0 || startIndex > this._length - count)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (oldValue.Length == 0)
			{
				throw new ArgumentException("The old value cannot be zero length.");
			}
			string text = this._str.Substring(startIndex, count);
			string text2 = text.Replace(oldValue, newValue);
			if (text2 == text)
			{
				return this;
			}
			this.InternalEnsureCapacity(text2.Length + (this._length - count));
			if (text2.Length < count)
			{
				string.CharCopy(this._str, startIndex + text2.Length, this._str, startIndex + count, this._length - startIndex - count);
			}
			else if (text2.Length > count)
			{
				string.CharCopyReverse(this._str, startIndex + text2.Length, this._str, startIndex + count, this._length - startIndex - count);
			}
			string.CharCopy(this._str, startIndex, text2, 0, text2.Length);
			this._length = text2.Length + (this._length - count);
			return this;
		}

		public StringBuilder Append(char[] value)
		{
			if (value == null)
			{
				return this;
			}
			int num = this._length + value.Length;
			if (this._cached_str != null || this._str.Length < num)
			{
				this.InternalEnsureCapacity(num);
			}
			string.CharCopy(this._str, this._length, value, 0, value.Length);
			this._length = num;
			return this;
		}

		public StringBuilder Append(string value)
		{
			if (value == null)
			{
				return this;
			}
			if (this._length == 0 && value.Length < this._maxCapacity && value.Length > this._str.Length)
			{
				this._length = value.Length;
				this._cached_str = value;
				this._str = value;
				return this;
			}
			int num = this._length + value.Length;
			if (this._cached_str != null || this._str.Length < num)
			{
				this.InternalEnsureCapacity(num);
			}
			string.CharCopy(this._str, this._length, value, 0, value.Length);
			this._length = num;
			return this;
		}

		public StringBuilder Append(bool value)
		{
			return this.Append(value.ToString());
		}

		public StringBuilder Append(byte value)
		{
			return this.Append(value.ToString());
		}

		public StringBuilder Append(decimal value)
		{
			return this.Append(value.ToString());
		}

		public StringBuilder Append(double value)
		{
			return this.Append(value.ToString());
		}

		public StringBuilder Append(short value)
		{
			return this.Append(value.ToString());
		}

		public StringBuilder Append(int value)
		{
			return this.Append(value.ToString());
		}

		public StringBuilder Append(long value)
		{
			return this.Append(value.ToString());
		}

		public StringBuilder Append(object value)
		{
			if (value == null)
			{
				return this;
			}
			return this.Append(value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Append(sbyte value)
		{
			return this.Append(value.ToString());
		}

		public StringBuilder Append(float value)
		{
			return this.Append(value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Append(ushort value)
		{
			return this.Append(value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Append(uint value)
		{
			return this.Append(value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Append(ulong value)
		{
			return this.Append(value.ToString());
		}

		public StringBuilder Append(char value)
		{
			int num = this._length + 1;
			if (this._cached_str != null || this._str.Length < num)
			{
				this.InternalEnsureCapacity(num);
			}
			this._str.InternalSetChar(this._length, value);
			this._length = num;
			return this;
		}

		public StringBuilder Append(char value, int repeatCount)
		{
			if (repeatCount < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			this.InternalEnsureCapacity(this._length + repeatCount);
			for (int i = 0; i < repeatCount; i++)
			{
				this._str.InternalSetChar(this._length++, value);
			}
			return this;
		}

		public StringBuilder Append(char[] value, int startIndex, int charCount)
		{
			if (value == null)
			{
				if (startIndex != 0 || charCount != 0)
				{
					throw new ArgumentNullException("value");
				}
				return this;
			}
			else
			{
				if (charCount < 0 || startIndex < 0 || startIndex > value.Length - charCount)
				{
					throw new ArgumentOutOfRangeException();
				}
				int num = this._length + charCount;
				this.InternalEnsureCapacity(num);
				string.CharCopy(this._str, this._length, value, startIndex, charCount);
				this._length = num;
				return this;
			}
		}

		public StringBuilder Append(string value, int startIndex, int count)
		{
			if (value == null)
			{
				if (startIndex != 0 && count != 0)
				{
					throw new ArgumentNullException("value");
				}
				return this;
			}
			else
			{
				if (count < 0 || startIndex < 0 || startIndex > value.Length - count)
				{
					throw new ArgumentOutOfRangeException();
				}
				int num = this._length + count;
				if (this._cached_str != null || this._str.Length < num)
				{
					this.InternalEnsureCapacity(num);
				}
				string.CharCopy(this._str, this._length, value, startIndex, count);
				this._length = num;
				return this;
			}
		}

		[ComVisible(false)]
		public StringBuilder AppendLine()
		{
			return this.Append(Environment.NewLine);
		}

		[ComVisible(false)]
		public StringBuilder AppendLine(string value)
		{
			return this.Append(value).Append(Environment.NewLine);
		}

		public StringBuilder AppendFormat(string format, params object[] args)
		{
			return this.AppendFormat(null, format, args);
		}

		public StringBuilder AppendFormat(IFormatProvider provider, string format, params object[] args)
		{
			string.FormatHelper(this, provider, format, args);
			return this;
		}

		public StringBuilder AppendFormat(string format, object arg0)
		{
			return this.AppendFormat(null, format, new object[] { arg0 });
		}

		public StringBuilder AppendFormat(string format, object arg0, object arg1)
		{
			return this.AppendFormat(null, format, new object[] { arg0, arg1 });
		}

		public StringBuilder AppendFormat(string format, object arg0, object arg1, object arg2)
		{
			return this.AppendFormat(null, format, new object[] { arg0, arg1, arg2 });
		}

		public StringBuilder Insert(int index, char[] value)
		{
			return this.Insert(index, new string(value));
		}

		public StringBuilder Insert(int index, string value)
		{
			if (index > this._length || index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (value == null || value.Length == 0)
			{
				return this;
			}
			this.InternalEnsureCapacity(this._length + value.Length);
			string.CharCopyReverse(this._str, index + value.Length, this._str, index, this._length - index);
			string.CharCopy(this._str, index, value, 0, value.Length);
			this._length += value.Length;
			return this;
		}

		public StringBuilder Insert(int index, bool value)
		{
			return this.Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, byte value)
		{
			return this.Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, char value)
		{
			if (index > this._length || index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this.InternalEnsureCapacity(this._length + 1);
			string.CharCopyReverse(this._str, index + 1, this._str, index, this._length - index);
			this._str.InternalSetChar(index, value);
			this._length++;
			return this;
		}

		public StringBuilder Insert(int index, decimal value)
		{
			return this.Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, double value)
		{
			return this.Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, short value)
		{
			return this.Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, int value)
		{
			return this.Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, long value)
		{
			return this.Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, object value)
		{
			return this.Insert(index, value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Insert(int index, sbyte value)
		{
			return this.Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, float value)
		{
			return this.Insert(index, value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Insert(int index, ushort value)
		{
			return this.Insert(index, value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Insert(int index, uint value)
		{
			return this.Insert(index, value.ToString());
		}

		[CLSCompliant(false)]
		public StringBuilder Insert(int index, ulong value)
		{
			return this.Insert(index, value.ToString());
		}

		public StringBuilder Insert(int index, string value, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (value != null && value != string.Empty)
			{
				for (int i = 0; i < count; i++)
				{
					this.Insert(index, value);
				}
			}
			return this;
		}

		public StringBuilder Insert(int index, char[] value, int startIndex, int charCount)
		{
			if (value == null)
			{
				if (startIndex == 0 && charCount == 0)
				{
					return this;
				}
				throw new ArgumentNullException("value");
			}
			else
			{
				if (charCount < 0 || startIndex < 0 || startIndex > value.Length - charCount)
				{
					throw new ArgumentOutOfRangeException();
				}
				return this.Insert(index, new string(value, startIndex, charCount));
			}
		}

		private void InternalEnsureCapacity(int size)
		{
			if (size > this._str.Length || this._cached_str == this._str)
			{
				int num = this._str.Length;
				if (size > num)
				{
					if (this._cached_str == this._str && num < 16)
					{
						num = 16;
					}
					num <<= 1;
					if (size > num)
					{
						num = size;
					}
					if (num >= 2147483647 || num < 0)
					{
						num = int.MaxValue;
					}
					if (num > this._maxCapacity && size <= this._maxCapacity)
					{
						num = this._maxCapacity;
					}
					if (num > this._maxCapacity)
					{
						throw new ArgumentOutOfRangeException("size", "capacity was less than the current size.");
					}
				}
				string text = string.InternalAllocateStr(num);
				if (this._length > 0)
				{
					string.CharCopy(text, 0, this._str, 0, this._length);
				}
				this._str = text;
			}
			this._cached_str = null;
		}

		[ComVisible(false)]
		public void CopyTo(int sourceIndex, char[] destination, int destinationIndex, int count)
		{
			if (destination == null)
			{
				throw new ArgumentNullException("destination");
			}
			if (this.Length - count < sourceIndex || destination.Length - count < destinationIndex || sourceIndex < 0 || destinationIndex < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			for (int i = 0; i < count; i++)
			{
				destination[destinationIndex + i] = this._str[sourceIndex + i];
			}
		}

		private const int constDefaultCapacity = 16;

		private int _length;

		private string _str;

		private string _cached_str;

		private int _maxCapacity;
	}
}
