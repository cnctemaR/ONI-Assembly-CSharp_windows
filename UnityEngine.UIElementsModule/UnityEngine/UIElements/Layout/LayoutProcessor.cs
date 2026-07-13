using System;

namespace UnityEngine.UIElements.Layout
{
	internal static class LayoutProcessor
	{
		public static ILayoutProcessor Processor
		{
			get
			{
				return LayoutProcessor.s_Processor;
			}
			set
			{
				LayoutProcessor.s_Processor = value ?? new LayoutProcessorNative();
			}
		}

		public static void CalculateLayout(LayoutNode node, float parentWidth, float parentHeight, LayoutDirection parentDirection)
		{
			LayoutProcessor.s_Processor.CalculateLayout(node, parentWidth, parentHeight, parentDirection);
		}

		private static ILayoutProcessor s_Processor = new LayoutProcessorNative();
	}
}
