using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeType("Modules/Marshalling/MarshallingTests.h")]
	internal class ValueTypeArrayTests
	{
		[NativeThrows]
		public unsafe static void ParameterIntArrayReadOnly(int[] param)
		{
			Span<int> span = new Span<int>(param);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ValueTypeArrayTests.ParameterIntArrayReadOnly_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterIntArrayWritable(int[] param)
		{
			Span<int> span = new Span<int>(param);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ValueTypeArrayTests.ParameterIntArrayWritable_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterIntArrayEmpty(int[] param, int[] param2)
		{
			Span<int> span = new Span<int>(param);
			fixed (int* ptr = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, span.Length);
				Span<int> span2 = new Span<int>(param2);
				fixed (int* pinnableReference = span2.GetPinnableReference())
				{
					ManagedSpanWrapper managedSpanWrapper2 = new ManagedSpanWrapper((void*)pinnableReference, span2.Length);
					ValueTypeArrayTests.ParameterIntArrayEmpty_Injected(ref managedSpanWrapper, ref managedSpanWrapper2);
					ptr = null;
				}
			}
		}

		public unsafe static void ParameterIntArrayNullExceptions([NotNull] int[] param)
		{
			if (param == null)
			{
				ThrowHelper.ThrowArgumentNullException(param, "param");
			}
			Span<int> span = new Span<int>(param);
			fixed (int* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ValueTypeArrayTests.ParameterIntArrayNullExceptions_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterIntMultidimensionalArray(int[,] param)
		{
			int num;
			void* ptr;
			if (param == null || (num = param.Length) == 0)
			{
				num = 0;
				ptr = null;
			}
			else
			{
				ptr = (void*)(&param[0, 0]);
			}
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(ptr, num);
			ValueTypeArrayTests.ParameterIntMultidimensionalArray_Injected(ref managedSpanWrapper);
		}

		public unsafe static void ParameterIntMultidimensionalArrayNullExceptions([NotNull] int[,] param)
		{
			if (param == null)
			{
				ThrowHelper.ThrowArgumentNullException(param, "param");
			}
			int num;
			void* ptr;
			if (param == null || (num = param.Length) == 0)
			{
				num = 0;
				ptr = null;
			}
			else
			{
				ptr = (void*)(&param[0, 0]);
			}
			ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper(ptr, num);
			ValueTypeArrayTests.ParameterIntMultidimensionalArrayNullExceptions_Injected(ref managedSpanWrapper);
		}

		[NativeThrows]
		public unsafe static void ParameterCharArrayReadOnly(char[] param)
		{
			Span<char> span = new Span<char>(param);
			fixed (char* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ValueTypeArrayTests.ParameterCharArrayReadOnly_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterBlittableCornerCaseStructArrayReadOnly(BlittableCornerCases[] param)
		{
			Span<BlittableCornerCases> span = new Span<BlittableCornerCases>(param);
			fixed (BlittableCornerCases* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				ValueTypeArrayTests.ParameterBlittableCornerCaseStructArrayReadOnly_Injected(ref managedSpanWrapper);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterIntArrayOutAttr([Out] int[] param)
		{
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (param != null)
				{
					fixed (int[] array = param)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				ValueTypeArrayTests.ParameterIntArrayOutAttr_Injected(out blittableArrayWrapper);
			}
			finally
			{
				int[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<int>(ref array);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterCharArrayOutAttr([Out] char[] param)
		{
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (param != null)
				{
					fixed (char[] array = param)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				ValueTypeArrayTests.ParameterCharArrayOutAttr_Injected(out blittableArrayWrapper);
			}
			finally
			{
				char[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<char>(ref array);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterBlittableCornerCaseStructArrayOutAttr([Out] BlittableCornerCases[] param)
		{
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (param != null)
				{
					fixed (BlittableCornerCases[] array = param)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				ValueTypeArrayTests.ParameterBlittableCornerCaseStructArrayOutAttr_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableCornerCases[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<BlittableCornerCases>(ref array);
			}
		}

		public static int[] ParameterIntArrayReturn()
		{
			int[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ValueTypeArrayTests.ParameterIntArrayReturn_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static int[] ParameterIntArrayReturnEmpty()
		{
			int[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ValueTypeArrayTests.ParameterIntArrayReturnEmpty_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static int[] ParameterIntArrayReturnNull()
		{
			int[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ValueTypeArrayTests.ParameterIntArrayReturnNull_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				int[] array;
				blittableArrayWrapper.Unmarshal<int>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static char[] ParameterCharArrayReturn()
		{
			char[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ValueTypeArrayTests.ParameterCharArrayReturn_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				char[] array;
				blittableArrayWrapper.Unmarshal<char>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static BlittableCornerCases[] ParameterBlittableCornerCaseStructArrayReturn()
		{
			BlittableCornerCases[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ValueTypeArrayTests.ParameterBlittableCornerCaseStructArrayReturn_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				BlittableCornerCases[] array;
				blittableArrayWrapper.Unmarshal<BlittableCornerCases>(ref array);
				array2 = array;
			}
			return array2;
		}

		public static int[] CreateAndFillArray1UsingIn()
		{
			OutArray<int> outArray = default(OutArray<int>);
			ValueTypeArrayTests.CreateAndFillArray1In(in outArray);
			return outArray.Value;
		}

		public static int[] CreateAndFillArray1UsingRef()
		{
			OutArray<int> outArray = default(OutArray<int>);
			ValueTypeArrayTests.CreateAndFillArray1Ref(ref outArray);
			return outArray.Value;
		}

		public static int[,] CreateAndFillArray2UsingIn()
		{
			OutArray2D<int> outArray2D = default(OutArray2D<int>);
			ValueTypeArrayTests.CreateAndFillArray2In(in outArray2D);
			return outArray2D.Value;
		}

		public static int[,] CreateAndFillArray2UsingRef()
		{
			OutArray2D<int> outArray2D = default(OutArray2D<int>);
			ValueTypeArrayTests.CreateAndFillArray2Ref(ref outArray2D);
			return outArray2D.Value;
		}

		public static int[,,] CreateAndFillArray3UsingIn()
		{
			OutArray3D<int> outArray3D = default(OutArray3D<int>);
			ValueTypeArrayTests.CreateAndFillArray3In(in outArray3D);
			return outArray3D.Value;
		}

		public static int[,,] CreateAndFillArray3UsingRef()
		{
			OutArray3D<int> outArray3D = default(OutArray3D<int>);
			ValueTypeArrayTests.CreateAndFillArray3Ref(ref outArray3D);
			return outArray3D.Value;
		}

		[NativeName("CreateAndFillArray1")]
		private static void CreateAndFillArray1In(in OutArray<int> outArray)
		{
			OutArrayNativeData outArrayNativeData = OutArray<int>.BindingsMarshaller.ConvertToUnmanaged(ref outArray);
			ValueTypeArrayTests.CreateAndFillArray1In_Injected(in outArrayNativeData);
		}

		[NativeName("CreateAndFillArray1")]
		private static void CreateAndFillArray1Ref(ref OutArray<int> outArray)
		{
			OutArrayNativeData outArrayNativeData = OutArray<int>.BindingsMarshaller.ConvertToUnmanaged(ref outArray);
			ValueTypeArrayTests.CreateAndFillArray1Ref_Injected(ref outArrayNativeData);
			outArray = OutArray<int>.BindingsMarshaller.ConvertToManaged(in outArrayNativeData);
		}

		[NativeName("CreateAndFillArray2")]
		private static void CreateAndFillArray2In(in OutArray2D<int> outArray)
		{
			OutArrayNativeData outArrayNativeData = OutArray2D<int>.BindingsMarshaller.ConvertToUnmanaged(ref outArray);
			ValueTypeArrayTests.CreateAndFillArray2In_Injected(in outArrayNativeData);
		}

		[NativeName("CreateAndFillArray2")]
		private static void CreateAndFillArray2Ref(ref OutArray2D<int> outArray)
		{
			OutArrayNativeData outArrayNativeData = OutArray2D<int>.BindingsMarshaller.ConvertToUnmanaged(ref outArray);
			ValueTypeArrayTests.CreateAndFillArray2Ref_Injected(ref outArrayNativeData);
			outArray = OutArray2D<int>.BindingsMarshaller.ConvertToManaged(in outArrayNativeData);
		}

		[NativeName("CreateAndFillArray3")]
		private static void CreateAndFillArray3In(in OutArray3D<int> outArray)
		{
			OutArrayNativeData outArrayNativeData = OutArray3D<int>.BindingsMarshaller.ConvertToUnmanaged(ref outArray);
			ValueTypeArrayTests.CreateAndFillArray3In_Injected(in outArrayNativeData);
		}

		[NativeName("CreateAndFillArray3")]
		private static void CreateAndFillArray3Ref(ref OutArray3D<int> outArray)
		{
			OutArrayNativeData outArrayNativeData = OutArray3D<int>.BindingsMarshaller.ConvertToUnmanaged(ref outArray);
			ValueTypeArrayTests.CreateAndFillArray3Ref_Injected(ref outArrayNativeData);
			outArray = OutArray3D<int>.BindingsMarshaller.ConvertToManaged(in outArrayNativeData);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayReadOnly_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayWritable_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayEmpty_Injected(ref ManagedSpanWrapper param, ref ManagedSpanWrapper param2);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayNullExceptions_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntMultidimensionalArray_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntMultidimensionalArrayNullExceptions_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCharArrayReadOnly_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterBlittableCornerCaseStructArrayReadOnly_Injected(ref ManagedSpanWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayOutAttr_Injected(out BlittableArrayWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCharArrayOutAttr_Injected(out BlittableArrayWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterBlittableCornerCaseStructArrayOutAttr_Injected(out BlittableArrayWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayReturn_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayReturnEmpty_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterIntArrayReturnNull_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCharArrayReturn_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterBlittableCornerCaseStructArrayReturn_Injected(out BlittableArrayWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateAndFillArray1In_Injected(in OutArrayNativeData outArray);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateAndFillArray1Ref_Injected(ref OutArrayNativeData outArray);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateAndFillArray2In_Injected(in OutArrayNativeData outArray);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateAndFillArray2Ref_Injected(ref OutArrayNativeData outArray);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateAndFillArray3In_Injected(in OutArrayNativeData outArray);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void CreateAndFillArray3Ref_Injected(ref OutArrayNativeData outArray);
	}
}
