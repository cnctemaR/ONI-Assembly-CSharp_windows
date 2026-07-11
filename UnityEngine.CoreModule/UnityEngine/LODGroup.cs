using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[StaticAccessor("GetLODGroupManager()", StaticAccessorType.Dot)]
	[NativeHeader("Runtime/Graphics/LOD/LODUtility.h")]
	[NativeHeader("Runtime/Graphics/LOD/LODGroupManager.h")]
	[NativeHeader("Runtime/Graphics/LOD/LODGroup.h")]
	public class LODGroup : Component
	{
		public Vector3 localReferencePoint
		{
			get
			{
				Vector3 vector;
				this.get_localReferencePoint_Injected(out vector);
				return vector;
			}
			set
			{
				this.set_localReferencePoint_Injected(ref value);
			}
		}

		public extern float size
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int lodCount
		{
			[NativeMethod("GetLODCount")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern LODFadeMode fadeMode
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool animateCrossFading
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern bool enabled
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[FreeFunction("UpdateLODGroupBoundingBox", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void RecalculateBounds();

		[FreeFunction("GetLODs_Binding", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern LOD[] GetLODs();

		[Obsolete("Use SetLODs instead.")]
		public void SetLODS(LOD[] lods)
		{
			this.SetLODs(lods);
		}

		[FreeFunction("SetLODs_Binding", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void SetLODs(LOD[] lods);

		[FreeFunction("ForceLODLevel", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void ForceLOD(int index);

		[StaticAccessor("GetLODGroupManager()")]
		public static extern float crossFadeAnimationDuration
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void get_localReferencePoint_Injected(out Vector3 ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void set_localReferencePoint_Injected(ref Vector3 value);
	}
}
