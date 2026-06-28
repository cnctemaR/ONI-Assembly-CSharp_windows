using System;
using System.Collections.Generic;
using System.Linq;

namespace MIConvexHull
{
	public class VoronoiMesh<TVertex, TCell, TEdge> where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new() where TEdge : VoronoiEdge<TVertex, TCell>, new()
	{
		private VoronoiMesh()
		{
		}

		public IEnumerable<TCell> Vertices { get; private set; }

		public IEnumerable<TEdge> Edges { get; private set; }

		public static VoronoiMesh<TVertex, TCell, TEdge> Create(IList<TVertex> data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			DelaunayTriangulation<TVertex, TCell> delaunayTriangulation = DelaunayTriangulation<TVertex, TCell>.Create(data);
			List<TCell> list = delaunayTriangulation.Cells.ToList<TCell>();
			HashSet<TEdge> hashSet = new HashSet<TEdge>(new VoronoiMesh<TVertex, TCell, TEdge>.EdgeComparer());
			foreach (TCell tcell in list)
			{
				for (int i = 0; i < tcell.Adjacency.Length; i++)
				{
					TCell tcell2 = tcell.Adjacency[i];
					if (tcell2 != null)
					{
						HashSet<TEdge> hashSet2 = hashSet;
						TEdge tedge = new TEdge();
						tedge.Source = tcell;
						tedge.Target = tcell2;
						hashSet2.Add(tedge);
					}
				}
			}
			return new VoronoiMesh<TVertex, TCell, TEdge>
			{
				Vertices = list,
				Edges = hashSet.ToList<TEdge>()
			};
		}

		private class EdgeComparer : IEqualityComparer<TEdge>
		{
			public bool Equals(TEdge x, TEdge y)
			{
				return (x.Source == y.Source && x.Target == y.Target) || (x.Source == y.Target && x.Target == y.Source);
			}

			public int GetHashCode(TEdge obj)
			{
				TCell source = obj.Source;
				int hashCode = source.GetHashCode();
				TCell target = obj.Target;
				return hashCode ^ target.GetHashCode();
			}
		}
	}
}
