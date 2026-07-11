using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Tango
{
	[NativeConditional("PLATFORM_ANDROID")]
	[NativeHeader("PhysicsScriptingClasses.h")]
	[NativeHeader("Runtime/AR/Tango/TangoScriptApi.h")]
	[UsedByNativeCode]
	internal class MeshReconstructionServer
	{
		internal MeshReconstructionServer(MeshReconstructionConfig config)
		{
			int num = 0;
			this.m_ServerPtr = MeshReconstructionServer.Internal_Create(this, config, out num);
			this.m_Status = (MeshReconstructionServer.Status)num;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_ClearMeshes(IntPtr server);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool Internal_GetEnabled(IntPtr server);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_SetEnabled(IntPtr server, bool enabled);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_GetNativeReconstructionContextPtr(IntPtr server);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int Internal_GetNumGenerationRequests(IntPtr server);

		internal void Dispose()
		{
			if (this.m_ServerPtr != IntPtr.Zero)
			{
				MeshReconstructionServer.Destroy(this.m_ServerPtr);
				this.m_ServerPtr = IntPtr.Zero;
			}
			GC.SuppressFinalize(this);
		}

		private static IntPtr Internal_Create(MeshReconstructionServer self, MeshReconstructionConfig config, out int status)
		{
			return MeshReconstructionServer.Internal_Create_Injected(self, ref config, out status);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void Destroy(IntPtr server);

		[NativeMethod(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void DestroyThreaded(IntPtr server);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GetChangedSegments(IntPtr serverPtr, MeshReconstructionServer.SegmentChangedDelegate onSegmentChanged);

		private static void Internal_GenerateSegmentAsync(IntPtr serverPtr, GridIndex gridIndex, MeshFilter destinationMeshFilter, MeshCollider destinationMeshCollider, MeshReconstructionServer.SegmentReadyDelegate onSegmentReady, bool provideNormals, bool provideColors, bool providePhysics)
		{
			MeshReconstructionServer.Internal_GenerateSegmentAsync_Injected(serverPtr, ref gridIndex, destinationMeshFilter, destinationMeshCollider, onSegmentReady, provideNormals, provideColors, providePhysics);
		}

		internal MeshReconstructionServer.Status status
		{
			get
			{
				return this.m_Status;
			}
		}

		~MeshReconstructionServer()
		{
			if (this.m_ServerPtr != IntPtr.Zero)
			{
				MeshReconstructionServer.DestroyThreaded(this.m_ServerPtr);
				this.m_ServerPtr = IntPtr.Zero;
				GC.SuppressFinalize(this);
			}
		}

		internal void ClearMeshes()
		{
			MeshReconstructionServer.Internal_ClearMeshes(this.m_ServerPtr);
		}

		internal IntPtr GetNativeReconstructionContext()
		{
			return MeshReconstructionServer.Internal_GetNativeReconstructionContextPtr(this.m_ServerPtr);
		}

		internal void GetChangedSegments(MeshReconstructionServer.SegmentChangedDelegate onSegmentChanged)
		{
			if (onSegmentChanged == null)
			{
				throw new ArgumentNullException("onSegmentChanged");
			}
			MeshReconstructionServer.Internal_GetChangedSegments(this.m_ServerPtr, onSegmentChanged);
		}

		internal void GenerateSegmentAsync(SegmentGenerationRequest request, MeshReconstructionServer.SegmentReadyDelegate onSegmentReady)
		{
			if (onSegmentReady == null)
			{
				throw new ArgumentNullException("onSegmentRead");
			}
			MeshReconstructionServer.Internal_GenerateSegmentAsync(this.m_ServerPtr, request.gridIndex, request.destinationMeshFilter, request.destinationMeshCollider, onSegmentReady, request.provideNormals, request.provideColors, request.providePhysics);
		}

		internal int generationRequests
		{
			get
			{
				return MeshReconstructionServer.Internal_GetNumGenerationRequests(this.m_ServerPtr);
			}
		}

		internal bool enabled
		{
			get
			{
				return MeshReconstructionServer.Internal_GetEnabled(this.m_ServerPtr);
			}
			set
			{
				MeshReconstructionServer.Internal_SetEnabled(this.m_ServerPtr, value);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr Internal_Create_Injected(MeshReconstructionServer self, ref MeshReconstructionConfig config, out int status);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_GenerateSegmentAsync_Injected(IntPtr serverPtr, ref GridIndex gridIndex, MeshFilter destinationMeshFilter, MeshCollider destinationMeshCollider, MeshReconstructionServer.SegmentReadyDelegate onSegmentReady, bool provideNormals, bool provideColors, bool providePhysics);

		internal IntPtr m_ServerPtr = IntPtr.Zero;

		private MeshReconstructionServer.Status m_Status = MeshReconstructionServer.Status.UnsupportedPlatform;

		internal delegate void SegmentChangedDelegate(GridIndex gridIndex, SegmentChange changeType, double updateTime);

		internal delegate void SegmentReadyDelegate(SegmentGenerationResult generatedSegmentData);

		internal enum Status
		{
			UnsupportedPlatform,
			Ok,
			MissingMeshReconstructionLibrary,
			FailedToCreateMeshReconstructionContext,
			FailedToSetDepthCalibration
		}
	}
}
