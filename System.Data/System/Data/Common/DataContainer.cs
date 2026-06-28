using System;
using System.Collections;
using System.Reflection;

namespace System.Data.Common
{
	internal abstract class DataContainer
	{
		protected abstract object GetValue(int index);

		internal abstract long GetInt64(int index);

		protected abstract void ZeroOut(int index);

		protected abstract void SetValue(int index, object value);

		protected abstract void SetValueFromSafeDataRecord(int index, ISafeDataRecord record, int field);

		protected abstract void DoCopyValue(DataContainer from, int from_index, int to_index);

		protected abstract int DoCompareValues(int index1, int index2);

		protected abstract void Resize(int length);

		internal object this[int index]
		{
			get
			{
				return (!this.IsNull(index)) ? this.GetValue(index) : DBNull.Value;
			}
			set
			{
				if (value == null)
				{
					this.CopyValue(this.Column.Table.DefaultValuesRowIndex, index);
					return;
				}
				bool flag = value == DBNull.Value;
				if (flag)
				{
					this.ZeroOut(index);
				}
				else
				{
					this.SetValue(index, value);
				}
				this.null_values[index] = flag;
			}
		}

		internal int Capacity
		{
			get
			{
				return (this.null_values == null) ? 0 : this.null_values.Count;
			}
			set
			{
				int capacity = this.Capacity;
				if (value == capacity)
				{
					return;
				}
				if (this.null_values == null)
				{
					this.null_values = new BitArray(value);
				}
				else
				{
					this.null_values.Length = value;
				}
				this.Resize(value);
			}
		}

		internal Type Type
		{
			get
			{
				return this._type;
			}
		}

		protected DataColumn Column
		{
			get
			{
				return this._column;
			}
		}

		internal static DataContainer Create(Type type, DataColumn column)
		{
			DataContainer dataContainer;
			switch (Type.GetTypeCode(type))
			{
			case TypeCode.Boolean:
				dataContainer = new BitDataContainer();
				goto IL_0104;
			case TypeCode.Char:
				dataContainer = new CharDataContainer();
				goto IL_0104;
			case TypeCode.SByte:
				dataContainer = new SByteDataContainer();
				goto IL_0104;
			case TypeCode.Byte:
				dataContainer = new ByteDataContainer();
				goto IL_0104;
			case TypeCode.Int16:
				dataContainer = new Int16DataContainer();
				goto IL_0104;
			case TypeCode.UInt16:
				dataContainer = new UInt16DataContainer();
				goto IL_0104;
			case TypeCode.Int32:
				dataContainer = new Int32DataContainer();
				goto IL_0104;
			case TypeCode.UInt32:
				dataContainer = new UInt32DataContainer();
				goto IL_0104;
			case TypeCode.Int64:
				dataContainer = new Int64DataContainer();
				goto IL_0104;
			case TypeCode.UInt64:
				dataContainer = new UInt64DataContainer();
				goto IL_0104;
			case TypeCode.Single:
				dataContainer = new SingleDataContainer();
				goto IL_0104;
			case TypeCode.Double:
				dataContainer = new DoubleDataContainer();
				goto IL_0104;
			case TypeCode.Decimal:
				dataContainer = new DecimalDataContainer();
				goto IL_0104;
			case TypeCode.DateTime:
				dataContainer = new DateTimeDataContainer();
				goto IL_0104;
			case TypeCode.String:
				dataContainer = new StringDataContainer();
				goto IL_0104;
			}
			dataContainer = new ObjectDataContainer();
			IL_0104:
			dataContainer._type = type;
			dataContainer._column = column;
			return dataContainer;
		}

		internal static object GetExplicitValue(object value)
		{
			Type type = value.GetType();
			MethodInfo method = type.GetMethod("op_Explicit", new Type[] { type });
			if (method != null)
			{
				return method.Invoke(value, new object[] { value });
			}
			return null;
		}

		internal object GetContainerData(object value)
		{
			if (this._type.IsInstanceOfType(value))
			{
				return value;
			}
			if (value is IConvertible)
			{
				switch (Type.GetTypeCode(this._type))
				{
				case TypeCode.Boolean:
					return Convert.ToBoolean(value);
				case TypeCode.Char:
					return Convert.ToChar(value);
				case TypeCode.SByte:
					return Convert.ToSByte(value);
				case TypeCode.Byte:
					return Convert.ToByte(value);
				case TypeCode.Int16:
					return Convert.ToInt16(value);
				case TypeCode.UInt16:
					return Convert.ToUInt16(value);
				case TypeCode.Int32:
					return Convert.ToInt32(value);
				case TypeCode.UInt32:
					return Convert.ToUInt32(value);
				case TypeCode.Int64:
					return Convert.ToInt64(value);
				case TypeCode.UInt64:
					return Convert.ToUInt64(value);
				case TypeCode.Single:
					return Convert.ToSingle(value);
				case TypeCode.Double:
					return Convert.ToDouble(value);
				case TypeCode.Decimal:
					return Convert.ToDecimal(value);
				case TypeCode.DateTime:
					return Convert.ToDateTime(value);
				case TypeCode.String:
					return Convert.ToString(value);
				}
				throw new InvalidCastException();
			}
			object explicitValue;
			if ((explicitValue = DataContainer.GetExplicitValue(value)) != null)
			{
				return explicitValue;
			}
			throw new InvalidCastException();
		}

		internal bool IsNull(int index)
		{
			return this.null_values == null || this.null_values[index];
		}

		internal void FillValues(int fromIndex)
		{
			for (int i = 0; i < this.Capacity; i++)
			{
				this.CopyValue(fromIndex, i);
			}
		}

		internal void CopyValue(int from_index, int to_index)
		{
			this.CopyValue(this, from_index, to_index);
		}

		internal void CopyValue(DataContainer from, int from_index, int to_index)
		{
			this.DoCopyValue(from, from_index, to_index);
			this.null_values[to_index] = from.null_values[from_index];
		}

		internal void SetItemFromDataRecord(int index, IDataRecord record, int field)
		{
			if (record.IsDBNull(field))
			{
				this[index] = DBNull.Value;
			}
			else if (record is ISafeDataRecord)
			{
				this.SetValueFromSafeDataRecord(index, (ISafeDataRecord)record, field);
			}
			else
			{
				this[index] = record.GetValue(field);
			}
		}

		internal int CompareValues(int index1, int index2)
		{
			bool flag = this.IsNull(index1);
			bool flag2 = this.IsNull(index2);
			if (flag == flag2)
			{
				return (!flag) ? this.DoCompareValues(index1, index2) : 0;
			}
			return (!flag) ? 1 : (-1);
		}

		private BitArray null_values;

		private Type _type;

		private DataColumn _column;
	}
}
