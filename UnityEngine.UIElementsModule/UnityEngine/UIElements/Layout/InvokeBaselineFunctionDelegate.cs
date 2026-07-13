using System;
using System.Runtime.InteropServices;

namespace UnityEngine.UIElements.Layout
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	internal delegate float InvokeBaselineFunctionDelegate(ref LayoutNode node, float width, float height);
}
