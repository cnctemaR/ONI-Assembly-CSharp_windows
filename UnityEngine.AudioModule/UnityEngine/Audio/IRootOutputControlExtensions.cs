using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;

namespace UnityEngine.Audio
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static class IRootOutputControlExtensions
	{
		internal unsafe static IntPtr GetReflectionData<[global::System.Runtime.CompilerServices.IsUnmanaged] TUserControl, [global::System.Runtime.CompilerServices.IsUnmanaged] TUserProcessor>() where TUserControl : struct, ValueType, RootOutputInstance.IControl<TUserProcessor> where TUserProcessor : struct, ValueType, RootOutputInstance.IRealtime
		{
			IRootOutputControlExtensions.JobStruct<TUserControl, TUserProcessor>.Initialize();
			return *IRootOutputControlExtensions.JobStruct<TUserControl, TUserProcessor>.jobReflectionData.Data;
		}

		internal struct JobStruct<[global::System.Runtime.CompilerServices.IsUnmanaged] TUserControl, [global::System.Runtime.CompilerServices.IsUnmanaged] TUserProcessor> where TUserControl : struct, ValueType, RootOutputInstance.IControl<TUserProcessor> where TUserProcessor : struct, ValueType, RootOutputInstance.IRealtime
		{
			[BurstDiscard]
			internal unsafe static void Initialize()
			{
				bool flag = *IRootOutputControlExtensions.JobStruct<TUserControl, TUserProcessor>.jobReflectionData.Data == IntPtr.Zero;
				if (flag)
				{
					*IRootOutputControlExtensions.JobStruct<TUserControl, TUserProcessor>.jobReflectionData.Data = JobsUtility.CreateJobReflectionData(typeof(IRootOutputControlExtensions.JobStruct<TUserControl, TUserProcessor>.ControlStorage), typeof(TUserControl), new IRootOutputControlExtensions.JobStruct<TUserControl, TUserProcessor>.ExecuteJobFunction(IRootOutputControlExtensions.JobStruct<TUserControl, TUserProcessor>.Execute));
				}
			}

			public unsafe static void Execute(ref IRootOutputControlExtensions.JobStruct<TUserControl, TUserProcessor>.ControlStorage storage, IntPtr additionalPtr, IntPtr additionalPtr2, ref JobRanges ranges, int jobIndex)
			{
				ControlFunction controlFunction = (ControlFunction)(int)additionalPtr2;
				ControlFunction controlFunction2 = controlFunction;
				ControlFunction controlFunction3 = controlFunction2;
				if (controlFunction3 != ControlFunction.Configure)
				{
					ProcessorExtensions.DispatchGenericControl<TUserControl, TUserProcessor>(ref storage.UserControl, ref storage.HeaderAndProcessor.UserProcessor, in storage.HeaderAndProcessor.Header, (void*)additionalPtr, controlFunction);
				}
				else
				{
					ConfigureArguments* ptr = (ConfigureArguments*)(void*)additionalPtr;
					ControlContext controlContext = new ControlContext((void*)ptr->ControlContext);
					AudioFormat audioFormat = new AudioFormat(ptr->Now);
					storage.UserControl.Configure(controlContext, ref storage.HeaderAndProcessor.UserProcessor, in audioFormat).Complete();
				}
			}

			internal static readonly BurstLike.SharedStatic<IntPtr> jobReflectionData = BurstLike.SharedStatic<IntPtr>.GetOrCreate<IRootOutputControlExtensions.JobStruct<TUserControl, TUserProcessor>>(0U);

			internal struct ControlStorage
			{
				public IRootOutputProcessorExtensions.JobStruct<TUserProcessor>.Storage HeaderAndProcessor;

				public TUserControl UserControl;
			}

			internal delegate void ExecuteJobFunction(ref IRootOutputControlExtensions.JobStruct<TUserControl, TUserProcessor>.ControlStorage storage, IntPtr additionalPtr, IntPtr additionalPtr2, ref JobRanges ranges, int jobIndex);
		}
	}
}
