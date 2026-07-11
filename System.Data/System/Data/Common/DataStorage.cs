using System;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Numerics;
using System.Xml;
using System.Xml.Serialization;

namespace System.Data.Common
{
	internal abstract class DataStorage
	{
		protected DataStorage(DataColumn column, Type type, object defaultValue, StorageType storageType)
			: this(column, type, defaultValue, DBNull.Value, false, storageType)
		{
		}

		protected DataStorage(DataColumn column, Type type, object defaultValue, object nullValue, StorageType storageType)
			: this(column, type, defaultValue, nullValue, false, storageType)
		{
		}

		protected DataStorage(DataColumn column, Type type, object defaultValue, object nullValue, bool isICloneable, StorageType storageType)
		{
			this._column = column;
			this._table = column.Table;
			this._dataType = type;
			this._storageTypeCode = storageType;
			this._defaultValue = defaultValue;
			this._nullValue = nullValue;
			this._isCloneable = isICloneable;
			this._isCustomDefinedType = DataStorage.IsTypeCustomType(this._storageTypeCode);
			this._isStringType = StorageType.String == this._storageTypeCode || StorageType.SqlString == this._storageTypeCode;
			this._isValueType = DataStorage.DetermineIfValueType(this._storageTypeCode, type);
		}

		internal DataSetDateTime DateTimeMode
		{
			get
			{
				return this._column.DateTimeMode;
			}
		}

		internal IFormatProvider FormatProvider
		{
			get
			{
				return this._table.FormatProvider;
			}
		}

		public virtual object Aggregate(int[] recordNos, AggregateType kind)
		{
			if (AggregateType.Count == kind)
			{
				return this.AggregateCount(recordNos);
			}
			return null;
		}

		public object AggregateCount(int[] recordNos)
		{
			int num = 0;
			for (int i = 0; i < recordNos.Length; i++)
			{
				if (!this._dbNullBits.Get(recordNos[i]))
				{
					num++;
				}
			}
			return num;
		}

		protected int CompareBits(int recordNo1, int recordNo2)
		{
			bool flag = this._dbNullBits.Get(recordNo1);
			bool flag2 = this._dbNullBits.Get(recordNo2);
			if (!(flag ^ flag2))
			{
				return 0;
			}
			if (flag)
			{
				return -1;
			}
			return 1;
		}

		public abstract int Compare(int recordNo1, int recordNo2);

		public abstract int CompareValueTo(int recordNo1, object value);

		public virtual object ConvertValue(object value)
		{
			return value;
		}

		protected void CopyBits(int srcRecordNo, int dstRecordNo)
		{
			this._dbNullBits.Set(dstRecordNo, this._dbNullBits.Get(srcRecordNo));
		}

		public abstract void Copy(int recordNo1, int recordNo2);

		public abstract object Get(int recordNo);

		protected object GetBits(int recordNo)
		{
			if (this._dbNullBits.Get(recordNo))
			{
				return this._nullValue;
			}
			return this._defaultValue;
		}

		public virtual int GetStringLength(int record)
		{
			return int.MaxValue;
		}

		protected bool HasValue(int recordNo)
		{
			return !this._dbNullBits.Get(recordNo);
		}

		public virtual bool IsNull(int recordNo)
		{
			return this._dbNullBits.Get(recordNo);
		}

		public abstract void Set(int recordNo, object value);

		protected void SetNullBit(int recordNo, bool flag)
		{
			this._dbNullBits.Set(recordNo, flag);
		}

		public virtual void SetCapacity(int capacity)
		{
			if (this._dbNullBits == null)
			{
				this._dbNullBits = new BitArray(capacity);
				return;
			}
			this._dbNullBits.Length = capacity;
		}

		public abstract object ConvertXmlToObject(string s);

		public virtual object ConvertXmlToObject(XmlReader xmlReader, XmlRootAttribute xmlAttrib)
		{
			return this.ConvertXmlToObject(xmlReader.Value);
		}

