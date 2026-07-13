using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[NativeType(Header = "Modules/UIElements/VisualNodeImguiData.h")]
	internal struct VisualNodeImguiData
	{
		[MarshalAs(UnmanagedType.U1)]
		public bool IsContainer;

		public int DescendantCount;
	}
}
