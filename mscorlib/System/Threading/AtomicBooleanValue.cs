using System;

namespace System.Threading
{
	internal struct AtomicBooleanValue
	{
		public bool CompareAndExchange(bool expected, bool newVal)
		{
			int num = (newVal ? 1 : 0);
			int num2 = (expected ? 1 : 0);
			return Interlocked.CompareExchange(ref this.flag, num, num2) == num2;
		}

		public static AtomicBooleanValue FromValue(bool value)
		{
			return new AtomicBooleanValue
			{
				Value = value
			};
		}

		public bool TrySet()
		{
			return !this.Exchange(true);
		}

		public bool TryRelaxedSet()
		{
			return this.flag == 0 && !this.Exchange(true);
		}

		public bool Exchange(bool newVal)
		{
			int num = (newVal ? 1 : 0);
			return Interlocked.Exchange(ref this.flag, num) == 1;
		}

		public bool Value
		{
			get
			{
				return this.flag == 1;
			}
			set
			{
				this.Exchange(value);
			}
		}

		public bool Equals(AtomicBooleanValue rhs)
		{
			return this.flag == rhs.flag;
		}

		public override bool Equals(object rhs)
		{
			return rhs is AtomicBooleanValue && this.Equals((AtomicBooleanValue)rhs);
		}

		public override int GetHashCode()
		{
			return this.flag.GetHashCode();
		}

		public static explicit operator bool(AtomicBooleanValue rhs)
		{
			return rhs.Value;
		}

		public static implicit operator AtomicBooleanValue(bool rhs)
		{
			return AtomicBooleanValue.FromValue(rhs);
		}

		private int flag;

		private const int UnSet = 0;

		private const int Set = 1;
	}
}
