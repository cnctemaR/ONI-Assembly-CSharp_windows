using System;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.PlayerLoop
{
	[MovedFrom("UnityEngine.Experimental.PlayerLoop")]
	[RequiredByNativeCode]
	public struct PreLateUpdate
	{
		[RequiredByNativeCode]
		public struct AIUpdatePostScript
		{
		}

		[RequiredByNativeCode]
		public struct DirectorUpdateAnimationBegin
		{
		}

		[RequiredByNativeCode]
		public struct LegacyAnimationUpdate
		{
		}

		[RequiredByNativeCode]
		public struct DirectorUpdateAnimationEnd
		{
		}

		[RequiredByNativeCode]
		public struct DirectorDeferredEvaluate
		{
		}

		[RequiredByNativeCode]
		public struct UpdateNetworkManager
		{
		}

		[RequiredByNativeCode]
		public struct UpdateMasterServerInterface
		{
		}

		[RequiredByNativeCode]
		public struct UNetUpdate
		{
		}

		[RequiredByNativeCode]
		public struct EndGraphicsJobsAfterScriptUpdate
		{
		}

		[RequiredByNativeCode]
		public struct ParticleSystemBeginUpdateAll
		{
		}

		[RequiredByNativeCode]
		public struct ScriptRunBehaviourLateUpdate
		{
		}

		[RequiredByNativeCode]
		public struct ConstraintManagerUpdate
		{
		}
	}
}
