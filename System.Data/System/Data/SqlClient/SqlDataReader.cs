using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Globalization;
using System.IO;
using System.Text;
using Mono.Data.Tds.Protocol;

namespace System.Data.SqlClient
{
	public class SqlDataReader : DbDataReader, IDisposable, IDataReader, IDataRecord
	{
		internal SqlDataReader(SqlCommand command)
		{
			this.command = command;
			command.Tds.RecordsAffected = -1;
			this.NextResult();
		}

		public override int Depth
		{
			get
			{
				return 0;
			}
		}

		public override int FieldCount
		{
			get
			{
				this.ValidateState();
				return this.command.Tds.Columns.Count;
			}
		}

		public override bool IsClosed
		{
			get
			{
				return this.isClosed;
			}
		}

		public override object this[int i]
		{
			get
			{
				return this.GetValue(i);
			}
		}

		public override object this[string name]
		{
			get
			{
				return this.GetValue(this.GetOrdinal(name));
			}
		}

		public override int RecordsAffected
		{
			get
			{
				return this.command.Tds.RecordsAffected;
			}
		}

		public override bool HasRows
		{
			get
			{
				this.ValidateState();
				if (this.rowsRead > 0)
				{
					return true;
				}
				if (!this.haveRead)
				{
					this.readResult = this.ReadRecord();
				}
				return this.readResult;
			}
		}

		public override int VisibleFieldCount
		{
			get
			{
				return this.visibleFieldCount;
			}
		}

		protected SqlConnection Connection
		{
			get
			{
				return this.command.Connection;
			}
		}

		protected bool IsCommandBehavior(CommandBehavior condition)
		{
			return condition == this.command.CommandBehavior;
		}

		public override void Close()
		{
			if (this.IsClosed)
			{
				return;
			}
			while (this.NextResult())
			{
			}
			this.isClosed = true;
			this.command.CloseDataReader();
		}

		private static DataTable ConstructSchemaTable()
		{
			Type typeFromHandle = typeof(bool);
			Type typeFromHandle2 = typeof(string);
			Type typeFromHandle3 = typeof(int);
			Type typeFromHandle4 = typeof(Type);
			Type typeFromHandle5 = typeof(short);
			return new DataTable("SchemaTable")
			{
				Columns = 
				{
					{ "ColumnName", typeFromHandle2 },
					{ "ColumnOrdinal", typeFromHandle3 },
					{ "ColumnSize", typeFromHandle3 },
					{ "NumericPrecision", typeFromHandle5 },
					{ "NumericScale", typeFromHandle5 },
					{ "IsUnique", typeFromHandle },
					{ "IsKey", typeFromHandle },
					{ "BaseServerName", typeFromHandle2 },
					{ "BaseCatalogName", typeFromHandle2 },
					{ "BaseColumnName", typeFromHandle2 },
					{ "BaseSchemaName", typeFromHandle2 },
					{ "BaseTableName", typeFromHandle2 },
					{ "DataType", typeFromHandle4 },
					{ "AllowDBNull", typeFromHandle },
					{ "ProviderType", typeFromHandle3 },
					{ "IsAliased", typeFromHandle },
					{ "IsExpression", typeFromHandle },
					{ "IsIdentity", typeFromHandle },
					{ "IsAutoIncrement", typeFromHandle },
					{ "IsRowVersion", typeFromHandle },
					{ "IsHidden", typeFromHandle },
					{ "IsLong", typeFromHandle },
					{ "IsReadOnly", typeFromHandle },
					{ "ProviderSpecificDataType", typeFromHandle4 },
					{ "DataTypeName", typeFromHandle2 },
					{ "XmlSchemaCollectionDatabase", typeFromHandle2 },
					{ "XmlSchemaCollectionOwningSchema", typeFromHandle2 },
					{ "XmlSchemaCollectionName", typeFromHandle2 },
					{ "UdtAssemblyQualifiedName", typeFromHandle2 },
					{ "NonVersionedProviderType", typeFromHandle3 },
					{ "IsColumnSet", typeFromHandle }
				}
			};
		}

		private string GetSchemaRowTypeName(TdsColumnType ctype, int csize, short precision, short scale)
		{
			int num;
			Type type;
			bool flag;
			string text;
			this.GetSchemaRowType(ctype, csize, precision, scale, out num, out type, out flag, out text);
			return text;
		}

		private Type GetSchemaRowFieldType(TdsColumnType ctype, int csize, short precision, short scale)
		{
			int num;
			Type type;
			bool flag;
			string text;
			this.GetSchemaRowType(ctype, csize, precision, scale, out num, out type, out flag, out text);
			return type;
		}

