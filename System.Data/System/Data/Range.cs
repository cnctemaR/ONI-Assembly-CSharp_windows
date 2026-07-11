using System;

namespace System.Data
{
	internal struct Range
	{
		public Range(int min, int max)
		{
			if (min > max)
			{
				throw ExceptionBuilder.RangeArgument(min, max);
			}
			this._min = min;
			this._max = max;
			this._isNotNull = true;
		}

		public int Count
		{
			get
			{
				if (!this.IsNull)
				{
					return this._max - this._min + 1;
				}
				return 0;
			}
		}

		public bool IsNull
		{
			get
			{
				return !this._isNotNull;
			}
		}

		public int Max
		{
			get
			{
				this.CheckNull();
				return this._max;
			}
		}

		public int Min
		{
			get
			{
				this.CheckNull();
				return this._min;
			}
		}

		internal void CheckNull()
		{
			if (this.IsNull)
			{
				throw ExceptionBuilder.NullRange();
			}
		}

		private int _min;

		private int _max;

		private bool _isNotNull;
	}
}
