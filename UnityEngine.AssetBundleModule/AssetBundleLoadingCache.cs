using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Modules/AssetBundle/Public/AssetBundleLoadingCache.h")]
	[UsedByNativeCode]
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	internal static class AssetBundleLoadingCache
	{
		internal static extern uint maxBlocksPerFile
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern uint blockCount
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		internal static extern uint blockSize
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
		}

		internal static uint memoryBudgetKB
		{
			get
			{
				return AssetBundleLoadingCache.blockCount * AssetBundleLoadingCache.blockSize;
			}
			set
			{
				uint num = Math.Max(value / AssetBundleLoadingCache.blockSize, 2U);
				uint num2 = Math.Max(AssetBundleLoadingCache.blockCount / 4U, 2U);
				bool flag = num != AssetBundleLoadingCache.blockCount || num2 != AssetBundleLoadingCache.maxBlocksPerFile;
				if (flag)
				{
					AssetBundleLoadingCache.blockCount = num;
					AssetBundleLoadingCache.maxBlocksPerFile = num2;
				}
			}
		}

		internal const int kMinAllowedBlockCount = 2;

		internal const int kMinAllowedMaxBlocksPerFile = 2;
	}
}
