using System;
using UnityEngine.Bindings;

namespace UnityEngine.TextCore
{
	[NativeHeader("Modules/TextCoreTextEngine/Native/TextRenderingIndices.h")]
	[VisibleToOtherModules(new string[] { "UnityEngine.UIElementsModule" })]
	internal struct TextRenderingIndices
	{
		public int meshIndex;

		public int textElementInfoIndex;
	}
}
