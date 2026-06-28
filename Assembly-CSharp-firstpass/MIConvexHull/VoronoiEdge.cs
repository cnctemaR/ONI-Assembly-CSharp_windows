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
			return voronoiEdge != null && (object.ReferenceEquals(this, voronoiEdge) || (this.Source == voronoiEdge.Source && this.Target == voronoiEdge.Target) || (this.Source == voronoiEdge.Target && this.Target == voronoiEdge.Source));
		}

		public override int GetHashCode()
		{
			int num = 23;
			int num2 = num * 31;
			TCell source = this.Source;
			num = num2 + source.GetHashCode();
			int num3 = num * 31;
			TCell target = this.Target;
			return num3 + target.GetHashCode();
		}
	}
}
