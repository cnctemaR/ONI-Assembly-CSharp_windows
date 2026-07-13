using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.UnityConsent
{
	[NativeHeader("Modules/UnityConsent/EndUserConsent.h")]
	public static class EndUserConsent
	{
		[NativeMethod("GetConsentStateStatic")]
		public static ConsentState GetConsentState()
		{
			ConsentState consentState;
			EndUserConsent.GetConsentState_Injected(out consentState);
			return consentState;
		}

		[NativeMethod("SetConsentStateStatic")]
		public static void SetConsentState(ConsentState consentState)
		{
			EndUserConsent.SetConsentState_Injected(ref consentState);
		}

		[field: DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<ConsentState> consentStateChanged;

		[RequiredByNativeCode]
		private static void OnConsentStateChanged()
		{
			bool flag = EndUserConsent.consentStateChanged != null;
			if (flag)
			{
				EndUserConsent.consentStateChanged(EndUserConsent.GetConsentState());
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetConsentState_Injected(out ConsentState ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetConsentState_Injected([In] ref ConsentState consentState);
	}
}
