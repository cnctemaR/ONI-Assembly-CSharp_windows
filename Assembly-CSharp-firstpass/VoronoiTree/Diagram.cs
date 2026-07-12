using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Delaunay;
using Delaunay.Geo;
using KSerialization;
using UnityEngine;

namespace VoronoiTree
{
	public class Diagram
	{
		public Diagram()
		{
			this.diagram = null;
		}

		public Voronoi diagram { get; private set; }

		public Diagram(Rect bounds, IEnumerable<Diagram.Site> sites)
		{
			this.bounds = bounds;
			this.ids = new List<uint>();
			this.points = new List<Vector2>();
			this.weights = new List<float>();
			this.weightSum = 0f;
			IEnumerator<Diagram.Site> enumerator = sites.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				Diagram.Site site = enumerator.Current;
				this.AddSite(site);
				num++;
			}
			this.MakeVD();
		}

		private void AddSite(Diagram.Site site)
		{
			this.ids.Add(site.id);
			this.points.Add(site.position);
			this.weights.Add(site.weight);
			this.weightSum += site.weight;
			site.currentWeight = site.weight;
		}

		private void MakeVD()
		{
			this.diagram = new Voronoi(this.points, this.ids, this.weights, this.bounds);
		}

		public void UpdateWeights(List<Diagram.Site> sites)
		{
			for (int i = 0; i < sites.Count; i++)
			{
				Diagram.Site site = sites[i];
				site.position = site.poly.Centroid();
				site.currentWeight = Mathf.Max(site.currentWeight, 1f);
			}
			float num = 0f;
			for (int j = 0; j < sites.Count; j++)
			{
				Diagram.Site site2 = sites[j];
				float num2 = site2.poly.Area();
				float num3 = site2.weight / this.weightSum * this.Area();
				float num4 = Mathf.Sqrt(num2 / 3.1415927f);
				float num5 = Mathf.Sqrt(num3 / 3.1415927f);
				float num6 = num4 - num5;
				float num7 = num3 / num2;
				if (((double)num7 > 1.1 && (double)site2.previousWeightAdaption < 0.9) || ((double)num7 < 0.9 && (double)site2.previousWeightAdaption > 1.1))
				{
					num7 = Mathf.Sqrt(num7);
				}
				if ((double)num7 < 1.1 && (double)num7 > 0.9 && site2.currentWeight != 1f)
				{
					num7 = Mathf.Sqrt(num7);
				}
				if (site2.currentWeight < 10f)
				{
					num7 *= num7;
				}
				if (site2.currentWeight > 10f)
				{
					num7 = Mathf.Sqrt(num7);
				}
				site2.previousWeightAdaption = num7;
				site2.currentWeight *= num7;
				if (site2.currentWeight < 1f)
				{
					float num8 = Mathf.Sqrt(site2.currentWeight) - num6;
					if (num8 < 0f)
					{
						site2.currentWeight = -(num8 * num8);
						if (site2.currentWeight < num)
						{
							num = site2.currentWeight;
						}
					}
				}
			}
			if (num < 0f)
			{
				num = -num;
				for (int k = 0; k < sites.Count; k++)
				{
					sites[k].currentWeight += num + 1f;
				}
			}
			float num9 = 1f;
			for (int l = 0; l < sites.Count; l++)
			{
				Diagram.Site site3 = sites[l];
				List<uint> neighbours = this.diagram.ListNeighborSitesIDsForSite(this.points[l]);
				int nIndex2;
				int nIndex;
				Predicate<Diagram.Site> <>9__0;
				for (nIndex = 0; nIndex < neighbours.Count; nIndex = nIndex2 + 1)
				{
					Predicate<Diagram.Site> predicate;
					if ((predicate = <>9__0) == null)
					{
						predicate = (<>9__0 = (Diagram.Site s) => s.id == neighbours[nIndex]);
					}
					Diagram.Site site4 = sites.Find(predicate);
					float num10 = (site3.position - site4.position).sqrMagnitude / (Mathf.Abs(site3.currentWeight - site4.currentWeight) + 1f);
					if (num10 < num9)
					{
						num9 = num10;
					}
					nIndex2 = nIndex;
				}
			}
			for (int m = 0; m < sites.Count; m++)
			{
				sites[m].currentWeight *= num9;
				this.weights[m] = sites[m].currentWeight;
				this.points[m] = sites[m].position;
			}
		}

		private float Area()
		{
			return this.bounds.width * this.bounds.height;
		}

		public int completeIterations { get; set; }

		public List<Diagram.Site> ComputePowerDiagram(List<Diagram.Site> sites, int maxIterations)
		{
			this.completeIterations = 0;
			for (int i = 1; i <= maxIterations; i++)
			{
				this.UpdateWeights(sites);
				this.MakeVD();
				float num = 0f;
				foreach (Diagram.Site site in sites)
				{
					float num2 = site.poly.Area();
					float num3 = site.weight / this.weightSum * this.Area();
					num = Mathf.Max(Mathf.Abs(num2 - num3) / num3, num);
				}
				if (num < 0.001f)
				{
					this.completeIterations = i;
					break;
				}
			}
			return sites;
		}

		public int GetIdxForNode(uint nodeID)
		{
			for (int i = 0; i < this.points.Count; i++)
			{
				if (this.ids[i] == nodeID)
				{
					return i;
				}
			}
			return -1;
		}

		public List<uint> GetNodeIdsForTopEdgeCells()
		{
			List<uint> list = new List<uint>();
			for (int i = 0; i < this.points.Count; i++)
			{
				if (this.IsTopEdgeCell(i))
				{
					list.Add(this.ids[i]);
				}
			}
			return list;
		}

		public bool IsTopEdgeCell(int cell)
		{
			if (cell < 0 || cell >= this.points.Count)
			{
				return false;
			}
			List<Vector2> list = this.diagram.Region(this.points[cell]);
			if (list.Count == 0)
			{
				return false;
			}
			Vector2 vector = list[0];
			for (int i = 1; i < list.Count; i++)
			{
				Vector2 vector2 = list[i];
				if (vector.y == vector2.y && vector2.y == this.bounds.height)
				{
					return true;
				}
				vector = vector2;
			}
			return vector.y == list[0].y && list[0].y == this.bounds.height;
		}

		public static int maxPowerIterations;

		public static float maxPowerError;

		private List<Vector2> points;

		private List<float> weights;

		private Rect bounds;

		private float weightSum;

		private List<uint> ids = new List<uint>();

		public int siteIndex;

		[SerializationConfig(MemberSerialization.OptIn)]
		public class Site
		{
			public Site()
			{
				this.neighbours = new HashSet<KeyValuePair<uint, int>>();
			}

			public Site(uint id, Vector2 pos, float weight = 1f)
			{
				this.id = id;
				this.position = pos;
				this.weight = weight;
				this.currentWeight = weight;
				this.neighbours = new HashSet<KeyValuePair<uint, int>>();
			}

			[OnDeserializing]
			internal void OnDeserializingMethod()
			{
				this.neighbours = new HashSet<KeyValuePair<uint, int>>();
			}

			[Serialize]
			public uint id;

			[Serialize]
			public float weight;

			public float currentWeight;

			public float previousWeightAdaption;

			[Serialize]
			public Vector2 position;

			[Serialize]
			public Polygon poly;

			[Serialize]
			public HashSet<KeyValuePair<uint, int>> neighbours;
		}
	}
}
