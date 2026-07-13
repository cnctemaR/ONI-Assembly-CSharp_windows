using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Jobs/ScriptBindings/JobsBindingsTypes.h")]
	[NativeHeader("Modules/Physics/BatchCommands/RaycastCommand.h")]
	public struct RaycastCommand
	{
		public RaycastCommand(Vector3 from, Vector3 direction, QueryParameters queryParameters, float distance = 3.4028235E+38f)
		{
			this.from = from;
			this.direction = direction;
			this.physicsScene = Physics.defaultPhysicsScene;
			this.distance = distance;
			this.queryParameters = queryParameters;
		}

		public RaycastCommand(PhysicsScene physicsScene, Vector3 from, Vector3 direction, QueryParameters queryParameters, float distance = 3.4028235E+38f)
		{
			this.from = from;
			this.direction = direction;
			this.physicsScene = physicsScene;
			this.distance = distance;
			this.queryParameters = queryParameters;
		}

		public Vector3 from { readonly get; set; }

		public Vector3 direction { readonly get; set; }

		public PhysicsScene physicsScene { readonly get; set; }

		public float distance { readonly get; set; }

		public static JobHandle ScheduleBatch(NativeArray<RaycastCommand> commands, NativeArray<RaycastHit> results, int minCommandsPerJob, int maxHits, JobHandle dependsOn = default(JobHandle))
		{
			bool flag = maxHits < 1;
			JobHandle jobHandle;
			if (flag)
			{
				Debug.LogWarning("maxHits should be greater than 0.");
				jobHandle = default(JobHandle);
			}
			else
			{
				bool flag2 = results.Length < maxHits * commands.Length;
				if (flag2)
				{
					Debug.LogWarning("The supplied results buffer is too small, there should be at least maxHits space per each command in the batch.");
					jobHandle = default(JobHandle);
				}
				else
				{
					BatchQueryJob<RaycastCommand, RaycastHit> batchQueryJob = new BatchQueryJob<RaycastCommand, RaycastHit>(commands, results);
					JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<BatchQueryJob<RaycastCommand, RaycastHit>>(ref batchQueryJob), BatchQueryJobStruct<BatchQueryJob<RaycastCommand, RaycastHit>>.Initialize(), dependsOn, ScheduleMode.Batched);
					jobHandle = RaycastCommand.ScheduleRaycastBatch(ref jobScheduleParameters, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<RaycastCommand>(commands), commands.Length, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<RaycastHit>(results), results.Length, minCommandsPerJob, maxHits);
				}
			}
			return jobHandle;
		}

		public static JobHandle ScheduleBatch(NativeArray<RaycastCommand> commands, NativeArray<RaycastHit> results, int minCommandsPerJob, JobHandle dependsOn = default(JobHandle))
		{
			return RaycastCommand.ScheduleBatch(commands, results, minCommandsPerJob, 1, dependsOn);
		}

		[FreeFunction("ScheduleRaycastCommandBatch", ThrowsException = true)]
		private unsafe static JobHandle ScheduleRaycastBatch(ref JobsUtility.JobScheduleParameters parameters, void* commands, int commandLen, void* result, int resultLen, int minCommandsPerJob, int maxHits)
		{
			JobHandle jobHandle;
			RaycastCommand.ScheduleRaycastBatch_Injected(ref parameters, commands, commandLen, result, resultLen, minCommandsPerJob, maxHits, out jobHandle);
			return jobHandle;
		}

		[Obsolete("This struct signature is no longer supported. Use struct with a QueryParameters instead", false)]
		public RaycastCommand(Vector3 from, Vector3 direction, float distance = 3.4028235E+38f, int layerMask = -5, int maxHits = 1)
		{
			this.from = from;
			this.direction = direction;
			this.physicsScene = Physics.defaultPhysicsScene;
			this.queryParameters = QueryParameters.Default;
			this.distance = distance;
			this.layerMask = layerMask;
		}

		[Obsolete("This struct signature is no longer supported. Use struct with a QueryParameters instead", false)]
		public RaycastCommand(PhysicsScene physicsScene, Vector3 from, Vector3 direction, float distance = 3.4028235E+38f, int layerMask = -5, int maxHits = 1)
		{
			this.from = from;
			this.direction = direction;
			this.physicsScene = physicsScene;
			this.queryParameters = QueryParameters.Default;
			this.distance = distance;
			this.layerMask = layerMask;
		}

		[Obsolete("maxHits property was moved to be a part of RaycastCommand.ScheduleBatch.", false)]
		public int maxHits
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}

		[Obsolete("Layer Mask is now a part of QueryParameters struct", false)]
		public int layerMask
		{
			get
			{
				return this.queryParameters.layerMask;
			}
			set
			{
				this.queryParameters.layerMask = value;
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ScheduleRaycastBatch_Injected(ref JobsUtility.JobScheduleParameters parameters, void* commands, int commandLen, void* result, int resultLen, int minCommandsPerJob, int maxHits, out JobHandle ret);

		public QueryParameters queryParameters;
	}
}
