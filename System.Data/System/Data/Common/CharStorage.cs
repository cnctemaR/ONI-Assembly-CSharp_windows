using System;
using System.Collections;
using System.Xml;

namespace System.Data.Common
{
	internal sealed class CharStorage : DataStorage
	{
		internal CharStorage(DataColumn column)
			: base(column, typeof(char), '\0', StorageType.Char)
		{
		}

		public override object Aggregate(int[] records, AggregateType kind)
		{
			bool flag = false;
			try
			{
				switch (kind)
				{
				case AggregateType.Min:
				{
					char c = char.MaxValue;
					foreach (int num in records)
					{
						if (!this.IsNull(num))
						{
							c = ((this._values[num] < c) ? this._values[num] : c);
							flag = true;
						}
					}
					if (flag)
					{
						return c;
					}
					return this._nullValue;
				}
				case AggregateType.Max:
				{
					char c2 = '\0';
					foreach (int num2 in records)
					{
						if (!this.IsNull(num2))
						{
							c2 = ((this._values[num2] > c2) ? this._values[num2] : c2);
							flag = true;
						}
					}
					if (flag)
					{
						return c2;
					}
					return this._nullValue;
				}
				case AggregateType.First:
					if (records.Length != 0)
					{
						return this._values[records[0]];
					}
					return null;
				case AggregateType.Count:
					return base.Aggregate(records, kind);
				}
			}
			catch (OverflowException)
			{
				throw ExprException.Overflow(typeof(char));
			}
			throw ExceptionBuilder.AggregateException(kind, this._dataType);
		}

		public override int Compare(int recordNo1, int recordNo2)
		{
			char c = this._values[recordNo1];
			char c2 = this._values[recordNo2];
			if (c == '\0' || c2 == '\0')
			{
				int num = base.CompareBits(recordNo1, recordNo2);
				if (num != 0)
				{
					return num;
				}
			}
			return c.CompareTo(c2);
		}

		public override int CompareValueTo(int recordNo, object value)
		{
			if (this._nullValue == value)
			{
				if (this.IsNull(recordNo))
				{
					return 0;
				}
				return 1;
			}
			else
			{
				char c = this._values[recordNo];
				if (c == '\0' && this.IsNull(recordNo))
				{
					return -1;
				}
				return c.CompareTo((char)value);
			}
		}

		public override object ConvertValue(object value)
		{
			if (this._nullValue != value)
			{
				if (value != null)
				{
					value = ((IConvertible)value).ToChar(base.FormatProvider);
				}
				else
				{
					value = this._nullValue;
				}
			}
			return value;
		}

		public override void Copy(int recordNo1, int recordNo2)
		{
			base.CopyBits(recordNo1, recordNo2);
			this._values[recordNo2] = this._values[recordNo1];
		}

		public override object Get(int record)
		{
			char c = this._values[record];
			if (c != '\0')
			{
				return c;
			}
			return base.GetBits(record);
		}

		public override void Set(int record, object value)
		{
			if (this._nullValue == value)
			{
				this._values[record] = '\0';
				base.SetNullBit(record, true);
				return;
			}
			char c = ((IConvertible)value).ToChar(base.FormatProvider);
			if ((c >= '\ud800' && c <= '\udfff') || (c < '!' && (c == '\t' || c == '\n' || c == '\r')))
			{
				throw ExceptionBuilder.ProblematicChars(c);
			}
			this._values[record] = c;
			base.SetNullBit(record, false);
		}

		public override void SetCapacity(int capacity)
		{
			char[] array = new char[capacity];
			if (this._values != null)
			{
				Array.Copy(this._values, 0, array, 0, Math.Min(capacity, this._values.Length));
			}
			this._values = array;
			base.SetCapacity(capacity);
		}

		public override object ConvertXmlToObject(string s)
		{
			return XmlConvert.ToChar(s);
		}

		public override string ConvertObjectToXml(object value)
		{
			return XmlConvert.ToString((char)value);
		}

		protected override object GetEmptyStorage(int recordCount)
		{
			return new char[recordCount];
		}

		protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
		{
			((char[])store)[storeIndex] = this._values[record];
			nullbits.Set(storeIndex, this.IsNull(record));
		}

		protected override void SetStorage(object store, BitArray nullbits)
		{
			this._values = (char[])store;
			base.SetNullStorage(nullbits);
		}

		private const char defaultValue = '\0';

		private char[] _values;
	}
}
