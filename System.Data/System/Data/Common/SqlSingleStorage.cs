using System;
using System.Collections;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace System.Data.Common
{
	internal sealed class SqlSingleStorage : DataStorage
	{
		public SqlSingleStorage(DataColumn column)
			: base(column, typeof(SqlSingle), SqlSingle.Null, SqlSingle.Null, StorageType.SqlSingle)
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
					SqlSingle sqlSingle = 0f;
					foreach (int num in records)
					{
						if (!this.IsNull(num))
						{
							sqlSingle += this._values[num];
							flag = true;
						}
					}
					if (flag)
					{
						return sqlSingle;
					}
					return this._nullValue;
				}
				case AggregateType.Mean:
				{
					SqlDouble sqlDouble = 0.0;
					int num2 = 0;
					foreach (int num3 in records)
					{
						if (!this.IsNull(num3))
						{
							sqlDouble += this._values[num3].ToSqlDouble();
							num2++;
							flag = true;
						}
					}
					if (flag)
					{
						0f;
						return (sqlDouble / (double)num2).ToSqlSingle();
					}
					return this._nullValue;
				}
				case AggregateType.Min:
				{
					SqlSingle sqlSingle2 = SqlSingle.MaxValue;
					foreach (int num4 in records)
					{
						if (!this.IsNull(num4))
						{
							if (SqlSingle.LessThan(this._values[num4], sqlSingle2).IsTrue)
							{
								sqlSingle2 = this._values[num4];
							}
							flag = true;
						}
					}
					if (flag)
					{
						return sqlSingle2;
					}
					return this._nullValue;
				}
				case AggregateType.Max:
				{
					SqlSingle sqlSingle3 = SqlSingle.MinValue;
					foreach (int num5 in records)
					{
						if (!this.IsNull(num5))
						{
							if (SqlSingle.GreaterThan(this._values[num5], sqlSingle3).IsTrue)
							{
								sqlSingle3 = this._values[num5];
							}
							flag = true;
						}
					}
					if (flag)
					{
						return sqlSingle3;
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
					SqlDouble sqlDouble2 = 0.0;
					0.0;
					SqlDouble sqlDouble3 = 0.0;
					SqlDouble sqlDouble4 = 0.0;
					foreach (int num7 in records)
					{
						if (!this.IsNull(num7))
						{
							sqlDouble3 += this._values[num7].ToSqlDouble();
							sqlDouble4 += this._values[num7].ToSqlDouble() * this._values[num7].ToSqlDouble();
							num6++;
						}
					}
					if (num6 <= 1)
					{
						return this._nullValue;
					}
					sqlDouble2 = (double)num6 * sqlDouble4 - sqlDouble3 * sqlDouble3;
					SqlBoolean sqlBoolean = sqlDouble2 / (sqlDouble3 * sqlDouble3) < 1E-15;
					if (sqlBoolean ? sqlBoolean : (sqlBoolean | (sqlDouble2 < 0.0)))
					{
						sqlDouble2 = 0.0;
					}
					else
					{
						sqlDouble2 /= (double)(num6 * (num6 - 1));
					}
					if (kind == AggregateType.StDev)
					{
						return Math.Sqrt(sqlDouble2.Value);
					}
					return sqlDouble2;
				}
				}
			}
			catch (OverflowException)
			{
				throw ExprException.Overflow(typeof(SqlSingle));
			}
			throw ExceptionBuilder.AggregateException(kind, this._dataType);
		}

		public override int Compare(int recordNo1, int recordNo2)
		{
			return this._values[recordNo1].CompareTo(this._values[recordNo2]);
		}

		public override int CompareValueTo(int recordNo, object value)
		{
			return this._values[recordNo].CompareTo((SqlSingle)value);
		}

		public override object ConvertValue(object value)
		{
			if (value != null)
			{
				return SqlConvert.ConvertToSqlSingle(value);
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
			this._values[record] = SqlConvert.ConvertToSqlSingle(value);
		}

		public override void SetCapacity(int capacity)
		{
			SqlSingle[] array = new SqlSingle[capacity];
			if (this._values != null)
			{
				Array.Copy(this._values, 0, array, 0, Math.Min(capacity, this._values.Length));
			}
			this._values = array;
		}

		public override object ConvertXmlToObject(string s)
		{
			SqlSingle sqlSingle = default(SqlSingle);
			TextReader textReader = new StringReader("<col>" + s + "</col>");
			IXmlSerializable xmlSerializable = sqlSingle;
			using (XmlTextReader xmlTextReader = new XmlTextReader(textReader))
			{
				xmlSerializable.ReadXml(xmlTextReader);
			}
			return (SqlSingle)xmlSerializable;
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
			return new SqlSingle[recordCount];
		}

		protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
		{
			((SqlSingle[])store)[storeIndex] = this._values[record];
			nullbits.Set(storeIndex, this.IsNull(record));
		}

		protected override void SetStorage(object store, BitArray nullbits)
		{
			this._values = (SqlSingle[])store;
		}

		private SqlSingle[] _values;
	}
}
