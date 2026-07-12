using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour.Platforms
{
	public class DetourRuntimeNETCorePlatform : DetourRuntimeNETPlatform
	{
		public DetourRuntimeNETCorePlatform()
		{
			this.GlueThiscallInStructRetPtr = this.GlueThiscallStructRetPtr;
		}

		protected static IntPtr GetJitObject()
		{
			if (DetourRuntimeNETCorePlatform.getJit == null)
			{
				ProcessModule processModule = Process.GetCurrentProcess().Modules.Cast<ProcessModule>().FirstOrDefault<ProcessModule>((ProcessModule m) => Path.GetFileNameWithoutExtension(m.FileName).EndsWith("clrjit", StringComparison.Ordinal));
				if (processModule == null)
				{
					throw new PlatformNotSupportedException();
				}
				IntPtr intPtr;
				if (!DynDll.TryOpenLibrary(processModule.FileName, out intPtr, false, null))
				{
					throw new PlatformNotSupportedException();
				}
				if (PlatformHelper.Is(Platform.Windows))
				{
					DetourRuntimeNETCorePlatform.isNet5Jit = processModule.FileVersionInfo.ProductMajorPart >= 5;
				}
				else
				{
					DetourRuntimeNETCorePlatform.isNet5Jit = typeof(object).Assembly.GetName().Version.Major >= 5;
				}
				try
				{
					DetourRuntimeNETCorePlatform.getJit = intPtr.GetFunction("getJit").AsDelegate<DetourRuntimeNETCorePlatform.d_getJit>();
				}
				catch
				{
					DynDll.CloseLibrary(intPtr);
					throw;
				}
			}
			return DetourRuntimeNETCorePlatform.getJit();
		}

		protected static Guid GetJitGuid(IntPtr jit)
		{
			int num = (DetourRuntimeNETCorePlatform.isNet5Jit ? 2 : 4);
			Guid guid;
			DetourRuntimeNETCorePlatform.ReadObjectVTable(jit, num).AsDelegate<DetourRuntimeNETCorePlatform.d_getVersionIdentifier>()(jit, out guid);
			return guid;
		}

		protected virtual int VTableIndex_ICorJitCompiler_compileMethod
		{
			get
			{
				return 0;
			}
		}

		protected unsafe static IntPtr* GetVTableEntry(IntPtr @object, int index)
		{
			return *(IntPtr*)(void*)@object / (IntPtr)sizeof(IntPtr) + index * sizeof(IntPtr);
		}

		protected unsafe static IntPtr ReadObjectVTable(IntPtr @object, int index)
		{
			return *DetourRuntimeNETCorePlatform.GetVTableEntry(@object, index);
		}

		protected override void DisableInlining(MethodBase method, RuntimeMethodHandle handle)
		{
		}

		protected virtual void InstallJitHooks(IntPtr jitObject)
		{
			throw new PlatformNotSupportedException();
		}

		public override bool OnMethodCompiledWillBeCalled
		{
			get
			{
				return false;
			}
		}

		public override event OnMethodCompiledEvent OnMethodCompiled;

		protected virtual void JitHookCore(RuntimeTypeHandle declaringType, RuntimeMethodHandle methodHandle, IntPtr methodBodyStart, ulong methodBodySize, RuntimeTypeHandle[] genericClassArguments, RuntimeTypeHandle[] genericMethodArguments)
		{
			try
			{
				Type type = Type.GetTypeFromHandle(declaringType);
				if (genericClassArguments != null && type.IsGenericTypeDefinition)
				{
					type = type.MakeGenericType(genericClassArguments.Select<RuntimeTypeHandle, Type>(new Func<RuntimeTypeHandle, Type>(Type.GetTypeFromHandle)).ToArray<Type>());
				}
				MethodBase methodBase = MethodBase.GetMethodFromHandle(methodHandle, type.TypeHandle);
				if (methodBase == null)
				{
					methodBase = this.GetPin(methodHandle).Method;
				}
				try
				{
					OnMethodCompiledEvent onMethodCompiled = this.OnMethodCompiled;
					if (onMethodCompiled != null)
					{
						onMethodCompiled(methodBase, methodBodyStart, methodBodySize);
					}
				}
				catch (Exception ex)
				{
					MMDbgLog.Log(string.Format("Error executing OnMethodCompiled event: {0}", ex));
				}
			}
			catch (Exception ex2)
			{
				MMDbgLog.Log(string.Format("Error in JitHookCore: {0}", ex2));
			}
		}

		public static DetourRuntimeNETCorePlatform Create()
		{
			try
			{
				IntPtr jitObject = DetourRuntimeNETCorePlatform.GetJitObject();
				Guid jitGuid = DetourRuntimeNETCorePlatform.GetJitGuid(jitObject);
				DetourRuntimeNETCorePlatform detourRuntimeNETCorePlatform = null;
				if (jitGuid == DetourRuntimeNET60Platform.JitVersionGuid)
				{
					detourRuntimeNETCorePlatform = new DetourRuntimeNET60Platform();
				}
				else if (jitGuid == DetourRuntimeNET50Platform.JitVersionGuid)
				{
					detourRuntimeNETCorePlatform = new DetourRuntimeNET50Platform();
				}
				else if (jitGuid == DetourRuntimeNETCore30Platform.JitVersionGuid)
				{
					detourRuntimeNETCorePlatform = new DetourRuntimeNETCore30Platform();
				}
				if (detourRuntimeNETCorePlatform == null)
				{
					return new DetourRuntimeNETCorePlatform();
				}
				if (detourRuntimeNETCorePlatform != null)
				{
					detourRuntimeNETCorePlatform.InstallJitHooks(jitObject);
				}
				return detourRuntimeNETCorePlatform;
			}
			catch (Exception ex)
			{
				MMDbgLog.Log("Could not get JIT information for the runtime, falling out to the version without JIT hooks");
				MMDbgLog.Log(string.Format("Error: {0}", ex));
			}
			return new DetourRuntimeNETCorePlatform();
		}

		private static DetourRuntimeNETCorePlatform.d_getJit getJit;

		private static bool isNet5Jit;

		private const int vtableIndex_ICorJitCompiler_getVersionIdentifier = 4;

		private const int vtableIndex_ICorJitCompiler_getVersionIdentifier_net5 = 2;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		private delegate IntPtr d_getJit();

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private delegate void d_getVersionIdentifier(IntPtr thisPtr, out Guid versionIdentifier);
	}
}
