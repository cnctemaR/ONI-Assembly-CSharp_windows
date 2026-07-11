using System;
using Mono.Security.Interface;

namespace Mono.Unity
{
	internal static class Debug
	{
		public static void CheckAndThrow(UnityTls.unitytls_errorstate errorState, string context, AlertDescription defaultAlert = AlertDescription.InternalError)
		{
			if (errorState.code == UnityTls.unitytls_error_code.UNITYTLS_SUCCESS)
			{
				return;
			}
			string text = string.Format("{0} - error code: {1}", context, errorState.code);
			throw new TlsException(defaultAlert, text);
		}

		public static void CheckAndThrow(UnityTls.unitytls_errorstate errorState, UnityTls.unitytls_x509verify_result verifyResult, string context, AlertDescription defaultAlert = AlertDescription.InternalError)
		{
			if (verifyResult == UnityTls.unitytls_x509verify_result.UNITYTLS_X509VERIFY_SUCCESS)
			{
				Debug.CheckAndThrow(errorState, context, defaultAlert);
				return;
			}
			AlertDescription alertDescription = UnityTlsConversions.VerifyResultToAlertDescription(verifyResult, defaultAlert);
			string text = string.Format("{0} - error code: {1}, verify result: {2}", context, errorState.code, verifyResult);
			throw new TlsException(alertDescription, text);
		}
	}
}
