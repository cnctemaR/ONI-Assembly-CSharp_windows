using System;
using System.Collections;
using System.ComponentModel;
using System.Data.Common;
using System.Runtime.InteropServices;

namespace System.Data.OleDb
{
	public sealed class OleDbDataReader : DbDataReader, IDisposable
	{
		internal OleDbDataReader(OleDbCommand command, ArrayList results)
		{
			this.command = command;
			this.open = true;
			if (results != null)
			{
				this.gdaResults = results;
			}
			else
			{
				this.gdaResults = new ArrayList();
			}
			this.currentResult = -1;
			this.currentRow = -1;
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
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
				if (this.currentResult < 0 || this.currentResult >= this.gdaResults.Count)
				{
					return 0;
				}
				return libgda.gda_data_model_get_n_columns((IntPtr)this.gdaResults[this.currentResult]);
			}
		}

		public override bool IsClosed
		{
			get
			{
				return !this.open;
			}
		}

		public override object this[string name]
		{
			get
			{
				if (this.currentResult == -1)
				{
					throw new InvalidOperationException();
				}
				int num = libgda.gda_data_model_get_column_position((IntPtr)this.gdaResults[this.currentResult], name);
				if (num == -1)
				{
					throw new IndexOutOfRangeException();
				}
				return this[num];
			}
		}

		public override object this[int index]
		{
			get
			{
				return this.GetValue(index);
			}
		}

		public override int RecordsAffected
		{
			get
			{
				if (this.currentResult < 0 || this.currentResult >= this.gdaResults.Count)
				{
					return 0;
				}
				int num = libgda.gda_data_model_get_n_rows((IntPtr)this.gdaResults[this.currentResult]);
				if (num > 0 && this.FieldCount > 0)
				{
					return -1;
				}
				return (this.FieldCount <= 0) ? num : (-1);
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

		[MonoTODO]
		public override int VisibleFieldCount
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override void Close()
		{
			for (int i = 0; i < this.gdaResults.Count; i++)
			{
				IntPtr intPtr = (IntPtr)this.gdaResults[i];
				libgda.FreeObject(intPtr);
			}
			this.gdaResults.Clear();
			this.gdaResults = null;
			this.open = false;
			this.currentResult = -1;
			this.currentRow = -1;
		}

		public override bool GetBoolean(int ordinal)
		{
			if (this.currentResult == -1)
			{
				throw new InvalidCastException();
			}
			IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)this.gdaResults[this.currentResult], ordinal, this.currentRow);
			if (intPtr == IntPtr.Zero)
			{
				throw new InvalidCastException();
			}
			if (libgda.gda_value_get_type(intPtr) != GdaValueType.Boolean)
			{
				throw new InvalidCastException();
			}
			return libgda.gda_value_get_boolean(intPtr);
		}

