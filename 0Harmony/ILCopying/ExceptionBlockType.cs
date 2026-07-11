using System;

namespace Harmony.ILCopying
{
	public enum ExceptionBlockType
	{
		BeginExceptionBlock,
		BeginCatchBlock,
		BeginExceptFilterBlock,
		BeginFaultBlock,
		BeginFinallyBlock,
		EndExceptionBlock
	}
}
