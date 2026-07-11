using System;

namespace UnityEngine.UIElements
{
	public class FocusChangeDirection
	{
		public static FocusChangeDirection unspecified { get; } = new FocusChangeDirection(-1);

		public static FocusChangeDirection none { get; } = new FocusChangeDirection(0);

		protected static FocusChangeDirection lastValue { get; } = FocusChangeDirection.none;

		protected FocusChangeDirection(int value)
		{
			this.m_Value = value;
		}

		public static implicit operator int(FocusChangeDirection fcd)
		{
			return (fcd != null) ? fcd.m_Value : 0;
		}

		private readonly int m_Value;
	}
}
