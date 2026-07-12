using System;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.PlayerLoop
{
	[MovedFrom("UnityEngine.Experimental.PlayerLoop")]
	[RequiredByNativeCode]
	public struct Initialization
	{
		[RequiredByNativeCode]
		public struct ProfilerStartFrame
		{
		}

		[Obsolete("PlayerUpdateTime player loop component has been moved to its own category called TimeUpdate. (UnityUpgradable) -> UnityEngine.PlayerLoop.TimeUpdate/WaitForLastPresentationAndUpdateTime", true)]
		public struct PlayerUpdateTime
		{
		}

		[RequiredByNativeCode]
		public struct UpdateCameraMotionVectors
		{
		}

		[RequiredByNativeCode]
		public struct DirectorSampleTime
		{
		}

		[RequiredByNativeCode]
		public struct AsyncUploadTimeSlicedUpdate
		{
		}

		[RequiredByNativeCode]
		public struct SynchronizeState
		{
		}

		[RequiredByNativeCode]
		public struct SynchronizeInputs
		{
		}

		[RequiredByNativeCode]
		public struct XREarlyUpdate
		{
		}
	}
}
