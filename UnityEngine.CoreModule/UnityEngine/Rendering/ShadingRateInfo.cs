using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering
{
	[NativeHeader("Runtime/Graphics/ShadingRateInfo.h")]
	public static class ShadingRateInfo
	{
		public static bool supportsPerImageTile
		{
			get
			{
				return ShadingRateInfo.SupportsPerImageTile();
			}
		}

		public static bool supportsPerDrawCall
		{
			get
			{
				return ShadingRateInfo.SupportsPerDrawCall();
			}
		}

		public static Vector2Int imageTileSize
		{
			get
			{
				return ShadingRateInfo.GetImageTileSize();
			}
		}

		public static ShadingRateFragmentSize[] availableFragmentSizes
		{
			get
			{
				return ShadingRateInfo.GetAvailableFragmentSizes();
			}
		}

		public static GraphicsFormat graphicsFormat
		{
			get
			{
				return ShadingRateInfo.GetGraphicsFormat();
			}
		}

		[FreeFunction("ShadingRateInfo::QueryNativeValue")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern byte QueryNativeValue(ShadingRateFragmentSize fragmentSize);

		[FreeFunction("ShadingRateInfo::SupportsPerImageTile")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsPerImageTile();

		[FreeFunction("ShadingRateInfo::SupportsPerDrawCall")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool SupportsPerDrawCall();

		[FreeFunction("ShadingRateInfo::GetImageTileSize")]
		private static Vector2Int GetImageTileSize()
		{
			Vector2Int vector2Int;
			ShadingRateInfo.GetImageTileSize_Injected(out vector2Int);
			return vector2Int;
		}

		[FreeFunction("ShadingRateInfo::GetAvailableFragmentSizes")]
		private static ShadingRateFragmentSize[] GetAvailableFragmentSizes()
		{
			ShadingRateFragmentSize[] array2;
			try
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ShadingRateInfo.GetAvailableFragmentSizes_Injected(out blittableArrayWrapper);
			}
			finally
			{
				BlittableArrayWrapper blittableArrayWrapper;
				ShadingRateFragmentSize[] array;
				blittableArrayWrapper.Unmarshal<ShadingRateFragmentSize>(ref array);
				array2 = array;
			}
			return array2;
		}

		[FreeFunction("ShadingRateInfo::GetGraphicsFormat")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsFormat GetGraphicsFormat();

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetImageTileSize_Injected(out Vector2Int ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void GetAvailableFragmentSizes_Injected(out BlittableArrayWrapper ret);
	}
}
