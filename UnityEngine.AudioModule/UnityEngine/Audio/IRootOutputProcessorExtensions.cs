using System;
using System.Runtime.CompilerServices;
using Unity.Audio;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine.Audio
{
	[NativeHeader("Modules/Audio/Public/ScriptableProcessors/ScriptBindings/ScriptableProcessor.bindings.h")]
	internal static class IRootOutputProcessorExtensions
	{
		internal unsafe static IntPtr GetReflectionData<[global::System.Runtime.CompilerServices.IsUnmanaged] T>() where T : struct, ValueType, RootOutputInstance.IRealtime
		{
			IRootOutputProcessorExtensions.JobStruct<T>.Initialize();
			return *IRootOutputProcessorExtensions.JobStruct<T>.jobReflectionData.Data;
		}

		internal unsafe static void InitializeRootOutputHandle(ProcessorHeader* header, ControlHeader* control, ProcessorInstance.InitializationFlags flags)
		{
			IRootOutputProcessorExtensions.InternalInitializeRootOutputHandle((void*)header, (void*)control, flags);
		}

		[NativeMethod(Name = "audio::InitializeRootOutputHandle", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void InternalInitializeRootOutputHandle(void* header, void* control, ProcessorInstance.InitializationFlags flags);

		internal struct ProcessPhaseUpdateArguments
		{
			internal unsafe RealtimeContext* Context;

			internal JobHandle InOut;

			internal Handle Self;

			internal unsafe float* AudioBuffer;

			internal int OutputFrameCount;

			internal int OutputChannelCount;
		}

		internal struct JobStruct<[global::System.Runtime.CompilerServices.IsUnmanaged] TUserProcessor> where TUserProcessor : struct, ValueType, RootOutputInstance.IRealtime
		{
			[BurstDiscard]
			internal unsafe static void Initialize()
			{
				bool flag = *IRootOutputProcessorExtensions.JobStruct<TUserProcessor>.jobReflectionData.Data == IntPtr.Zero;
				if (flag)
				{
					*IRootOutputProcessorExtensions.JobStruct<TUserProcessor>.jobReflectionData.Data = JobsUtility.CreateJobReflectionData(typeof(IRootOutputProcessorExtensions.JobStruct<TUserProcessor>.Storage), new IRootOutputProcessorExtensions.JobStruct<TUserProcessor>.ExecuteJobFunction(IRootOutputProcessorExtensions.JobStruct<TUserProcessor>.Execute), null, null);
				}
			}

			public unsafe static void Execute(ref IRootOutputProcessorExtensions.JobStruct<TUserProcessor>.Storage storage, IntPtr additionalPtr, IntPtr processorFunction, ref JobRanges ranges, int jobIndex)
			{
				ProcessorFunction processorFunction2 = (ProcessorFunction)(int)processorFunction;
				switch (processorFunction2)
				{
				case ProcessorFunction.OutputProcessEarly:
				{
					IRootOutputProcessorExtensions.ProcessPhaseUpdateArguments* ptr = (IRootOutputProcessorExtensions.ProcessPhaseUpdateArguments*)(void*)additionalPtr;
					ptr->InOut = storage.UserProcessor.EarlyProcessing(ptr->Context, new ProcessorInstance.Pipe(ptr->Self, null));
					break;
				}
				case ProcessorFunction.OutputProcess:
				{
					IRootOutputProcessorExtensions.ProcessPhaseUpdateArguments* ptr2 = (IRootOutputProcessorExtensions.ProcessPhaseUpdateArguments*)(void*)additionalPtr;
					storage.UserProcessor.Process(ptr2->Context, new ProcessorInstance.Pipe(ptr2->Self, null), ptr2->InOut);
					break;
				}
				case ProcessorFunction.OutputProcessEnd:
				{
					IRootOutputProcessorExtensions.ProcessPhaseUpdateArguments* ptr3 = (IRootOutputProcessorExtensions.ProcessPhaseUpdateArguments*)(void*)additionalPtr;
					Span<float> span = new Span<float>((void*)ptr3->AudioBuffer, ptr3->OutputChannelCount * ptr3->OutputFrameCount);
					ChannelBuffer channelBuffer = new ChannelBuffer(span, ptr3->OutputChannelCount);
					storage.UserProcessor.EndProcessing(ptr3->Context, new ProcessorInstance.Pipe(ptr3->Self, null), channelBuffer);
					break;
				}
				case ProcessorFunction.OutputRemoved:
					storage.UserProcessor.RemovedFromProcessing();
					break;
				default:
					ProcessorExtensions.DispatchGenericProcessor<TUserProcessor>(ref storage.UserProcessor, in storage.Header, (void*)additionalPtr, processorFunction2);
					break;
				}
			}

			internal static readonly BurstLike.SharedStatic<IntPtr> jobReflectionData = BurstLike.SharedStatic<IntPtr>.GetOrCreate<IRootOutputProcessorExtensions.JobStruct<TUserProcessor>>(0U);

			internal struct Storage
			{
				public ProcessorHeader Header;

				public TUserProcessor UserProcessor;
			}

			internal delegate void ExecuteJobFunction(ref IRootOutputProcessorExtensions.JobStruct<TUserProcessor>.Storage storage, IntPtr additionalPtr, IntPtr additionalPtr2, ref JobRanges ranges, int jobIndex);
		}
	}
}
