using System;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.Odbc
{
	[TypeConverter("System.Data.Odbc.OdbcParameter+OdbcParameterConverter, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
	public sealed class OdbcParameter : DbParameter, IDataParameter, IDbDataParameter, ICloneable
	{
		public OdbcParameter()
		{
			this._cbLengthInd = new NativeBuffer();
			this.ParameterName = string.Empty;
			this.IsNullable = false;
			this.SourceColumn = string.Empty;
			this.Direction = ParameterDirection.Input;
			this._typeMap = OdbcTypeConverter.GetTypeMap(OdbcType.NVarChar);
		}

		public OdbcParameter(string name, object value)
			: this()
		{
			this.ParameterName = name;
			this.Value = value;
			this._typeMap = OdbcTypeConverter.InferFromValue(value);
			if (value != null && !value.GetType().IsValueType)
			{
				Type type = value.GetType();
				if (type.IsArray)
				{
					this.Size = ((type.GetElementType() != typeof(byte)) ? 0 : ((Array)value).Length);
				}
				else
				{
					this.Size = value.ToString().Length;
				}
			}
		}

		public OdbcParameter(string name, OdbcType type)
			: this()
		{
			this.ParameterName = name;
			this._typeMap = OdbcTypeConverter.GetTypeMap(type);
		}

		public OdbcParameter(string name, OdbcType type, int size)
			: this(name, type)
		{
			this.Size = size;
		}

		public OdbcParameter(string name, OdbcType type, int size, string sourcecolumn)
			: this(name, type, size)
		{
			this.SourceColumn = sourcecolumn;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public OdbcParameter(string parameterName, OdbcType odbcType, int size, ParameterDirection parameterDirection, bool isNullable, byte precision, byte scale, string srcColumn, DataRowVersion srcVersion, object value)
			: this(parameterName, odbcType, size, srcColumn)
		{
			this.Direction = parameterDirection;
			this.IsNullable = isNullable;
			this.SourceVersion = srcVersion;
		}

		[MonoTODO]
		object ICloneable.Clone()
		{
			throw new NotImplementedException();
		}

		internal OdbcParameterCollection Container
		{
			get
			{
				return this.container;
			}
			set
			{
				this.container = value;
			}
		}

		[OdbcDescription("The parameter generic type")]
		[OdbcCategory("Data")]
		public override DbType DbType
		{
			get
			{
				return this._typeMap.DbType;
			}
			set
			{
				if (value == this._typeMap.DbType)
				{
					return;
				}
				this._typeMap = OdbcTypeConverter.GetTypeMap(value);
			}
		}

		[OdbcDescription("Input, output, or bidirectional parameter")]
		[RefreshProperties(RefreshProperties.All)]
		[OdbcCategory("Data")]
		public override ParameterDirection Direction
		{
			get
			{
				return this.direction;
			}
			set
			{
				this.direction = value;
			}
		}

		[OdbcDescription("A design-time property used for strongly typed code generation")]
		public override bool IsNullable
		{
			get
			{
				return this.isNullable;
			}
			set
			{
				this.isNullable = value;
			}
		}

		[OdbcDescription("The parameter native type")]
		[DbProviderSpecificTypeProperty(true)]
		[OdbcCategory("Data")]
		[DefaultValue(OdbcType.NChar)]
		[RefreshProperties(RefreshProperties.All)]
		public OdbcType OdbcType
		{
			get
			{
				return this._typeMap.OdbcType;
			}
			set
			{
				if (value == this._typeMap.OdbcType)
				{
					return;
				}
				this._typeMap = OdbcTypeConverter.GetTypeMap(value);
			}
		}

		[OdbcDescription("DataParameter_ParameterName")]
		public override string ParameterName
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[OdbcCategory("DataCategory_Data")]
		[OdbcDescription("DbDataParameter_Precision")]
		[DefaultValue(0)]
		public byte Precision
		{
			get
			{
				return this._precision;
			}
			set
			{
				this._precision = value;
			}
		}

		[DefaultValue(0)]
		[OdbcDescription("DbDataParameter_Scale")]
		[OdbcCategory("DataCategory_Data")]
		public byte Scale
		{
			get
			{
				return this._scale;
			}
			set
			{
				this._scale = value;
			}
		}

		[OdbcCategory("DataCategory_Data")]
		[OdbcDescription("DbDataParameter_Size")]
		public override int Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		[OdbcCategory("DataCategory_Data")]
		[OdbcDescription("DataParameter_SourceColumn")]
		public override string SourceColumn
		{
			get
			{
				return this.sourceColumn;
			}
			set
			{
				this.sourceColumn = value;
			}
		}

		[OdbcCategory("DataCategory_Data")]
		[OdbcDescription("DataParameter_SourceVersion")]
		public override DataRowVersion SourceVersion
		{
			get
			{
				return this.sourceVersion;
			}
			set
			{
				this.sourceVersion = value;
			}
		}

		[OdbcDescription("DataParameter_Value")]
		[RefreshProperties(RefreshProperties.All)]
		[TypeConverter(typeof(StringConverter))]
		[OdbcCategory("DataCategory_Data")]
		public override object Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		internal void Bind(OdbcCommand command, IntPtr hstmt, int ParamNum)
		{
			OdbcInputOutputDirection odbcInputOutputDirection = libodbc.ConvertParameterDirection(this.Direction);
			this._cbLengthInd.EnsureAlloc(Marshal.SizeOf(typeof(int)));
			int num;
			if (this.Value is DBNull)
			{
				num = -1;
			}
			else
			{
				num = this.GetNativeSize();
				this.AllocateBuffer();
			}
			Marshal.WriteInt32(this._cbLengthInd, num);
			OdbcReturn odbcReturn = libodbc.SQLBindParameter(hstmt, (ushort)ParamNum, (short)odbcInputOutputDirection, this._typeMap.NativeType, this._typeMap.SqlType, Convert.ToUInt32(this.Size), 0, this._nativeBuffer, 0, this._cbLengthInd);
			if (odbcReturn != OdbcReturn.Success && odbcReturn != OdbcReturn.SuccessWithInfo)
			{
				throw command.Connection.CreateOdbcException(OdbcHandleType.Stmt, hstmt);
			}
		}

		public override string ToString()
		{
			return this.ParameterName;
		}

		private int GetNativeSize()
		{
			TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
			Encoding encoding = Encoding.GetEncoding(textInfo.ANSICodePage);
			switch (this._typeMap.OdbcType)
			{
			case OdbcType.BigInt:
				return Marshal.SizeOf(typeof(long));
			case OdbcType.Binary:
				if (this.Value.GetType().IsArray && this.Value.GetType().GetElementType() == typeof(byte))
				{
					return ((Array)this.Value).Length;
				}
				return this.Value.ToString().Length;
			case OdbcType.Bit:
				return Marshal.SizeOf(typeof(byte));
			case OdbcType.Char:
			case OdbcType.Text:
			case OdbcType.VarChar:
				return encoding.GetByteCount(Convert.ToString(this.Value)) + 1;
			case OdbcType.DateTime:
			case OdbcType.SmallDateTime:
			case OdbcType.Timestamp:
			case OdbcType.Date:
			case OdbcType.Time:
				return 18;
			case OdbcType.Decimal:
			case OdbcType.Numeric:
				return 19;
			case OdbcType.Double:
				return Marshal.SizeOf(typeof(double));
			case OdbcType.Image:
			case OdbcType.VarBinary:
				if (this.Value.GetType().IsArray && this.Value.GetType().GetElementType() == typeof(byte))
				{
					return ((Array)this.Value).Length;
				}
				throw new ArgumentException("Unsupported Native Type!");
			case OdbcType.Int:
				return Marshal.SizeOf(typeof(int));
			case OdbcType.NChar:
			case OdbcType.NText:
			case OdbcType.NVarChar:
				return encoding.GetByteCount(Convert.ToString(this.Value)) + 1;
			case OdbcType.Real:
				return Marshal.SizeOf(typeof(float));
			case OdbcType.UniqueIdentifier:
				return Marshal.SizeOf(typeof(Guid));
			case OdbcType.SmallInt:
				return Marshal.SizeOf(typeof(short));
			case OdbcType.TinyInt:
				return Marshal.SizeOf(typeof(byte));
			default:
				if (this.Value.GetType().IsArray && this.Value.GetType().GetElementType() == typeof(byte))
				{
					return ((Array)this.Value).Length;
				}
				return this.Value.ToString().Length;
			}
		}

		private void AllocateBuffer()
		{
			int nativeSize = this.GetNativeSize();
			if (this._nativeBuffer.Size == nativeSize)
			{
				return;
			}
			this._nativeBuffer.AllocBuffer(nativeSize);
		}

		internal void CopyValue()
		{
			if (this._nativeBuffer.Handle == IntPtr.Zero)
			{
				return;
			}
			if (this.Value is DBNull)
			{
				return;
			}
			TextInfo textInfo = CultureInfo.InvariantCulture.TextInfo;
			Encoding encoding = Encoding.GetEncoding(textInfo.ANSICodePage);
			switch (this._typeMap.OdbcType)
			{
			case OdbcType.BigInt:
				Marshal.WriteInt64(this._nativeBuffer, Convert.ToInt64(this.Value));
				return;
			case OdbcType.Binary:
			case OdbcType.Image:
			case OdbcType.VarBinary:
				if (this.Value.GetType().IsArray && this.Value.GetType().GetElementType() == typeof(byte))
				{
					Marshal.Copy((byte[])this.Value, 0, this._nativeBuffer, ((byte[])this.Value).Length);
					return;
				}
				throw new ArgumentException("Unsupported Native Type!");
			case OdbcType.Bit:
				Marshal.WriteByte(this._nativeBuffer, Convert.ToByte(this.Value));
				return;
			case OdbcType.Char:
			case OdbcType.Text:
			case OdbcType.VarChar:
			{
				byte[] array = new byte[this.GetNativeSize()];
				byte[] array2 = encoding.GetBytes(Convert.ToString(this.Value));
				Array.Copy(array2, 0, array, 0, array2.Length);
				array[array.Length - 1] = 0;
				Marshal.Copy(array, 0, this._nativeBuffer, array.Length);
				Marshal.WriteInt32(this._cbLengthInd, -3);
				return;
			}
			case OdbcType.DateTime:
			case OdbcType.SmallDateTime:
			case OdbcType.Timestamp:
			{
				DateTime dateTime = (DateTime)this.Value;
				Marshal.WriteInt16(this._nativeBuffer, 0, (short)dateTime.Year);
				Marshal.WriteInt16(this._nativeBuffer, 2, (short)dateTime.Month);
				Marshal.WriteInt16(this._nativeBuffer, 4, (short)dateTime.Day);
				Marshal.WriteInt16(this._nativeBuffer, 6, (short)dateTime.Hour);
				Marshal.WriteInt16(this._nativeBuffer, 8, (short)dateTime.Minute);
				Marshal.WriteInt16(this._nativeBuffer, 10, (short)dateTime.Second);
				Marshal.WriteInt32(this._nativeBuffer, 12, (int)(dateTime.Ticks % 10000000L) * 100);
				return;
			}
			case OdbcType.Decimal:
			case OdbcType.Numeric:
			{
				int[] bits = decimal.GetBits(Convert.ToDecimal(this.Value));
				byte[] array = new byte[19];
				array[0] = this.Precision;
				array[1] = (byte)((bits[3] & 16711680) >> 16);
				array[2] = ((((long)bits[3] & (long)((ulong)int.MinValue)) <= 0L) ? 1 : 2);
				Buffer.BlockCopy(bits, 0, array, 3, 12);
				for (int i = 16; i < 19; i++)
				{
					array[i] = 0;
				}
				Marshal.Copy(array, 0, this._nativeBuffer, 19);
				return;
			}
			case OdbcType.Double:
				Marshal.StructureToPtr(Convert.ToDouble(this.Value), this._nativeBuffer, false);
				return;
			case OdbcType.Int:
				Marshal.WriteInt32(this._nativeBuffer, Convert.ToInt32(this.Value));
				return;
			case OdbcType.NChar:
			case OdbcType.NText:
			case OdbcType.NVarChar:
			{
				byte[] array = new byte[this.GetNativeSize()];
				byte[] array2 = encoding.GetBytes(Convert.ToString(this.Value));
				Array.Copy(array2, 0, array, 0, array2.Length);
				array[array.Length - 1] = 0;
				Marshal.Copy(array, 0, this._nativeBuffer, array.Length);
				Marshal.WriteInt32(this._cbLengthInd, -3);
				return;
			}
			case OdbcType.Real:
				Marshal.StructureToPtr(Convert.ToSingle(this.Value), this._nativeBuffer, false);
				return;
			case OdbcType.UniqueIdentifier:
				throw new NotImplementedException();
			case OdbcType.SmallInt:
				Marshal.WriteInt16(this._nativeBuffer, Convert.ToInt16(this.Value));
				return;
			case OdbcType.TinyInt:
				Marshal.WriteByte(this._nativeBuffer, Convert.ToByte(this.Value));
				return;
			case OdbcType.Date:
			{
				DateTime dateTime = (DateTime)this.Value;
				Marshal.WriteInt16(this._nativeBuffer, 0, (short)dateTime.Year);
				Marshal.WriteInt16(this._nativeBuffer, 2, (short)dateTime.Month);
				Marshal.WriteInt16(this._nativeBuffer, 4, (short)dateTime.Day);
				return;
			}
			case OdbcType.Time:
			{
				DateTime dateTime = (DateTime)this.Value;
				Marshal.WriteInt16(this._nativeBuffer, 0, (short)dateTime.Hour);
				Marshal.WriteInt16(this._nativeBuffer, 2, (short)dateTime.Minute);
				Marshal.WriteInt16(this._nativeBuffer, 4, (short)dateTime.Second);
				return;
			}
			default:
				if (this.Value.GetType().IsArray && this.Value.GetType().GetElementType() == typeof(byte))
				{
					Marshal.Copy((byte[])this.Value, 0, this._nativeBuffer, ((byte[])this.Value).Length);
					return;
				}
				throw new ArgumentException("Unsupported Native Type!");
			}
		}

		public override bool SourceColumnNullMapping
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public override void ResetDbType()
		{
			this._typeMap = OdbcTypeConverter.GetTypeMap(OdbcType.NVarChar);
		}

		public void ResetOdbcType()
		{
			this._typeMap = OdbcTypeConverter.GetTypeMap(OdbcType.NVarChar);
		}

		private string name;

		private ParameterDirection direction;

		private bool isNullable;

		private int size;

		private DataRowVersion sourceVersion;

		private string sourceColumn;

		private byte _precision;

		private byte _scale;

		private object _value;

		private OdbcTypeMap _typeMap;

		private NativeBuffer _nativeBuffer = new NativeBuffer();

		private NativeBuffer _cbLengthInd;

		private OdbcParameterCollection container;
	}
}
