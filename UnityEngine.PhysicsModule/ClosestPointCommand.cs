using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Modules/Physics/BatchCommands/ClosestPointCommand.h")]
	[NativeHeader("Runtime/Jobs/ScriptBindings/JobsBindingsTypes.h")]
	public struct ClosestPointCommand
	{
		public ClosestPointCommand(Vector3 point, int colliderInstanceID, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			this.point = point;
			this.colliderInstanceID = colliderInstanceID;
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}

		public ClosestPointCommand(Vector3 point, Collider collider, Vector3 position, Quaternion rotation, Vector3 scale)
		{
			this.point = point;
			this.colliderInstanceID = collider.GetInstanceID();
			this.position = position;
			this.rotation = rotation;
			this.scale = scale;
		}

		public Vector3 point { readonly get; set; }

		public int colliderInstanceID { readonly get; set; }

		public Vector3 position { readonly get; set; }

		public Quaternion rotation { readonly get; set; }

		public Vector3 scale { readonly get; set; }

		public static JobHandle ScheduleBatch(NativeArray<ClosestPointCommand> commands, NativeArray<Vector3> results, int minCommandsPerJob, JobHandle dependsOn = default(JobHandle))
		{
			BatchQueryJob<ClosestPointCommand, Vector3> batchQueryJob = new BatchQueryJob<ClosestPointCommand, Vector3>(commands, results);
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<BatchQueryJob<ClosestPointCommand, Vector3>>(ref batchQueryJob), BatchQueryJobStruct<BatchQueryJob<ClosestPointCommand, Vector3>>.Initialize(), dependsOn, ScheduleMode.Batched);
			return ClosestPointCommand.ScheduleClosestPointCommandBatch(ref jobScheduleParameters, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<ClosestPointCommand>(commands), commands.Length, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<Vector3>(results), results.Length, minCommandsPerJob);
		}

		[FreeFunction("ScheduleClosestPointCommandBatch", ThrowsException = true)]
		private unsafe static JobHandle ScheduleClosestPointCommandBatch(ref JobsUtility.JobScheduleParameters parameters, void* commands, int commandLen, void* result, int resultLen, int minCommandsPerJob)
		{
			JobHandle jobHandle;
			ClosestPointCommand.ScheduleClosestPointCommandBatch_Injected(ref parameters, commands, commandLen, result, resultLen, minCommandsPerJob, out jobHandle);
			return jobHandle;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ScheduleClosestPointCommandBatch_Injected(ref JobsUtility.JobScheduleParameters parameters, void* commands, int commandLen, void* result, int resultLen, int minCommandsPerJob, out JobHandle ret);
	}
}
