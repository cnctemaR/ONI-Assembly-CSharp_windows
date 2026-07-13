using System;
using System.Collections.Generic;
using ClipperLib;
using LibTessDotNet;
using UnityEngine;

namespace Unity.VectorGraphics
{
	internal static class VectorClip
	{
		internal static void ResetClip()
		{
			VectorClip.m_ClipStack.Clear();
		}

		internal static void PushClip(List<Vector2[]> clipper, Matrix2D transform)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>(10);
			foreach (Vector2[] array in clipper)
			{
				List<IntPoint> list2 = new List<IntPoint>(array.Length);
				foreach (Vector2 vector in array)
				{
					Vector2 vector2 = transform * vector;
					list2.Add(new IntPoint((double)(vector2.x * 100000f), (double)(vector2.y * 100000f)));
				}
				list.Add(list2);
			}
			VectorClip.m_ClipStack.Push(list);
		}

		internal static void PopClip()
		{
			VectorClip.m_ClipStack.Pop();
		}

		internal static void ClipGeometry(VectorUtils.Geometry geom)
		{
			Clipper clipper = new Clipper(0);
			foreach (List<List<IntPoint>> list in VectorClip.m_ClipStack)
			{
				List<Vector2> list2 = new List<Vector2>(geom.Vertices.Length);
				List<ushort> list3 = new List<ushort>(geom.Indices.Length);
				List<List<IntPoint>> list4 = VectorClip.BuildTriangleClipPaths(geom);
				List<List<IntPoint>> list5 = new List<List<IntPoint>>();
				ushort num = 0;
				foreach (List<IntPoint> list6 in list4)
				{
					clipper.AddPaths(list, PolyType.ptClip, true);
					clipper.AddPath(list6, PolyType.ptSubject, true);
					clipper.Execute(ClipType.ctIntersection, list5, PolyFillType.pftNonZero, PolyFillType.pftNonZero);
					bool flag = list5.Count > 0;
					if (flag)
					{
						VectorClip.BuildGeometryFromClipPaths(geom, list5, list2, list3, ref num);
					}
					clipper.Clear();
					list5.Clear();
				}
				geom.Vertices = list2.ToArray();
				geom.Indices = list3.ToArray();
			}
		}

		private static List<List<IntPoint>> BuildTriangleClipPaths(VectorUtils.Geometry geom)
		{
			List<List<IntPoint>> list = new List<List<IntPoint>>(geom.Indices.Length / 3);
			Vector2[] vertices = geom.Vertices;
			ushort[] indices = geom.Indices;
			int num = geom.Indices.Length;
			Matrix2D worldTransform = geom.WorldTransform;
			for (int i = 0; i < num; i += 3)
			{
				Vector2 vector = worldTransform * vertices[(int)indices[i]];
				Vector2 vector2 = worldTransform * vertices[(int)indices[i + 1]];
				Vector2 vector3 = worldTransform * vertices[(int)indices[i + 2]];
				list.Add(new List<IntPoint>(3)
				{
					new IntPoint((double)(vector.x * 100000f), (double)(vector.y * 100000f)),
					new IntPoint((double)(vector2.x * 100000f), (double)(vector2.y * 100000f)),
					new IntPoint((double)(vector3.x * 100000f), (double)(vector3.y * 100000f))
				});
			}
			return list;
		}

		private static void BuildGeometryFromClipPaths(VectorUtils.Geometry geom, List<List<IntPoint>> paths, List<Vector2> outVerts, List<ushort> outInds, ref ushort maxIndex)
		{
			List<Vector2> list = new List<Vector2>(100);
			List<ushort> list2 = new List<ushort>(list.Capacity * 3);
			Dictionary<IntPoint, ushort> dictionary = new Dictionary<IntPoint, ushort>();
			foreach (List<IntPoint> list3 in paths)
			{
				bool flag = list3.Count == 3;
				if (flag)
				{
					foreach (IntPoint intPoint in list3)
					{
						VectorClip.StoreClipVertex(dictionary, list, list2, intPoint, ref maxIndex);
					}
				}
				else
				{
					bool flag2 = list3.Count > 3;
					if (flag2)
					{
						Tess tess = new Tess();
						ContourVertex[] array = new ContourVertex[list3.Count];
						for (int i = 0; i < list3.Count; i++)
						{
							array[i] = new ContourVertex
							{
								Position = new Vec3
								{
									X = (float)list3[i].X,
									Y = (float)list3[i].Y,
									Z = 0f
								}
							};
						}
						tess.AddContour(array, ContourOrientation.Original);
						WindingRule windingRule = WindingRule.NonZero;
						tess.Tessellate(windingRule, ElementType.Polygons, 3);
						foreach (int num in tess.Elements)
						{
							ContourVertex contourVertex = tess.Vertices[num];
							IntPoint intPoint2 = new IntPoint((double)contourVertex.Position.X, (double)contourVertex.Position.Y);
							VectorClip.StoreClipVertex(dictionary, list, list2, intPoint2, ref maxIndex);
						}
					}
				}
			}
			Matrix2D matrix2D = geom.WorldTransform.Inverse();
			for (int k = 0; k < list.Count; k++)
			{
				outVerts.Add(matrix2D * list[k]);
			}
			outInds.AddRange(list2);
		}

		private static void StoreClipVertex(Dictionary<IntPoint, ushort> vertexIndex, List<Vector2> vertices, List<ushort> indices, IntPoint pt, ref ushort index)
		{
			ushort num;
			bool flag = vertexIndex.TryGetValue(pt, out num);
			if (flag)
			{
				indices.Add(num);
			}
			else
			{
				vertices.Add(new Vector2((float)pt.X / 100000f, (float)pt.Y / 100000f));
				indices.Add(index);
				vertexIndex[pt] = index;
				index += 1;
			}
		}

		private const int k_ClipperScale = 100000;

		private static Stack<List<List<IntPoint>>> m_ClipStack = new Stack<List<List<IntPoint>>>();
	}
}
