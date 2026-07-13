using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeType("Modules/Marshalling/MarshallingTests.h")]
	internal class ValueTypeListOfTTests
	{
		[NativeThrows]
		public unsafe static void ParameterListOfIntRead(List<int> param)
		{
			try
			{
				BlittableListWrapper blittableListWrapper;
				if (param != null)
				{
					fixed (int[] array = NoAllocHelpers.ExtractArrayFromList<int>(param))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, param.Count);
					}
				}
				ValueTypeListOfTTests.ParameterListOfIntRead_Injected(ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<int>(param);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterListOfIntReadChangeVaules(List<int> param)
		{
			try
			{
				BlittableListWrapper blittableListWrapper;
				if (param != null)
				{
					fixed (int[] array = NoAllocHelpers.ExtractArrayFromList<int>(param))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, param.Count);
					}
				}
				ValueTypeListOfTTests.ParameterListOfIntReadChangeVaules_Injected(ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<int>(param);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterListOfIntAddNoGrow(List<int> param)
		{
			try
			{
				BlittableListWrapper blittableListWrapper;
				if (param != null)
				{
					fixed (int[] array = NoAllocHelpers.ExtractArrayFromList<int>(param))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, param.Count);
					}
				}
				ValueTypeListOfTTests.ParameterListOfIntAddNoGrow_Injected(ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<int>(param);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterListOfIntAddAndGrow(List<int> param)
		{
			try
			{
				BlittableListWrapper blittableListWrapper;
				if (param != null)
				{
					fixed (int[] array = NoAllocHelpers.ExtractArrayFromList<int>(param))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, param.Count);
					}
				}
				ValueTypeListOfTTests.ParameterListOfIntAddAndGrow_Injected(ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<int>(param);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterListOfIntPassNullThrow([NotNull] List<int> param)
		{
			if (param == null)
			{
				ThrowHelper.ThrowArgumentNullException(param, "param");
			}
			try
			{
				fixed (int[] array = NoAllocHelpers.ExtractArrayFromList<int>(param))
				{
					BlittableArrayWrapper blittableArrayWrapper;
					if (array.Length != 0)
					{
						blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
					}
					BlittableListWrapper blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, param.Count);
					ValueTypeListOfTTests.ParameterListOfIntPassNullThrow_Injected(ref blittableListWrapper);
				}
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<int>(param);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterListOfIntPassNullNoThrow(List<int> param)
		{
			try
			{
				BlittableListWrapper blittableListWrapper;
				if (param != null)
				{
					fixed (int[] array = NoAllocHelpers.ExtractArrayFromList<int>(param))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, param.Count);
					}
				}
				ValueTypeListOfTTests.ParameterListOfIntPassNullNoThrow_Injected(ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<int>(param);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterListOfIntNativeAllocateSmaller(List<int> param)
		{
			try
			{
				BlittableListWrapper blittableListWrapper;
				if (param != null)
				{
					fixed (int[] array = NoAllocHelpers.ExtractArrayFromList<int>(param))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, param.Count);
					}
				}
				ValueTypeListOfTTests.ParameterListOfIntNativeAllocateSmaller_Injected(ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<int>(param);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterListOfIntNativeAttachOtherMemoryBlock(List<int> param)
		{
			try
			{
				BlittableListWrapper blittableListWrapper;
				if (param != null)
				{
					fixed (int[] array = NoAllocHelpers.ExtractArrayFromList<int>(param))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, param.Count);
					}
				}
				ValueTypeListOfTTests.ParameterListOfIntNativeAttachOtherMemoryBlock_Injected(ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<int>(param);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterListOfIntNativeCallsClear(List<int> param)
		{
			try
			{
				BlittableListWrapper blittableListWrapper;
				if (param != null)
				{
					fixed (int[] array = NoAllocHelpers.ExtractArrayFromList<int>(param))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, param.Count);
					}
				}
				ValueTypeListOfTTests.ParameterListOfIntNativeCallsClear_Injected(ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<int>(param);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterListOfBoolReadWrite(List<bool> param)
		{
			try
			{
				BlittableListWrapper blittableListWrapper;
				if (param != null)
				{
					fixed (bool[] array = NoAllocHelpers.ExtractArrayFromList<bool>(param))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, param.Count);
					}
				}
				ValueTypeListOfTTests.ParameterListOfBoolReadWrite_Injected(ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<bool>(param);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterListOfCharReadWrite(List<char> param)
		{
			try
			{
				BlittableListWrapper blittableListWrapper;
				if (param != null)
				{
					fixed (char[] array = NoAllocHelpers.ExtractArrayFromList<char>(param))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, param.Count);
					}
				}
				ValueTypeListOfTTests.ParameterListOfCharReadWrite_Injected(ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<char>(param);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterListOfEnumReadWrite(List<SomeEnum> param)
		{
			try
			{
				BlittableListWrapper blittableListWrapper;
				if (param != null)
				{
					fixed (SomeEnum[] array = NoAllocHelpers.ExtractArrayFromList<SomeEnum>(param))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, param.Count);
					}
				}
				ValueTypeListOfTTests.ParameterListOfEnumReadWrite_Injected(ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<SomeEnum>(param);
			}
		}

		[NativeThrows]
		public unsafe static void ParameterListOfCornerCaseStructReadWrite(List<BlittableCornerCases> param)
		{
			try
			{
				BlittableListWrapper blittableListWrapper;
				if (param != null)
				{
					fixed (BlittableCornerCases[] array = NoAllocHelpers.ExtractArrayFromList<BlittableCornerCases>(param))
					{
						BlittableArrayWrapper blittableArrayWrapper;
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
						blittableListWrapper = new BlittableListWrapper(blittableArrayWrapper, param.Count);
					}
				}
				ValueTypeListOfTTests.ParameterListOfCornerCaseStructReadWrite_Injected(ref blittableListWrapper);
			}
			finally
			{
				BlittableListWrapper blittableListWrapper;
				blittableListWrapper.Unmarshal<BlittableCornerCases>(param);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterListOfIntRead_Injected(ref BlittableListWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterListOfIntReadChangeVaules_Injected(ref BlittableListWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterListOfIntAddNoGrow_Injected(ref BlittableListWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterListOfIntAddAndGrow_Injected(ref BlittableListWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterListOfIntPassNullThrow_Injected(ref BlittableListWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterListOfIntPassNullNoThrow_Injected(ref BlittableListWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterListOfIntNativeAllocateSmaller_Injected(ref BlittableListWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterListOfIntNativeAttachOtherMemoryBlock_Injected(ref BlittableListWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterListOfIntNativeCallsClear_Injected(ref BlittableListWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterListOfBoolReadWrite_Injected(ref BlittableListWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterListOfCharReadWrite_Injected(ref BlittableListWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterListOfEnumReadWrite_Injected(ref BlittableListWrapper param);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterListOfCornerCaseStructReadWrite_Injected(ref BlittableListWrapper param);
	}
}
