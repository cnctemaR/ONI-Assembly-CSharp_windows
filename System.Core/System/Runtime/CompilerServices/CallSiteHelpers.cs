using System;
using System.Dynamic;
using System.Reflection;

namespace System.Runtime.CompilerServices
{
	public static class CallSiteHelpers
	{
		public static bool IsInternalFrame(MethodBase mb)
		{
			return (mb.Name == "CallSite.Target" && mb.GetType() != CallSiteHelpers.s_knownNonDynamicMethodType) || mb.DeclaringType == typeof(UpdateDelegates);
		}

		private static readonly Type s_knownNonDynamicMethodType = typeof(object).GetMethod("ToString").GetType();
	}
}