		public abstract string ConvertObjectToXml(object value);

		public virtual void ConvertObjectToXml(object value, XmlWriter xmlWriter, XmlRootAttribute xmlAttrib)
		{
			xmlWriter.WriteString(this.ConvertObjectToXml(value));
		}

		public static DataStorage CreateStorage(DataColumn column, Type dataType, StorageType typeCode)
		{
			if (typeCode != StorageType.Empty || !(null != dataType))
			{
				switch (typeCode)
				{
				case StorageType.Empty:
					throw ExceptionBuilder.InvalidStorageType(TypeCode.Empty);
				case StorageType.DBNull:
					throw ExceptionBuilder.InvalidStorageType(TypeCode.DBNull);
				case StorageType.Boolean:
					return new BooleanStorage(column);
				case StorageType.Char:
					return new CharStorage(column);
				case StorageType.SByte:
					return new SByteStorage(column);
				case StorageType.Byte:
					return new ByteStorage(column);
				case StorageType.Int16:
					return new Int16Storage(column);
				case StorageType.UInt16:
					return new UInt16Storage(column);
				case StorageType.Int32:
					return new Int32Storage(column);
				case StorageType.UInt32:
					return new UInt32Storage(column);
				case StorageType.Int64:
					return new Int64Storage(column);
				case StorageType.UInt64:
					return new UInt64Storage(column);
				case StorageType.Single:
					return new SingleStorage(column);
				case StorageType.Double:
					return new DoubleStorage(column);
				case StorageType.Decimal:
					return new DecimalStorage(column);
				case StorageType.DateTime:
					return new DateTimeStorage(column);
				case StorageType.TimeSpan:
					return new TimeSpanStorage(column);
				case StorageType.String:
					return new StringStorage(column);
				case StorageType.Guid:
					return new ObjectStorage(column, dataType);
				case StorageType.ByteArray:
					return new ObjectStorage(column, dataType);
				case StorageType.CharArray:
					return new ObjectStorage(column, dataType);
				case StorageType.Type:
					return new ObjectStorage(column, dataType);
				case StorageType.DateTimeOffset:
					return new DateTimeOffsetStorage(column);
				case StorageType.BigInteger:
					return new BigIntegerStorage(column);
				case StorageType.Uri:
					return new ObjectStorage(column, dataType);
				case StorageType.SqlBinary:
					return new SqlBinaryStorage(column);
				case StorageType.SqlBoolean:
					return new SqlBooleanStorage(column);
				case StorageType.SqlByte:
					return new SqlByteStorage(column);
				case StorageType.SqlBytes:
					return new SqlBytesStorage(column);
				case StorageType.SqlChars:
					return new SqlCharsStorage(column);
				case StorageType.SqlDateTime:
					return new SqlDateTimeStorage(column);
				case StorageType.SqlDecimal:
					return new SqlDecimalStorage(column);
				case StorageType.SqlDouble:
					return new SqlDoubleStorage(column);
				case StorageType.SqlGuid:
					return new SqlGuidStorage(column);
				case StorageType.SqlInt16:
					return new SqlInt16Storage(column);
				case StorageType.SqlInt32:
					return new SqlInt32Storage(column);
				case StorageType.SqlInt64:
					return new SqlInt64Storage(column);
				case StorageType.SqlMoney:
					return new SqlMoneyStorage(column);
				case StorageType.SqlSingle:
					return new SqlSingleStorage(column);
				case StorageType.SqlString:
					return new SqlStringStorage(column);
				}
				return new ObjectStorage(column, dataType);
			}
			if (typeof(INullable).IsAssignableFrom(dataType))
			{
				return new SqlUdtStorage(column, dataType);
			}
			return new ObjectStorage(column, dataType);
		}

