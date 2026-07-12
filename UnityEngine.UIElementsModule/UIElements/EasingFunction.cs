using System;

namespace UnityEngine.UIElements
{
	public struct EasingFunction : IEquatable<EasingFunction>
	{
		public EasingMode mode
		{
			get
			{
				return this.m_Mode;
			}
			set
			{
				this.m_Mode = value;
			}
		}

		public EasingFunction(EasingMode mode)
		{
			this.m_Mode = mode;
		}

		public static implicit operator EasingFunction(EasingMode easingMode)
		{
			return new EasingFunction(easingMode);
		}

		public static bool operator ==(EasingFunction lhs, EasingFunction rhs)
		{
			return lhs.m_Mode == rhs.m_Mode;
		}

		public static bool operator !=(EasingFunction lhs, EasingFunction rhs)
		{
			return !(lhs == rhs);
		}

		public bool Equals(EasingFunction other)
		{
			return other == this;
		}

		public override bool Equals(object obj)
		{
			bool flag;
			if (obj is EasingFunction)
			{
				EasingFunction easingFunction = (EasingFunction)obj;
				flag = this.Equals(easingFunction);
			}
			else
			{
				flag = false;
			}
			return flag;
		}

		public override string ToString()
		{
			return this.m_Mode.ToString();
		}

		public override int GetHashCode()
		{
			return (int)this.m_Mode;
		}

		private EasingMode m_Mode;
	}
}
