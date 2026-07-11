using System;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	public struct EventRegistrationToken
	{
		internal EventRegistrationToken(ulong value)
		{
			this.m_value = value;
		}

		internal ulong Value
		{
			get
			{
				return this.m_value;
			}
		}

		public static bool operator ==(EventRegistrationToken left, EventRegistrationToken right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(EventRegistrationToken left, EventRegistrationToken right)
		{
			return !left.Equals(right);
		}

		public override bool Equals(object obj)
		{
			return obj is EventRegistrationToken && ((EventRegistrationToken)obj).Value == this.Value;
		}

		public override int GetHashCode()
		{
			return this.m_value.GetHashCode();
		}

		internal ulong m_value;
	}
}
