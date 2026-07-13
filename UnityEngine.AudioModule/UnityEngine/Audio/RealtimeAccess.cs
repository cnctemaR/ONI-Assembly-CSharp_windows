using System;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Audio
{
	internal readonly struct RealtimeAccess
	{
		internal bool IsCreated
		{
			get
			{
				return this.m_Realtime != null;
			}
		}

		[NativeDisableUnsafePtrRestriction]
		private unsafe readonly void* m_Realtime;

		private readonly int m_Frame;

		private readonly int m_DTM;
	}
}
