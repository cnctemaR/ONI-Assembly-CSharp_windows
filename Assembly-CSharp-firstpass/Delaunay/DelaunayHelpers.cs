using System;
using System.Collections.Generic;
using Delaunay.Geo;
using Delaunay.LR;
using UnityEngine;

namespace Delaunay
{
	public static class DelaunayHelpers
	{
		public static List<LineSegment> VisibleLineSegments(List<Edge> edges)
		{
			List<LineSegment> list = new List<LineSegment>();
			for (int i = 0; i < edges.Count; i++)
			{
				Edge edge = edges[i];
				if (edge.visible)
				{
					Vector2? vector = edge.clippedEnds[Side.LEFT];
					Vector2? vector2 = edge.clippedEnds[Side.RIGHT];
					list.Add(new LineSegment(vector, vector2));
				}
			}
			return list;
		}

		public static List<DelaunayHelpers.LineSegmentWithSites> VisibleLineSegmentsWithSite(List<Edge> edges)
		{
			List<DelaunayHelpers.LineSegmentWithSites> list = new List<DelaunayHelpers.LineSegmentWithSites>();
			for (int i = 0; i < edges.Count; i++)
			{
				Edge edge = edges[i];
				if (edge.visible)
				{
					Vector2? vector = edge.clippedEnds[Side.LEFT];
					Vector2? vector2 = edge.clippedEnds[Side.RIGHT];
					list.Add(new DelaunayHelpers.LineSegmentWithSites(vector, vector2, edge.leftSite.color, edge.rightSite.color));
				}
			}
			return list;
		}

		public static List<Edge> SelectEdgesForSitePoint(Vector2 coord, List<Edge> edgesToTest)
		{
			return edgesToTest.FindAll((Edge edge) => (edge.leftSite != null && edge.leftSite.Coord == coord) || (edge.rightSite != null && edge.rightSite.Coord == coord));
		}

		public static List<Edge> SelectNonIntersectingEdges(List<Edge> edgesToTest)
		{
			return edgesToTest;
		}

		public static List<LineSegment> DelaunayLinesForEdges(List<Edge> edges)
		{
			List<LineSegment> list = new List<LineSegment>();
			for (int i = 0; i < edges.Count; i++)
			{
				Edge edge = edges[i];
				list.Add(edge.DelaunayLine());
			}
			return list;
		}

		public static List<LineSegment> Kruskal(List<LineSegment> lineSegments, KruskalType type = KruskalType.MINIMUM)
		{
			Dictionary<Vector2?, Node> dictionary = new Dictionary<Vector2?, Node>();
			List<LineSegment> list = new List<LineSegment>();
			Stack<Node> pool = Node.pool;
			if (type != KruskalType.MAXIMUM)
			{
				lineSegments.Sort((LineSegment l1, LineSegment l2) => LineSegment.CompareLengths_MAX(l1, l2));
			}
			else
			{
				lineSegments.Sort((LineSegment l1, LineSegment l2) => LineSegment.CompareLengths(l1, l2));
			}
			int num = lineSegments.Count;
			while (--num > -1)
			{
				LineSegment lineSegment = lineSegments[num];
				Node node2;
				if (!dictionary.ContainsKey(lineSegment.p0))
				{
					Node node = ((pool.Count <= 0) ? new Node() : pool.Pop());
					node2 = (node.parent = node);
					node.treeSize = 1;
					dictionary[lineSegment.p0] = node;
				}
				else
				{
					Node node = dictionary[lineSegment.p0];
					node2 = DelaunayHelpers.Find(node);
				}
				Node node4;
				if (!dictionary.ContainsKey(lineSegment.p1))
				{
					Node node3 = ((pool.Count <= 0) ? new Node() : pool.Pop());
					node4 = (node3.parent = node3);
					node3.treeSize = 1;
					dictionary[lineSegment.p1] = node3;
				}
				else
				{
					Node node3 = dictionary[lineSegment.p1];
					node4 = DelaunayHelpers.Find(node3);
				}
				if (node2 != node4)
				{
					list.Add(lineSegment);
					int treeSize = node2.treeSize;
					int treeSize2 = node4.treeSize;
					if (treeSize >= treeSize2)
					{
						node4.parent = node2;
						node2.treeSize += treeSize2;
					}
					else
					{
						node2.parent = node4;
						node4.treeSize += treeSize;
					}
				}
			}
			foreach (Node node5 in dictionary.Values)
			{
				pool.Push(node5);
			}
			return list;
		}

		private static Node Find(Node node)
		{
			Node node2;
			if (node.parent == node)
			{
				node2 = node;
			}
			else
			{
				Node node3 = DelaunayHelpers.Find(node.parent);
				node.parent = node3;
				node2 = node3;
			}
			return node2;
		}

		public class LineSegmentWithSites : LineSegment
		{
			public LineSegmentWithSites(Vector2? p0, Vector2? p1, uint id0, uint id1)
				: base(p0, p1)
			{
				this.id0 = id0;
				this.id1 = id1;
			}

			public uint id0 { get; private set; }

			public uint id1 { get; private set; }
		}
	}
}
