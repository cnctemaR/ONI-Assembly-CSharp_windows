using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Graphics/Graphics.bindings.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class LightProbes : Object
	{
		internal LightProbes()
		{
			LightProbes.Internal_Create(this);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Create([Writable] LightProbes self);

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action lightProbesUpdated;

		[RequiredByNativeCode]
		private static void Internal_CallLightProbesUpdatedFunction()
		{
			bool flag = LightProbes.lightProbesUpdated != null;
			if (flag)
			{
				LightProbes.lightProbesUpdated();
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action tetrahedralizationCompleted;

		[RequiredByNativeCode]
		private static void Internal_CallTetrahedralizationCompletedFunction()
		{
			bool flag = LightProbes.tetrahedralizationCompleted != null;
			if (flag)
			{
				LightProbes.tetrahedralizationCompleted();
			}
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action needsRetetrahedralization;

		[RequiredByNativeCode]
		private static void Internal_CallNeedsRetetrahedralizationFunction()
		{
			bool flag = LightProbes.needsRetetrahedralization != null;
			if (flag)
			{
				LightProbes.needsRetetrahedralization();
			}
		}

		[FreeFunction]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Tetrahedralize();

		[FreeFunction]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void TetrahedralizeAsync();

		[FreeFunction]
		public static void GetInterpolatedProbe(Vector3 position, Renderer renderer, out SphericalHarmonicsL2 probe)
		{
			LightProbes.GetInterpolatedProbe_Injected(ref position, Object.MarshalledUnityObject.Marshal<Renderer>(renderer), out probe);
		}

		[FreeFunction]
		internal static bool AreLightProbesAllowed(Renderer renderer)
		{
			return LightProbes.AreLightProbesAllowed_Injected(Object.MarshalledUnityObject.Marshal<Renderer>(renderer));
		}

		public static void CalculateInterpolatedLightAndOcclusionProbes(Vector3[] positions, SphericalHarmonicsL2[] lightProbes, Vector4[] occlusionProbes)
		{
			bool flag = positions == null;
			if (flag)
			{
				throw new ArgumentNullException("positions");
			}
			bool flag2 = lightProbes == null && occlusionProbes == null;
			if (flag2)
			{
				throw new ArgumentException("Argument lightProbes and occlusionProbes cannot both be null.");
			}
			bool flag3 = lightProbes != null && lightProbes.Length < positions.Length;
			if (flag3)
			{
				throw new ArgumentException("lightProbes", "Argument lightProbes has less elements than positions");
			}
			bool flag4 = occlusionProbes != null && occlusionProbes.Length < positions.Length;
			if (flag4)
			{
				throw new ArgumentException("occlusionProbes", "Argument occlusionProbes has less elements than positions");
			}
			LightProbes.CalculateInterpolatedLightAndOcclusionProbes_Internal(positions, positions.Length, lightProbes, occlusionProbes);
		}

		public static void CalculateInterpolatedLightAndOcclusionProbes(List<Vector3> positions, List<SphericalHarmonicsL2> lightProbes, List<Vector4> occlusionProbes)
		{
			bool flag = positions == null;
			if (flag)
			{
				throw new ArgumentNullException("positions");
			}
			bool flag2 = lightProbes == null && occlusionProbes == null;
			if (flag2)
			{
				throw new ArgumentException("Argument lightProbes and occlusionProbes cannot both be null.");
			}
			bool flag3 = lightProbes != null;
			if (flag3)
			{
				NoAllocHelpers.EnsureListElemCount<SphericalHarmonicsL2>(lightProbes, positions.Count);
			}
			bool flag4 = occlusionProbes != null;
			if (flag4)
			{
				NoAllocHelpers.EnsureListElemCount<Vector4>(occlusionProbes, positions.Count);
			}
			LightProbes.CalculateInterpolatedLightAndOcclusionProbes_Internal(NoAllocHelpers.ExtractArrayFromList<Vector3>(positions), positions.Count, NoAllocHelpers.ExtractArrayFromList<SphericalHarmonicsL2>(lightProbes), NoAllocHelpers.ExtractArrayFromList<Vector4>(occlusionProbes));
		}

		[FreeFunction]
		[NativeName("CalculateInterpolatedLightAndOcclusionProbes")]
		internal unsafe static void CalculateInterpolatedLightAndOcclusionProbes_Internal(Vector3[] positions, int positionsCount, SphericalHarmonicsL2[] lightProbes, Vector4[] occlusionProbes)
		{
			Span<Vector3> span = new Span<Vector3>(positions);
			fixed (Vector3* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<SphericalHarmonicsL2> span2 = new Span<SphericalHarmonicsL2>(lightProbes);
				fixed (SphericalHarmonicsL2* ptr2 = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, span2.Length);
					Span<Vector4> span3 = new Span<Vector4>(occlusionProbes);
					fixed (Vector4* pinnableReference = span3.GetPinnableReference())
					{
						ManagedSpanWrapper managedSpanWrapper3 = new ManagedSpanWrapper((void*)pinnableReference, span3.Length);
						LightProbes.CalculateInterpolatedLightAndOcclusionProbes_Internal_Injected(ref managedSpanWrapper, positionsCount, ref managedSpanWrapper2, ref managedSpanWrapper3);
						ptr = null;
						ptr2 = null;
					}
				}
			}
		}

		[NativeName("GetSharedLightProbesForScene")]
		[FreeFunction]
		public static LightProbes GetSharedLightProbesForScene(Scene scene)
		{
			return Unmarshal.UnmarshalUnityObject<LightProbes>(LightProbes.GetSharedLightProbesForScene_Injected(ref scene));
		}

		[NativeName("GetInstantiatedLightProbesForScene")]
		[FreeFunction]
		public static LightProbes GetInstantiatedLightProbesForScene(Scene scene)
		{
			return Unmarshal.UnmarshalUnityObject<LightProbes>(LightProbes.GetInstantiatedLightProbesForScene_Injected(ref scene));
		}

		public unsafe Vector3[] positions
		{
			[NativeName("GetLightProbePositions")]
			[FreeFunction(HasExplicitThis = true)]
			get
			{
				Vector3[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					LightProbes.get_positions_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Vector3[] array;
					blittableArrayWrapper.Unmarshal<Vector3>(ref array);
					array2 = array;
				}
				return array2;
			}
			[FreeFunction(HasExplicitThis = true)]
			[NativeName("SetLightProbePositions")]
			internal set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<Vector3> span = new Span<Vector3>(value);
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					LightProbes.set_positions_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		[NativeName("GetLightProbePositionsSelf")]
		[FreeFunction(HasExplicitThis = true)]
		public Vector3[] GetPositionsSelf()
		{
			Vector3[] array2;
			try
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				BlittableArrayWrapper blittableArrayWrapper;
				LightProbes.GetPositionsSelf_Injected(intPtr, out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				Vector3[] array;
				blittableArrayWrapper.Unmarshal<Vector3>(ref array);
				array2 = array;
			}
			return array2;
		}

		[NativeName("SetLightProbePositionsSelf")]
		[FreeFunction(HasExplicitThis = true)]
		public unsafe bool SetPositionsSelf(Vector3[] positions, bool checkForDuplicatePositions)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<Vector3> span = new Span<Vector3>(positions);
			bool flag;
			fixed (Vector3* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				flag = LightProbes.SetPositionsSelf_Injected(intPtr, ref managedSpanWrapper, checkForDuplicatePositions);
			}
			return flag;
		}

		public unsafe SphericalHarmonicsL2[] bakedProbes
		{
			[FreeFunction(HasExplicitThis = true)]
			[NativeName("GetBakedCoefficients")]
			get
			{
				SphericalHarmonicsL2[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					LightProbes.get_bakedProbes_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					SphericalHarmonicsL2[] array;
					blittableArrayWrapper.Unmarshal<SphericalHarmonicsL2>(ref array);
					array2 = array;
				}
				return array2;
			}
			[NativeName("SetBakedCoefficients")]
			[FreeFunction(HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<SphericalHarmonicsL2> span = new Span<SphericalHarmonicsL2>(value);
				fixed (SphericalHarmonicsL2* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					LightProbes.set_bakedProbes_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		[FreeFunction(HasExplicitThis = true)]
		internal unsafe void SetBakedCoefficients_Internal(SphericalHarmonicsL2[] coefficients)
		{
			IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
			if (intPtr == 0)
			{
				ThrowHelper.ThrowNullReferenceException(this);
			}
			Span<SphericalHarmonicsL2> span = new Span<SphericalHarmonicsL2>(coefficients);
			fixed (SphericalHarmonicsL2* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				LightProbes.SetBakedCoefficients_Internal_Injected(intPtr, ref managedSpanWrapper);
			}
		}

		public int count
		{
			[FreeFunction(HasExplicitThis = true)]
			[NativeName("GetLightProbeCount")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightProbes.get_count_Injected(intPtr);
			}
		}

		public int countSelf
		{
			[FreeFunction(HasExplicitThis = true)]
			[NativeName("GetLightProbeCountSelf")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightProbes.get_countSelf_Injected(intPtr);
			}
		}

		public int cellCount
		{
			[FreeFunction(HasExplicitThis = true)]
			[NativeName("GetTetrahedraSize")]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightProbes.get_cellCount_Injected(intPtr);
			}
		}

		public int cellCountSelf
		{
			[NativeName("GetTetrahedraSizeSelf")]
			[FreeFunction(HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				return LightProbes.get_cellCountSelf_Injected(intPtr);
			}
		}

		internal unsafe LightProbes.Hash128IntPair[] nonTetrahedralizedProbeSetIndexMap
		{
			[NativeName("GetNonTetrahedralizedProbeSetIndexMap")]
			[FreeFunction(HasExplicitThis = true)]
			get
			{
				LightProbes.Hash128IntPair[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					LightProbes.get_nonTetrahedralizedProbeSetIndexMap_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					LightProbes.Hash128IntPair[] array;
					blittableArrayWrapper.Unmarshal<LightProbes.Hash128IntPair>(ref array);
					array2 = array;
				}
				return array2;
			}
			[NativeName("SetNonTetrahedralizedProbeSetIndexMap")]
			[FreeFunction(HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<LightProbes.Hash128IntPair> span = new Span<LightProbes.Hash128IntPair>(value);
				fixed (LightProbes.Hash128IntPair* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					LightProbes.set_nonTetrahedralizedProbeSetIndexMap_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		internal unsafe LightProbeOcclusion[] bakedLightOcclusion
		{
			[NativeName("GetBakedLightOcclusion")]
			[FreeFunction(HasExplicitThis = true)]
			get
			{
				LightProbeOcclusion[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					LightProbes.get_bakedLightOcclusion_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					LightProbeOcclusion[] array;
					blittableArrayWrapper.Unmarshal<LightProbeOcclusion>(ref array);
					array2 = array;
				}
				return array2;
			}
			[FreeFunction(HasExplicitThis = true)]
			[NativeName("SetBakedLightOcclusion")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<LightProbeOcclusion> span = new Span<LightProbeOcclusion>(value);
				fixed (LightProbeOcclusion* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					LightProbes.set_bakedLightOcclusion_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		internal Bounds boundingBox
		{
			[NativeName("GetBoundingBox")]
			[FreeFunction(HasExplicitThis = true)]
			get
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Bounds bounds;
				LightProbes.get_boundingBox_Injected(intPtr, out bounds);
				return bounds;
			}
			[NativeName("SetBoundingBox")]
			[FreeFunction(HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				LightProbes.set_boundingBox_Injected(intPtr, ref value);
			}
		}

		internal unsafe ProbeSetIndex[] probeSets
		{
			[FreeFunction(HasExplicitThis = true)]
			[NativeName("GetProbeSets")]
			get
			{
				ProbeSetIndex[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					LightProbes.get_probeSets_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					ProbeSetIndex[] array;
					blittableArrayWrapper.Unmarshal<ProbeSetIndex>(ref array);
					array2 = array;
				}
				return array2;
			}
			[FreeFunction(HasExplicitThis = true)]
			[NativeName("SetProbeSets")]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<ProbeSetIndex> span = new Span<ProbeSetIndex>(value);
				fixed (ProbeSetIndex* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					LightProbes.set_probeSets_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		internal unsafe Tetrahedron[] tetrahedra
		{
			[FreeFunction(HasExplicitThis = true)]
			[NativeName("GetTetrahedra")]
			get
			{
				Tetrahedron[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					LightProbes.get_tetrahedra_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Tetrahedron[] array;
					blittableArrayWrapper.Unmarshal<Tetrahedron>(ref array);
					array2 = array;
				}
				return array2;
			}
			[NativeName("SetTetrahedra")]
			[FreeFunction(HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<Tetrahedron> span = new Span<Tetrahedron>(value);
				fixed (Tetrahedron* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					LightProbes.set_tetrahedra_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		internal unsafe Vector3[] hullRays
		{
			[NativeName("GetHullRays")]
			[FreeFunction(HasExplicitThis = true)]
			get
			{
				Vector3[] array2;
				try
				{
					IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
					if (intPtr == 0)
					{
						ThrowHelper.ThrowNullReferenceException(this);
					}
					BlittableArrayWrapper blittableArrayWrapper;
					LightProbes.get_hullRays_Injected(intPtr, out blittableArrayWrapper);
				}
				finally
				{
					BlittableArrayWrapper blittableArrayWrapper;
					Vector3[] array;
					blittableArrayWrapper.Unmarshal<Vector3>(ref array);
					array2 = array;
				}
				return array2;
			}
			[NativeName("SetHullRays")]
			[FreeFunction(HasExplicitThis = true)]
			set
			{
				IntPtr intPtr = Object.MarshalledUnityObject.MarshalNotNull<LightProbes>(this);
				if (intPtr == 0)
				{
					ThrowHelper.ThrowNullReferenceException(this);
				}
				Span<Vector3> span = new Span<Vector3>(value);
				fixed (Vector3* pinnableReference = span.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
					LightProbes.set_hullRays_Injected(intPtr, ref managedSpanWrapper);
				}
			}
		}

		[NativeName("GetLightProbeCount")]
		[FreeFunction]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetCount();

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Use GetInterpolatedProbe instead.", true)]
		public void GetInterpolatedLightProbe(Vector3 position, Renderer renderer, float[] coefficients)
		{
		}

		[Obsolete("Use bakedProbes instead.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public float[] coefficients
		{
			get
			{
				return new float[0];
			}
			set
			{
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetInterpolatedProbe_Injected([In] ref Vector3 position, IntPtr renderer, out SphericalHarmonicsL2 probe);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool AreLightProbesAllowed_Injected(IntPtr renderer);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CalculateInterpolatedLightAndOcclusionProbes_Internal_Injected(ref ManagedSpanWrapper positions, int positionsCount, ref ManagedSpanWrapper lightProbes, ref ManagedSpanWrapper occlusionProbes);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetSharedLightProbesForScene_Injected([In] ref Scene scene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr GetInstantiatedLightProbesForScene_Injected([In] ref Scene scene);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_positions_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_positions_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetPositionsSelf_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SetPositionsSelf_Injected(IntPtr _unity_self, ref ManagedSpanWrapper positions, bool checkForDuplicatePositions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_bakedProbes_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bakedProbes_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetBakedCoefficients_Internal_Injected(IntPtr _unity_self, ref ManagedSpanWrapper coefficients);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_count_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_countSelf_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_cellCount_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int get_cellCountSelf_Injected(IntPtr _unity_self);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_nonTetrahedralizedProbeSetIndexMap_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_nonTetrahedralizedProbeSetIndexMap_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_bakedLightOcclusion_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_bakedLightOcclusion_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_boundingBox_Injected(IntPtr _unity_self, out Bounds ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_boundingBox_Injected(IntPtr _unity_self, [In] ref Bounds value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_probeSets_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_probeSets_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_tetrahedra_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_tetrahedra_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_hullRays_Injected(IntPtr _unity_self, out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_hullRays_Injected(IntPtr _unity_self, ref ManagedSpanWrapper value);

		internal struct Hash128IntPair
		{
			internal Hash128 Hash;

			internal int Value;
		}
	}
}
