using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[Serializable]
	public enum OperandType
	{
		InlineBrTarget,
		InlineField,
		InlineI,
		InlineI8,
		InlineMethod,
		InlineNone,
		[Obsolete("This API has been deprecated.")]
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