		private SqlDbType GetSchemaRowDbType(int ordinal)
		{
			if (ordinal < 0 || ordinal >= this.command.Tds.Columns.Count)
			{
				throw new IndexOutOfRangeException();
			}
			TdsDataColumn tdsDataColumn = this.command.Tds.Columns[ordinal];
			TdsColumnType value = tdsDataColumn.ColumnType.Value;
			int value2 = tdsDataColumn.ColumnSize.Value;
			short? numericPrecision = tdsDataColumn.NumericPrecision;
			short num = ((numericPrecision == null) ? 0 : numericPrecision.Value);
			short? numericScale = tdsDataColumn.NumericScale;
			short num2 = ((numericScale == null) ? 0 : numericScale.Value);
			return this.GetSchemaRowDbType(value, value2, num, num2);
		}

		private SqlDbType GetSchemaRowDbType(TdsColumnType ctype, int csize, short precision, short scale)
		{
			int num;
			Type type;
			bool flag;
			string text;
			this.GetSchemaRowType(ctype, csize, precision, scale, out num, out type, out flag, out text);
			return (SqlDbType)num;
		}

		private void GetSchemaRowType(TdsColumnType ctype, int csize, short precision, short scale, out int dbType, out Type fieldType, out bool isLong, out string typeName)
		{
			dbType = -1;
			typeName = string.Empty;
			isLong = false;
			fieldType = typeof(Type);
			switch (ctype)
			{
			case TdsColumnType.Image:
				typeName = "image";
				dbType = 7;
				fieldType = typeof(byte[]);
				isLong = true;
				return;
			case TdsColumnType.Text:
				typeName = "text";
				dbType = 18;
				fieldType = typeof(string);
				isLong = true;
				return;
			case TdsColumnType.UniqueIdentifier:
				typeName = "uniqueidentifier";
				dbType = 14;
				fieldType = typeof(Guid);
				isLong = false;
				return;
			case TdsColumnType.VarBinary:
				goto IL_02B2;
			case TdsColumnType.IntN:
			case TdsColumnType.Int1:
			case TdsColumnType.Int2:
			case TdsColumnType.Int4:
				break;
			case TdsColumnType.VarChar:
				goto IL_02D5;
			default:
				switch (ctype)
				{
				case TdsColumnType.NText:
					typeName = "ntext";
					dbType = 11;
					fieldType = typeof(string);
					isLong = true;
					return;
				default:
					switch (ctype)
					{
					case TdsColumnType.BigVarBinary:
						goto IL_02B2;
					default:
						switch (ctype)
						{
						case TdsColumnType.BigBinary:
							goto IL_02F8;
						default:
							if (ctype == TdsColumnType.SmallMoney)
							{
								typeName = "smallmoney";
								dbType = 17;
								fieldType = typeof(decimal);
								isLong = false;
								return;
							}
							if (ctype != TdsColumnType.BigInt)
							{
								if (ctype != TdsColumnType.NChar)
								{
									typeName = "variant";
									dbType = 23;
									fieldType = typeof(object);
									isLong = false;
									return;
								}
								typeName = "nchar";
								dbType = 10;
								fieldType = typeof(string);
								isLong = false;
								return;
							}
							break;
						case TdsColumnType.BigChar:
							goto IL_031A;
						}
						break;
					case TdsColumnType.BigVarChar:
						goto IL_02D5;
					}
					break;
				case TdsColumnType.NVarChar:
					typeName = "nvarchar";
					dbType = 12;
					fieldType = typeof(string);
					isLong = false;
					return;
				case TdsColumnType.BitN:
					goto IL_033C;
				case TdsColumnType.Decimal:
				case TdsColumnType.Numeric:
					if (precision == 19 && scale == 0)
					{
						typeName = "bigint";
						dbType = 0;
						fieldType = typeof(long);
					}
					else
					{
						typeName = "decimal";
						dbType = 5;
						fieldType = typeof(decimal);
					}
					isLong = false;
					return;
				case TdsColumnType.FloatN:
					goto IL_01EB;
				case TdsColumnType.MoneyN:
				case TdsColumnType.Money4:
					goto IL_03BD;
				case TdsColumnType.DateTimeN:
					goto IL_035E;
				}
				break;
			case TdsColumnType.Binary:
				goto IL_02F8;
			case TdsColumnType.Char:
				goto IL_031A;
			case TdsColumnType.Bit:
				goto IL_033C;
			case TdsColumnType.DateTime4:
			case TdsColumnType.DateTime:
				goto IL_035E;
			case TdsColumnType.Real:
			case TdsColumnType.Float8:
				goto IL_01EB;
			case TdsColumnType.Money:
				goto IL_03BD;
			}
			switch (csize)
			{
			case 1:
				typeName = "tinyint";
				dbType = 20;
				fieldType = typeof(byte);
				isLong = false;
				break;
			case 2:
				typeName = "smallint";
				dbType = 16;
				fieldType = typeof(short);
				isLong = false;
				break;
			case 4:
				typeName = "int";
				dbType = 8;
				fieldType = typeof(int);
				isLong = false;
				break;
			case 8:
				typeName = "bigint";
				dbType = 0;
				fieldType = typeof(long);
				isLong = false;
				break;
			}
			return;
			IL_01EB:
			if (csize != 4)
			{
				if (csize == 8)
				{
					typeName = "float";
					dbType = 6;
					fieldType = typeof(double);
					isLong = false;
				}
			}
			else
			{
				typeName = "real";
				dbType = 13;
				fieldType = typeof(float);
				isLong = false;
			}
			return;
			IL_02B2:
			typeName = "varbinary";
			dbType = 21;
			fieldType = typeof(byte[]);
			isLong = false;
			return;
			IL_02D5:
			typeName = "varchar";
			dbType = 22;
			fieldType = typeof(string);
			isLong = false;
			return;
			IL_02F8:
			typeName = "binary";
			dbType = 1;
			fieldType = typeof(byte[]);
			isLong = false;
			return;
			IL_031A:
			typeName = "char";
			dbType = 3;
			fieldType = typeof(string);
			isLong = false;
			return;
			IL_033C:
			typeName = "bit";
			dbType = 2;
			fieldType = typeof(bool);
			isLong = false;
			return;
			IL_035E:
			if (csize != 4)
			{
				if (csize == 8)
				{
					typeName = "datetime";
					dbType = 4;
					fieldType = typeof(DateTime);
					isLong = false;
				}
			}
			else
			{
				typeName = "smalldatetime";
				dbType = 15;
				fieldType = typeof(DateTime);
				isLong = false;
			}
			return;
			IL_03BD:
			if (csize != 4)
			{
				if (csize == 8)
				{
					typeName = "money";
					dbType = 9;
					fieldType = typeof(decimal);
					isLong = false;
				}
			}
			else
			{
				typeName = "smallmoney";
				dbType = 17;
				fieldType = typeof(decimal);
				isLong = false;
			}
		}

