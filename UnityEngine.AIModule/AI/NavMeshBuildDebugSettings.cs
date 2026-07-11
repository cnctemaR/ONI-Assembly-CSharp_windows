using System;
using UnityEngine.Bindings;

namespace UnityEngine.AI
{
	[NativeHeader("Modules/AI/Public/NavMeshBuildDebugSettings.h")]
	public struct NavMeshBuildDebugSettings
	{
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
