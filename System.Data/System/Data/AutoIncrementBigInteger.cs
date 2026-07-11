using System;
using System.Data.Common;
using System.Numerics;

namespace System.Data
{
	internal sealed class AutoIncrementBigInteger : AutoIncrementValue
	{
		internal override object Current
		{
			get
			{
				return this._current;
			}
			set
			{
				this._current = (BigInteger)value;
			}
		}

		internal override Type DataType
		{
			get
			{
				return typeof(BigInteger);
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
				return (long)this._step;
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
			this._current = BigIntegerStorage.ConvertToBigInteger(value, formatProvider);
		}

		internal override void SetCurrentAndIncrement(object value)
		{
			BigInteger bigInteger = (BigInteger)value;
			if (this.BoundaryCheck(bigInteger))
			{
				this._current = bigInteger + this._step;
			}
		}

		private bool BoundaryCheck(BigInteger value)
		{
			return (this._step < 0L && value <= this._current) || (0L < this._step && this._current <= value);
		}

		private BigInteger _current;

		private long _seed;

		private BigInteger _step = 1;
	}
}
