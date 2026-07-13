using System;
using System.Runtime.InteropServices;
using AOT;
using Unity.Profiling;

namespace UnityEngine.UIElements.Layout
{
	internal static class LayoutDelegates
	{
		[MonoPInvokeCallback(typeof(InvokeMeasureFunctionDelegate))]
		private static void InvokeMeasureFunction(ref LayoutNode node, float width, LayoutMeasureMode widthMode, float height, LayoutMeasureMode heightMode, ref IntPtr exception, out LayoutSize result)
		{
			LayoutMeasureFunction measure = node.Config.Measure;
			bool flag = measure == null;
			if (flag)
			{
				Debug.Assert(false, "No measure function set in this node's config");
				result = default(LayoutSize);
			}
			else
			{
				try
				{
					using (LayoutDelegates.s_InvokeMeasureFunctionMarker.Auto())
					{
						measure(node.GetOwner(), ref node, width, widthMode, height, heightMode, out result);
					}
				}
				catch (Exception ex)
				{
					GCHandle gchandle = GCHandle.Alloc(ex);
					exception = GCHandle.ToIntPtr(gchandle);
					result = default(LayoutSize);
				}
			}
		}

		[MonoPInvokeCallback(typeof(InvokeBaselineFunctionDelegate))]
		private static float InvokeBaselineFunction(ref LayoutNode node, float width, float height)
		{
			LayoutBaselineFunction baseline = node.Config.Baseline;
			bool flag = baseline == null;
			float num;
			if (flag)
			{
				Debug.Assert(false, "No baselineFunction function set in this node's config");
				num = 0f;
			}
			else
			{
				using (LayoutDelegates.s_InvokeBaselineFunctionMarker.Auto())
				{
					num = baseline(ref node, width, height);
				}
			}
			return num;
		}

		private static readonly ProfilerMarker s_InvokeMeasureFunctionMarker = new ProfilerMarker("InvokeMeasureFunction");

		private static readonly ProfilerMarker s_InvokeBaselineFunctionMarker = new ProfilerMarker("InvokeBaselineFunction");

		private static readonly InvokeMeasureFunctionDelegate s_InvokeMeasureDelegate = new InvokeMeasureFunctionDelegate(LayoutDelegates.InvokeMeasureFunction);

		private static readonly InvokeBaselineFunctionDelegate s_InvokeBaselineDelegate = new InvokeBaselineFunctionDelegate(LayoutDelegates.InvokeBaselineFunction);

		internal static readonly IntPtr s_InvokeMeasureFunction = Marshal.GetFunctionPointerForDelegate<InvokeMeasureFunctionDelegate>(LayoutDelegates.s_InvokeMeasureDelegate);

		internal static readonly IntPtr s_InvokeBaselineFunction = Marshal.GetFunctionPointerForDelegate<InvokeBaselineFunctionDelegate>(LayoutDelegates.s_InvokeBaselineDelegate);
	}
}
