using System;

namespace MIConvexHull
{
	public class VoronoiEdge<TVertex, TCell> where TVertex : IVertex where TCell : TriangulationCell<TVertex, TCell>
	{
		public VoronoiEdge()
		{
		}

		public VoronoiEdge(TCell source, TCell target)
		{
			this.Source = source;
			this.Target = target;
		}

		public TCell Source { get; internal set; }

		public TCell Target { get; internal set; }

		public override bool Equals(object obj)
		{
			VoronoiEdge<TVertex, TCell> voronoiEdge = obj as VoronoiEdge<TVertex, TCell>;
			return voronoiEdge != null && (this == voronoiEdge || (this.Source == voronoiEdge.Source && this.Target == voronoiEdge.Target) || (this.Source == voronoiEdge.Target && this.Target == voronoiEdge.Source));
		}

		public override int GetHashCode()
		{
			return (23 * 31 + this.Source.GetHashCode()) * 31 + this.Target.GetHashCode();
		}
	}
}
