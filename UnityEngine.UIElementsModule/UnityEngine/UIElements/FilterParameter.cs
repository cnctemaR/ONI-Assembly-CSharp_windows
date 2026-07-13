using System;
using System.Globalization;

namespace UnityEngine.UIElements
{
	[Serializable]
	public struct FilterParameter : IEquatable<FilterParameter>
	{
		public FilterParameterType type
		{
			get
			{
				return this.m_Type;
			}
			set
			{
				this.m_Type = value;
			}
		}

		public float floatValue
		{
			get
			{
				return this.m_FloatValue;
			}
			set
			{
				this.m_FloatValue = value;
			}
		}

		public Color colorValue
		{
			get
			{
				return this.m_ColorValue;
			}
			set
			{
				this.m_ColorValue = value;
			}
		}

		public FilterParameter(float value)
		{
			this.m_Type = FilterParameterType.Float;
			this.m_FloatValue = value;
			this.m_ColorValue = Color.clear;
		}

		public FilterParameter(Color value)
		{
			this.m_Type = FilterParameterType.Color;
			this.m_ColorValue = value;
			this.m_FloatValue = 0f;
		}

		public static bool operator ==(FilterParameter a, FilterParameter b)
		{
			bool flag = a.type != b.type;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = a.type == FilterParameterType.Float;
				if (flag3)
				{
					flag2 = a.floatValue == b.floatValue;
				}
				else
				{
					flag2 = a.colorValue == b.colorValue;
				}
			}
			return flag2;
		}

		public static bool operator !=(FilterParameter a, FilterParameter b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is FilterParameter)
			{
				FilterParameter filterParameter = (FilterParameter)obj;
				flag = this == filterParameter;
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public bool Equals(FilterParameter other)
		{
			return this == other;
		}

		public override int GetHashCode()
		{
			return (this.type == FilterParameterType.Float) ? this.floatValue.GetHashCode() : this.colorValue.GetHashCode();
		}

		public override string ToString()
		{
			return (this.type == FilterParameterType.Float) ? this.floatValue.ToString(CultureInfo.InvariantCulture) : this.colorValue.ToString();
		}

		[SerializeField]
		private FilterParameterType m_Type;

		[SerializeField]
		private float m_FloatValue;

		[SerializeField]
		private Color m_ColorValue;
	}
}
