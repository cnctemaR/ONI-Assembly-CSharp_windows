using System;

namespace UnityEngine.Animations
{
	internal static class DiscreteEvaluationAttributeUtilities
	{
		public unsafe static int ConvertFloatToDiscreteInt(float f)
		{
			float* ptr = &f;
			int* ptr2 = (int*)ptr;
			return *ptr2;
		}

		public unsafe static float ConvertDiscreteIntToFloat(int f)
		{
			int* ptr = &f;
			float* ptr2 = (float*)ptr;
			return *ptr2;
		}
	}
}
