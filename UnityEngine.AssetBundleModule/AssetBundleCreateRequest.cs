using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	/// <summary>
	///   <para>Asynchronous create request for an AssetBundle.</para>
	/// </summary>
	[RequiredByNativeCode]
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadFromAsyncOperation.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class AssetBundleCreateRequest : AsyncOperation
	{
		/// <summary>
		///   <para>Asset object being loaded (Read Only).</para>
		/// </summary>
		public extern AssetBundle assetBundle
		{
			[NativeMethod("GetAssetBundleBlocking")]
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		[NativeMethod("SetEnableCompatibilityChecks")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void SetEnableCompatibilityChecks(bool set);

		internal void DisableCompatibilityChecks()
		{
			this.SetEnableCompatibilityChecks(false);
		}
	}
}
