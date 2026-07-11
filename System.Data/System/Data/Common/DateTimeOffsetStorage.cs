using System;
using System.Collections;
using System.Xml;

namespace System.Data.Common
{
	internal sealed class DateTimeOffsetStorage : DataStorage
	{
		internal DateTimeOffsetStorage(DataColumn column)
			: base(column, typeof(DateTimeOffset), DateTimeOffsetStorage.s_defaultValue, StorageType.DateTimeOffset)
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
					DateTimeOffset dateTimeOffset = DateTimeOffset.MaxValue;
					foreach (int num in records)
					{
						if (base.HasValue(num))
						{
							dateTimeOffset = ((DateTimeOffset.Compare(this._values[num], dateTimeOffset) < 0) ? this._values[num] : dateTimeOffset);
							flag = true;
						}
					}
					if (flag)
					{
						return dateTimeOffset;
					}
					return this._nullValue;
				}
				case AggregateType.Max:
				{
					DateTimeOffset dateTimeOffset2 = DateTimeOffset.MinValue;
					foreach (int num2 in records)
					{
						if (base.HasValue(num2))
						{
							dateTimeOffset2 = ((DateTimeOffset.Compare(this._values[num2], dateTimeOffset2) >= 0) ? this._values[num2] : dateTimeOffset2);
							flag = true;
						}
					}
					if (flag)
					{
						return dateTimeOffset2;
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
				{
					int num3 = 0;
					for (int k = 0; k < records.Length; k++)
					{
						if (base.HasValue(records[k]))
						{
							num3++;
						}
					}
					return num3;
				}
				}
			}
			catch (OverflowException)
			{
				throw ExprException.Overflow(typeof(DateTimeOffset));
			}
			throw ExceptionBuilder.AggregateException(kind, this._dataType);
		}

		public override int Compare(int recordNo1, int recordNo2)
		{
			DateTimeOffset dateTimeOffset = this._values[recordNo1];
			DateTimeOffset dateTimeOffset2 = this._values[recordNo2];
			if (dateTimeOffset == DateTimeOffsetStorage.s_defaultValue || dateTimeOffset2 == DateTimeOffsetStorage.s_defaultValue)
			{
				int num = base.CompareBits(recordNo1, recordNo2);
				if (num != 0)
				{
					return num;
				}
			}
			return DateTimeOffset.Compare(dateTimeOffset, dateTimeOffset2);
		}

		public override int CompareValueTo(int recordNo, object value)
		{
			if (this._nullValue == value)
			{
				if (!base.HasValue(recordNo))
				{
					return 0;
				}
				return 1;
			}
			else
			{
				DateTimeOffset dateTimeOffset = this._values[recordNo];
				if (DateTimeOffsetStorage.s_defaultValue == dateTimeOffset && !base.HasValue(recordNo))
				{
					return -1;
				}
				return DateTimeOffset.Compare(dateTimeOffset, (DateTimeOffset)value);
			}
		}

		public override object ConvertValue(object value)
		{
			if (this._nullValue != value)
			{
				if (value != null)
				{
					value = (DateTimeOffset)value;
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
			DateTimeOffset dateTimeOffset = this._values[record];
			if (dateTimeOffset != DateTimeOffsetStorage.s_defaultValue || base.HasValue(record))
			{
				return dateTimeOffset;
			}
			return this._nullValue;
		}

		public override void Set(int record, object value)
		{
			if (this._nullValue == value)
			{
				this._values[record] = DateTimeOffsetStorage.s_defaultValue;
				base.SetNullBit(record, true);
				return;
			}
			this._values[record] = (DateTimeOffset)value;
			base.SetNullBit(record, false);
		}

		public override void SetCapacity(int capacity)
		{
			DateTimeOffset[] array = new DateTimeOffset[capacity];
			if (this._values != null)
			{
				Array.Copy(this._values, 0, array, 0, Math.Min(capacity, this._values.Length));
			}
			this._values = array;
			base.SetCapacity(capacity);
		}

		public override object ConvertXmlToObject(string s)
		{
			return XmlConvert.ToDateTimeOffset(s);
		}

		public override string ConvertObjectToXml(object value)
		{
			return XmlConvert.ToString((DateTimeOffset)value);
		}

		protected override object GetEmptyStorage(int recordCount)
		{
			return new DateTimeOffset[recordCount];
		}

		protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
		{
			((DateTimeOffset[])store)[storeIndex] = this._values[record];
			nullbits.Set(storeIndex, !base.HasValue(record));
		}

		protected override void SetStorage(object store, BitArray nullbits)
		{
			this._values = (DateTimeOffset[])store;
			base.SetNullStorage(nullbits);
		}

		private static readonly DateTimeOffset s_defaultValue = DateTimeOffset.MinValue;

		private DateTimeOffset[] _values;
	}
}
