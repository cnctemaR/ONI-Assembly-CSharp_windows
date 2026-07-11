using System;

namespace System.Net.NetworkInformation
{
	public enum IPStatus
	{
		Success,
		DestinationNetworkUnreachable = 11002,
		DestinationHostUnreachable,
		DestinationProtocolUnreachable,
		DestinationPortUnreachable,
		DestinationProhibited = 11004,
		NoResources = 11006,
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
		DestinationScopeMismatch,
		Unknown = -1
	}
}
