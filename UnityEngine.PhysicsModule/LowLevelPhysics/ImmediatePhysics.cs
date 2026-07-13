using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.LowLevelPhysics
{
	[NativeHeader("Modules/Physics/ImmediatePhysics.h")]
	public static class ImmediatePhysics
	{
		[FreeFunction("Physics::Immediate::GenerateContacts", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern int GenerateContacts_Native(void* geom1, void* geom2, void* xform1, void* xform2, int numPairs, void* contacts, int contactArrayLength, void* sizes, int sizesArrayLength, float contactDistance);

		public static int GenerateContacts(NativeArray<GeometryHolder>.ReadOnly geom1, NativeArray<GeometryHolder>.ReadOnly geom2, NativeArray<ImmediateTransform>.ReadOnly xform1, NativeArray<ImmediateTransform>.ReadOnly xform2, int pairCount, NativeArray<ImmediateContact> outContacts, NativeArray<int> outContactCounts, float contactDistance = 0.01f)
		{
			bool flag = geom1.Length < pairCount || geom2.Length < pairCount || xform1.Length < pairCount || xform2.Length < pairCount;
			if (flag)
			{
				throw new ArgumentException("Provided geometry or transform arrays are not large enough to fit the count of pairs.");
			}
			bool flag2 = pairCount > outContactCounts.Length;
			if (flag2)
			{
				throw new ArgumentException("The output contact counts array is not big enough. The size of the array needs to match or exceed the amount of pairs.");
			}
			bool flag3 = contactDistance <= 0f;
			if (flag3)
			{
				throw new ArgumentException("Contact distance must be positive and not equal to zero.");
			}
			return ImmediatePhysics.GenerateContacts_Native(geom1.GetUnsafeReadOnlyPtr<GeometryHolder>(), geom2.GetUnsafeReadOnlyPtr<GeometryHolder>(), xform1.GetUnsafeReadOnlyPtr<ImmediateTransform>(), xform2.GetUnsafeReadOnlyPtr<ImmediateTransform>(), pairCount, outContacts.GetUnsafePtr<ImmediateContact>(), outContacts.Length, outContactCounts.GetUnsafePtr<int>(), outContactCounts.Length, contactDistance);
		}
	}
}
