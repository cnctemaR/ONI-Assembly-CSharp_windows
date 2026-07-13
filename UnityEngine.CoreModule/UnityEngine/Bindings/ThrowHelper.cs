using System;
using System.Diagnostics.CodeAnalysis;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	internal static class ThrowHelper
	{
		[DoesNotReturn]
		public static void ThrowArgumentNullException(object obj, string parameterName)
		{
			Object @object = obj as Object;
			bool flag = @object != null;
			if (flag)
			{
				Object.MarshalledUnityObject.TryThrowEditorNullExceptionObject(@object, parameterName);
			}
			throw new ArgumentNullException(parameterName);
		}

		[DoesNotReturn]
		public static void ThrowNullReferenceException(object obj)
		{
			Object @object = obj as Object;
			bool flag = @object != null;
			if (flag)
			{
				Object.MarshalledUnityObject.TryThrowEditorNullExceptionObject(@object, null);
			}
			throw new NullReferenceException();
		}
	}
}
