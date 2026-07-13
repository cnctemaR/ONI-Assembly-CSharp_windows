using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Jobs.LowLevel.Unsafe;

namespace Unity.Burst
{
	public sealed class BurstCompilerOptions
	{
		internal static string SerialiseCompilationOptionsSafe(string[] roots, string[] folders, string options)
		{
			return SafeStringArrayHelper.SerialiseStringArraySafe(new string[]
			{
				SafeStringArrayHelper.SerialiseStringArraySafe(roots),
				SafeStringArrayHelper.SerialiseStringArraySafe(folders),
				options
			});
		}

		[return: TupleElementNames(new string[] { "roots", "folders", "options" })]
		internal static ValueTuple<string[], string[], string> DeserialiseCompilationOptionsSafe(string from)
		{
			string[] array = SafeStringArrayHelper.DeserialiseStringArraySafe(from);
			return new ValueTuple<string[], string[], string>(SafeStringArrayHelper.DeserialiseStringArraySafe(array[0]), SafeStringArrayHelper.DeserialiseStringArraySafe(array[1]), array[2]);
		}

		private BurstCompilerOptions()
			: this(false)
		{
		}

		internal BurstCompilerOptions(bool isGlobal)
		{
			this.IsGlobal = isGlobal;
			this.EnableBurstCompilation = true;
			this.EnableBurstSafetyChecks = true;
		}

		private bool IsGlobal { get; }

		public bool IsEnabled
		{
			get
			{
				return this.EnableBurstCompilation && !BurstCompilerOptions.ForceDisableBurstCompilation;
			}
		}

		public bool EnableBurstCompilation
		{
			get
			{
				return this._enableBurstCompilation;
			}
			set
			{
				if (this.IsGlobal && BurstCompilerOptions.ForceDisableBurstCompilation)
				{
					value = false;
				}
				bool flag = this._enableBurstCompilation != value;
				this._enableBurstCompilation = value;
				if (this.IsGlobal)
				{
					JobsUtility.JobCompilerEnabled = value;
					BurstCompiler._IsEnabled = value;
				}
				if (flag)
				{
					this.OnOptionsChanged();
				}
			}
		}

		public bool EnableBurstCompileSynchronously
		{
			get
			{
				return this._enableBurstCompileSynchronously;
			}
			set
			{
				bool flag = this._enableBurstCompileSynchronously != value;
				this._enableBurstCompileSynchronously = value;
				if (flag)
				{
					this.OnOptionsChanged();
				}
			}
		}

		public bool EnableBurstSafetyChecks
		{
			get
			{
				return this._enableBurstSafetyChecks;
			}
			set
			{
				bool flag = this._enableBurstSafetyChecks != value;
				this._enableBurstSafetyChecks = value;
				if (flag)
				{
					this.OnOptionsChanged();
					this.MaybeTriggerRecompilation();
				}
			}
		}

		public bool ForceEnableBurstSafetyChecks
		{
			get
			{
				return this._forceEnableBurstSafetyChecks;
			}
			set
			{
				bool flag = this._forceEnableBurstSafetyChecks != value;
				this._forceEnableBurstSafetyChecks = value;
				if (flag)
				{
					this.OnOptionsChanged();
					this.MaybeTriggerRecompilation();
				}
			}
		}

		public bool EnableBurstDebug
		{
			get
			{
				return this._enableBurstDebug;
			}
			set
			{
				bool flag = this._enableBurstDebug != value;
				this._enableBurstDebug = value;
				if (flag)
				{
					this.OnOptionsChanged();
					this.MaybeTriggerRecompilation();
				}
			}
		}

		[Obsolete("This property is no longer used and will be removed in a future major release")]
		public bool DisableOptimizations
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		[Obsolete("This property is no longer used and will be removed in a future major release. Use the [BurstCompile(FloatMode = FloatMode.Fast)] on the method directly to enable this feature")]
		public bool EnableFastMath
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		internal bool EnableBurstTimings
		{
			get
			{
				return this._enableBurstTimings;
			}
			set
			{
				bool flag = this._enableBurstTimings != value;
				this._enableBurstTimings = value;
				if (flag)
				{
					this.OnOptionsChanged();
				}
			}
		}

