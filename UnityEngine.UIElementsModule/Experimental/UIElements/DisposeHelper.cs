using System;
using System.Diagnostics;

namespace UnityEngine.Experimental.UIElements
{
	internal class DisposeHelper
	{
		[Conditional("UNITY_UIELEMENTS_DEBUG_DISPOSE")]
		public static void NotifyMissingDispose(IDisposable disposable)
		{
			if (disposable != null)
			{
				Debug.LogError(string.Format("An IDisposable instance of type '{0}' has not been disposed.", disposable.GetType().FullName));
			}
		}
	}
}
