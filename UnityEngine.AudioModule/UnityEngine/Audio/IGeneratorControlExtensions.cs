using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.Audio
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static class IGeneratorControlExtensions
	{
		internal unsafe static IntPtr GetReflectionData<[global::System.Runtime.CompilerServices.IsUnmanaged] TUserControl, [global::System.Runtime.CompilerServices.IsUnmanaged] TUserGenerator>() where TUserControl : struct, ValueType, GeneratorInstance.IControl<TUserGenerator> where TUserGenerator : struct, ValueType, GeneratorInstance.IRealtime
		{
			IGeneratorControlExtensions.JobStruct<TUserControl, TUserGenerator>.Initialize();
			return *IGeneratorControlExtensions.JobStruct<TUserControl, TUserGenerator>.jobReflectionData.Data;
		}

		internal struct JobStruct<[global::System.Runtime.CompilerServices.IsUnmanaged] TUserControl, [global::System.Runtime.CompilerServices.IsUnmanaged] TUserProcessor> where TUserControl : struct, ValueType, GeneratorInstance.IControl<TUserProcessor> where TUserProcessor : struct, ValueType, GeneratorInstance.IRealtime
		{
			[BurstDiscard]
			internal unsafe static void Initialize()
			{
				bool flag = *IGeneratorControlExtensions.JobStruct<TUserControl, TUserProcessor>.jobReflectionData.Data == IntPtr.Zero;
				if (flag)
				{
					*IGeneratorControlExtensions.JobStruct<TUserControl, TUserProcessor>.jobReflectionData.Data = JobsUtility.CreateJobReflectionData(typeof(IGeneratorControlExtensions.JobStruct<TUserControl, TUserProcessor>.ControlStorage), typeof(TUserControl), new IGeneratorControlExtensions.JobStruct<TUserControl, TUserProcessor>.ExecuteJobFunction(IGeneratorControlExtensions.JobStruct<TUserControl, TUserProcessor>.Execute));
				}
			}

			public unsafe static void Execute(ref IGeneratorControlExtensions.JobStruct<TUserControl, TUserProcessor>.ControlStorage storage, IntPtr additionalPtr, IntPtr additionalPtr2, ref JobRanges ranges, int jobIndex)
			{
				ControlFunction controlFunction = (ControlFunction)(int)additionalPtr2;
				ControlFunction controlFunction2 = controlFunction;
				ControlFunction controlFunction3 = controlFunction2;
				if (controlFunction3 != ControlFunction.Configure)
				{
					ProcessorExtensions.DispatchGenericControl<TUserControl, TUserProcessor>(ref storage.UserControl, ref storage.HeaderAndProcessor.UserProcessor, in storage.HeaderAndProcessor.Header.Processor, (void*)additionalPtr, controlFunction);
				}
				else
				{
					ConfigureArguments* ptr = (ConfigureArguments*)(void*)additionalPtr;
					ControlContext controlContext = new ControlContext((void*)ptr->ControlContext);
					AudioFormat audioFormat = new AudioFormat(ptr->Now);
					storage.UserControl.Configure(controlContext, ref storage.HeaderAndProcessor.UserProcessor, in audioFormat, out storage.HeaderAndProcessor.Header.Configuration.Setup, ref storage.HeaderAndProcessor.Header.Configuration.Properties);
					bool flag = storage.HeaderAndProcessor.Header.Configuration.IsRealtime && storage.HeaderAndProcessor.Header.Configuration.Setup.sampleRate != ptr->Now.sampleRate;
					if (flag)
					{
						Debug.LogError("Realtime generators must obey system sampling rate");
					}
				}
			}

			internal static readonly BurstLike.SharedStatic<IntPtr> jobReflectionData = BurstLike.SharedStatic<IntPtr>.GetOrCreate<IGeneratorControlExtensions.JobStruct<TUserControl, TUserProcessor>>(0U);

			internal struct ControlStorage
			{
				public IGeneratorProcessorExtensions.JobStruct<TUserProcessor>.Storage HeaderAndProcessor;

				public TUserControl UserControl;
			}

			internal delegate void ExecuteJobFunction(ref IGeneratorControlExtensions.JobStruct<TUserControl, TUserProcessor>.ControlStorage storage, IntPtr additionalPtr, IntPtr additionalPtr2, ref JobRanges ranges, int jobIndex);
		}
	}
}