		internal bool RequiresSynchronousCompilation
		{
			get
			{
				return this.EnableBurstCompileSynchronously || BurstCompilerOptions.ForceBurstCompilationSynchronously;
			}
		}

		internal Action OptionsChanged { get; set; }

		internal BurstCompilerOptions Clone()
		{
			return new BurstCompilerOptions
			{
				EnableBurstCompilation = this.EnableBurstCompilation,
				EnableBurstCompileSynchronously = this.EnableBurstCompileSynchronously,
				EnableBurstSafetyChecks = this.EnableBurstSafetyChecks,
				EnableBurstTimings = this.EnableBurstTimings,
				EnableBurstDebug = this.EnableBurstDebug,
				ForceEnableBurstSafetyChecks = this.ForceEnableBurstSafetyChecks
			};
		}

		private static bool TryGetAttribute(MemberInfo member, out BurstCompileAttribute attribute)
		{
			attribute = null;
			if (member == null)
			{
				return false;
			}
			attribute = BurstCompilerOptions.GetBurstCompileAttribute(member);
			return attribute != null;
		}

		private static bool TryGetAttribute(Assembly assembly, out BurstCompileAttribute attribute)
		{
			if (assembly == null)
			{
				attribute = null;
				return false;
			}
			attribute = assembly.GetCustomAttribute<BurstCompileAttribute>();
			return attribute != null;
		}

