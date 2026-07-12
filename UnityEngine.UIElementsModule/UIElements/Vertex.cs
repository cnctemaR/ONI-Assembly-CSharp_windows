using System;

namespace UnityEngine.UIElements
{
	public struct Vertex
	{
		public static readonly float nearZ;

		public Vector3 position;

		public Color32 tint;

		public Vector2 uv;

		internal Color32 xformClipPages;

		internal Color32 ids;

		internal Color32 flags;

		internal Color32 opacityColorPages;

		internal Color32 settingIndex;

		internal Vector4 circle;

		internal float textureId;
	}
}
