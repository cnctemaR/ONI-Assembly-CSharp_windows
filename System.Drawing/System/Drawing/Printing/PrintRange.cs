using System;

namespace System.Drawing.Printing
{
	public enum PrintRange
	{
		AllPages,
		SomePages = 2,
		Selection = 1,
		CurrentPage = 4194304
	}
}
