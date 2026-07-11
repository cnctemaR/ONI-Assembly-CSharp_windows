using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Rendering;

namespace UnityEngine
{
	[NativeAsStruct]
	[NativeHeader("Runtime/Export/Graphics.bindings.h")]
	[StructLayout(LayoutKind.Sequential)]
	public sealed class LightProbes : Object
	{
		private LightProbes()
		{
		}

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
			if (positions == null)
			{
				throw new ArgumentNullException("positions");
			}
			if (lightProbes == null && occlusionProbes == null)
			{
				throw new ArgumentException("Argument lightProbes and occlusionProbes cannot both be null.");
			}
			if (lightProbes != null && lightProbes.Length < positions.Length)
			{
				throw new ArgumentException("lightProbes", "Argument lightProbes has less elements than positions");
			}
			if (occlusionProbes != null && occlusionProbes.Length < positions.Length)
			{
				throw new ArgumentException("occlusionProbes", "Argument occlusionProbes has less elements than positions");
			}
			LightProbes.CalculateInterpolatedLightAndOcclusionProbes_Internal(positions, positions.Length, lightProbes, occlusionProbes);
		}

		public static void CalculateInterpolatedLightAndOcclusionProbes(List<Vector3> positions, List<SphericalHarmonicsL2> lightProbes, List<Vector4> occlusionProbes)
		{
			if (positions == null)
			{
				throw new ArgumentNullException("positions");
			}
			if (lightProbes == null && occlusionProbes == null)
			{
				throw new ArgumentException("Argument lightProbes and occlusionProbes cannot both be null.");
			}
			if (lightProbes != null)
			{
				if (lightProbes.Capacity < positions.Count)
				{
					lightProbes.Capacity = positions.Count;
				}
				if (lightProbes.Count < positions.Count)
				{
					NoAllocHelpers.ResizeList<SphericalHarmonicsL2>(lightProbes, positions.Count);
				}
			}
			if (occlusionProbes != null)
			{
				if (occlusionProbes.Capacity < positions.Count)
				{
					occlusionProbes.Capacity = positions.Count;
				}
				if (occlusionProbes.Count < positions.Count)
				{
					NoAllocHelpers.ResizeList<Vector4>(occlusionProbes, positions.Count);
				}
			}
			LightProbes.CalculateInterpolatedLightAndOcclusionProbes_Internal(NoAllocHelpers.ExtractArrayFromListT<Vector3>(positions), positions.Count, NoAllocHelpers.ExtractArrayFromListT<SphericalHarmonicsL2>(lightProbes), NoAllocHelpers.ExtractArrayFromListT<Vector4>(occlusionProbes));
		}

		[FreeFunction]
		[NativeName("CalculateInterpolatedLightAndOcclusionProbes")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void CalculateInterpolatedLightAndOcclusionProbes_Internal(Vector3[] positions, int positionsCount, SphericalHarmonicsL2[] lightProbes, Vector4[] occlusionProbes);

		public extern Vector3[] positions
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern SphericalHarmonicsL2[] bakedProbes
		{
			[NativeName("GetBakedCoefficients")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[NativeName("SetBakedCoefficients")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		public extern int count
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int cellCount
		{
			[NativeName("GetTetrahedraSize")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

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
		private static extern void GetInterpolatedProbe_Injected(ref Vector3 position, Renderer renderer, out SphericalHarmonicsL2 probe);
	}
}
