using System;

namespace System.Drawing.Printing
{
	[Serializable]
	public enum PrinterResolutionKind
	{
		Custom,
		Draft = -1,
		High = -4,
		Low = -2,
		Medium = -3
	}
}
