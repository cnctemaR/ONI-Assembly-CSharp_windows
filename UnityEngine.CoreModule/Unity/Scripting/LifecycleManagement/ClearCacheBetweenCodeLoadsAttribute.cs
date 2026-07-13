using System;

namespace Unity.Scripting.LifecycleManagement
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	internal sealed class ClearCacheBetweenCodeLoadsAttribute : LifecycleAttributeBase
	{
	}
}
