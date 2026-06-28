using System;
using System.Collections.Generic;
using NodeEditorFramework.Utilities;
using UnityEngine;

public class ResourceTreeNode : Resource
{
	public Vector2 position
	{
		get
		{
			return new Vector2(this.nodeX, this.nodeY);
		}
	}

	public Vector2 center
	{
		get
		{
			return this.position + new Vector2(this.width / 2f, -this.height / 2f);
		}
	}

	public float nodeX;

	public float nodeY;

	public float width;

	public float height;

	public List<ResourceTreeNode> references = new List<ResourceTreeNode>();

	public List<ResourceTreeNode.Edge> edges = new List<ResourceTreeNode.Edge>();

	public class Edge
	{
		public Edge(ResourceTreeNode source, ResourceTreeNode target, ResourceTreeNode.Edge.EdgeType edgeType)
		{
			this.edgeType = edgeType;
			this.source = source;
			this.target = target;
			this.path = null;
		}

		public ResourceTreeNode.Edge.EdgeType edgeType { get; private set; }

		public ResourceTreeNode source { get; private set; }

		public ResourceTreeNode target { get; private set; }

		private Vector2 SourcePos()
		{
			return this.source.center + this.sourceOffset;
		}

		private Vector2 TargetPos()
		{
			return this.target.center + this.targetOffset;
		}

		public List<Vector2> SrcTarget
		{
			get
			{
				return new List<Vector2>
				{
					this.SourcePos(),
					this.TargetPos()
				};
			}
		}

		public List<Vector2> path { get; private set; }

		public void AddToPath(Vector2f point)
		{
			if (this.path == null)
			{
				this.path = new List<Vector2>();
			}
			this.path.Add(point);
		}

		public void Render(Rect rect, float width, Color colour)
		{
			ResourceTreeNode.Edge.EdgeType edgeType = this.edgeType;
			if (edgeType != ResourceTreeNode.Edge.EdgeType.GenericEdge)
			{
			}
			RTEditorGUI.DrawLine(rect, this.SourcePos(), this.TargetPos(), colour, null, width);
		}

		public Vector2f sourceOffset = new Vector2f(0, 0);

		public Vector2f targetOffset = new Vector2f(0, 0);

		public enum EdgeType
		{
			PolyLineEdge,
			QuadCurveEdge,
			ArcEdge,
			SplineEdge,
			BezierEdge,
			GenericEdge
		}
	}
}
