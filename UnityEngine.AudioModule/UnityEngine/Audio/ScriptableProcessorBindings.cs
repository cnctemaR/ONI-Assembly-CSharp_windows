using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Audio;
using UnityEngine.Bindings;

namespace UnityEngine.Audio
{
	[NativeHeader("Modules/Audio/Public/ScriptableProcessors/ScriptBindings/ScriptableProcessor.bindings.h")]
	internal static class ScriptableProcessorBindings
	{
		public unsafe static void QueueProcessorDispose(ProcessorHeader* header, ControlHeader* control)
		{
			ScriptableProcessorBindings.QueueProcessorDisposeInternal((void*)header, (void*)control);
		}

		public unsafe static bool AddDataToProcessorHandle(ControlHeader* control, in Handle handle, void* data, int size, int align, long typeHash)
		{
			return ScriptableProcessorBindings.AddDataToProcessorHandleInternal((void*)control, in handle, data, size, align, typeHash);
		}

		public unsafe static ProcessorInstance.AvailableData.Element* GetAvailableDataForRealtime(in RealtimeAccess access, in Handle handle)
		{
			fixed (RealtimeAccess* ptr = &access)
			{
				RealtimeAccess* ptr2 = ptr;
				return (ProcessorInstance.AvailableData.Element*)ScriptableProcessorBindings.GetRealtimeDataElementListForProcessorInternal((void*)ptr2, in handle);
			}
		}

		public unsafe static ProcessorInstance.AvailableData.Element* GetAvailableDataForControl(ControlHeader* control, in Handle handle)
		{
			return (ProcessorInstance.AvailableData.Element*)ScriptableProcessorBindings.GetControlDataElementListForProcessorInternal((void*)control, in handle);
		}

		public unsafe static void ReturnDataFromProcessor(in RealtimeAccess access, in Handle handle, void* data, int size, int align, long typeHash)
		{
			fixed (RealtimeAccess* ptr = &access)
			{
				RealtimeAccess* ptr2 = ptr;
				ScriptableProcessorBindings.ReturnDataFromProcessorInternal((void*)ptr2, in handle, data, size, align, typeHash);
			}
		}

		public unsafe static void ValidateCanProcess(in Handle handle, in RealtimeContext ctx)
		{
			fixed (RealtimeContext* ptr = &ctx)
			{
				RealtimeContext* ptr2 = ptr;
				ScriptableProcessorBindings.ValidateCanProcessInternal(in handle, (void*)ptr2);
			}
		}

		public unsafe static bool CheckProcessorExists(Handle handle, ControlHeader* control)
		{
			return ScriptableProcessorBindings.CheckProcessorExistsInternal(handle, (void*)control);
		}

		public unsafe static void PerformRecursiveConfigure(Handle handle, ControlHeader* control, in AudioConfiguration configuration)
		{
			ScriptableProcessorBindings.PerformRecursiveConfigureInternal(handle, (void*)control, in configuration);
		}

		public unsafe static void PerformRecursiveUpdate(Handle handle, ControlHeader* control)
		{
			ScriptableProcessorBindings.PerformRecursiveUpdateInternal(handle, (void*)control);
		}

		public unsafe static ProcessorInstance.Response SendMessageToProcessor(ProcessorHeader* header, ControlHeader* control, ProcessorInstance.Message* message)
		{
			return ScriptableProcessorBindings.SendMessageToProcessorInternal((void*)header, (void*)control, (void*)message);
		}

		[NativeMethod(Name = "audio::SendMessageToProcessor", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern ProcessorInstance.Response SendMessageToProcessorInternal(void* header, void* control, void* message);

		[NativeMethod(Name = "audio::PerformRecursiveUpdate", IsFreeFunction = true, ThrowsException = true)]
		private unsafe static void PerformRecursiveUpdateInternal(Handle handle, void* control)
		{
			ScriptableProcessorBindings.PerformRecursiveUpdateInternal_Injected(ref handle, control);
		}

		[NativeMethod(Name = "audio::PerformRecursiveConfigure", IsFreeFunction = true, ThrowsException = true)]
		private unsafe static void PerformRecursiveConfigureInternal(Handle handle, void* control, in AudioConfiguration configuration)
		{
			ScriptableProcessorBindings.PerformRecursiveConfigureInternal_Injected(ref handle, control, in configuration);
		}

		[NativeMethod(Name = "audio::ValidateCanProcess", IsFreeFunction = true, IsThreadSafe = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ValidateCanProcessInternal(in Handle handle, void* processingContext);

		[NativeMethod(Name = "audio::QueueProcessorDispose", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void QueueProcessorDisposeInternal(void* header, void* control);

		[NativeMethod(Name = "audio::GetRealtimeDataElementListForProcessor", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* GetRealtimeDataElementListForProcessorInternal(void* access, in Handle handle);

		[NativeMethod(Name = "audio::GetControlDataElementListForProcessor", IsFreeFunction = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void* GetControlDataElementListForProcessorInternal(void* control, in Handle handle);

		[NativeMethod(Name = "audio::ReturnDataFromProcessor", IsFreeFunction = true, IsThreadSafe = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ReturnDataFromProcessorInternal(void* access, in Handle handle, void* data, int size, int align, long typeHash);

		[NativeMethod(Name = "audio::AddDataToProcessor", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool AddDataToProcessorHandleInternal(void* control, in Handle handle, void* data, int size, int align, long typeHash);

		[NativeMethod(Name = "audio::CheckProcessorExists", IsFreeFunction = true)]
		private unsafe static bool CheckProcessorExistsInternal(Handle handle, void* control)
		{
			return ScriptableProcessorBindings.CheckProcessorExistsInternal_Injected(ref handle, control);
		}

		[NativeMethod(Name = "audio::ThrowScriptingExceptionForTest", IsFreeFunction = true, IsThreadSafe = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void ThrowScriptingExceptionForTest();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void PerformRecursiveUpdateInternal_Injected([In] ref Handle handle, void* control);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void PerformRecursiveConfigureInternal_Injected([In] ref Handle handle, void* control, in AudioConfiguration configuration);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern bool CheckProcessorExistsInternal_Injected([In] ref Handle handle, void* control);
	}
}
