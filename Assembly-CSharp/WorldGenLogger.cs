using System;

public static class WorldGenLogger
{
	public static void LogException(string message, string stack)
	{
		Output.LogError(new object[] { message + "\n" + stack });
	}
}
