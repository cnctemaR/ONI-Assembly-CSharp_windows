using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Enum, Inherited = false)]
	[Serializable]
	public class FlagsAttribute : Attribute
	{
	}
}
