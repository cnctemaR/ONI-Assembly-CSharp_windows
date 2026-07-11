using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;
using Delaunay.Geo;
using MIConvexHull;
using ProcGen.Map;
using UnityEngine;

namespace VoronoiTree
{
	public class PowerDiagram
	{
		public PowerDiagram(Polygon polyBounds, IEnumerable<Site> newSites)
		{
			this.bounds = polyBounds;
			this.bounds.ForceWinding(Winding.COUNTERCLOCKWISE);
			this.weightSum = 0f;
			this.sites.Clear();
			IEnumerator<Site> enumerator = newSites.GetEnumerator();
			int num = 0;
			while (enumerator.MoveNext())
			{
				if (!this.bounds.Contains(enumerator.Current.position))
				{
					global::Debug.LogErrorFormat("Cant feed points [{0}] to powerdiagram that are outside its area [{1}] ", new object[]
					{
						enumerator.Current.id,
						enumerator.Current.position
					});
				}
				if (this.bounds.Contains(enumerator.Current.position))
				{
					this.AddSite(enumerator.Current);
				}
				num++;
			}
			Vector2 vector = this.bounds.Centroid();
			for (int i = 0; i < this.bounds.Vertices.Count; i++)
			{
				Vector2 vector2 = this.bounds.Vertices[i];
				Vector2 vector3 = this.bounds.Vertices[(i >= this.bounds.Vertices.Count - 1) ? 0 : (i + 1)];
				Vector2 vector4 = (vector2 - vector).normalized * 1000f;
				Vector2 vector5 = vector2 + vector4;
				Site site = new Site(vector5);
				site.dummy = true;
				this.externalEdgePoints.Add(site);
				site.weight = Mathf.Epsilon;
				site.currentWeight = Mathf.Epsilon;
				this.dualSites.Add(new PowerDiagram.DualSite2d(site));
				Vector2 vector6 = (vector3 - vector2) * 0.5f + vector3;
				Vector2 vector7 = (vector6 - vector).normalized * 1000f;
				Vector2 vector8 = vector3 + vector7;
				Site site2 = new Site(vector8);
				site2.dummy = true;
				site2.weight = Mathf.Epsilon;
				site2.currentWeight = Mathf.Epsilon;
				this.externalEdgePoints.Add(site2);
				this.dualSites.Add(new PowerDiagram.DualSite2d(site2));
			}
		}

		public VoronoiMesh<PowerDiagram.DualSite2d, Site, VoronoiEdge<PowerDiagram.DualSite2d, Site>> voronoiMesh { get; private set; }

		public List<Site> GetSites()
		{
			return this.sites;
		}

		public int completedIterations { get; set; }

		public void ComputePowerDiagram(int maxIterations, float threashold = 1f)
		{
			this.completedIterations = 0;
			float num = 0f;
			foreach (Site site in this.sites)
			{
				if (site.poly == null)
				{
					throw new Exception(string.Concat(new object[] { "site poly is null for [", site.id, "]", site.position }));
				}
				site.position = site.poly.Centroid();
			}
			int i = 0;
			while (i <= maxIterations)
			{
				try
				{
					this.UpdateWeights(this.sites);
					this.ComputePD();
				}
				catch (Exception ex)
				{
					global::Debug.LogError(string.Concat(new object[] { "Error [", num, "] iters ", this.completedIterations, "/", maxIterations, " Exception:", ex.Message, "\n", ex.StackTrace }), null);
					return;
				}
				num = 0f;
				foreach (Site site2 in this.sites)
				{
					float num2 = ((site2.poly != null) ? site2.poly.Area() : 0.1f);
					float num3 = site2.weight / this.weightSum * this.bounds.Area();
					num = Mathf.Max(Mathf.Abs(num2 - num3) / num3, num);
				}
				if (num < threashold)
				{
					this.completedIterations = i;
					break;
				}
				this.completedIterations++;
				i++;
				continue;
			}
			global::Debug.Log(string.Concat(new object[] { "error [", num, "] iters ", this.completedIterations, "/", maxIterations }), null);
		}

