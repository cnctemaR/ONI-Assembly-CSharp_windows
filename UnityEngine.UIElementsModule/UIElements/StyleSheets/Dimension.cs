using System;
using System.Globalization;

namespace UnityEngine.UIElements.StyleSheets
{
	[Serializable]
	internal struct Dimension : IEquatable<Dimension>
	{
		public Dimension(float value, Dimension.Unit unit)
		{
			this.unit = unit;
			this.value = value;
		}

		public Length ToLength()
		{
			LengthUnit lengthUnit = ((this.unit == Dimension.Unit.Percent) ? LengthUnit.Percent : LengthUnit.Pixel);
			return new Length(this.value, lengthUnit);
		}

		public static bool operator ==(Dimension lhs, Dimension rhs)
		{
			return lhs.value == rhs.value && lhs.unit == rhs.unit;
		}

		public static bool operator !=(Dimension lhs, Dimension rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(Dimension other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag = !(obj is Dimension);
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				Dimension dimension = (Dimension)obj;
				flag2 = dimension == this;
			}
			return flag2;
		}

		public override int GetHashCode()
		{
			int num = -799583767;
			num = num * -1521134295 + this.unit.GetHashCode();
			return num * -1521134295 + this.value.GetHashCode();
		}

		public override string ToString()
		{
			string text = string.Empty;
			Dimension.Unit unit = this.unit;
			if (unit != Dimension.Unit.Pixel)
			{
				if (unit == Dimension.Unit.Percent)
				{
					text = "%";
				}
			}
			else
			{
				text = "px";
			}
			return this.value.ToString(CultureInfo.InvariantCulture.NumberFormat) + text;
		}

		public Dimension.Unit unit;

		public float value;

		public enum Unit
		{
			Unitless,
			Pixel,
			Percent
		}
	}
}