		private static BurstCompileAttribute GetBurstCompileAttribute(MemberInfo memberInfo)
		{
			BurstCompileAttribute customAttribute = memberInfo.GetCustomAttribute<BurstCompileAttribute>();
			if (customAttribute != null)
			{
				return customAttribute;
			}
			using (IEnumerator<Attribute> enumerator = memberInfo.GetCustomAttributes().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.GetType().FullName == "Burst.Compiler.IL.Tests.TestCompilerAttribute")
					{
						List<string> list = new List<string>();
						return new BurstCompileAttribute(FloatPrecision.Standard, FloatMode.Default)
						{
							CompileSynchronously = true,
							Options = list.ToArray()
						};
					}
				}
			}
			return null;
		}

		internal static bool HasBurstCompileAttribute(MemberInfo member)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			BurstCompileAttribute burstCompileAttribute;
			return BurstCompilerOptions.TryGetAttribute(member, out burstCompileAttribute);
		}

		internal static void MergeAttributes(ref BurstCompileAttribute memberAttribute, in BurstCompileAttribute assemblyAttribute)
		{
			if (memberAttribute.FloatMode == FloatMode.Default)
			{
				memberAttribute.FloatMode = assemblyAttribute.FloatMode;
			}
			if (memberAttribute.FloatPrecision == FloatPrecision.Standard)
			{
				memberAttribute.FloatPrecision = assemblyAttribute.FloatPrecision;
			}
			if (memberAttribute.OptimizeFor == OptimizeFor.Default)
			{
				memberAttribute.OptimizeFor = assemblyAttribute.OptimizeFor;
			}
			if (memberAttribute._compileSynchronously == null && assemblyAttribute._compileSynchronously != null)
			{
				memberAttribute._compileSynchronously = assemblyAttribute._compileSynchronously;
			}
			if (memberAttribute._debug == null && assemblyAttribute._debug != null)
			{
				memberAttribute._debug = assemblyAttribute._debug;
			}
			if (memberAttribute._disableDirectCall == null && assemblyAttribute._disableDirectCall != null)
			{
				memberAttribute._disableDirectCall = assemblyAttribute._disableDirectCall;
			}
			if (memberAttribute._disableSafetyChecks == null && assemblyAttribute._disableSafetyChecks != null)
			{
				memberAttribute._disableSafetyChecks = assemblyAttribute._disableSafetyChecks;
			}
		}

		internal bool TryGetOptions(MemberInfo member, out string flagsOut, bool isForILPostProcessing = false, bool isForCompilerClient = false)
		{
			flagsOut = null;
			BurstCompileAttribute burstCompileAttribute;
			if (!BurstCompilerOptions.TryGetAttribute(member, out burstCompileAttribute))
			{
				return false;
			}
			BurstCompileAttribute burstCompileAttribute2;
			if (BurstCompilerOptions.TryGetAttribute(member.Module.Assembly, out burstCompileAttribute2))
			{
				BurstCompilerOptions.MergeAttributes(ref burstCompileAttribute, in burstCompileAttribute2);
			}
			flagsOut = this.GetOptions(burstCompileAttribute, isForILPostProcessing, isForCompilerClient);
			return true;
		}

		internal string GetOptions(BurstCompileAttribute attr = null, bool isForILPostProcessing = false, bool isForCompilerClient = false)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!isForCompilerClient && ((attr != null && attr.CompileSynchronously) || this.RequiresSynchronousCompilation))
			{
				BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("enable-synchronous-compilation", null));
			}
			BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("debug=", "LineOnly"));
			if (isForILPostProcessing)
			{
				BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("compilation-priority=", CompilationPriority.ILPP));
			}
			if (attr != null)
			{
				if (attr.FloatMode != FloatMode.Default)
				{
					BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("float-mode=", attr.FloatMode));
				}
				if (attr.FloatPrecision != FloatPrecision.Standard)
				{
					BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("float-precision=", attr.FloatPrecision));
				}
				if (attr.DisableSafetyChecks)
				{
					BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("disable-safety-checks", null));
				}
				if (attr.Options != null)
				{
					foreach (string text in attr.Options)
					{
						if (!string.IsNullOrEmpty(text))
						{
							BurstCompilerOptions.AddOption(stringBuilder, text);
						}
					}
				}
				switch (attr.OptimizeFor)
				{
				case OptimizeFor.Default:
				case OptimizeFor.Balanced:
					BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("opt-level=", 2));
					break;
				case OptimizeFor.Performance:
					BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("opt-level=", 3));
					break;
				case OptimizeFor.Size:
					BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("opt-for-size", null));
					BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("opt-level=", 3));
					break;
				case OptimizeFor.FastCompilation:
					BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("opt-level=", 1));
					break;
				}
			}
			if (this.ForceEnableBurstSafetyChecks)
			{
				BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("global-safety-checks-setting=", GlobalSafetyChecksSettingKind.ForceOn));
			}
			else if (this.EnableBurstSafetyChecks)
			{
				BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("global-safety-checks-setting=", GlobalSafetyChecksSettingKind.On));
			}
			else
			{
				BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("global-safety-checks-setting=", GlobalSafetyChecksSettingKind.Off));
			}
			if (this.EnableBurstTimings)
			{
				BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("log-timings", null));
			}
			if (this.EnableBurstDebug || (attr != null && attr.Debug))
			{
				BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("debugMode", null));
			}
			BurstCompilerOptions.AddOption(stringBuilder, BurstCompilerOptions.GetOption("temp-folder=", Path.Combine(Environment.CurrentDirectory, "Temp", "Burst")));
			return stringBuilder.ToString();
		}

		private static void AddOption(StringBuilder builder, string option)
		{
			if (builder.Length != 0)
			{
				builder.Append('\n');
			}
			builder.Append(option);
		}

		internal static string GetOption(string optionName, object value = null)
		{
			if (optionName == null)
			{
				throw new ArgumentNullException("optionName");
			}
			string text = "--";
			object obj = value ?? string.Empty;
			return text + optionName + ((obj != null) ? obj.ToString() : null);
		}

		private void OnOptionsChanged()
		{
			Action optionsChanged = this.OptionsChanged;
			if (optionsChanged == null)
			{
				return;
			}
			optionsChanged();
		}

		private void MaybeTriggerRecompilation()
		{
		}

		static BurstCompilerOptions()
		{
			foreach (string text in Environment.GetCommandLineArgs())
			{
				if (!(text == "--burst-disable-compilation"))
				{
					if (text == "--burst-force-sync-compilation")
					{
						BurstCompilerOptions.ForceBurstCompilationSynchronously = true;
					}
				}
				else
				{
					BurstCompilerOptions.ForceDisableBurstCompilation = true;
				}
			}
			if (BurstCompilerOptions.CheckIsSecondaryUnityProcess())
			{
				BurstCompilerOptions.ForceDisableBurstCompilation = true;
				BurstCompilerOptions.IsSecondaryUnityProcess = true;
			}
			string environmentVariable = Environment.GetEnvironmentVariable("UNITY_BURST_DISABLE_COMPILATION");
			if (!string.IsNullOrEmpty(environmentVariable) && environmentVariable != "0")
			{
				BurstCompilerOptions.ForceDisableBurstCompilation = true;
			}
		}

		private static bool CheckIsSecondaryUnityProcess()
		{
			return false;
		}

		private const string DisableCompilationArg = "--burst-disable-compilation";

		private const string ForceSynchronousCompilationArg = "--burst-force-sync-compilation";

		internal const string DefaultLibraryName = "lib_burst_generated";

		internal const string BurstInitializeName = "burst.initialize";

		internal const string BurstInitializeExternalsName = "burst.initialize.externals";

		internal const string BurstInitializeStaticsName = "burst.initialize.statics";

		internal const string OptionBurstcSwitch = "+burstc";

		internal const string OptionGroup = "group";

		internal const string OptionPlatform = "platform=";

		internal const string OptionBackend = "backend=";

		internal const string OptionGlobalSafetyChecksSetting = "global-safety-checks-setting=";

		internal const string OptionDisableSafetyChecks = "disable-safety-checks";

		internal const string OptionDisableOpt = "disable-opt";

		internal const string OptionFastMath = "fastmath";

		internal const string OptionTarget = "target=";

		internal const string OptionOptLevel = "opt-level=";

		internal const string OptionLogTimings = "log-timings";

		internal const string OptionOptForSize = "opt-for-size";

		internal const string OptionFloatPrecision = "float-precision=";

		internal const string OptionFloatMode = "float-mode=";

		internal const string OptionBranchProtection = "branch-protection=";

		internal const string OptionDisableWarnings = "disable-warnings=";

		internal const string OptionAssemblyDefines = "assembly-defines=";

		internal const string OptionDump = "dump=";

		internal const string OptionFormat = "format=";

		internal const string OptionDebugTrap = "debugtrap";

		internal const string OptionDisableVectors = "disable-vectors";

		internal const string OptionDebug = "debug=";

		internal const string OptionDebugMode = "debugMode";

		internal const string OptionStaticLinkage = "generate-static-linkage-methods";

		internal const string OptionJobMarshalling = "generate-job-marshalling-methods";

		internal const string OptionTempDirectory = "temp-folder=";

		internal const string OptionEnableDirectExternalLinking = "enable-direct-external-linking";

		internal const string OptionLinkerOptions = "linker-options=";

		internal const string OptionEnableAutoLayoutFallbackCheck = "enable-autolayout-fallback-check";

		internal const string OptionGenerateLinkXml = "generate-link-xml=";

		internal const string OptionMetaDataGeneration = "meta-data-generation=";

		internal const string OptionDisableStringInterpolationInExceptionMessages = "disable-string-interpolation-in-exception-messages";

		internal const string OptionPlatformConfiguration = "platform-configuration=";

		internal const string OptionStackProtector = "stack-protector=";

		internal const string OptionStackProtectorBufferSize = "stack-protector-buffer-size=";

		internal const string OptionCacheDirectory = "cache-directory=";

		internal const string OptionJitDisableFunctionCaching = "disable-function-caching";

		internal const string OptionJitDisableAssemblyCaching = "disable-assembly-caching";

		internal const string OptionJitEnableAssemblyCachingLogs = "enable-assembly-caching-logs";

		internal const string OptionJitEnableSynchronousCompilation = "enable-synchronous-compilation";

		internal const string OptionJitCompilationPriority = "compilation-priority=";

		internal const string OptionJitIsForFunctionPointer = "is-for-function-pointer";

		internal const string OptionJitManagedFunctionPointer = "managed-function-pointer=";

		internal const string OptionJitManagedDelegateHandle = "managed-delegate-handle=";

		internal const string OptionEnableInterpreter = "enable-interpreter";

		internal const string OptionAotAssemblyFolder = "assembly-folder=";

		internal const string OptionRootAssembly = "root-assembly=";

		internal const string OptionIncludeRootAssemblyReferences = "include-root-assembly-references=";

		internal const string OptionAotMethod = "method=";

		internal const string OptionAotType = "type=";

		internal const string OptionAotAssembly = "assembly=";

		internal const string OptionAotOutputPath = "output=";

		internal const string OptionAotKeepIntermediateFiles = "keep-intermediate-files";

		internal const string OptionAotNoLink = "nolink";

		internal const string OptionAotOnlyStaticMethods = "only-static-methods";

		internal const string OptionMethodPrefix = "method-prefix=";

		internal const string OptionAotNoNativeToolchain = "no-native-toolchain";

		internal const string OptionAotEmitLlvmObjects = "emit-llvm-objects";

		internal const string OptionAotKeyFolder = "key-folder=";

		internal const string OptionAotDecodeFolder = "decode-folder=";

		internal const string OptionVerbose = "verbose";

		internal const string OptionValidateExternalToolChain = "validate-external-tool-chain";

		internal const string OptionCompilerThreads = "threads=";

		internal const string OptionChunkSize = "chunk-size=";

		internal const string OptionPrintLogOnMissingPInvokeCallbackAttribute = "print-monopinvokecallbackmissing-message";

		internal const string OptionOutputMode = "output-mode=";

		internal const string OptionAlwaysCreateOutput = "always-create-output=";

		internal const string OptionAotPdbSearchPaths = "pdb-search-paths=";

		internal const string OptionSafetyChecks = "safety-checks";

		internal const string OptionLibraryOutputMode = "library-output-mode=";

		internal const string OptionCompilationId = "compilation-id=";

		internal const string OptionTargetFramework = "target-framework=";

		internal const string OptionDiscardAssemblies = "discard-assemblies=";

		internal const string OptionSaveExtraContext = "save-extra-context";

		internal const string CompilerCommandShutdown = "$shutdown";

		internal const string CompilerCommandCancel = "$cancel";

		internal const string CompilerCommandEnableCompiler = "$enable_compiler";

		internal const string CompilerCommandDisableCompiler = "$disable_compiler";

		internal const string CompilerCommandSetDefaultOptions = "$set_default_options";

		internal const string CompilerCommandTriggerSetupRecompilation = "$trigger_setup_recompilation";

		internal const string CompilerCommandIsCurrentCompilationDone = "$is_current_compilation_done";

		internal const string CompilerCommandTriggerRecompilation = "$trigger_recompilation";

		internal const string CompilerCommandInitialize = "$initialize";

		internal const string CompilerCommandDomainReload = "$domain_reload";

		internal const string CompilerCommandVersionNotification = "$version";

		internal const string CompilerCommandGetTargetCpuFromHost = "$get_target_cpu_from_host";

		internal const string CompilerCommandSetProfileCallbacks = "$set_profile_callbacks";

		internal const string CompilerCommandUnloadBurstNatives = "$unload_burst_natives";

		internal const string CompilerCommandIsNativeApiAvailable = "$is_native_api_available";

		internal const string CompilerCommandILPPCompilation = "$ilpp_compilation";

		internal const string CompilerCommandIsArmTestEnv = "$is_arm_test_env";

		internal const string CompilerCommandNotifyAssemblyCompilationNotRequired = "$notify_assembly_compilation_not_required";

		internal const string CompilerCommandNotifyAssemblyCompilationFinished = "$notify_assembly_compilation_finished";

		internal const string CompilerCommandNotifyCompilationStarted = "$notify_compilation_started";

		internal const string CompilerCommandNotifyCompilationFinished = "$notify_compilation_finished";

		internal const string CompilerCommandAotCompilation = "$aot_compilation";

		internal const string CompilerCommandRequestInitialiseDebuggerCommmand = "$request_debug_command";

		internal const string CompilerCommandInitialiseDebuggerCommmand = "$load_debugger_interface";

		internal const string CompilerCommandRequestSetProtocolVersionEditor = "$request_set_protocol_version_editor";

		internal const string CompilerCommandSetProtocolVersionBurst = "$set_protocol_version_burst";

		internal static readonly bool ForceDisableBurstCompilation;

		private static readonly bool ForceBurstCompilationSynchronously;

		internal static readonly bool IsSecondaryUnityProcess;

		private bool _enableBurstCompilation;

		private bool _enableBurstCompileSynchronously;

		private bool _enableBurstSafetyChecks;

		private bool _enableBurstTimings;

		private bool _enableBurstDebug;

		private bool _forceEnableBurstSafetyChecks;
	}
}
