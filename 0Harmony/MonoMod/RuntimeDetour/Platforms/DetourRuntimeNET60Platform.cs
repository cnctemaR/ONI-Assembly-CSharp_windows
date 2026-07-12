using System;
using System.Runtime.InteropServices;
using MonoMod.Utils;

namespace MonoMod.RuntimeDetour.Platforms
{
	internal class DetourRuntimeNET60Platform : DetourRuntimeNETCore30Platform
	{
		protected unsafe override DetourRuntimeNETCore30Platform.CorJitResult InvokeRealCompileMethod(IntPtr thisPtr, IntPtr corJitInfo, in DetourRuntimeNETCore30Platform.CORINFO_METHOD_INFO methodInfo, uint flags, out byte* nativeEntry, out uint nativeSizeOfCode)
		{
			if (this.real_compileMethod == null)
			{
				return base.InvokeRealCompileMethod(thisPtr, corJitInfo, in methodInfo, flags, out nativeEntry, out nativeSizeOfCode);
			}
			return this.real_compileMethod(thisPtr, corJitInfo, in methodInfo, flags, out nativeEntry, out nativeSizeOfCode);
		}

		protected unsafe override IntPtr GetCompileMethodHook(IntPtr real)
		{
			if (PlatformHelper.Is(Platform.Windows) && IntPtr.Size == 4)
			{
				this.real_compileMethod = real.AsDelegate<DetourRuntimeNET60Platform.d_compileMethod_thiscall>();
				this.our_compileMethod = new DetourRuntimeNET60Platform.d_compileMethod_thiscall(base.CompileMethodHook);
				IntPtr functionPointerForDelegate = Marshal.GetFunctionPointerForDelegate<DetourRuntimeNET60Platform.d_compileMethod_thiscall>(this.our_compileMethod);
				NativeDetourData nativeDetourData = DetourRuntimeNETCore30Platform.CreateNativeTrampolineTo(functionPointerForDelegate);
				DetourRuntimeNET60Platform.d_compileMethod_thiscall d_compileMethod_thiscall = nativeDetourData.Method.AsDelegate<DetourRuntimeNET60Platform.d_compileMethod_thiscall>();
				IntPtr zero = IntPtr.Zero;
				IntPtr zero2 = IntPtr.Zero;
				DetourRuntimeNETCore30Platform.CORINFO_METHOD_INFO corinfo_METHOD_INFO = default(DetourRuntimeNETCore30Platform.CORINFO_METHOD_INFO);
				byte* ptr;
				uint num;
				d_compileMethod_thiscall(zero, zero2, in corinfo_METHOD_INFO, 0U, out ptr, out num);
				DetourRuntimeNETCore30Platform.FreeNativeTrampoline(nativeDetourData);
				return functionPointerForDelegate;
			}
			return base.GetCompileMethodHook(real);
		}

		public new static readonly Guid JitVersionGuid = new Guid("5ed35c58-857b-48dd-a818-7c0136dc9f73");

		private DetourRuntimeNET60Platform.d_compileMethod_thiscall our_compileMethod;

		private DetourRuntimeNET60Platform.d_compileMethod_thiscall real_compileMethod;

		[UnmanagedFunctionPointer(CallingConvention.ThisCall)]
		private unsafe delegate DetourRuntimeNETCore30Platform.CorJitResult d_compileMethod_thiscall(IntPtr thisPtr, IntPtr corJitInfo, in DetourRuntimeNETCore30Platform.CORINFO_METHOD_INFO methodInfo, uint flags, out byte* nativeEntry, out uint nativeSizeOfCode);
	}
}
