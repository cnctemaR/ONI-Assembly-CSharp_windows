using System;
using System.Runtime.InteropServices;

namespace System
{
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class ParamArrayAttribute : Attribute
	{
	}
}
