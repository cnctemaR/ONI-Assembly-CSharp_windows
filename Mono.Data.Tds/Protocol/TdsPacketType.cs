using System;

namespace Mono.Data.Tds.Protocol
{
	public enum TdsPacketType
	{
		None,
		Query,
		Logon,
		Proc,
		Reply,
		Cancel = 6,
		Bulk,
		Logon70 = 16,
		SspAuth,
		Logoff = 113,
		Normal = 15,
		DBRPC = 230,
		RPC = 3
	}
}
