using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.TextCore.Text
{
	[NativeHeader("Modules/TextCoreTextEngine/Native/IMGUI/MeshInfo.h")]
	[VisibleToOtherModules(new string[] { "UnityEngine.IMGUIModule" })]
	[UsedByNativeCode("MeshInfo")]
	internal struct MeshInfoBindings
	{
		public TextCoreVertex[] vertexData;

		public Material material;

		public int vertexCount;
	}
}
