using System;

namespace System.IO
{
	[Flags]
	internal enum FilterFlags : uint
	{
		ReadPoll = 4096U,
		ReadOutOfBand = 8192U,
		ReadLowWaterMark = 1U,
		WriteLowWaterMark = 1U,
		NoteTrigger = 16777216U,
		NoteFFNop = 0U,
		NoteFFAnd = 1073741824U,
		NoteFFOr = 2147483648U,
		NoteFFCopy = 3221225472U,
		NoteFFCtrlMask = 3221225472U,
		NoteFFlagsMask = 16777215U,
		VNodeDelete = 1U,
		VNodeWrite = 2U,
		VNodeExtend = 4U,
		VNodeAttrib = 8U,
		VNodeLink = 16U,
		VNodeRename = 32U,
		VNodeRevoke = 64U,
		VNodeNone = 128U,
		ProcExit = 2147483648U,
		ProcFork = 1073741824U,
		ProcExec = 536870912U,
		ProcReap = 268435456U,
		ProcSignal = 134217728U,
		ProcExitStatus = 67108864U,
		ProcResourceEnd = 33554432U,
		ProcAppactive = 8388608U,
		ProcAppBackground = 4194304U,
		ProcAppNonUI = 2097152U,
		ProcAppInactive = 1048576U,
		ProcAppAllStates = 15728640U,
		ProcPDataMask = 1048575U,
		ProcControlMask = 4293918720U,
		VMPressure = 2147483648U,
		VMPressureTerminate = 1073741824U,
		VMPressureSuddenTerminate = 536870912U,
		VMError = 268435456U,
		TimerSeconds = 1U,
		TimerMicroSeconds = 2U,
		TimerNanoSeconds = 4U,
		TimerAbsolute = 8U
	}
}
