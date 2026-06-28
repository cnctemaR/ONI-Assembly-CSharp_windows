using System;

namespace System.Drawing.Imaging
{
	public enum EncoderValue
	{
		ColorTypeCMYK,
		ColorTypeYCCK,
		CompressionCCITT3 = 3,
		CompressionCCITT4,
		CompressionLZW = 2,
		CompressionNone = 6,
		CompressionRle = 5,
		Flush = 20,
		FrameDimensionPage = 23,
		FrameDimensionResolution = 22,
		FrameDimensionTime = 21,
		LastFrame = 19,
		MultiFrame = 18,
		RenderNonProgressive = 12,
		RenderProgressive = 11,
		ScanMethodInterlaced = 7,
		ScanMethodNonInterlaced,
		TransformFlipHorizontal = 16,
		TransformFlipVertical,
		TransformRotate180 = 14,
		TransformRotate270,
		TransformRotate90 = 13,
		VersionGif87 = 9,
		VersionGif89
	}
}
