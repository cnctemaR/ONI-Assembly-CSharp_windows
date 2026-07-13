using System;

namespace UnityEngine.UIElements.Layout
{
	internal struct LayoutDefaults
	{
		// Note: this type is marked as 'beforefieldinit'.
		unsafe static LayoutDefaults()
		{
			FixedBuffer9<LayoutValue> fixedBuffer = default(FixedBuffer9<LayoutValue>);
			*fixedBuffer[0] = LayoutValue.Undefined();
			*fixedBuffer[1] = LayoutValue.Undefined();
			*fixedBuffer[2] = LayoutValue.Undefined();
			*fixedBuffer[3] = LayoutValue.Undefined();
			*fixedBuffer[4] = LayoutValue.Undefined();
			*fixedBuffer[5] = LayoutValue.Undefined();
			*fixedBuffer[6] = LayoutValue.Undefined();
			*fixedBuffer[7] = LayoutValue.Undefined();
			*fixedBuffer[8] = LayoutValue.Undefined();
			LayoutDefaults.EdgeValuesUnit = fixedBuffer;
			LayoutDefaults.DimensionValues = new float[] { float.NaN, float.NaN };
			FixedBuffer2<LayoutValue> fixedBuffer2 = default(FixedBuffer2<LayoutValue>);
			*fixedBuffer2[0] = LayoutValue.Undefined();
			*fixedBuffer2[1] = LayoutValue.Undefined();
			LayoutDefaults.DimensionValuesUnit = fixedBuffer2;
			fixedBuffer2 = default(FixedBuffer2<LayoutValue>);
			*fixedBuffer2[0] = LayoutValue.Auto();
			*fixedBuffer2[1] = LayoutValue.Auto();
			LayoutDefaults.DimensionValuesAutoUnit = fixedBuffer2;
		}

		public static readonly FixedBuffer9<LayoutValue> EdgeValuesUnit;

		public static readonly float[] DimensionValues;

		public static readonly FixedBuffer2<LayoutValue> DimensionValuesUnit;

		public static readonly FixedBuffer2<LayoutValue> DimensionValuesAutoUnit;
	}
}
