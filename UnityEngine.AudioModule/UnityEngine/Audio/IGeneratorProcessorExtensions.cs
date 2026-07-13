using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Audio;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.Audio
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static class IGeneratorProcessorExtensions
	{
		internal unsafe static IntPtr GetReflectionData<[global::System.Runtime.CompilerServices.IsUnmanaged] TUserProcessor>() where TUserProcessor : struct, ValueType, GeneratorInstance.IRealtime
		{
			IGeneratorProcessorExtensions.JobStruct<TUserProcessor>.Initialize();
			return *IGeneratorProcessorExtensions.JobStruct<TUserProcessor>.jobReflectionData.Data;
		}

		[Obsolete("Types with embedded references are not supported in this version of your compiler.", true)]
		internal ref struct ProcessArguments
		{
			internal unsafe RealtimeContext* Context;

			internal unsafe float* AudioBuffer;

			internal Handle Self;

			internal int FrameCount;

			internal GeneratorInstance.Arguments GeneratorArguments;

			internal GeneratorInstance.Result Result;
		}

		internal struct JobStruct<[global::System.Runtime.CompilerServices.IsUnmanaged] TUserProcessor> where TUserProcessor : struct, ValueType, GeneratorInstance.IRealtime
		{
			[BurstDiscard]
			internal unsafe static void Initialize()
			{
				bool flag = *IGeneratorProcessorExtensions.JobStruct<TUserProcessor>.jobReflectionData.Data == IntPtr.Zero;
				if (flag)
				{
					*IGeneratorProcessorExtensions.JobStruct<TUserProcessor>.jobReflectionData.Data = JobsUtility.CreateJobReflectionData(typeof(IGeneratorProcessorExtensions.JobStruct<TUserProcessor>.Storage), typeof(TUserProcessor), new IGeneratorProcessorExtensions.JobStruct<TUserProcessor>.ExecuteJobFunction(IGeneratorProcessorExtensions.JobStruct<TUserProcessor>.Execute));
				}
			}

			public unsafe static void Execute(ref IGeneratorProcessorExtensions.JobStruct<TUserProcessor>.Storage storage, IntPtr additionalPtr, IntPtr additionalPtr2, ref JobRanges ranges, int jobIndex)
			{
				ProcessorFunction processorFunction = (ProcessorFunction)(int)additionalPtr2;
				ProcessorFunction processorFunction2 = processorFunction;
				ProcessorFunction processorFunction3 = processorFunction2;
				if (processorFunction3 != ProcessorFunction.Process)
				{
					ProcessorExtensions.DispatchGenericProcessor<TUserProcessor>(ref storage.UserProcessor, in storage.Header.Processor, (void*)additionalPtr, processorFunction);
				}
				else
				{
					IGeneratorProcessorExtensions.ProcessArguments* ptr = (IGeneratorProcessorExtensions.ProcessArguments*)(void*)additionalPtr;
					Span<float> span = new Span<float>((void*)ptr->AudioBuffer, storage.Header.Configuration.Setup.speakerMode.ChannelCount() * ptr->FrameCount);
					ChannelBuffer channelBuffer = new ChannelBuffer(span, storage.Header.Configuration.Setup.speakerMode.ChannelCount());
					ptr->Result = storage.UserProcessor.Process(ptr->Context, new ProcessorInstance.Pipe(ptr->Self, null), channelBuffer, ptr->GeneratorArguments);
				}
			}

			internal static readonly BurstLike.SharedStatic<IntPtr> jobReflectionData = BurstLike.SharedStatic<IntPtr>.GetOrCreate<IGeneratorProcessorExtensions.JobStruct<TUserProcessor>>(0U);

			internal struct Storage
			{
				public GeneratorInstance.GeneratorHeader Header;

				public TUserProcessor UserProcessor;
			}

			internal delegate void ExecuteJobFunction(ref IGeneratorProcessorExtensions.JobStruct<TUserProcessor>.Storage storage, IntPtr additionalPtr, IntPtr additionalPtr2, ref JobRanges ranges, int jobIndex);
		}
	}
}
