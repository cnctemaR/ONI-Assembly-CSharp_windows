using System;
using System.Collections.Generic;

namespace MIConvexHull
{
	public class DelaunayTriangulation<TVertex, TCell> : ITriangulation<TVertex, TCell> where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new()
	{
		private DelaunayTriangulation()
		{
		}

		public IEnumerable<TCell> Cells { get; private set; }

		public static DelaunayTriangulation<TVertex, TCell> Create(IList<TVertex> data)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}
			DelaunayTriangulation<TVertex, TCell> delaunayTriangulation;
			if (data.Count == 0)
			{
				delaunayTriangulation = new DelaunayTriangulation<TVertex, TCell>
				{
					Cells = new TCell[0]
				};
			}
			else
			{
				TCell[] delaunayTriangulation2 = ConvexHullAlgorithm.GetDelaunayTriangulation<TVertex, TCell>(data);
				delaunayTriangulation = new DelaunayTriangulation<TVertex, TCell>
				{
					Cells = delaunayTriangulation2
				};
			}
			return delaunayTriangulation;
		}
	}
}
