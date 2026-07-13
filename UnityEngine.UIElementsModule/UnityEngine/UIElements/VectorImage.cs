using System;
using UnityEngine.Bindings;

namespace UnityEngine.UIElements
{
	[Serializable]
	public sealed class VectorImage : ScriptableObject
	{
		public float width
		{
			get
			{
				return this.size.x;
			}
		}

		public float height
		{
			get
			{
				return this.size.y;
			}
		}

		private void OnDestroy()
		{
			bool flag = this.atlas != null;
			if (flag)
			{
				UIRUtility.Destroy(this.atlas);
			}
		}

		[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule" })]
		[SerializeField]
		internal int version = 0;

		[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule", "UnityEditor.VectorGraphicsModule" })]
		[SerializeField]
		internal Texture2D atlas = null;

		[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule", "UnityEditor.VectorGraphicsModule" })]
		[SerializeField]
		internal VectorImageVertex[] vertices = null;

		[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule" })]
		[SerializeField]
		internal ushort[] indices = null;

		[SerializeField]
		[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule" })]
		internal GradientSettings[] settings = null;

		[SerializeField]
		[VisibleToOtherModules(new string[] { "UnityEngine.VectorGraphicsModule" })]
		internal Vector2 size = Vector2.zero;
	}
}
