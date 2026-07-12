using System;

namespace System.Drawing.Drawing2D
{
	public enum LineCap
	{
		Flat,
		Square,
		Round,
		Triangle,
		NoAnchor = 16,
		SquareAnchor,
		RoundAnchor,
		DiamondAnchor,
		ArrowAnchor,
		Custom = 255,
		AnchorMask = 240
	}
}
