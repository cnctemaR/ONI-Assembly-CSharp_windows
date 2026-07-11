using System;

namespace UnityEngine.Experimental.UIElements.StyleSheets
{
	public struct StyleValue<T>
	{
		public StyleValue(T value)
		{
			this.value = value;
			this.specificity = 0;
		}

		internal StyleValue(T value, int specifity)
		{
			this.value = value;
			this.specificity = specifity;
		}

		public static StyleValue<T> nil
		{
			get
			{
				return StyleValue<T>.defaultStyle;
			}
		}

		public T GetSpecifiedValueOrDefault(T defaultValue)
		{
			if (this.specificity > 0)
			{
				defaultValue = this.value;
			}
			return defaultValue;
		}

		public static implicit operator T(StyleValue<T> sp)
		{
			return sp.value;
		}

		internal bool Apply(StyleValue<T> other, StylePropertyApplyMode mode)
		{
			return this.Apply(other.value, other.specificity, mode);
		}

		internal bool Apply(T otherValue, int otherSpecificity, StylePropertyApplyMode mode)
		{
			bool flag;
			switch (mode)
			{
			case StylePropertyApplyMode.Copy:
				this.value = otherValue;
				this.specificity = otherSpecificity;
				flag = true;
				break;
			case StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity:
				if (otherSpecificity >= this.specificity)
				{
					this.value = otherValue;
					this.specificity = otherSpecificity;
					flag = true;
				}
				else
				{
					flag = false;
				}
				break;
			case StylePropertyApplyMode.CopyIfNotInline:
				if (this.specificity < 2147483647)
				{
					this.value = otherValue;
					this.specificity = otherSpecificity;
					flag = true;
				}
				else
				{
					flag = false;
				}
				break;
			default:
				Debug.Assert(false, "Invalid mode " + mode);
				flag = false;
				break;
			}
			return flag;
		}

		public static implicit operator StyleValue<T>(T value)
		{
			return StyleValue<T>.Create(value);
		}

		public static StyleValue<T> Create(T value)
		{
			return new StyleValue<T>(value, int.MaxValue);
		}

		public override string ToString()
		{
			return string.Format("[StyleProperty<{2}>: specifity={0}, value={1}]", this.specificity, this.value, typeof(T).Name);
		}

		internal int specificity;

		public T value;

		private static readonly StyleValue<T> defaultStyle = default(StyleValue<T>);
	}
}
