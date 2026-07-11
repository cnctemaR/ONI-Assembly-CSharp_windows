using System;

namespace System.Runtime.Diagnostics
{
	internal enum EventSeverity : uint
	{
		Success,
		Informational = 1073741824U,
		Warning = 2147483648U,
		Error = 3221225472U
	}
}
