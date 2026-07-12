using System;

namespace UnityEngine.UIElements
{
	[Serializable]
	public class VectorImage : ScriptableObject
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

		[SerializeField]
		internal int version = 0;

		[SerializeField]
		internal Texture2D atlas = null;

		[SerializeField]
		internal VectorImageVertex[] vertices = null;

		[SerializeField]
		internal ushort[] indices = null;

		[SerializeField]
		internal GradientSettings[] settings = null;

		[SerializeField]
		internal Vector2 size = Vector2.zero;
	}
}
