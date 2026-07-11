using System;
using System.Collections.Generic;

namespace UnityEngine.Assertions.Comparers
{
	/// <summary>
	///   <para>A float comparer used by Assertions.Assert performing approximate comparison.</para>
	/// </summary>
	public class FloatComparer : IEqualityComparer<float>
	{
		/// <summary>
		///   <para>Creates an instance of the comparer.</para>
		/// </summary>
		/// <param name="relative">Should a relative check be used when comparing values? By default, an absolute check will be used.</param>
		/// <param name="error">Allowed comparison error. By default, the FloatComparer.kEpsilon is used.</param>
		public FloatComparer()
			: this(1E-05f, false)
		{
		}

		/// <summary>
		///   <para>Creates an instance of the comparer.</para>
		/// </summary>
		/// <param name="relative">Should a relative check be used when comparing values? By default, an absolute check will be used.</param>
		/// <param name="error">Allowed comparison error. By default, the FloatComparer.kEpsilon is used.</param>
		public FloatComparer(bool relative)
			: this(1E-05f, relative)
		{
		}

		/// <summary>
		///   <para>Creates an instance of the comparer.</para>
		/// </summary>
		/// <param name="relative">Should a relative check be used when comparing values? By default, an absolute check will be used.</param>
		/// <param name="error">Allowed comparison error. By default, the FloatComparer.kEpsilon is used.</param>
		public FloatComparer(float error)
			: this(error, false)
		{
		}

		/// <summary>
		///   <para>Creates an instance of the comparer.</para>
		/// </summary>
		/// <param name="relative">Should a relative check be used when comparing values? By default, an absolute check will be used.</param>
		/// <param name="error">Allowed comparison error. By default, the FloatComparer.kEpsilon is used.</param>
		public FloatComparer(float error, bool relative)
		{
			this.m_Error = error;
			this.m_Relative = relative;
		}

		public bool Equals(float a, float b)
		{
			return (!this.m_Relative) ? FloatComparer.AreEqual(a, b, this.m_Error) : FloatComparer.AreEqualRelative(a, b, this.m_Error);
		}

		public int GetHashCode(float obj)
		{
			return base.GetHashCode();
		}

		/// <summary>
		///   <para>Performs equality check with absolute error check.</para>
		/// </summary>
		/// <param name="expected">Expected value.</param>
		/// <param name="actual">Actual value.</param>
		/// <param name="error">Comparison error.</param>
		/// <returns>
		///   <para>Result of the comparison.</para>
		/// </returns>
		public static bool AreEqual(float expected, float actual, float error)
		{
			return Math.Abs(actual - expected) <= error;
		}

		/// <summary>
		///   <para>Performs equality check with relative error check.</para>
		/// </summary>
		/// <param name="expected">Expected value.</param>
		/// <param name="actual">Actual value.</param>
		/// <param name="error">Comparison error.</param>
		/// <returns>
		///   <para>Result of the comparison.</para>
		/// </returns>
		public static bool AreEqualRelative(float expected, float actual, float error)
		{
			bool flag;
			if (expected == actual)
			{
				flag = true;
			}
			else
			{
				float num = Math.Abs(expected);
				float num2 = Math.Abs(actual);
				float num3 = Math.Abs((actual - expected) / ((num <= num2) ? num2 : num));
				flag = num3 <= error;
			}
			return flag;
		}

		private readonly float m_Error;

		private readonly bool m_Relative;

		/// <summary>
		///   <para>Default instance of a comparer class with deafult error epsilon and absolute error check.</para>
		/// </summary>
		public static readonly FloatComparer s_ComparerWithDefaultTolerance = new FloatComparer(1E-05f);

		/// <summary>
		///   <para>Default epsilon used by the comparer.</para>
		/// </summary>
		public const float kEpsilon = 1E-05f;
	}
}
