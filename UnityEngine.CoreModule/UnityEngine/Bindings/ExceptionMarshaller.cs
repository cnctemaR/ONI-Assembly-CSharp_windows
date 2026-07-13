using System;
using System.Runtime.CompilerServices;
using UnityEngine.Scripting;

namespace UnityEngine.Bindings
{
	[VisibleToOtherModules]
	internal static class ExceptionMarshaller
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CheckPendingException()
		{
			Exception ex = ExceptionMarshaller.s_pendingException;
			bool flag = ex != null;
			if (flag)
			{
				ExceptionMarshaller.s_pendingException = null;
				throw ex;
			}
		}

		[RequiredByNativeCode]
		private static void SetPendingException(Exception ex)
		{
			ExceptionMarshaller.s_pendingException = ex;
		}

		[ThreadStatic]
		private static Exception s_pendingException;
	}
}
