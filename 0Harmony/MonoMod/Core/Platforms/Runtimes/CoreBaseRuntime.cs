using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using MonoMod.Core.Utils;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Runtimes
{
	[NullableContext(1)]
	[Nullable(0)]
	internal abstract class CoreBaseRuntime : FxCoreBaseRuntime, IInitialize
	{
		public static CoreBaseRuntime CreateForVersion(Version version, ISystem system, IArchitecture arch)
		{
			switch (version.Major)
			{
			case 2:
			case 4:
				return new Core21Runtime(system);
			case 3:
			{
				int minor = version.Minor;
				Core30Runtime core30Runtime;
				if (minor != 0)
				{
					if (minor != 1)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
						defaultInterpolatedStringHandler.AppendLiteral("Unknown .NET Core 3.x minor version ");
						defaultInterpolatedStringHandler.AppendFormatted<int>(version.Minor);
						throw new PlatformNotSupportedException(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					core30Runtime = new Core31Runtime(system);
				}
				else
				{
					core30Runtime = new Core30Runtime(system);
				}
				return core30Runtime;
			}
			case 5:
				return new Core50Runtime(system);
			case 6:
				return new Core60Runtime(system, arch);
			case 7:
				return new Core70Runtime(system, arch);
			case 8:
				return new Core80Runtime(system, arch);
			case 9:
				return new Core90Runtime(system, arch);
			case 10:
				return new Core100Runtime(system, arch);
			default:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(33, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("CoreCLR version ");
				defaultInterpolatedStringHandler2.AppendFormatted<Version>(version);
				defaultInterpolatedStringHandler2.AppendLiteral(" is not supported");
				throw new PlatformNotSupportedException(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			}
		}

		public override RuntimeKind Target
		{
			get
			{
				return RuntimeKind.CoreCLR;
			}
		}

		protected ISystem System { get; }

		protected CoreBaseRuntime(ISystem system)
		{
			this.System = system;
			Abi? defaultAbi = system.DefaultAbi;
			if (defaultAbi != null)
			{
				Abi valueOrDefault = defaultAbi.GetValueOrDefault();
				if (PlatformDetection.Architecture == ArchitectureKind.x86_64)
				{
					this.AbiCore = new Abi?(FxCoreBaseRuntime.AbiForCoreFx45X64(valueOrDefault));
					return;
				}
				if (PlatformDetection.Architecture == ArchitectureKind.Arm64)
				{
					this.AbiCore = new Abi?(FxCoreBaseRuntime.AbiForCoreFx45ARM64(valueOrDefault));
				}
			}
		}

		void IInitialize.Initialize()
		{
			this.InstallJitHook(this.JitObject);
		}

		private static bool IsMaybeClrJitPath(string path)
		{
			return Path.GetFileNameWithoutExtension(path).EndsWith("clrjit", StringComparison.Ordinal);
		}

		protected virtual string GetClrJitPath()
		{
			string text = null;
			object obj;
			bool flag;
			if (Switches.TryGetSwitchValue("JitPath", out obj))
			{
				string jitPath = obj as string;
				if (jitPath != null)
				{
					if (!CoreBaseRuntime.IsMaybeClrJitPath(jitPath))
					{
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(77, 1, out flag);
						if (flag)
						{
							debugLogWarningStringHandler.AppendLiteral("Provided value for MonoMod.JitPath switch '");
							debugLogWarningStringHandler.AppendFormatted(jitPath);
							debugLogWarningStringHandler.AppendLiteral("' does not look like a ClrJIT path");
						}
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler);
					}
					else
					{
						text = this.System.EnumerateLoadedModuleFiles().FirstOrDefault<string>((string f) => f != null && f == jitPath);
						if (text == null)
						{
							<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler2 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(82, 1, out flag);
							if (flag)
							{
								debugLogWarningStringHandler2.AppendLiteral("Provided path for MonoMod.JitPath switch was not loaded in this process. jitPath: ");
								debugLogWarningStringHandler2.AppendFormatted(jitPath);
							}
							<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler2);
						}
					}
				}
			}
			if (text == null)
			{
				text = this.System.EnumerateLoadedModuleFiles().FirstOrDefault<string>((string f) => f != null && CoreBaseRuntime.IsMaybeClrJitPath(f));
			}
			if (text == null)
			{
				throw new PlatformNotSupportedException("Could not locate clrjit library");
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(14, 1, out flag);
			if (flag)
			{
				debugLogTraceStringHandler.AppendLiteral("Got jit path: ");
				debugLogTraceStringHandler.AppendFormatted(text);
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
			return text;
		}

		protected IntPtr JitObject
		{
			get
			{
				IntPtr intPtr = this.lazyJitObject.GetValueOrDefault();
				if (this.lazyJitObject == null)
				{
					intPtr = this.GetJitObject();
					this.lazyJitObject = new IntPtr?(intPtr);
					return intPtr;
				}
				return intPtr;
			}
		}

		private unsafe IntPtr GetJitObject()
		{
			IntPtr intPtr;
			if (!DynDll.TryOpenLibrary(this.GetClrJitPath(), out intPtr))
			{
				throw new PlatformNotSupportedException("Could not open clrjit library");
			}
			IntPtr intPtr2;
			try
			{
				intPtr2 = calli(System.IntPtr(), (void*)intPtr.GetExport("getJit"));
			}
			catch
			{
				DynDll.CloseLibrary(intPtr);
				throw;
			}
			return intPtr2;
		}

		protected virtual void InstallJitHook(IntPtr jit)
		{
			this.InstallManagedJitHook(jit);
		}

		protected abstract void InstallManagedJitHook(IntPtr jit);

		[Nullable(2)]
		protected INativeExceptionHelper NativeExceptionHelper
		{
			[NullableContext(2)]
			get
			{
				INativeExceptionHelper nativeExceptionHelper;
				if ((nativeExceptionHelper = this.lazyNativeExceptionHelper) == null)
				{
					nativeExceptionHelper = (this.lazyNativeExceptionHelper = this.System.NativeExceptionHelper);
				}
				return nativeExceptionHelper;
			}
		}

		[NullableContext(2)]
		protected IntPtr EHNativeToManaged(IntPtr target, out IDisposable handle)
		{
			if (this.NativeExceptionHelper != null)
			{
				return this.NativeExceptionHelper.CreateNativeToManagedHelper(target, out handle);
			}
			handle = null;
			return target;
		}

		[NullableContext(2)]
		protected IntPtr EHManagedToNative(IntPtr target, out IDisposable handle)
		{
			if (this.NativeExceptionHelper != null)
			{
				return this.NativeExceptionHelper.CreateManagedToNativeHelper(target, out handle);
			}
			handle = null;
			return target;
		}

		private IntPtr? lazyJitObject;

		[Nullable(2)]
		private INativeExceptionHelper lazyNativeExceptionHelper;
	}
}
