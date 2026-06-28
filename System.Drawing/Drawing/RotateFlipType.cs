using System;

namespace System.Drawing
{
	public enum RotateFlipType
	{
		RotateNoneFlipNone,
		Rotate180FlipXY = 0,
		Rotate90FlipNone,
		Rotate270FlipXY = 1,
		Rotate180FlipNone,
		RotateNoneFlipXY = 2,
		Rotate270FlipNone,
		Rotate90FlipXY = 3,
		RotateNoneFlipX,
		Rotate180FlipY = 4,
		Rotate90FlipX,
		Rotate270FlipY = 5,
		Rotate180FlipX,
		RotateNoneFlipY = 6,
		Rotate270FlipX,
		Rotate90FlipY = 7
	}
}
