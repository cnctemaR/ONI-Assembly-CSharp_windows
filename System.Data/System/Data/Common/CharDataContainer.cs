using System;

namespace System.Data.Common
{
	internal sealed class CharDataContainer : DataContainer
	{
		protected override object GetValue(int index)
		{
			return this._values[index];
		}

		protected override void ZeroOut(int index)
		{
			this._values[index] = '\0';
		}

		protected override void SetValue(int index, object value)
		{
			this._values[index] = (char)base.GetContainerData(value);
		}

		protected override void SetValueFromSafeDataRecord(int index, ISafeDataRecord record, int field)
		{
			this._values[index] = record.GetCharSafe(field);
		}

		protected override void DoCopyValue(DataContainer from, int from_index, int to_index)
		{
			this._values[to_index] = ((CharDataContainer)from)._values[from_index];
		}

		protected override int DoCompareValues(int index1, int index2)
		{
			char c = this._values[index1];
			char c2 = this._values[index2];
			return (c != c2) ? ((c >= c2) ? 1 : (-1)) : 0;
		}

		protected override void Resize(int size)
		{
			if (this._values == null)
			{
				this._values = new char[size];
				return;
			}
			char[] array = new char[size];
			Array.Copy(this._values, 0, array, 0, this._values.Length);
			this._values = array;
		}

		internal override long GetInt64(int index)
		{
			return Convert.ToInt64(this._values[index]);
		}

		private char[] _values;
	}
}
