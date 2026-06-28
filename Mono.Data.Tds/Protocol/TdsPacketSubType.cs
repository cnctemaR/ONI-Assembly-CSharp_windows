using System;

namespace Mono.Data.Tds.Protocol
{
	public enum TdsPacketSubType
	{
		Capability = 226,
		Dynamic = 231,
		Dynamic2 = 163,
		EnvironmentChange = 227,
		Error = 170,
		Info,
		EED = 229,
		Param = 172,
		Authentication = 237,
		LoginAck = 173,
		ReturnStatus = 121,
		ProcId = 124,
		Done = 253,
		DoneProc,
		DoneInProc,
		ColumnName = 160,
		ColumnInfo,
		ColumnDetail = 165,
		AltName = 167,
		AltFormat,
		TableName = 164,
		ColumnOrder = 169,
		Control = 174,
		Row = 209,
		ColumnMetadata = 129,
		RowFormat = 238,
		ParamFormat = 236,
		Parameters = 215
	}
}
