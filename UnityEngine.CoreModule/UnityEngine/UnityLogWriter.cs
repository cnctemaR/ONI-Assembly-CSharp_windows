using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Bindings;

namespace UnityEngine
{
	[NativeHeader("Runtime/Export/Logging/UnityLogWriter.bindings.h")]
	internal class UnityLogWriter : TextWriter
	{
		[ThreadAndSerializationSafe]
		public static void WriteStringToUnityLog(string s)
		{
			bool flag = s == null;
			if (!flag)
			{
				UnityLogWriter.WriteStringToUnityLogImpl(s);
			}
		}

		[FreeFunction(IsThreadSafe = true)]
		private unsafe static void WriteStringToUnityLogImpl(string s)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(s, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = s.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				UnityLogWriter.WriteStringToUnityLogImpl_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		public static void Init()
		{
			TextWriter textWriter = TextWriter.Synchronized(new UnityLogWriter());
			Console.SetOut(textWriter);
			Console.SetError(textWriter);
		}

		public override Encoding Encoding
		{
			get
			{
				return Encoding.UTF8;
			}
		}

		public override void Write(char value)
		{
			UnityLogWriter.WriteStringToUnityLog(value.ToString());
		}

		public override void Write(string s)
		{
			UnityLogWriter.WriteStringToUnityLog(s);
		}

		public override void Write(char[] buffer, int index, int count)
		{
			UnityLogWriter.WriteStringToUnityLogImpl(new string(buffer, index, count));
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WriteStringToUnityLogImpl_Injected(ref ManagedSpanWrapper s);
	}
}