		public void ComputeVD()
		{
			this.voronoiMesh = VoronoiMesh.Create<PowerDiagram.DualSite2d, Site>(this.dualSites);
			foreach (Site site in this.voronoiMesh.Vertices)
			{
				Vector2 circumcenter = site.Circumcenter;
				Cell cell = this.mg.GetCell(circumcenter);
				foreach (PowerDiagram.DualSite2d dualSite2d in site.Vertices)
				{
					if (!dualSite2d.visited)
					{
						dualSite2d.visited = true;
						if (!dualSite2d.site.dummy)
						{
							List<Vector2> list = new List<Vector2>();
							dualSite2d.site.neighbours = this.TouchingFaces(dualSite2d, site);
							foreach (Site site2 in dualSite2d.site.neighbours)
							{
								Vector2 circumcenter2 = site2.Circumcenter;
								Color.red.a = 0.3f;
								list.Add(circumcenter2);
								Vector2 vector = circumcenter2;
								Cell cell2 = this.mg.GetCell(vector);
								if (cell != null && cell2 != null)
								{
									cell.Add(cell2);
									cell2.Add(cell);
									Corner corner = this.mg.GetCorner(circumcenter, true);
									Corner corner2 = this.mg.GetCorner(vector, true);
									cell.Add(corner);
									cell.Add(corner2);
									cell2.Add(corner);
									cell2.Add(corner2);
								}
							}
							if (list.Count > 0)
							{
								Polygon polygon = PowerDiagram.PolyForRandomPoints(list);
								dualSite2d.site.poly = polygon.Clip(this.bounds, ClipType.ctIntersection);
							}
						}
					}
				}
			}
			this.ClipNeighbors();
		}

		public void ComputeVD3d()
		{
			List<PowerDiagram.DualSite3d> list = new List<PowerDiagram.DualSite3d>();
			foreach (Site site in this.sites)
			{
				list.Add(site.ToDualSite());
			}
			for (int i = 0; i < this.externalEdgePoints.Count; i++)
			{
				list.Add(this.externalEdgePoints[i].ToDualSite());
			}
			VoronoiMesh<PowerDiagram.DualSite3d, PowerDiagram.TriangulationCellExt<PowerDiagram.DualSite3d>, VoronoiEdge<PowerDiagram.DualSite3d, PowerDiagram.TriangulationCellExt<PowerDiagram.DualSite3d>>> voronoiMesh = VoronoiMesh.Create<PowerDiagram.DualSite3d, PowerDiagram.TriangulationCellExt<PowerDiagram.DualSite3d>>(list);
			foreach (PowerDiagram.TriangulationCellExt<PowerDiagram.DualSite3d> triangulationCellExt in voronoiMesh.Vertices)
			{
				Vector3 vector = Vector3.zero;
				foreach (PowerDiagram.DualSite3d dualSite3d in triangulationCellExt.Vertices)
				{
					vector += dualSite3d.coord;
				}
				vector *= 0.33333334f;
				DebugExtension.DebugPoint(vector, Color.red, 1f, 0f, true);
			}
		}

		private bool ContainsVert(Site face, PowerDiagram.DualSite2d target)
		{
			if (face == null || face.Vertices == null)
			{
				return false;
			}
			for (int i = 0; i < face.Vertices.Length; i++)
			{
				if (face.Vertices[i] == target)
				{
					return true;
				}
			}
			return false;
		}

		private void AddSite(Site site)
		{
			this.weightSum += site.weight;
			site.currentWeight = site.weight;
			this.sites.Add(site);
			this.dualSites.Add(new PowerDiagram.DualSite2d(site));
		}

		private List<Site> TouchingFaces(PowerDiagram.DualSite2d site, Site startingFace)
		{
			List<Site> list = new List<Site>();
			Stack<Site> stack = new Stack<Site>();
			stack.Push(startingFace);
			while (stack.Count > 0)
			{
				Site site2 = stack.Pop();
				if (this.ContainsVert(site2, site) && !list.Contains(site2))
				{
					list.Add(site2);
					for (int i = 0; i < site2.Adjacency.Length; i++)
					{
						if (this.ContainsVert(site2.Adjacency[i], site))
						{
							stack.Push(site2.Adjacency[i]);
						}
					}
				}
			}
			return list;
		}

