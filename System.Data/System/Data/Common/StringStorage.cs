using System;
using System.Collections;

namespace System.Data.Common
{
	internal sealed class StringStorage : DataStorage
	{
		public StringStorage(DataColumn column)
			: base(column, typeof(string), string.Empty, StorageType.String)
		{
		}

		public override object Aggregate(int[] recordNos, AggregateType kind)
		{
			switch (kind)
			{
			case AggregateType.Min:
			{
				int num = -1;
				int i;
				for (i = 0; i < recordNos.Length; i++)
				{
					if (!this.IsNull(recordNos[i]))
					{
						num = recordNos[i];
						break;
					}
				}
				if (num >= 0)
				{
					for (i++; i < recordNos.Length; i++)
					{
						if (!this.IsNull(recordNos[i]) && this.Compare(num, recordNos[i]) > 0)
						{
							num = recordNos[i];
						}
					}
					return this.Get(num);
				}
				return this._nullValue;
			}
			case AggregateType.Max:
			{
				int num2 = -1;
				int i;
				for (i = 0; i < recordNos.Length; i++)
				{
					if (!this.IsNull(recordNos[i]))
					{
						num2 = recordNos[i];
						break;
					}
				}
				if (num2 >= 0)
				{
					for (i++; i < recordNos.Length; i++)
					{
						if (this.Compare(num2, recordNos[i]) < 0)
						{
							num2 = recordNos[i];
						}
					}
					return this.Get(num2);
				}
				return this._nullValue;
			}
			case AggregateType.Count:
			{
				int num3 = 0;
				for (int i = 0; i < recordNos.Length; i++)
				{
					if (this._values[recordNos[i]] != null)
					{
						num3++;
					}
				}
				return num3;
			}
			}
			throw ExceptionBuilder.AggregateException(kind, this._dataType);
		}

		public override int Compare(int recordNo1, int recordNo2)
		{
			string text = this._values[recordNo1];
			string text2 = this._values[recordNo2];
			if (text == text2)
			{
				return 0;
			}
			if (text == null)
			{
				return -1;
			}
			if (text2 == null)
			{
				return 1;
			}
			return this._table.Compare(text, text2);
		}

		public override int CompareValueTo(int recordNo, object value)
		{
			string text = this._values[recordNo];
			if (text == null)
			{
				if (this._nullValue == value)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (this._nullValue == value)
				{
					return 1;
				}
				return this._table.Compare(text, (string)value);
			}
		}

		public override object ConvertValue(object value)
		{
			if (this._nullValue != value)
			{
				if (value != null)
				{
					value = value.ToString();
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
			this._values[recordNo2] = this._values[recordNo1];
		}

		public override object Get(int recordNo)
		{
			string text = this._values[recordNo];
			if (text != null)
			{
				return text;
			}
			return this._nullValue;
		}

		public override int GetStringLength(int record)
		{
			string text = this._values[record];
			if (text == null)
			{
				return 0;
			}
			return text.Length;
		}

		public override bool IsNull(int record)
		{
			return this._values[record] == null;
		}

		public override void Set(int record, object value)
		{
			if (this._nullValue == value)
			{
				this._values[record] = null;
				return;
			}
			this._values[record] = value.ToString();
		}

		public override void SetCapacity(int capacity)
		{
			string[] array = new string[capacity];
			if (this._values != null)
			{
				Array.Copy(this._values, 0, array, 0, Math.Min(capacity, this._values.Length));
			}
			this._values = array;
		}

		public override object ConvertXmlToObject(string s)
		{
			return s;
		}

		public override string ConvertObjectToXml(object value)
		{
			return (string)value;
		}

		protected override object GetEmptyStorage(int recordCount)
		{
			return new string[recordCount];
		}

		protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
		{
			((string[])store)[storeIndex] = this._values[record];
			nullbits.Set(storeIndex, this.IsNull(record));
		}

		protected override void SetStorage(object store, BitArray nullbits)
		{
			this._values = (string[])store;
		}

		private string[] _values;
	}
}
