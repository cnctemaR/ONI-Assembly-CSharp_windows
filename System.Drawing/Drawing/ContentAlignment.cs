using System;
using System.ComponentModel;
using System.Drawing.Design;

namespace System.Drawing
{
	[Editor("System.Drawing.Design.ContentAlignmentEditor, System.Drawing.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	public enum ContentAlignment
	{
		TopLeft = 1,
		TopCenter,
		TopRight = 4,
		MiddleLeft = 16,
		MiddleCenter = 32,
		MiddleRight = 64,
		BottomLeft = 256,
		BottomCenter = 512,
		BottomRight = 1024
	}
}
