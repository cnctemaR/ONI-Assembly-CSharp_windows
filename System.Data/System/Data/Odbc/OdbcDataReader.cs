using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Text;

namespace System.Data.Odbc
{
	public sealed class OdbcDataReader : DbDataReader
	{
		internal OdbcDataReader(OdbcCommand command, CommandBehavior behavior)
		{
			this.command = command;
			this.CommandBehavior = behavior;
			this.open = true;
			this.currentRow = -1;
			this.hstmt = command.hStmt;
			short num = 0;
			libodbc.SQLNumResultCols(this.hstmt, ref num);
			this.cols = new OdbcColumn[(int)num];
			this.GetColumns();
		}

		internal OdbcDataReader(OdbcCommand command, CommandBehavior behavior, int recordAffected)
			: this(command, behavior)
		{
			this._recordsAffected = recordAffected;
		}

		private CommandBehavior CommandBehavior
		{
			get
			{
				return this.behavior;
			}
			set
			{
				this.behavior = value;
			}
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
				if (this.IsClosed)
				{
					throw new InvalidOperationException("The reader is closed.");
				}
				return this.cols.Length;
			}
		}

		public override bool IsClosed
		{
			get
			{
				return !this.open;
			}
		}

		public override object this[string value]
		{
			get
			{
				int ordinal = this.GetOrdinal(value);
				return this[ordinal];
			}
		}

		public override object this[int i]
		{
			get
			{
				return this.GetValue(i);
			}
		}

		public override int RecordsAffected
		{
			get
			{
				return this._recordsAffected;
			}
		}

		[MonoTODO]
		public override bool HasRows
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		private OdbcConnection Connection
		{
			get
			{
				if (this.command != null)
				{
					return this.command.Connection;
				}
				return null;
			}
		}

		private int ColIndex(string colname)
		{
			int num = 0;
			foreach (OdbcColumn odbcColumn in this.cols)
			{
				if (odbcColumn != null)
				{
					if (odbcColumn.ColumnName == colname)
					{
						return num;
					}
					if (string.Compare(odbcColumn.ColumnName, colname, true) == 0)
					{
						return num;
					}
				}
				num++;
			}
			return -1;
		}