		private new void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					if (this.schemaTable != null)
					{
						this.schemaTable.Dispose();
					}
					this.Close();
					this.command = null;
				}
				this.disposed = true;
			}
		}

		public override bool GetBoolean(int i)
		{
			object value = this.GetValue(i);
			if (value is bool)
			{
				return (bool)value;
			}
			if (value is DBNull)
			{
				throw new SqlNullValueException();
			}
			throw new InvalidCastException("Type is " + value.GetType().ToString());
		}

		public override byte GetByte(int i)
		{
			object value = this.GetValue(i);
			if (value is byte)
			{
				return (byte)value;
			}
			if (value is DBNull)
			{
				throw new SqlNullValueException();
			}
			throw new InvalidCastException("Type is " + value.GetType().ToString());
		}

		public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			if ((this.command.CommandBehavior & CommandBehavior.SequentialAccess) != CommandBehavior.Default)
			{
				this.ValidateState();
				this.EnsureDataAvailable();
				try
				{
					long sequentialColumnValue = this.command.Tds.GetSequentialColumnValue(i, dataIndex, buffer, bufferIndex, length);
					if (sequentialColumnValue == -1L)
					{
						throw this.CreateGetBytesOnInvalidColumnTypeException(i);
					}
					if (sequentialColumnValue == -2L)
					{
						throw new SqlNullValueException();
					}
					return sequentialColumnValue;
				}
				catch (TdsInternalException ex)
				{
					this.command.Connection.Close();
					throw SqlException.FromTdsInternalException(ex);
				}
			}
			object obj = this.GetValue(i);
			if (!(obj is byte[]))
			{
				SqlDbType schemaRowDbType = this.GetSchemaRowDbType(i);
				SqlDbType sqlDbType = schemaRowDbType;
				if (sqlDbType != SqlDbType.Image)
				{
					if (sqlDbType != SqlDbType.NText)
					{
						if (sqlDbType != SqlDbType.Text)
						{
							throw this.CreateGetBytesOnInvalidColumnTypeException(i);
						}
						string text = obj as string;
						if (text != null)
						{
							obj = Encoding.Default.GetBytes(text);
						}
						else
						{
							obj = null;
						}
					}
					else
					{
						string text2 = obj as string;
						if (text2 != null)
						{
							obj = Encoding.Unicode.GetBytes(text2);
						}
						else
						{
							obj = null;
						}
					}
				}
				else if (obj is DBNull)
				{
					throw new SqlNullValueException();
				}
			}
			if (buffer == null)
			{
				return (long)((byte[])obj).Length;
			}
			int num = (int)((long)((byte[])obj).Length - dataIndex);
			if (num < length)
			{
				length = num;
			}
			if (dataIndex < 0L)
			{
				return 0L;
			}
			Array.Copy((byte[])obj, (int)dataIndex, buffer, bufferIndex, length);
			return (long)length;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override char GetChar(int i)
		{
			throw new NotSupportedException();
		}

		public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			if ((this.command.CommandBehavior & CommandBehavior.SequentialAccess) != CommandBehavior.Default)
			{
				this.ValidateState();
				this.EnsureDataAvailable();
				if (i < 0 || i >= this.command.Tds.Columns.Count)
				{
					throw new IndexOutOfRangeException();
				}
				byte b = 1;
				TdsColumnType tdsColumnType = (TdsColumnType)((int)this.command.Tds.Columns[i]["ColumnType"]);
				TdsColumnType tdsColumnType2 = tdsColumnType;
				Encoding encoding;
				if (tdsColumnType2 != TdsColumnType.Text && tdsColumnType2 != TdsColumnType.VarChar && tdsColumnType2 != TdsColumnType.Char)
				{
					if (tdsColumnType2 != TdsColumnType.NText && tdsColumnType2 != TdsColumnType.NVarChar)
					{
						if (tdsColumnType2 == TdsColumnType.BigVarChar)
						{
							goto IL_00BE;
						}
						if (tdsColumnType2 != TdsColumnType.NChar)
						{
							return -1L;
						}
					}
					encoding = Encoding.Unicode;
					b = 2;
					goto IL_00D9;
				}
				IL_00BE:
				encoding = Encoding.ASCII;
				IL_00D9:
				long num;
				if (buffer == null)
				{
					num = this.GetBytes(i, 0L, null, 0, 0);
					return num / (long)b;
				}
				length *= (int)b;
				byte[] array = new byte[length];
				num = this.GetBytes(i, dataIndex, array, 0, length);
				if (num == -1L)
				{
					throw new InvalidCastException("Specified cast is not valid");
				}
				char[] chars = encoding.GetChars(array, 0, (int)num);
				chars.CopyTo(buffer, bufferIndex);
				return (long)chars.Length;
			}
			else
			{
				object value = this.GetValue(i);
				char[] array2;
				if (value is char[])
				{
					array2 = (char[])value;
				}
				else if (value is string)
				{
					array2 = ((string)value).ToCharArray();
				}
				else
				{
					if (value is DBNull)
					{
						throw new SqlNullValueException();
					}
					throw new InvalidCastException("Type is " + value.GetType().ToString());
				}
				if (buffer == null)
				{
					return (long)array2.Length;
				}
				Array.Copy(array2, (int)dataIndex, buffer, bufferIndex, length);
				return (long)array2.Length - dataIndex;
			}
		}

		public override string GetDataTypeName(int i)
		{
			this.ValidateState();
			if (i < 0 || i >= this.command.Tds.Columns.Count)
			{
				throw new IndexOutOfRangeException();
			}
			TdsDataColumn tdsDataColumn = this.command.Tds.Columns[i];
			TdsColumnType value = tdsDataColumn.ColumnType.Value;
			int value2 = tdsDataColumn.ColumnSize.Value;
			short? numericPrecision = tdsDataColumn.NumericPrecision;
			short num = ((numericPrecision == null) ? 0 : numericPrecision.Value);
			short? numericScale = tdsDataColumn.NumericScale;
			short num2 = ((numericScale == null) ? 0 : numericScale.Value);
			return this.GetSchemaRowTypeName(value, value2, num, num2);
		}

		public override DateTime GetDateTime(int i)
		{
			object value = this.GetValue(i);
			if (value is DateTime)
			{
				return (DateTime)value;
			}
			if (value is DBNull)
			{
				throw new SqlNullValueException();
			}
			throw new InvalidCastException("Type is " + value.GetType().ToString());
		}

		public override decimal GetDecimal(int i)
		{
			object value = this.GetValue(i);
			if (value is decimal)
			{
				return (decimal)value;
			}
			if (value is DBNull)
			{
				throw new SqlNullValueException();
			}
			throw new InvalidCastException("Type is " + value.GetType().ToString());
		}

		public override double GetDouble(int i)
		{
			object value = this.GetValue(i);
			if (value is double)
			{
				return (double)value;
			}
			if (value is DBNull)
			{
				throw new SqlNullValueException();
			}
			throw new InvalidCastException("Type is " + value.GetType().ToString());
		}

		public override Type GetFieldType(int i)
		{
			this.ValidateState();
			if (i < 0 || i >= this.command.Tds.Columns.Count)
			{
				throw new IndexOutOfRangeException();
			}
			TdsDataColumn tdsDataColumn = this.command.Tds.Columns[i];
			TdsColumnType value = tdsDataColumn.ColumnType.Value;
			int value2 = tdsDataColumn.ColumnSize.Value;
			short? numericPrecision = tdsDataColumn.NumericPrecision;
			short num = ((numericPrecision == null) ? 0 : numericPrecision.Value);
			short? numericScale = tdsDataColumn.NumericScale;
			short num2 = ((numericScale == null) ? 0 : numericScale.Value);
			return this.GetSchemaRowFieldType(value, value2, num, num2);
		}

		public override float GetFloat(int i)
		{
			object value = this.GetValue(i);
			if (value is float)
			{
				return (float)value;
			}
			if (value is DBNull)
			{
				throw new SqlNullValueException();
			}
			throw new InvalidCastException("Type is " + value.GetType().ToString());
		}

		public override Guid GetGuid(int i)
		{
			object value = this.GetValue(i);
			if (value is Guid)
			{
				return (Guid)value;
			}
			if (value is DBNull)
			{
				throw new SqlNullValueException();
			}
			throw new InvalidCastException("Type is " + value.GetType().ToString());
		}

		public override short GetInt16(int i)
		{
			object value = this.GetValue(i);
			if (value is short)
			{
				return (short)value;
			}
			if (value is DBNull)
			{
				throw new SqlNullValueException();
			}
			throw new InvalidCastException("Type is " + value.GetType().ToString());
		}

		public override int GetInt32(int i)
		{
			object value = this.GetValue(i);
			if (value is int)
			{
				return (int)value;
			}
			if (value is DBNull)
			{
				throw new SqlNullValueException();
			}
			throw new InvalidCastException("Type is " + value.GetType().ToString());
		}

		public override long GetInt64(int i)
		{
			object value = this.GetValue(i);
			if (value is long)
			{
				return (long)value;
			}
			if (value is DBNull)
			{
				throw new SqlNullValueException();
			}
			throw new InvalidCastException("Type is " + value.GetType().ToString());
		}

		public override string GetName(int i)
		{
			this.ValidateState();
			if (i < 0 || i >= this.command.Tds.Columns.Count)
			{
				throw new IndexOutOfRangeException();
			}
			return this.command.Tds.Columns[i].ColumnName;
		}

		public override int GetOrdinal(string name)
		{
			this.ValidateState();
			if (name == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			foreach (object obj in this.command.Tds.Columns)
			{
				TdsDataColumn tdsDataColumn = (TdsDataColumn)obj;
				string columnName = tdsDataColumn.ColumnName;
				if (columnName.Equals(name) || string.Compare(columnName, name, true) == 0)
				{
					return tdsDataColumn.ColumnOrdinal.Value;
				}
			}
			throw new IndexOutOfRangeException();
		}

		public override DataTable GetSchemaTable()
		{
			this.ValidateState();
			if (this.schemaTable == null)
			{
				this.schemaTable = SqlDataReader.ConstructSchemaTable();
			}
			if (this.schemaTable.Rows != null && this.schemaTable.Rows.Count > 0)
			{
				return this.schemaTable;
			}
			if (!this.moreResults)
			{
				return null;
			}
			foreach (object obj in this.command.Tds.Columns)
			{
				TdsDataColumn tdsDataColumn = (TdsDataColumn)obj;
				DataRow dataRow = this.schemaTable.NewRow();
				dataRow[0] = SqlDataReader.GetSchemaValue(tdsDataColumn.ColumnName);
				dataRow[1] = SqlDataReader.GetSchemaValue(tdsDataColumn.ColumnOrdinal);
				dataRow[5] = SqlDataReader.GetSchemaValue(tdsDataColumn.IsUnique);
				dataRow[18] = SqlDataReader.GetSchemaValue(tdsDataColumn.IsAutoIncrement);
				dataRow[19] = SqlDataReader.GetSchemaValue(tdsDataColumn.IsRowVersion);
				dataRow[20] = SqlDataReader.GetSchemaValue(tdsDataColumn.IsHidden);
				dataRow[17] = SqlDataReader.GetSchemaValue(tdsDataColumn.IsIdentity);
				dataRow[3] = SqlDataReader.GetSchemaValue(tdsDataColumn.NumericPrecision);
				dataRow[6] = SqlDataReader.GetSchemaValue(tdsDataColumn.IsKey);
				dataRow[15] = SqlDataReader.GetSchemaValue(tdsDataColumn.IsAliased);
				dataRow[16] = SqlDataReader.GetSchemaValue(tdsDataColumn.IsExpression);
				dataRow[22] = SqlDataReader.GetSchemaValue(tdsDataColumn.IsReadOnly);
				dataRow[7] = SqlDataReader.GetSchemaValue(tdsDataColumn.BaseServerName);
				dataRow[8] = SqlDataReader.GetSchemaValue(tdsDataColumn.BaseCatalogName);
				dataRow[9] = SqlDataReader.GetSchemaValue(tdsDataColumn.BaseColumnName);
				dataRow[10] = SqlDataReader.GetSchemaValue(tdsDataColumn.BaseSchemaName);
				dataRow[11] = SqlDataReader.GetSchemaValue(tdsDataColumn.BaseTableName);
				dataRow[13] = SqlDataReader.GetSchemaValue(tdsDataColumn.AllowDBNull);
				dataRow[23] = DBNull.Value;
				dataRow[24] = SqlDataReader.GetSchemaValue(tdsDataColumn.DataTypeName);
				dataRow[25] = DBNull.Value;
				dataRow[26] = DBNull.Value;
				dataRow[27] = DBNull.Value;
				dataRow[28] = DBNull.Value;
				dataRow[29] = DBNull.Value;
				dataRow[30] = DBNull.Value;
				if (dataRow[9] == DBNull.Value)
				{
					dataRow[9] = dataRow[0];
				}
				TdsColumnType value = tdsDataColumn.ColumnType.Value;
				int value2 = tdsDataColumn.ColumnSize.Value;
				short num = (short)SqlDataReader.GetSchemaValue(tdsDataColumn.NumericPrecision);
				short num2 = (short)SqlDataReader.GetSchemaValue(tdsDataColumn.NumericScale);
				int num3;
				Type type;
				bool flag;
				string text;
				this.GetSchemaRowType(value, value2, num, num2, out num3, out type, out flag, out text);
				dataRow[2] = value2;
				dataRow[3] = num;
				dataRow[4] = num2;
				dataRow[14] = num3;
				dataRow[12] = type;
				dataRow[21] = flag;
				if (!(bool)dataRow[20])
				{
					this.visibleFieldCount++;
				}
				this.schemaTable.Rows.Add(dataRow);
			}
			return this.schemaTable;
		}

		private static object GetSchemaValue(TdsDataColumn schema, string key)
		{
			object obj = schema[key];
			if (obj != null)
			{
				return obj;
			}
			return DBNull.Value;
		}

		private static object GetSchemaValue(object value)
		{
			if (value == null)
			{
				return DBNull.Value;
			}
			return value;
		}

		public virtual SqlBinary GetSqlBinary(int i)
		{
			object sqlValue = this.GetSqlValue(i);
			if (!(sqlValue is SqlBinary))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlBinary)sqlValue;
		}

		public virtual SqlBoolean GetSqlBoolean(int i)
		{
			object sqlValue = this.GetSqlValue(i);
			if (!(sqlValue is SqlBoolean))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlBoolean)sqlValue;
		}

		public virtual SqlByte GetSqlByte(int i)
		{
			object sqlValue = this.GetSqlValue(i);
			if (!(sqlValue is SqlByte))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlByte)sqlValue;
		}

		public virtual SqlDateTime GetSqlDateTime(int i)
		{
			object sqlValue = this.GetSqlValue(i);
			if (!(sqlValue is SqlDateTime))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlDateTime)sqlValue;
		}

		public virtual SqlDecimal GetSqlDecimal(int i)
		{
			object sqlValue = this.GetSqlValue(i);
			if (!(sqlValue is SqlDecimal))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlDecimal)sqlValue;
		}

		public virtual SqlDouble GetSqlDouble(int i)
		{
			object sqlValue = this.GetSqlValue(i);
			if (!(sqlValue is SqlDouble))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlDouble)sqlValue;
		}

		public virtual SqlGuid GetSqlGuid(int i)
		{
			object sqlValue = this.GetSqlValue(i);
			if (!(sqlValue is SqlGuid))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlGuid)sqlValue;
		}

		public virtual SqlInt16 GetSqlInt16(int i)
		{
			object sqlValue = this.GetSqlValue(i);
			if (!(sqlValue is SqlInt16))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlInt16)sqlValue;
		}

		public virtual SqlInt32 GetSqlInt32(int i)
		{
			object sqlValue = this.GetSqlValue(i);
			if (!(sqlValue is SqlInt32))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlInt32)sqlValue;
		}

		public virtual SqlInt64 GetSqlInt64(int i)
		{
			object sqlValue = this.GetSqlValue(i);
			if (!(sqlValue is SqlInt64))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlInt64)sqlValue;
		}

		public virtual SqlMoney GetSqlMoney(int i)
		{
			object sqlValue = this.GetSqlValue(i);
			if (!(sqlValue is SqlMoney))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlMoney)sqlValue;
		}

		public virtual SqlSingle GetSqlSingle(int i)
		{
			object sqlValue = this.GetSqlValue(i);
			if (!(sqlValue is SqlSingle))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlSingle)sqlValue;
		}

		public virtual SqlString GetSqlString(int i)
		{
			object sqlValue = this.GetSqlValue(i);
			if (!(sqlValue is SqlString))
			{
				throw new InvalidCastException("Type is " + sqlValue.GetType().ToString());
			}
			return (SqlString)sqlValue;
		}

		public virtual SqlXml GetSqlXml(int i)
		{
			object obj = this.GetSqlValue(i);
			if (!(obj is SqlXml))
			{
				if (obj is DBNull)
				{
					throw new SqlNullValueException();
				}
				if (this.command.Tds.TdsVersion > TdsVersion.tds80 || !(obj is SqlString))
				{
					throw new InvalidCastException("Type is " + obj.GetType().ToString());
				}
				MemoryStream memoryStream = null;
				if (!((SqlString)obj).IsNull)
				{
					memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(obj.ToString()));
				}
				obj = new SqlXml(memoryStream);
			}
			return (SqlXml)obj;
		}

		public virtual object GetSqlValue(int i)
		{
			object value = this.GetValue(i);
			switch (this.GetSchemaRowDbType(i))
			{
			case SqlDbType.BigInt:
				if (value == DBNull.Value)
				{
					return SqlInt64.Null;
				}
				return (long)value;
			case SqlDbType.Binary:
			case SqlDbType.Image:
			case SqlDbType.Timestamp:
			case SqlDbType.VarBinary:
				if (value == DBNull.Value)
				{
					return SqlBinary.Null;
				}
				return (byte[])value;
			case SqlDbType.Bit:
				if (value == DBNull.Value)
				{
					return SqlBoolean.Null;
				}
				return (bool)value;
			case SqlDbType.Char:
			case SqlDbType.NChar:
			case SqlDbType.NText:
			case SqlDbType.NVarChar:
			case SqlDbType.Text:
			case SqlDbType.VarChar:
				if (value == DBNull.Value)
				{
					return SqlString.Null;
				}
				return (string)value;
			case SqlDbType.DateTime:
			case SqlDbType.SmallDateTime:
				if (value == DBNull.Value)
				{
					return SqlDateTime.Null;
				}
				return (DateTime)value;
			case SqlDbType.Decimal:
				if (value == DBNull.Value)
				{
					return SqlDecimal.Null;
				}
				if (value is TdsBigDecimal)
				{
					return SqlDecimal.FromTdsBigDecimal((TdsBigDecimal)value);
				}
				return (decimal)value;
			case SqlDbType.Float:
				if (value == DBNull.Value)
				{
					return SqlDouble.Null;
				}
				return (double)value;
			case SqlDbType.Int:
				if (value == DBNull.Value)
				{
					return SqlInt32.Null;
				}
				return (int)value;
			case SqlDbType.Money:
			case SqlDbType.SmallMoney:
				if (value == DBNull.Value)
				{
					return SqlMoney.Null;
				}
				return (decimal)value;
			case SqlDbType.Real:
				if (value == DBNull.Value)
				{
					return SqlSingle.Null;
				}
				return (float)value;
			case SqlDbType.UniqueIdentifier:
				if (value == DBNull.Value)
				{
					return SqlGuid.Null;
				}
				return (Guid)value;
			case SqlDbType.SmallInt:
				if (value == DBNull.Value)
				{
					return SqlInt16.Null;
				}
				return (short)value;
			case SqlDbType.TinyInt:
				if (value == DBNull.Value)
				{
					return SqlByte.Null;
				}
				return (byte)value;
			case SqlDbType.Xml:
				if (value == DBNull.Value)
				{
					return SqlByte.Null;
				}
				return (SqlXml)value;
			}
			throw new InvalidOperationException("The type of this column is unknown.");
		}

		public virtual int GetSqlValues(object[] values)
		{
			this.ValidateState();
			this.EnsureDataAvailable();
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			int count = this.command.Tds.Columns.Count;
			int num = values.Length;
			int num2;
			if (num > count)
			{
				num2 = count;
			}
			else
			{
				num2 = num;
			}
			for (int i = 0; i < num2; i++)
			{
				values[i] = this.GetSqlValue(i);
			}
			return num2;
		}

		public override string GetString(int i)
		{
			object value = this.GetValue(i);
			if (value is string)
			{
				return (string)value;
			}
			if (value is DBNull)
			{
				throw new SqlNullValueException();
			}
			throw new InvalidCastException("Type is " + value.GetType().ToString());
		}

		public override object GetValue(int i)
		{
			this.ValidateState();
			this.EnsureDataAvailable();
			if (i < 0 || i >= this.command.Tds.Columns.Count)
			{
				throw new IndexOutOfRangeException();
			}
			try
			{
				if ((this.command.CommandBehavior & CommandBehavior.SequentialAccess) != CommandBehavior.Default)
				{
					return this.command.Tds.GetSequentialColumnValue(i);
				}
			}
			catch (TdsInternalException ex)
			{
				this.command.Connection.Close();
				throw SqlException.FromTdsInternalException(ex);
			}
			return this.command.Tds.ColumnValues[i];
		}

		public override int GetValues(object[] values)
		{
			this.ValidateState();
			this.EnsureDataAvailable();
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			int num = values.Length;
			int bigDecimalIndex = this.command.Tds.ColumnValues.BigDecimalIndex;
			if (bigDecimalIndex >= 0 && bigDecimalIndex < num)
			{
				throw new OverflowException();
			}
			try
			{
				this.command.Tds.ColumnValues.CopyTo(0, values, 0, (num <= this.command.Tds.ColumnValues.Count) ? num : this.command.Tds.ColumnValues.Count);
			}
			catch (TdsInternalException ex)
			{
				this.command.Connection.Close();
				throw SqlException.FromTdsInternalException(ex);
			}
			return (num >= this.FieldCount) ? this.FieldCount : num;
		}

		public override IEnumerator GetEnumerator()
		{
			return new DbEnumerator(this);
		}

		public override bool IsDBNull(int i)
		{
			return this.GetValue(i) == DBNull.Value;
		}

		public override bool NextResult()
		{
			this.ValidateState();
			if ((this.command.CommandBehavior & CommandBehavior.SingleResult) != CommandBehavior.Default && this.resultsRead > 0)
			{
				this.moreResults = false;
				this.rowsRead = 0;
				this.haveRead = false;
				return false;
			}
			try
			{
				this.moreResults = this.command.Tds.NextResult();
			}
			catch (TdsInternalException ex)
			{
				this.command.Connection.Close();
				throw SqlException.FromTdsInternalException(ex);
			}
			if (!this.moreResults)
			{
				this.command.GetOutputParameters();
			}
			else
			{
				this.schemaTable = null;
			}
			this.rowsRead = 0;
			this.haveRead = false;
			this.resultsRead++;
			return this.moreResults;
		}

		public override bool Read()
		{
			this.ValidateState();
			if (!this.haveRead || this.readResultUsed)
			{
				this.readResult = this.ReadRecord();
			}
			this.readResultUsed = true;
			return this.readResult;
		}

		internal bool ReadRecord()
		{
			this.readResultUsed = false;
			if ((this.command.CommandBehavior & CommandBehavior.SingleRow) != CommandBehavior.Default && this.haveRead)
			{
				return false;
			}
			if ((this.command.CommandBehavior & CommandBehavior.SchemaOnly) != CommandBehavior.Default)
			{
				return false;
			}
			if (!this.moreResults)
			{
				return false;
			}
			bool flag2;
			try
			{
				bool flag = this.command.Tds.NextRow();
				if (flag)
				{
					this.rowsRead++;
				}
				this.haveRead = true;
				flag2 = flag;
			}
			catch (TdsInternalException ex)
			{
				this.command.Connection.Close();
				throw SqlException.FromTdsInternalException(ex);
			}
			return flag2;
		}

		private void ValidateState()
		{
			if (this.IsClosed)
			{
				throw new InvalidOperationException("Invalid attempt to read data when reader is closed");
			}
		}

		private void EnsureDataAvailable()
		{
			if (!this.readResult || !this.haveRead || !this.readResultUsed)
			{
				throw new InvalidOperationException("No data available.");
			}
		}

		private InvalidCastException CreateGetBytesOnInvalidColumnTypeException(int ordinal)
		{
			string text = string.Format(CultureInfo.InvariantCulture, "Invalid attempt to GetBytes on column '{0}'.The GetBytes function can only be used on columns of type Text, NText, or Image.", new object[] { this.GetName(ordinal) });
			return new InvalidCastException(text);
		}

		public override Type GetProviderSpecificFieldType(int i)
		{
			return this.GetSqlValue(i).GetType();
		}

		public override object GetProviderSpecificValue(int i)
		{
			return this.GetSqlValue(i);
		}

		public override int GetProviderSpecificValues(object[] values)
		{
			return this.GetSqlValues(values);
		}

		public virtual SqlBytes GetSqlBytes(int i)
		{
			byte[] array = (byte[])this.GetValue(i);
			return new SqlBytes(array);
		}

		private const int COLUMN_NAME_IDX = 0;

		private const int COLUMN_ORDINAL_IDX = 1;

		private const int COLUMN_SIZE_IDX = 2;

		private const int NUMERIC_PRECISION_IDX = 3;

		private const int NUMERIC_SCALE_IDX = 4;

		private const int IS_UNIQUE_IDX = 5;

		private const int IS_KEY_IDX = 6;

		private const int BASE_SERVER_NAME_IDX = 7;

		private const int BASE_CATALOG_NAME_IDX = 8;

		private const int BASE_COLUMN_NAME_IDX = 9;

		private const int BASE_SCHEMA_NAME_IDX = 10;

		private const int BASE_TABLE_NAME_IDX = 11;

		private const int DATA_TYPE_IDX = 12;

		private const int ALLOW_DBNULL_IDX = 13;

		private const int PROVIDER_TYPE_IDX = 14;

		private const int IS_ALIASED_IDX = 15;

		private const int IS_EXPRESSION_IDX = 16;

		private const int IS_IDENTITY_IDX = 17;

		private const int IS_AUTO_INCREMENT_IDX = 18;

		private const int IS_ROW_VERSION_IDX = 19;

		private const int IS_HIDDEN_IDX = 20;

		private const int IS_LONG_IDX = 21;

		private const int IS_READ_ONLY_IDX = 22;

		private const int PROVIDER_SPECIFIC_TYPE_IDX = 23;

		private const int DATA_TYPE_NAME_IDX = 24;

		private const int XML_SCHEMA_COLLCTN_DB_IDX = 25;

		private const int XML_SCHEMA_COLLCTN_OWN_SCHEMA_IDX = 26;

		private const int XML_SCHEMA_COLLCTN_NAME_IDX = 27;

		private const int UDT_ASMBLY_QUALIFIED_NAME_IDX = 28;

		private const int NON_VER_PROVIDER_TYPE_IDX = 29;

		private const int IS_COLUMN_SET = 30;

		private SqlCommand command;

		private bool disposed;

		private bool isClosed;

		private bool moreResults;

		private int resultsRead;

		private int rowsRead;

		private DataTable schemaTable;

		private bool haveRead;

		private bool readResult;

		private bool readResultUsed;

		private int visibleFieldCount;
	}
}
