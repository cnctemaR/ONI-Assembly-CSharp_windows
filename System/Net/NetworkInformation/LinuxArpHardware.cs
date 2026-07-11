using System;

namespace System.Net.NetworkInformation
{
	internal enum LinuxArpHardware
	{
		ETHER = 1,
		EETHER,
		PRONET = 4,
		ATM = 19,
		SLIP = 256,
		CSLIP,
		SLIP6,
		CSLIP6,
		PPP = 512,
		LOOPBACK = 772,
		FDDI = 774,
		TUNNEL = 768,
		TUNNEL6,
		SIT = 776,
		IPDDP,
		IPGRE,
		IP6GRE = 823
	}
}
