using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Graphics/Graphics.bindings.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class LightProbes : Object
	{
		private LightProbes()
		{
		}

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
			LightProbes.GetInterpolatedProbe_Injected(ref position, renderer, out probe);
		}

		[FreeFunction]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool AreLightProbesAllowed(Renderer renderer);

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
				bool flag4 = lightProbes.Capacity < positions.Count;
				if (flag4)
				{
					lightProbes.Capacity = positions.Count;
				}
				bool flag5 = lightProbes.Count < positions.Count;
				if (flag5)
				{
					NoAllocHelpers.ResizeList<SphericalHarmonicsL2>(lightProbes, positions.Count);
				}
			}
			bool flag6 = occlusionProbes != null;
			if (flag6)
			{
				bool flag7 = occlusionProbes.Capacity < positions.Count;
				if (flag7)
				{
					occlusionProbes.Capacity = positions.Count;
				}
				bool flag8 = occlusionProbes.Count < positions.Count;
				if (flag8)
				{
					NoAllocHelpers.ResizeList<Vector4>(occlusionProbes, positions.Count);
				}
			}
			LightProbes.CalculateInterpolatedLightAndOcclusionProbes_Internal(NoAllocHelpers.ExtractArrayFromListT<Vector3>(positions), positions.Count, NoAllocHelpers.ExtractArrayFromListT<SphericalHarmonicsL2>(lightProbes), NoAllocHelpers.ExtractArrayFromListT<Vector4>(occlusionProbes));
		}

		[FreeFunction]
		[NativeName("CalculateInterpolatedLightAndOcclusionProbes")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CalculateInterpolatedLightAndOcclusionProbes_Internal([Unmarshalled] Vector3[] positions, int positionsCount, [Unmarshalled] SphericalHarmonicsL2[] lightProbes, [Unmarshalled] Vector4[] occlusionProbes);

		public extern Vector3[] positions
		{
			[FreeFunction(HasExplicitThis = true)]
			[NativeName("GetLightProbePositions")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern SphericalHarmonicsL2[] bakedProbes
		{
			[NativeName("GetBakedCoefficients")]
			[FreeFunction(HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[FreeFunction(HasExplicitThis = true)]
			[NativeName("SetBakedCoefficients")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int count
		{
			[NativeName("GetLightProbeCount")]
			[FreeFunction(HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int cellCount
		{
			[NativeName("GetTetrahedraSize")]
			[FreeFunction(HasExplicitThis = true)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[FreeFunction]
		[NativeName("GetLightProbeCount")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern int GetCount();

		[Obsolete("Use GetInterpolatedProbe instead.", true)]
		[EditorBrowsable(EditorBrowsableState.Never)]
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
		private static extern void GetInterpolatedProbe_Injected(ref Vector3 position, Renderer renderer, out SphericalHarmonicsL2 probe);
	}
}
