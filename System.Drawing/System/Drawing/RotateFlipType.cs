using System;

namespace System.Drawing
{
	public enum RotateFlipType
	{
		RotateNoneFlipNone,
		Rotate90FlipNone,
		Rotate180FlipNone,
		Rotate270FlipNone,
		RotateNoneFlipX,
		Rotate90FlipX,
		Rotate180FlipX,
		Rotate270FlipX,
		RotateNoneFlipY = 6,
		Rotate90FlipY,
		Rotate180FlipY = 4,
		Rotate270FlipY,
		RotateNoneFlipXY = 2,
		Rotate90FlipXY,
		Rotate180FlipXY = 0,
		Rotate270FlipXY
	}
}
