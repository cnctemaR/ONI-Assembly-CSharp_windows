using System;
using System.Globalization;
using Unity.Properties;

namespace UnityEngine.UIElements
{
	public readonly struct Ratio : IEquatable<Ratio>
	{
		public Ratio(float value)
		{
			this.m_Value = value;
		}

		public static Ratio Auto()
		{
			Ratio ratio = new Ratio(float.NaN);
			return ratio;
		}

		public float value
		{
			get
			{
				return this.m_Value;
			}
		}

		public bool IsAuto()
		{
			return float.IsNaN(this.value);
		}

		public static implicit operator Ratio(float value)
		{
			return new Ratio(value);
		}

		public static implicit operator float(Ratio value)
		{
			return value.value;
		}

		public static bool operator ==(Ratio lhs, Ratio rhs)
		{
			bool flag = lhs.IsAuto() && rhs.IsAuto();
			return flag || lhs.m_Value == rhs.m_Value;
		}

		public static bool operator !=(Ratio lhs, Ratio rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(Ratio other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is Ratio)
			{
				Ratio ratio = (Ratio)obj;
				flag = this.Equals(ratio);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return this.m_Value.GetHashCode() * 793;
		}

		public override string ToString()
		{
			return this.IsAuto() ? StyleValueKeyword.Auto.ToUssString() : this.m_Value.ToString(CultureInfo.InvariantCulture.NumberFormat);
		}

		private readonly float m_Value;

		internal class PropertyBag : ContainerPropertyBag<Ratio>
		{
			public PropertyBag()
			{
				base.AddProperty<float>(new Ratio.PropertyBag.ValueProperty());
				base.AddProperty<bool>(new Ratio.PropertyBag.AutoProperty());
			}

			private class ValueProperty : Property<Ratio, float>
			{
				public override string Name { get; } = "value";

				public override bool IsReadOnly { get; } = false;

				public override float GetValue(ref Ratio container)
				{
					return container.value;
				}

				public override void SetValue(ref Ratio container, float value)
				{
					throw new InvalidOperationException();
				}
			}

			private class AutoProperty : Property<Ratio, bool>
			{
				public override string Name { get; } = "IsAuto";

				public override bool IsReadOnly { get; } = true;

				public override bool GetValue(ref Ratio container)
				{
					return container.IsAuto();
				}

				public override void SetValue(ref Ratio container, bool value)
				{
					throw new InvalidOperationException();
				}
			}
		}
	}
}
