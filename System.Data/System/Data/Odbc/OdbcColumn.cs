using System;

namespace System.Data.Odbc
{
	internal class OdbcColumn
	{
		internal OdbcColumn(string Name, OdbcType Type)
		{
			this.ColumnName = Name;
			this.OdbcType = Type;
			this.AllowDBNull = false;
			this.MaxLength = 0;
			this.Digits = 0;
			this.Value = null;
		}

		internal OdbcColumn(string Name, SQL_TYPE type)
		{
			this.ColumnName = Name;
			this.AllowDBNull = false;
			this.MaxLength = 0;
			this.Digits = 0;
			this.Value = null;
			this.UpdateTypes(type);
		}

		internal Type DataType
		{
			get
			{
				switch (this.OdbcType)
				{
				case OdbcType.BigInt:
					return typeof(long);
				case OdbcType.Binary:
				case OdbcType.Image:
				case OdbcType.VarBinary:
					return typeof(byte[]);
				case OdbcType.Bit:
					return typeof(bool);
				case OdbcType.Char:
				case OdbcType.NChar:
					return typeof(string);
				case OdbcType.DateTime:
				case OdbcType.SmallDateTime:
				case OdbcType.Timestamp:
				case OdbcType.Date:
					return typeof(DateTime);
				case OdbcType.Decimal:
				case OdbcType.Numeric:
					return typeof(decimal);
				case OdbcType.Double:
					return typeof(double);
				case OdbcType.Int:
					return typeof(int);
				case OdbcType.NText:
				case OdbcType.NVarChar:
				case OdbcType.Text:
				case OdbcType.VarChar:
					return typeof(string);
				case OdbcType.Real:
					return typeof(float);
				case OdbcType.UniqueIdentifier:
					return typeof(Guid);
				case OdbcType.SmallInt:
					return typeof(short);
				case OdbcType.TinyInt:
					return typeof(byte);
				case OdbcType.Time:
					return typeof(TimeSpan);
				default:
					throw new InvalidCastException();
				}
			}
		}

		internal bool IsDateType
		{
			get
			{
				OdbcType odbcType = this.OdbcType;
				switch (odbcType)
				{
				case OdbcType.SmallDateTime:
				case OdbcType.Timestamp:
					break;
				default:
					if (odbcType != OdbcType.Date && odbcType != OdbcType.Time && odbcType != OdbcType.DateTime)
					{
						return false;
					}
					break;
				}
				return true;
			}
		}

		internal bool IsStringType
		{
			get
			{
				OdbcType odbcType = this.OdbcType;
				return odbcType == OdbcType.NText || odbcType == OdbcType.NVarChar || odbcType == OdbcType.Char || odbcType == OdbcType.Text || odbcType == OdbcType.VarChar;
			}
		}

		internal bool IsVariableSizeType
		{
			get
			{
				if (this.IsStringType)
				{
					return true;
				}
				OdbcType odbcType = this.OdbcType;
				return odbcType == OdbcType.Binary || odbcType == OdbcType.Image || odbcType == OdbcType.VarBinary;
			}
		}

		internal SQL_TYPE SqlType
		{
			get
			{
				if (this._sqlType == SQL_TYPE.UNASSIGNED)
				{
					this._sqlType = OdbcTypeConverter.GetTypeMap(this.OdbcType).SqlType;
				}
				return this._sqlType;
			}
			set
			{
				this._sqlType = value;
			}
		}

		internal SQL_C_TYPE SqlCType
		{
			get
			{
				if (this._sqlCType == SQL_C_TYPE.UNASSIGNED)
				{
					this._sqlCType = OdbcTypeConverter.GetTypeMap(this.OdbcType).NativeType;
				}
				return this._sqlCType;
			}
			set
			{
				this._sqlCType = value;
			}
		}

		internal void UpdateTypes(SQL_TYPE sqlType)
		{
			this.SqlType = sqlType;
			OdbcTypeMap typeMap = OdbcTypeConverter.GetTypeMap(this.SqlType);
			this.OdbcType = typeMap.OdbcType;
			this.SqlCType = typeMap.NativeType;
		}

		internal string ColumnName;

		internal OdbcType OdbcType;

		private SQL_TYPE _sqlType = SQL_TYPE.UNASSIGNED;

		private SQL_C_TYPE _sqlCType = SQL_C_TYPE.UNASSIGNED;

		internal bool AllowDBNull;

		internal int MaxLength;

		internal int Digits;

		internal object Value;
	}
}
