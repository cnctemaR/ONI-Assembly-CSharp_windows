using System;
using System.Runtime.ExceptionServices;

namespace Internal.Runtime.Augments
{
	internal class RuntimeAugments
	{
		public static void ReportUnhandledException(Exception exception)
		{
			ExceptionDispatchInfo.Capture(exception).Throw();
		}

		internal static ReflectionExecutionDomainCallbacks Callbacks
		{
			get
			{
				return RuntimeAugments.s_reflectionExecutionDomainCallbacks;
			}
		}

		private static ReflectionExecutionDomainCallbacks s_reflectionExecutionDomainCallbacks = new ReflectionExecutionDomainCallbacks();
	}
}
