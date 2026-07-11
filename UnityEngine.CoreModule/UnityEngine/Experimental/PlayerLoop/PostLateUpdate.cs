using System;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.PlayerLoop
{
	/// <summary>
	///   <para>Update phase in the native player loop.</para>
	/// </summary>
	[RequiredByNativeCode]
	public struct PostLateUpdate
	{
		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct PlayerSendFrameStarted
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct UpdateRectTransform
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct UpdateCanvasRectTransform
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct PlayerUpdateCanvases
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct UpdateAudio
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct UpdateVideo
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct DirectorLateUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ScriptRunDelayedDynamicFrameRate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ParticlesLegacyUpdateAllParticleSystems
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ParticleSystemEndUpdateAll
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct UpdateSubstance
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct UpdateCustomRenderTextures
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct UpdateAllRenderers
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct EnlightenRuntimeUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct UpdateAllSkinnedMeshes
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ProcessWebSendMessages
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct SortingGroupsUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct UpdateVideoTextures
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct DirectorRenderImage
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct PlayerEmitCanvasGeometry
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct FinishFrameRendering
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct BatchModeUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct PlayerSendFrameComplete
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct UpdateCaptureScreenshot
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct PresentAfterDraw
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ClearImmediateRenderers
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct XRPostPresent
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct UpdateResolution
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct InputEndFrame
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct GUIClearEvents
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ShaderHandleErrors
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ResetInputAxis
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ThreadedLoadingDebug
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ProfilerSynchronizeStats
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct MemoryFrameMaintenance
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ExecuteGameCenterCallbacks
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct ProfilerEndFrame
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct PlayerSendFramePostPresent
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct PhysicsSkinnedClothBeginUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct PhysicsSkinnedClothFinishUpdate
		{
		}

		/// <summary>
		///   <para>Native engine system updated by the native player loop.</para>
		/// </summary>
		[RequiredByNativeCode]
		public struct TriggerEndOfFrameCallbacks
		{
		}
	}
}
