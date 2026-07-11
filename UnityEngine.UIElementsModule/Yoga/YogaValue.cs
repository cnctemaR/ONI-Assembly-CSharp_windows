using System;

namespace UnityEngine.Yoga
{
	internal struct YogaValue
	{
		public YogaUnit Unit
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

		public static YogaValue Point(float value)
		{
			return new YogaValue
			{
				value = value,
				unit = ((!YogaConstants.IsUndefined(value)) ? YogaUnit.Point : YogaUnit.Undefined)
			};
		}

		public bool Equals(YogaValue other)
		{
			return this.Unit == other.Unit && (this.Value.Equals(other.Value) || this.Unit == YogaUnit.Undefined);
		}

		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && obj is YogaValue && this.Equals((YogaValue)obj);
		}

		public override int GetHashCode()
		{
			return (this.Value.GetHashCode() * 397) ^ (int)this.Unit;
		}

		public static YogaValue Undefined()
		{
			return new YogaValue
			{
				value = float.NaN,
				unit = YogaUnit.Undefined
			};
		}

		public static YogaValue Auto()
		{
			return new YogaValue
			{
				value = 0f,
				unit = YogaUnit.Auto
			};
		}

		public static YogaValue Percent(float value)
		{
			return new YogaValue
			{
				value = value,
				unit = ((!YogaConstants.IsUndefined(value)) ? YogaUnit.Percent : YogaUnit.Undefined)
			};
		}

		public static implicit operator YogaValue(float pointValue)
		{
			return YogaValue.Point(pointValue);
		}

		internal static YogaValue MarshalValue(YogaValue value)
		{
			return value;
		}

		private float value;

		private YogaUnit unit;
	}
}