		private void ClipNeighbors()
		{
			foreach (Site site in this.sites)
			{
				Cell cell = this.mg.GetCell(site.position);
				if (cell != null && cell.corners != null && cell.corners.Count > 2)
				{
					Cell cell2 = this.mg.GetCell(site.position);
					if (site.poly != null)
					{
						foreach (ProcGen.Map.Edge edge in cell2.edges)
						{
							LineSegment lineSegment = new LineSegment(new Vector2?(edge.corner0.position), new Vector2?(edge.corner1.position));
							LineSegment lineSegment2 = new LineSegment(null, null);
							Vector2 zero = Vector2.zero;
							Vector2 zero2 = Vector2.zero;
							bool flag = site.poly.ClipSegment(lineSegment, ref lineSegment2, ref zero, ref zero2);
							Vector2? p = lineSegment2.p0;
							if (p != null)
							{
								Vector2? p2 = lineSegment2.p1;
								if (p2 != null)
								{
									if (flag)
									{
										edge.corner0.SetPosition(lineSegment2.p0.Value);
										edge.corner1.SetPosition(lineSegment2.p1.Value);
									}
								}
							}
						}
					}
				}
			}
		}

		private PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d> GetNeigborFaceForEdge(PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d> currentFace, PowerDiagram.DualSite3d sharedVert0, PowerDiagram.DualSite3d sharedVert1)
		{
			for (int i = 0; i < currentFace.Adjacency.Length; i++)
			{
				PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d> convexFaceExt = currentFace.Adjacency[i];
				if (convexFaceExt != null)
				{
					int num = 0;
					for (int j = 0; j < convexFaceExt.Vertices.Length; j++)
					{
						if (sharedVert0 == convexFaceExt.Vertices[j])
						{
							num++;
						}
						else if (sharedVert1 == convexFaceExt.Vertices[j])
						{
							num++;
						}
						if (num == 2)
						{
							return convexFaceExt;
						}
					}
				}
			}
			return null;
		}

		private PowerDiagram.Edge GetEdge(PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d> face0, PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d> face1)
		{
			PowerDiagram.Edge edge = null;
			for (int i = 0; i < face0.Vertices.Length; i++)
			{
				for (int j = 0; j < face1.Vertices.Length; j++)
				{
					if (face0.Vertices[i] == face1.Vertices[j])
					{
						if (edge == null)
						{
							edge = new PowerDiagram.Edge(face0.Vertices[i], null);
						}
						else
						{
							edge.Second = face0.Vertices[i];
						}
					}
				}
			}
			return edge;
		}

		private bool ContainsVert(PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d> face, PowerDiagram.DualSite3d target)
		{
			for (int i = 0; i < face.Vertices.Length; i++)
			{
				if (face.Vertices[i] == target)
				{
					return true;
				}
			}
			return false;
		}

		private List<PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>> TouchingFaces(PowerDiagram.DualSite3d site, PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d> startingFace)
		{
			List<PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>> list = new List<PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>>();
			Stack<PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>> stack = new Stack<PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>>();
			stack.Push(startingFace);
			while (stack.Count > 0)
			{
				PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d> convexFaceExt = stack.Pop();
				if (this.ContainsVert(convexFaceExt, site) && !list.Contains(convexFaceExt))
				{
					list.Add(convexFaceExt);
					for (int i = 0; i < convexFaceExt.Adjacency.Length; i++)
					{
						if (this.ContainsVert(convexFaceExt.Adjacency[i], site) && !list.Contains(convexFaceExt.Adjacency[i]))
						{
							stack.Push(convexFaceExt.Adjacency[i]);
						}
					}
				}
			}
			return list;
		}

		private List<Site> GenerateNeighbors(PowerDiagram.DualSite3d dualSite, PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d> startingFace)
		{
			List<Site> list = new List<Site>();
			List<PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>> list2 = new List<PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>>();
			Stack<PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>> stack = new Stack<PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>>();
			stack.Push(startingFace);
			while (stack.Count > 0)
			{
				PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d> convexFaceExt = stack.Pop();
				for (int i = 0; i < convexFaceExt.Adjacency.Length; i++)
				{
					if (this.ContainsVert(convexFaceExt.Adjacency[i], dualSite) && !list2.Contains(convexFaceExt.Adjacency[i]))
					{
						PowerDiagram.Edge edge = this.GetEdge(convexFaceExt, convexFaceExt.Adjacency[i]);
						PowerDiagram.DualSite3d dualSite3d = ((edge.First != dualSite) ? edge.First : edge.Second);
						list.Add(dualSite3d.site);
						list2.Add(convexFaceExt.Adjacency[i]);
						stack.Push(convexFaceExt.Adjacency[i]);
					}
				}
			}
			return list;
		}

