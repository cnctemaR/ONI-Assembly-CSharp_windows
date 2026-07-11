using System;

namespace UnityEngine.Experimental.UIElements
{
	/// <summary>
	///   <para>Base class for defining in which direction the focus moves in a focus ring.</para>
	/// </summary>
	public class FocusChangeDirection
	{
		protected FocusChangeDirection(int value)
		{
			this.m_Value = value;
		}

		/// <summary>
		///   <para>Focus came from an unspecified direction, for example after a mouse down.</para>
		/// </summary>
		public static FocusChangeDirection unspecified
		{
			get
			{
				return FocusChangeDirection.s_Unspecified;
			}
		}

		/// <summary>
		///   <para>The null direction. This is usually used when the focus stays on the same element.</para>
		/// </summary>
		public static FocusChangeDirection none
		{
			get
			{
				return FocusChangeDirection.s_None;
			}
		}

		/// <summary>
		///   <para>Last value for the direction defined by this class.</para>
		/// </summary>
		protected static FocusChangeDirection lastValue
		{
			get
			{
				return FocusChangeDirection.s_None;
			}
		}

		public static implicit operator int(FocusChangeDirection fcd)
		{
			return fcd.m_Value;
		}

		private static readonly FocusChangeDirection s_Unspecified = new FocusChangeDirection(-1);

		private static readonly FocusChangeDirection s_None = new FocusChangeDirection(0);

		private int m_Value;
	}
}
