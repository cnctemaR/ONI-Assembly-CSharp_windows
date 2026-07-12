using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[RequiredByNativeCode]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleUnloadOperation.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class AssetBundleUnloadOperation : AsyncOperation
	{
		[NativeMethod("WaitForCompletion")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public extern void WaitForCompletion();
	}
}
