using System;
using System.Collections.Generic;
using Delaunay.LR;
using Delaunay.Utils;

namespace Delaunay
{
	internal sealed class EdgeReorderer : Delaunay.Utils.IDisposable
	{
		public EdgeReorderer(List<Edge> origEdges, VertexOrSite criterion)
		{
			this._edges = new List<Edge>();
			this._edgeOrientations = new List<Side>();
			if (origEdges.Count > 0)
			{
				this._edges = this.ReorderEdges(origEdges, criterion);
			}
		}

		public List<Edge> edges
		{
			get
			{
				return this._edges;
			}
		}

		public List<Side> edgeOrientations
		{
			get
			{
				return this._edgeOrientations;
			}
		}

		public void Dispose()
		{
			this._edges = null;
			this._edgeOrientations = null;
		}

		private List<Edge> ReorderEdges(List<Edge> origEdges, VertexOrSite criterion)
		{
			int count = origEdges.Count;
			bool[] array = new bool[count];
			int i = 0;
			for (int j = 0; j < count; j++)
			{
				array[j] = false;
			}
			List<Edge> list = new List<Edge>();
			int k = 0;
			Edge edge = origEdges[k];
			list.Add(edge);
			this._edgeOrientations.Add(Side.LEFT);
			ICoord coord2;
			if (criterion == VertexOrSite.VERTEX)
			{
				ICoord coord = edge.leftVertex;
				coord2 = coord;
			}
			else
			{
				coord2 = edge.leftSite;
			}
			ICoord coord3 = coord2;
			ICoord coord4;
			if (criterion == VertexOrSite.VERTEX)
			{
				ICoord coord = edge.rightVertex;
				coord4 = coord;
			}
			else
			{
				coord4 = edge.rightSite;
			}
			ICoord coord5 = coord4;
			if (coord3 == Vertex.VERTEX_AT_INFINITY || coord5 == Vertex.VERTEX_AT_INFINITY)
			{
				return new List<Edge>();
			}
			array[k] = true;
			i++;
			while (i < count)
			{
				for (k = 1; k < count; k++)
				{
					if (!array[k])
					{
						edge = origEdges[k];
						ICoord coord6;
						if (criterion == VertexOrSite.VERTEX)
						{
							ICoord coord = edge.leftVertex;
							coord6 = coord;
						}
						else
						{
							coord6 = edge.leftSite;
						}
						ICoord coord7 = coord6;
						ICoord coord8;
						if (criterion == VertexOrSite.VERTEX)
						{
							ICoord coord = edge.rightVertex;
							coord8 = coord;
						}
						else
						{
							coord8 = edge.rightSite;
						}
						ICoord coord9 = coord8;
						if (coord7 == Vertex.VERTEX_AT_INFINITY || coord9 == Vertex.VERTEX_AT_INFINITY)
						{
							return new List<Edge>();
						}
						if (coord7 == coord5)
						{
							coord5 = coord9;
							this._edgeOrientations.Add(Side.LEFT);
							list.Add(edge);
							array[k] = true;
						}
						else if (coord9 == coord3)
						{
							coord3 = coord7;
							this._edgeOrientations.Insert(0, Side.LEFT);
							list.Insert(0, edge);
							array[k] = true;
						}
						else if (coord7 == coord3)
						{
							coord3 = coord9;
							this._edgeOrientations.Insert(0, Side.RIGHT);
							list.Insert(0, edge);
							array[k] = true;
						}
						else if (coord9 == coord5)
						{
							coord5 = coord7;
							this._edgeOrientations.Add(Side.RIGHT);
							list.Add(edge);
							array[k] = true;
						}
						if (array[k])
						{
							i++;
						}
					}
				}
			}
			return list;
		}

		private List<Edge> _edges;

		private List<Side> _edgeOrientations;
	}
}
