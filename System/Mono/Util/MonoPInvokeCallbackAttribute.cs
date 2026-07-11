using System;
using System.Diagnostics;

namespace Mono.Util
{
	[Conditional("MONOTOUCH")]
	[AttributeUsage(AttributeTargets.Method)]
	[Conditional("FULL_AOT_RUNTIME")]
	[Conditional("UNITY")]
	internal sealed class MonoPInvokeCallbackAttribute : Attribute
	{
		public MonoPInvokeCallbackAttribute(Type t)
		{
		}
	}
}
