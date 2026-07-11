using System;
using System.Collections.Generic;

namespace Satsuma
{
	public static class PathExtensions
	{
		public static bool IsCycle(this IPath path)
		{
			return path.FirstNode == path.LastNode && path.ArcCount(ArcFilter.All) > 0;
		}

		public static Node NextNode(this IPath path, Node node)
		{
			Arc arc = path.NextArc(node);
			if (arc == Arc.Invalid)
			{
				return Node.Invalid;
			}
			return path.Other(arc, node);
		}

		public static Node PrevNode(this IPath path, Node node)
		{
			Arc arc = path.PrevArc(node);
			if (arc == Arc.Invalid)
			{
				return Node.Invalid;
			}
			return path.Other(arc, node);
		}

		internal static IEnumerable<Arc> ArcsHelper(this IPath path, Node u, ArcFilter filter)
		{
			Arc arc = path.PrevArc(u);
			Arc arc2 = path.NextArc(u);
			if (arc == arc2)
			{
				arc2 = Arc.Invalid;
			}
			for (int i = 0; i < 2; i++)
			{
				Arc arc3 = ((i != 0) ? arc2 : arc);
				if (!(arc3 == Arc.Invalid))
				{
					switch (filter)
					{
					case ArcFilter.All:
						yield return arc3;
						break;
					case ArcFilter.Edge:
						if (path.IsEdge(arc3))
						{
							yield return arc3;
						}
						break;
					case ArcFilter.Forward:
						if (path.IsEdge(arc3) || path.U(arc3) == u)
						{
							yield return arc3;
						}
						break;
					case ArcFilter.Backward:
						if (path.IsEdge(arc3) || path.V(arc3) == u)
						{
							yield return arc3;
						}
						break;
					}
				}
			}
			yield break;
		}
	}
}
