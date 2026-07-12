using System;

namespace System.ComponentModel.Composition
{
	internal enum ExportCardinalityCheckResult
	{
		Match,
		NoExports,
		TooManyExports
	}
}
