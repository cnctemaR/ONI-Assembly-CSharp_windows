using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine.Bindings;

namespace Unity.Hierarchy
{
	[VisibleToOtherModules(new string[] { "UnityEngine.HierarchyModule", "UnityEditor.HierarchyModule" })]
	[NativeHeader("Modules/HierarchyCore/HierarchyLogging.h")]
	internal static class HierarchyLogging
	{
		[ThreadSafe]
		[StaticAccessor("HierarchyLogging", StaticAccessorType.DoubleColon)]
		[Conditional("ENABLE_HIERARCHY_LOGGING")]
		public unsafe static void SetLogFile(string path)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(path, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = path.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				HierarchyLogging.SetLogFile_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[Conditional("ENABLE_HIERARCHY_LOGGING")]
		[StaticAccessor("HierarchyLogging", StaticAccessorType.DoubleColon)]
		[NativeThrows]
		[ThreadSafe]
		public unsafe static void Log(string message)
		{
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(message, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = message.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				HierarchyLogging.Log_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
		}

		[Conditional("ENABLE_HIERARCHY_LOGGING")]
		[ThreadSafe]
		[StaticAccessor("HierarchyLogging", StaticAccessorType.DoubleColon)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Flush();

		public static string ToString<T>(T[] elements)
		{
			return HierarchyLogging.ToString<T>(new ReadOnlySpan<T>(elements));
		}

		public unsafe static string ToString<T>(IEnumerable<T> elements)
		{
			int num = 0;
			foreach (T t in elements)
			{
				num++;
			}
			int num2 = 0;
			T[] array = ArrayPool<T>.Shared.Rent(num);
			Span<T> span = array.AsSpan<T>(0, num);
			foreach (T t2 in elements)
			{
				*span[num2++] = t2;
			}
			string text = HierarchyLogging.ToString<T>(span);
			ArrayPool<T>.Shared.Return(array, false);
			return text;
		}

		public static string ToString<T>(ReadOnlySpan<T> elements)
		{
			return string.Format("[{0}]{{{1}}}", elements.Length, HierarchyLogging.Join<T>(", ", elements));
		}

		public static string Join<T>(string separator, T[] elements)
		{
			return HierarchyLogging.Join<T>(separator, new ReadOnlySpan<T>(elements));
		}

		public unsafe static string Join<T>(string separator, ReadOnlySpan<T> elements)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < elements.Length; i++)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				T t = *elements[i];
				stringBuilder2.Append(t.ToString());
				bool flag = i < elements.Length - 1;
				if (flag)
				{
					stringBuilder.Append(separator);
				}
			}
			return stringBuilder.ToString();
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetLogFile_Injected(ref ManagedSpanWrapper path);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Log_Injected(ref ManagedSpanWrapper message);
	}
}
