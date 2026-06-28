using System;

namespace System.Drawing.Printing
{
	[Serializable]
	public enum PrintingPermissionLevel
	{
		AllPrinting = 3,
		DefaultPrinting = 2,
		NoPrinting = 0,
		SafePrinting
	}
}
