using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod.Core.Platforms.Architectures;
using MonoMod.Core.Platforms.Runtimes;
using MonoMod.Core.Platforms.Systems;
using MonoMod.Core.Utils;
using MonoMod.Logs;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class PlatformTriple
	{
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static IRuntime CreateCurrentRuntime(ISystem system, IArchitecture arch)
		{
			Helpers.ThrowIfArgumentNull<ISystem>(system, "system");
			Helpers.ThrowIfArgumentNull<IArchitecture>(arch, "arch");
			RuntimeKind runtime = PlatformDetection.Runtime;
			IRuntime runtime2;
			switch (runtime)
			{
			case RuntimeKind.Framework:
				runtime2 = FxBaseRuntime.CreateForVersion(PlatformDetection.RuntimeVersion, system);
				break;
			case RuntimeKind.CoreCLR:
				runtime2 = CoreBaseRuntime.CreateForVersion(PlatformDetection.RuntimeVersion, system, arch);
				break;
			case RuntimeKind.Mono:
				runtime2 = new MonoRuntime(system);
				break;
			default:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Runtime kind ");
				defaultInterpolatedStringHandler.AppendFormatted<RuntimeKind>(runtime);
				defaultInterpolatedStringHandler.AppendLiteral(" not supported");
				throw new PlatformNotSupportedException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			}
			return runtime2;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static IArchitecture CreateCurrentArchitecture(ISystem system)
		{
			Helpers.ThrowIfArgumentNull<ISystem>(system, "system");
			ArchitectureKind architecture = PlatformDetection.Architecture;
			IArchitecture architecture2;
			switch (architecture)
			{
			case ArchitectureKind.x86:
				architecture2 = new x86Arch(system);
				break;
			case ArchitectureKind.x86_64:
				architecture2 = new x86_64Arch(system);
				break;
			case ArchitectureKind.Arm:
				throw new NotImplementedException();
			case ArchitectureKind.Arm64:
				architecture2 = new Arm64Arch(system);
				break;
			default:
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Architecture kind ");
				defaultInterpolatedStringHandler.AppendFormatted<ArchitectureKind>(architecture);
				defaultInterpolatedStringHandler.AppendLiteral(" not supported");
				throw new PlatformNotSupportedException(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			}
			return architecture2;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static ISystem CreateCurrentSystem()
		{
			OSKind os = PlatformDetection.OS;
			if (os <= OSKind.BSD)
			{
				switch (os)
				{
				case OSKind.Posix:
					throw new NotImplementedException();
				case OSKind.Windows:
					break;
				case (OSKind)3:
				case (OSKind)4:
					goto IL_0074;
				case OSKind.OSX:
					return new MacOSSystem();
				default:
					if (os == OSKind.Linux)
					{
						return new LinuxSystem();
					}
					if (os != OSKind.BSD)
					{
						goto IL_0074;
					}
					throw new NotImplementedException();
				}
			}
			else if (os != OSKind.Wine)
			{
				if (os == OSKind.IOS)
				{
					throw new NotImplementedException();
				}
				if (os != OSKind.Android)
				{
					goto IL_0074;
				}
				throw new NotImplementedException();
			}
			return new WindowsSystem();
			IL_0074:
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 1);
			defaultInterpolatedStringHandler.AppendLiteral("OS kind ");
			defaultInterpolatedStringHandler.AppendFormatted<OSKind>(os);
			defaultInterpolatedStringHandler.AppendLiteral(" not supported");
			throw new PlatformNotSupportedException(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		public IArchitecture Architecture { get; }

		public ISystem System { get; }

		public IRuntime Runtime { get; }

		public static PlatformTriple Current
		{
			get
			{
				return Helpers.GetOrInitWithLock<PlatformTriple>(ref PlatformTriple.lazyCurrent, PlatformTriple.lazyCurrentLock, PlatformTriple.createCurrentFunc);
			}
		}

		private static PlatformTriple CreateCurrent()
		{
			ISystem system = PlatformTriple.CreateCurrentSystem();
			IArchitecture architecture = PlatformTriple.CreateCurrentArchitecture(system);
			IRuntime runtime = PlatformTriple.CreateCurrentRuntime(system, architecture);
			return new PlatformTriple(architecture, system, runtime);
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public static void SetPlatformTriple(PlatformTriple triple)
		{
			Helpers.ThrowIfArgumentNull<PlatformTriple>(triple, "triple");
			if (PlatformTriple.lazyCurrent == null)
			{
				PlatformTriple.ThrowTripleAlreadyExists();
			}
			object obj = PlatformTriple.lazyCurrentLock;
			lock (obj)
			{
				if (PlatformTriple.lazyCurrent == null)
				{
					PlatformTriple.ThrowTripleAlreadyExists();
				}
				PlatformTriple.lazyCurrent = triple;
			}
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ThrowTripleAlreadyExists()
		{
			throw new InvalidOperationException("The platform triple has already been initialized; cannot set a new one");
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public PlatformTriple(IArchitecture architecture, ISystem system, IRuntime runtime)
		{
			Helpers.ThrowIfArgumentNull<IArchitecture>(architecture, "architecture");
			Helpers.ThrowIfArgumentNull<ISystem>(system, "system");
			Helpers.ThrowIfArgumentNull<IRuntime>(runtime, "runtime");
			this.Architecture = architecture;
			this.System = system;
			this.Runtime = runtime;
			this.SupportedFeatures = new FeatureFlags(this.Architecture.Features, this.System.Features, this.Runtime.Features);
			this.InitIfNeeded(this.Architecture);
			this.InitIfNeeded(this.System);
			this.InitIfNeeded(this.Runtime);
			this.Abi = this.Runtime.Abi;
		}

		private void InitIfNeeded(object obj)
		{
			IInitialize<ISystem> initialize = obj as IInitialize<ISystem>;
			if (initialize != null)
			{
				initialize.Initialize(this.System);
			}
			IInitialize<IArchitecture> initialize2 = obj as IInitialize<IArchitecture>;
			if (initialize2 != null)
			{
				initialize2.Initialize(this.Architecture);
			}
			IInitialize<IRuntime> initialize3 = obj as IInitialize<IRuntime>;
			if (initialize3 != null)
			{
				initialize3.Initialize(this.Runtime);
			}
			IInitialize<PlatformTriple> initialize4 = obj as IInitialize<PlatformTriple>;
			if (initialize4 != null)
			{
				initialize4.Initialize(this);
			}
			IInitialize initialize5 = obj as IInitialize;
			if (initialize5 == null)
			{
				return;
			}
			initialize5.Initialize();
		}

		[TupleElementNames(new string[] { "Arch", "OS", "Runtime" })]
		[Nullable(0)]
		public ValueTuple<ArchitectureKind, OSKind, RuntimeKind> HostTriple
		{
			[NullableContext(0)]
			[return: TupleElementNames(new string[] { "Arch", "OS", "Runtime" })]
			get
			{
				return new ValueTuple<ArchitectureKind, OSKind, RuntimeKind>(this.Architecture.Target, this.System.Target, this.Runtime.Target);
			}
		}

		public FeatureFlags SupportedFeatures { get; }

		public Abi Abi { get; }

		public void Compile(MethodBase method)
		{
			Helpers.ThrowIfArgumentNull<MethodBase>(method, "method");
			if (method.IsGenericMethodDefinition)
			{
				throw new ArgumentException("Cannot prepare generic method definition", "method");
			}
			method = this.GetIdentifiable(method);
			if (this.SupportedFeatures.Has(RuntimeFeature.RequiresCustomMethodCompile))
			{
				this.Runtime.Compile(method);
				return;
			}
			RuntimeMethodHandle methodHandle = this.Runtime.GetMethodHandle(method);
			if (method.IsGenericMethod)
			{
				Type[] genericArguments = method.GetGenericArguments();
				RuntimeTypeHandle[] array = new RuntimeTypeHandle[genericArguments.Length];
				for (int i = 0; i < genericArguments.Length; i++)
				{
					array[i] = genericArguments[i].TypeHandle;
				}
				RuntimeHelpers.PrepareMethod(methodHandle, array);
				return;
			}
			RuntimeHelpers.PrepareMethod(methodHandle);
		}

		public MethodBase GetIdentifiable(MethodBase method)
		{
			Helpers.ThrowIfArgumentNull<MethodBase>(method, "method");
			if (this.SupportedFeatures.Has(RuntimeFeature.RequiresMethodIdentification))
			{
				method = this.Runtime.GetIdentifiable(method);
			}
			if (method.ReflectedType != method.DeclaringType)
			{
				ParameterInfo[] parameters = method.GetParameters();
				Type[] array = new Type[parameters.Length];
				for (int i = 0; i < parameters.Length; i++)
				{
					array[i] = parameters[i].ParameterType;
				}
				if (method.DeclaringType == null)
				{
					MethodInfo method2 = method.Module.GetMethod(method.Name, (BindingFlags)(-1), null, method.CallingConvention, array, null);
					bool flag = method2 != null;
					bool flag2 = flag;
					bool flag3;
					AssertionInterpolatedStringHandler assertionInterpolatedStringHandler = new AssertionInterpolatedStringHandler(16, 2, flag, out flag3);
					if (flag3)
					{
						assertionInterpolatedStringHandler.AppendLiteral("orig: ");
						assertionInterpolatedStringHandler.AppendFormatted<MethodBase>(method);
						assertionInterpolatedStringHandler.AppendLiteral(", module: ");
						assertionInterpolatedStringHandler.AppendFormatted<Module>(method.Module);
					}
					Helpers.Assert(flag2, ref assertionInterpolatedStringHandler, "got is not null");
					method = method2;
				}
				else if (method.IsConstructor)
				{
					ConstructorInfo constructor = method.DeclaringType.GetConstructor((BindingFlags)(-1), null, method.CallingConvention, array, null);
					bool flag3 = constructor != null;
					bool flag4 = flag3;
					bool flag;
					AssertionInterpolatedStringHandler assertionInterpolatedStringHandler2 = new AssertionInterpolatedStringHandler(6, 1, flag3, out flag);
					if (flag)
					{
						assertionInterpolatedStringHandler2.AppendLiteral("orig: ");
						assertionInterpolatedStringHandler2.AppendFormatted<MethodBase>(method);
					}
					Helpers.Assert(flag4, ref assertionInterpolatedStringHandler2, "got is not null");
					method = constructor;
				}
				else
				{
					MethodInfo method3 = method.DeclaringType.GetMethod(method.Name, (BindingFlags)(-1), null, method.CallingConvention, array, null);
					bool flag = method3 != null;
					bool flag5 = flag;
					bool flag3;
					AssertionInterpolatedStringHandler assertionInterpolatedStringHandler3 = new AssertionInterpolatedStringHandler(6, 1, flag, out flag3);
					if (flag3)
					{
						assertionInterpolatedStringHandler3.AppendLiteral("orig: ");
						assertionInterpolatedStringHandler3.AppendFormatted<MethodBase>(method);
					}
					Helpers.Assert(flag5, ref assertionInterpolatedStringHandler3, "got is not null");
					method = method3;
				}
			}
			return method;
		}

		[return: Nullable(2)]
		public IDisposable PinMethodIfNeeded(MethodBase method)
		{
			if (this.SupportedFeatures.Has(RuntimeFeature.RequiresMethodPinning))
			{
				return this.Runtime.PinMethodIfNeeded(method);
			}
			return null;
		}

		public bool TryDisableInlining(MethodBase method)
		{
			if (this.SupportedFeatures.Has(RuntimeFeature.DisableInlining))
			{
				this.Runtime.DisableInlining(method);
				return true;
			}
			return false;
		}

		public unsafe SimpleNativeDetour CreateSimpleDetour(IntPtr from, IntPtr to, int detourMaxSize = -1, IntPtr fromRw = default(IntPtr))
		{
			if (fromRw == (IntPtr)0)
			{
				fromRw = from;
			}
			bool flag = from != to;
			bool flag2 = flag;
			bool flag3;
			AssertionInterpolatedStringHandler assertionInterpolatedStringHandler = new AssertionInterpolatedStringHandler(48, 2, flag, out flag3);
			if (flag3)
			{
				assertionInterpolatedStringHandler.AppendLiteral("Cannot detour a method to itself! (from: ");
				assertionInterpolatedStringHandler.AppendFormatted<IntPtr>(from);
				assertionInterpolatedStringHandler.AppendLiteral(", to: ");
				assertionInterpolatedStringHandler.AppendFormatted<IntPtr>(to);
				assertionInterpolatedStringHandler.AppendLiteral(")");
			}
			Helpers.Assert(flag2, ref assertionInterpolatedStringHandler, "from != to");
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(31, 2, out flag3);
			if (flag3)
			{
				debugLogTraceStringHandler.AppendLiteral("Creating simple detour 0x");
				debugLogTraceStringHandler.AppendFormatted<IntPtr>(from, "x16");
				debugLogTraceStringHandler.AppendLiteral(" => 0x");
				debugLogTraceStringHandler.AppendFormatted<IntPtr>(to, "x16");
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
			NativeDetourInfo nativeDetourInfo = this.Architecture.ComputeDetourInfo(from, to, detourMaxSize);
			int size = nativeDetourInfo.Size;
			global::System.Span<byte> span = new global::System.Span<byte>(stackalloc byte[(UIntPtr)size], size);
			IDisposable disposable;
			this.Architecture.GetDetourBytes(nativeDetourInfo, span, out disposable);
			byte[] array = new byte[nativeDetourInfo.Size];
			this.System.PatchData(PatchTargetKind.Executable, fromRw, span, array);
			return new SimpleNativeDetour(this, nativeDetourInfo, array, disposable);
		}

		public unsafe PlatformTriple.NativeDetour CreateNativeDetour(IntPtr from, IntPtr to, int detourMaxSize = -1, IntPtr fromRw = default(IntPtr))
		{
			if (fromRw == (IntPtr)0)
			{
				fromRw = from;
			}
			bool flag = from != to;
			bool flag2 = flag;
			bool flag3;
			AssertionInterpolatedStringHandler assertionInterpolatedStringHandler = new AssertionInterpolatedStringHandler(48, 2, flag, out flag3);
			if (flag3)
			{
				assertionInterpolatedStringHandler.AppendLiteral("Cannot detour a method to itself! (from: ");
				assertionInterpolatedStringHandler.AppendFormatted<IntPtr>(from);
				assertionInterpolatedStringHandler.AppendLiteral(", to: ");
				assertionInterpolatedStringHandler.AppendFormatted<IntPtr>(to);
				assertionInterpolatedStringHandler.AppendLiteral(")");
			}
			Helpers.Assert(flag2, ref assertionInterpolatedStringHandler, "from != to");
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(31, 2, out flag3);
			if (flag3)
			{
				debugLogTraceStringHandler.AppendLiteral("Creating simple detour 0x");
				debugLogTraceStringHandler.AppendFormatted<IntPtr>(from, "x16");
				debugLogTraceStringHandler.AppendLiteral(" => 0x");
				debugLogTraceStringHandler.AppendFormatted<IntPtr>(to, "x16");
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
			NativeDetourInfo nativeDetourInfo = this.Architecture.ComputeDetourInfo(from, to, detourMaxSize);
			int size = nativeDetourInfo.Size;
			global::System.Span<byte> span = new global::System.Span<byte>(stackalloc byte[(UIntPtr)size], size);
			IDisposable disposable;
			int detourBytes = this.Architecture.GetDetourBytes(nativeDetourInfo, span, out disposable);
			IntPtr intPtr = IntPtr.Zero;
			IDisposable disposable2 = null;
			if (this.SupportedFeatures.Has(ArchitectureFeature.CreateAltEntryPoint))
			{
				intPtr = this.Architecture.AltEntryFactory.CreateAlternateEntrypoint(from, detourBytes, out disposable2);
			}
			else
			{
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(67, 2, out flag3);
				if (flag3)
				{
					debugLogWarningStringHandler.AppendLiteral("Cannot create alternate entry point for native detour (from: ");
					debugLogWarningStringHandler.AppendFormatted<IntPtr>(from, "x16");
					debugLogWarningStringHandler.AppendLiteral(", to: ");
					debugLogWarningStringHandler.AppendFormatted<IntPtr>(to, "x16");
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler);
			}
			byte[] array = new byte[nativeDetourInfo.Size];
			this.System.PatchData(PatchTargetKind.Executable, fromRw, span, array);
			return new PlatformTriple.NativeDetour(new SimpleNativeDetour(this, nativeDetourInfo, array, disposable), intPtr, disposable2);
		}

		public IntPtr GetNativeMethodBody(MethodBase method)
		{
			if (this.SupportedFeatures.Has(RuntimeFeature.RequiresBodyThunkWalking))
			{
				return this.GetNativeMethodBodyWalk(method, true);
			}
			return this.GetNativeMethodBodyDirect(method);
		}

		private IntPtr GetNativeMethodBodyWalk(MethodBase method, bool reloadPtr)
		{
			bool flag = false;
			bool flag2 = false;
			int num = 0;
			BytePatternCollection knownMethodThunks = this.Architecture.KnownMethodThunks;
			bool flag3;
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(32, 1, out flag3);
			if (flag3)
			{
				debugLogTraceStringHandler.AppendLiteral("Performing method body walk for ");
				debugLogTraceStringHandler.AppendFormatted<MethodBase>(method);
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
			IntPtr intPtr = (IntPtr)(-1);
			IntPtr intPtr2;
			for (;;)
			{
				IL_0041:
				intPtr2 = this.Runtime.GetMethodEntryPoint(method);
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler2 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(25, 1, out flag3);
				if (flag3)
				{
					debugLogTraceStringHandler2.AppendLiteral("Starting entry point = 0x");
					debugLogTraceStringHandler2.AppendFormatted<IntPtr>(intPtr2, "x16");
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler2);
				while (num++ <= 20)
				{
					if (!flag2 && intPtr == intPtr2)
					{
						return intPtr2;
					}
					intPtr = intPtr2;
					IntPtr sizeOfReadableMemory = this.System.GetSizeOfReadableMemory(intPtr2, (IntPtr)knownMethodThunks.MaxMinLength);
					if (sizeOfReadableMemory <= (IntPtr)0)
					{
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler debugLogWarningStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogWarningStringHandler(43, 2, out flag3);
						if (flag3)
						{
							debugLogWarningStringHandler.AppendLiteral("Got zero or negative readable length ");
							debugLogWarningStringHandler.AppendFormatted<IntPtr>(sizeOfReadableMemory);
							debugLogWarningStringHandler.AppendLiteral(" at 0x");
							debugLogWarningStringHandler.AppendFormatted<IntPtr>(intPtr2, "x16");
						}
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Warning(ref debugLogWarningStringHandler);
					}
					global::System.ReadOnlySpan<byte> readOnlySpan = new global::System.ReadOnlySpan<byte>(intPtr2, Math.Min((int)sizeOfReadableMemory, knownMethodThunks.MaxMinLength));
					ulong num2;
					BytePattern bytePattern;
					int num3;
					int num4;
					if (!knownMethodThunks.TryFindMatch(readOnlySpan, out num2, out bytePattern, out num3, out num4))
					{
						return intPtr2;
					}
					IntPtr intPtr3 = intPtr2;
					flag2 = false;
					AddressMeaning addressMeaning = bytePattern.AddressMeaning;
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler3 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(46, 4, out flag3);
					if (flag3)
					{
						debugLogTraceStringHandler3.AppendLiteral("Matched thunk with ");
						debugLogTraceStringHandler3.AppendFormatted<AddressMeaning>(addressMeaning);
						debugLogTraceStringHandler3.AppendLiteral(" at 0x");
						debugLogTraceStringHandler3.AppendFormatted<IntPtr>(intPtr2, "x16");
						debugLogTraceStringHandler3.AppendLiteral(" (addr: 0x");
						debugLogTraceStringHandler3.AppendFormatted<ulong>(num2, "x8");
						debugLogTraceStringHandler3.AppendLiteral(", offset: ");
						debugLogTraceStringHandler3.AppendFormatted<int>(num3);
						debugLogTraceStringHandler3.AppendLiteral(")");
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler3);
					if (addressMeaning.Kind.IsPrecodeFixup() && !flag)
					{
						IntPtr intPtr4 = addressMeaning.ProcessAddress(intPtr2, num3, num2);
						if (reloadPtr)
						{
							<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler4 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(56, 1, out flag3);
							if (flag3)
							{
								debugLogTraceStringHandler4.AppendLiteral("Method thunk reset; regenerating (PrecodeFixupThunk: 0x");
								debugLogTraceStringHandler4.AppendFormatted<IntPtr>(intPtr4, "X16");
								debugLogTraceStringHandler4.AppendLiteral(")");
							}
							<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler4);
							this.Compile(method);
							flag2 = true;
							goto IL_0041;
						}
						intPtr2 = intPtr4;
					}
					else
					{
						intPtr2 = addressMeaning.ProcessAddress(intPtr2, num3, num2);
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler5 = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(23, 1, out flag3);
					if (flag3)
					{
						debugLogTraceStringHandler5.AppendLiteral("Got next entry point 0x");
						debugLogTraceStringHandler5.AppendFormatted<IntPtr>(intPtr2, "x16");
					}
					<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler5);
					bool flag4;
					intPtr2 = this.NotThePreStub(intPtr3, intPtr2, out flag4);
					if (flag4 && reloadPtr)
					{
						<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace("Matched ThePreStub");
						this.Compile(method);
						goto IL_0041;
					}
				}
				break;
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler debugLogErrorStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogErrorStringHandler(70, 4, out flag3);
			if (flag3)
			{
				debugLogErrorStringHandler.AppendLiteral("Could not get entry point for ");
				debugLogErrorStringHandler.AppendFormatted<MethodBase>(method);
				debugLogErrorStringHandler.AppendLiteral("! (tried ");
				debugLogErrorStringHandler.AppendFormatted<int>(num);
				debugLogErrorStringHandler.AppendLiteral(" times) entry: 0x");
				debugLogErrorStringHandler.AppendFormatted<IntPtr>(intPtr2, "x16");
				debugLogErrorStringHandler.AppendLiteral(" prevEntry: 0x");
				debugLogErrorStringHandler.AppendFormatted<IntPtr>(intPtr, "x16");
			}
			<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Error(ref debugLogErrorStringHandler);
			FormatInterpolatedStringHandler formatInterpolatedStringHandler = new FormatInterpolatedStringHandler(47, 1);
			formatInterpolatedStringHandler.AppendLiteral("Could not get entrypoint for ");
			formatInterpolatedStringHandler.AppendFormatted<MethodBase>(method);
			formatInterpolatedStringHandler.AppendLiteral(" (stuck in a loop)");
			throw new NotSupportedException(DebugFormatter.Format(ref formatInterpolatedStringHandler));
		}

		private IntPtr GetNativeMethodBodyDirect(MethodBase method)
		{
			return this.Runtime.GetMethodEntryPoint(method);
		}

		private IntPtr NotThePreStub(IntPtr ptrGot, IntPtr ptrParsed, out bool wasPreStub)
		{
			if (this.ThePreStub == IntPtr.Zero)
			{
				this.ThePreStub = (IntPtr)(-2);
				Type type = typeof(HttpWebRequest).Assembly.GetType("System.Net.Connection");
				IntPtr intPtr;
				if (type == null)
				{
					intPtr = (IntPtr)(-1);
				}
				else
				{
					intPtr = (from m in type.GetMethods()
						group m by this.GetNativeMethodBodyWalk(m, false)).First<IGrouping<IntPtr, MethodInfo>>((IGrouping<IntPtr, MethodInfo> g) => g.Count<MethodInfo>() > 1).Key;
				}
				IntPtr intPtr2 = intPtr;
				this.ThePreStub = intPtr2;
				bool flag;
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler debugLogTraceStringHandler = new <24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.DebugLogTraceStringHandler(14, 1, out flag);
				if (flag)
				{
					debugLogTraceStringHandler.AppendLiteral("ThePreStub: 0x");
					debugLogTraceStringHandler.AppendFormatted<IntPtr>(this.ThePreStub, "X16");
				}
				<24b3ba8a-00b7-40fc-a603-2711fa115297>MMDbgLog.Trace(ref debugLogTraceStringHandler);
			}
			wasPreStub = ptrParsed == this.ThePreStub;
			if (!wasPreStub)
			{
				return ptrParsed;
			}
			return ptrGot;
		}

		public unsafe MethodBase GetRealDetourTarget(MethodBase from, MethodBase to)
		{
			Helpers.ThrowIfArgumentNull<MethodBase>(from, "from");
			Helpers.ThrowIfArgumentNull<MethodBase>(to, "to");
			to = this.GetIdentifiable(to);
			MethodInfo methodInfo = from as MethodInfo;
			if (methodInfo != null)
			{
				MethodInfo methodInfo2 = to as MethodInfo;
				if (methodInfo2 != null)
				{
					bool flag = false;
					global::System.ReadOnlySpan<SpecialArgumentKind> span = this.Abi.ArgumentOrder.Span;
					for (int i = 0; i < span.Length; i++)
					{
						if (*span[i] == 1)
						{
							flag = true;
							break;
						}
					}
					Type returnType = methodInfo.ReturnType;
					bool flag2 = this.Abi.Classify(returnType, true) == TypeClassification.ByReference;
					bool flag3 = !methodInfo.IsStatic;
					bool flag4 = flag3 && methodInfo2.IsStatic && flag2 && flag;
					bool flag5 = PlatformTriple.HasGenericContext(this.Abi) && this.Runtime.RequiresGenericContext(methodInfo);
					if (!flag4 && !flag5)
					{
						return to;
					}
					Type type = (flag2 ? returnType.MakeByRefType() : returnType);
					Type type2 = ((flag2 && !this.Abi.ReturnsReturnBuffer) ? typeof(void) : type);
					int num = -1;
					int num2 = -1;
					int num3 = -1;
					ParameterInfo[] parameters = from.GetParameters();
					List<Type> list = new List<Type>(parameters.Length + 3);
					global::System.ReadOnlySpan<SpecialArgumentKind> span2 = this.Abi.ArgumentOrder.Span;
					for (int j = 0; j < span2.Length; j++)
					{
						switch (*span2[j])
						{
						case 0:
							if (flag3)
							{
								num = list.Count;
								list.Add(from.GetThisParamType());
							}
							break;
						case 1:
							if (flag2)
							{
								num2 = list.Count;
								list.Add(type);
							}
							break;
						case 2:
							if (flag5)
							{
								list.Add(typeof(IntPtr));
							}
							break;
						case 3:
							num3 = list.Count;
							list.AddRange(parameters.Select<ParameterInfo, Type>((ParameterInfo p) => p.ParameterType));
							break;
						}
					}
					FormatInterpolatedStringHandler formatInterpolatedStringHandler = new FormatInterpolatedStringHandler(16, 2);
					formatInterpolatedStringHandler.AppendLiteral("Glue:AbiFixup<");
					formatInterpolatedStringHandler.AppendFormatted<MethodBase>(from);
					formatInterpolatedStringHandler.AppendLiteral(",");
					formatInterpolatedStringHandler.AppendFormatted<MethodBase>(to);
					formatInterpolatedStringHandler.AppendLiteral(">");
					MethodBase methodBase;
					using (DynamicMethodDefinition dynamicMethodDefinition = new DynamicMethodDefinition(DebugFormatter.Format(ref formatInterpolatedStringHandler), type2, list.ToArray()))
					{
						dynamicMethodDefinition.Definition.ImplAttributes |= Mono.Cecil.MethodImplAttributes.NoInlining | Mono.Cecil.MethodImplAttributes.AggressiveOptimization;
						ILProcessor ilprocessor = dynamicMethodDefinition.GetILProcessor();
						if (flag2 && num2 >= 0)
						{
							ilprocessor.Emit(OpCodes.Ldarg, num2);
						}
						if (flag3)
						{
							ilprocessor.Emit(OpCodes.Ldarg, num);
						}
						for (int k = 0; k < parameters.Length; k++)
						{
							ilprocessor.Emit(OpCodes.Ldarg, k + num3);
						}
						ilprocessor.Emit(OpCodes.Call, ilprocessor.Body.Method.Module.ImportReference(to));
						if (flag2 && num2 >= 0)
						{
							ilprocessor.Emit(OpCodes.Stobj, ilprocessor.Body.Method.Module.ImportReference(returnType));
						}
						if (flag2 && this.Abi.ReturnsReturnBuffer)
						{
							ilprocessor.Emit(OpCodes.Ldarg, num2);
						}
						ilprocessor.Emit(OpCodes.Ret);
						methodBase = dynamicMethodDefinition.Generate();
					}
					return methodBase;
				}
			}
			return to;
		}

		private unsafe static bool HasGenericContext(Abi abi)
		{
			global::System.ReadOnlySpan<SpecialArgumentKind> span = abi.ArgumentOrder.Span;
			for (int i = 0; i < span.Length; i++)
			{
				if (*span[i] == 2)
				{
					return true;
				}
			}
			return false;
		}

		private static object lazyCurrentLock = new object();

		[Nullable(2)]
		private static PlatformTriple lazyCurrent;

		private static readonly Func<PlatformTriple> createCurrentFunc = new Func<PlatformTriple>(PlatformTriple.CreateCurrent);

		private IntPtr ThePreStub = IntPtr.Zero;

		[Nullable(0)]
		public struct NativeDetour : IEquatable<PlatformTriple.NativeDetour>
		{
			public NativeDetour(SimpleNativeDetour Simple, IntPtr AltEntry, [Nullable(2)] IDisposable AltHandle)
			{
				this.Simple = Simple;
				this.AltEntry = AltEntry;
				this.AltHandle = AltHandle;
			}

			public SimpleNativeDetour Simple { readonly get; set; }

			public IntPtr AltEntry { readonly get; set; }

			[Nullable(2)]
			public IDisposable AltHandle
			{
				[NullableContext(2)]
				readonly get;
				[NullableContext(2)]
				set;
			}

			public bool HasAltEntry
			{
				get
				{
					return this.AltEntry != IntPtr.Zero;
				}
			}

			[NullableContext(0)]
			[CompilerGenerated]
			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("NativeDetour");
				stringBuilder.Append(" { ");
				if (this.PrintMembers(stringBuilder))
				{
					stringBuilder.Append(' ');
				}
				stringBuilder.Append('}');
				return stringBuilder.ToString();
			}

			[NullableContext(0)]
			[CompilerGenerated]
			private bool PrintMembers(StringBuilder builder)
			{
				builder.Append("Simple = ");
				builder.Append(this.Simple);
				builder.Append(", AltEntry = ");
				builder.Append(this.AltEntry.ToString());
				builder.Append(", AltHandle = ");
				builder.Append(this.AltHandle);
				builder.Append(", HasAltEntry = ");
				builder.Append(this.HasAltEntry.ToString());
				return true;
			}

			[CompilerGenerated]
			public static bool operator !=(PlatformTriple.NativeDetour left, PlatformTriple.NativeDetour right)
			{
				return !(left == right);
			}

			[CompilerGenerated]
			public static bool operator ==(PlatformTriple.NativeDetour left, PlatformTriple.NativeDetour right)
			{
				return left.Equals(right);
			}

			[CompilerGenerated]
			public override readonly int GetHashCode()
			{
				return (EqualityComparer<SimpleNativeDetour>.Default.GetHashCode(this.<Simple>k__BackingField) * -1521134295 + EqualityComparer<IntPtr>.Default.GetHashCode(this.<AltEntry>k__BackingField)) * -1521134295 + EqualityComparer<IDisposable>.Default.GetHashCode(this.<AltHandle>k__BackingField);
			}

			[NullableContext(0)]
			[CompilerGenerated]
			public override readonly bool Equals(object obj)
			{
				return obj is PlatformTriple.NativeDetour && this.Equals((PlatformTriple.NativeDetour)obj);
			}

			[CompilerGenerated]
			public readonly bool Equals(PlatformTriple.NativeDetour other)
			{
				return EqualityComparer<SimpleNativeDetour>.Default.Equals(this.<Simple>k__BackingField, other.<Simple>k__BackingField) && EqualityComparer<IntPtr>.Default.Equals(this.<AltEntry>k__BackingField, other.<AltEntry>k__BackingField) && EqualityComparer<IDisposable>.Default.Equals(this.<AltHandle>k__BackingField, other.<AltHandle>k__BackingField);
			}

			[CompilerGenerated]
			public readonly void Deconstruct(out SimpleNativeDetour Simple, out IntPtr AltEntry, [Nullable(2)] out IDisposable AltHandle)
			{
				Simple = this.Simple;
				AltEntry = this.AltEntry;
				AltHandle = this.AltHandle;
			}
		}
	}
}