		private OdbcColumn GetColumn(int ordinal)
		{
			if (this.cols[ordinal] == null)
			{
				short num = 255;
				byte[] array = new byte[(int)num];
				short num2 = 0;
				uint num3 = 0U;
				short num4 = 0;
				short num5 = 0;
				short num6 = 0;
				OdbcReturn odbcReturn = libodbc.SQLDescribeCol(this.hstmt, Convert.ToUInt16(ordinal + 1), array, num, ref num2, ref num6, ref num3, ref num4, ref num5);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.Connection.CreateOdbcException(OdbcHandleType.Stmt, this.hstmt);
				}
				string text = OdbcDataReader.RemoveTrailingNullChar(Encoding.Unicode.GetString(array));
				OdbcColumn odbcColumn = new OdbcColumn(text, (SQL_TYPE)num6);
				odbcColumn.AllowDBNull = num5 != 0;
				odbcColumn.Digits = (int)num4;
				if (odbcColumn.IsVariableSizeType)
				{
					odbcColumn.MaxLength = (int)num3;
				}
				this.cols[ordinal] = odbcColumn;
			}
			return this.cols[ordinal];
		}

		private void GetColumns()
		{
			for (int i = 0; i < this.cols.Length; i++)
			{
				this.GetColumn(i);
			}
		}

		public override void Close()
		{
			this.open = false;
			this.currentRow = -1;
			this.command.FreeIfNotPrepared();
			if ((this.CommandBehavior & CommandBehavior.CloseConnection) == CommandBehavior.CloseConnection)
			{
				this.command.Connection.Close();
			}
		}

		public override bool GetBoolean(int i)
		{
			return (bool)this.GetValue(i);
		}

		public override byte GetByte(int i)
		{
			return Convert.ToByte(this.GetValue(i));
		}

		public override long GetBytes(int i, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			if (this.IsClosed)
			{
				throw new InvalidOperationException("Reader is not open.");
			}
			if (this.currentRow == -1)
			{
				throw new InvalidOperationException("No data available.");
			}
			int num = 0;
			int num2 = 0;
			byte[] array = new byte[length + 1];
			if (buffer == null)
			{
				length = 0;
			}
			OdbcReturn odbcReturn = libodbc.SQLGetData(this.hstmt, (ushort)(i + 1), SQL_C_TYPE.BINARY, array, length, ref num2);
			if (odbcReturn == OdbcReturn.NoData)
			{
				return 0L;
			}
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw this.Connection.CreateOdbcException(OdbcHandleType.Stmt, this.hstmt);
			}
			OdbcException ex = null;
			if (odbcReturn == OdbcReturn.SuccessWithInfo)
			{
				ex = this.Connection.CreateOdbcException(OdbcHandleType.Stmt, this.hstmt);
			}
			if (buffer == null)
			{
				return (long)num2;
			}
			bool flag;
			if (odbcReturn == OdbcReturn.SuccessWithInfo)
			{
				if (num2 == -4)
				{
					flag = true;
				}
				else if (num2 == -1)
				{
					flag = false;
					num = -1;
				}
				else
				{
					string sqlstate = ex.Errors[0].SQLState;
					if (sqlstate != "01004")
					{
						throw ex;
					}
					flag = true;
				}
			}
			else
			{
				flag = num2 != -1;
				num = num2;
			}
			if (flag)
			{
				if (num2 == -4)
				{
					int num3 = 0;
					while (array[num3] != 0)
					{
						buffer[bufferIndex + num3] = array[num3];
						num3++;
					}
					num = num3;
				}
				else
				{
					int num4 = Math.Min(num2, length);
					for (int j = 0; j < num4; j++)
					{
						buffer[bufferIndex + j] = array[j];
					}
					num = num4;
				}
			}
			return (long)num;
		}

		[MonoTODO]
		public override char GetChar(int i)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		public override long GetChars(int i, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			if (this.IsClosed)
			{
				throw new InvalidOperationException("The reader is closed.");
			}
			if (this.currentRow == -1)
			{
				throw new InvalidOperationException("No data available.");
			}
			if (i < 0 || i >= this.FieldCount)
			{
				throw new IndexOutOfRangeException();
			}
			throw new NotImplementedException();
		}

		[MonoTODO]
		[EditorBrowsable(EditorBrowsableState.Never)]
		private new IDataReader GetData(int i)
		{
			throw new NotImplementedException();
		}

		public override string GetDataTypeName(int i)
		{
			if (this.IsClosed)
			{
				throw new InvalidOperationException("The reader is closed.");
			}
			if (i < 0 || i >= this.FieldCount)
			{
				throw new IndexOutOfRangeException();
			}
			return this.GetColumnAttributeStr(i + 1, FieldIdentifier.TypeName);
		}

		public DateTime GetDate(int i)
		{
			return this.GetDateTime(i);
		}

		public override DateTime GetDateTime(int i)
		{
			return (DateTime)this.GetValue(i);
		}

		public override decimal GetDecimal(int i)
		{
			return (decimal)this.GetValue(i);
		}

		public override double GetDouble(int i)
		{
			return (double)this.GetValue(i);
		}

		public override Type GetFieldType(int i)
		{
			if (this.IsClosed)
			{
				throw new InvalidOperationException("The reader is closed.");
			}
			return this.GetColumn(i).DataType;
		}

		public override float GetFloat(int i)
		{
			return (float)this.GetValue(i);
		}

		[MonoTODO]
		public override Guid GetGuid(int i)
		{
			throw new NotImplementedException();
		}

		public override short GetInt16(int i)
		{
			return (short)this.GetValue(i);
		}

		public override int GetInt32(int i)
		{
			return (int)this.GetValue(i);
		}

		public override long GetInt64(int i)
		{
			return (long)this.GetValue(i);
		}

		public override string GetName(int i)
		{
			if (this.IsClosed)
			{
				throw new InvalidOperationException("The reader is closed.");
			}
			return this.GetColumn(i).ColumnName;
		}

		public override int GetOrdinal(string value)
		{
			if (this.IsClosed)
			{
				throw new InvalidOperationException("The reader is closed.");
			}
			if (value == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			int num = this.ColIndex(value);
			if (num == -1)
			{
				throw new IndexOutOfRangeException();
			}
			return num;
		}

		[MonoTODO]
		public override DataTable GetSchemaTable()
		{
			if (this.IsClosed)
			{
				throw new InvalidOperationException("The reader is closed.");
			}
			if (this._dataTableSchema != null)
			{
				return this._dataTableSchema;
			}
			DataTable dataTable = null;
			if (this.cols.Length > 0)
			{
				dataTable = new DataTable();
				dataTable.Columns.Add("ColumnName", typeof(string));
				dataTable.Columns.Add("ColumnOrdinal", typeof(int));
				dataTable.Columns.Add("ColumnSize", typeof(int));
				dataTable.Columns.Add("NumericPrecision", typeof(int));
				dataTable.Columns.Add("NumericScale", typeof(int));
				dataTable.Columns.Add("IsUnique", typeof(bool));
				dataTable.Columns.Add("IsKey", typeof(bool));
				DataColumn dataColumn = dataTable.Columns["IsKey"];
				dataColumn.AllowDBNull = true;
				dataTable.Columns.Add("BaseCatalogName", typeof(string));
				dataTable.Columns.Add("BaseColumnName", typeof(string));
				dataTable.Columns.Add("BaseSchemaName", typeof(string));
				dataTable.Columns.Add("BaseTableName", typeof(string));
				dataTable.Columns.Add("DataType", typeof(Type));
				dataTable.Columns.Add("AllowDBNull", typeof(bool));
				dataTable.Columns.Add("ProviderType", typeof(int));
				dataTable.Columns.Add("IsAliased", typeof(bool));
				dataTable.Columns.Add("IsExpression", typeof(bool));
				dataTable.Columns.Add("IsIdentity", typeof(bool));
				dataTable.Columns.Add("IsAutoIncrement", typeof(bool));
				dataTable.Columns.Add("IsRowVersion", typeof(bool));
				dataTable.Columns.Add("IsHidden", typeof(bool));
				dataTable.Columns.Add("IsLong", typeof(bool));
				dataTable.Columns.Add("IsReadOnly", typeof(bool));
				for (int i = 0; i < this.cols.Length; i++)
				{
					OdbcColumn column = this.GetColumn(i);
					DataRow dataRow = dataTable.NewRow();
					dataTable.Rows.Add(dataRow);
					dataRow["ColumnName"] = column.ColumnName;
					dataRow["ColumnOrdinal"] = i;
					dataRow["ColumnSize"] = column.MaxLength;
					dataRow["NumericPrecision"] = this.GetColumnAttribute(i + 1, FieldIdentifier.Precision);
					dataRow["NumericScale"] = this.GetColumnAttribute(i + 1, FieldIdentifier.Scale);
					dataRow["BaseTableName"] = this.GetColumnAttributeStr(i + 1, FieldIdentifier.TableName);
					dataRow["BaseSchemaName"] = this.GetColumnAttributeStr(i + 1, FieldIdentifier.SchemaName);
					dataRow["BaseCatalogName"] = this.GetColumnAttributeStr(i + 1, FieldIdentifier.CatelogName);
					dataRow["BaseColumnName"] = this.GetColumnAttributeStr(i + 1, FieldIdentifier.BaseColumnName);
					dataRow["DataType"] = column.DataType;
					dataRow["IsUnique"] = false;
					dataRow["IsKey"] = DBNull.Value;
					dataRow["AllowDBNull"] = this.GetColumnAttribute(i + 1, FieldIdentifier.Nullable) != 0;
					dataRow["ProviderType"] = (int)column.OdbcType;
					dataRow["IsAutoIncrement"] = this.GetColumnAttribute(i + 1, FieldIdentifier.AutoUniqueValue) == 1;
					dataRow["IsExpression"] = dataRow.IsNull("BaseTableName") || (string)dataRow["BaseTableName"] == string.Empty;
					dataRow["IsAliased"] = (string)dataRow["BaseColumnName"] != (string)dataRow["ColumnName"];
					dataRow["IsReadOnly"] = (bool)dataRow["IsExpression"] || this.GetColumnAttribute(i + 1, FieldIdentifier.Updatable) == 0;
					dataRow["IsIdentity"] = false;
					dataRow["IsRowVersion"] = false;
					dataRow["IsHidden"] = false;
					dataRow["IsLong"] = false;
				}
				DataRow[] array = dataTable.Select("BaseTableName <> ''", "BaseCatalogName, BaseSchemaName, BaseTableName ASC");
				string text = string.Empty;
				string text2 = string.Empty;
				string text3 = string.Empty;
				string[] array2 = null;
				foreach (DataRow dataRow2 in array)
				{
					string text4 = (string)dataRow2["BaseTableName"];
					string text5 = (string)dataRow2["BaseSchemaName"];
					string text6 = (string)dataRow2["BaseCatalogName"];
					if (text4 != text || text5 != text2 || text6 != text3)
					{
						array2 = this.GetPrimaryKeys(text6, text5, text4);
					}
					if (array2 != null && Array.BinarySearch<string>(array2, (string)dataRow2["BaseColumnName"]) >= 0)
					{
						dataRow2["IsKey"] = true;
						dataRow2["IsUnique"] = true;
						dataRow2["AllowDBNull"] = false;
						this.GetColumn(this.ColIndex((string)dataRow2["ColumnName"])).AllowDBNull = false;
					}
					text = text4;
					text2 = text5;
					text3 = text6;
				}
				dataTable.AcceptChanges();
			}
			return this._dataTableSchema = dataTable;
		}

		public override string GetString(int i)
		{
			object value = this.GetValue(i);
			if (value != null && value.GetType() != typeof(string))
			{
				return Convert.ToString(value);
			}
			return (string)this.GetValue(i);
		}

		[MonoTODO]
		public TimeSpan GetTime(int i)
		{
			throw new NotImplementedException();
		}

		public override object GetValue(int i)
		{
			if (this.IsClosed)
			{
				throw new InvalidOperationException("The reader is closed.");
			}
			if (this.currentRow == -1)
			{
				throw new InvalidOperationException("No data available.");
			}
			if (i > this.cols.Length - 1 || i < 0)
			{
				throw new IndexOutOfRangeException();
			}
			int num = 0;
			OdbcColumn column = this.GetColumn(i);
			object obj = null;
			ushort num2 = Convert.ToUInt16(i + 1);
			if (column.Value == null)
			{
				OdbcReturn odbcReturn;
				int num4;
				byte[] array;
				switch (column.OdbcType)
				{
				case OdbcType.BigInt:
				{
					long num3 = 0L;
					odbcReturn = libodbc.SQLGetData(this.hstmt, num2, column.SqlCType, ref num3, 0, ref num);
					obj = num3;
					goto IL_0698;
				}
				case OdbcType.Binary:
				{
					num4 = column.MaxLength;
					array = new byte[num4];
					long bytes = this.GetBytes(i, 0L, array, 0, num4);
					odbcReturn = OdbcReturn.Success;
					obj = array;
					goto IL_0698;
				}
				case OdbcType.Bit:
				{
					short num5 = 0;
					odbcReturn = libodbc.SQLGetData(this.hstmt, num2, column.SqlCType, ref num5, 0, ref num);
					if (num != -1)
					{
						obj = ((num5 != 0) ? "True" : "False");
					}
					goto IL_0698;
				}
				case OdbcType.DateTime:
				case OdbcType.Timestamp:
				case OdbcType.Date:
				case OdbcType.Time:
				{
					OdbcTimestamp odbcTimestamp = default(OdbcTimestamp);
					odbcReturn = libodbc.SQLGetData(this.hstmt, num2, column.SqlCType, ref odbcTimestamp, 0, ref num);
					if (num != -1)
					{
						if (column.OdbcType == OdbcType.Time)
						{
							obj = new TimeSpan((int)odbcTimestamp.year, (int)odbcTimestamp.month, (int)odbcTimestamp.day);
						}
						else
						{
							obj = new DateTime((int)odbcTimestamp.year, (int)odbcTimestamp.month, (int)odbcTimestamp.day, (int)odbcTimestamp.hour, (int)odbcTimestamp.minute, (int)odbcTimestamp.second);
							if (odbcTimestamp.fraction != 0UL)
							{
								obj = ((DateTime)obj).AddTicks((long)(odbcTimestamp.fraction / 100UL));
							}
						}
					}
					goto IL_0698;
				}
				case OdbcType.Decimal:
				case OdbcType.Numeric:
					num4 = 50;
					array = new byte[num4];
					odbcReturn = libodbc.SQLGetData(this.hstmt, num2, SQL_C_TYPE.CHAR, array, num4, ref num);
					if (num != -1)
					{
						byte[] array2 = new byte[num];
						for (int j = 0; j < num; j++)
						{
							array2[j] = array[j];
						}
						obj = decimal.Parse(Encoding.Default.GetString(array2), CultureInfo.InvariantCulture);
					}
					goto IL_0698;
				case OdbcType.Double:
				{
					double num6 = 0.0;
					odbcReturn = libodbc.SQLGetData(this.hstmt, num2, column.SqlCType, ref num6, 0, ref num);
					obj = num6;
					goto IL_0698;
				}
				case OdbcType.Image:
				case OdbcType.VarBinary:
				{
					num4 = ((column.MaxLength >= 255 || column.MaxLength <= 0) ? 255 : column.MaxLength);
					array = new byte[num4];
					ArrayList arrayList = new ArrayList();
					odbcReturn = libodbc.SQLGetData(this.hstmt, num2, SQL_C_TYPE.BINARY, array, 0, ref num);
					if (num != -1)
					{
						do
						{
							odbcReturn = libodbc.SQLGetData(this.hstmt, num2, SQL_C_TYPE.BINARY, array, num4, ref num);
							if (odbcReturn == OdbcReturn.Error)
							{
								break;
							}
							if (odbcReturn == OdbcReturn.NoData || num == -1)
							{
								break;
							}
							if (num < num4)
							{
								byte[] array3 = new byte[num];
								Array.Copy(array, 0, array3, 0, num);
								arrayList.AddRange(array3);
							}
							else
							{
								arrayList.AddRange(array);
							}
						}
						while (odbcReturn != OdbcReturn.NoData);
					}
					obj = arrayList.ToArray(typeof(byte));
					goto IL_0698;
				}
				case OdbcType.Int:
				{
					int num7 = 0;
					odbcReturn = libodbc.SQLGetData(this.hstmt, num2, column.SqlCType, ref num7, 0, ref num);
					obj = num7;
					goto IL_0698;
				}
				case OdbcType.NChar:
					num4 = 255;
					array = new byte[num4];
					odbcReturn = libodbc.SQLGetData(this.hstmt, num2, SQL_C_TYPE.WCHAR, array, num4, ref num);
					if (num != -1 && (odbcReturn != OdbcReturn.SuccessWithInfo || num != -4))
					{
						obj = Encoding.Unicode.GetString(array, 0, num);
					}
					goto IL_0698;
				case OdbcType.NText:
				case OdbcType.NVarChar:
				{
					num4 = ((column.MaxLength >= 127) ? 255 : (column.MaxLength * 2 + 1));
					array = new byte[num4];
					StringBuilder stringBuilder = new StringBuilder();
					do
					{
						odbcReturn = libodbc.SQLGetData(this.hstmt, num2, column.SqlCType, array, num4, ref num);
						if (odbcReturn == OdbcReturn.Error)
						{
							break;
						}
						if (odbcReturn == OdbcReturn.Success && num == -1)
						{
							odbcReturn = OdbcReturn.NoData;
						}
						if (odbcReturn != OdbcReturn.NoData && num > 0)
						{
							string text;
							if (num < num4)
							{
								text = Encoding.Unicode.GetString(array, 0, num);
							}
							else
							{
								text = Encoding.Unicode.GetString(array, 0, num4);
							}
							stringBuilder.Append(OdbcDataReader.RemoveTrailingNullChar(text));
						}
					}
					while (odbcReturn != OdbcReturn.NoData);
					obj = stringBuilder.ToString();
					goto IL_0698;
				}
				case OdbcType.Real:
				{
					float num8 = 0f;
					odbcReturn = libodbc.SQLGetData(this.hstmt, num2, column.SqlCType, ref num8, 0, ref num);
					obj = num8;
					goto IL_0698;
				}
				case OdbcType.SmallInt:
				{
					short num9 = 0;
					odbcReturn = libodbc.SQLGetData(this.hstmt, num2, column.SqlCType, ref num9, 0, ref num);
					obj = num9;
					goto IL_0698;
				}
				case OdbcType.Text:
				case OdbcType.VarChar:
				{
					num4 = ((column.MaxLength >= 255) ? 255 : (column.MaxLength + 1));
					array = new byte[num4];
					StringBuilder stringBuilder2 = new StringBuilder();
					do
					{
						odbcReturn = libodbc.SQLGetData(this.hstmt, num2, column.SqlCType, array, num4, ref num);
						if (odbcReturn == OdbcReturn.Error)
						{
							break;
						}
						if (odbcReturn == OdbcReturn.Success && num == -1)
						{
							odbcReturn = OdbcReturn.NoData;
						}
						if (odbcReturn != OdbcReturn.NoData && num > 0)
						{
							if (num < num4)
							{
								stringBuilder2.Append(Encoding.Default.GetString(array, 0, num));
							}
							else
							{
								stringBuilder2.Append(Encoding.Default.GetString(array, 0, num4 - 1));
							}
						}
					}
					while (odbcReturn != OdbcReturn.NoData);
					obj = stringBuilder2.ToString();
					goto IL_0698;
				}
				case OdbcType.TinyInt:
				{
					short num10 = 0;
					odbcReturn = libodbc.SQLGetData(this.hstmt, num2, column.SqlCType, ref num10, 0, ref num);
					obj = Convert.ToByte(num10);
					goto IL_0698;
				}
				}
				num4 = 255;
				array = new byte[num4];
				odbcReturn = libodbc.SQLGetData(this.hstmt, num2, SQL_C_TYPE.CHAR, array, num4, ref num);
				if (num != -1 && (odbcReturn != OdbcReturn.SuccessWithInfo || num != -4))
				{
					obj = Encoding.Default.GetString(array, 0, num);
				}
				IL_0698:
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo && odbcReturn != OdbcReturn.NoData)
				{
					throw this.Connection.CreateOdbcException(OdbcHandleType.Stmt, this.hstmt);
				}
				if (num == -1)
				{
					column.Value = DBNull.Value;
				}
				else
				{
					column.Value = obj;
				}
			}
			return column.Value;
		}

		public override int GetValues(object[] values)
		{
			if (this.IsClosed)
			{
				throw new InvalidOperationException("The reader is closed.");
			}
			if (this.currentRow == -1)
			{
				throw new InvalidOperationException("No data available.");
			}
			for (int i = 0; i < values.Length; i++)
			{
				if (i < this.FieldCount)
				{
					values[i] = this.GetValue(i);
				}
				else
				{
					values[i] = null;
				}
			}
			int num;
			if (values.Length < this.FieldCount)
			{
				num = values.Length;
			}
			else if (values.Length == this.FieldCount)
			{
				num = this.FieldCount;
			}
			else
			{
				num = this.FieldCount;
			}
			return num;
		}

		public override IEnumerator GetEnumerator()
		{
			return new DbEnumerator(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (this.disposed)
			{
				return;
			}
			if (disposing)
			{
				this.Close();
			}
			this.command = null;
			this.cols = null;
			this._dataTableSchema = null;
			this.disposed = true;
		}

		public override bool IsDBNull(int i)
		{
			return this.GetValue(i) is DBNull;
		}

		public override bool NextResult()
		{
			OdbcReturn odbcReturn = libodbc.SQLMoreResults(this.hstmt);
			if (odbcReturn == OdbcReturn.Success)
			{
				short num = 0;
				libodbc.SQLNumResultCols(this.hstmt, ref num);
				this.cols = new OdbcColumn[(int)num];
				this._dataTableSchema = null;
				this.GetColumns();
			}
			return odbcReturn == OdbcReturn.Success;
		}

		private bool NextRow()
		{
			OdbcReturn odbcReturn = libodbc.SQLFetch(this.hstmt);
			if (odbcReturn != OdbcReturn.Success)
			{
				this.currentRow = -1;
			}
			else
			{
				this.currentRow++;
			}
			foreach (OdbcColumn odbcColumn in this.cols)
			{
				if (odbcColumn != null)
				{
					odbcColumn.Value = null;
				}
			}
			return odbcReturn == OdbcReturn.Success;
		}

		private int GetColumnAttribute(int column, FieldIdentifier fieldId)
		{
			byte[] array = new byte[255];
			short num = 0;
			int num2 = 0;
			OdbcReturn odbcReturn = libodbc.SQLColAttribute(this.hstmt, (short)column, fieldId, array, (short)array.Length, ref num, ref num2);
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw this.Connection.CreateOdbcException(OdbcHandleType.Stmt, this.hstmt);
			}
			return num2;
		}

		private string GetColumnAttributeStr(int column, FieldIdentifier fieldId)
		{
			byte[] array = new byte[255];
			short num = 0;
			int num2 = 0;
			OdbcReturn odbcReturn = libodbc.SQLColAttribute(this.hstmt, (short)column, fieldId, array, (short)array.Length, ref num, ref num2);
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw this.Connection.CreateOdbcException(OdbcHandleType.Stmt, this.hstmt);
			}
			string text = string.Empty;
			if (num > 0)
			{
				text = Encoding.Unicode.GetString(array, 0, (int)num);
			}
			return text;
		}

		private string[] GetPrimaryKeys(string catalog, string schema, string table)
		{
			if (this.cols.Length <= 0)
			{
				return new string[0];
			}
			ArrayList arrayList = null;
			try
			{
				arrayList = this.GetPrimaryKeysBySQLPrimaryKey(catalog, schema, table);
			}
			catch (OdbcException)
			{
				try
				{
					arrayList = this.GetPrimaryKeysBySQLStatistics(catalog, schema, table);
				}
				catch (OdbcException)
				{
				}
			}
			if (arrayList == null)
			{
				return null;
			}
			arrayList.Sort();
			return (string[])arrayList.ToArray(typeof(string));
		}

		private ArrayList GetPrimaryKeysBySQLPrimaryKey(string catalog, string schema, string table)
		{
			ArrayList arrayList = new ArrayList();
			IntPtr zero = IntPtr.Zero;
			try
			{
				OdbcReturn odbcReturn = libodbc.SQLAllocHandle(OdbcHandleType.Stmt, this.command.Connection.hDbc, ref zero);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.Connection.CreateOdbcException(OdbcHandleType.Dbc, this.Connection.hDbc);
				}
				odbcReturn = libodbc.SQLPrimaryKeys(zero, catalog, -3, schema, -3, table, -3);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.Connection.CreateOdbcException(OdbcHandleType.Stmt, zero);
				}
				int num = 0;
				byte[] array = new byte[255];
				odbcReturn = libodbc.SQLBindCol(zero, 4, SQL_C_TYPE.CHAR, array, array.Length, ref num);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.Connection.CreateOdbcException(OdbcHandleType.Stmt, zero);
				}
				for (;;)
				{
					odbcReturn = libodbc.SQLFetch(zero);
					if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
					{
						break;
					}
					string @string = Encoding.Default.GetString(array, 0, num);
					arrayList.Add(@string);
				}
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					OdbcReturn odbcReturn = libodbc.SQLFreeStmt(zero, libodbc.SQLFreeStmtOptions.Close);
					if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
					{
						throw this.Connection.CreateOdbcException(OdbcHandleType.Stmt, zero);
					}
					odbcReturn = libodbc.SQLFreeHandle(3, zero);
					if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
					{
						throw this.Connection.CreateOdbcException(OdbcHandleType.Stmt, zero);
					}
				}
			}
			return arrayList;
		}

		private ArrayList GetPrimaryKeysBySQLStatistics(string catalog, string schema, string table)
		{
			ArrayList arrayList = new ArrayList();
			IntPtr zero = IntPtr.Zero;
			try
			{
				OdbcReturn odbcReturn = libodbc.SQLAllocHandle(OdbcHandleType.Stmt, this.command.Connection.hDbc, ref zero);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.Connection.CreateOdbcException(OdbcHandleType.Dbc, this.Connection.hDbc);
				}
				odbcReturn = libodbc.SQLStatistics(zero, catalog, -3, schema, -3, table, -3, 0, 0);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.Connection.CreateOdbcException(OdbcHandleType.Stmt, zero);
				}
				int num = 0;
				short num2 = 0;
				odbcReturn = libodbc.SQLBindCol(zero, 4, SQL_C_TYPE.SHORT, ref num2, 2, ref num);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.Connection.CreateOdbcException(OdbcHandleType.Stmt, zero);
				}
				int num3 = 0;
				byte[] array = new byte[255];
				odbcReturn = libodbc.SQLBindCol(zero, 9, SQL_C_TYPE.CHAR, array, array.Length, ref num3);
				if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
				{
					throw this.Connection.CreateOdbcException(OdbcHandleType.Stmt, zero);
				}
				for (;;)
				{
					odbcReturn = libodbc.SQLFetch(zero);
					if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
					{
						break;
					}
					if (num2 == 1)
					{
						goto Block_13;
					}
				}
				goto IL_0126;
				Block_13:
				string @string = Encoding.Default.GetString(array, 0, num3);
				arrayList.Add(@string);
				IL_0126:;
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					OdbcReturn odbcReturn = libodbc.SQLFreeStmt(zero, libodbc.SQLFreeStmtOptions.Close);
					if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
					{
						throw this.Connection.CreateOdbcException(OdbcHandleType.Stmt, zero);
					}
					odbcReturn = libodbc.SQLFreeHandle(3, zero);
					if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
					{
						throw this.Connection.CreateOdbcException(OdbcHandleType.Stmt, zero);
					}
				}
			}
			return arrayList;
		}

		public override bool Read()
		{
			return this.NextRow();
		}

		private static string RemoveTrailingNullChar(string value)
		{
			return value.TrimEnd(new char[1]);
		}

		private OdbcCommand command;

		private bool open;

		private int currentRow;

		private OdbcColumn[] cols;

		private IntPtr hstmt;

		private int _recordsAffected = -1;

		private bool disposed;

		private DataTable _dataTableSchema;

		private CommandBehavior behavior;
	}
}
