using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Debug/Debug.bindings.h")]
	internal sealed class DebugLogHandler : ILogHandler
	{
		[ThreadAndSerializationSafe]
		internal unsafe static void Internal_Log(LogType level, LogOption options, string msg, Object obj)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(msg, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = msg.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				DebugLogHandler.Internal_Log_Injected(level, options, ref managedSpanWrapper, Object.MarshalledUnityObject.Marshal<Object>(obj));
			}
			finally
			{
				char* ptr = null;
			}
		}

		[ThreadAndSerializationSafe]
		internal static void Internal_LogException(Exception ex, Object obj)
		{
			DebugLogHandler.Internal_LogException_Injected(ex, Object.MarshalledUnityObject.Marshal<Object>(obj));
		}

		public void LogFormat(LogType logType, Object context, string format, params object[] args)
		{
			DebugLogHandler.Internal_Log(logType, LogOption.None, string.Format(format, args), context);
		}

		public void LogFormat(LogType logType, LogOption logOptions, Object context, string format, params object[] args)
		{
			DebugLogHandler.Internal_Log(logType, logOptions, string.Format(format, args), context);
		}

		public void LogException(Exception exception, Object context)
		{
			bool flag = exception == null;
			if (flag)
			{
				throw new ArgumentNullException("exception");
			}
			DebugLogHandler.Internal_LogException(exception, context);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_Log_Injected(LogType level, LogOption options, ref ManagedSpanWrapper msg, IntPtr obj);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Internal_LogException_Injected(Exception ex, IntPtr obj);
	}
}
