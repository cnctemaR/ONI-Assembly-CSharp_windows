using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Accessibility
{
	[RequiredByNativeCode]
	[NativeType(CodegenOptions.Custom, "MonoAccessibilityNodeData")]
	[NativeHeader("Modules/Accessibility/Bindings/AccessibilityNodeData.bindings.h")]
	[NativeHeader("Modules/Accessibility/Native/AccessibilityNodeData.h")]
	internal struct AccessibilityNodeData
	{
		public AccessibilityNodeData()
		{
			this.nodeId = -1;
			this.parentId = -1;
			this.childIds = new int[0];
			this.isActive = true;
			this.frame = default(Rect);
			this.label = null;
			this.value = null;
			this.hint = null;
			this.role = AccessibilityRole.None;
			this.state = AccessibilityState.None;
			this.allowsDirectInteraction = false;
			this.implementsInvoked = false;
			this.implementsScrolled = false;
			this.implementsDismissed = false;
		}

		public int[] childIds { readonly get; set; }

		public string label { readonly get; set; }

		public string value { readonly get; set; }

		public string hint { readonly get; set; }

		public Rect frame { readonly get; set; }

		public int nodeId { readonly get; set; }

		public int parentId { readonly get; set; }

		public AccessibilityRole role { readonly get; set; }

		public AccessibilityState state { readonly get; set; }

		public bool isActive { readonly get; set; }

		public bool allowsDirectInteraction { readonly get; set; }

		public bool implementsInvoked { readonly get; set; }

		public bool implementsScrolled { readonly get; set; }

		public bool implementsDismissed { readonly get; set; }
	}
}
