using System;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.Odbc
{
	public sealed class OdbcParameter : DbParameter, ICloneable, IDataParameter, IDbDataParameter
	{
		public OdbcParameter()
		{
		}

		public OdbcParameter(string name, object value)
			: this()
		{
			this.ParameterName = name;
			this.Value = value;
		}

		public OdbcParameter(string name, OdbcType type)
			: this()
		{
			this.ParameterName = name;
			this.OdbcType = type;
		}

		public OdbcParameter(string name, OdbcType type, int size)
			: this()
		{
			this.ParameterName = name;
			this.OdbcType = type;
			this.Size = size;
		}

		public OdbcParameter(string name, OdbcType type, int size, string sourcecolumn)
			: this()
		{
			this.ParameterName = name;
			this.OdbcType = type;
			this.Size = size;
			this.SourceColumn = sourcecolumn;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public OdbcParameter(string parameterName, OdbcType odbcType, int size, ParameterDirection parameterDirection, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value)
			: this()
		{
			this.ParameterName = parameterName;
			this.OdbcType = odbcType;
			this.Size = size;
			this.Direction = parameterDirection;
			this.IsNullable = isNullable;
			this.PrecisionInternal = precision;
			this.ScaleInternal = scale;
			this.SourceColumn = srcColumn;
			this.SourceVersion = srcVersion;
			this.Value = value;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public OdbcParameter(string parameterName, OdbcType odbcType, int size, ParameterDirection parameterDirection, byte precision, byte scale, string sourceColumn, DataRowVersion sourceVersion, bool sourceColumnNullMapping, object value)
			: this()
		{
			this.ParameterName = parameterName;
			this.OdbcType = odbcType;
			this.Size = size;
			this.Direction = parameterDirection;
			this.PrecisionInternal = precision;
			this.ScaleInternal = scale;
			this.SourceColumn = sourceColumn;
			this.SourceVersion = sourceVersion;
			this.SourceColumnNullMapping = sourceColumnNullMapping;
			this.Value = value;
		}

		public override DbType DbType
		{
			get
			{
				if (this._userSpecifiedType)
				{
					return this._typemap._dbType;
				}
				return TypeMap._NVarChar._dbType;
			}
			set
			{
				if (this._typemap == null || this._typemap._dbType != value)
				{
					this.PropertyTypeChanging();
					this._typemap = TypeMap.FromDbType(value);
					this._userSpecifiedType = true;
				}
			}
		}

		public override void ResetDbType()
		{
			this.ResetOdbcType();
		}

		[DbProviderSpecificTypeProperty(true)]
		[DefaultValue(OdbcType.NChar)]
		public OdbcType OdbcType
		{
			get
			{
				if (this._userSpecifiedType)
				{
					return this._typemap._odbcType;
				}
				return TypeMap._NVarChar._odbcType;
			}
			set
			{
				if (this._typemap == null || this._typemap._odbcType != value)
				{
					this.PropertyTypeChanging();
					this._typemap = TypeMap.FromOdbcType(value);
					this._userSpecifiedType = true;
				}
			}
		}

		public void ResetOdbcType()
		{
			this.PropertyTypeChanging();
			this._typemap = null;
			this._userSpecifiedType = false;
		}

		internal bool HasChanged
		{
			set
			{
				this._hasChanged = value;
			}
		}

		internal bool UserSpecifiedType
		{
			get
			{
				return this._userSpecifiedType;
			}
		}

		public override string ParameterName
		{
			get
			{
				string parameterName = this._parameterName;
				if (parameterName == null)
				{
					return ADP.StrEmpty;
				}
				return parameterName;
			}
			set
			{
				if (this._parameterName != value)
				{
					this.PropertyChanging();
					this._parameterName = value;
				}
			}
		}

		public new byte Precision
		{
			get
			{
				return this.PrecisionInternal;
			}
			set
			{
				this.PrecisionInternal = value;
			}
		}

		internal byte PrecisionInternal
		{
			get
			{
				byte b = this._precision;
				if (b == 0)
				{
					b = this.ValuePrecision(this.Value);
				}
				return b;
			}
			set
			{
				if (this._precision != value)
				{
					this.PropertyChanging();
					this._precision = value;
				}
			}
		}

		private bool ShouldSerializePrecision()
		{
			return this._precision > 0;
		}

		public new byte Scale
		{
			get
			{
				return this.ScaleInternal;
			}
			set
			{
				this.ScaleInternal = value;
			}
		}

		internal byte ScaleInternal
		{
			get
			{
				byte b = this._scale;
				if (!this.ShouldSerializeScale(b))
				{
					b = this.ValueScale(this.Value);
				}
				return b;
			}
			set
			{
				if (this._scale != value || !this._hasScale)
				{
					this.PropertyChanging();
					this._scale = value;
					this._hasScale = true;
				}
			}
		}

		private bool ShouldSerializeScale()
		{
			return this.ShouldSerializeScale(this._scale);
		}

		private bool ShouldSerializeScale(byte scale)
		{
			return this._hasScale && (scale != 0 || this.ShouldSerializePrecision());
		}

		private int GetColumnSize(object value, int offset, int ordinal)
		{
			if (ODBC32.SQL_C.NUMERIC == this._bindtype._sql_c && this._internalPrecision != 0)
			{
				return Math.Min((int)this._internalPrecision, 29);
			}
			int num = this._bindtype._columnSize;
			if (0 >= num)
			{
				if (ODBC32.SQL_C.NUMERIC == this._typemap._sql_c)
				{
					num = 62;
				}
				else
				{
					num = this._internalSize;
					if (!this._internalShouldSerializeSize || 1073741823 <= num || num < 0)
					{
						if (!this._internalShouldSerializeSize && (ParameterDirection.Output & this._internalDirection) != (ParameterDirection)0)
						{
							throw ADP.UninitializedParameterSize(ordinal, this._bindtype._type);
						}
						if (value == null || Convert.IsDBNull(value))
						{
							num = 0;
						}
						else if (value is string)
						{
							num = ((string)value).Length - offset;
							if ((ParameterDirection.Output & this._internalDirection) != (ParameterDirection)0 && 1073741823 <= this._internalSize)
							{
								num = Math.Max(num, 4096);
							}
							if (ODBC32.SQL_TYPE.CHAR == this._bindtype._sql_type || ODBC32.SQL_TYPE.VARCHAR == this._bindtype._sql_type || ODBC32.SQL_TYPE.LONGVARCHAR == this._bindtype._sql_type)
							{
								num = Encoding.Default.GetMaxByteCount(num);
							}
						}
						else if (value is char[])
						{
							num = ((char[])value).Length - offset;
							if ((ParameterDirection.Output & this._internalDirection) != (ParameterDirection)0 && 1073741823 <= this._internalSize)
							{
								num = Math.Max(num, 4096);
							}
							if (ODBC32.SQL_TYPE.CHAR == this._bindtype._sql_type || ODBC32.SQL_TYPE.VARCHAR == this._bindtype._sql_type || ODBC32.SQL_TYPE.LONGVARCHAR == this._bindtype._sql_type)
							{
								num = Encoding.Default.GetMaxByteCount(num);
							}
						}
						else if (value is byte[])
						{
							num = ((byte[])value).Length - offset;
							if ((ParameterDirection.Output & this._internalDirection) != (ParameterDirection)0 && 1073741823 <= this._internalSize)
							{
								num = Math.Max(num, 8192);
							}
						}
						num = Math.Max(2, num);
					}
				}
			}
			return num;
		}

		private int GetValueSize(object value, int offset)
		{
			if (ODBC32.SQL_C.NUMERIC == this._bindtype._sql_c && this._internalPrecision != 0)
			{
				return Math.Min((int)this._internalPrecision, 29);
			}
			int num = this._bindtype._columnSize;
			if (0 >= num)
			{
				bool flag = false;
				if (value is string)
				{
					num = ((string)value).Length - offset;
					flag = true;
				}
				else if (value is char[])
				{
					num = ((char[])value).Length - offset;
					flag = true;
				}
				else if (value is byte[])
				{
					num = ((byte[])value).Length - offset;
				}
				else
				{
					num = 0;
				}
				if (this._internalShouldSerializeSize && this._internalSize >= 0 && this._internalSize < num && this._bindtype == this._originalbindtype)
				{
					num = this._internalSize;
				}
				if (flag)
				{
					num *= 2;
				}
			}
			return num;
		}

		private int GetParameterSize(object value, int offset, int ordinal)
		{
			int num = this._bindtype._bufferSize;
			if (0 >= num)
			{
				if (ODBC32.SQL_C.NUMERIC == this._typemap._sql_c)
				{
					num = 518;
				}
				else
				{
					num = this._internalSize;
					if (!this._internalShouldSerializeSize || 1073741823 <= num || num < 0)
					{
						if (num <= 0 && (ParameterDirection.Output & this._internalDirection) != (ParameterDirection)0)
						{
							throw ADP.UninitializedParameterSize(ordinal, this._bindtype._type);
						}
						if (value == null || Convert.IsDBNull(value))
						{
							if (this._bindtype._sql_c == ODBC32.SQL_C.WCHAR)
							{
								num = 2;
							}
							else
							{
								num = 0;
							}
						}
						else if (value is string)
						{
							num = (((string)value).Length - offset) * 2 + 2;
						}
						else if (value is char[])
						{
							num = (((char[])value).Length - offset) * 2 + 2;
						}
						else if (value is byte[])
						{
							num = ((byte[])value).Length - offset;
						}
						if ((ParameterDirection.Output & this._internalDirection) != (ParameterDirection)0 && 1073741823 <= this._internalSize)
						{
							num = Math.Max(num, 8192);
						}
					}
					else if (ODBC32.SQL_C.WCHAR == this._bindtype._sql_c)
					{
						if (value is string && num < ((string)value).Length && this._bindtype == this._originalbindtype)
						{
							num = ((string)value).Length;
						}
						num = num * 2 + 2;
					}
					else if (value is byte[] && num < ((byte[])value).Length && this._bindtype == this._originalbindtype)
					{
						num = ((byte[])value).Length;
					}
				}
			}
			return num;
		}

		private byte GetParameterPrecision(object value)
		{
			if (this._internalPrecision != 0 && value is decimal)
			{
				if (this._internalPrecision < 29)
				{
					if (this._internalPrecision != 0)
					{
						byte precision = ((decimal)value).Precision;
						this._internalPrecision = Math.Max(this._internalPrecision, precision);
					}
					return this._internalPrecision;
				}
				return 29;
			}
			else
			{
				if (value == null || value is decimal || Convert.IsDBNull(value))
				{
					return 28;
				}
				return 0;
			}
		}

		private byte GetParameterScale(object value)
		{
			if (!(value is decimal))
			{
				return this._internalScale;
			}
			byte b = (byte)((decimal.GetBits((decimal)value)[3] & 16711680) >> 16);
			if (this._internalScale > 0 && this._internalScale < b)
			{
				return this._internalScale;
			}
			return b;
		}

		object ICloneable.Clone()
		{
			return new OdbcParameter(this);
		}

		private void CopyParameterInternal()
		{
			this._internalValue = this.Value;
			this._internalPrecision = (this.ShouldSerializePrecision() ? this.PrecisionInternal : this.ValuePrecision(this._internalValue));
			this._internalShouldSerializeSize = this.ShouldSerializeSize();
			this._internalSize = (this._internalShouldSerializeSize ? this.Size : this.ValueSize(this._internalValue));
			this._internalDirection = this.Direction;
			this._internalScale = (this.ShouldSerializeScale() ? this.ScaleInternal : this.ValueScale(this._internalValue));
			this._internalOffset = this.Offset;
			this._internalUserSpecifiedType = this.UserSpecifiedType;
		}

		private void CloneHelper(OdbcParameter destination)
		{
			this.CloneHelperCore(destination);
			destination._userSpecifiedType = this._userSpecifiedType;
			destination._typemap = this._typemap;
			destination._parameterName = this._parameterName;
			destination._precision = this._precision;
			destination._scale = this._scale;
			destination._hasScale = this._hasScale;
		}

		internal void ClearBinding()
		{
			if (!this._userSpecifiedType)
			{
				this._typemap = null;
			}
			this._bindtype = null;
		}

		internal void PrepareForBind(OdbcCommand command, short ordinal, ref int parameterBufferSize)
		{
			this.CopyParameterInternal();
			object obj = this.ProcessAndGetParameterValue();
			int num = this._internalOffset;
			int num2 = this._internalSize;
			if (num > 0)
			{
				if (obj is string)
				{
					if (num > ((string)obj).Length)
					{
						throw ADP.OffsetOutOfRangeException();
					}
				}
				else if (obj is char[])
				{
					if (num > ((char[])obj).Length)
					{
						throw ADP.OffsetOutOfRangeException();
					}
				}
				else if (obj is byte[])
				{
					if (num > ((byte[])obj).Length)
					{
						throw ADP.OffsetOutOfRangeException();
					}
				}
				else
				{
					num = 0;
				}
			}
			ODBC32.SQL_TYPE sql_TYPE = this._bindtype._sql_type;
			if (sql_TYPE - ODBC32.SQL_TYPE.WLONGVARCHAR > 2)
			{
				if (sql_TYPE != ODBC32.SQL_TYPE.BIGINT)
				{
					if (sql_TYPE - ODBC32.SQL_TYPE.NUMERIC <= 1 && (!command.Connection.IsV3Driver || !command.Connection.TestTypeSupport(ODBC32.SQL_TYPE.NUMERIC) || command.Connection.TestRestrictedSqlBindType(this._bindtype._sql_type)))
					{
						this._bindtype = TypeMap._VarChar;
						if (obj != null && !Convert.IsDBNull(obj))
						{
							obj = ((decimal)obj).ToString(CultureInfo.CurrentCulture);
							num2 = ((string)obj).Length;
							num = 0;
						}
					}
				}
				else if (!command.Connection.IsV3Driver)
				{
					this._bindtype = TypeMap._VarChar;
					if (obj != null && !Convert.IsDBNull(obj))
					{
						obj = ((long)obj).ToString(CultureInfo.CurrentCulture);
						num2 = ((string)obj).Length;
						num = 0;
					}
				}
			}
			else
			{
				if (obj is char)
				{
					obj = obj.ToString();
					num2 = ((string)obj).Length;
					num = 0;
				}
				if (!command.Connection.TestTypeSupport(this._bindtype._sql_type))
				{
					if (ODBC32.SQL_TYPE.WCHAR == this._bindtype._sql_type)
					{
						this._bindtype = TypeMap._Char;
					}
					else if (ODBC32.SQL_TYPE.WVARCHAR == this._bindtype._sql_type)
					{
						this._bindtype = TypeMap._VarChar;
					}
					else if (ODBC32.SQL_TYPE.WLONGVARCHAR == this._bindtype._sql_type)
					{
						this._bindtype = TypeMap._Text;
					}
				}
			}
			ODBC32.SQL_C sql_C = this._bindtype._sql_c;
			if (!command.Connection.IsV3Driver && sql_C == ODBC32.SQL_C.WCHAR)
			{
				sql_C = ODBC32.SQL_C.CHAR;
				if (obj != null && !Convert.IsDBNull(obj) && obj is string)
				{
					obj = Encoding.GetEncoding(new CultureInfo(CultureInfo.CurrentCulture.LCID).TextInfo.ANSICodePage).GetBytes(obj.ToString());
					num2 = ((byte[])obj).Length;
				}
			}
			int parameterSize = this.GetParameterSize(obj, num, (int)ordinal);
			sql_TYPE = this._bindtype._sql_type;
			if (sql_TYPE != ODBC32.SQL_TYPE.WVARCHAR)
			{
				if (sql_TYPE != ODBC32.SQL_TYPE.VARBINARY)
				{
					if (sql_TYPE == ODBC32.SQL_TYPE.VARCHAR)
					{
						if (num2 > 8000)
						{
							this._bindtype = TypeMap._Text;
						}
					}
				}
				else if (num2 > 8000)
				{
					this._bindtype = TypeMap._Image;
				}
			}
			else if (num2 > 4000)
			{
				this._bindtype = TypeMap._NText;
			}
			this._prepared_Sql_C_Type = sql_C;
			this._preparedOffset = num;
			this._preparedSize = num2;
			this._preparedValue = obj;
			this._preparedBufferSize = parameterSize;
			this._preparedIntOffset = parameterBufferSize;
			this._preparedValueOffset = this._preparedIntOffset + IntPtr.Size;
			parameterBufferSize += parameterSize + IntPtr.Size;
		}

		internal void Bind(OdbcStatementHandle hstmt, OdbcCommand command, short ordinal, CNativeBuffer parameterBuffer, bool allowReentrance)
		{
			ODBC32.SQL_C prepared_Sql_C_Type = this._prepared_Sql_C_Type;
			ODBC32.SQL_PARAM sql_PARAM = this.SqlDirectionFromParameterDirection();
			int preparedOffset = this._preparedOffset;
			int preparedSize = this._preparedSize;
			object obj = this._preparedValue;
			int valueSize = this.GetValueSize(obj, preparedOffset);
			int columnSize = this.GetColumnSize(obj, preparedOffset, (int)ordinal);
			byte parameterPrecision = this.GetParameterPrecision(obj);
			byte b = this.GetParameterScale(obj);
			HandleRef handleRef = parameterBuffer.PtrOffset(this._preparedValueOffset, this._preparedBufferSize);
			HandleRef handleRef2 = parameterBuffer.PtrOffset(this._preparedIntOffset, IntPtr.Size);
			if (ODBC32.SQL_C.NUMERIC == prepared_Sql_C_Type)
			{
				if (ODBC32.SQL_PARAM.INPUT_OUTPUT == sql_PARAM && obj is decimal && b < this._internalScale)
				{
					while (b < this._internalScale)
					{
						obj = (decimal)obj * 10m;
						b += 1;
					}
				}
				this.SetInputValue(obj, prepared_Sql_C_Type, valueSize, (int)parameterPrecision, 0, parameterBuffer);
				if (ODBC32.SQL_PARAM.INPUT != sql_PARAM)
				{
					parameterBuffer.WriteInt16(this._preparedValueOffset, (short)(((int)b << 8) | (int)parameterPrecision));
				}
			}
			else
			{
				this.SetInputValue(obj, prepared_Sql_C_Type, valueSize, preparedSize, preparedOffset, parameterBuffer);
			}
			if (!this._hasChanged && this._boundSqlCType == prepared_Sql_C_Type && this._boundParameterType == this._bindtype._sql_type && this._boundSize == columnSize && this._boundScale == (int)b && this._boundBuffer == handleRef.Handle && this._boundIntbuffer == handleRef2.Handle)
			{
				return;
			}
			ODBC32.RetCode retCode = hstmt.BindParameter(ordinal, (short)sql_PARAM, prepared_Sql_C_Type, this._bindtype._sql_type, (IntPtr)columnSize, (IntPtr)((int)b), handleRef, (IntPtr)this._preparedBufferSize, handleRef2);
			if (retCode != ODBC32.RetCode.SUCCESS)
			{
				if ("07006" == command.GetDiagSqlState())
				{
					command.Connection.FlagRestrictedSqlBindType(this._bindtype._sql_type);
					if (allowReentrance)
					{
						this.Bind(hstmt, command, ordinal, parameterBuffer, false);
						return;
					}
				}
				command.Connection.HandleError(hstmt, retCode);
			}
			this._hasChanged = false;
			this._boundSqlCType = prepared_Sql_C_Type;
			this._boundParameterType = this._bindtype._sql_type;
			this._boundSize = columnSize;
			this._boundScale = (int)b;
			this._boundBuffer = handleRef.Handle;
			this._boundIntbuffer = handleRef2.Handle;
			if (ODBC32.SQL_C.NUMERIC == prepared_Sql_C_Type)
			{
				OdbcDescriptorHandle descriptorHandle = command.GetDescriptorHandle(ODBC32.SQL_ATTR.APP_PARAM_DESC);
				retCode = descriptorHandle.SetDescriptionField1(ordinal, ODBC32.SQL_DESC.TYPE, (IntPtr)2);
				if (retCode != ODBC32.RetCode.SUCCESS)
				{
					command.Connection.HandleError(hstmt, retCode);
				}
				int num = (int)parameterPrecision;
				retCode = descriptorHandle.SetDescriptionField1(ordinal, ODBC32.SQL_DESC.PRECISION, (IntPtr)num);
				if (retCode != ODBC32.RetCode.SUCCESS)
				{
					command.Connection.HandleError(hstmt, retCode);
				}
				num = (int)b;
				retCode = descriptorHandle.SetDescriptionField1(ordinal, ODBC32.SQL_DESC.SCALE, (IntPtr)num);
				if (retCode != ODBC32.RetCode.SUCCESS)
				{
					command.Connection.HandleError(hstmt, retCode);
				}
				retCode = descriptorHandle.SetDescriptionField2(ordinal, ODBC32.SQL_DESC.DATA_PTR, handleRef);
				if (retCode != ODBC32.RetCode.SUCCESS)
				{
					command.Connection.HandleError(hstmt, retCode);
				}
			}
		}

		internal void GetOutputValue(CNativeBuffer parameterBuffer)
		{
			if (this._hasChanged)
			{
				return;
			}
			if (this._bindtype != null && this._internalDirection != ParameterDirection.Input)
			{
				TypeMap bindtype = this._bindtype;
				this._bindtype = null;
				int num = (int)parameterBuffer.ReadIntPtr(this._preparedIntOffset);
				if (-1 == num)
				{
					this.Value = DBNull.Value;
					return;
				}
				if (0 <= num || num == -3)
				{
					this.Value = parameterBuffer.MarshalToManaged(this._preparedValueOffset, this._boundSqlCType, num);
					if (this._boundSqlCType == ODBC32.SQL_C.CHAR && this.Value != null && !Convert.IsDBNull(this.Value))
					{
						Encoding encoding = Encoding.GetEncoding(new CultureInfo(CultureInfo.CurrentCulture.LCID).TextInfo.ANSICodePage);
						this.Value = encoding.GetString((byte[])this.Value);
					}
					if (bindtype != this._typemap && this.Value != null && !Convert.IsDBNull(this.Value) && this.Value.GetType() != this._typemap._type)
					{
						this.Value = decimal.Parse((string)this.Value, CultureInfo.CurrentCulture);
					}
				}
			}
		}

		private object ProcessAndGetParameterValue()
		{
			object obj = this._internalValue;
			if (this._internalUserSpecifiedType)
			{
				if (obj != null && !Convert.IsDBNull(obj))
				{
					Type type = obj.GetType();
					if (!type.IsArray)
					{
						if (!(type != this._typemap._type))
						{
							goto IL_00CE;
						}
						try
						{
							obj = Convert.ChangeType(obj, this._typemap._type, null);
							goto IL_00CE;
						}
						catch (Exception ex)
						{
							if (!ADP.IsCatchableExceptionType(ex))
							{
								throw;
							}
							throw ADP.ParameterConversionFailed(obj, this._typemap._type, ex);
						}
					}
					if (type == typeof(char[]))
					{
						obj = new string((char[])obj);
					}
				}
			}
			else if (this._typemap == null)
			{
				if (obj == null || Convert.IsDBNull(obj))
				{
					this._typemap = TypeMap._NVarChar;
				}
				else
				{
					Type type2 = obj.GetType();
					this._typemap = TypeMap.FromSystemType(type2);
				}
			}
			IL_00CE:
			this._originalbindtype = (this._bindtype = this._typemap);
			return obj;
		}

		private void PropertyChanging()
		{
			this._hasChanged = true;
		}

		private void PropertyTypeChanging()
		{
			this.PropertyChanging();
		}

		internal void SetInputValue(object value, ODBC32.SQL_C sql_c_type, int cbsize, int sizeorprecision, int offset, CNativeBuffer parameterBuffer)
		{
			if (ParameterDirection.Input != this._internalDirection && ParameterDirection.InputOutput != this._internalDirection)
			{
				this._internalValue = null;
				parameterBuffer.WriteIntPtr(this._preparedIntOffset, (IntPtr)(-1));
				return;
			}
			if (value == null)
			{
				parameterBuffer.WriteIntPtr(this._preparedIntOffset, (IntPtr)(-5));
				return;
			}
			if (Convert.IsDBNull(value))
			{
				parameterBuffer.WriteIntPtr(this._preparedIntOffset, (IntPtr)(-1));
				return;
			}
			if (sql_c_type == ODBC32.SQL_C.WCHAR || sql_c_type == ODBC32.SQL_C.BINARY || sql_c_type == ODBC32.SQL_C.CHAR)
			{
				parameterBuffer.WriteIntPtr(this._preparedIntOffset, (IntPtr)cbsize);
			}
			else
			{
				parameterBuffer.WriteIntPtr(this._preparedIntOffset, IntPtr.Zero);
			}
			parameterBuffer.MarshalToNative(this._preparedValueOffset, value, sql_c_type, sizeorprecision, offset);
		}

		private ODBC32.SQL_PARAM SqlDirectionFromParameterDirection()
		{
			switch (this._internalDirection)
			{
			case ParameterDirection.Input:
				return ODBC32.SQL_PARAM.INPUT;
			case ParameterDirection.Output:
			case ParameterDirection.ReturnValue:
				return ODBC32.SQL_PARAM.OUTPUT;
			case ParameterDirection.InputOutput:
				return ODBC32.SQL_PARAM.INPUT_OUTPUT;
			}
			return ODBC32.SQL_PARAM.INPUT;
		}

		public override object Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._coercedValue = null;
				this._value = value;
			}
		}

		private byte ValuePrecision(object value)
		{
			return this.ValuePrecisionCore(value);
		}

		private byte ValueScale(object value)
		{
			return this.ValueScaleCore(value);
		}

		private int ValueSize(object value)
		{
			return this.ValueSizeCore(value);
		}

		private OdbcParameter(OdbcParameter source)
			: this()
		{
			ADP.CheckArgumentNull(source, "source");
			source.CloneHelper(this);
			ICloneable cloneable = this._value as ICloneable;
			if (cloneable != null)
			{
				this._value = cloneable.Clone();
			}
		}

		private object CoercedValue
		{
			get
			{
				return this._coercedValue;
			}
			set
			{
				this._coercedValue = value;
			}
		}

		public override ParameterDirection Direction
		{
			get
			{
				ParameterDirection direction = this._direction;
				if (direction == (ParameterDirection)0)
				{
					return ParameterDirection.Input;
				}
				return direction;
			}
			set
			{
				if (this._direction == value)
				{
					return;
				}
				if (value - ParameterDirection.Input <= 2 || value == ParameterDirection.ReturnValue)
				{
					this.PropertyChanging();
					this._direction = value;
					return;
				}
				throw ADP.InvalidParameterDirection(value);
			}
		}

		public override bool IsNullable
		{
			get
			{
				return this._isNullable;
			}
			set
			{
				this._isNullable = value;
			}
		}

		public int Offset
		{
			get
			{
				return this._offset;
			}
			set
			{
				if (value < 0)
				{
					throw ADP.InvalidOffsetValue(value);
				}
				this._offset = value;
			}
		}

		public override int Size
		{
			get
			{
				int num = this._size;
				if (num == 0)
				{
					num = this.ValueSize(this.Value);
				}
				return num;
			}
			set
			{
				if (this._size != value)
				{
					if (value < -1)
					{
						throw ADP.InvalidSizeValue(value);
					}
					this.PropertyChanging();
					this._size = value;
				}
			}
		}

		private bool ShouldSerializeSize()
		{
			return this._size != 0;
		}

		public override string SourceColumn
		{
			get
			{
				string sourceColumn = this._sourceColumn;
				if (sourceColumn == null)
				{
					return ADP.StrEmpty;
				}
				return sourceColumn;
			}
			set
			{
				this._sourceColumn = value;
			}
		}

		public override bool SourceColumnNullMapping
		{
			get
			{
				return this._sourceColumnNullMapping;
			}
			set
			{
				this._sourceColumnNullMapping = value;
			}
		}

		public override DataRowVersion SourceVersion
		{
			get
			{
				DataRowVersion sourceVersion = this._sourceVersion;
				if (sourceVersion == (DataRowVersion)0)
				{
					return DataRowVersion.Current;
				}
				return sourceVersion;
			}
			set
			{
				if (value <= DataRowVersion.Current)
				{
					if (value != DataRowVersion.Original && value != DataRowVersion.Current)
					{
						goto IL_0032;
					}
				}
				else if (value != DataRowVersion.Proposed && value != DataRowVersion.Default)
				{
					goto IL_0032;
				}
				this._sourceVersion = value;
				return;
				IL_0032:
				throw ADP.InvalidDataRowVersion(value);
			}
		}

		private void CloneHelperCore(OdbcParameter destination)
		{
			destination._value = this._value;
			destination._direction = this._direction;
			destination._size = this._size;
			destination._offset = this._offset;
			destination._sourceColumn = this._sourceColumn;
			destination._sourceVersion = this._sourceVersion;
			destination._sourceColumnNullMapping = this._sourceColumnNullMapping;
			destination._isNullable = this._isNullable;
		}

		internal object CompareExchangeParent(object value, object comparand)
		{
			object parent = this._parent;
			if (comparand == parent)
			{
				this._parent = value;
			}
			return parent;
		}

		internal void ResetParent()
		{
			this._parent = null;
		}

		public override string ToString()
		{
			return this.ParameterName;
		}

		private byte ValuePrecisionCore(object value)
		{
			if (value is decimal)
			{
				return ((decimal)value).Precision;
			}
			return 0;
		}

		private byte ValueScaleCore(object value)
		{
			if (value is decimal)
			{
				return (byte)((decimal.GetBits((decimal)value)[3] & 16711680) >> 16);
			}
			return 0;
		}

		private int ValueSizeCore(object value)
		{
			if (!ADP.IsNull(value))
			{
				string text = value as string;
				if (text != null)
				{
					return text.Length;
				}
				byte[] array = value as byte[];
				if (array != null)
				{
					return array.Length;
				}
				char[] array2 = value as char[];
				if (array2 != null)
				{
					return array2.Length;
				}
				if (value is byte || value is char)
				{
					return 1;
				}
			}
			return 0;
		}

		private bool _hasChanged;

		private bool _userSpecifiedType;

		private TypeMap _typemap;

		private TypeMap _bindtype;

		private string _parameterName;

		private byte _precision;

		private byte _scale;

		private bool _hasScale;

		private ODBC32.SQL_C _boundSqlCType;

		private ODBC32.SQL_TYPE _boundParameterType;

		private int _boundSize;

		private int _boundScale;

		private IntPtr _boundBuffer;

		private IntPtr _boundIntbuffer;

		private TypeMap _originalbindtype;

		private byte _internalPrecision;

		private bool _internalShouldSerializeSize;

		private int _internalSize;

		private ParameterDirection _internalDirection;

		private byte _internalScale;

		private int _internalOffset;

		internal bool _internalUserSpecifiedType;

		private object _internalValue;

		private int _preparedOffset;

		private int _preparedSize;

		private int _preparedBufferSize;

		private object _preparedValue;

		private int _preparedIntOffset;

		private int _preparedValueOffset;

		private ODBC32.SQL_C _prepared_Sql_C_Type;

		private object _value;

		private object _parent;

		private ParameterDirection _direction;

		private int _size;

		private int _offset;

		private string _sourceColumn;

		private DataRowVersion _sourceVersion;

		private bool _sourceColumnNullMapping;

		private bool _isNullable;

		private object _coercedValue;
	}
}