		internal static StorageType GetStorageType(Type dataType)
		{
			for (int i = 0; i < DataStorage.s_storageClassType.Length; i++)
			{
				if (dataType == DataStorage.s_storageClassType[i])
				{
					return (StorageType)i;
				}
			}
			TypeCode typeCode = Type.GetTypeCode(dataType);
			if (TypeCode.Object != typeCode)
			{
				return (StorageType)typeCode;
			}
			return StorageType.Empty;
		}

		internal static Type GetTypeStorage(StorageType storageType)
		{
			return DataStorage.s_storageClassType[(int)storageType];
		}

		internal static bool IsTypeCustomType(Type type)
		{
			return DataStorage.IsTypeCustomType(DataStorage.GetStorageType(type));
		}

		internal static bool IsTypeCustomType(StorageType typeCode)
		{
			return StorageType.Object == typeCode || typeCode == StorageType.Empty || StorageType.CharArray == typeCode;
		}

		internal static bool IsSqlType(StorageType storageType)
		{
			return StorageType.SqlBinary <= storageType;
		}

		public static bool IsSqlType(Type dataType)
		{
			for (int i = 26; i < DataStorage.s_storageClassType.Length; i++)
			{
				if (dataType == DataStorage.s_storageClassType[i])
				{
					return true;
				}
			}
			return false;
		}

		private static bool DetermineIfValueType(StorageType typeCode, Type dataType)
		{
			bool flag;
			switch (typeCode)
			{
			case StorageType.Boolean:
			case StorageType.Char:
			case StorageType.SByte:
			case StorageType.Byte:
			case StorageType.Int16:
			case StorageType.UInt16:
			case StorageType.Int32:
			case StorageType.UInt32:
			case StorageType.Int64:
			case StorageType.UInt64:
			case StorageType.Single:
			case StorageType.Double:
			case StorageType.Decimal:
			case StorageType.DateTime:
			case StorageType.TimeSpan:
			case StorageType.Guid:
			case StorageType.DateTimeOffset:
			case StorageType.BigInteger:
			case StorageType.SqlBinary:
			case StorageType.SqlBoolean:
			case StorageType.SqlByte:
			case StorageType.SqlDateTime:
			case StorageType.SqlDecimal:
			case StorageType.SqlDouble:
			case StorageType.SqlGuid:
			case StorageType.SqlInt16:
			case StorageType.SqlInt32:
			case StorageType.SqlInt64:
			case StorageType.SqlMoney:
			case StorageType.SqlSingle:
			case StorageType.SqlString:
				flag = true;
				break;
			case StorageType.String:
			case StorageType.ByteArray:
			case StorageType.CharArray:
			case StorageType.Type:
			case StorageType.Uri:
			case StorageType.SqlBytes:
			case StorageType.SqlChars:
				flag = false;
				break;
			default:
				flag = dataType.IsValueType;
				break;
			}
			return flag;
		}

		internal static void ImplementsInterfaces(StorageType typeCode, Type dataType, out bool sqlType, out bool nullable, out bool xmlSerializable, out bool changeTracking, out bool revertibleChangeTracking)
		{
			if (DataStorage.IsSqlType(typeCode))
			{
				sqlType = true;
				nullable = true;
				changeTracking = false;
				revertibleChangeTracking = false;
				xmlSerializable = true;
				return;
			}
			if (typeCode != StorageType.Empty)
			{
				sqlType = false;
				nullable = false;
				changeTracking = false;
				revertibleChangeTracking = false;
				xmlSerializable = false;
				return;
			}
			Tuple<bool, bool, bool, bool> orAdd = DataStorage.s_typeImplementsInterface.GetOrAdd(dataType, DataStorage.s_inspectTypeForInterfaces);
			sqlType = false;
			nullable = orAdd.Item1;
			changeTracking = orAdd.Item2;
			revertibleChangeTracking = orAdd.Item3;
			xmlSerializable = orAdd.Item4;
		}

