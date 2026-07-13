using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AOT;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	internal readonly struct ArrayHandleOnStack<[IsUnmanaged] TT> where TT : struct, ValueType
	{
		public unsafe ArrayHandleOnStack(void* arrayRefPtr)
		{
			this._arrayRefPtr = arrayRefPtr;
			this._allocArrayCallbackPtr = ArrayHandleOnStack<TT>.s_createArrayFcnPtr;
		}

		public unsafe ArrayHandleOnStack(void* arrayRefPtr, IntPtr allocArrayCallbackPtr)
		{
			this._arrayRefPtr = arrayRefPtr;
			this._allocArrayCallbackPtr = allocArrayCallbackPtr;
		}

		[MonoPInvokeCallback(typeof(ArrayHandleOnStack.CreateArrayDelegate))]
		public unsafe static void* AllocArrayManagedCallback(void* targetRef, int size)
		{
			TT[] array = new TT[size];
			*UnsafeUtility.ClassAsRef<TT[]>(targetRef) = array;
			bool flag = size < 1;
			void* ptr;
			if (flag)
			{
				ptr = null;
			}
			else
			{
				TT[] array2;
				void* ptr2;
				if ((array2 = array) == null || array2.Length == 0)
				{
					ptr2 = null;
				}
				else
				{
					ptr2 = (void*)(&array2[0]);
				}
				ptr = ptr2;
			}
			return ptr;
		}

		private unsafe readonly void* _arrayRefPtr;

		private readonly IntPtr _allocArrayCallbackPtr;

		private static ArrayHandleOnStack.CreateArrayDelegate s_createArrayDelegate = new ArrayHandleOnStack.CreateArrayDelegate(ArrayHandleOnStack<TT>.AllocArrayManagedCallback);

		private static IntPtr s_createArrayFcnPtr = Marshal.GetFunctionPointerForDelegate<ArrayHandleOnStack.CreateArrayDelegate>(ArrayHandleOnStack<TT>.s_createArrayDelegate);
	}
}
