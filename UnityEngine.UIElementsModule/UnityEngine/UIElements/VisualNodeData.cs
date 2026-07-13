using System;
using System.Runtime.InteropServices;
using UnityEngine.Bindings;
using UnityEngine.UIElements.Layout;

namespace UnityEngine.UIElements
{
	[NativeType(Header = "Modules/UIElements/VisualNodeData.h")]
	internal struct VisualNodeData
	{
		public VisualPanelHandle Panel;

		public VisualNodeHandle LogicalParent;

		public VisualElementFlags Flags;

		public VisualNodeCallbackInterest CallbackInterest;

		public LayoutNode LayoutNode;

		public uint ControlId;

		[MarshalAs(UnmanagedType.U1)]
		public bool Enabled;

		[MarshalAs(UnmanagedType.U1)]
		public bool IsRootVisualContainer;
	}
}
