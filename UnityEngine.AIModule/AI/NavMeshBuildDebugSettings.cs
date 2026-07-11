using System;
using UnityEngine.Bindings;

namespace UnityEngine.AI
{
	/// <summary>
	///   <para>Specify which of the temporary data generated while building the NavMesh should be retained in memory after the process has completed.</para>
	/// </summary>
	[NativeHeader("Modules/AI/Public/NavMeshBuildDebugSettings.h")]
	public struct NavMeshBuildDebugSettings
	{
		/// <summary>
		///   <para>Specify which types of debug data to collect when building the NavMesh.</para>
		/// </summary>
		public NavMeshBuildDebugFlags flags
		{
			get
			{
				return (NavMeshBuildDebugFlags)this.m_Flags;
			}
			set
			{
				this.m_Flags = (byte)value;
			}
		}

		private byte m_Flags;
	}
}
