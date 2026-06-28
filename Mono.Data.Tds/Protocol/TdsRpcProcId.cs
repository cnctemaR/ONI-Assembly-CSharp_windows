using System;

namespace Mono.Data.Tds.Protocol
{
	public enum TdsRpcProcId
	{
		Cursor = 1,
		CursorOpen,
		CursorPrepare,
		CursorExecute,
		CursorPrepExec,
		CursorUnprepare,
		CursorFetch,
		CursorOption,
		CursorClose,
		ExecuteSql,
		Prepare,
		Execute,
		PrepExec,
		PrepExecRpc,
		Unprepare
	}
}
