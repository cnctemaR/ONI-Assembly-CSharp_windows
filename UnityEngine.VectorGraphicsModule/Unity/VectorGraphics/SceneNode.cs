using System;
using System.Collections.Generic;

namespace Unity.VectorGraphics
{
	public class SceneNode
	{
		public List<SceneNode> Children { get; set; }

		public List<Shape> Shapes { get; set; }

		public Matrix2D Transform
		{
			get
			{
				return this.m_Transform;
			}
			set
			{
				this.m_Transform = value;
			}
		}

		public SceneNode Clipper { get; set; }

		private Matrix2D m_Transform = Matrix2D.identity;
	}
}
