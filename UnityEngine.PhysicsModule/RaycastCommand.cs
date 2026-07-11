using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine.Bindings;

namespace UnityEngine
{
	/// <summary>
	///   <para>Struct used to set up a raycast command to be performed asynchronously during a job.</para>
	/// </summary>
	[NativeHeader("Runtime/Jobs/ScriptBindings/JobsBindingsTypes.h")]
	[NativeHeader("Runtime/Dynamics/BatchCommands/RaycastCommand.h")]
	public struct RaycastCommand
	{
		/// <summary>
		///   <para>Create a RaycastCommand.</para>
		/// </summary>
		/// <param name="from">The starting point of the ray in world coordinates.</param>
		/// <param name="direction">The direction of the ray.</param>
		/// <param name="distance">The maximum distance the ray should check for collisions.</param>
		/// <param name="layerMask">A LayerMask that is used to selectively ignore Colliders when casting a ray.</param>
		/// <param name="maxHits">The maximum number of Colliders the ray can hit.</param>
		public RaycastCommand(Vector3 from, Vector3 direction, float distance = 3.4028235E+38f, int layerMask = -5, int maxHits = 1)
		{
			this.from = from;
			this.direction = direction;
			this.distance = distance;
			this.layerMask = layerMask;
			this.maxHits = maxHits;
		}

		/// <summary>
		///   <para>The maximum distance the ray should check for collisions.</para>
		/// </summary>
		public float distance { get; set; }

		/// <summary>
		///   <para>A LayerMask that is used to selectively ignore Colliders when casting a ray.</para>
		/// </summary>
		public int layerMask { get; set; }

		/// <summary>
		///   <para>The maximum number of Colliders the ray can hit.</para>
		/// </summary>
		public int maxHits { get; set; }

		public static JobHandle ScheduleBatch(NativeArray<RaycastCommand> commands, NativeArray<RaycastHit> results, int minCommandsPerJob, JobHandle dependsOn = default(JobHandle))
		{
			BatchQueryJob<RaycastCommand, RaycastHit> batchQueryJob = new BatchQueryJob<RaycastCommand, RaycastHit>(commands, results);
			JobsUtility.JobScheduleParameters jobScheduleParameters = new JobsUtility.JobScheduleParameters(UnsafeUtility.AddressOf<BatchQueryJob<RaycastCommand, RaycastHit>>(ref batchQueryJob), BatchQueryJobStruct<BatchQueryJob<RaycastCommand, RaycastHit>>.Initialize(), dependsOn, ScheduleMode.Batched);
			return RaycastCommand.ScheduleRaycastBatch(ref jobScheduleParameters, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<RaycastCommand>(commands), commands.Length, NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks<RaycastHit>(results), results.Length, minCommandsPerJob);
		}

		[FreeFunction("ScheduleRaycastCommandBatch")]
		private unsafe static JobHandle ScheduleRaycastBatch(ref JobsUtility.JobScheduleParameters parameters, void* commands, int commandLen, void* result, int resultLen, int minCommandsPerJob)
		{
			JobHandle jobHandle;
			RaycastCommand.ScheduleRaycastBatch_Injected(ref parameters, commands, commandLen, result, resultLen, minCommandsPerJob, out jobHandle);
			return jobHandle;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void ScheduleRaycastBatch_Injected(ref JobsUtility.JobScheduleParameters parameters, void* commands, int commandLen, void* result, int resultLen, int minCommandsPerJob, out JobHandle ret);

		/// <summary>
		///   <para>The starting point of the ray in world coordinates.</para>
		/// </summary>
		public Vector3 from;

		/// <summary>
		///   <para>The direction of the ray.</para>
		/// </summary>
		public Vector3 direction;
	}
}
