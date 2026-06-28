using System;

namespace System.Net.NetworkInformation
{
	public enum NetBiosNodeType
	{
		Unknown,
		Broadcast,
		Peer2Peer,
		Mixed = 4,
		Hybrid = 8
	}
}
