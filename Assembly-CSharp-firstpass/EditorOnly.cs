using System;
using System.Diagnostics;

public static class EditorOnly
{
	[Conditional("UNITY_EDITOR")]
	public static void Assert(bool condition, string message)
	{
		DebugUtil.Assert(condition, message);
	}
}