		private void ComputePD()
		{
			List<PowerDiagram.DualSite3d> list = new List<PowerDiagram.DualSite3d>();
			foreach (Site site in this.sites)
			{
				list.Add(site.ToDualSite());
			}
			for (int i = 0; i < this.externalEdgePoints.Count; i++)
			{
				list.Add(this.externalEdgePoints[i].ToDualSite());
			}
			this.CheckPositions(list);
			ConvexHull<PowerDiagram.DualSite3d, PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>> convexHull = PowerDiagram.CreateHull(list, 1E-10);
			foreach (PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d> convexFaceExt in convexHull.Faces)
			{
				if (convexFaceExt.Normal[2] < (double)(-(double)Mathf.Epsilon))
				{
					foreach (PowerDiagram.DualSite3d dualSite3d in convexFaceExt.Vertices)
					{
						if (!dualSite3d.site.dummy)
						{
							if (!dualSite3d.visited)
							{
								dualSite3d.visited = true;
								List<Vector2> list2 = new List<Vector2>();
								List<PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>> list3 = this.TouchingFaces(dualSite3d, convexFaceExt);
								dualSite3d.site.neighbours = this.GenerateNeighbors(dualSite3d, convexFaceExt);
								foreach (PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d> convexFaceExt2 in list3)
								{
									Vector2 dualPoint = convexFaceExt2.GetDualPoint();
									list2.Add(dualPoint);
								}
								Polygon polygon = PowerDiagram.PolyForRandomPoints(list2);
								Polygon polygon2 = polygon.Clip(this.bounds, ClipType.ctIntersection);
								if (polygon2 == null)
								{
									polygon.DebugDraw(Color.yellow, false, 1f, 0f);
									dualSite3d.site.poly.DebugDraw(Color.black, false, 1f, 2f);
									DebugExtension.DebugCircle2d(dualSite3d.site.position, Color.magenta, 5f, 0f, true, 20f);
								}
								else
								{
									dualSite3d.site.poly = polygon2;
								}
							}
						}
					}
				}
			}
		}

		private void UpdateWeights(List<Site> sites)
		{
			foreach (Site site in sites)
			{
				if (site.poly == null)
				{
					throw new Exception(string.Concat(new object[] { "site poly is null for [", site.id, "]", site.position }));
				}
				site.position = site.poly.Centroid();
				site.currentWeight = Mathf.Max(site.currentWeight, 1f);
			}
			float num = 0f;
			foreach (Site site2 in sites)
			{
				float num2 = ((site2.poly != null) ? site2.poly.Area() : 0.1f);
				float num3 = site2.weight / this.weightSum * this.bounds.Area();
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
				foreach (Site site3 in sites)
				{
					site3.currentWeight += num + 1f;
				}
			}
			float num9 = 1f;
			foreach (Site site4 in sites)
			{
				foreach (Site site5 in site4.neighbours)
				{
					float num10 = (site4.position - site5.position).sqrMagnitude / (Mathf.Abs(site4.currentWeight - site5.currentWeight) + 1f);
					if (num10 < num9)
					{
						num9 = num10;
					}
				}
			}
			foreach (Site site6 in sites)
			{
				site6.currentWeight *= num9;
			}
		}

		private List<PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>> GetNeigborFaces(PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d> currentFace)
		{
			List<PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>> list = new List<PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>>();
			for (int i = 0; i < currentFace.Adjacency.Length; i++)
			{
				PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d> convexFaceExt = currentFace.Adjacency[i];
				if (convexFaceExt != null)
				{
					list.Add(convexFaceExt);
				}
			}
			return list;
		}

		private void CheckPositions(List<PowerDiagram.DualSite3d> dual3dSites)
		{
			for (int i = 0; i < dual3dSites.Count; i++)
			{
				if (!dual3dSites[i].site.dummy)
				{
					for (int j = i + 1; j < dual3dSites.Count; j++)
					{
						if (!dual3dSites[j].site.dummy)
						{
							if (dual3dSites[i].coord == dual3dSites[j].coord)
							{
								dual3dSites[j].coord += new Vector3(global::UnityEngine.Random.value, global::UnityEngine.Random.value, 0f);
							}
						}
					}
				}
			}
		}

