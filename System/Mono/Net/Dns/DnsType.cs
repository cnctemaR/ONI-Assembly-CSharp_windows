using System;

namespace Mono.Net.Dns
{
	internal enum DnsType : ushort
	{
		A = 1,
		NS,
		[Obsolete]
		MD,
		[Obsolete]
		MF,
		CNAME,
		SOA,
		[Obsolete]
		MB,
		[Obsolete]
		MG,
		[Obsolete]
		MR,
		[Obsolete]
		NULL,
		[Obsolete]
		WKS,
		PTR,
		[Obsolete]
		HINFO,
		[Obsolete]
		MINFO,
		MX,
		TXT,
		[Obsolete]
		RP,
		AFSDB,
		[Obsolete]
		X25,
		[Obsolete]
		ISDN,
		[Obsolete]
		RT,
		[Obsolete]
		NSAP,
		[Obsolete]
		NSAPPTR,
		SIG,
		KEY,
		[Obsolete]
		PX,
		[Obsolete]
		GPOS,
		AAAA,
		LOC,
		[Obsolete]
		NXT,
		[Obsolete]
		EID,
		[Obsolete]
		NIMLOC,
		SRV,
		[Obsolete]
		ATMA,
		NAPTR,
		KX,
		CERT,
		[Obsolete]
		A6,
		DNAME,
		[Obsolete]
		SINK,
		OPT,
		[Obsolete]
		APL,
		DS,
		SSHFP,
		IPSECKEY,
		RRSIG,
		NSEC,
		DNSKEY,
		DHCID,
		NSEC3,
		NSEC3PARAM,
		HIP = 55,
		NINFO,
		RKEY,
		TALINK,
		SPF = 99,
		[Obsolete]
		UINFO,
		[Obsolete]
		UID,
		[Obsolete]
		GID,
		[Obsolete]
		UNSPEC,
		TKEY = 249,
		TSIG,
		IXFR,
		AXFR,
		[Obsolete]
		MAILB,
		[Obsolete]
		MAILA,
		URI = 256,
		TA = 32768,
		DLV
	}
}
