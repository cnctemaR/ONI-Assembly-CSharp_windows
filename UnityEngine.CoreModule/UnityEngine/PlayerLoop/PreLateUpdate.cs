using System;
using UnityEngine.Scripting;
using UnityEngine.Scripting.APIUpdating;

namespace UnityEngine.PlayerLoop
{
	[RequiredByNativeCode]
	[MovedFrom("UnityEngine.Experimental.PlayerLoop")]
	public struct PreLateUpdate
	{
		[RequiredByNativeCode]
		public struct Physics2DLateUpdate
		{
		}

		[RequiredByNativeCode]
		public struct PhysicsLateUpdate
		{
		}

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
		public struct UIElementsUpdatePanels
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
