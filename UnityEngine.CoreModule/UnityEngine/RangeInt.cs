using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>Describes an integer range.</para>
	/// </summary>
	public struct RangeInt
	{
		/// <summary>
		///   <para>Constructs a new RangeInt with given start, length values.</para>
		/// </summary>
		/// <param name="start">The starting index of the range.</param>
		/// <param name="length">The length of the range.</param>
		public RangeInt(int start, int length)
		{
			this.start = start;
			this.length = length;
		}

		/// <summary>
		///   <para>The end index of the range (not inclusive).</para>
		/// </summary>
		public int end
		{
			get
			{
				return this.start + this.length;
			}
		}

		/// <summary>
		///   <para>The starting index of the range, where 0 is the first position, 1 is the second, 2 is the third, and so on.</para>
		/// </summary>
		public int start;

		/// <summary>
		///   <para>The length of the range.</para>
		/// </summary>
		public int length;
	}
}
