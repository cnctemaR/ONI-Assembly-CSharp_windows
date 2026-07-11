using System;
using System.Collections;
using System.Globalization;
using System.Numerics;

namespace System.Data.Common
{
	internal sealed class BigIntegerStorage : DataStorage
	{
		internal BigIntegerStorage(DataColumn column)
			: base(column, typeof(BigInteger), BigInteger.Zero, StorageType.BigInteger)
		{
		}

		public override object Aggregate(int[] records, AggregateType kind)
		{
			throw ExceptionBuilder.AggregateException(kind, this._dataType);
		}

		public override int Compare(int recordNo1, int recordNo2)
		{
			BigInteger bigInteger = this._values[recordNo1];
			BigInteger bigInteger2 = this._values[recordNo2];
			if (bigInteger.IsZero || bigInteger2.IsZero)
			{
				int num = base.CompareBits(recordNo1, recordNo2);
				if (num != 0)
				{
					return num;
				}
			}
			return bigInteger.CompareTo(bigInteger2);
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
				BigInteger bigInteger = this._values[recordNo];
				if (bigInteger.IsZero && !base.HasValue(recordNo))
				{
					return -1;
				}
				return bigInteger.CompareTo((BigInteger)value);
			}
		}

		internal static BigInteger ConvertToBigInteger(object value, IFormatProvider formatProvider)
		{
			if (value.GetType() == typeof(BigInteger))
			{
				return (BigInteger)value;
			}
			if (value.GetType() == typeof(string))
			{
				return BigInteger.Parse((string)value, formatProvider);
			}
			if (value.GetType() == typeof(long))
			{
				return (long)value;
			}
			if (value.GetType() == typeof(int))
			{
				return (int)value;
			}
			if (value.GetType() == typeof(short))
			{
				return (short)value;
			}
			if (value.GetType() == typeof(sbyte))
			{
				return (sbyte)value;
			}
			if (value.GetType() == typeof(ulong))
			{
				return (ulong)value;
			}
			if (value.GetType() == typeof(uint))
			{
				return (uint)value;
			}
			if (value.GetType() == typeof(ushort))
			{
				return (ushort)value;
			}
			if (value.GetType() == typeof(byte))
			{
				return (byte)value;
			}
			throw ExceptionBuilder.ConvertFailed(value.GetType(), typeof(BigInteger));
		}

		internal static object ConvertFromBigInteger(BigInteger value, Type type, IFormatProvider formatProvider)
		{
			if (type == typeof(string))
			{
				return value.ToString("D", formatProvider);
			}
			if (type == typeof(sbyte))
			{
				return (sbyte)value;
			}
			if (type == typeof(short))
			{
				return (short)value;
			}
			if (type == typeof(int))
			{
				return (int)value;
			}
			if (type == typeof(long))
			{
				return (long)value;
			}
			if (type == typeof(byte))
			{
				return (byte)value;
			}
			if (type == typeof(ushort))
			{
				return (ushort)value;
			}
			if (type == typeof(uint))
			{
				return (uint)value;
			}
			if (type == typeof(ulong))
			{
				return (ulong)value;
			}
			if (type == typeof(float))
			{
				return (float)value;
			}
			if (type == typeof(double))
			{
				return (double)value;
			}
			if (type == typeof(decimal))
			{
				return (decimal)value;
			}
			if (type == typeof(BigInteger))
			{
				return value;
			}
			throw ExceptionBuilder.ConvertFailed(typeof(BigInteger), type);
		}

		public override object ConvertValue(object value)
		{
			if (this._nullValue != value)
			{
				if (value != null)
				{
					value = BigIntegerStorage.ConvertToBigInteger(value, base.FormatProvider);
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
			BigInteger bigInteger = this._values[record];
			if (!bigInteger.IsZero)
			{
				return bigInteger;
			}
			return base.GetBits(record);
		}

		public override void Set(int record, object value)
		{
			if (this._nullValue == value)
			{
				this._values[record] = BigInteger.Zero;
				base.SetNullBit(record, true);
				return;
			}
			this._values[record] = BigIntegerStorage.ConvertToBigInteger(value, base.FormatProvider);
			base.SetNullBit(record, false);
		}

		public override void SetCapacity(int capacity)
		{
			BigInteger[] array = new BigInteger[capacity];
			if (this._values != null)
			{
				Array.Copy(this._values, 0, array, 0, Math.Min(capacity, this._values.Length));
			}
			this._values = array;
			base.SetCapacity(capacity);
		}

		public override object ConvertXmlToObject(string s)
		{
			return BigInteger.Parse(s, CultureInfo.InvariantCulture);
		}

		public override string ConvertObjectToXml(object value)
		{
			return ((BigInteger)value).ToString("D", CultureInfo.InvariantCulture);
		}

		protected override object GetEmptyStorage(int recordCount)
		{
			return new BigInteger[recordCount];
		}

		protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
		{
			((BigInteger[])store)[storeIndex] = this._values[record];
			nullbits.Set(storeIndex, !base.HasValue(record));
		}

		protected override void SetStorage(object store, BitArray nullbits)
		{
			this._values = (BigInteger[])store;
			base.SetNullStorage(nullbits);
		}

		private BigInteger[] _values;
	}
}
