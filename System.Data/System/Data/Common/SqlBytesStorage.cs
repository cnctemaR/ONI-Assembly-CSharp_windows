using System;
using System.Collections;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace System.Data.Common
{
	internal sealed class SqlBytesStorage : DataStorage
	{
		public SqlBytesStorage(DataColumn column)
			: base(column, typeof(SqlBytes), SqlBytes.Null, SqlBytes.Null, StorageType.SqlBytes)
		{
		}

		public override object Aggregate(int[] records, AggregateType kind)
		{
			try
			{
				if (kind != AggregateType.First)
				{
					if (kind == AggregateType.Count)
					{
						int num = 0;
						for (int i = 0; i < records.Length; i++)
						{
							if (!this.IsNull(records[i]))
							{
								num++;
							}
						}
						return num;
					}
				}
				else
				{
					if (records.Length != 0)
					{
						return this._values[records[0]];
					}
					return null;
				}
			}
			catch (OverflowException)
			{
				throw ExprException.Overflow(typeof(SqlBytes));
			}
			throw ExceptionBuilder.AggregateException(kind, this._dataType);
		}

		public override int Compare(int recordNo1, int recordNo2)
		{
			return 0;
		}

		public override int CompareValueTo(int recordNo, object value)
		{
			return 0;
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
			if (value == DBNull.Value || value == null)
			{
				this._values[record] = SqlBytes.Null;
				return;
			}
			this._values[record] = (SqlBytes)value;
		}

		public override void SetCapacity(int capacity)
		{
			SqlBytes[] array = new SqlBytes[capacity];
			if (this._values != null)
			{
				Array.Copy(this._values, 0, array, 0, Math.Min(capacity, this._values.Length));
			}
			this._values = array;
		}

		public override object ConvertXmlToObject(string s)
		{
			SqlBinary sqlBinary = default(SqlBinary);
			TextReader textReader = new StringReader("<col>" + s + "</col>");
			IXmlSerializable xmlSerializable = sqlBinary;
			using (XmlTextReader xmlTextReader = new XmlTextReader(textReader))
			{
				xmlSerializable.ReadXml(xmlTextReader);
			}
			return new SqlBytes((SqlBinary)xmlSerializable);
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
			return new SqlBytes[recordCount];
		}

		protected override void CopyValue(int record, object store, BitArray nullbits, int storeIndex)
		{
			((SqlBytes[])store)[storeIndex] = this._values[record];
			nullbits.Set(storeIndex, this.IsNull(record));
		}

		protected override void SetStorage(object store, BitArray nullbits)
		{
			this._values = (SqlBytes[])store;
		}

		private SqlBytes[] _values;
	}
}
