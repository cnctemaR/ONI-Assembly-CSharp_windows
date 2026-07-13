using System;
using UnityEngine;

namespace TMPro
{
	public struct TMP_Vertex
	{
		public static TMP_Vertex zero
		{
			get
			{
				return TMP_Vertex.k_Zero;
			}
		}

		public Vector3 position;

		public Vector4 uv;

		public Vector2 uv2;

		public Color32 color;

		private static readonly TMP_Vertex k_Zero;
	}
}
