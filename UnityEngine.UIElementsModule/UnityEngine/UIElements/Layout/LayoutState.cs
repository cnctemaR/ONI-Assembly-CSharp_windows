using System;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements.Layout
{
	internal struct LayoutState
	{
		public static LayoutState Default
		{
			get
			{
				return new LayoutState
				{
					measureFunctionCallback = LayoutDelegates.s_InvokeMeasureFunction,
					baselineFunctionCallback = LayoutDelegates.s_InvokeBaselineFunction
				};
			}
		}

		public IntPtr measureFunctionCallback;

		public IntPtr baselineFunctionCallback;

		public IntPtr unusedExceptionPointer;

		public uint depth;

		public uint currentGenerationCount;

		[MarshalAs(UnmanagedType.U1)]
		public bool error;
	}
}
