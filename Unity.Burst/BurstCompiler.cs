using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using AOT;
using Unity.Burst.LowLevel;
using UnityEngine;
using UnityEngine.Scripting;

namespace Unity.Burst
{
	public static class BurstCompiler
	{
		public static bool IsLoadAdditionalLibrarySupported()
		{
			return BurstCompiler.IsApiAvailable("LoadBurstLibrary");
		}

		private static BurstCompiler.CommandBuilder BeginCompilerCommand(string cmd)
		{
			if (BurstCompiler._cmdBuilder == null)
			{
				BurstCompiler._cmdBuilder = new BurstCompiler.CommandBuilder();
			}
			return BurstCompiler._cmdBuilder.Begin(cmd);
		}

		public static bool IsEnabled
		{
			get
			{
				return BurstCompiler._IsEnabled && BurstCompiler.BurstCompilerHelper.IsBurstGenerated;
			}
		}

		public static void SetExecutionMode(BurstExecutionEnvironment mode)
		{
			BurstCompilerService.SetCurrentExecutionMode((uint)mode);
		}

		public static BurstExecutionEnvironment GetExecutionMode()
		{
			return (BurstExecutionEnvironment)BurstCompilerService.GetCurrentExecutionMode();
		}

		internal static T CompileDelegate<T>(T delegateMethod, bool deterministicCompilation = false) where T : class
		{
			return (T)((object)Marshal.GetDelegateForFunctionPointer((IntPtr)BurstCompiler.Compile(delegateMethod, false, deterministicCompilation), delegateMethod.GetType()));
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void VerifyDelegateIsNotMulticast<T>(T delegateMethod) where T : class
		{
			if ((delegateMethod as Delegate).GetInvocationList().Length > 1)
			{
				throw new InvalidOperationException(string.Format("Burst does not support multicast delegates, please use a regular delegate for `{0}'", delegateMethod));
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void VerifyDelegateHasCorrectUnmanagedFunctionPointerAttribute<T>(T delegateMethod) where T : class
		{
			UnmanagedFunctionPointerAttribute customAttribute = delegateMethod.GetType().GetCustomAttribute<UnmanagedFunctionPointerAttribute>();
			if (customAttribute == null || customAttribute.CallingConvention != CallingConvention.Cdecl)
			{
				global::UnityEngine.Debug.LogWarning("The delegate type " + delegateMethod.GetType().FullName + " should be decorated with [UnmanagedFunctionPointer(CallingConvention.Cdecl)] to ensure runtime interoperabilty between managed code and Burst-compiled code.");
			}
		}

		[Obsolete("This method will be removed in a future version of Burst")]
		public static IntPtr CompileILPPMethod(RuntimeMethodHandle burstMethodHandle, RuntimeMethodHandle managedMethodHandle, RuntimeTypeHandle delegateTypeHandle)
		{
			throw new NotImplementedException();
		}

		public static IntPtr CompileILPPMethod2(RuntimeMethodHandle burstMethodHandle)
		{
			if (burstMethodHandle.Value == IntPtr.Zero)
			{
				throw new ArgumentNullException("burstMethodHandle");
			}
			Action onCompileILPPMethod = BurstCompiler.OnCompileILPPMethod2;
			if (onCompileILPPMethod != null)
			{
				onCompileILPPMethod();
			}
			MethodInfo methodInfo = (MethodInfo)MethodBase.GetMethodFromHandle(burstMethodHandle);
			return (IntPtr)BurstCompiler.Compile(new BurstCompiler.FakeDelegate(methodInfo), methodInfo, true, true, false);
		}

		[Obsolete("This method will be removed in a future version of Burst")]
		public unsafe static void* GetILPPMethodFunctionPointer(IntPtr ilppMethod)
		{
			throw new NotImplementedException();
		}

		public unsafe static void* GetILPPMethodFunctionPointer2(IntPtr ilppMethod, RuntimeMethodHandle managedMethodHandle, RuntimeTypeHandle delegateTypeHandle)
		{
			BurstCompiler.<>c__DisplayClass17_0 CS$<>8__locals1;
			CS$<>8__locals1.managedMethodHandle = managedMethodHandle;
			CS$<>8__locals1.delegateTypeHandle = delegateTypeHandle;
			if (CS$<>8__locals1.managedMethodHandle.Value == IntPtr.Zero)
			{
				throw new ArgumentNullException("managedMethodHandle");
			}
			if (CS$<>8__locals1.delegateTypeHandle.Value == IntPtr.Zero)
			{
				throw new ArgumentNullException("delegateTypeHandle");
			}
			if (ilppMethod == IntPtr.Zero)
			{
				Delegate @delegate;
				GCHandle gchandle;
				BurstCompiler.<GetILPPMethodFunctionPointer2>g__GetManagedFallbackDelegate|17_0(out @delegate, out gchandle, ref CS$<>8__locals1);
				return (void*)Marshal.GetFunctionPointerForDelegate(@delegate);
			}
			return ilppMethod.ToPointer();
		}

		[Obsolete("This method will be removed in a future version of Burst")]
		public unsafe static void* CompileUnsafeStaticMethod(RuntimeMethodHandle handle)
		{
			throw new NotImplementedException();
		}

		public static FunctionPointer<T> CompileFunctionPointer<T>(T delegateMethod) where T : class
		{
			return new FunctionPointer<T>(new IntPtr(BurstCompiler.Compile(delegateMethod, true, false)));
		}

		private unsafe static void* Compile(object delegateObj, bool isFunctionPointer, bool deterministicCompilation = false)
		{
			if (!(delegateObj is Delegate))
			{
				throw new ArgumentException("object instance must be a System.Delegate", "delegateObj");
			}
			Delegate @delegate = (Delegate)delegateObj;
			return BurstCompiler.Compile(@delegate, @delegate.Method, isFunctionPointer, false, deterministicCompilation);
		}

		private unsafe static void* Compile(object delegateObj, MethodInfo methodInfo, bool isFunctionPointer, bool isILPostProcessing, bool deterministicCompilation = false)
		{
			if (delegateObj == null)
			{
				throw new ArgumentNullException("delegateObj");
			}
			if (delegateObj.GetType().IsGenericType)
			{
				throw new InvalidOperationException(string.Format("The delegate type `{0}` must be a non-generic type", delegateObj.GetType()));
			}
			if (!methodInfo.IsStatic)
			{
				throw new InvalidOperationException(string.Format("The method `{0}` must be static. Instance methods are not supported", methodInfo));
			}
			if (methodInfo.IsGenericMethod)
			{
				throw new InvalidOperationException(string.Format("The method `{0}` must be a non-generic method", methodInfo));
			}
			Delegate @delegate = null;
			if (!isILPostProcessing)
			{
				@delegate = delegateObj as Delegate;
			}
			Delegate delegate2 = delegateObj as Delegate;
			if (!BurstCompilerOptions.HasBurstCompileAttribute(methodInfo))
			{
				throw new InvalidOperationException(string.Format("Burst cannot compile the function pointer `{0}` because the `[BurstCompile]` attribute is missing", methodInfo));
			}
			void* ptr = null;
			if (BurstCompiler.Options.EnableBurstCompilation && BurstCompiler.BurstCompilerHelper.IsBurstGenerated)
			{
				if (isFunctionPointer && methodInfo.Name.EndsWith("$BurstManaged"))
				{
					delegateObj = methodInfo.DeclaringType.GetMethod(methodInfo.Name.Replace("$BurstManaged", ""), BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).CreateDelegate(delegate2.GetType());
				}
				ptr = BurstCompilerService.GetAsyncCompiledAsyncDelegateMethod(BurstCompilerService.CompileAsyncDelegateMethod(delegateObj, string.Empty));
			}
			if (ptr == null)
			{
				if (isILPostProcessing)
				{
					return null;
				}
				GCHandle.Alloc(@delegate);
				ptr = (void*)Marshal.GetFunctionPointerForDelegate(@delegate);
			}
			if (ptr == null)
			{
				throw new InvalidOperationException(string.Format("Burst failed to compile the function pointer `{0}`", methodInfo));
			}
			return ptr;
		}

		internal static void Shutdown()
		{
		}

		internal static void Cancel()
		{
		}

		internal static bool IsCurrentCompilationDone()
		{
			return true;
		}

		internal static void Enable()
		{
		}

		internal static void Disable()
		{
		}

		internal static bool IsHostEditorArm()
		{
			return false;
		}

		internal static void TriggerUnsafeStaticMethodRecompilation()
		{
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = 0; i < assemblies.Length; i++)
			{
				foreach (Attribute attribute in from x in assemblies[i].GetCustomAttributes()
					where x.GetType().FullName == "Unity.Burst.BurstCompiler+StaticTypeReinitAttribute"
					select x)
				{
					(attribute as BurstCompiler.StaticTypeReinitAttribute).reinitType.GetMethod("Constructor", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[0]);
				}
			}
		}

		internal static void TriggerRecompilation()
		{
		}

		internal static void UnloadAdditionalLibraries()
		{
			BurstCompiler.SendCommandToCompiler("$unload_burst_natives", null);
		}

		internal static bool IsApiAvailable(string apiName)
		{
			return BurstCompiler.SendCommandToCompiler("$is_native_api_available", apiName) == "True";
		}

		internal static int RequestSetProtocolVersion(int version)
		{
			string text = BurstCompiler.SendCommandToCompiler("$request_set_protocol_version_editor", string.Format("{0}", version));
			int num;
			if (string.IsNullOrEmpty(text) || !int.TryParse(text, out num))
			{
				num = 0;
			}
			BurstCompiler.SendCommandToCompiler("$set_protocol_version_burst", string.Format("{0}", num));
			return num;
		}

		internal static void Initialize(string[] assemblyFolders, string[] ignoreAssemblies)
		{
		}

		internal static void NotifyCompilationStarted(string[] assemblyFolders, string[] ignoreAssemblies)
		{
		}

		internal static void NotifyAssemblyCompilationNotRequired(string assemblyName)
		{
		}

		internal static void NotifyAssemblyCompilationFinished(string assemblyName, string[] defines)
		{
		}

		internal static void NotifyCompilationFinished()
		{
		}

		internal static string AotCompilation(string[] assemblyFolders, string[] assemblyRoots, string options)
		{
			return "failed";
		}

		internal static void SetProfilerCallbacks()
		{
		}

		private static string SendRawCommandToCompiler(string command)
		{
			string disassembly = BurstCompilerService.GetDisassembly(BurstCompiler.DummyMethodInfo, command);
			if (!string.IsNullOrEmpty(disassembly))
			{
				return disassembly.TrimStart('\n');
			}
			return "";
		}

		private static string SendCommandToCompiler(string commandName, string commandArgs = null)
		{
			if (commandName == null)
			{
				throw new ArgumentNullException("commandName");
			}
			if (commandArgs == null)
			{
				return BurstCompiler.SendRawCommandToCompiler(commandName);
			}
			return BurstCompiler.BeginCompilerCommand(commandName).With(commandArgs).SendToCompiler();
		}

		private static void DummyMethod()
		{
		}

		[CompilerGenerated]
		internal static void <GetILPPMethodFunctionPointer2>g__GetManagedFallbackDelegate|17_0(out Delegate managedFallbackDelegate, out GCHandle gcHandle, ref BurstCompiler.<>c__DisplayClass17_0 A_2)
		{
			MethodInfo methodInfo = (MethodInfo)MethodBase.GetMethodFromHandle(A_2.managedMethodHandle);
			Type typeFromHandle = Type.GetTypeFromHandle(A_2.delegateTypeHandle);
			managedFallbackDelegate = Delegate.CreateDelegate(typeFromHandle, methodInfo);
			gcHandle = GCHandle.Alloc(managedFallbackDelegate);
		}

		[ThreadStatic]
		private static BurstCompiler.CommandBuilder _cmdBuilder;

		internal static bool _IsEnabled;

		public static readonly BurstCompilerOptions Options = new BurstCompilerOptions(true);

		internal static Action OnCompileILPPMethod2;

		private static readonly MethodInfo DummyMethodInfo = typeof(BurstCompiler).GetMethod("DummyMethod", BindingFlags.Static | BindingFlags.NonPublic);

		private class CommandBuilder
		{
			public CommandBuilder()
			{
				this._builder = new StringBuilder();
				this._hasArgs = false;
			}

			public BurstCompiler.CommandBuilder Begin(string cmd)
			{
				this._builder.Clear();
				this._hasArgs = false;
				this._builder.Append(cmd);
				return this;
			}

			public BurstCompiler.CommandBuilder With(string arg)
			{
				if (!this._hasArgs)
				{
					this._builder.Append(' ');
				}
				this._hasArgs = true;
				this._builder.Append(arg);
				return this;
			}

			public BurstCompiler.CommandBuilder With(IntPtr arg)
			{
				if (!this._hasArgs)
				{
					this._builder.Append(' ');
				}
				this._hasArgs = true;
				this._builder.AppendFormat("0x{0:X16}", arg.ToInt64());
				return this;
			}

			public BurstCompiler.CommandBuilder And(char sep = '|')
			{
				this._builder.Append(sep);
				return this;
			}

			public string SendToCompiler()
			{
				return BurstCompiler.SendRawCommandToCompiler(this._builder.ToString());
			}

			private StringBuilder _builder;

			private bool _hasArgs;
		}

		[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
		internal class StaticTypeReinitAttribute : Attribute
		{
			public StaticTypeReinitAttribute(Type toReinit)
			{
				this.reinitType = toReinit;
			}

			public readonly Type reinitType;
		}

		[BurstCompile]
		internal static class BurstCompilerHelper
		{
			[BurstCompile]
			[MonoPInvokeCallback(typeof(BurstCompiler.BurstCompilerHelper.IsBurstEnabledDelegate))]
			private static bool IsBurstEnabled()
			{
				return BurstCompiler.BurstCompilerHelper.IsBurstEnabled_00000145$BurstDirectCall.Invoke();
			}

			[BurstDiscard]
			private static void DiscardedMethod(ref bool value)
			{
				value = false;
			}

			private static bool IsCompiledByBurst(Delegate del)
			{
				return BurstCompilerService.GetAsyncCompiledAsyncDelegateMethod(BurstCompilerService.CompileAsyncDelegateMethod(del, string.Empty)) != null;
			}

			[BurstCompile]
			[MonoPInvokeCallback(typeof(BurstCompiler.BurstCompilerHelper.IsBurstEnabledDelegate))]
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal static bool IsBurstEnabled$BurstManaged()
			{
				bool flag = true;
				BurstCompiler.BurstCompilerHelper.DiscardedMethod(ref flag);
				return flag;
			}

			private static readonly BurstCompiler.BurstCompilerHelper.IsBurstEnabledDelegate IsBurstEnabledImpl = new BurstCompiler.BurstCompilerHelper.IsBurstEnabledDelegate(BurstCompiler.BurstCompilerHelper.IsBurstEnabled);

			public static readonly bool IsBurstGenerated = BurstCompiler.BurstCompilerHelper.IsCompiledByBurst(BurstCompiler.BurstCompilerHelper.IsBurstEnabledImpl);

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private delegate bool IsBurstEnabledDelegate();

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			internal delegate bool IsBurstEnabled_00000145$PostfixBurstDelegate();

			internal static class IsBurstEnabled_00000145$BurstDirectCall
			{
				[BurstDiscard]
				private static void GetFunctionPointerDiscard(ref IntPtr A_0)
				{
					if (BurstCompiler.BurstCompilerHelper.IsBurstEnabled_00000145$BurstDirectCall.Pointer == 0)
					{
						BurstCompiler.BurstCompilerHelper.IsBurstEnabled_00000145$BurstDirectCall.Pointer = BurstCompiler.CompileFunctionPointer<BurstCompiler.BurstCompilerHelper.IsBurstEnabled_00000145$PostfixBurstDelegate>(new BurstCompiler.BurstCompilerHelper.IsBurstEnabled_00000145$PostfixBurstDelegate(BurstCompiler.BurstCompilerHelper.IsBurstEnabled)).Value;
					}
					A_0 = BurstCompiler.BurstCompilerHelper.IsBurstEnabled_00000145$BurstDirectCall.Pointer;
				}

				private static IntPtr GetFunctionPointer()
				{
					IntPtr intPtr = (IntPtr)0;
					BurstCompiler.BurstCompilerHelper.IsBurstEnabled_00000145$BurstDirectCall.GetFunctionPointerDiscard(ref intPtr);
					return intPtr;
				}

				public static bool Invoke()
				{
					if (BurstCompiler.IsEnabled)
					{
						IntPtr functionPointer = BurstCompiler.BurstCompilerHelper.IsBurstEnabled_00000145$BurstDirectCall.GetFunctionPointer();
						if (functionPointer != 0)
						{
							return calli(System.Boolean(), functionPointer);
						}
					}
					return BurstCompiler.BurstCompilerHelper.IsBurstEnabled$BurstManaged();
				}

				private static IntPtr Pointer;
			}
		}

		private class FakeDelegate
		{
			public FakeDelegate(MethodInfo method)
			{
				this.Method = method;
			}

			[Preserve]
			public MethodInfo Method { get; }
		}
	}
}
