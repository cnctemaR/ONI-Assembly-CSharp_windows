using System;

namespace System.Data.Common
{
	internal sealed class Int64DataContainer : DataContainer
	{
		protected override object GetValue(int index)
		{
			return this._values[index];
		}

		protected override void ZeroOut(int index)
		{
			this._values[index] = 0L;
		}

		protected override void SetValue(int index, object value)
		{
			this._values[index] = (long)base.GetContainerData(value);
		}

		protected override void SetValueFromSafeDataRecord(int index, ISafeDataRecord record, int field)
		{
			this._values[index] = record.GetInt64Safe(field);
		}

		protected override void DoCopyValue(DataContainer from, int from_index, int to_index)
		{
			this._values[to_index] = ((Int64DataContainer)from)._values[from_index];
		}

		protected override int DoCompareValues(int index1, int index2)
		{
			long num = this._values[index1];
			long num2 = this._values[index2];
			return (num != num2) ? ((num >= num2) ? 1 : (-1)) : 0;
		}

		protected override void Resize(int size)
		{
			if (this._values == null)
			{
				this._values = new long[size];
				return;
			}
			long[] array = new long[size];
			Array.Copy(this._values, 0, array, 0, this._values.Length);
			this._values = array;
		}

		internal override long GetInt64(int index)
		{
			return this._values[index];
		}

		private long[] _values;
	}
}
