using System;

namespace UnityEngine.Experimental.UIElements
{
	public static class IBindingExtensions
	{
		public static bool IsBound(this IBindable control)
		{
			return ((control != null) ? control.binding : null) != null;
		}
	}
}
