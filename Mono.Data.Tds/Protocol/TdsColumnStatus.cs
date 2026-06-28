using System;

namespace Mono.Data.Tds.Protocol
{
	public enum TdsColumnStatus
	{
		IsExpression = 4,
		IsKey = 8,
		Hidden = 16,
		Rename = 32
	}
}
