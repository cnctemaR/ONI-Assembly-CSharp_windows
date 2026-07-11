using System;
using System.Collections;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace System.Data.Common
{
	internal sealed class SqlDateTimeStorage : DataStorage
	{
		public SqlDateTimeStorage(DataColumn column)
			: base(column, typeof(SqlDateTime), SqlDateTime.Null, SqlDateTime.Null, StorageType.SqlDateTime)
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
					SqlDateTime sqlDateTime = SqlDateTime.MaxValue;
					foreach (int num in records)
					{
						if (!this.IsNull(num))
						{
							if (SqlDateTime.LessThan(this._values[num], sqlDateTime).IsTrue)
							{
								sqlDateTime = this._values[num];
							}
							flag = true;
						}
					}
					if (flag)
					{
						return sqlDateTime;
					}
					return this._nullValue;
				}
				case AggregateType.Max:
				{
					SqlDateTime sqlDateTime2 = SqlDateTime.MinValue;
					foreach (int num2 in records)
					{
						if (!this.IsNull(num2))
						{
							if (SqlDateTime.GreaterThan(this._values[num2], sqlDateTime2).IsTrue)
							{
								sqlDateTime2 = this._values[num2];
							}
							flag = true;
						}
					}
					if (flag)
					{
						return sqlDateTime2;
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
						if (!this.IsNull(records[k]))
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
				throw ExprException.Overflow(typeof(SqlDateTime));
			}
			throw ExceptionBuilder.AggregateException(kind, this._dataType);
		}

		public override int Compare(int recordNo1, int recordNo2)
		{
			return this._values[recordNo1].CompareTo(this._values[recordNo2]);
		}

		public override int CompareValueTo(int recordNo, object value)
		{
			return this._values[recordNo].CompareTo((SqlDateTime)value);
		}

		public override object ConvertValue(object value)
		{
			if (value != null)
			{
				return SqlConvert.ConvertToSqlDateTime(value);
			}
			return this._nullValue;
		}

		public override void Copy(int recordNo1, int recordNo2)
		{
			this._values[recordNo2] = this._values[recordNo1];
		}

		public override object Get(int record)
		{
			return this._values[record];
		}

		public override bool IsNull(int record)
		{
			return this._values[record].IsNull;
		}

		public override void Set(int record, object value)
		{
			this._values[record] = SqlConvert.ConvertToSqlDateTime(value);
		}

		public override void SetCapacity(int capacity)
		{
			SqlDateTime[] array = new SqlDateTime[capacity];
			if (this._values != null)
			{
				Array.Copy(this._values, 0, array, 0, Math.Min(capacity, this._values.Length));
			}
			this._values = array;
		}

		public override object ConvertXmlToObject(string s)
		{
			SqlDateTime sqlDateTime = default(SqlDateTime);
			TextReader textReader = new StringReader("<col>" + s + "</col>");
			IXmlSerializable xmlSerializable = sqlDateTime;
			using (XmlTextReader xmlTextReader = new XmlTextReader(textReader))
			{
				xmlSerializable.ReadXml(xmlTextReader);
			}
			return (SqlDateTime)xmlSerializable;
		}

		public override string ConvertObjectToXml(object value)
		{
			StringWriter stringWriter = new StringWriter(base.FormatProvider);
			using (XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter))
			{
				((IXmlSerializable)value).WriteXml(xmlTextWriter);
			}
			return stringWriter.ToString();
		}

		protected override object GetEmptyStorage(int recordCount)
		{
			return new SqlDateTime[recordCount];
		}

		protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
		{
			((SqlDateTime[])store)[storeIndex] = this._values[record];
			nullbits.Set(record, this.IsNull(record));
		}

		protected override void SetStorage(object store, BitArray nullbits)
		{
			this._values = (SqlDateTime[])store;
		}

		private SqlDateTime[] _values;
	}
}
