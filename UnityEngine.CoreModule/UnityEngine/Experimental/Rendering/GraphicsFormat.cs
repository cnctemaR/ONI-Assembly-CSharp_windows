using System;

namespace UnityEngine.Experimental.Rendering
{
	/// <summary>
	///   <para>Use this format to create either Textures or RenderTextures from scripts.</para>
	/// </summary>
	public enum GraphicsFormat
	{
		/// <summary>
		///   <para>The format is not specified.</para>
		/// </summary>
		None,
		/// <summary>
		///   <para>A one-component, 8-bit unsigned normalized format that has a single 8-bit R component stored with sRGB nonlinear encoding.</para>
		/// </summary>
		R8_SRGB,
		/// <summary>
		///   <para>A two-component, 16-bit unsigned normalized format that has an 8-bit R component stored with sRGB nonlinear encoding in byte 0, and an 8-bit G component stored with sRGB nonlinear encoding in byte 1.</para>
		/// </summary>
		R8G8_SRGB,
		/// <summary>
		///   <para>A three-component, 24-bit unsigned normalized format that has an 8-bit R component stored with sRGB nonlinear encoding in byte 0, an 8-bit G component stored with sRGB nonlinear encoding in byte 1, and an 8-bit B component stored with sRGB nonlinear encoding in byte 2.</para>
		/// </summary>
		R8G8B8_SRGB,
		/// <summary>
		///   <para>A four-component, 32-bit unsigned normalized format that has an 8-bit R component stored with sRGB nonlinear encoding in byte 0, an 8-bit G component stored with sRGB nonlinear encoding in byte 1, an 8-bit B component stored with sRGB nonlinear encoding in byte 2, and an 8-bit A component in byte 3.</para>
		/// </summary>
		R8G8B8A8_SRGB,
		/// <summary>
		///   <para>A one-component, 8-bit unsigned normalized format that has a single 8-bit R component.</para>
		/// </summary>
		R8_UNorm,
		/// <summary>
		///   <para>A two-component, 16-bit unsigned normalized format that has an 8-bit R component stored with sRGB nonlinear encoding in byte 0, and an 8-bit G component stored with sRGB nonlinear encoding in byte 1.</para>
		/// </summary>
		R8G8_UNorm,
		/// <summary>
		///   <para>A three-component, 24-bit unsigned normalized format that has an 8-bit R component in byte 0, an 8-bit G component in byte 1, and an 8-bit B component in byte 2.</para>
		/// </summary>
		R8G8B8_UNorm,
		/// <summary>
		///   <para>A four-component, 32-bit unsigned normalized format that has an 8-bit R component in byte 0, an 8-bit G component in byte 1, an 8-bit B component in byte 2, and an 8-bit A component in byte 3.</para>
		/// </summary>
		R8G8B8A8_UNorm,
		/// <summary>
		///   <para>A one-component, 8-bit signed normalized format that has a single 8-bit R component.</para>
		/// </summary>
		R8_SNorm,
		/// <summary>
		///   <para>A two-component, 16-bit signed normalized format that has an 8-bit R component stored with sRGB nonlinear encoding in byte 0, and an 8-bit G component stored with sRGB nonlinear encoding in byte 1.</para>
		/// </summary>
		R8G8_SNorm,
		/// <summary>
		///   <para>A three-component, 24-bit signed normalized format that has an 8-bit R component in byte 0, an 8-bit G component in byte 1, and an 8-bit B component in byte 2.</para>
		/// </summary>
		R8G8B8_SNorm,
		/// <summary>
		///   <para>A four-component, 32-bit signed normalized format that has an 8-bit R component in byte 0, an 8-bit G component in byte 1, an 8-bit B component in byte 2, and an 8-bit A component in byte 3.</para>
		/// </summary>
		R8G8B8A8_SNorm,
		/// <summary>
		///   <para>A one-component, 8-bit unsigned integer format that has a single 8-bit R component.</para>
		/// </summary>
		R8_UInt,
		/// <summary>
		///   <para>A two-component, 16-bit unsigned integer format that has an 8-bit R component in byte 0, and an 8-bit G component in byte 1.</para>
		/// </summary>
		R8G8_UInt,
		/// <summary>
		///   <para>A three-component, 24-bit unsigned integer format that has an 8-bit R component in byte 0, an 8-bit G component in byte 1, and an 8-bit B component in byte 2.</para>
		/// </summary>
		R8G8B8_UInt,
		/// <summary>
		///   <para>A four-component, 32-bit unsigned integer format that has an 8-bit R component in byte 0, an 8-bit G component in byte 1, an 8-bit B component in byte 2, and an 8-bit A component in byte 3.</para>
		/// </summary>
		R8G8B8A8_UInt,
		/// <summary>
		///   <para>A one-component, 8-bit signed integer format that has a single 8-bit R component.</para>
		/// </summary>
		R8_SInt,
		/// <summary>
		///   <para>A two-component, 16-bit signed integer format that has an 8-bit R component in byte 0, and an 8-bit G component in byte 1.</para>
		/// </summary>
		R8G8_SInt,
		/// <summary>
		///   <para>A three-component, 24-bit signed integer format that has an 8-bit R component in byte 0, an 8-bit G component in byte 1, and an 8-bit B component in byte 2.</para>
		/// </summary>
		R8G8B8_SInt,
		/// <summary>
		///   <para>A four-component, 32-bit signed integer format that has an 8-bit R component in byte 0, an 8-bit G component in byte 1, an 8-bit B component in byte 2, and an 8-bit A component in byte 3.</para>
		/// </summary>
		R8G8B8A8_SInt,
		/// <summary>
		///   <para>A one-component, 16-bit unsigned normalized format that has a single 16-bit R component.</para>
		/// </summary>
		R16_UNorm,
		/// <summary>
		///   <para>A two-component, 32-bit unsigned normalized format that has a 16-bit R component in bytes 0..1, and a 16-bit G component in bytes 2..3.</para>
		/// </summary>
		R16G16_UNorm,
		/// <summary>
		///   <para>A three-component, 48-bit unsigned normalized format that has a 16-bit R component in bytes 0..1, a 16-bit G component in bytes 2..3, and a 16-bit B component in bytes 4..5.</para>
		/// </summary>
		R16G16B16_UNorm,
		/// <summary>
		///   <para>A four-component, 64-bit unsigned normalized format that has a 16-bit R component in bytes 0..1, a 16-bit G component in bytes 2..3, a 16-bit B component in bytes 4..5, and a 16-bit A component in bytes 6..7.</para>
		/// </summary>
		R16G16B16A16_UNorm,
		/// <summary>
		///   <para>A one-component, 16-bit signed normalized format that has a single 16-bit R component.</para>
		/// </summary>
		R16_SNorm,
		/// <summary>
		///   <para>A two-component, 32-bit signed normalized format that has a 16-bit R component in bytes 0..1, and a 16-bit G component in bytes 2..3.</para>
		/// </summary>
		R16G16_SNorm,
		/// <summary>
		///   <para>A three-component, 48-bit signed normalized format that has a 16-bit R component in bytes 0..1, a 16-bit G component in bytes 2..3, and a 16-bit B component in bytes 4..5.</para>
		/// </summary>
		R16G16B16_SNorm,
		/// <summary>
		///   <para>A four-component, 64-bit signed normalized format that has a 16-bit R component in bytes 0..1, a 16-bit G component in bytes 2..3, a 16-bit B component in bytes 4..5, and a 16-bit A component in bytes 6..7.</para>
		/// </summary>
		R16G16B16A16_SNorm,
		/// <summary>
		///   <para>A one-component, 16-bit unsigned integer format that has a single 16-bit R component.</para>
		/// </summary>
		R16_UInt,
		/// <summary>
		///   <para>A two-component, 32-bit unsigned integer format that has a 16-bit R component in bytes 0..1, and a 16-bit G component in bytes 2..3.</para>
		/// </summary>
		R16G16_UInt,
		/// <summary>
		///   <para>A three-component, 48-bit unsigned integer format that has a 16-bit R component in bytes 0..1, a 16-bit G component in bytes 2..3, and a 16-bit B component in bytes 4..5.</para>
		/// </summary>
		R16G16B16_UInt,
		/// <summary>
		///   <para>A four-component, 64-bit unsigned integer format that has a 16-bit R component in bytes 0..1, a 16-bit G component in bytes 2..3, a 16-bit B component in bytes 4..5, and a 16-bit A component in bytes 6..7.</para>
		/// </summary>
		R16G16B16A16_UInt,
		/// <summary>
		///   <para>A one-component, 16-bit signed integer format that has a single 16-bit R component.</para>
		/// </summary>
		R16_SInt,
		/// <summary>
		///   <para>A two-component, 32-bit signed integer format that has a 16-bit R component in bytes 0..1, and a 16-bit G component in bytes 2..3.</para>
		/// </summary>
		R16G16_SInt,
		/// <summary>
		///   <para>A three-component, 48-bit signed integer format that has a 16-bit R component in bytes 0..1, a 16-bit G component in bytes 2..3, and a 16-bit B component in bytes 4..5.</para>
		/// </summary>
		R16G16B16_SInt,
		/// <summary>
		///   <para>A four-component, 64-bit signed integer format that has a 16-bit R component in bytes 0..1, a 16-bit G component in bytes 2..3, a 16-bit B component in bytes 4..5, and a 16-bit A component in bytes 6..7.</para>
		/// </summary>
		R16G16B16A16_SInt,
		/// <summary>
		///   <para>A one-component, 32-bit unsigned integer format that has a single 32-bit R component.</para>
		/// </summary>
		R32_UInt,
		/// <summary>
		///   <para>A two-component, 64-bit unsigned integer format that has a 32-bit R component in bytes 0..3, and a 32-bit G component in bytes 4..7.</para>
		/// </summary>
		R32G32_UInt,
		/// <summary>
		///   <para>A three-component, 96-bit unsigned integer format that has a 32-bit R component in bytes 0..3, a 32-bit G component in bytes 4..7, and a 32-bit B component in bytes 8..11.</para>
		/// </summary>
		R32G32B32_UInt,
		/// <summary>
		///   <para>A four-component, 128-bit unsigned integer format that has a 32-bit R component in bytes 0..3, a 32-bit G component in bytes 4..7, a 32-bit B component in bytes 8..11, and a 32-bit A component in bytes 12..15.</para>
		/// </summary>
		R32G32B32A32_UInt,
		/// <summary>
		///   <para>A one-component, 32-bit signed integer format that has a single 32-bit R component.</para>
		/// </summary>
		R32_SInt,
		/// <summary>
		///   <para>A two-component, 64-bit signed integer format that has a 32-bit R component in bytes 0..3, and a 32-bit G component in bytes 4..7.</para>
		/// </summary>
		R32G32_SInt,
		/// <summary>
		///   <para>A three-component, 96-bit signed integer format that has a 32-bit R component in bytes 0..3, a 32-bit G component in bytes 4..7, and a 32-bit B component in bytes 8..11.</para>
		/// </summary>
		R32G32B32_SInt,
		/// <summary>
		///   <para>A four-component, 128-bit signed integer format that has a 32-bit R component in bytes 0..3, a 32-bit G component in bytes 4..7, a 32-bit B component in bytes 8..11, and a 32-bit A component in bytes 12..15.</para>
		/// </summary>
		R32G32B32A32_SInt,
		/// <summary>
		///   <para>A one-component, 16-bit signed floating-point format that has a single 16-bit R component.</para>
		/// </summary>
		R16_SFloat,
		/// <summary>
		///   <para>A two-component, 32-bit signed floating-point format that has a 16-bit R component in bytes 0..1, and a 16-bit G component in bytes 2..3.</para>
		/// </summary>
		R16G16_SFloat,
		/// <summary>
		///   <para>A three-component, 48-bit signed floating-point format that has a 16-bit R component in bytes 0..1, a 16-bit G component in bytes 2..3, and a 16-bit B component in bytes 4..5.</para>
		/// </summary>
		R16G16B16_SFloat,
		/// <summary>
		///   <para>A four-component, 64-bit signed floating-point format that has a 16-bit R component in bytes 0..1, a 16-bit G component in bytes 2..3, a 16-bit B component in bytes 4..5, and a 16-bit A component in bytes 6..7.</para>
		/// </summary>
		R16G16B16A16_SFloat,
		/// <summary>
		///   <para>A one-component, 32-bit signed floating-point format that has a single 32-bit R component.</para>
		/// </summary>
		R32_SFloat,
		/// <summary>
		///   <para>A two-component, 64-bit signed floating-point format that has a 32-bit R component in bytes 0..3, and a 32-bit G component in bytes 4..7.</para>
		/// </summary>
		R32G32_SFloat,
		/// <summary>
		///   <para>A three-component, 96-bit signed floating-point format that has a 32-bit R component in bytes 0..3, a 32-bit G component in bytes 4..7, and a 32-bit B component in bytes 8..11.</para>
		/// </summary>
		R32G32B32_SFloat,
		/// <summary>
		///   <para>A four-component, 128-bit signed floating-point format that has a 32-bit R component in bytes 0..3, a 32-bit G component in bytes 4..7, a 32-bit B component in bytes 8..11, and a 32-bit A component in bytes 12..15.</para>
		/// </summary>
		R32G32B32A32_SFloat,
		/// <summary>
		///   <para>A three-component, 24-bit unsigned normalized format that has an 8-bit R component stored with sRGB nonlinear encoding in byte 0, an 8-bit G component stored with sRGB nonlinear encoding in byte 1, and an 8-bit B component stored with sRGB nonlinear encoding in byte 2.</para>
		/// </summary>
		B8G8R8_SRGB = 56,
		/// <summary>
		///   <para>A four-component, 32-bit unsigned normalized format that has an 8-bit B component stored with sRGB nonlinear encoding in byte 0, an 8-bit G component stored with sRGB nonlinear encoding in byte 1, an 8-bit R component stored with sRGB nonlinear encoding in byte 2, and an 8-bit A component in byte 3.</para>
		/// </summary>
		B8G8R8A8_SRGB,
		/// <summary>
		///   <para>A three-component, 24-bit unsigned normalized format that has an 8-bit B component in byte 0, an 8-bit G component in byte 1, and an 8-bit R component in byte 2.</para>
		/// </summary>
		B8G8R8_UNorm,
		/// <summary>
		///   <para>A four-component, 32-bit unsigned normalized format that has an 8-bit B component in byte 0, an 8-bit G component in byte 1, an 8-bit R component in byte 2, and an 8-bit A component in byte 3.</para>
		/// </summary>
		B8G8R8A8_UNorm,
		/// <summary>
		///   <para>A three-component, 24-bit signed normalized format that has an 8-bit B component in byte 0, an 8-bit G component in byte 1, and an 8-bit R component in byte 2.</para>
		/// </summary>
		B8G8R8_SNorm,
		/// <summary>
		///   <para>A four-component, 32-bit signed normalized format that has an 8-bit B component in byte 0, an 8-bit G component in byte 1, an 8-bit R component in byte 2, and an 8-bit A component in byte 3.</para>
		/// </summary>
		B8G8R8A8_SNorm,
		/// <summary>
		///   <para>A three-component, 24-bit unsigned integer format that has an 8-bit B component in byte 0, an 8-bit G component in byte 1, and an 8-bit R component in byte 2</para>
		/// </summary>
		B8G8R8_UInt,
		/// <summary>
		///   <para>A four-component, 32-bit unsigned integer format that has an 8-bit B component in byte 0, an 8-bit G component in byte 1, an 8-bit R component in byte 2, and an 8-bit A component in byte 3.</para>
		/// </summary>
		B8G8R8A8_UInt,
		/// <summary>
		///   <para>A three-component, 24-bit signed integer format that has an 8-bit B component in byte 0, an 8-bit G component in byte 1, and an 8-bit R component in byte 2.</para>
		/// </summary>
		B8G8R8_SInt,
		/// <summary>
		///   <para>A four-component, 32-bit signed integer format that has an 8-bit B component in byte 0, an 8-bit G component in byte 1, an 8-bit R component in byte 2, and an 8-bit A component in byte 3.</para>
		/// </summary>
		B8G8R8A8_SInt,
		/// <summary>
		///   <para>A four-component, 16-bit packed unsigned normalized format that has a 4-bit R component in bits 12..15, a 4-bit G component in bits 8..11, a 4-bit B component in bits 4..7, and a 4-bit A component in bits 0..3.</para>
		/// </summary>
		R4G4B4A4_UNormPack16,
		/// <summary>
		///   <para>A four-component, 16-bit packed unsigned normalized format that has a 4-bit B component in bits 12..15, a 4-bit G component in bits 8..11, a 4-bit R component in bits 4..7, and a 4-bit A component in bits 0..3.</para>
		/// </summary>
		B4G4R4A4_UNormPack16,
		/// <summary>
		///   <para>A three-component, 16-bit packed unsigned normalized format that has a 5-bit R component in bits 11..15, a 6-bit G component in bits 5..10, and a 5-bit B component in bits 0..4.</para>
		/// </summary>
		R5G6B5_UNormPack16,
		/// <summary>
		///   <para>A three-component, 16-bit packed unsigned normalized format that has a 5-bit B component in bits 11..15, a 6-bit G component in bits 5..10, and a 5-bit R component in bits 0..4.</para>
		/// </summary>
		B5G6R5_UNormPack16,
		/// <summary>
		///   <para>A four-component, 16-bit packed unsigned normalized format that has a 5-bit R component in bits 11..15, a 5-bit G component in bits 6..10, a 5-bit B component in bits 1..5, and a 1-bit A component in bit 0.</para>
		/// </summary>
		R5G5B5A1_UNormPack16,
		/// <summary>
		///   <para>A four-component, 16-bit packed unsigned normalized format that has a 5-bit B component in bits 11..15, a 5-bit G component in bits 6..10, a 5-bit R component in bits 1..5, and a 1-bit A component in bit 0.</para>
		/// </summary>
		B5G5R5A1_UNormPack16,
		/// <summary>
		///   <para>A four-component, 16-bit packed unsigned normalized format that has a 1-bit A component in bit 15, a 5-bit R component in bits 10..14, a 5-bit G component in bits 5..9, and a 5-bit B component in bits 0..4.</para>
		/// </summary>
		A1R5G5B5_UNormPack16,
		/// <summary>
		///   <para>A three-component, 32-bit packed unsigned floating-point format that has a 5-bit shared exponent in bits 27..31, a 9-bit B component mantissa in bits 18..26, a 9-bit G component mantissa in bits 9..17, and a 9-bit R component mantissa in bits 0..8.</para>
		/// </summary>
		E5B9G9R9_UFloatPack32,
		/// <summary>
		///   <para>A three-component, 32-bit packed unsigned floating-point format that has a 10-bit B component in bits 22..31, an 11-bit G component in bits 11..21, an 11-bit R component in bits 0..10. </para>
		/// </summary>
		B10G11R11_UFloatPack32,
		/// <summary>
		///   <para>A four-component, 32-bit packed unsigned normalized format that has a 2-bit A component in bits 30..31, a 10-bit B component in bits 20..29, a 10-bit G component in bits 10..19, and a 10-bit R component in bits 0..9.</para>
		/// </summary>
		A2B10G10R10_UNormPack32,
		/// <summary>
		///   <para>A four-component, 32-bit packed unsigned integer format that has a 2-bit A component in bits 30..31, a 10-bit B component in bits 20..29, a 10-bit G component in bits 10..19, and a 10-bit R component in bits 0..9.</para>
		/// </summary>
		A2B10G10R10_UIntPack32,
		/// <summary>
		///   <para>A four-component, 32-bit packed signed integer format that has a 2-bit A component in bits 30..31, a 10-bit B component in bits 20..29, a 10-bit G component in bits 10..19, and a 10-bit R component in bits 0..9.</para>
		/// </summary>
		A2B10G10R10_SIntPack32,
		/// <summary>
		///   <para>A four-component, 32-bit packed unsigned normalized format that has a 2-bit A component in bits 30..31, a 10-bit R component in bits 20..29, a 10-bit G component in bits 10..19, and a 10-bit B component in bits 0..9.</para>
		/// </summary>
		A2R10G10B10_UNormPack32,
		/// <summary>
		///   <para>A four-component, 32-bit packed unsigned integer format that has a 2-bit A component in bits 30..31, a 10-bit R component in bits 20..29, a 10-bit G component in bits 10..19, and a 10-bit B component in bits 0..9.</para>
		/// </summary>
		A2R10G10B10_UIntPack32,
		/// <summary>
		///   <para>A four-component, 32-bit packed signed integer format that has a 2-bit A component in bits 30..31, a 10-bit R component in bits 20..29, a 10-bit G component in bits 10..19, and a 10-bit B component in bits 0..9.</para>
		/// </summary>
		A2R10G10B10_SIntPack32,
		/// <summary>
		///   <para>A four-component, 32-bit packed unsigned normalized format that has a 2-bit A component in bits 30..31, a 10-bit R component in bits 20..29, a 10-bit G component in bits 10..19, and a 10-bit B component in bits 0..9. The components are gamma encoded and their values range from -0.5271 to 1.66894. The alpha component is clamped to either 0.0 or 1.0 on sampling, rendering, and writing operations.</para>
		/// </summary>
		A2R10G10B10_XRSRGBPack32,
		/// <summary>
		///   <para>A four-component, 32-bit packed unsigned normalized format that has a 2-bit A component in bits 30..31, a 10-bit R component in bits 20..29, a 10-bit G component in bits 10..19, and a 10-bit B component in bits 0..9. The components are linearly encoded and their values range from -0.752941 to 1.25098 (pre-expansion). The alpha component is clamped to either 0.0 or 1.0 on sampling, rendering, and writing operations.</para>
		/// </summary>
		A2R10G10B10_XRUNormPack32,
		/// <summary>
		///   <para>A four-component, 32-bit packed unsigned normalized format that has a 10-bit R component in bits 20..29, a 10-bit G component in bits 10..19, and a 10-bit B component in bits 0..9. The components are gamma encoded and their values range from -0.5271 to 1.66894. The alpha component is clamped to either 0.0 or 1.0 on sampling, rendering, and writing operations.</para>
		/// </summary>
		R10G10B10_XRSRGBPack32,
		/// <summary>
		///   <para>A four-component, 32-bit packed unsigned normalized format that has a 10-bit R component in bits 20..29, a 10-bit G component in bits 10..19, and a 10-bit B component in bits 0..9. The components are linearly encoded and their values range from -0.752941 to 1.25098 (pre-expansion).</para>
		/// </summary>
		R10G10B10_XRUNormPack32,
		/// <summary>
		///   <para>A four-component, 64-bit packed unsigned normalized format that has a 10-bit A component in bits 30..39, a 10-bit R component in bits 20..29, a 10-bit G component in bits 10..19, and a 10-bit B component in bits 0..9. The components are gamma encoded and their values range from -0.5271 to 1.66894. The alpha component is clamped to either 0.0 or 1.0 on sampling, rendering, and writing operations.</para>
		/// </summary>
		A10R10G10B10_XRSRGBPack32,
		/// <summary>
		///   <para>A four-component, 64-bit packed unsigned normalized format that has a 10-bit A component in bits 30..39, a 10-bit R component in bits 20..29, a 10-bit G component in bits 10..19, and a 10-bit B component in bits 0..9. The components are linearly encoded and their values range from -0.752941 to 1.25098 (pre-expansion). The alpha component is clamped to either 0.0 or 1.0 on sampling, rendering, and writing operations.</para>
		/// </summary>
		A10R10G10B10_XRUNormPack32,
		/// <summary>
		///   <para>A one-component, 16-bit unsigned normalized format that has a single 16-bit depth component.</para>
		/// </summary>
		D16_UNorm = 90,
		/// <summary>
		///   <para>A two-component, 32-bit format that has 24 unsigned normalized bits in the depth component and, optionally: 8 bits that are unused.</para>
		/// </summary>
		D24_UNorm,
		/// <summary>
		///   <para>A two-component, 32-bit packed format that has 8 unsigned integer bits in the stencil component, and 24 unsigned normalized bits in the depth component.</para>
		/// </summary>
		D24_UNorm_S8_UInt,
		/// <summary>
		///   <para>A one-component, 32-bit signed floating-point format that has 32-bits in the depth component.</para>
		/// </summary>
		D32_SFloat,
		/// <summary>
		///   <para>A two-component format that has 32 signed float bits in the depth component and 8 unsigned integer bits in the stencil component. There are optionally: 24-bits that are unused.</para>
		/// </summary>
		D32_SFloat_S8_Uint,
		/// <summary>
		///   <para>A one-component, 8-bit unsigned integer format that has 8-bits in the stencil component.</para>
		/// </summary>
		S8_Uint,
		/// <summary>
		///   <para>A three-component, block-compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGB texel data with sRGB nonlinear encoding. This format has no alpha and is considered opaque.</para>
		/// </summary>
		RGB_DXT1_SRGB,
		/// <summary>
		///   <para>A three-component, block-compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGB texel data. This format has no alpha and is considered opaque.</para>
		/// </summary>
		RGB_DXT1_UNorm,
		/// <summary>
		///   <para>A four-component, block-compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGBA texel data with the first 64 bits encoding alpha values followed by 64 bits encoding RGB values with sRGB nonlinear encoding.</para>
		/// </summary>
		RGBA_DXT3_SRGB,
		/// <summary>
		///   <para>A four-component, block-compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGBA texel data with the first 64 bits encoding alpha values followed by 64 bits encoding RGB values.</para>
		/// </summary>
		RGBA_DXT3_UNorm,
		/// <summary>
		///   <para>A four-component, block-compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGBA texel data with the first 64 bits encoding alpha values followed by 64 bits encoding RGB values with sRGB nonlinear encoding.</para>
		/// </summary>
		RGBA_DXT5_SRGB,
		/// <summary>
		///   <para>A four-component, block-compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGBA texel data with the first 64 bits encoding alpha values followed by 64 bits encoding RGB values.</para>
		/// </summary>
		RGBA_DXT5_UNorm,
		/// <summary>
		///   <para>A one-component, block-compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized red texel data.</para>
		/// </summary>
		R_BC4_UNorm,
		/// <summary>
		///   <para>A one-component, block-compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of signed normalized red texel data.</para>
		/// </summary>
		R_BC4_SNorm,
		/// <summary>
		///   <para>A two-component, block-compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RG texel data with the first 64 bits encoding red values followed by 64 bits encoding green values.</para>
		/// </summary>
		RG_BC5_UNorm,
		/// <summary>
		///   <para>A two-component, block-compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of signed normalized RG texel data with the first 64 bits encoding red values followed by 64 bits encoding green values.</para>
		/// </summary>
		RG_BC5_SNorm,
		/// <summary>
		///   <para>A three-component, block-compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of unsigned floating-point RGB texel data.</para>
		/// </summary>
		RGB_BC6H_UFloat,
		/// <summary>
		///   <para>A three-component, block-compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of signed floating-point RGB texel data.</para>
		/// </summary>
		RGB_BC6H_SFloat,
		/// <summary>
		///   <para>A four-component, block-compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGBA texel data with sRGB nonlinear encoding applied to the RGB components.</para>
		/// </summary>
		RGBA_BC7_SRGB,
		/// <summary>
		///   <para>A four-component, block-compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGBA texel data.</para>
		/// </summary>
		RGBA_BC7_UNorm,
		/// <summary>
		///   <para>A three-component, PVRTC compressed format where each 64-bit compressed texel block encodes a 8×4 rectangle of unsigned normalized RGB texel data with sRGB nonlinear encoding. This format has no alpha and is considered opaque.</para>
		/// </summary>
		RGB_PVRTC_2Bpp_SRGB,
		/// <summary>
		///   <para>A three-component, PVRTC compressed format where each 64-bit compressed texel block encodes a 8×4 rectangle of unsigned normalized RGB texel data. This format has no alpha and is considered opaque.</para>
		/// </summary>
		RGB_PVRTC_2Bpp_UNorm,
		/// <summary>
		///   <para>A three-component, PVRTC compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGB texel data with sRGB nonlinear encoding. This format has no alpha and is considered opaque.</para>
		/// </summary>
		RGB_PVRTC_4Bpp_SRGB,
		/// <summary>
		///   <para>A three-component, PVRTC compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGB texel data. This format has no alpha and is considered opaque.</para>
		/// </summary>
		RGB_PVRTC_4Bpp_UNorm,
		/// <summary>
		///   <para>A four-component, PVRTC compressed format where each 64-bit compressed texel block encodes a 8×4 rectangle of unsigned normalized RGBA texel data with the first 32 bits encoding alpha values followed by 32 bits encoding RGB values with sRGB nonlinear encoding applied.</para>
		/// </summary>
		RGBA_PVRTC_2Bpp_SRGB,
		/// <summary>
		///   <para>A four-component, PVRTC compressed format where each 64-bit compressed texel block encodes a 8×4 rectangle of unsigned normalized RGBA texel data with the first 32 bits encoding alpha values followed by 32 bits encoding RGB values.</para>
		/// </summary>
		RGBA_PVRTC_2Bpp_UNorm,
		/// <summary>
		///   <para>A four-component, PVRTC compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGBA texel data with the first 32 bits encoding alpha values followed by 32 bits encoding RGB values with sRGB nonlinear encoding applied.</para>
		/// </summary>
		RGBA_PVRTC_4Bpp_SRGB,
		/// <summary>
		///   <para>A four-component, PVRTC compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGBA texel data with the first 32 bits encoding alpha values followed by 32 bits encoding RGB values.</para>
		/// </summary>
		RGBA_PVRTC_4Bpp_UNorm,
		/// <summary>
		///   <para>A three-component, ETC compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGB texel data. This format has no alpha and is considered opaque.</para>
		/// </summary>
		RGB_ETC_UNorm,
		/// <summary>
		///   <para>A three-component, ETC2 compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGB texel data with sRGB nonlinear encoding. This format has no alpha and is considered opaque.</para>
		/// </summary>
		RGB_ETC2_SRGB,
		/// <summary>
		///   <para>A three-component, ETC2 compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGB texel data. This format has no alpha and is considered opaque.</para>
		/// </summary>
		RGB_ETC2_UNorm,
		/// <summary>
		///   <para>A four-component, ETC2 compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGB texel data with sRGB nonlinear encoding, and provides 1 bit of alpha.</para>
		/// </summary>
		RGB_A1_ETC2_SRGB,
		/// <summary>
		///   <para>A four-component, ETC2 compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGB texel data, and provides 1 bit of alpha.</para>
		/// </summary>
		RGB_A1_ETC2_UNorm,
		/// <summary>
		///   <para>A four-component, ETC2 compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGBA texel data with the first 64 bits encoding alpha values followed by 64 bits encoding RGB values with sRGB nonlinear encoding applied.</para>
		/// </summary>
		RGBA_ETC2_SRGB,
		/// <summary>
		///   <para>A four-component, ETC2 compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGBA texel data with the first 64 bits encoding alpha values followed by 64 bits encoding RGB values.</para>
		/// </summary>
		RGBA_ETC2_UNorm,
		/// <summary>
		///   <para>A one-component, ETC2 compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized red texel data.</para>
		/// </summary>
		R_EAC_UNorm,
		/// <summary>
		///   <para>A one-component, ETC2 compressed format where each 64-bit compressed texel block encodes a 4×4 rectangle of signed normalized red texel data.</para>
		/// </summary>
		R_EAC_SNorm,
		/// <summary>
		///   <para>A two-component, ETC2 compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RG texel data with the first 64 bits encoding red values followed by 64 bits encoding green values.</para>
		/// </summary>
		RG_EAC_UNorm,
		/// <summary>
		///   <para>A two-component, ETC2 compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of signed normalized RG texel data with the first 64 bits encoding red values followed by 64 bits encoding green values.</para>
		/// </summary>
		RG_EAC_SNorm,
		/// <summary>
		///   <para>A four-component, ASTC compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGBA texel data with sRGB nonlinear encoding applied to the RGB components.</para>
		/// </summary>
		RGBA_ASTC4X4_SRGB,
		/// <summary>
		///   <para>A four-component, ASTC compressed format where each 128-bit compressed texel block encodes a 4×4 rectangle of unsigned normalized RGBA texel data.</para>
		/// </summary>
		RGBA_ASTC4X4_UNorm,
		/// <summary>
		///   <para>A four-component, ASTC compressed format where each 128-bit compressed texel block encodes a 5×5 rectangle of unsigned normalized RGBA texel data with sRGB nonlinear encoding applied to the RGB components.</para>
		/// </summary>
		RGBA_ASTC5X5_SRGB,
		/// <summary>
		///   <para>A four-component, ASTC compressed format where each 128-bit compressed texel block encodes a 5×5 rectangle of unsigned normalized RGBA texel data.</para>
		/// </summary>
		RGBA_ASTC5X5_UNorm,
		/// <summary>
		///   <para>A four-component, ASTC compressed format where each 128-bit compressed texel block encodes a 6×6 rectangle of unsigned normalized RGBA texel data with sRGB nonlinear encoding applied to the RGB components.</para>
		/// </summary>
		RGBA_ASTC6X6_SRGB,
		/// <summary>
		///   <para>A four-component, ASTC compressed format where each 128-bit compressed texel block encodes a 6×6 rectangle of unsigned normalized RGBA texel data.</para>
		/// </summary>
		RGBA_ASTC6X6_UNorm,
		/// <summary>
		///   <para>A four-component, ASTC compressed format where each 128-bit compressed texel block encodes an 8×8 rectangle of unsigned normalized RGBA texel data with sRGB nonlinear encoding applied to the RGB components.</para>
		/// </summary>
		RGBA_ASTC8X8_SRGB,
		/// <summary>
		///   <para>A four-component, ASTC compressed format where each 128-bit compressed texel block encodes an 8×8 rectangle of unsigned normalized RGBA texel data.</para>
		/// </summary>
		RGBA_ASTC8X8_UNorm,
		/// <summary>
		///   <para>A four-component, ASTC compressed format where each 128-bit compressed texel block encodes a 10×10 rectangle of unsigned normalized RGBA texel data with sRGB nonlinear encoding applied to the RGB components.</para>
		/// </summary>
		RGBA_ASTC10X10_SRGB,
		/// <summary>
		///   <para>A four-component, ASTC compressed format where each 128-bit compressed texel block encodes a 10×10 rectangle of unsigned normalized RGBA texel data.</para>
		/// </summary>
		RGBA_ASTC10X10_UNorm,
		/// <summary>
		///   <para>A four-component, ASTC compressed format where each 128-bit compressed texel block encodes a 12×12 rectangle of unsigned normalized RGBA texel data with sRGB nonlinear encoding applied to the RGB components.</para>
		/// </summary>
		RGBA_ASTC12X12_SRGB,
		/// <summary>
		///   <para>A four-component, ASTC compressed format where each 128-bit compressed texel block encodes a 12×12 rectangle of unsigned normalized RGBA texel data.</para>
		/// </summary>
		RGBA_ASTC12X12_UNorm
	}
}