		private static Tuple<bool, bool, bool, bool> InspectTypeForInterfaces(Type dataType)
		{
			return new Tuple<bool, bool, bool, bool>(typeof(INullable).IsAssignableFrom(dataType), typeof(IChangeTracking).IsAssignableFrom(dataType), typeof(IRevertibleChangeTracking).IsAssignableFrom(dataType), typeof(IXmlSerializable).IsAssignableFrom(dataType));
		}

		internal static bool ImplementsINullableValue(StorageType typeCode, Type dataType)
		{
			return typeCode == StorageType.Empty && dataType.IsGenericType && dataType.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public static bool IsObjectNull(object value)
		{
			return value == null || DBNull.Value == value || DataStorage.IsObjectSqlNull(value);
		}

		public static bool IsObjectSqlNull(object value)
		{
			INullable nullable = value as INullable;
			return nullable != null && nullable.IsNull;
		}

		internal object GetEmptyStorageInternal(int recordCount)
		{
			return this.GetEmptyStorage(recordCount);
		}

		internal void CopyValueInternal(int record, object store, BitArray nullbits, int storeIndex)
		{
			this.CopyValue(record, store, nullbits, storeIndex);
		}

		internal void SetStorageInternal(object store, BitArray nullbits)
		{
			this.SetStorage(store, nullbits);
		}

		protected abstract object GetEmptyStorage(int recordCount);

		protected abstract void CopyValue(int record, object store, BitArray nullbits, int storeIndex);

		protected abstract void SetStorage(object store, BitArray nullbits);

		protected void SetNullStorage(BitArray nullbits)
		{
			this._dbNullBits = nullbits;
		}

		internal static Type GetType(string value)
		{
			Type type = Type.GetType(value);
			if (null == type && "System.Numerics.BigInteger" == value)
			{
				type = typeof(BigInteger);
			}
			ObjectStorage.VerifyIDynamicMetaObjectProvider(type);
			return type;
		}

		internal static string GetQualifiedName(Type type)
		{
			ObjectStorage.VerifyIDynamicMetaObjectProvider(type);
			return type.AssemblyQualifiedName;
		}

		private static readonly Type[] s_storageClassType = new Type[]
		{
			null,
			typeof(object),
			typeof(DBNull),
			typeof(bool),
			typeof(char),
			typeof(sbyte),
			typeof(byte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(decimal),
			typeof(DateTime),
			typeof(TimeSpan),
			typeof(string),
			typeof(Guid),
			typeof(byte[]),
			typeof(char[]),
			typeof(Type),
			typeof(DateTimeOffset),
			typeof(BigInteger),
			typeof(Uri),
			typeof(SqlBinary),
			typeof(SqlBoolean),
			typeof(SqlByte),
			typeof(SqlBytes),
			typeof(SqlChars),
			typeof(SqlDateTime),
			typeof(SqlDecimal),
			typeof(SqlDouble),
			typeof(SqlGuid),
			typeof(SqlInt16),
			typeof(SqlInt32),
			typeof(SqlInt64),
			typeof(SqlMoney),
			typeof(SqlSingle),
			typeof(SqlString)
		};

		internal readonly DataColumn _column;

		internal readonly DataTable _table;

		internal readonly Type _dataType;

		internal readonly StorageType _storageTypeCode;

		private BitArray _dbNullBits;

		private readonly object _defaultValue;

		internal readonly object _nullValue;

		internal readonly bool _isCloneable;

		internal readonly bool _isCustomDefinedType;

		internal readonly bool _isStringType;

		internal readonly bool _isValueType;

		private static readonly Func<Type, Tuple<bool, bool, bool, bool>> s_inspectTypeForInterfaces = new Func<Type, Tuple<bool, bool, bool, bool>>(DataStorage.InspectTypeForInterfaces);

		private static readonly ConcurrentDictionary<Type, Tuple<bool, bool, bool, bool>> s_typeImplementsInterface = new ConcurrentDictionary<Type, Tuple<bool, bool, bool, bool>>();
	}
}
