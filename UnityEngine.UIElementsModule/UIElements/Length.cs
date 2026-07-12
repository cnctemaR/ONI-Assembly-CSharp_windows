using System;
using System.Globalization;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct Length : IEquatable<Length>
	{
		public static Length Percent(float value)
		{
			return new Length(value, LengthUnit.Percent);
		}

		public static Length Auto()
		{
			return new Length(0f, Length.Unit.Auto);
		}

		public static Length None()
		{
			return new Length(0f, Length.Unit.None);
		}

		public float value
		{
			get
			{
				return this.m_Value;
			}
			set
			{
				this.m_Value = Mathf.Clamp(value, -8388608f, 8388608f);
			}
		}

		public LengthUnit unit
		{
			get
			{
				return (LengthUnit)this.m_Unit;
			}
			set
			{
				this.m_Unit = (Length.Unit)value;
			}
		}

		public bool IsAuto()
		{
			return this.m_Unit == Length.Unit.Auto;
		}

		public bool IsNone()
		{
			return this.m_Unit == Length.Unit.None;
		}

		public Length(float value)
		{
			this = new Length(value, Length.Unit.Pixel);
		}

		public Length(float value, LengthUnit unit)
		{
			this = new Length(value, (Length.Unit)unit);
		}

		private Length(float value, Length.Unit unit)
		{
			this = default(Length);
			this.value = value;
			this.m_Unit = unit;
		}

		public static implicit operator Length(float value)
		{
			return new Length(value, LengthUnit.Pixel);
		}

		public static bool operator ==(Length lhs, Length rhs)
		{
			return lhs.m_Value == rhs.m_Value && lhs.m_Unit == rhs.m_Unit;
		}

		public static bool operator !=(Length lhs, Length rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(Length other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is Length)
			{
				Length length = (Length)obj;
				flag = this.Equals(length);
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
			case Length.Unit.Pixel:
			{
				bool flag = !Mathf.Approximately(0f, this.value);
				if (flag)
				{
					text2 = "px";
				}
				break;
			}
			case Length.Unit.Percent:
				text2 = "%";
				break;
			case Length.Unit.Auto:
				text = "auto";
				break;
			case Length.Unit.None:
				text = "none";
				break;
			}
			return text + text2;
		}

		internal const float k_MaxValue = 8388608f;

		[SerializeField]
		private float m_Value;

		[SerializeField]
		private Length.Unit m_Unit;

		private enum Unit
		{
			Pixel,
			Percent,
			Auto,
			None
		}
	}
}
