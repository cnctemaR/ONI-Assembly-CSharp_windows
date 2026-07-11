using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Bindings;

namespace Unity.Burst.LowLevel
{
	[StaticAccessor("BurstCompilerService::Get()", StaticAccessorType.Arrow)]
	[NativeHeader("Runtime/Burst/BurstDelegateCache.h")]
	[NativeHeader("Runtime/Burst/Burst.h")]
	internal static class BurstCompilerService
	{
		[NativeMethod("Initialize")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string InitializeInternal(string path, BurstCompilerService.ExtractCompilerFlags extractCompilerFlags);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetDisassembly(MethodInfo m, string compilerOptions);

		[FreeFunction]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int CompileAsyncDelegateMethod(object delegateMethod, string compilerOptions);

		[FreeFunction]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public unsafe static extern void* GetAsyncCompiledAsyncDelegateMethod(int userID);

		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetMethodSignature(MethodInfo method);

		public static extern bool IsInitialized
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		public static void Initialize(string folderRuntime, BurstCompilerService.ExtractCompilerFlags extractCompilerFlags)
		{
			if (folderRuntime == null)
			{
				throw new ArgumentNullException("folderRuntime");
			}
			if (extractCompilerFlags == null)
			{
				throw new ArgumentNullException("extractCompilerFlags");
			}
			if (!Directory.Exists(folderRuntime))
			{
				Debug.LogError(string.Format("Unable to initialize the burst JIT compiler. The folder `{0}` does not exist", folderRuntime));
			}
			else
			{
				string text = BurstCompilerService.InitializeInternal(folderRuntime, extractCompilerFlags);
				if (!string.IsNullOrEmpty(text))
				{
					Debug.LogError(string.Format("Unexpected error while trying to initialize the burst JIT compiler: {0}", text));
				}
			}
		}

		public delegate bool ExtractCompilerFlags(Type jobType, out string flags);
	}
}
