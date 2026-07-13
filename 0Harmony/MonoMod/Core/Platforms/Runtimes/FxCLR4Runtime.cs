using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using MonoMod.Core.Interop;
using MonoMod.Utils;

namespace MonoMod.Core.Platforms.Runtimes
{
	[NullableContext(1)]
	[Nullable(0)]
	internal sealed class FxCLR4Runtime : FxBaseRuntime
	{
		public FxCLR4Runtime(ISystem system)
		{
			this.system = system;
			if (PlatformDetection.Architecture == ArchitectureKind.x86_64 && (PlatformDetection.RuntimeVersion.Revision >= 17379 || PlatformDetection.RuntimeVersion.Minor >= 5))
			{
				Abi? defaultAbi = system.DefaultAbi;
				if (defaultAbi != null)
				{
					Abi valueOrDefault = defaultAbi.GetValueOrDefault();
					this.AbiCore = new Abi?(FxCoreBaseRuntime.AbiForCoreFx45X64(valueOrDefault));
				}
			}
		}

		public override RuntimeFeature Features
		{
			get
			{
				return base.Features & ~RuntimeFeature.RequiresBodyThunkWalking;
			}
		}

		private unsafe IntPtr GetMethodBodyPtr(MethodBase method, RuntimeMethodHandle handle)
		{
			Fx.V48.MethodDesc* ptr = (Fx.V48.MethodDesc*)(void*)handle.Value;
			ptr = Fx.V48.MethodDesc.FindTightlyBoundWrappedMethodDesc(ptr);
			return (IntPtr)ptr->GetNativeCode();
		}

		public override IntPtr GetMethodEntryPoint(MethodBase method)
		{
			method = this.GetIdentifiable(method);
			RuntimeMethodHandle methodHandle = this.GetMethodHandle(method);
			bool flag = false;
			IntPtr intPtr;
			for (;;)
			{
				Helpers.Assert(base.TryInvokeBclCompileMethod(methodHandle), null, "TryInvokeBclCompileMethod(handle)");
				methodHandle.GetFunctionPointer();
				intPtr = this.GetMethodBodyPtr(method, methodHandle);
				if (!(intPtr == IntPtr.Zero))
				{
					return intPtr;
				}
				if (flag)
				{
					break;
				}
				Helpers.Assert(base.TryInvokeBclCompileMethod(methodHandle), null, "TryInvokeBclCompileMethod(handle)");
				flag = true;
			}
			intPtr = methodHandle.GetFunctionPointer();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(59, 1);
			defaultInterpolatedStringHandler.AppendLiteral("Could not get entry point normally, GetFunctionPointer() = ");
			defaultInterpolatedStringHandler.AppendFormatted<IntPtr>(intPtr, "x16");
			throw new InvalidOperationException(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		private ISystem system;
	}
}