		public static Polygon PolyForRandomPoints(List<Vector2> verts)
		{
			double[][] array = new double[verts.Count][];
			for (int i = 0; i < verts.Count; i++)
			{
				array[i] = new double[]
				{
					(double)verts[i].x,
					(double)verts[i].y
				};
			}
			ConvexHull<DefaultVertex, DefaultConvexFace<DefaultVertex>> convexHull = ConvexHull.Create(array, 1E-10);
			double[][] array2 = convexHull.Points.Select<DefaultVertex, double[]>((DefaultVertex p) => p.Position).ToArray<double[]>();
			Polygon polygon = new Polygon();
			for (int j = 0; j < array2.Length; j++)
			{
				polygon.Add(new Vector2((float)array2[j][0], (float)array2[j][1]));
			}
			polygon.Initialize();
			polygon.ForceWinding(Winding.COUNTERCLOCKWISE);
			return polygon;
		}

		public static ConvexHull<PowerDiagram.DualSite3d, PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>> CreateHull(IList<PowerDiagram.DualSite3d> data, double PlaneDistanceTolerance = 1E-10)
		{
			return ConvexHull<PowerDiagram.DualSite3d, PowerDiagram.ConvexFaceExt<PowerDiagram.DualSite3d>>.Create(data, PlaneDistanceTolerance);
		}

		public const Winding ForcedWinding = Winding.COUNTERCLOCKWISE;

		public MapGraph mg = new MapGraph(0);

		private Polygon bounds;

		private List<Site> externalEdgePoints = new List<Site>();

		private float weightSum;

		private List<Site> sites = new List<Site>();

		private List<PowerDiagram.DualSite2d> dualSites = new List<PowerDiagram.DualSite2d>();

		private class Edge : MathUtil.Pair<PowerDiagram.DualSite3d, PowerDiagram.DualSite3d>
		{
			public Edge(PowerDiagram.DualSite3d first, PowerDiagram.DualSite3d second)
			{
				base.First = first;
				base.Second = second;
			}
		}

		public class ConvexFaceExt<TVertex> : ConvexFace<TVertex, PowerDiagram.ConvexFaceExt<TVertex>> where TVertex : IVertex
		{
			public TVertex vertex0
			{
				get
				{
					return base.Vertices[0];
				}
			}

			public TVertex vertex1
			{
				get
				{
					return base.Vertices[1];
				}
			}

			public TVertex vertex2
			{
				get
				{
					return base.Vertices[2];
				}
			}

			public PowerDiagram.ConvexFaceExt<TVertex> edge0
			{
				get
				{
					return base.Adjacency[0];
				}
			}

			public PowerDiagram.ConvexFaceExt<TVertex> edge1
			{
				get
				{
					return base.Adjacency[1];
				}
			}

			public PowerDiagram.ConvexFaceExt<TVertex> edge2
			{
				get
				{
					return base.Adjacency[2];
				}
			}

			public Vector2 GetDualPoint()
			{
				if (this.dualPoint.x == 0f && this.dualPoint.y == 0f)
				{
					TVertex vertex = this.vertex0;
					float num = (float)vertex.Position[0];
					TVertex vertex2 = this.vertex0;
					float num2 = (float)vertex2.Position[1];
					TVertex vertex3 = this.vertex0;
					Vector3 vector = new Vector3(num, num2, (float)vertex3.Position[2]);
					TVertex vertex4 = this.vertex1;
					float num3 = (float)vertex4.Position[0];
					TVertex vertex5 = this.vertex1;
					float num4 = (float)vertex5.Position[1];
					TVertex vertex6 = this.vertex1;
					Vector3 vector2 = new Vector3(num3, num4, (float)vertex6.Position[2]);
					TVertex vertex7 = this.vertex2;
					float num5 = (float)vertex7.Position[0];
					TVertex vertex8 = this.vertex2;
					float num6 = (float)vertex8.Position[1];
					TVertex vertex9 = this.vertex2;
					Vector3 vector3 = new Vector3(num5, num6, (float)vertex9.Position[2]);
					double num7 = (double)(vector.y * (vector2.z - vector3.z) + vector2.y * (vector3.z - vector.z) + vector3.y * (vector.z - vector2.z));
					double num8 = (double)(vector.z * (vector2.x - vector3.x) + vector2.z * (vector3.x - vector.x) + vector3.z * (vector.x - vector2.x));
					double num9 = -0.5 / (double)(vector.x * (vector2.y - vector3.y) + vector2.x * (vector3.y - vector.y) + vector3.x * (vector.y - vector2.y));
					this.dualPoint = new Vector2((float)(num7 * num9), (float)(num8 * num9));
				}
				return this.dualPoint;
			}