		public override byte GetByte(int ordinal)
		{
			if (this.currentResult == -1)
			{
				throw new InvalidCastException();
			}
			IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)this.gdaResults[this.currentResult], ordinal, this.currentRow);
			if (intPtr == IntPtr.Zero)
			{
				throw new InvalidCastException();
			}
			if (libgda.gda_value_get_type(intPtr) != GdaValueType.Tinyint)
			{
				throw new InvalidCastException();
			}
			return libgda.gda_value_get_tinyint(intPtr);
		}

		[MonoTODO]
		public override long GetBytes(int ordinal, long dataIndex, byte[] buffer, int bufferIndex, int length)
		{
			throw new NotImplementedException();
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override char GetChar(int ordinal)
		{
			if (this.currentResult == -1)
			{
				throw new InvalidCastException();
			}
			IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)this.gdaResults[this.currentResult], ordinal, this.currentRow);
			if (intPtr == IntPtr.Zero)
			{
				throw new InvalidCastException();
			}
			if (libgda.gda_value_get_type(intPtr) != GdaValueType.Tinyint)
			{
				throw new InvalidCastException();
			}
			return (char)libgda.gda_value_get_tinyint(intPtr);
		}

		[MonoTODO]
		public override long GetChars(int ordinal, long dataIndex, char[] buffer, int bufferIndex, int length)
		{
			throw new NotImplementedException();
		}

		[MonoTODO]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public new OleDbDataReader GetData(int ordinal)
		{
			throw new NotImplementedException();
		}

		protected override DbDataReader GetDbDataReader(int ordinal)
		{
			return this.GetData(ordinal);
		}

		public override string GetDataTypeName(int index)
		{
			if (this.currentResult == -1)
			{
				return "unknown";
			}
			IntPtr intPtr = libgda.gda_data_model_describe_column((IntPtr)this.gdaResults[this.currentResult], index);
			if (intPtr == IntPtr.Zero)
			{
				return "unknown";
			}
			GdaValueType gdaValueType = libgda.gda_field_attributes_get_gdatype(intPtr);
			libgda.gda_field_attributes_free(intPtr);
			return libgda.gda_type_to_string(gdaValueType);
		}

		public override DateTime GetDateTime(int ordinal)
		{
			if (this.currentResult == -1)
			{
				throw new InvalidCastException();
			}
			IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)this.gdaResults[this.currentResult], ordinal, this.currentRow);
			if (intPtr == IntPtr.Zero)
			{
				throw new InvalidCastException();
			}
			if (libgda.gda_value_get_type(intPtr) == GdaValueType.Date)
			{
				GdaDate gdaDate = (GdaDate)Marshal.PtrToStructure(libgda.gda_value_get_date(intPtr), typeof(GdaDate));
				return new DateTime((int)gdaDate.year, (int)gdaDate.month, (int)gdaDate.day);
			}
			if (libgda.gda_value_get_type(intPtr) == GdaValueType.Time)
			{
				GdaTime gdaTime = (GdaTime)Marshal.PtrToStructure(libgda.gda_value_get_time(intPtr), typeof(GdaTime));
				return new DateTime(0, 0, 0, (int)gdaTime.hour, (int)gdaTime.minute, (int)gdaTime.second, 0);
			}
			if (libgda.gda_value_get_type(intPtr) == GdaValueType.Timestamp)
			{
				GdaTimestamp gdaTimestamp = (GdaTimestamp)Marshal.PtrToStructure(libgda.gda_value_get_timestamp(intPtr), typeof(GdaTimestamp));
				return new DateTime((int)gdaTimestamp.year, (int)gdaTimestamp.month, (int)gdaTimestamp.day, (int)gdaTimestamp.hour, (int)gdaTimestamp.minute, (int)gdaTimestamp.second, (int)gdaTimestamp.fraction);
			}
			throw new InvalidCastException();
		}

		[MonoTODO]
		public override decimal GetDecimal(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override double GetDouble(int ordinal)
		{
			if (this.currentResult == -1)
			{
				throw new InvalidCastException();
			}
			IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)this.gdaResults[this.currentResult], ordinal, this.currentRow);
			if (intPtr == IntPtr.Zero)
			{
				throw new InvalidCastException();
			}
			if (libgda.gda_value_get_type(intPtr) != GdaValueType.Double)
			{
				throw new InvalidCastException();
			}
			return libgda.gda_value_get_double(intPtr);
		}

		public override Type GetFieldType(int index)
		{
			if (this.currentResult == -1)
			{
				throw new IndexOutOfRangeException();
			}
			IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)this.gdaResults[this.currentResult], index, this.currentRow);
			if (intPtr == IntPtr.Zero)
			{
				throw new IndexOutOfRangeException();
			}
			switch (libgda.gda_value_get_type(intPtr))
			{
			case GdaValueType.Bigint:
				return typeof(long);
			case GdaValueType.Boolean:
				return typeof(bool);
			case GdaValueType.Date:
				return typeof(DateTime);
			case GdaValueType.Double:
				return typeof(double);
			case GdaValueType.Integer:
				return typeof(int);
			case GdaValueType.Single:
				return typeof(float);
			case GdaValueType.Smallint:
				return typeof(byte);
			case GdaValueType.String:
				return typeof(string);
			case GdaValueType.Time:
				return typeof(DateTime);
			case GdaValueType.Timestamp:
				return typeof(DateTime);
			case GdaValueType.Tinyint:
				return typeof(byte);
			}
			return typeof(string);
		}

		public override float GetFloat(int ordinal)
		{
			if (this.currentResult == -1)
			{
				throw new InvalidCastException();
			}
			IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)this.gdaResults[this.currentResult], ordinal, this.currentRow);
			if (intPtr == IntPtr.Zero)
			{
				throw new InvalidCastException();
			}
			if (libgda.gda_value_get_type(intPtr) != GdaValueType.Single)
			{
				throw new InvalidCastException();
			}
			return libgda.gda_value_get_single(intPtr);
		}

		[MonoTODO]
		public override Guid GetGuid(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override short GetInt16(int ordinal)
		{
			if (this.currentResult == -1)
			{
				throw new InvalidCastException();
			}
			IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)this.gdaResults[this.currentResult], ordinal, this.currentRow);
			if (intPtr == IntPtr.Zero)
			{
				throw new InvalidCastException();
			}
			if (libgda.gda_value_get_type(intPtr) != GdaValueType.Smallint)
			{
				throw new InvalidCastException();
			}
			return (short)libgda.gda_value_get_smallint(intPtr);
		}

		public override int GetInt32(int ordinal)
		{
			if (this.currentResult == -1)
			{
				throw new InvalidCastException();
			}
			IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)this.gdaResults[this.currentResult], ordinal, this.currentRow);
			if (intPtr == IntPtr.Zero)
			{
				throw new InvalidCastException();
			}
			if (libgda.gda_value_get_type(intPtr) != GdaValueType.Integer)
			{
				throw new InvalidCastException();
			}
			return libgda.gda_value_get_integer(intPtr);
		}

		public override long GetInt64(int ordinal)
		{
			if (this.currentResult == -1)
			{
				throw new InvalidCastException();
			}
			IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)this.gdaResults[this.currentResult], ordinal, this.currentRow);
			if (intPtr == IntPtr.Zero)
			{
				throw new InvalidCastException();
			}
			if (libgda.gda_value_get_type(intPtr) != GdaValueType.Bigint)
			{
				throw new InvalidCastException();
			}
			return libgda.gda_value_get_bigint(intPtr);
		}

		public override string GetName(int index)
		{
			if (this.currentResult == -1)
			{
				return null;
			}
			return libgda.gda_data_model_get_column_title((IntPtr)this.gdaResults[this.currentResult], index);
		}

		public override int GetOrdinal(string name)
		{
			if (this.currentResult == -1)
			{
				throw new IndexOutOfRangeException();
			}
			for (int i = 0; i < this.FieldCount; i++)
			{
				if (this.GetName(i) == name)
				{
					return i;
				}
			}
			throw new IndexOutOfRangeException();
		}

		public override DataTable GetSchemaTable()
		{
			DataTable dataTable = null;
			if (this.FieldCount > 0)
			{
				if (this.currentResult == -1)
				{
					return null;
				}
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
				for (int i = 0; i < this.FieldCount; i++)
				{
					DataRow dataRow = dataTable.NewRow();
					IntPtr intPtr = libgda.gda_data_model_describe_column((IntPtr)this.gdaResults[this.currentResult], i);
					if (intPtr == IntPtr.Zero)
					{
						return null;
					}
					GdaValueType gdaValueType = libgda.gda_field_attributes_get_gdatype(intPtr);
					long num = libgda.gda_field_attributes_get_defined_size(intPtr);
					libgda.gda_field_attributes_free(intPtr);
					dataRow["ColumnName"] = this.GetName(i);
					dataRow["ColumnOrdinal"] = i + 1;
					dataRow["ColumnSize"] = (int)num;
					dataRow["NumericPrecision"] = 0;
					dataRow["NumericScale"] = 0;
					dataRow["IsUnique"] = false;
					dataRow["IsKey"] = DBNull.Value;
					dataRow["BaseCatalogName"] = string.Empty;
					dataRow["BaseColumnName"] = this.GetName(i);
					dataRow["BaseSchemaName"] = string.Empty;
					dataRow["BaseTableName"] = string.Empty;
					dataRow["DataType"] = this.GetFieldType(i);
					dataRow["AllowDBNull"] = false;
					dataRow["ProviderType"] = (int)gdaValueType;
					dataRow["IsAliased"] = false;
					dataRow["IsExpression"] = false;
					dataRow["IsIdentity"] = false;
					dataRow["IsAutoIncrement"] = false;
					dataRow["IsRowVersion"] = false;
					dataRow["IsHidden"] = false;
					dataRow["IsLong"] = false;
					dataRow["IsReadOnly"] = false;
					dataRow.AcceptChanges();
					dataTable.Rows.Add(dataRow);
				}
			}
			return dataTable;
		}

		public override string GetString(int ordinal)
		{
			if (this.currentResult == -1)
			{
				throw new InvalidCastException();
			}
			IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)this.gdaResults[this.currentResult], ordinal, this.currentRow);
			if (intPtr == IntPtr.Zero)
			{
				throw new InvalidCastException();
			}
			if (libgda.gda_value_get_type(intPtr) != GdaValueType.String)
			{
				throw new InvalidCastException();
			}
			return libgda.gda_value_get_string(intPtr);
		}

		[MonoTODO]
		public TimeSpan GetTimeSpan(int ordinal)
		{
			throw new NotImplementedException();
		}

		public override object GetValue(int ordinal)
		{
			if (this.currentResult == -1)
			{
				throw new IndexOutOfRangeException();
			}
			IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)this.gdaResults[this.currentResult], ordinal, this.currentRow);
			if (intPtr == IntPtr.Zero)
			{
				throw new IndexOutOfRangeException();
			}
			switch (libgda.gda_value_get_type(intPtr))
			{
			case GdaValueType.Bigint:
				return this.GetInt64(ordinal);
			case GdaValueType.Boolean:
				return this.GetBoolean(ordinal);
			case GdaValueType.Date:
				return this.GetDateTime(ordinal);
			case GdaValueType.Double:
				return this.GetDouble(ordinal);
			case GdaValueType.Integer:
				return this.GetInt32(ordinal);
			case GdaValueType.Single:
				return this.GetFloat(ordinal);
			case GdaValueType.Smallint:
				return this.GetByte(ordinal);
			case GdaValueType.String:
				return this.GetString(ordinal);
			case GdaValueType.Time:
				return this.GetDateTime(ordinal);
			case GdaValueType.Timestamp:
				return this.GetDateTime(ordinal);
			case GdaValueType.Tinyint:
				return this.GetByte(ordinal);
			}
			return libgda.gda_value_stringify(intPtr);
		}

		[MonoTODO]
		public override int GetValues(object[] values)
		{
			throw new NotImplementedException();
		}

		public override IEnumerator GetEnumerator()
		{
			return new DbEnumerator(this);
		}

		public override bool IsDBNull(int ordinal)
		{
			if (this.currentResult == -1)
			{
				throw new IndexOutOfRangeException();
			}
			IntPtr intPtr = libgda.gda_data_model_get_value_at((IntPtr)this.gdaResults[this.currentResult], ordinal, this.currentRow);
			if (intPtr == IntPtr.Zero)
			{
				throw new IndexOutOfRangeException();
			}
			return libgda.gda_value_is_null(intPtr);
		}

		public override bool NextResult()
		{
			int num = this.currentResult + 1;
			if (num >= 0 && num < this.gdaResults.Count)
			{
				this.currentResult++;
				return true;
			}
			return false;
		}

		public override bool Read()
		{
			if (this.currentResult < 0 || this.currentResult >= this.gdaResults.Count)
			{
				return false;
			}
			this.currentRow++;
			return this.currentRow < libgda.gda_data_model_get_n_rows((IntPtr)this.gdaResults[this.currentResult]);
		}

		private new void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.command = null;
					GC.SuppressFinalize(this);
				}
				if (this.gdaResults != null)
				{
					this.gdaResults.Clear();
					this.gdaResults = null;
				}
				if (this.open)
				{
					this.Close();
				}
				this.disposed = true;
			}
		}

		private OleDbCommand command;

		private bool open;

		private ArrayList gdaResults;

		private int currentResult;

		private int currentRow;

		private bool disposed;
	}
}
