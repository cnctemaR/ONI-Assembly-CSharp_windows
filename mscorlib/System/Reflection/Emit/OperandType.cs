using System;

namespace System.Reflection.Emit
{
	public enum OperandType
	{
		InlineBrTarget,
		InlineField,
		InlineI,
		InlineI8,
		InlineMethod,
		InlineNone,
		[Obsolete("This API has been deprecated. http://go.microsoft.com/fwlink/?linkid=14202")]
		InlinePhi,
		InlineR,
		InlineSig = 9,
		InlineString,
		InlineSwitch,
		InlineTok,
		InlineType,
		InlineVar,
		ShortInlineBrTarget,
		ShortInlineI,
		ShortInlineR,
		ShortInlineVar
	}
}
