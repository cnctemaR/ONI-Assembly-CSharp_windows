using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	internal class BlittableNestedCollectionMarshallerTests
	{
		[NativeThrows]
		[NativeMethod("BlittableNestedCollectionMarshallerTests::PassInNestedCollection")]
		public unsafe static void PassInNestedLists([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(BlittableNestedCollectionMarshaller<int>))] List<List<int>> nested, int exectedCount, int[] expectedValues1, int[] expectedValues2)
		{
			NestedCollectionData nestedCollectionData = BlittableNestedCollectionMarshaller<int>.ConvertToUnmanaged(nested);
			Span<int> span = new Span<int>(expectedValues1);
			fixed (int* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<int> span2 = new Span<int>(expectedValues2);
				fixed (int* pinnableReference = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span2.Length);
					BlittableNestedCollectionMarshallerTests.PassInNestedLists_Injected(ref nestedCollectionData, exectedCount, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		[NativeThrows]
		[NativeMethod("BlittableNestedCollectionMarshallerTests::PassInNestedCollection")]
		public unsafe static void PassInNestedArrays([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(BlittableNestedCollectionMarshaller<int>))] int[][] nested, int exectedCount, int[] expectedValues1, int[] expectedValues2)
		{
			NestedCollectionData nestedCollectionData = BlittableNestedCollectionMarshaller<int>.ConvertToUnmanaged(nested);
			Span<int> span = new Span<int>(expectedValues1);
			fixed (int* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<int> span2 = new Span<int>(expectedValues2);
				fixed (int* pinnableReference = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span2.Length);
					BlittableNestedCollectionMarshallerTests.PassInNestedArrays_Injected(ref nestedCollectionData, exectedCount, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		[NativeThrows]
		[NativeMethod("BlittableNestedCollectionMarshallerTests::PassInNestedCollection")]
		public unsafe static void PassInListOfInts([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(BlittableNestedCollectionMarshaller<int>))] List<int[]> nested, int exectedCount, int[] expectedValues1, int[] expectedValues2)
		{
			NestedCollectionData nestedCollectionData = BlittableNestedCollectionMarshaller<int>.ConvertToUnmanaged(nested);
			Span<int> span = new Span<int>(expectedValues1);
			fixed (int* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<int> span2 = new Span<int>(expectedValues2);
				fixed (int* pinnableReference = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span2.Length);
					BlittableNestedCollectionMarshallerTests.PassInListOfInts_Injected(ref nestedCollectionData, exectedCount, ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PassInNestedLists_Injected(ref NestedCollectionData nested, int exectedCount, ref ManagedSpanWrapper expectedValues1, ref ManagedSpanWrapper expectedValues2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PassInNestedArrays_Injected(ref NestedCollectionData nested, int exectedCount, ref ManagedSpanWrapper expectedValues1, ref ManagedSpanWrapper expectedValues2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PassInListOfInts_Injected(ref NestedCollectionData nested, int exectedCount, ref ManagedSpanWrapper expectedValues1, ref ManagedSpanWrapper expectedValues2);
	}
}
