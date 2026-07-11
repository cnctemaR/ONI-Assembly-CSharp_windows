using System;

namespace System.Data
{
	internal abstract class AutoIncrementValue
	{
		internal bool Auto { get; set; }

		internal abstract object Current { get; set; }

		internal abstract long Seed { get; set; }

		internal abstract long Step { get; set; }

		internal abstract Type DataType { get; }

		internal abstract void SetCurrent(object value, IFormatProvider formatProvider);

		internal abstract void SetCurrentAndIncrement(object value);

		internal abstract void MoveAfter();

		internal AutoIncrementValue Clone()
		{
			AutoIncrementInt64 autoIncrementInt = ((this is AutoIncrementInt64) ? new AutoIncrementInt64() : new AutoIncrementBigInteger());
			autoIncrementInt.Auto = this.Auto;
			autoIncrementInt.Seed = this.Seed;
			autoIncrementInt.Step = this.Step;
			autoIncrementInt.Current = this.Current;
			return autoIncrementInt;
		}
	}
}
