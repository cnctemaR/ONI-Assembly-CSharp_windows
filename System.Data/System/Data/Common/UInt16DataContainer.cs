using System;

namespace System.Data.Common
{
	internal sealed class UInt16DataContainer : DataContainer
	{
		protected override object GetValue(int index)
		{
			return this._values[index];
		}

		protected override void ZeroOut(int index)
		{
			this._values[index] = 0;
		}

		protected override void SetValue(int index, object value)
		{
			this._values[index] = (ushort)base.GetContainerData(value);
		}

		protected override void SetValueFromSafeDataRecord(int index, ISafeDataRecord record, int field)
		{
			this._values[index] = (ushort)record.GetInt16Safe(field);
		}

		protected override void DoCopyValue(DataContainer from, int from_index, int to_index)
		{
			this._values[to_index] = ((UInt16DataContainer)from)._values[from_index];
		}

		protected override int DoCompareValues(int index1, int index2)
		{
			int num = (int)this._values[index1];
			int num2 = (int)this._values[index2];
			return num - num2;
		}

		protected override void Resize(int size)
		{
			if (this._values == null)
			{
				this._values = new ushort[size];
				return;
			}
			ushort[] array = new ushort[size];
			Array.Copy(this._values, 0, array, 0, this._values.Length);
			this._values = array;
		}

		internal override long GetInt64(int index)
		{
			return (long)this._values[index];
		}

		private ushort[] _values;
	}
}
