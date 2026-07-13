using System;
using Unity.Audio;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Audio
{
	[NativeHeader("Modules/Audio/Public/ScriptableProcessors/ScriptableProcessor.h")]
	[RequiredByNativeCode]
	internal struct ProcessorHeader
	{
		public unsafe void InvokeProcessor(ProcessorFunction fn, void* args)
		{
			fixed (ProcessorHeader* ptr = &this)
			{
				ProcessorHeader* ptr2 = ptr;
				if (fn - ProcessorFunction.Update > 4U)
				{
					delegate* unmanaged[Cdecl]<ProcessorHeader*, ProcessorFunction, void*, void> nativeProcessorFunction = this.NativeProcessorFunction;
					calli(System.Void(UnityEngine.Audio.ProcessorHeader*,UnityEngine.Audio.ProcessorFunction,System.Void*), ptr2, fn, args, nativeProcessorFunction);
					ptr = null;
					return;
				}
				throw new NotSupportedException(string.Format("Cannot manually invoke {0}, these are called automatically", fn));
			}
		}

		public unsafe bool IsSameControl(ControlHeader* other)
		{
			return this.m_Control == (void*)other;
		}

		private unsafe void* m_Control;

		internal Handle DualThreadHandle;

		internal unsafe delegate* unmanaged[Cdecl]<ProcessorHeader*, ProcessorFunction, void*, void> NativeProcessorFunction;

		internal unsafe delegate* unmanaged[Cdecl]<ProcessorHeader*, ControlFunction, void*, void> NativeControlFunction;

		internal IntPtr ProcessorReflectionData;

		internal IntPtr ControlReflectionData;
	}
}
