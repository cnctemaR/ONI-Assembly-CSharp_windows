using System;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore.Text
{
	[NativeHeader("Modules/TextCoreTextEngine/Native/TextElementInfo.h")]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule", "UnityEngine.IMGUIModule" })]
	internal struct NativeTextElementInfo
	{
		public int glyphID;

		public TextCoreVertex bottomLeft;

		public TextCoreVertex topLeft;

		public TextCoreVertex topRight;

		public TextCoreVertex bottomRight;
	}
}
