using System;
using System.Drawing.Imaging;

namespace System.Drawing
{
	internal struct GdipEncoderParameter
	{
		internal Guid guid;

		internal uint numberOfValues;

		internal EncoderParameterValueType type;

		internal IntPtr value;
	}
}
