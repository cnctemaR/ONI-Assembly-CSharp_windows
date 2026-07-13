using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule", "UnityEditor.VectorGraphicsModule" })]
	[Serializable]
	internal struct VectorImageVertex
	{
		public Vector3 position;

		public Color32 tint;

		public Vector2 uv;

		public uint settingIndex;

		public Color32 flags;

		public Vector4 circle;
	}
}
