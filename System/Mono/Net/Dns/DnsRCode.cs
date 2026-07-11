using System;

namespace Mono.Net.Dns
{
	internal enum DnsRCode : ushort
	{
		NoError,
		FormErr,
		ServFail,
		NXDomain,
		NotImp,
		Refused,
		YXDomain,
		YXRRSet,
		NXRRSet,
		NotAuth,
		NotZone,
		BadVers = 16,
		BadSig = 16,
		BadKey,
		BadTime,
		BadMode,
		BadName,
		BadAlg,
		BadTrunc
	}
}
