using System;

namespace UnityEngine.UIElements
{
	[Flags]
	internal enum InvokePolicy
	{
		Default = 0,
		IncludeDisabled = 1,
		Once = 2
	}
}
