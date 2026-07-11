using System;

namespace UnityEngine
{
	/// <summary>
	///   <para>An exception that will prevent all subsequent immediate mode GUI functions from evaluating for the remainder of the GUI loop.</para>
	/// </summary>
	public sealed class ExitGUIException : Exception
	{
	}
}
