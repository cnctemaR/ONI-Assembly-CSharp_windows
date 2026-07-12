using System;
using System.Globalization;

namespace UnityEngine.UIElements
{
	public struct Angle : IEquatable<Angle>
	{
		public static Angle Degrees(float value)
		{
			return new Angle(value, AngleUnit.Degree);
		}

		public static Angle Gradians(float value)
		{
			return new Angle(value, AngleUnit.Gradian);
		}

		public static Angle Radians(float value)
		{
			return new Angle(value, AngleUnit.Radian);
		}

		public static Angle Turns(float value)
		{
			return new Angle(value, AngleUnit.Turn);
		}

		internal static Angle None()
		{
			return new Angle(0f, Angle.Unit.None);
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

		public AngleUnit unit
		{
			get
			{
				return (AngleUnit)this.m_Unit;
			}
			set
			{
				this.m_Unit = (Angle.Unit)value;
			}
		}

		internal bool IsNone()
		{
			return this.m_Unit == Angle.Unit.None;
		}

		public Angle(float value)
		{
			this = new Angle(value, Angle.Unit.Degree);
		}

		public Angle(float value, AngleUnit unit)
		{
			this = new Angle(value, (Angle.Unit)unit);
		}

		private Angle(float value, Angle.Unit unit)
		{
			this.m_Value = value;
			this.m_Unit = unit;
		}

		public float ToDegrees()
		{
			float num;
			switch (this.m_Unit)
			{
			case Angle.Unit.Degree:
				num = this.m_Value;
				break;
			case Angle.Unit.Gradian:
				num = this.m_Value * 360f / 400f;
				break;
			case Angle.Unit.Radian:
				num = this.m_Value * 180f / 3.1415927f;
				break;
			case Angle.Unit.Turn:
				num = this.m_Value * 360f;
				break;
			case Angle.Unit.None:
				num = 0f;
				break;
			default:
				num = 0f;
				break;
			}
			return num;
		}

		public float ToGradians()
		{
			float num;
			switch (this.m_Unit)
			{
			case Angle.Unit.Degree:
				num = this.m_Value * 10f / 9f;
				break;
			case Angle.Unit.Gradian:
				num = this.m_Value;
				break;
			case Angle.Unit.Radian:
				num = this.m_Value * 200f / 3.1415927f;
				break;
			case Angle.Unit.Turn:
				num = this.m_Value * 400f;
				break;
			case Angle.Unit.None:
				num = 0f;
				break;
			default:
				num = 0f;
				break;
			}
			return num;
		}

		public float ToRadians()
		{
			float num;
			switch (this.m_Unit)
			{
			case Angle.Unit.Degree:
				num = this.m_Value * 3.1415927f / 180f;
				break;
			case Angle.Unit.Gradian:
				num = this.m_Value * 3.1415927f / 200f;
				break;
			case Angle.Unit.Radian:
				num = this.m_Value;
				break;
			case Angle.Unit.Turn:
				num = this.m_Value * 3.1415927f * 2f;
				break;
			case Angle.Unit.None:
				num = 0f;
				break;
			default:
				num = 0f;
				break;
			}
			return num;
		}

		public float ToTurns()
		{
			float num;
			switch (this.m_Unit)
			{
			case Angle.Unit.Degree:
				num = this.m_Value / 360f;
				break;
			case Angle.Unit.Gradian:
				num = this.m_Value / 400f;
				break;
			case Angle.Unit.Radian:
				num = this.m_Value / 6.2831855f;
				break;
			case Angle.Unit.Turn:
				num = this.m_Value;
				break;
			case Angle.Unit.None:
				num = 0f;
				break;
			default:
				num = 0f;
				break;
			}
			return num;
		}

		internal void ConvertTo(AngleUnit newUnit)
		{
			if (!true)
			{
			}
			float num;
			switch (newUnit)
			{
			case AngleUnit.Degree:
				num = this.ToDegrees();
				break;
			case AngleUnit.Gradian:
				num = this.ToGradians();
				break;
			case AngleUnit.Radian:
				num = this.ToRadians();
				break;
			case AngleUnit.Turn:
				num = this.ToTurns();
				break;
			default:
				throw new NotImplementedException();
			}
			if (!true)
			{
			}
			this.m_Value = num;
			this.m_Unit = (Angle.Unit)newUnit;
		}

		public static implicit operator Angle(float value)
		{
			return new Angle(value, AngleUnit.Degree);
		}

		public static bool operator ==(Angle lhs, Angle rhs)
		{
			return lhs.m_Value == rhs.m_Value && lhs.m_Unit == rhs.m_Unit;
		}

		public static bool operator !=(Angle lhs, Angle rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(Angle other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is Angle)
			{
				Angle angle = (Angle)obj;
				flag = this.Equals(angle);
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
			switch (this.m_Unit)
			{
			case Angle.Unit.Degree:
			{
				bool flag = !Mathf.Approximately(0f, this.value);
				if (flag)
				{
					text2 = "deg";
				}
				break;
			}
			case Angle.Unit.Gradian:
				text2 = "grad";
				break;
			case Angle.Unit.Radian:
				text2 = "rad";
				break;
			case Angle.Unit.Turn:
				text2 = "turn";
				break;
			case Angle.Unit.None:
				text = "";
				break;
			}
			return text + text2;
		}

		private float m_Value;

		private Angle.Unit m_Unit;

		private enum Unit
		{
			Degree,
			Gradian,
			Radian,
			Turn,
			None
		}
	}
}
