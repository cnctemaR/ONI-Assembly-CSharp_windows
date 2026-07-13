using System;
using System.Globalization;
using Unity.Properties;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct TimeValue : IEquatable<TimeValue>
	{
		public static TimeValue Seconds(float value)
		{
			return new TimeValue(value, TimeUnit.Second);
		}

		public static TimeValue Milliseconds(float value)
		{
			return new TimeValue(value, TimeUnit.Millisecond);
		}

		public float value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = value;
			}
		}

		public TimeUnit unit
		{
			get
			{
				return this.m_Unit;
			}
			set
			{
				this.m_Unit = value;
			}
		}

		public TimeValue(float value)
		{
			this = new TimeValue(value, TimeUnit.Second);
		}

		public TimeValue(float value, TimeUnit unit)
		{
			this.m_Value = value;
			this.m_Unit = unit;
		}

		public static implicit operator TimeValue(float value)
		{
			return new TimeValue(value, TimeUnit.Second);
		}

		public static bool operator ==(TimeValue lhs, TimeValue rhs)
		{
			return lhs.m_Value == rhs.m_Value && lhs.m_Unit == rhs.m_Unit;
		}

		public static bool operator !=(TimeValue lhs, TimeValue rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(TimeValue other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is TimeValue)
			{
				TimeValue timeValue = (TimeValue)obj;
				flag = this.Equals(timeValue);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return (this.m_Value.GetHashCode() * 397) ^ (int)this.m_Unit;
		}

		public override string ToString()
		{
			string text = this.value.ToString(CultureInfo.InvariantCulture.NumberFormat);
			string text2 = string.Empty;
			TimeUnit unit = this.unit;
			TimeUnit timeUnit = unit;
			if (timeUnit != TimeUnit.Second)
			{
				if (timeUnit == TimeUnit.Millisecond)
				{
					text2 = "ms";
				}
			}
			else
			{
				text2 = "s";
			}
			return text + text2;
		}

		internal unsafe static bool TryParseString(string str, out TimeValue timeValue)
		{
			timeValue = default(TimeValue);
			bool flag = string.IsNullOrEmpty(str);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				ReadOnlySpan<char> readOnlySpan = str.AsSpan().Trim();
				int num = 0;
				int num2 = -1;
				int i = 0;
				while (i < readOnlySpan.Length)
				{
					char c = (char)(*readOnlySpan[i]);
					bool flag3 = char.IsNumber(c) || c == '.';
					if (flag3)
					{
						num++;
						i++;
					}
					else
					{
						bool flag4 = char.IsLetter(c);
						if (flag4)
						{
							num2 = i;
							break;
						}
						return false;
					}
				}
				ReadOnlySpan<char> readOnlySpan2 = readOnlySpan.Slice(0, num);
				ReadOnlySpan<char> readOnlySpan3 = default(ReadOnlySpan<char>);
				bool flag5 = num2 > 0;
				if (flag5)
				{
					readOnlySpan3 = readOnlySpan.Slice(num2, readOnlySpan.Length - num2);
				}
				else
				{
					readOnlySpan3 = "s";
				}
				float num3;
				bool flag6 = StylePropertyUtil.TryParseFloat(readOnlySpan2, out num3);
				if (flag6)
				{
					timeValue.value = num3;
				}
				bool flag7 = readOnlySpan3.Equals("ms", StringComparison.OrdinalIgnoreCase);
				if (flag7)
				{
					timeValue.unit = TimeUnit.Millisecond;
				}
				else
				{
					bool flag8 = readOnlySpan3.Equals("s", StringComparison.OrdinalIgnoreCase);
					if (flag8)
					{
						timeValue.unit = TimeUnit.Second;
					}
				}
				flag2 = true;
			}
			return flag2;
		}

		[SerializeField]
		private float m_Value;

		[SerializeField]
		private TimeUnit m_Unit;

		internal class PropertyBag : ContainerPropertyBag<TimeValue>
		{
			public PropertyBag()
			{
				base.AddProperty<float>(new TimeValue.PropertyBag.ValueProperty());
				base.AddProperty<TimeUnit>(new TimeValue.PropertyBag.UnitProperty());
			}

			private class ValueProperty : Property<TimeValue, float>
			{
				public override string Name { get; } = "value";

				public override bool IsReadOnly { get; } = false;

				public override float GetValue(ref TimeValue container)
				{
					return container.value;
				}

				public override void SetValue(ref TimeValue container, float value)
				{
					container.value = value;
				}
			}

			private class UnitProperty : Property<TimeValue, TimeUnit>
			{
				public override string Name { get; } = "unit";

				public override bool IsReadOnly { get; } = false;

				public override TimeUnit GetValue(ref TimeValue container)
				{
					return container.unit;
				}

				public override void SetValue(ref TimeValue container, TimeUnit value)
				{
					container.unit = value;
				}
			}
		}
	}
}
