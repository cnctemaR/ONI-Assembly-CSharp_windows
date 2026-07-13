using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Jobs;
using UnityEngine.Bindings;

namespace UnityEngine.Experimental.AI
{
	[StaticAccessor("NavMeshWorldBindings", StaticAccessorType.DoubleColon)]
	[Obsolete("The experimental NavMeshWorld struct has been deprecated without replacement.")]
	public struct NavMeshWorld
	{
		public bool IsValid()
		{
			return this.world != IntPtr.Zero;
		}

		public static NavMeshWorld GetDefaultWorld()
		{
			NavMeshWorld navMeshWorld;
			NavMeshWorld.GetDefaultWorld_Injected(out navMeshWorld);
			return navMeshWorld;
		}

		private static void AddDependencyInternal(IntPtr navmesh, JobHandle handle)
		{
			NavMeshWorld.AddDependencyInternal_Injected(navmesh, ref handle);
		}

		public void AddDependency(JobHandle job)
		{
			NavMeshWorld.AddDependencyInternal(this.world, job);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDefaultWorld_Injected(out NavMeshWorld ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void AddDependencyInternal_Injected(IntPtr navmesh, [In] ref JobHandle handle);

		internal IntPtr world;
	}
}
