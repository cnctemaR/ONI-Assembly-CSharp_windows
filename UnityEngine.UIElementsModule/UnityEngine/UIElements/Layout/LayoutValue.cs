using System;

namespace UnityEngine.UIElements.Layout
{
	internal struct LayoutValue
	{
		public LayoutUnit Unit
		{
			get
			{
				return this.unit;
			}
		}

		public float Value
		{
			get
			{
				return this.value;
			}
		}

		public static LayoutValue Point(float value)
		{
			return new LayoutValue
			{
				value = value,
				unit = (float.IsNaN(value) ? LayoutUnit.Undefined : LayoutUnit.Point)
			};
		}

		public bool Equals(LayoutValue other)
		{
			return this.Unit == other.Unit && (this.Value.Equals(other.Value) || this.Unit == LayoutUnit.Undefined);
		}

		public override bool Equals(object obj)
		{
			bool flag = obj == null;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3;
				if (obj is LayoutValue)
				{
					LayoutValue layoutValue = (LayoutValue)obj;
					flag3 = this.Equals(layoutValue);
				}
				else
				{
					flag3 = false;
				}
				flag2 = flag3;
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			return (this.Value.GetHashCode() * 397) ^ (int)this.Unit;
		}

		public static LayoutValue Undefined()
		{
			return new LayoutValue
			{
				value = float.NaN,
				unit = LayoutUnit.Undefined
			};
		}

		public static LayoutValue Auto()
		{
			return new LayoutValue
			{
				value = float.NaN,
				unit = LayoutUnit.Auto
			};
		}

		public static LayoutValue Percent(float value)
		{
			return new LayoutValue
			{
				value = value,
				unit = (float.IsNaN(value) ? LayoutUnit.Undefined : LayoutUnit.Percent)
			};
		}

		public static implicit operator LayoutValue(float value)
		{
			return LayoutValue.Point(value);
		}

		private float value;

		private LayoutUnit unit;
	}
}
