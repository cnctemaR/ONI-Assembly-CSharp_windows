using System;

namespace UnityEngine.Analytics
{
	[Flags]
	public enum SendEventOptions
	{
		kAppendNone = 0,
		kAppendBuildGuid = 1,
		kAppendBuildTarget = 2
	}
}
