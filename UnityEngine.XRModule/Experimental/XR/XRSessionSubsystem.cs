using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Experimental.XR
{
	[NativeHeader("Modules/XR/Subsystems/Session/XRSessionSubsystem.h")]
	[NativeConditional("ENABLE_XR")]
	[UsedByNativeCode]
	[NativeHeader("Modules/XR/XRPrefix.h")]
	public class XRSessionSubsystem : IntegratedSubsystem<XRSessionSubsystemDescriptor>
	{
		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public event Action<SessionTrackingStateChangedEventArgs> TrackingStateChanged;

		[NativeConditional("ENABLE_XR", StubReturnStatement = "kUnityXRTrackingStateUnknown")]
		public extern TrackingState TrackingState
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public extern int LastUpdatedFrame
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[RequiredByNativeCode]
		private void InvokeTrackingStateChangedEvent(TrackingState newState)
		{
			if (this.TrackingStateChanged != null)
			{
				this.TrackingStateChanged(new SessionTrackingStateChangedEventArgs
				{
					m_Session = this,
					NewState = newState
				});
			}
		}
	}
}
