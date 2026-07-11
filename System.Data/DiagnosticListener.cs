using System;

internal class DiagnosticListener
{
	internal DiagnosticListener(string s)
	{
	}

	internal bool IsEnabled(string s)
	{
		return DiagnosticListener.DiagnosticListenerEnabled;
	}

	internal void Write(string s1, object s2)
	{
		Console.WriteLine(string.Format("|| {0},  {1}", s1, s2));
	}

	internal static bool DiagnosticListenerEnabled;
}
