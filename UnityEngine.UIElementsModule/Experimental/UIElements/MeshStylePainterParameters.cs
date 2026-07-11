using System;

namespace UnityEngine.Experimental.UIElements
{
	internal struct MeshStylePainterParameters
	{
		public static MeshStylePainterParameters GetDefault(Mesh mesh, Material mat)
		{
			return new MeshStylePainterParameters
			{
				mesh = mesh,
				material = mat
			};
		}

		public Mesh mesh;

		public Material material;

		public int pass;
	}
}
