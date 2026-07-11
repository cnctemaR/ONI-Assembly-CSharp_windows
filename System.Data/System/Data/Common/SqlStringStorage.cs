using System;
using System.Collections;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace System.Data.Common
{
	internal sealed class SqlStringStorage : DataStorage
	{
		public SqlStringStorage(DataColumn column)
			: base(column, typeof(SqlString), SqlString.Null, SqlString.Null, StorageType.SqlString)
		{
		}

		public override object Aggregate(int[] recordNos, AggregateType kind)
		{
			try
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
						if (!this.IsNull(recordNos[i]))
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
				throw ExprException.Overflow(typeof(SqlString));
			}
			throw ExceptionBuilder.AggregateException(kind, this._dataType);
		}

		public override int Compare(int recordNo1, int recordNo2)
		{
			return this.Compare(this._values[recordNo1], this._values[recordNo2]);
		}

		public int Compare(SqlString valueNo1, SqlString valueNo2)
		{
			if (valueNo1.IsNull && valueNo2.IsNull)
			{
				return 0;
			}
			if (valueNo1.IsNull)
			{
				return -1;
			}
			if (valueNo2.IsNull)
			{
				return 1;
			}
			return this._table.Compare(valueNo1.Value, valueNo2.Value);
		}

		public override int CompareValueTo(int recordNo, object value)
		{
			return this.Compare(this._values[recordNo], (SqlString)value);
		}

		public override object ConvertValue(object value)
		{
			if (value != null)
			{
				return SqlConvert.ConvertToSqlString(value);
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

		public override int GetStringLength(int record)
		{
			SqlString sqlString = this._values[record];
			if (!sqlString.IsNull)
			{
				return sqlString.Value.Length;
			}
			return 0;
		}

		public override bool IsNull(int record)
		{
			return this._values[record].IsNull;
		}

		public override void Set(int record, object value)
		{
			this._values[record] = SqlConvert.ConvertToSqlString(value);
		}

		public override void SetCapacity(int capacity)
		{
			SqlString[] array = new SqlString[capacity];
			if (this._values != null)
			{
				Array.Copy(this._values, 0, array, 0, Math.Min(capacity, this._values.Length));
			}
			this._values = array;
		}

		public override object ConvertXmlToObject(string s)
		{
			SqlString sqlString = default(SqlString);
			TextReader textReader = new StringReader("<col>" + s + "</col>");
			IXmlSerializable xmlSerializable = sqlString;
			using (XmlTextReader xmlTextReader = new XmlTextReader(textReader))
			{
				xmlSerializable.ReadXml(xmlTextReader);
			}
			return (SqlString)xmlSerializable;
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
			return new SqlString[recordCount];
		}

		protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
		{
			((SqlString[])store)[storeIndex] = this._values[record];
			nullbits.Set(storeIndex, this.IsNull(record));
		}

		protected override void SetStorage(object store, BitArray nullbits)
		{
			this._values = (SqlString[])store;
		}

		private SqlString[] _values;
	}
}
