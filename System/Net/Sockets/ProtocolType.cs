using System;

namespace System.Net.Sockets
{
	public enum ProtocolType
	{
		IP,
		Icmp,
		Igmp,
		Ggp,
		Tcp = 6,
		Pup = 12,
		Udp = 17,
		Idp = 22,
		IPv6 = 41,
		ND = 77,
		Raw = 255,
		Unspecified = 0,
		Ipx = 1000,
		Spx = 1256,
		SpxII,
		Unknown = -1,
		IPv4 = 4,
		IPv6RoutingHeader = 43,
		IPv6FragmentHeader,
		IPSecEncapsulatingSecurityPayload = 50,
		IPSecAuthenticationHeader,
		IcmpV6 = 58,
		IPv6NoNextHeader,
		IPv6DestinationOptions,
		IPv6HopByHopOptions = 0
	}
}
