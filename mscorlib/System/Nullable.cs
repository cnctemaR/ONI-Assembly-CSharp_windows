using System;

namespace System
{
	[Serializable]
	public struct Nullable<T> where T : struct
	{
		public Nullable(T value)
		{
			this.has_value = true;
			this.value = value;
		}

		public bool HasValue
		{
			get
			{
				return this.has_value;
			}
		}

		public T Value
		{
			get
			{
				if (!this.has_value)
				{
					throw new InvalidOperationException("Nullable object must have a value.");
				}
				return this.value;
			}
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return !this.has_value;
			}
			return other is T? && this.Equals((T?)other);
		}

		private bool Equals(T? other)
		{
			return other.has_value == this.has_value && (!this.has_value || other.value.Equals(this.value));
		}

		public override int GetHashCode()
		{
			if (!this.has_value)
			{
				return 0;
			}
			return this.value.GetHashCode();
		}

		public T GetValueOrDefault()
		{
			return (!this.has_value) ? default(T) : this.value;
		}

		public T GetValueOrDefault(T defaultValue)
		{
			return (!this.has_value) ? defaultValue : this.value;
		}

		public override string ToString()
		{
			if (this.has_value)
			{
				return this.value.ToString();
			}
			return string.Empty;
		}

		private static object Box(T? o)
		{
			if (!o.has_value)
			{
				return null;
			}
			return o.value;
		}

		private static T? Unbox(object o)
		{
			if (o == null)
			{
				return null;
			}
			return new T?((T)((object)o));
		}

		public static implicit operator T?(T value)
		{
			return new T?(value);
		}

		public static explicit operator T(T? value)
		{
			return value.Value;
		}

		internal T value;

		internal bool has_value;
	}
}
