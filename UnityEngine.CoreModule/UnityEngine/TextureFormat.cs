using System;
using System.ComponentModel;

namespace UnityEngine
{
	public enum TextureFormat
	{
		Alpha8 = 1,
		ARGB4444,
		RGB24,
		RGBA32,
		ARGB32,
		RGB565 = 7,
		R16 = 9,
		DXT1,
		DXT5 = 12,
		RGBA4444,
		BGRA32,
		RHalf,
		RGHalf,
		RGBAHalf,
		RFloat,
		RGFloat,
		RGBAFloat,
		YUY2,
		RGB9e5Float,
		BC4 = 26,
		BC5,
		BC6H = 24,
		BC7,
		DXT1Crunched = 28,
		DXT5Crunched,
		[Obsolete("Texture compression format PVRTC has been deprecated and will be removed in a future release")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		PVRTC_RGB2,
		[Obsolete("Texture compression format PVRTC has been deprecated and will be removed in a future release")]
		[EditorBrowsable(EditorBrowsableState.Never)]
		PVRTC_RGBA2,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Texture compression format PVRTC has been deprecated and will be removed in a future release")]
		PVRTC_RGB4,
		[EditorBrowsable(EditorBrowsableState.Never)]
		[Obsolete("Texture compression format PVRTC has been deprecated and will be removed in a future release")]
		PVRTC_RGBA4,
		ETC_RGB4,
		EAC_R = 41,
		EAC_R_SIGNED,
		EAC_RG,
		EAC_RG_SIGNED,
		ETC2_RGB,
		ETC2_RGBA1,
		ETC2_RGBA8,
		ASTC_4x4,
		ASTC_5x5,
		ASTC_6x6,
		ASTC_8x8,
		ASTC_10x10,
		ASTC_12x12,
		RG16 = 62,
		R8,
		ETC_RGB4Crunched,
		ETC2_RGBA8Crunched,
		ASTC_HDR_4x4,
		ASTC_HDR_5x5,
		ASTC_HDR_6x6,
		ASTC_HDR_8x8,
		ASTC_HDR_10x10,
		ASTC_HDR_12x12,
		RG32,
		RGB48,
		RGBA64,
		R8_SIGNED,
		RG16_SIGNED,
		RGB24_SIGNED,
		RGBA32_SIGNED,
		R16_SIGNED,
		RG32_SIGNED,
		RGB48_SIGNED,
		RGBA64_SIGNED
	}
}
