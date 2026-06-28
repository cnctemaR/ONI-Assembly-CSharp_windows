using System;

namespace Mono.Data.Tds.Protocol
{
	public enum TdsEnvPacketSubType
	{
		Database = 1,
		CharSet = 3,
		BlockSize,
		Locale,
		CollationInfo = 7
	}
}