			private double Det(double[,] m)
			{
				return m[0, 0] * (m[1, 1] * m[2, 2] - m[2, 1] * m[1, 2]) - m[0, 1] * (m[1, 0] * m[2, 2] - m[2, 0] * m[1, 2]) + m[0, 2] * (m[1, 0] * m[2, 1] - m[2, 0] * m[1, 1]);
			}

			private double LengthSquared(double[] v)
			{
				double num = 0.0;
				foreach (double num2 in v)
				{
					num += num2 * num2;
				}
				return num;
			}

			private Vector2 GetCircumcenter()
			{
				TVertex[] vertices = base.Vertices;
				double[,] array = new double[3, 3];
				for (int i = 0; i < 3; i++)
				{
					array[i, 0] = vertices[i].Position[0];
					array[i, 1] = vertices[i].Position[1];
					array[i, 2] = 1.0;
				}
				double num = this.Det(array);
				double num2 = -1.0 / (2.0 * num);
				for (int j = 0; j < 3; j++)
				{
					array[j, 0] = this.LengthSquared(vertices[j].Position);
				}
				double num3 = -this.Det(array);
				for (int k = 0; k < 3; k++)
				{
					array[k, 1] = vertices[k].Position[0];
				}
				double num4 = this.Det(array);
				return new Vector2((float)(num2 * num3), (float)(num2 * num4));
			}

			public Vector2 Circumcenter
			{
				get
				{
					Vector2? vector = this.circumCenter;
					this.circumCenter = new Vector2?((vector == null) ? this.GetCircumcenter() : vector.Value);
					return this.circumCenter.Value;
				}
			}

			private Site site;

			private Vector2 dualPoint;

			private Vector2? circumCenter;
		}

		public class TriangulationCellExt<TVertex> : TriangulationCell<TVertex, PowerDiagram.TriangulationCellExt<TVertex>> where TVertex : IVertex
		{
			public TVertex Vertex0
			{
				get
				{
					return base.Vertices[0];
				}
			}

			public TVertex Vertex1
			{
				get
				{
					return base.Vertices[1];
				}
			}

			public TVertex Vertex2
			{
				get
				{
					return base.Vertices[2];
				}
			}

			public PowerDiagram.TriangulationCellExt<TVertex> Edge0
			{
				get
				{
					return base.Adjacency[0];
				}
			}

			public PowerDiagram.TriangulationCellExt<TVertex> Edge1
			{
				get
				{
					return base.Adjacency[1];
				}
			}

			public PowerDiagram.TriangulationCellExt<TVertex> Edge2
			{
				get
				{
					return base.Adjacency[2];
				}
			}
		}

		public class DualSite2d : IVertex
		{
			public DualSite2d(Site site)
			{
				this.site = site;
				this.visited = false;
			}

			public double[] Position
			{
				get
				{
					return new double[]
					{
						(double)this.site.position[0],
						(double)this.site.position[1]
					};
				}
			}

			public Site site { get; set; }

			public bool visited { get; set; }
		}

		public class DualSite3d : IVertex
		{
			public DualSite3d()
				: this(0.0, 0.0, 0.0)
			{
			}

			public DualSite3d(double _x, double _y, double _z)
			{
				this.coord = new Vector3((float)_x, (float)_y, (float)_z);
				this.visited = false;
			}

			public DualSite3d(Vector3 pos)
			{
				this.coord = pos;
				this.visited = false;
			}

			public DualSite3d(double _x, double _y, double _z, Site _originalSite)
				: this(_x, _y, _z)
			{
				this.site = _originalSite;
				this.visited = false;
			}

			public double[] Position
			{
				get
				{
					return new double[]
					{
						(double)this.coord[0],
						(double)this.coord[1],
						(double)this.coord[2]
					};
				}
			}

			public Vector3 coord { get; set; }

			public Site site { get; set; }

			public bool visited { get; set; }
		}
	}
}
