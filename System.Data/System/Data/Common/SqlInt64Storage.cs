using System;
using System.Collections;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace System.Data.Common
{
	internal sealed class SqlInt64Storage : DataStorage
	{
		public SqlInt64Storage(DataColumn column)
			: base(column, typeof(SqlInt64), SqlInt64.Null, SqlInt64.Null, StorageType.SqlInt64)
		{
		}

		public override object Aggregate(int[] records, AggregateType kind)
		{
			bool flag = false;
			try
			{
				switch (kind)
				{
				case AggregateType.Sum:
				{
					SqlInt64 sqlInt = 0L;
					foreach (int num in records)
					{
						if (!this.IsNull(num))
						{
							sqlInt += this._values[num];
							flag = true;
						}
					}
					if (flag)
					{
						return sqlInt;
					}
					return this._nullValue;
				}
				case AggregateType.Mean:
				{
					SqlDecimal sqlDecimal = 0L;
					int num2 = 0;
					foreach (int num3 in records)
					{
						if (!this.IsNull(num3))
						{
							sqlDecimal += this._values[num3].ToSqlDecimal();
							num2++;
							flag = true;
						}
					}
					if (flag)
					{
						0L;
						return (sqlDecimal / (long)num2).ToSqlInt64();
					}
					return this._nullValue;
				}
				case AggregateType.Min:
				{
					SqlInt64 sqlInt2 = SqlInt64.MaxValue;
					foreach (int num4 in records)
					{
						if (!this.IsNull(num4))
						{
							if (SqlInt64.LessThan(this._values[num4], sqlInt2).IsTrue)
							{
								sqlInt2 = this._values[num4];
							}
							flag = true;
						}
					}
					if (flag)
					{
						return sqlInt2;
					}
					return this._nullValue;
				}
				case AggregateType.Max:
				{
					SqlInt64 sqlInt3 = SqlInt64.MinValue;
					foreach (int num5 in records)
					{
						if (!this.IsNull(num5))
						{
							if (SqlInt64.GreaterThan(this._values[num5], sqlInt3).IsTrue)
							{
								sqlInt3 = this._values[num5];
							}
							flag = true;
						}
					}
					if (flag)
					{
						return sqlInt3;
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
					int num6 = 0;
					for (int l = 0; l < records.Length; l++)
					{
						if (!this.IsNull(records[l]))
						{
							num6++;
						}
					}
					return num6;
				}
				case AggregateType.Var:
				case AggregateType.StDev:
				{
					int num6 = 0;
					SqlDouble sqlDouble = 0.0;
					0.0;
					SqlDouble sqlDouble2 = 0.0;
					SqlDouble sqlDouble3 = 0.0;
					foreach (int num7 in records)
					{
						if (!this.IsNull(num7))
						{
							sqlDouble2 += this._values[num7].ToSqlDouble();
							sqlDouble3 += this._values[num7].ToSqlDouble() * this._values[num7].ToSqlDouble();
							num6++;
						}
					}
					if (num6 <= 1)
					{
						return this._nullValue;
					}
					sqlDouble = (double)num6 * sqlDouble3 - sqlDouble2 * sqlDouble2;
					SqlBoolean sqlBoolean = sqlDouble / (sqlDouble2 * sqlDouble2) < 1E-15;
					if (sqlBoolean ? sqlBoolean : (sqlBoolean | (sqlDouble < 0.0)))
					{
						sqlDouble = 0.0;
					}
					else
					{
						sqlDouble /= (double)(num6 * (num6 - 1));
					}
					if (kind == AggregateType.StDev)
					{
						return Math.Sqrt(sqlDouble.Value);
					}
					return sqlDouble;
				}
				}
			}
			catch (OverflowException)
			{
				throw ExprException.Overflow(typeof(SqlInt64));
			}
			throw ExceptionBuilder.AggregateException(kind, this._dataType);
		}

		public override int Compare(int recordNo1, int recordNo2)
		{
			return this._values[recordNo1].CompareTo(this._values[recordNo2]);
		}

		public override int CompareValueTo(int recordNo, object value)
		{
			return this._values[recordNo].CompareTo((SqlInt64)value);
		}

		public override object ConvertValue(object value)
		{
			if (value != null)
			{
				return SqlConvert.ConvertToSqlInt64(value);
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
			this._values[record] = SqlConvert.ConvertToSqlInt64(value);
		}

		public override void SetCapacity(int capacity)
		{
			SqlInt64[] array = new SqlInt64[capacity];
			if (this._values != null)
			{
				Array.Copy(this._values, 0, array, 0, Math.Min(capacity, this._values.Length));
			}
			this._values = array;
		}

		public override object ConvertXmlToObject(string s)
		{
			SqlInt64 sqlInt = default(SqlInt64);
			TextReader textReader = new StringReader("<col>" + s + "</col>");
			IXmlSerializable xmlSerializable = sqlInt;
			using (XmlTextReader xmlTextReader = new XmlTextReader(textReader))
			{
				xmlSerializable.ReadXml(xmlTextReader);
			}
			return (SqlInt64)xmlSerializable;
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
			return new SqlInt64[recordCount];
		}

		protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
		{
			((SqlInt64[])store)[storeIndex] = this._values[record];
			nullbits.Set(storeIndex, this.IsNull(record));
		}

		protected override void SetStorage(object store, BitArray nullbits)
		{
			this._values = (SqlInt64[])store;
		}

		private SqlInt64[] _values;
	}
}
