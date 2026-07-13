using System;
using UnityEngine.Bindings;

namespace Unity.Scripting.LifecycleManagement
{
	[VisibleToOtherModules]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	internal sealed class AfterAssemblyLoadedAttribute : LifecycleAttributeBase
	{
	}
}
