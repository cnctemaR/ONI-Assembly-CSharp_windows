using System;

namespace Mono.Security.Protocol.Ntlm
{
	public static class NtlmSettings
	{
		public static NtlmAuthLevel DefaultAuthLevel
		{
			get
			{
				return NtlmSettings.defaultAuthLevel;
			}
			set
			{
				NtlmSettings.defaultAuthLevel = value;
			}
		}

		private static NtlmAuthLevel defaultAuthLevel = NtlmAuthLevel.LM_and_NTLM_and_try_NTLMv2_Session;
	}
}
