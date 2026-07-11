using System;
using System.Collections.Generic;
using System.Linq;

namespace MIConvexHull
{
	public static class VoronoiMesh
	{
		public static VoronoiMesh<TVertex, TCell, TEdge> Create<TVertex, TCell, TEdge>(IList<TVertex> data) where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new() where TEdge : VoronoiEdge<TVertex, TCell>, new()
		{
			return VoronoiMesh<TVertex, TCell, TEdge>.Create(data);
		}

		public static VoronoiMesh<TVertex, DefaultTriangulationCell<TVertex>, VoronoiEdge<TVertex, DefaultTriangulationCell<TVertex>>> Create<TVertex>(IList<TVertex> data) where TVertex : IVertex
		{
			return VoronoiMesh<TVertex, DefaultTriangulationCell<TVertex>, VoronoiEdge<TVertex, DefaultTriangulationCell<TVertex>>>.Create(data);
		}

		public static VoronoiMesh<DefaultVertex, DefaultTriangulationCell<DefaultVertex>, VoronoiEdge<DefaultVertex, DefaultTriangulationCell<DefaultVertex>>> Create(IList<double[]> data)
		{
			return VoronoiMesh<DefaultVertex, DefaultTriangulationCell<DefaultVertex>, VoronoiEdge<DefaultVertex, DefaultTriangulationCell<DefaultVertex>>>.Create(data.Select<double[], DefaultVertex>((double[] p) => new DefaultVertex
			{
				Position = p.ToArray<double>()
			}).ToList<DefaultVertex>());
		}

		public static VoronoiMesh<TVertex, TCell, VoronoiEdge<TVertex, TCell>> Create<TVertex, TCell>(IList<TVertex> data) where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>, new()
		{
			return VoronoiMesh<TVertex, TCell, VoronoiEdge<TVertex, TCell>>.Create(data);
		}
	}
}
