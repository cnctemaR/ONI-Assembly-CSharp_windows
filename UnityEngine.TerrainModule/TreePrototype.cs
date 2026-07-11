using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Simple class that contains a pointer to a tree prototype.</para>
	/// </summary>
	[UsedByNativeCode]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class TreePrototype
	{
		/// <summary>
		///   <para>Retrieves the actual GameObject used by the tree.</para>
		/// </summary>
		public GameObject prefab
		{
			get
			{
				return this.m_Prefab;
			}
			set
			{
				this.m_Prefab = value;
			}
		}

		/// <summary>
		///   <para>Bend factor of the tree prototype.</para>
		/// </summary>
		public float bendFactor
		{
			get
			{
				return this.m_BendFactor;
			}
			set
			{
				this.m_BendFactor = value;
			}
		}

		internal GameObject m_Prefab;

		internal float m_BendFactor;
	}
}
