using System;
using System.Data.Common;
using System.Globalization;
using System.Numerics;

namespace System.Data
{
	internal sealed class AutoIncrementInt64 : AutoIncrementValue
	{
		internal override object Current
		{
			get
			{
				return this._current;
			}
			set
			{
				this._current = (long)value;
			}
		}

		internal override Type DataType
		{
			get
			{
				return typeof(long);
			}
		}

		internal override long Seed
		{
			get
			{
				return this._seed;
			}
			set
			{
				if (this._current == this._seed || this.BoundaryCheck(value))
				{
					this._current = value;
				}
				this._seed = value;
			}
		}

		internal override long Step
		{
			get
			{
				return this._step;
			}
			set
			{
				if (value == 0L)
				{
					throw ExceptionBuilder.AutoIncrementSeed();
				}
				if (this._step != value)
				{
					if (this._current != this.Seed)
					{
						this._current = this._current - this._step + value;
					}
					this._step = value;
				}
			}
		}

		internal override void MoveAfter()
		{
			this._current += this._step;
		}

		internal override void SetCurrent(object value, IFormatProvider formatProvider)
		{
			this._current = Convert.ToInt64(value, formatProvider);
		}

		internal override void SetCurrentAndIncrement(object value)
		{
			long num = (long)SqlConvert.ChangeType2(value, StorageType.Int64, typeof(long), CultureInfo.InvariantCulture);
			if (this.BoundaryCheck(num))
			{
				this._current = num + this._step;
			}
		}

		private bool BoundaryCheck(BigInteger value)
		{
			return (this._step < 0L && value <= this._current) || (0L < this._step && this._current <= value);
		}

		private long _current;

		private long _seed;

		private long _step = 1L;
	}
}
