using System;

namespace System.Net.Sockets
{
	public enum ProtocolType
	{
		IP,
		IPv6HopByHopOptions = 0,
		Icmp,
		Igmp,
		Ggp,
		IPv4,
		Tcp = 6,
		Pup = 12,
		Udp = 17,
		Idp = 22,
		IPv6 = 41,
		IPv6RoutingHeader = 43,
		IPv6FragmentHeader,
		IPSecEncapsulatingSecurityPayload = 50,
		IPSecAuthenticationHeader,
		IcmpV6 = 58,
		IPv6NoNextHeader,
		IPv6DestinationOptions,
		ND = 77,
		Raw = 255,
		Unspecified = 0,
		Ipx = 1000,
		Spx = 1256,
		SpxII,
		Unknown = -1
	}
}
