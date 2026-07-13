using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	internal static class CustomMarshallingTests
	{
		[NativeThrows]
		public static void ParameterCustomMarshalled(CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			CustomMarshallingTests.ParameterCustomMarshalled_Injected(CustomMarshallingTests.CustomMarshalledClass.BindingsMarshaller.ConvertToUnmanaged(arg), expected);
		}

		[NativeThrows]
		public static void ParameterCustomMarshalledIn(in CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			int num = CustomMarshallingTests.CustomMarshalledClass.BindingsMarshaller.ConvertToUnmanaged(arg);
			CustomMarshallingTests.ParameterCustomMarshalledIn_Injected(in num, expected);
		}

		[NativeThrows]
		public static void ParameterCustomMarshalledOut(out CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			CustomMarshallingTests.CustomMarshalledClass customMarshalledClass;
			int num = CustomMarshallingTests.CustomMarshalledClass.BindingsMarshaller.ConvertToUnmanaged(customMarshalledClass);
			CustomMarshallingTests.ParameterCustomMarshalledOut_Injected(out num, expected);
			arg = CustomMarshallingTests.CustomMarshalledClass.BindingsMarshaller.ConvertToManaged(num);
		}

		[NativeThrows]
		public static void ParameterCustomMarshalledRef(ref CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			int num = CustomMarshallingTests.CustomMarshalledClass.BindingsMarshaller.ConvertToUnmanaged(arg);
			CustomMarshallingTests.ParameterCustomMarshalledRef_Injected(ref num, expected);
			arg = CustomMarshallingTests.CustomMarshalledClass.BindingsMarshaller.ConvertToManaged(num);
		}

		public static CustomMarshallingTests.CustomMarshalledClass ParameterCustomMarshalledReturn(int value)
		{
			return CustomMarshallingTests.CustomMarshalledClass.BindingsMarshaller.ConvertToManaged(CustomMarshallingTests.ParameterCustomMarshalledReturn_Injected(value));
		}

		[NativeThrows]
		[NativeMethod("ParameterCustomMarshalled")]
		public static void ParameterCustomMarshalled_Attribute([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller))] CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			CustomMarshallingTests.ParameterCustomMarshalled_Attribute_Injected(CustomMarshallingTests.CustomMarshaller.ConvertToUnmanaged(arg), expected);
		}

		[NativeMethod("ParameterCustomMarshalledIn")]
		[NativeThrows]
		public static void ParameterCustomMarshalledIn_Attribute([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller))] in CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			int num = CustomMarshallingTests.CustomMarshaller.ConvertToUnmanaged(arg);
			CustomMarshallingTests.ParameterCustomMarshalledIn_Attribute_Injected(in num, expected);
		}

		[NativeThrows]
		[NativeMethod("ParameterCustomMarshalledOut")]
		public static void ParameterCustomMarshalledOut_Attribute([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller))] out CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			CustomMarshallingTests.CustomMarshalledClass customMarshalledClass;
			int num = CustomMarshallingTests.CustomMarshaller.ConvertToUnmanaged(customMarshalledClass);
			CustomMarshallingTests.ParameterCustomMarshalledOut_Attribute_Injected(out num, expected);
			arg = CustomMarshallingTests.CustomMarshaller.ConvertToManaged(num);
		}

		[NativeMethod("ParameterCustomMarshalledRef")]
		[NativeThrows]
		public static void ParameterCustomMarshalledRef_Attribute([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller))] ref CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			int num = CustomMarshallingTests.CustomMarshaller.ConvertToUnmanaged(arg);
			CustomMarshallingTests.ParameterCustomMarshalledRef_Attribute_Injected(ref num, expected);
			arg = CustomMarshallingTests.CustomMarshaller.ConvertToManaged(num);
		}

		[NativeMethod("ParameterCustomMarshalledReturn")]
		[return: UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller))]
		public static CustomMarshallingTests.CustomMarshalledClass ParameterCustomMarshalledReturn_Attribute(int value)
		{
			return CustomMarshallingTests.CustomMarshaller.ConvertToManaged(CustomMarshallingTests.ParameterCustomMarshalledReturn_Attribute_Injected(value));
		}

		[NativeThrows]
		[NativeMethod("ParameterCustomMarshalled")]
		public static void ParameterCustomMarshalled_CustomMarshallerUsesInParameters([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshallerUsingInParameters))] CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			CustomMarshallingTests.ParameterCustomMarshalled_CustomMarshallerUsesInParameters_Injected(CustomMarshallingTests.CustomMarshallerUsingInParameters.ConvertToUnmanaged(in arg), expected);
		}

		[NativeThrows]
		[NativeMethod("ParameterCustomMarshalledIn")]
		public static void ParameterCustomMarshalledIn_CustomMarshallerUsesInParameters([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshallerUsingInParameters))] in CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			int num = CustomMarshallingTests.CustomMarshallerUsingInParameters.ConvertToUnmanaged(in arg);
			CustomMarshallingTests.ParameterCustomMarshalledIn_CustomMarshallerUsesInParameters_Injected(in num, expected);
		}

		[NativeMethod("ParameterCustomMarshalledOut")]
		[NativeThrows]
		public static void ParameterCustomMarshalledOut_CustomMarshallerUsesInParameters([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshallerUsingInParameters))] out CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			CustomMarshallingTests.CustomMarshalledClass customMarshalledClass;
			int num = CustomMarshallingTests.CustomMarshallerUsingInParameters.ConvertToUnmanaged(in customMarshalledClass);
			CustomMarshallingTests.ParameterCustomMarshalledOut_CustomMarshallerUsesInParameters_Injected(out num, expected);
			arg = CustomMarshallingTests.CustomMarshallerUsingInParameters.ConvertToManaged(in num);
		}

		[NativeThrows]
		[NativeMethod("ParameterCustomMarshalledRef")]
		public static void ParameterCustomMarshalledRef_CustomMarshallerUsesInParameters([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshallerUsingInParameters))] ref CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			int num = CustomMarshallingTests.CustomMarshallerUsingInParameters.ConvertToUnmanaged(in arg);
			CustomMarshallingTests.ParameterCustomMarshalledRef_CustomMarshallerUsesInParameters_Injected(ref num, expected);
			arg = CustomMarshallingTests.CustomMarshallerUsingInParameters.ConvertToManaged(in num);
		}

		[NativeMethod("ParameterCustomMarshalledReturn")]
		[return: UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshallerUsingInParameters))]
		public static CustomMarshallingTests.CustomMarshalledClass ParameterCustomMarshalledReturn_CustomMarshallerUsesInParameters(int value)
		{
			int num = CustomMarshallingTests.ParameterCustomMarshalledReturn_CustomMarshallerUsesInParameters_Injected(value);
			return CustomMarshallingTests.CustomMarshallerUsingInParameters.ConvertToManaged(in num);
		}

		[NativeMethod("ParameterCustomMarshalled")]
		[NativeThrows]
		public static void ParameterCustomMarshalled_Free([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller_WithFree))] CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			try
			{
				int num = CustomMarshallingTests.CustomMarshaller_WithFree.ConvertToUnmanaged(arg);
				CustomMarshallingTests.ParameterCustomMarshalled_Free_Injected(num, expected);
			}
			finally
			{
				int num;
				CustomMarshallingTests.CustomMarshaller_WithFree.Free(num);
			}
		}

		[NativeThrows]
		[NativeMethod("ParameterCustomMarshalledIn")]
		public static void ParameterCustomMarshalledIn_Free([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller_WithFree))] in CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			try
			{
				int num = CustomMarshallingTests.CustomMarshaller_WithFree.ConvertToUnmanaged(arg);
				CustomMarshallingTests.ParameterCustomMarshalledIn_Free_Injected(in num, expected);
			}
			finally
			{
				int num;
				CustomMarshallingTests.CustomMarshaller_WithFree.Free(num);
			}
		}

		[NativeThrows]
		[NativeMethod("ParameterCustomMarshalledOut")]
		public static void ParameterCustomMarshalledOut_Free([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller_WithFree))] out CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			try
			{
				CustomMarshallingTests.CustomMarshalledClass customMarshalledClass;
				int num = CustomMarshallingTests.CustomMarshaller_WithFree.ConvertToUnmanaged(customMarshalledClass);
				CustomMarshallingTests.ParameterCustomMarshalledOut_Free_Injected(out num, expected);
			}
			finally
			{
				int num;
				arg = CustomMarshallingTests.CustomMarshaller_WithFree.ConvertToManaged(num);
				CustomMarshallingTests.CustomMarshaller_WithFree.Free(num);
			}
		}

		[NativeThrows]
		[NativeMethod("ParameterCustomMarshalledRef")]
		public static void ParameterCustomMarshalledRef_Free([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller_WithFree))] ref CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			try
			{
				int num = CustomMarshallingTests.CustomMarshaller_WithFree.ConvertToUnmanaged(arg);
				CustomMarshallingTests.ParameterCustomMarshalledRef_Free_Injected(ref num, expected);
			}
			finally
			{
				int num;
				arg = CustomMarshallingTests.CustomMarshaller_WithFree.ConvertToManaged(num);
				CustomMarshallingTests.CustomMarshaller_WithFree.Free(num);
			}
		}

		[NativeMethod("ParameterCustomMarshalledReturn")]
		[return: UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller_WithFree))]
		public static CustomMarshallingTests.CustomMarshalledClass ParameterCustomMarshalledReturn_Free(int value)
		{
			CustomMarshallingTests.CustomMarshalledClass customMarshalledClass2;
			try
			{
				int num = CustomMarshallingTests.ParameterCustomMarshalledReturn_Free_Injected(value);
			}
			finally
			{
				int num;
				int num2 = num;
				int num3 = num2;
				CustomMarshallingTests.CustomMarshalledClass customMarshalledClass = CustomMarshallingTests.CustomMarshaller_WithFree.ConvertToManaged(num2);
				CustomMarshallingTests.CustomMarshaller_WithFree.Free(num3);
				customMarshalledClass2 = customMarshalledClass;
			}
			return customMarshalledClass2;
		}

		[NativeThrows]
		public unsafe static void ParameterCustomMarshalled_NeedingMarshalling([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller_NeeedingMarshalling))] CustomMarshallingTests.CustomMarshalledClass arg, string expected)
		{
			try
			{
				string text = CustomMarshallingTests.CustomMarshaller_NeeedingMarshalling.ConvertToUnmanaged(arg);
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(text, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = text.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(expected, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = expected.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				CustomMarshallingTests.ParameterCustomMarshalled_NeedingMarshalling_Injected(ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		[NativeThrows]
		public unsafe static void ParameterCustomMarshalled_NeedingMarshalling_In([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller_NeeedingMarshalling))] in CustomMarshallingTests.CustomMarshalledClass arg, string expected)
		{
			try
			{
				string text = CustomMarshallingTests.CustomMarshaller_NeeedingMarshalling.ConvertToUnmanaged(arg);
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(text, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = text.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(expected, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = expected.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				CustomMarshallingTests.ParameterCustomMarshalled_NeedingMarshalling_In_Injected(in managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		[NativeThrows]
		public unsafe static void ParameterCustomMarshalled_NeedingMarshalling_Out([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller_NeeedingMarshalling))] out CustomMarshallingTests.CustomMarshalledClass arg, string expected)
		{
			try
			{
				CustomMarshallingTests.CustomMarshalledClass customMarshalledClass;
				string text = CustomMarshallingTests.CustomMarshaller_NeeedingMarshalling.ConvertToUnmanaged(customMarshalledClass);
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(text, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = text.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(expected, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = expected.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				CustomMarshallingTests.ParameterCustomMarshalled_NeedingMarshalling_Out_Injected(out managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				arg = CustomMarshallingTests.CustomMarshaller_NeeedingMarshalling.ConvertToManaged(OutStringMarshaller.GetStringAndDispose(managedSpanWrapper));
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		[NativeThrows]
		public unsafe static void ParameterCustomMarshalled_NeedingMarshalling_Ref([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller_NeeedingMarshalling))] ref CustomMarshallingTests.CustomMarshalledClass arg, string expected)
		{
			try
			{
				string text = CustomMarshallingTests.CustomMarshaller_NeeedingMarshalling.ConvertToUnmanaged(arg);
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(text, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = text.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(expected, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = expected.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				CustomMarshallingTests.ParameterCustomMarshalled_NeedingMarshalling_Ref_Injected(ref managedSpanWrapper, ref managedSpanWrapper2);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				arg = CustomMarshallingTests.CustomMarshaller_NeeedingMarshalling.ConvertToManaged(OutStringMarshaller.GetStringAndDispose(managedSpanWrapper));
				char* ptr = null;
				char* ptr2 = null;
			}
		}

		[return: UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshaller_NeeedingMarshalling))]
		public unsafe static CustomMarshallingTests.CustomMarshalledClass ParameterCustomMarshalled_NeedingMarshalling_Return(string value)
		{
			CustomMarshallingTests.CustomMarshalledClass customMarshalledClass3;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = value.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				CustomMarshallingTests.CustomMarshalledClass customMarshalledClass;
				string text = CustomMarshallingTests.CustomMarshaller_NeeedingMarshalling.ConvertToUnmanaged(customMarshalledClass);
				ManagedSpanWrapper managedSpanWrapper2;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(text, ref managedSpanWrapper2))
				{
					ReadOnlySpan<char> readOnlySpan2 = text.AsSpan();
					fixed (char* ptr2 = readOnlySpan2.GetPinnableReference())
					{
						managedSpanWrapper2 = new ManagedSpanWrapper((void*)ptr2, readOnlySpan2.Length);
					}
				}
				CustomMarshallingTests.ParameterCustomMarshalled_NeedingMarshalling_Return_Injected(ref managedSpanWrapper, out managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				ManagedSpanWrapper managedSpanWrapper2;
				CustomMarshallingTests.CustomMarshalledClass customMarshalledClass2 = CustomMarshallingTests.CustomMarshaller_NeeedingMarshalling.ConvertToManaged(OutStringMarshaller.GetStringAndDispose(managedSpanWrapper2));
				char* ptr2 = null;
				customMarshalledClass3 = customMarshalledClass2;
			}
			return customMarshalledClass3;
		}

		[NativeThrows]
		[NativeMethod("ParameterCustomMarshalled")]
		public static void ParameterCustomMarshalled_GenericMarshaller([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshallerGeneric<CustomMarshallingTests.CustomMarshalledClass>))] CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			CustomMarshallingTests.ParameterCustomMarshalled_GenericMarshaller_Injected(CustomMarshallingTests.CustomMarshallerGeneric<CustomMarshallingTests.CustomMarshalledClass>.ConvertToUnmanaged(arg), expected);
		}

		[NativeThrows]
		[NativeMethod("ParameterCustomMarshalledIn")]
		public static void ParameterCustomMarshalledIn_GenericMarshaller([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshallerGeneric<CustomMarshallingTests.CustomMarshalledClass>))] in CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			int num = CustomMarshallingTests.CustomMarshallerGeneric<CustomMarshallingTests.CustomMarshalledClass>.ConvertToUnmanaged(arg);
			CustomMarshallingTests.ParameterCustomMarshalledIn_GenericMarshaller_Injected(in num, expected);
		}

		[NativeThrows]
		[NativeMethod("ParameterCustomMarshalledOut")]
		public static void ParameterCustomMarshalledOut_GenericMarshaller([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshallerGeneric<CustomMarshallingTests.CustomMarshalledClass>))] out CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			CustomMarshallingTests.CustomMarshalledClass customMarshalledClass;
			int num = CustomMarshallingTests.CustomMarshallerGeneric<CustomMarshallingTests.CustomMarshalledClass>.ConvertToUnmanaged(customMarshalledClass);
			CustomMarshallingTests.ParameterCustomMarshalledOut_GenericMarshaller_Injected(out num, expected);
			arg = CustomMarshallingTests.CustomMarshallerGeneric<CustomMarshallingTests.CustomMarshalledClass>.ConvertToManaged(num);
		}

		[NativeThrows]
		[NativeMethod("ParameterCustomMarshalledRef")]
		public static void ParameterCustomMarshalledRef_GenericMarshaller([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshallerGeneric<CustomMarshallingTests.CustomMarshalledClass>))] ref CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			int num = CustomMarshallingTests.CustomMarshallerGeneric<CustomMarshallingTests.CustomMarshalledClass>.ConvertToUnmanaged(arg);
			CustomMarshallingTests.ParameterCustomMarshalledRef_GenericMarshaller_Injected(ref num, expected);
			arg = CustomMarshallingTests.CustomMarshallerGeneric<CustomMarshallingTests.CustomMarshalledClass>.ConvertToManaged(num);
		}

		[NativeMethod("ParameterCustomMarshalledReturn")]
		[return: UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshallerGeneric<CustomMarshallingTests.CustomMarshalledClass>))]
		public static CustomMarshallingTests.CustomMarshalledClass ParameterCustomMarshalledReturn_GenericMarshaller(int value)
		{
			return CustomMarshallingTests.CustomMarshallerGeneric<CustomMarshallingTests.CustomMarshalledClass>.ConvertToManaged(CustomMarshallingTests.ParameterCustomMarshalledReturn_GenericMarshaller_Injected(value));
		}

		[NativeMethod("ParameterCustomMarshalled")]
		[NativeThrows]
		public static void ParameterCustomMarshalled_DerivedType([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshallerGeneric<CustomMarshallingTests.CustomMarshalledClass>))] CustomMarshallingTests.CustomMarshalledDerivedClass arg, int expected)
		{
			CustomMarshallingTests.ParameterCustomMarshalled_DerivedType_Injected(CustomMarshallingTests.CustomMarshallerGeneric<CustomMarshallingTests.CustomMarshalledClass>.ConvertToUnmanaged(arg), expected);
		}

		[NativeThrows]
		[NativeMethod("ParameterCustomMarshalled")]
		public static void ParameterCustomMarshalled_InterfaceMarshaller([UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshallerInterface))] CustomMarshallingTests.CustomMarshalledClass arg, int expected)
		{
			CustomMarshallingTests.ParameterCustomMarshalled_InterfaceMarshaller_Injected(CustomMarshallingTests.CustomMarshallerInterface.ConvertToUnmanaged(arg), expected);
		}

		[NativeThrows]
		[NativeMethod("BlittableStructTests::ParameterStructInt", true)]
		public static void ParameterCustomMarshalled_AsStruct(CustomMarshallingTests.CustomMarshalledAsStruct arg)
		{
			StructInt structInt = CustomMarshallingTests.CustomMarshalledAsStruct.BindingsMarshaller.ConvertToUnmanaged(in arg);
			CustomMarshallingTests.ParameterCustomMarshalled_AsStruct_Injected(ref structInt);
		}

		[NativeThrows]
		[NativeMethod("BlittableStructTests::ParameterStructIntIn", true)]
		public static void ParameterCustomMarshalled_AsStruct_In(in CustomMarshallingTests.CustomMarshalledAsStruct arg)
		{
			StructInt structInt = CustomMarshallingTests.CustomMarshalledAsStruct.BindingsMarshaller.ConvertToUnmanaged(in arg);
			CustomMarshallingTests.ParameterCustomMarshalled_AsStruct_In_Injected(in structInt);
		}

		[NativeThrows]
		[NativeMethod("BlittableStructTests::ParameterStructIntOut", true)]
		public static void ParameterCustomMarshalled_AsStruct_Out(out CustomMarshallingTests.CustomMarshalledAsStruct arg)
		{
			CustomMarshallingTests.CustomMarshalledAsStruct customMarshalledAsStruct;
			StructInt structInt = CustomMarshallingTests.CustomMarshalledAsStruct.BindingsMarshaller.ConvertToUnmanaged(in customMarshalledAsStruct);
			CustomMarshallingTests.ParameterCustomMarshalled_AsStruct_Out_Injected(out structInt);
			arg = CustomMarshallingTests.CustomMarshalledAsStruct.BindingsMarshaller.ConvertToManaged(in structInt);
		}

		[NativeMethod("BlittableStructTests::ParameterStructIntByRef", true)]
		[NativeThrows]
		public static void ParameterCustomMarshalled_AsStruct_Ref(ref CustomMarshallingTests.CustomMarshalledAsStruct arg)
		{
			StructInt structInt = CustomMarshallingTests.CustomMarshalledAsStruct.BindingsMarshaller.ConvertToUnmanaged(in arg);
			CustomMarshallingTests.ParameterCustomMarshalled_AsStruct_Ref_Injected(ref structInt);
			arg = CustomMarshallingTests.CustomMarshalledAsStruct.BindingsMarshaller.ConvertToManaged(in structInt);
		}

		[NativeMethod("BlittableStructTests::ReturnStructInt", true)]
		public static CustomMarshallingTests.CustomMarshalledAsStruct ParameterCustomMarshalled_AsStruct_Return()
		{
			CustomMarshallingTests.CustomMarshalledAsStruct customMarshalledAsStruct;
			StructInt structInt = CustomMarshallingTests.CustomMarshalledAsStruct.BindingsMarshaller.ConvertToUnmanaged(in customMarshalledAsStruct);
			CustomMarshallingTests.ParameterCustomMarshalled_AsStruct_Return_Injected(out structInt);
			return CustomMarshallingTests.CustomMarshalledAsStruct.BindingsMarshaller.ConvertToManaged(in structInt);
		}

		[NativeMethod("BlittableStructTests::ParameterStructIntByRef", IsFreeFunction = true, ThrowsException = true)]
		public unsafe static void PassClassWithPinnableInnerData_PinnedRef(CustomMarshallingTests.ClassWithPinnableInnerData c)
		{
			fixed (StructInt* pinnableReference = CustomMarshallingTests.ClassWithPinnableInnerData.GetPinnableReference(c))
			{
				CustomMarshallingTests.PassClassWithPinnableInnerData_PinnedRef_Injected(ref *pinnableReference);
			}
		}

		[NativeMethod("BlittableStructTests::ParameterStructIntVector", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void PassClassWithPinnableInnerData_AsArray(CustomMarshallingTests.ClassWithPinnableInnerData[] arr);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_Injected(int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledIn_Injected(in int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledOut_Injected(out int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledRef_Injected(ref int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ParameterCustomMarshalledReturn_Injected(int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_Attribute_Injected(int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledIn_Attribute_Injected(in int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledOut_Attribute_Injected(out int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledRef_Attribute_Injected(ref int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ParameterCustomMarshalledReturn_Attribute_Injected(int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_CustomMarshallerUsesInParameters_Injected(int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledIn_CustomMarshallerUsesInParameters_Injected(in int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledOut_CustomMarshallerUsesInParameters_Injected(out int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledRef_CustomMarshallerUsesInParameters_Injected(ref int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ParameterCustomMarshalledReturn_CustomMarshallerUsesInParameters_Injected(int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_Free_Injected(int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledIn_Free_Injected(in int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledOut_Free_Injected(out int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledRef_Free_Injected(ref int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ParameterCustomMarshalledReturn_Free_Injected(int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_NeedingMarshalling_Injected(ref ManagedSpanWrapper arg, ref ManagedSpanWrapper expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_NeedingMarshalling_In_Injected(in ManagedSpanWrapper arg, ref ManagedSpanWrapper expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_NeedingMarshalling_Out_Injected(out ManagedSpanWrapper arg, ref ManagedSpanWrapper expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_NeedingMarshalling_Ref_Injected(ref ManagedSpanWrapper arg, ref ManagedSpanWrapper expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_NeedingMarshalling_Return_Injected(ref ManagedSpanWrapper value, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_GenericMarshaller_Injected(int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledIn_GenericMarshaller_Injected(in int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledOut_GenericMarshaller_Injected(out int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalledRef_GenericMarshaller_Injected(ref int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int ParameterCustomMarshalledReturn_GenericMarshaller_Injected(int value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_DerivedType_Injected(int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_InterfaceMarshaller_Injected(int arg, int expected);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_AsStruct_Injected(ref StructInt arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_AsStruct_In_Injected(in StructInt arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_AsStruct_Out_Injected(out StructInt arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_AsStruct_Ref_Injected(ref StructInt arg);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterCustomMarshalled_AsStruct_Return_Injected(out StructInt ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void PassClassWithPinnableInnerData_PinnedRef_Injected(ref StructInt c);

		[UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshalledClass.BindingsMarshaller))]
		public class CustomMarshalledClass : CustomMarshallingTests.ICustomMarshalled
		{
			public string Value { get; set; }

			public static class BindingsMarshaller
			{
				public static int ConvertToUnmanaged(CustomMarshallingTests.CustomMarshalledClass c)
				{
					return (c == null) ? 0 : int.Parse(c.Value);
				}

				public static CustomMarshallingTests.CustomMarshalledClass ConvertToManaged(int n)
				{
					return new CustomMarshallingTests.CustomMarshalledClass
					{
						Value = n.ToString()
					};
				}
			}
		}

		public class CustomMarshalledDerivedClass : CustomMarshallingTests.CustomMarshalledClass
		{
		}

		public interface ICustomMarshalled
		{
			string Value { get; set; }
		}

		public class CustomMarshaller
		{
			public static int ConvertToUnmanaged(CustomMarshallingTests.CustomMarshalledClass c)
			{
				return (c == null) ? 0 : (int.Parse(c.Value) * 2);
			}

			public static CustomMarshallingTests.CustomMarshalledClass ConvertToManaged(int n)
			{
				return new CustomMarshallingTests.CustomMarshalledClass
				{
					Value = (n * 2).ToString()
				};
			}
		}

		public class CustomMarshallerUsingInParameters
		{
			public static int ConvertToUnmanaged(in CustomMarshallingTests.CustomMarshalledClass c)
			{
				return (c == null) ? 0 : (int.Parse(c.Value) * 2);
			}

			public static CustomMarshallingTests.CustomMarshalledClass ConvertToManaged(in int n)
			{
				return new CustomMarshallingTests.CustomMarshalledClass
				{
					Value = (n * 2).ToString()
				};
			}
		}

		public class CustomMarshaller_NeeedingMarshalling
		{
			public static string ConvertToUnmanaged(CustomMarshallingTests.CustomMarshalledClass c)
			{
				return (c == null) ? null : (c.Value + "_ConvertedToUnmanaged");
			}

			public static CustomMarshallingTests.CustomMarshalledClass ConvertToManaged(string s)
			{
				return new CustomMarshallingTests.CustomMarshalledClass
				{
					Value = s + "_ConvertedToManaged"
				};
			}
		}

		public class CustomMarshaller_WithFree
		{
			public static int GetLastFreeValue()
			{
				return CustomMarshallingTests.CustomMarshaller_WithFree._lastFreeValue;
			}

			public static int ConvertToUnmanaged(CustomMarshallingTests.CustomMarshalledClass c)
			{
				return (c == null) ? 0 : (int.Parse(c.Value) * 3);
			}

			public static CustomMarshallingTests.CustomMarshalledClass ConvertToManaged(int n)
			{
				return new CustomMarshallingTests.CustomMarshalledClass
				{
					Value = (n * 3).ToString()
				};
			}

			public static void Free(int value)
			{
				CustomMarshallingTests.CustomMarshaller_WithFree._lastFreeValue = value;
			}

			private static int _lastFreeValue = int.MinValue;
		}

		public class CustomMarshallerGeneric<T> where T : CustomMarshallingTests.ICustomMarshalled, new()
		{
			public static int ConvertToUnmanaged(T c)
			{
				return (c == null) ? 0 : (int.Parse(c.Value) * 2);
			}

			public static T ConvertToManaged(int n)
			{
				T t = new T();
				t.Value = (n * 2).ToString();
				return t;
			}
		}

		public class CustomMarshallerInterface
		{
			public static int ConvertToUnmanaged(CustomMarshallingTests.ICustomMarshalled c)
			{
				return (c == null) ? 0 : (int.Parse(c.Value) * 2);
			}
		}

		[UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.CustomMarshalledAsStruct.BindingsMarshaller))]
		public struct CustomMarshalledAsStruct
		{
			public int field;

			public static class BindingsMarshaller
			{
				public static StructInt ConvertToUnmanaged(in CustomMarshallingTests.CustomMarshalledAsStruct s)
				{
					return new StructInt
					{
						field = s.field
					};
				}

				public static CustomMarshallingTests.CustomMarshalledAsStruct ConvertToManaged(in StructInt s)
				{
					return new CustomMarshallingTests.CustomMarshalledAsStruct
					{
						field = s.field
					};
				}
			}
		}

		public class MarshalThisAsStructInt
		{
			[UnityMarshalThisAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.MarshalThisAsStructInt.BindingsMarshaller))]
			public int GetField()
			{
				StructInt structInt = CustomMarshallingTests.MarshalThisAsStructInt.BindingsMarshaller.ConvertToUnmanaged(this);
				return CustomMarshallingTests.MarshalThisAsStructInt.GetField_Injected(ref structInt);
			}

			[MethodImpl(MethodImplOptions.InternalCall)]
			private static extern int GetField_Injected(ref StructInt _unity_self);

			public int field;

			private static class BindingsMarshaller
			{
				public static StructInt ConvertToUnmanaged(CustomMarshallingTests.MarshalThisAsStructInt s)
				{
					return new StructInt
					{
						field = s.field
					};
				}
			}
		}

		[UnityMarshalAs(NativeType.Custom, CustomMarshaller = typeof(CustomMarshallingTests.ClassWithPinnableInnerData))]
		public class ClassWithPinnableInnerData
		{
			internal static ref StructInt GetPinnableReference(CustomMarshallingTests.ClassWithPinnableInnerData c)
			{
				return ref c.NativeData;
			}

			internal static StructInt ConvertToUnmanaged(CustomMarshallingTests.ClassWithPinnableInnerData data)
			{
				return data.NativeData;
			}

			public StructInt NativeData;
		}
	}
}
