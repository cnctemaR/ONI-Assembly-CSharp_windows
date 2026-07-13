using System;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
	[NativeHeader("Runtime/Graphics/TextureFormat.h")]
	[NativeHeader("Runtime/Graphics/GraphicsFormatUtility.bindings.h")]
	[NativeHeader("Runtime/Graphics/Format.h")]
	public class GraphicsFormatUtility
	{
		[FreeFunction("GetGraphicsFormat_Native_Texture")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern GraphicsFormat GetFormat([NotNull("NullExceptionObject")] Texture texture);

		public static GraphicsFormat GetGraphicsFormat(TextureFormat format, bool isSRGB)
		{
			return GraphicsFormatUtility.GetGraphicsFormat_Native_TextureFormat(format, isSRGB);
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsFormat GetGraphicsFormat_Native_TextureFormat(TextureFormat format, bool isSRGB);

		public static TextureFormat GetTextureFormat(GraphicsFormat format)
		{
			return GraphicsFormatUtility.GetTextureFormat_Native_GraphicsFormat(format);
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureFormat GetTextureFormat_Native_GraphicsFormat(GraphicsFormat format);

		public static GraphicsFormat GetGraphicsFormat(RenderTextureFormat format, bool isSRGB)
		{
			return GraphicsFormatUtility.GetGraphicsFormat_Native_RenderTextureFormat(format, isSRGB);
		}

		[FreeFunction(IsThreadSafe = false)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsFormat GetGraphicsFormat_Native_RenderTextureFormat(RenderTextureFormat format, bool isSRGB);

		public static GraphicsFormat GetGraphicsFormat(RenderTextureFormat format, RenderTextureReadWrite readWrite)
		{
			bool flag = QualitySettings.activeColorSpace == ColorSpace.Linear;
			bool flag2 = ((readWrite == RenderTextureReadWrite.Default) ? flag : (readWrite == RenderTextureReadWrite.sRGB));
			return GraphicsFormatUtility.GetGraphicsFormat(format, flag2);
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern GraphicsFormat GetDepthStencilFormatFromBitsLegacy_Native(int minimumDepthBits);

		internal static GraphicsFormat GetDepthStencilFormat(int minimumDepthBits)
		{
			return GraphicsFormatUtility.GetDepthStencilFormatFromBitsLegacy_Native(minimumDepthBits);
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern int GetDepthBits(GraphicsFormat format);

		public static GraphicsFormat GetDepthStencilFormat(int minimumDepthBits, int minimumStencilBits)
		{
			bool flag = minimumDepthBits == 0 && minimumStencilBits == 0;
			GraphicsFormat graphicsFormat;
			if (flag)
			{
				graphicsFormat = GraphicsFormat.None;
			}
			else
			{
				bool flag2 = minimumDepthBits < 0 || minimumStencilBits < 0;
				if (flag2)
				{
					throw new ArgumentException("Number of bits in DepthStencil format can't be negative.");
				}
				bool flag3 = minimumDepthBits > 32;
				if (flag3)
				{
					throw new ArgumentException("Number of depth buffer bits cannot exceed 32.");
				}
				bool flag4 = minimumStencilBits > 8;
				if (flag4)
				{
					throw new ArgumentException("Number of stencil buffer bits cannot exceed 8.");
				}
				bool flag5 = minimumDepthBits == 0;
				if (flag5)
				{
					minimumDepthBits = 0;
				}
				else
				{
					bool flag6 = minimumDepthBits <= 16;
					if (flag6)
					{
						minimumDepthBits = 16;
					}
					else
					{
						bool flag7 = minimumDepthBits <= 24;
						if (flag7)
						{
							minimumDepthBits = 24;
						}
						else
						{
							minimumDepthBits = 32;
						}
					}
				}
				bool flag8 = minimumStencilBits != 0;
				if (flag8)
				{
					minimumStencilBits = 8;
				}
				Debug.Assert(GraphicsFormatUtility.tableNoStencil.Length == GraphicsFormatUtility.tableStencil.Length);
				GraphicsFormat[] array = ((minimumStencilBits > 0) ? GraphicsFormatUtility.tableStencil : GraphicsFormatUtility.tableNoStencil);
				int num = minimumDepthBits / 8;
				for (int i = num; i < array.Length; i++)
				{
					GraphicsFormat graphicsFormat2 = array[i];
					bool flag9 = SystemInfo.IsFormatSupported(graphicsFormat2, FormatUsage.Render);
					if (flag9)
					{
						return graphicsFormat2;
					}
				}
				graphicsFormat = GraphicsFormat.None;
			}
			return graphicsFormat;
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsSRGBFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsSwizzleFormat(GraphicsFormat format);

		public static bool IsSwizzleFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsSwizzleFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GraphicsFormat GetSRGBFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GraphicsFormat GetLinearFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern RenderTextureFormat GetRenderTextureFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetColorComponentCount(GraphicsFormat format);

		public static uint GetColorComponentCount(TextureFormat format)
		{
			return GraphicsFormatUtility.GetColorComponentCount(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetAlphaComponentCount(GraphicsFormat format);

		public static uint GetAlphaComponentCount(TextureFormat format)
		{
			return GraphicsFormatUtility.GetAlphaComponentCount(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetComponentCount(GraphicsFormat format);

		public static uint GetComponentCount(TextureFormat format)
		{
			return GraphicsFormatUtility.GetComponentCount(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern string GetFormatString(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetFormatString_Native_TextureFormat(TextureFormat format);

		public static string GetFormatString(TextureFormat format)
		{
			return GraphicsFormatUtility.GetFormatString_Native_TextureFormat(format);
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsCompressedFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsCompressedFormat_Native_TextureFormat(TextureFormat format);

		[Obsolete("IsCompressedTextureFormat is obsolete, please use IsCompressedFormat instead.")]
		internal static bool IsCompressedTextureFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsCompressedFormat(format);
		}

		public static bool IsCompressedFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsCompressedFormat_Native_TextureFormat(format);
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CanDecompressFormat(GraphicsFormat format, bool wholeImage);

		internal static bool CanDecompressFormat(GraphicsFormat format)
		{
			return GraphicsFormatUtility.CanDecompressFormat(format, true);
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsPackedFormat(GraphicsFormat format);

		public static bool IsPackedFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsPackedFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool Is16BitPackedFormat(GraphicsFormat format);

		public static bool Is16BitPackedFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.Is16BitPackedFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern GraphicsFormat ConvertToAlphaFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern TextureFormat ConvertToAlphaFormat_Native_TextureFormat(TextureFormat format);

		public static TextureFormat ConvertToAlphaFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.ConvertToAlphaFormat_Native_TextureFormat(format);
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsAlphaOnlyFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsAlphaOnlyFormat_Native_TextureFormat(TextureFormat format);

		public static bool IsAlphaOnlyFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsAlphaOnlyFormat_Native_TextureFormat(format);
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsAlphaTestFormat(GraphicsFormat format);

		public static bool IsAlphaTestFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsAlphaTestFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool HasAlphaChannel(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool HasAlphaChannel_Native_TextureFormat(TextureFormat format);

		public static bool HasAlphaChannel(TextureFormat format)
		{
			return GraphicsFormatUtility.HasAlphaChannel_Native_TextureFormat(format);
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsDepthFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsStencilFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsDepthStencilFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsIEEE754Format(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsFloatFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsHalfFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsUnsignedFormat(GraphicsFormat format);

		public static bool IsUnsignedFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsUnsignedFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsSignedFormat(GraphicsFormat format);

		public static bool IsSignedFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsSignedFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsNormFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsUNormFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsSNormFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsIntegerFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsUIntFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsSIntFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsXRFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsDXTCFormat(GraphicsFormat format);

		public static bool IsDXTCFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsDXTCFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsRGTCFormat(GraphicsFormat format);

		public static bool IsRGTCFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsRGTCFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsBPTCFormat(GraphicsFormat format);

		public static bool IsBPTCFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsBPTCFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsBCFormat(GraphicsFormat format);

		public static bool IsBCFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsBCFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsPVRTCFormat(GraphicsFormat format);

		public static bool IsPVRTCFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsPVRTCFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsETCFormat(GraphicsFormat format);

		public static bool IsETCFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsETCFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsEACFormat(GraphicsFormat format);

		public static bool IsEACFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsEACFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsASTCFormat(GraphicsFormat format);

		public static bool IsASTCFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsASTCFormat(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsHDRFormat(GraphicsFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsHDRFormat_Native_TextureFormat(TextureFormat format);

		public static bool IsHDRFormat(TextureFormat format)
		{
			return GraphicsFormatUtility.IsHDRFormat_Native_TextureFormat(format);
		}

		[FreeFunction("IsCompressedCrunchTextureFormat", IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern bool IsCrunchFormat(TextureFormat format);

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern FormatSwizzle GetSwizzleR(GraphicsFormat format);

		public static FormatSwizzle GetSwizzleR(TextureFormat format)
		{
			return GraphicsFormatUtility.GetSwizzleR(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern FormatSwizzle GetSwizzleG(GraphicsFormat format);

		public static FormatSwizzle GetSwizzleG(TextureFormat format)
		{
			return GraphicsFormatUtility.GetSwizzleG(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern FormatSwizzle GetSwizzleB(GraphicsFormat format);

		public static FormatSwizzle GetSwizzleB(TextureFormat format)
		{
			return GraphicsFormatUtility.GetSwizzleB(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern FormatSwizzle GetSwizzleA(GraphicsFormat format);

		public static FormatSwizzle GetSwizzleA(TextureFormat format)
		{
			return GraphicsFormatUtility.GetSwizzleA(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetBlockSize(GraphicsFormat format);

		public static uint GetBlockSize(TextureFormat format)
		{
			return GraphicsFormatUtility.GetBlockSize(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetBlockWidth(GraphicsFormat format);

		public static uint GetBlockWidth(TextureFormat format)
		{
			return GraphicsFormatUtility.GetBlockWidth(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern uint GetBlockHeight(GraphicsFormat format);

		public static uint GetBlockHeight(TextureFormat format)
		{
			return GraphicsFormatUtility.GetBlockHeight(GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		public static uint ComputeMipmapSize(int width, int height, GraphicsFormat format)
		{
			return GraphicsFormatUtility.ComputeMipChainSize_Native_2D(width, height, format, 1);
		}

		public static uint ComputeMipmapSize(int width, int height, TextureFormat format)
		{
			return GraphicsFormatUtility.ComputeMipmapSize(width, height, GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint ComputeMipChainSize_Native_2D(int width, int height, GraphicsFormat format, int mipCount);

		public static uint ComputeMipChainSize(int width, int height, GraphicsFormat format, [DefaultValue("-1")] int mipCount = -1)
		{
			return GraphicsFormatUtility.ComputeMipChainSize_Native_2D(width, height, format, mipCount);
		}

		public static uint ComputeMipChainSize(int width, int height, TextureFormat format, [DefaultValue("-1")] int mipCount = -1)
		{
			return GraphicsFormatUtility.ComputeMipChainSize_Native_2D(width, height, GraphicsFormatUtility.GetGraphicsFormat(format, false), mipCount);
		}

		public static uint ComputeMipmapSize(int width, int height, int depth, GraphicsFormat format)
		{
			return GraphicsFormatUtility.ComputeMipChainSize_Native_3D(width, height, depth, format, 1);
		}

		public static uint ComputeMipmapSize(int width, int height, int depth, TextureFormat format)
		{
			return GraphicsFormatUtility.ComputeMipmapSize(width, height, depth, GraphicsFormatUtility.GetGraphicsFormat(format, false));
		}

		[FreeFunction(IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern uint ComputeMipChainSize_Native_3D(int width, int height, int depth, GraphicsFormat format, int mipCount);

		public static uint ComputeMipChainSize(int width, int height, int depth, GraphicsFormat format, [DefaultValue("-1")] int mipCount = -1)
		{
			return GraphicsFormatUtility.ComputeMipChainSize_Native_3D(width, height, depth, format, mipCount);
		}

		public static uint ComputeMipChainSize(int width, int height, int depth, TextureFormat format, [DefaultValue("-1")] int mipCount = -1)
		{
			return GraphicsFormatUtility.ComputeMipChainSize_Native_3D(width, height, depth, GraphicsFormatUtility.GetGraphicsFormat(format, false), mipCount);
		}

		private static readonly GraphicsFormat[] tableNoStencil = new GraphicsFormat[]
		{
			GraphicsFormat.None,
			GraphicsFormat.D16_UNorm,
			GraphicsFormat.D16_UNorm,
			GraphicsFormat.D24_UNorm,
			GraphicsFormat.D32_SFloat
		};

		private static readonly GraphicsFormat[] tableStencil = new GraphicsFormat[]
		{
			GraphicsFormat.S8_UInt,
			GraphicsFormat.D16_UNorm_S8_UInt,
			GraphicsFormat.D16_UNorm_S8_UInt,
			GraphicsFormat.D24_UNorm_S8_UInt,
			GraphicsFormat.D32_SFloat_S8_UInt
		};
	}
}
