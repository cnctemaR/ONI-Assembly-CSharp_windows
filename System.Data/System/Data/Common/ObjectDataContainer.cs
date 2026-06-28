using System;

namespace System.Data.Common
{
	internal class ObjectDataContainer : DataContainer
	{
		protected override object GetValue(int index)
		{
			return this._values[index];
		}

		protected override void ZeroOut(int index)
		{
			this._values[index] = null;
		}

		protected override void SetValue(int index, object value)
		{
			this._values[index] = value;
		}

		protected override void SetValueFromSafeDataRecord(int index, ISafeDataRecord record, int field)
		{
			this._values[index] = record.GetValue(field);
		}

		protected override void DoCopyValue(DataContainer from, int from_index, int to_index)
		{
			this._values[to_index] = ((ObjectDataContainer)from)._values[from_index];
		}

		protected override int DoCompareValues(int index1, int index2)
		{
			object obj = this._values[index1];
			object obj2 = this._values[index2];
			if (obj == obj2)
			{
				return 0;
			}
			if (obj is IComparable)
			{
				try
				{
					return ((IComparable)obj).CompareTo(obj2);
				}
				catch
				{
					if (obj2 is IComparable)
					{
						obj2 = Convert.ChangeType(obj2, Type.GetTypeCode(obj.GetType()));
						return ((IComparable)obj).CompareTo(obj2);
					}
				}
			}
			return string.Compare(obj.ToString(), obj2.ToString());
		}

		protected override void Resize(int size)
		{
			if (this._values == null)
			{
				this._values = new object[size];
				return;
			}
			object[] array = new object[size];
			Array.Copy(this._values, 0, array, 0, this._values.Length);
			this._values = array;
		}

		internal override long GetInt64(int index)
		{
			return Convert.ToInt64(this._values[index]);
		}

		private object[] _values;
	}
}
