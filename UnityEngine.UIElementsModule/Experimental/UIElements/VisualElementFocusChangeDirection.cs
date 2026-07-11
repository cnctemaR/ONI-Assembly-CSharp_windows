using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Define focus change directions for the VisualElementFocusRing.</para>
	/// </summary>
	public class VisualElementFocusChangeDirection : FocusChangeDirection
	{
		protected VisualElementFocusChangeDirection(int value)
			: base(value)
		{
		}

		/// <summary>
		///   <para>The focus is moving to the left.</para>
		/// </summary>
		public static FocusChangeDirection left
		{
			get
			{
				return VisualElementFocusChangeDirection.s_Left;
			}
		}

		/// <summary>
		///   <para>The focus is moving to the right.</para>
		/// </summary>
		public static FocusChangeDirection right
		{
			get
			{
				return VisualElementFocusChangeDirection.s_Right;
			}
		}

		/// <summary>
		///   <para>Last value for the direction defined by this class.</para>
		/// </summary>
		protected new static VisualElementFocusChangeDirection lastValue
		{
			get
			{
				return VisualElementFocusChangeDirection.s_Right;
			}
		}

		private static readonly VisualElementFocusChangeDirection s_Left = new VisualElementFocusChangeDirection(FocusChangeDirection.lastValue + 1);

		private static readonly VisualElementFocusChangeDirection s_Right = new VisualElementFocusChangeDirection(FocusChangeDirection.lastValue + 2);
	}
}
