using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeHeader("Modules/XR/XRPrefix.h")]
	[NativeHeader("Modules/XR/Subsystems/Meshing/XRMeshSubsystem.h")]
	[NativeConditional("ENABLE_XR")]
	[UsedByNativeCode]
	public class XRMeshSubsystem : IntegratedSubsystem<XRMeshSubsystemDescriptor>
	{
		public bool TryGetMeshInfos(List<MeshInfo> meshInfosOut)
		{
			if (meshInfosOut == null)
			{
				throw new ArgumentNullException("meshInfosOut");
			}
			return this.GetMeshInfosAsList(meshInfosOut);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern bool GetMeshInfosAsList(List<MeshInfo> meshInfos);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern MeshInfo[] GetMeshInfosAsFixedArray();

		public void GenerateMeshAsync(TrackableId meshId, Mesh mesh, MeshCollider meshCollider, MeshVertexAttributes attributes, Action<MeshGenerationResult> onMeshGenerationComplete)
		{
			this.GenerateMeshAsync_Injected(ref meshId, mesh, meshCollider, attributes, onMeshGenerationComplete);
		}

		[RequiredByNativeCode]
		private void InvokeMeshReadyDelegate(MeshGenerationResult result, Action<MeshGenerationResult> onMeshGenerationComplete)
		{
			if (onMeshGenerationComplete != null)
			{
				onMeshGenerationComplete(result);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GenerateMeshAsync_Injected(ref TrackableId meshId, Mesh mesh, MeshCollider meshCollider, MeshVertexAttributes attributes, Action<MeshGenerationResult> onMeshGenerationComplete);
	}
}
