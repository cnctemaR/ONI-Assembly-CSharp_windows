using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequireComponent(typeof(Transform))]
	[NativeHeader("Runtime/Graphics/Mesh/MeshFilter.h")]
	public sealed class MeshFilter : Component
	{
		[RequiredByNativeCode]
		private void DontStripMeshFilter()
		{
		}

		public extern Mesh sharedMesh
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern Mesh mesh
		{
			[NativeName("GetInstantiatedMeshFromScript")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetInstantiatedMesh")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}
	}
}
