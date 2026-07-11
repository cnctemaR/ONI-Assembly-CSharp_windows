using System;

namespace System.Net
{
	internal static class ContextFlagsAdapterPal
	{
		internal static ContextFlagsPal GetContextFlagsPalFromInterop(Interop.NetSecurityNative.GssFlags gssFlags, bool isServer)
		{
			ContextFlagsPal contextFlagsPal = ContextFlagsPal.None;
			if ((gssFlags & Interop.NetSecurityNative.GssFlags.GSS_C_INTEG_FLAG) != (Interop.NetSecurityNative.GssFlags)0U)
			{
				contextFlagsPal |= (isServer ? ContextFlagsPal.AcceptIntegrity : ContextFlagsPal.AcceptStream);
			}
			foreach (ContextFlagsAdapterPal.ContextFlagMapping contextFlagMapping in ContextFlagsAdapterPal.s_contextFlagMapping)
			{
				if ((gssFlags & contextFlagMapping.GssFlags) == contextFlagMapping.GssFlags)
				{
					contextFlagsPal |= contextFlagMapping.ContextFlag;
				}
			}
			return contextFlagsPal;
		}

		internal static Interop.NetSecurityNative.GssFlags GetInteropFromContextFlagsPal(ContextFlagsPal flags, bool isServer)
		{
			Interop.NetSecurityNative.GssFlags gssFlags = (Interop.NetSecurityNative.GssFlags)0U;
			if (isServer)
			{
				if ((flags & ContextFlagsPal.AcceptIntegrity) != ContextFlagsPal.None)
				{
					gssFlags |= Interop.NetSecurityNative.GssFlags.GSS_C_INTEG_FLAG;
				}
			}
			else if ((flags & ContextFlagsPal.AcceptStream) != ContextFlagsPal.None)
			{
				gssFlags |= Interop.NetSecurityNative.GssFlags.GSS_C_INTEG_FLAG;
			}
			foreach (ContextFlagsAdapterPal.ContextFlagMapping contextFlagMapping in ContextFlagsAdapterPal.s_contextFlagMapping)
			{
				if ((flags & contextFlagMapping.ContextFlag) == contextFlagMapping.ContextFlag)
				{
					gssFlags |= contextFlagMapping.GssFlags;
				}
			}
			return gssFlags;
		}

		private static readonly ContextFlagsAdapterPal.ContextFlagMapping[] s_contextFlagMapping = new ContextFlagsAdapterPal.ContextFlagMapping[]
		{
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.NetSecurityNative.GssFlags.GSS_C_CONF_FLAG, ContextFlagsPal.Confidentiality),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.NetSecurityNative.GssFlags.GSS_C_IDENTIFY_FLAG, ContextFlagsPal.AcceptIntegrity),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.NetSecurityNative.GssFlags.GSS_C_MUTUAL_FLAG, ContextFlagsPal.MutualAuth),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.NetSecurityNative.GssFlags.GSS_C_REPLAY_FLAG, ContextFlagsPal.ReplayDetect),
			new ContextFlagsAdapterPal.ContextFlagMapping(Interop.NetSecurityNative.GssFlags.GSS_C_SEQUENCE_FLAG, ContextFlagsPal.SequenceDetect)
		};

		private struct ContextFlagMapping
		{
			public ContextFlagMapping(Interop.NetSecurityNative.GssFlags gssFlag, ContextFlagsPal contextFlag)
			{
				this.GssFlags = gssFlag;
				this.ContextFlag = contextFlag;
			}

			public readonly Interop.NetSecurityNative.GssFlags GssFlags;

			public readonly ContextFlagsPal ContextFlag;
		}
	}
}
