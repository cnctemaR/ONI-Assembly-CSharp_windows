using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[NativeType(Header = "Modules/UIElements/VisualPanelData.h")]
	internal struct VisualPanelData
	{
		public VisualNodeHandle RootContainer;

		[MarshalAs(UnmanagedType.U1)]
		public bool DuringLayoutPhase;
	}
}
