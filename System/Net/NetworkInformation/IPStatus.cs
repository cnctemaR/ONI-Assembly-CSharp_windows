using System;

namespace System.Net.NetworkInformation
{
	public enum IPStatus
	{
		Unknown = -1,
		Success,
		DestinationNetworkUnreachable = 11002,
		DestinationHostUnreachable,
		DestinationProhibited,
		DestinationProtocolUnreachable = 11004,
		DestinationPortUnreachable,
		NoResources,
		BadOption,
		HardwareError,
		PacketTooBig,
		TimedOut,
		BadRoute = 11012,
		TtlExpired,
		TtlReassemblyTimeExceeded,
		ParameterProblem,
		SourceQuench,
		BadDestination = 11018,
		DestinationUnreachable = 11040,
		TimeExceeded,
		BadHeader,
		UnrecognizedNextHeader,
		IcmpError,
		DestinationScopeMismatch
	}
}
