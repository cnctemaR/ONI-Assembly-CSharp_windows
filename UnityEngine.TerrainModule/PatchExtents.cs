using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Structure containing minimum and maximum terrain patch height values.</para>
	/// </summary>
	[UsedByNativeCode]
	public struct PatchExtents
	{
		/// <summary>
		///   <para>Minimum height of a terrain patch.</para>
		/// </summary>
		public float min
		{
			get
			{
				return this.m_min;
			}
			set
			{
				this.m_min = value;
			}
		}

		/// <summary>
		///   <para>Maximum height of a terrain patch.</para>
		/// </summary>
		public float max
		{
			get
			{
				return this.m_max;
			}
			set
			{
				this.m_max = value;
			}
		}

		internal float m_min;

		internal float m_max;
	}
}
