using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.Text
{
	[NativeHeader("Modules/TextCoreTextEngine/Native/TextCoreVertex.h")]
	[UsedByNativeCode("TextCoreVertex")]
	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule", "UnityEngine.UIElementsModule" })]
	internal struct TextCoreVertex
	{
		public Vector3 position;

		public Color32 color;

		public Vector2 uv0;

		public Vector2 uv2;
	}
}
