using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.Audio
{
	internal static class ProcessorExtensions
	{
		internal unsafe static T* CAllocChunk<[global::System.Runtime.CompilerServices.IsUnmanaged] T>() where T : struct, ValueType
		{
			T* ptr = (T*)UnsafeUtility.MallocTracked((long)sizeof(T), UnsafeUtility.AlignOf<T>(), Allocator.Persistent, 3);
			*ptr = default(T);
			return ptr;
		}

		public unsafe static void DispatchGenericControl<[global::System.Runtime.CompilerServices.IsUnmanaged] TControl, [global::System.Runtime.CompilerServices.IsUnmanaged] TRealtime>(ref TControl control, ref TRealtime realtime, in ProcessorHeader header, void* additionalPtr, ControlFunction function) where TControl : struct, ValueType, ProcessorInstance.IControl<TRealtime> where TRealtime : struct, ValueType, ProcessorInstance.IRealtime
		{
			switch (function)
			{
			case ControlFunction.Dispose:
				control.Dispose(new ControlContext((void*)((DisposeArguments*)additionalPtr)->ControlContext), ref realtime);
				fixed (ProcessorHeader* ptr = &header)
				{
					ProcessorHeader* ptr2 = ptr;
					UnsafeUtility.FreeTracked((void*)ptr2, Allocator.Persistent);
				}
				return;
			case ControlFunction.Update:
				control.Update(new ControlContext((void*)((UpdateArguments*)additionalPtr)->ControlContext), new ProcessorInstance.Pipe(((UpdateArguments*)additionalPtr)->Self, null));
				return;
			case ControlFunction.Message:
				((MessageArguments*)additionalPtr)->StatusReturn = control.OnMessage(new ControlContext((void*)((MessageArguments*)additionalPtr)->Context), new ProcessorInstance.Pipe(((MessageArguments*)additionalPtr)->Self, null), *((MessageArguments*)additionalPtr)->MessageData);
				return;
			}
			throw new ArgumentOutOfRangeException();
		}

		public unsafe static void DispatchGenericProcessor<[global::System.Runtime.CompilerServices.IsUnmanaged] T>(ref T processor, in ProcessorHeader header, void* additionalPtr, ProcessorFunction function) where T : struct, ValueType, ProcessorInstance.IRealtime
		{
			if (function != ProcessorFunction.Update)
			{
				throw new ArgumentOutOfRangeException();
			}
			ProcessorRealtimeUpdateArguments* ptr = *(IntPtr*)additionalPtr;
			processor.Update(new ProcessorInstance.UpdatedDataContext(in ptr->Access), new ProcessorInstance.Pipe(ptr->Self, ptr->Head));
		}
	}
}
