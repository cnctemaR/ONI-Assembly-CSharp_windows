using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.Burst.LowLevel
{
	[NativeHeader("Runtime/Burst/Burst.h")]
	[NativeHeader("Runtime/Burst/BurstDelegateCache.h")]
	[StaticAccessor("BurstCompilerService::Get()", StaticAccessorType.Arrow)]
	internal static class BurstCompilerService
	{
		[NativeMethod("Initialize")]
		private unsafe static string InitializeInternal(string path, BurstCompilerService.ExtractCompilerFlags extractCompilerFlags)
		{
			string stringAndDispose;
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
				ManagedSpanWrapper managedSpanWrapper2;
				BurstCompilerService.InitializeInternal_Injected(ref managedSpanWrapper, extractCompilerFlags, out managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				ManagedSpanWrapper managedSpanWrapper2;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper2);
			}
			return stringAndDispose;
		}

		[ThreadSafe]
		public unsafe static string GetDisassembly(MethodInfo m, string compilerOptions)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(compilerOptions, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = compilerOptions.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				ManagedSpanWrapper managedSpanWrapper2;
				BurstCompilerService.GetDisassembly_Injected(m, ref managedSpanWrapper, out managedSpanWrapper2);
			}
			finally
			{
				char* ptr = null;
				ManagedSpanWrapper managedSpanWrapper2;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper2);
			}
			return stringAndDispose;
		}

		[FreeFunction(IsThreadSafe = true)]
		public unsafe static int CompileAsyncDelegateMethod(object delegateMethod, string compilerOptions)
		{
			int num;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(compilerOptions, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = compilerOptions.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				num = BurstCompilerService.CompileAsyncDelegateMethod_Injected(delegateMethod, ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return num;
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void* GetAsyncCompiledAsyncDelegateMethod(int userID);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void* GetOrCreateSharedMemory(ref Hash128 key, uint size_of, uint alignment);

		[ThreadSafe]
		public static string GetMethodSignature(MethodInfo method)
		{
			string stringAndDispose;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				BurstCompilerService.GetMethodSignature_Injected(method, out managedSpanWrapper);
			}
			finally
			{
				ManagedSpanWrapper managedSpanWrapper;
				stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpanWrapper);
			}
			return stringAndDispose;
		}

		public static extern bool IsInitialized
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void SetCurrentExecutionMode(uint environment);

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetCurrentExecutionMode();

		[FreeFunction("DefaultBurstLogCallback", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void Log(void* userData, BurstCompilerService.BurstLogType logType, byte* message, byte* filename, int lineNumber);

		[FreeFunction("DefaultBurstRuntimeLogCallback", true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void RuntimeLog(void* userData, BurstCompilerService.BurstLogType logType, byte* message, byte* filename, int lineNumber);

		public unsafe static bool LoadBurstLibrary(string fullPathToLibBurstGenerated)
		{
			bool flag;
			try
			{
				ManagedSpanWrapper managedSpanWrapper;
				if (!StringMarshaller.TryMarshalEmptyOrNullString(fullPathToLibBurstGenerated, ref managedSpanWrapper))
				{
					ReadOnlySpan<char> readOnlySpan = fullPathToLibBurstGenerated.AsSpan();
					fixed (char* ptr = readOnlySpan.GetPinnableReference())
					{
						managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
					}
				}
				flag = BurstCompilerService.LoadBurstLibrary_Injected(ref managedSpanWrapper);
			}
			finally
			{
				char* ptr = null;
			}
			return flag;
		}

		public static void Initialize(string folderRuntime, BurstCompilerService.ExtractCompilerFlags extractCompilerFlags)
		{
			bool flag = folderRuntime == null;
			if (flag)
			{
				throw new ArgumentNullException("folderRuntime");
			}
			bool flag2 = extractCompilerFlags == null;
			if (flag2)
			{
				throw new ArgumentNullException("extractCompilerFlags");
			}
			bool flag3 = !Directory.Exists(folderRuntime);
			if (flag3)
			{
				Debug.LogError("Unable to initialize the burst JIT compiler. The folder `" + folderRuntime + "` does not exist");
			}
			else
			{
				string text = BurstCompilerService.InitializeInternal(folderRuntime, extractCompilerFlags);
				bool flag4 = !string.IsNullOrEmpty(text);
				if (flag4)
				{
					Debug.LogError("Unexpected error while trying to initialize the burst JIT compiler: " + text);
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void InitializeInternal_Injected(ref ManagedSpanWrapper path, BurstCompilerService.ExtractCompilerFlags extractCompilerFlags, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetDisassembly_Injected(MethodInfo m, ref ManagedSpanWrapper compilerOptions, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int CompileAsyncDelegateMethod_Injected(object delegateMethod, ref ManagedSpanWrapper compilerOptions);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetMethodSignature_Injected(MethodInfo method, out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool LoadBurstLibrary_Injected(ref ManagedSpanWrapper fullPathToLibBurstGenerated);

		public delegate bool ExtractCompilerFlags(Type jobType, out string flags);

		public enum BurstLogType
		{
			Info,
			Warning,
			Error
		}
	}
}
