using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;

namespace UnityEngine
{
	[NativeHeader("Modules/Marshalling/MarshallingTests.h")]
	[ExcludeFromDocs]
	internal class EnumTests
	{
		[NativeThrows]
		public unsafe static void ParameterVectorEnum(SomeEnum[] enumArray)
		{
			Span<SomeEnum> span = new Span<SomeEnum>(enumArray);
			fixed (SomeEnum* pinnableReference = span.GetPinnableReference())
			{
				ManagedSpanWrapper managedSpanWrapper = new ManagedSpanWrapper((void*)pinnableReference, span.Length);
				EnumTests.ParameterVectorEnum_Injected(ref managedSpanWrapper);
			}
		}

		public unsafe static void ParameterOutVectorEnum([Out] SomeEnum[] enumArray)
		{
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				if (enumArray != null)
				{
					fixed (SomeEnum[] array = enumArray)
					{
						if (array.Length != 0)
						{
							blittableArrayWrapper = new BlittableArrayWrapper((void*)(&array[0]), array.Length);
						}
					}
				}
				EnumTests.ParameterOutVectorEnum_Injected(out blittableArrayWrapper);
			}
			finally
			{
				SomeEnum[] array;
				BlittableArrayWrapper blittableArrayWrapper;
				blittableArrayWrapper.Unmarshal<SomeEnum>(ref array);
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterVectorEnum_Injected(ref ManagedSpanWrapper enumArray);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ParameterOutVectorEnum_Injected(out BlittableArrayWrapper enumArray);
	}
}
