using System;
using Unity.Collections;
using Unity.Profiling;

namespace UnityEngine.UIElements.UIR
{
	internal static class Tessellation
	{
		public static void TessellateRect(MeshGenerationContextUtils.RectangleParams rectParams, float posZ, MeshBuilder.AllocMeshData meshAlloc, bool computeUVs)
		{
			bool flag = rectParams.rect.width < Tessellation.kEpsilon || rectParams.rect.height < Tessellation.kEpsilon;
			if (!flag)
			{
				Vector2 vector = new Vector2(rectParams.rect.width * 0.5f, rectParams.rect.height * 0.5f);
				rectParams.topLeftRadius = Vector2.Min(rectParams.topLeftRadius, vector);
				rectParams.topRightRadius = Vector2.Min(rectParams.topRightRadius, vector);
				rectParams.bottomRightRadius = Vector2.Min(rectParams.bottomRightRadius, vector);
				rectParams.bottomLeftRadius = Vector2.Min(rectParams.bottomLeftRadius, vector);
				ushort num = 0;
				ushort num2 = 0;
				Tessellation.CountRectTriangles(ref rectParams, ref num, ref num2);
				MeshWriteData meshWriteData = meshAlloc.Allocate((uint)num, (uint)num2);
				num = 0;
				num2 = 0;
				Tessellation.TessellateRectInternal(ref rectParams, posZ, meshWriteData, ref num, ref num2, false);
				if (computeUVs)
				{
					Tessellation.ComputeUVs(rectParams.rect, rectParams.uv, meshWriteData.uvRegion, meshWriteData.m_Vertices);
				}
				Debug.Assert((int)num == meshWriteData.vertexCount);
				Debug.Assert((int)num2 == meshWriteData.indexCount);
			}
		}

		public static void TessellateBorder(MeshGenerationContextUtils.BorderParams borderParams, float posZ, MeshBuilder.AllocMeshData meshAlloc)
		{
			bool flag = borderParams.rect.width < Tessellation.kEpsilon || borderParams.rect.height < Tessellation.kEpsilon;
			if (!flag)
			{
				Vector2 vector = new Vector2(borderParams.rect.width * 0.5f, borderParams.rect.height * 0.5f);
				borderParams.topLeftRadius = Vector2.Min(borderParams.topLeftRadius, vector);
				borderParams.topRightRadius = Vector2.Min(borderParams.topRightRadius, vector);
				borderParams.bottomRightRadius = Vector2.Min(borderParams.bottomRightRadius, vector);
				borderParams.bottomLeftRadius = Vector2.Min(borderParams.bottomLeftRadius, vector);
				borderParams.leftWidth = Mathf.Min(borderParams.leftWidth, vector.x);
				borderParams.topWidth = Mathf.Min(borderParams.topWidth, vector.y);
				borderParams.rightWidth = Mathf.Min(borderParams.rightWidth, vector.x);
				borderParams.bottomWidth = Mathf.Min(borderParams.bottomWidth, vector.y);
				ushort num = 0;
				ushort num2 = 0;
				Tessellation.CountBorderTriangles(ref borderParams, ref num, ref num2);
				MeshWriteData meshWriteData = meshAlloc.Allocate((uint)num, (uint)num2);
				num = 0;
				num2 = 0;
				Tessellation.TessellateBorderInternal(ref borderParams, posZ, meshWriteData, ref num, ref num2, false);
				Debug.Assert((int)num == meshWriteData.vertexCount);
				Debug.Assert((int)num2 == meshWriteData.indexCount);
			}
		}

		private static void CountRectTriangles(ref MeshGenerationContextUtils.RectangleParams rectParams, ref ushort vertexCount, ref ushort indexCount)
		{
			Tessellation.TessellateRectInternal(ref rectParams, 0f, null, ref vertexCount, ref indexCount, true);
		}

		private static void CountBorderTriangles(ref MeshGenerationContextUtils.BorderParams border, ref ushort vertexCount, ref ushort indexCount)
		{
			Tessellation.TessellateBorderInternal(ref border, 0f, null, ref vertexCount, ref indexCount, true);
		}

		private static void TessellateRectInternal(ref MeshGenerationContextUtils.RectangleParams rectParams, float posZ, MeshWriteData mesh, ref ushort vertexCount, ref ushort indexCount, bool countOnly = false)
		{
			bool flag = !rectParams.HasRadius(Tessellation.kEpsilon);
			if (flag)
			{
				Tessellation.TessellateQuad(rectParams.rect, 0f, 0f, 0f, Tessellation.TessellationType.Content, rectParams.color, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
			}
			else
			{
				Tessellation.TessellateRoundedCorners(ref rectParams, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
			}
		}

		private static void TessellateBorderInternal(ref MeshGenerationContextUtils.BorderParams border, float posZ, MeshWriteData mesh, ref ushort vertexCount, ref ushort indexCount, bool countOnly = false)
		{
			Tessellation.TessellateRoundedBorders(ref border, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
		}

		private static void TessellateRoundedCorners(ref MeshGenerationContextUtils.RectangleParams rectParams, float posZ, MeshWriteData mesh, ref ushort vertexCount, ref ushort indexCount, bool countOnly)
		{
			Vector2 vector = new Vector2(rectParams.rect.width * 0.5f, rectParams.rect.height * 0.5f);
			Rect rect = new Rect(rectParams.rect.x, rectParams.rect.y, vector.x, vector.y);
			Tessellation.TessellateRoundedCorner(rect, rectParams.color, posZ, rectParams.topLeftRadius, mesh, ref vertexCount, ref indexCount, countOnly);
			ushort num = vertexCount;
			ushort num2 = indexCount;
			Tessellation.TessellateRoundedCorner(rect, rectParams.color, posZ, rectParams.topRightRadius, mesh, ref vertexCount, ref indexCount, countOnly);
			bool flag = !countOnly;
			if (flag)
			{
				Tessellation.MirrorVertices(rect, mesh.m_Vertices, (int)num, (int)(vertexCount - num), true);
				Tessellation.FlipWinding(mesh.m_Indices, (int)num2, (int)(indexCount - num2));
			}
			num = vertexCount;
			num2 = indexCount;
			Tessellation.TessellateRoundedCorner(rect, rectParams.color, posZ, rectParams.bottomRightRadius, mesh, ref vertexCount, ref indexCount, countOnly);
			bool flag2 = !countOnly;
			if (flag2)
			{
				Tessellation.MirrorVertices(rect, mesh.m_Vertices, (int)num, (int)(vertexCount - num), true);
				Tessellation.MirrorVertices(rect, mesh.m_Vertices, (int)num, (int)(vertexCount - num), false);
			}
			num = vertexCount;
			num2 = indexCount;
			Tessellation.TessellateRoundedCorner(rect, rectParams.color, posZ, rectParams.bottomLeftRadius, mesh, ref vertexCount, ref indexCount, countOnly);
			bool flag3 = !countOnly;
			if (flag3)
			{
				Tessellation.MirrorVertices(rect, mesh.m_Vertices, (int)num, (int)(vertexCount - num), false);
				Tessellation.FlipWinding(mesh.m_Indices, (int)num2, (int)(indexCount - num2));
			}
		}

		private static void TessellateRoundedBorders(ref MeshGenerationContextUtils.BorderParams border, float posZ, MeshWriteData mesh, ref ushort vertexCount, ref ushort indexCount, bool countOnly)
		{
			Vector2 vector = new Vector2(border.rect.width * 0.5f, border.rect.height * 0.5f);
			Rect rect = new Rect(border.rect.x, border.rect.y, vector.x, vector.y);
			Color32 color = border.leftColor;
			Color32 color2 = border.topColor;
			Color32 color3 = border.bottomColor;
			Color32 color4 = border.rightColor;
			Tessellation.TessellateRoundedBorder(rect, color, color2, posZ, border.topLeftRadius, border.leftWidth, border.topWidth, mesh, ref vertexCount, ref indexCount, countOnly);
			ushort num = vertexCount;
			ushort num2 = indexCount;
			Tessellation.TessellateRoundedBorder(rect, color4, color2, posZ, border.topRightRadius, border.rightWidth, border.topWidth, mesh, ref vertexCount, ref indexCount, countOnly);
			bool flag = !countOnly;
			if (flag)
			{
				Tessellation.MirrorVertices(rect, mesh.m_Vertices, (int)num, (int)(vertexCount - num), true);
				Tessellation.FlipWinding(mesh.m_Indices, (int)num2, (int)(indexCount - num2));
			}
			num = vertexCount;
			num2 = indexCount;
			Tessellation.TessellateRoundedBorder(rect, color4, color3, posZ, border.bottomRightRadius, border.rightWidth, border.bottomWidth, mesh, ref vertexCount, ref indexCount, countOnly);
			bool flag2 = !countOnly;
			if (flag2)
			{
				Tessellation.MirrorVertices(rect, mesh.m_Vertices, (int)num, (int)(vertexCount - num), true);
				Tessellation.MirrorVertices(rect, mesh.m_Vertices, (int)num, (int)(vertexCount - num), false);
			}
			num = vertexCount;
			num2 = indexCount;
			Tessellation.TessellateRoundedBorder(rect, color, color3, posZ, border.bottomLeftRadius, border.leftWidth, border.bottomWidth, mesh, ref vertexCount, ref indexCount, countOnly);
			bool flag3 = !countOnly;
			if (flag3)
			{
				Tessellation.MirrorVertices(rect, mesh.m_Vertices, (int)num, (int)(vertexCount - num), false);
				Tessellation.FlipWinding(mesh.m_Indices, (int)num2, (int)(indexCount - num2));
			}
		}

		private static void TessellateRoundedCorner(Rect rect, Color32 color, float posZ, Vector2 radius, MeshWriteData mesh, ref ushort vertexCount, ref ushort indexCount, bool countOnly)
		{
			Vector2 vector = rect.position + radius;
			Rect zero = Rect.zero;
			bool flag = radius == Vector2.zero;
			if (flag)
			{
				Tessellation.TessellateQuad(rect, 0f, 0f, 0f, Tessellation.TessellationType.Content, color, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
			}
			else
			{
				Tessellation.TessellateFilledFan(Tessellation.TessellationType.Content, vector, radius, 0f, 0f, color, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
				bool flag2 = radius.x < rect.width;
				if (flag2)
				{
					zero = new Rect(rect.x + radius.x, rect.y, rect.width - radius.x, rect.height);
					Tessellation.TessellateQuad(zero, 0f, 0f, 0f, Tessellation.TessellationType.Content, color, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
				}
				bool flag3 = radius.y < rect.height;
				if (flag3)
				{
					zero = new Rect(rect.x, rect.y + radius.y, (radius.x < rect.width) ? radius.x : rect.width, rect.height - radius.y);
					Tessellation.TessellateQuad(zero, 0f, 0f, 0f, Tessellation.TessellationType.Content, color, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
				}
			}
		}

		private static void TessellateRoundedBorder(Rect rect, Color32 leftColor, Color32 topColor, float posZ, Vector2 radius, float leftWidth, float topWidth, MeshWriteData mesh, ref ushort vertexCount, ref ushort indexCount, bool countOnly)
		{
			bool flag = leftWidth < Tessellation.kEpsilon && topWidth < Tessellation.kEpsilon;
			if (!flag)
			{
				leftWidth = Mathf.Max(0f, leftWidth);
				topWidth = Mathf.Max(0f, topWidth);
				radius.x = Mathf.Clamp(radius.x, 0f, rect.width);
				radius.y = Mathf.Clamp(radius.y, 0f, rect.height);
				Vector2 vector = rect.position + radius;
				Rect zero = Rect.zero;
				bool flag2 = radius.x < Tessellation.kEpsilon || radius.y < Tessellation.kEpsilon;
				if (flag2)
				{
					bool flag3 = leftWidth > Tessellation.kEpsilon;
					if (flag3)
					{
						zero = new Rect(rect.x, rect.y, leftWidth, rect.height);
						Tessellation.TessellateQuad(zero, topWidth, leftWidth, topWidth, Tessellation.TessellationType.EdgeVertical, leftColor, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
					}
					bool flag4 = topWidth > Tessellation.kEpsilon;
					if (flag4)
					{
						zero = new Rect(rect.x, rect.y, rect.width, topWidth);
						Tessellation.TessellateQuad(zero, leftWidth, leftWidth, topWidth, Tessellation.TessellationType.EdgeHorizontal, topColor, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
					}
				}
				else
				{
					bool flag5 = Tessellation.LooseCompare(radius.x, leftWidth) == 0 && Tessellation.LooseCompare(radius.y, topWidth) == 0;
					if (flag5)
					{
						bool flag6 = leftColor.InternalEquals(topColor);
						if (flag6)
						{
							Tessellation.TessellateFilledFan(Tessellation.TessellationType.EdgeCorner, vector, radius, leftWidth, topWidth, leftColor, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
						}
						else
						{
							Tessellation.TessellateFilledFan(vector, radius, leftWidth, topWidth, leftColor, topColor, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
						}
					}
					else
					{
						bool flag7 = Tessellation.LooseCompare(radius.x, leftWidth) > 0 && Tessellation.LooseCompare(radius.y, topWidth) > 0;
						if (flag7)
						{
							bool flag8 = leftColor.InternalEquals(topColor);
							if (flag8)
							{
								Tessellation.TessellateBorderedFan(vector, radius, leftWidth, topWidth, leftColor, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
							}
							else
							{
								Tessellation.TessellateBorderedFan(vector, radius, leftWidth, topWidth, leftColor, topColor, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
							}
						}
						else
						{
							zero = new Rect(rect.x, rect.y, Mathf.Max(radius.x, leftWidth), Mathf.Max(radius.y, topWidth));
							bool flag9 = leftColor.InternalEquals(topColor);
							if (flag9)
							{
								Tessellation.TessellateComplexBorderCorner(zero, radius, leftWidth, topWidth, leftColor, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
							}
							else
							{
								Tessellation.TessellateComplexBorderCorner(zero, radius, leftWidth, topWidth, leftColor, topColor, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
							}
						}
					}
					float num = Mathf.Max(radius.y, topWidth);
					zero = new Rect(rect.x, rect.y + num, leftWidth, rect.height - num);
					Tessellation.TessellateQuad(zero, 0f, leftWidth, topWidth, Tessellation.TessellationType.EdgeVertical, leftColor, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
					num = Mathf.Max(radius.x, leftWidth);
					zero = new Rect(rect.x + num, rect.y, rect.width - num, topWidth);
					Tessellation.TessellateQuad(zero, 0f, leftWidth, topWidth, Tessellation.TessellationType.EdgeHorizontal, topColor, posZ, mesh, ref vertexCount, ref indexCount, countOnly);
				}
			}
		}

		private static Vector2 IntersectEllipseWithLine(float a, float b, Vector2 dir)
		{
			Debug.Assert(dir.x > 0f || dir.y > 0f);
			bool flag = a < Mathf.Epsilon || b < Mathf.Epsilon;
			Vector2 vector;
			if (flag)
			{
				vector = new Vector2(0f, 0f);
			}
			else
			{
				bool flag2 = (double)dir.y < 0.001 * (double)dir.x;
				if (flag2)
				{
					vector = new Vector2(a, 0f);
				}
				else
				{
					bool flag3 = (double)dir.x < 0.001 * (double)dir.y;
					if (flag3)
					{
						vector = new Vector2(0f, b);
					}
					else
					{
						float num = dir.y / dir.x;
						float num2 = b / a;
						float num3 = b * (num2 + num - Mathf.Sqrt(2f * num * num2)) / (num * num + num2 * num2);
						float num4 = num * num3;
						vector = new Vector2(num3, num4);
					}
				}
			}
			return vector;
		}

		private static float GetCenteredEllipseLineIntersectionTheta(float a, float b, Vector2 dir)
		{
			return Mathf.Atan2(dir.y * a, dir.x * b);
		}

		private static Vector2 IntersectLines(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
		{
			Vector2 vector = p3 - p2;
			Vector2 vector2 = p2 - p0;
			Vector2 vector3 = p1 - p0;
			float num = vector.x * vector3.y - vector3.x * vector.y;
			bool flag = Mathf.Approximately(num, 0f);
			Vector2 vector4;
			if (flag)
			{
				vector4 = new Vector2(float.NaN, float.NaN);
			}
			else
			{
				float num2 = vector.x * vector2.y - vector2.x * vector.y;
				float num3 = num2 / num;
				Vector2 vector5 = p0 + vector3 * num3;
				vector4 = vector5;
			}
			return vector4;
		}

		private static int LooseCompare(float a, float b)
		{
			bool flag = a < b - Tessellation.kEpsilon;
			int num;
			if (flag)
			{
				num = -1;
			}
			else
			{
				bool flag2 = a > b + Tessellation.kEpsilon;
				if (flag2)
				{
					num = 1;
				}
				else
				{
					num = 0;
				}
			}
			return num;
		}

		private static void TessellateComplexBorderCorner(Rect rect, Vector2 radius, float leftWidth, float topWidth, Color32 color, float posZ, MeshWriteData mesh, ref ushort refVertexCount, ref ushort refIndexCount, bool countOnly)
		{
			bool flag = rect.width < Tessellation.kEpsilon || rect.height < Tessellation.kEpsilon;
			if (!flag)
			{
				int num = Tessellation.LooseCompare(leftWidth, radius.x);
				int num2 = Tessellation.LooseCompare(topWidth, radius.y);
				Debug.Assert(num != num2 || (num > 0 && num2 > 0));
				ushort num3 = refVertexCount;
				ushort num4 = refIndexCount;
				int num5 = (int)(Tessellation.kSubdivisions - 1);
				if (countOnly)
				{
					int num6 = num5;
					bool flag2 = num2 != 0;
					if (flag2)
					{
						num6++;
					}
					bool flag3 = num != 0;
					if (flag3)
					{
						num6++;
					}
					num3 += (ushort)(num6 + 3);
					num4 += (ushort)(num6 * 3);
					refIndexCount = num4;
					refVertexCount = num3;
				}
				else
				{
					Color32 color2 = new Color32(0, 0, 0, 5);
					Color32 color3 = new Color32(0, 0, 0, 0);
					Vector2 vector = new Vector2(leftWidth, topWidth);
					ushort num7 = num3;
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(leftWidth, topWidth, posZ),
						tint = color,
						uv = vector,
						idsFlags = color2
					});
					num3 += 1;
					ushort num8 = num3;
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(leftWidth, topWidth, posZ),
						tint = color,
						uv = vector,
						idsFlags = color2
					});
					num3 += 1;
					bool flag4 = num2 < 0;
					if (flag4)
					{
						mesh.SetNextVertex(new Vertex
						{
							position = new Vector3(rect.xMax, rect.yMax, posZ),
							tint = color,
							uv = vector,
							idsFlags = color2
						});
						mesh.SetNextVertex(new Vertex
						{
							position = new Vector3(0f, rect.yMax, posZ),
							tint = color,
							idsFlags = color3
						});
						num3 += 2;
						mesh.SetNextIndex(num8);
						mesh.SetNextIndex(num3 - 2);
						mesh.SetNextIndex(num3 - 1);
						num4 += 3;
					}
					else
					{
						mesh.SetNextVertex(new Vertex
						{
							position = new Vector3(0f, rect.yMax, posZ),
							tint = color,
							idsFlags = color3
						});
						num3 += 1;
					}
					bool flag5 = num2 > 0;
					if (flag5)
					{
						mesh.SetNextVertex(new Vertex
						{
							position = new Vector3(0f, radius.y, posZ),
							tint = color,
							idsFlags = color3
						});
						num3 += 1;
						mesh.SetNextIndex(num8);
						mesh.SetNextIndex(num3 - 2);
						mesh.SetNextIndex(num3 - 1);
						num4 += 3;
					}
					float num9 = 1.5707964f / (float)num5;
					for (int i = 1; i < num5; i++)
					{
						float num10 = (float)i * num9;
						Vector2 vector2 = new Vector2(radius.x - Mathf.Cos(num10) * radius.x, radius.y - Mathf.Sin(num10) * radius.y);
						mesh.SetNextVertex(new Vertex
						{
							position = new Vector3(vector2.x, vector2.y, posZ),
							tint = color,
							idsFlags = color3
						});
						num3 += 1;
						mesh.SetNextIndex(num8);
						mesh.SetNextIndex(num3 - 2);
						mesh.SetNextIndex(num3 - 1);
						num4 += 3;
					}
					bool flag6 = num > 0;
					if (flag6)
					{
						mesh.SetNextVertex(new Vertex
						{
							position = new Vector3(radius.x, 0f, posZ),
							tint = color,
							idsFlags = color3
						});
						num3 += 1;
						mesh.SetNextIndex(num8);
						mesh.SetNextIndex(num3 - 2);
						mesh.SetNextIndex(num3 - 1);
						num4 += 3;
					}
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(rect.xMax, 0f, posZ),
						tint = color,
						idsFlags = color3
					});
					num3 += 1;
					mesh.SetNextIndex(num7);
					mesh.SetNextIndex(num3 - 2);
					mesh.SetNextIndex(num3 - 1);
					num4 += 3;
					bool flag7 = num < 0;
					if (flag7)
					{
						mesh.SetNextVertex(new Vertex
						{
							position = new Vector3(rect.xMax, rect.yMax, posZ),
							tint = color,
							uv = vector,
							idsFlags = color2
						});
						num3 += 1;
						mesh.SetNextIndex(num7);
						mesh.SetNextIndex(num3 - 2);
						mesh.SetNextIndex(num3 - 1);
						num4 += 3;
					}
					refIndexCount = num4;
					refVertexCount = num3;
				}
			}
		}

		private static void TessellateComplexBorderCorner(Rect rect, Vector2 radius, float leftWidth, float topWidth, Color32 leftColor, Color32 topColor, float posZ, MeshWriteData mesh, ref ushort vertexCount, ref ushort indexCount, bool countOnly)
		{
			bool flag = rect.width < Tessellation.kEpsilon || rect.height < Tessellation.kEpsilon;
			if (!flag)
			{
				int num = Tessellation.LooseCompare(leftWidth, radius.x);
				int num2 = Tessellation.LooseCompare(topWidth, radius.y);
				Debug.Assert(num != num2 || (num > 0 && num2 > 0));
				if (countOnly)
				{
					vertexCount += Tessellation.kSubdivisions;
					vertexCount += 2;
					vertexCount += 3;
					int num3 = 2;
					num3 += (int)(Tessellation.kSubdivisions - 1);
					bool flag2 = num != 0;
					if (flag2)
					{
						vertexCount += 1;
						num3++;
					}
					bool flag3 = num2 != 0;
					if (flag3)
					{
						vertexCount += 1;
						num3++;
					}
					indexCount += (ushort)(num3 * 3);
				}
				else
				{
					Vector2 vector = new Vector2(rect.x + leftWidth, rect.y + topWidth);
					Vector2 vector2 = new Vector2(rect.x, rect.y);
					Vector2 vector3 = new Vector2(rect.x, rect.y + radius.y);
					Vector2 vector4 = new Vector2(rect.x + radius.x, rect.y);
					Vector2 vector5 = new Vector2(vector4.x, vector3.y);
					Vector2 vector6 = Tessellation.IntersectLines(vector3, vector4, vector, vector2);
					Vector2 vector7 = Tessellation.IntersectEllipseWithLine(radius.x, radius.y, vector - vector2);
					Vector2 vector8 = new Vector2(rect.xMax, rect.y);
					Vector2 vector9 = new Vector2(rect.x, rect.yMax);
					Vector2 vector10 = new Vector2(rect.xMax, rect.yMax);
					float centeredEllipseLineIntersectionTheta = Tessellation.GetCenteredEllipseLineIntersectionTheta(radius.x, radius.y, radius - vector7);
					vector7.x += rect.x;
					vector7.y += rect.y;
					int num4 = (int)(Tessellation.kSubdivisions - 1);
					int num5 = Mathf.Clamp(Mathf.RoundToInt(centeredEllipseLineIntersectionTheta / 1.5707964f * (float)num4), 1, num4 - 1);
					int num6 = num4 - num5;
					Color32 color = new Color32(0, 0, 0, 5);
					Color32 color2 = new Color32(0, 0, 0, 0);
					Vector2 vector11 = new Vector2(leftWidth, topWidth);
					ushort num7 = vertexCount;
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector6.x, vector6.y, posZ),
						tint = leftColor,
						idsFlags = color2
					});
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector.x, vector.y, posZ),
						tint = leftColor,
						uv = vector11,
						idsFlags = color
					});
					vertexCount += 2;
					bool flag4 = num2 < 0;
					if (flag4)
					{
						mesh.SetNextVertex(new Vertex
						{
							position = new Vector3(vector10.x, vector10.y, posZ),
							tint = leftColor,
							uv = vector11,
							idsFlags = color
						});
						vertexCount += 1;
						mesh.SetNextIndex(num7);
						mesh.SetNextIndex(vertexCount - 2);
						mesh.SetNextIndex(vertexCount - 1);
						indexCount += 3;
					}
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector9.x, vector9.y, posZ),
						tint = leftColor,
						idsFlags = color2
					});
					vertexCount += 1;
					mesh.SetNextIndex(num7);
					mesh.SetNextIndex(vertexCount - 2);
					mesh.SetNextIndex(vertexCount - 1);
					indexCount += 3;
					bool flag5 = num2 > 0;
					if (flag5)
					{
						mesh.SetNextVertex(new Vertex
						{
							position = new Vector3(vector3.x, vector3.y, posZ),
							tint = leftColor,
							idsFlags = color2
						});
						vertexCount += 1;
						mesh.SetNextIndex(num7);
						mesh.SetNextIndex(vertexCount - 2);
						mesh.SetNextIndex(vertexCount - 1);
						indexCount += 3;
					}
					float num8 = centeredEllipseLineIntersectionTheta / (float)num5;
					for (int i = 1; i < num5; i++)
					{
						float num9 = (float)i * num8;
						Vector2 vector12 = vector5 - new Vector2(Mathf.Cos(num9), Mathf.Sin(num9)) * radius;
						mesh.SetNextVertex(new Vertex
						{
							position = new Vector3(vector12.x, vector12.y, posZ),
							tint = leftColor,
							idsFlags = color2
						});
						vertexCount += 1;
						mesh.SetNextIndex(num7);
						mesh.SetNextIndex(vertexCount - 2);
						mesh.SetNextIndex(vertexCount - 1);
						indexCount += 3;
					}
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector7.x, vector7.y, posZ),
						tint = leftColor,
						idsFlags = color2
					});
					vertexCount += 1;
					mesh.SetNextIndex(num7);
					mesh.SetNextIndex(vertexCount - 2);
					mesh.SetNextIndex(vertexCount - 1);
					indexCount += 3;
					ushort num10 = vertexCount;
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector6.x, vector6.y, posZ),
						tint = topColor,
						idsFlags = color2
					});
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector7.x, vector7.y, posZ),
						tint = topColor,
						idsFlags = color2
					});
					vertexCount += 2;
					float num11 = (1.5707964f - centeredEllipseLineIntersectionTheta) / (float)num6;
					for (int j = 1; j < num6; j++)
					{
						float num12 = centeredEllipseLineIntersectionTheta + (float)j * num11;
						Vector2 vector13 = vector5 - new Vector2(Mathf.Cos(num12), Mathf.Sin(num12)) * radius;
						mesh.SetNextVertex(new Vertex
						{
							position = new Vector3(vector13.x, vector13.y, posZ),
							tint = topColor,
							idsFlags = color2
						});
						vertexCount += 1;
						mesh.SetNextIndex(num10);
						mesh.SetNextIndex(vertexCount - 2);
						mesh.SetNextIndex(vertexCount - 1);
						indexCount += 3;
					}
					bool flag6 = num > 0;
					if (flag6)
					{
						mesh.SetNextVertex(new Vertex
						{
							position = new Vector3(vector4.x, vector4.y, posZ),
							tint = topColor,
							idsFlags = color2
						});
						vertexCount += 1;
						mesh.SetNextIndex(num10);
						mesh.SetNextIndex(vertexCount - 2);
						mesh.SetNextIndex(vertexCount - 1);
						indexCount += 3;
					}
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector8.x, vector8.y, posZ),
						tint = topColor,
						idsFlags = color2
					});
					vertexCount += 1;
					mesh.SetNextIndex(num10);
					mesh.SetNextIndex(vertexCount - 2);
					mesh.SetNextIndex(vertexCount - 1);
					indexCount += 3;
					bool flag7 = num < 0;
					if (flag7)
					{
						mesh.SetNextVertex(new Vertex
						{
							position = new Vector3(vector10.x, vector10.y, posZ),
							tint = topColor,
							uv = vector11,
							idsFlags = color
						});
						vertexCount += 1;
						mesh.SetNextIndex(num10);
						mesh.SetNextIndex(vertexCount - 2);
						mesh.SetNextIndex(vertexCount - 1);
						indexCount += 3;
					}
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector.x, vector.y, posZ),
						tint = topColor,
						uv = vector11,
						idsFlags = color
					});
					vertexCount += 1;
					mesh.SetNextIndex(num10);
					mesh.SetNextIndex(vertexCount - 2);
					mesh.SetNextIndex(vertexCount - 1);
					indexCount += 3;
				}
			}
		}

		private static void TessellateQuad(Rect rect, float miterOffset, float leftWidth, float topWidth, Tessellation.TessellationType tessellationType, Color32 color, float posZ, MeshWriteData mesh, ref ushort vertexCount, ref ushort indexCount, bool countOnly)
		{
			bool flag = (rect.width < Tessellation.kEpsilon || rect.height < Tessellation.kEpsilon) && tessellationType != Tessellation.TessellationType.EdgeHorizontal && tessellationType != Tessellation.TessellationType.EdgeVertical;
			if (!flag)
			{
				if (countOnly)
				{
					vertexCount += 4;
					indexCount += 6;
				}
				else
				{
					Vector3 vector = new Vector3(rect.x, rect.y, posZ);
					Vector3 vector2 = new Vector3(rect.xMax, rect.y, posZ);
					Vector3 vector3 = new Vector3(rect.x, rect.yMax, posZ);
					Vector3 vector4 = new Vector3(rect.xMax, rect.yMax, posZ);
					Vector2 vector5 = new Vector2(leftWidth, topWidth);
					Vector2 vector7;
					Vector2 vector6;
					Vector2 vector9;
					Vector2 vector8;
					Color32 color2;
					Color32 color3;
					Color32 color4;
					Color32 color5;
					switch (tessellationType)
					{
					case Tessellation.TessellationType.EdgeHorizontal:
						vector3.x += miterOffset;
						vector6 = (vector7 = Vector2.zero);
						vector8 = (vector9 = vector5);
						color2 = new Color32(0, 0, 0, 0);
						color3 = color2;
						color4 = new Color32(0, 0, 0, 5);
						color5 = new Color32(0, 0, 0, 6);
						break;
					case Tessellation.TessellationType.EdgeVertical:
						vector2.y += miterOffset;
						vector9 = (vector7 = Vector2.zero);
						vector8 = (vector6 = vector5);
						color4 = new Color32(0, 0, 0, 0);
						color3 = color4;
						color2 = new Color32(0, 0, 0, 5);
						color5 = new Color32(0, 0, 0, 7);
						break;
					case Tessellation.TessellationType.EdgeCorner:
						vector6 = (vector7 = (vector9 = (vector8 = Vector2.zero)));
						color5 = new Color32(0, 0, 0, 0);
						color2 = (color3 = (color4 = color5));
						break;
					case Tessellation.TessellationType.Content:
						vector6 = (vector7 = (vector9 = (vector8 = Vector2.zero)));
						color5 = new Color32(0, 0, 0, 0);
						color2 = (color3 = (color4 = color5));
						break;
					default:
						throw new NotImplementedException();
					}
					mesh.SetNextVertex(new Vertex
					{
						position = vector,
						uv = vector7,
						tint = color,
						idsFlags = color3
					});
					mesh.SetNextVertex(new Vertex
					{
						position = vector2,
						uv = vector6,
						tint = color,
						idsFlags = color2
					});
					mesh.SetNextVertex(new Vertex
					{
						position = vector3,
						uv = vector9,
						tint = color,
						idsFlags = color4
					});
					mesh.SetNextVertex(new Vertex
					{
						position = vector4,
						uv = vector8,
						tint = color,
						idsFlags = color5
					});
					mesh.SetNextIndex(vertexCount);
					mesh.SetNextIndex(vertexCount + 1);
					mesh.SetNextIndex(vertexCount + 2);
					mesh.SetNextIndex(vertexCount + 3);
					mesh.SetNextIndex(vertexCount + 2);
					mesh.SetNextIndex(vertexCount + 1);
					vertexCount += 4;
					indexCount += 6;
				}
			}
		}

		private static void TessellateFilledFan(Vector2 center, Vector2 radius, float leftWidth, float topWidth, Color32 leftColor, Color32 topColor, float posZ, MeshWriteData mesh, ref ushort vertexCount, ref ushort indexCount, bool countOnly)
		{
			if (countOnly)
			{
				vertexCount += Tessellation.kSubdivisions + 3;
				indexCount += (Tessellation.kSubdivisions - 1) * 3;
			}
			else
			{
				Color32 color = new Color32(0, 0, 0, 5);
				Color32 color2 = new Color32(0, 0, 0, 0);
				Vector2 vector = new Vector2(leftWidth, topWidth);
				float centeredEllipseLineIntersectionTheta = Tessellation.GetCenteredEllipseLineIntersectionTheta(radius.x, radius.y, radius);
				int num = (int)(Tessellation.kSubdivisions - 1);
				int num2 = Mathf.Clamp(Mathf.RoundToInt(centeredEllipseLineIntersectionTheta / 1.5707964f * (float)num), 1, num - 1);
				int num3 = num - num2;
				ushort num4 = vertexCount;
				Vector2 vector2 = new Vector2(center.x - radius.x, center.y);
				mesh.SetNextVertex(new Vertex
				{
					position = new Vector3(center.x, center.y, posZ),
					tint = leftColor,
					idsFlags = color,
					uv = vector
				});
				mesh.SetNextVertex(new Vertex
				{
					position = new Vector3(vector2.x, vector2.y, posZ),
					tint = leftColor,
					idsFlags = color2
				});
				vertexCount += 2;
				float num5 = centeredEllipseLineIntersectionTheta / (float)num2;
				for (int i = 1; i <= num2; i++)
				{
					float num6 = num5 * (float)i;
					vector2 = center - new Vector2(Mathf.Cos(num6), Mathf.Sin(num6)) * radius;
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector2.x, vector2.y, posZ),
						tint = leftColor,
						idsFlags = color2
					});
					vertexCount += 1;
					mesh.SetNextIndex(num4);
					mesh.SetNextIndex(vertexCount - 2);
					mesh.SetNextIndex(vertexCount - 1);
					indexCount += 3;
				}
				ushort num7 = vertexCount;
				mesh.SetNextVertex(new Vertex
				{
					position = new Vector3(center.x, center.y, posZ),
					tint = topColor,
					idsFlags = color,
					uv = vector
				});
				mesh.SetNextVertex(new Vertex
				{
					position = new Vector3(vector2.x, vector2.y, posZ),
					tint = topColor,
					idsFlags = color2
				});
				vertexCount += 2;
				float num8 = (1.5707964f - centeredEllipseLineIntersectionTheta) / (float)num3;
				for (int j = 1; j <= num3; j++)
				{
					float num9 = centeredEllipseLineIntersectionTheta + num8 * (float)j;
					vector2 = center - new Vector2(Mathf.Cos(num9), Mathf.Sin(num9)) * radius;
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector2.x, vector2.y, posZ),
						tint = topColor,
						idsFlags = color2
					});
					vertexCount += 1;
					mesh.SetNextIndex(num7);
					mesh.SetNextIndex(vertexCount - 2);
					mesh.SetNextIndex(vertexCount - 1);
					indexCount += 3;
				}
			}
		}

		private static void TessellateFilledFan(Tessellation.TessellationType tessellationType, Vector2 center, Vector2 radius, float leftWidth, float topWidth, Color32 color, float posZ, MeshWriteData mesh, ref ushort vertexCount, ref ushort indexCount, bool countOnly)
		{
			if (countOnly)
			{
				vertexCount += Tessellation.kSubdivisions + 1;
				indexCount += (Tessellation.kSubdivisions - 1) * 3;
			}
			else
			{
				bool flag = tessellationType == Tessellation.TessellationType.EdgeCorner;
				Color32 color2;
				Color32 color3;
				if (flag)
				{
					color2 = new Color32(0, 0, 0, 5);
					color3 = new Color32(0, 0, 0, 0);
				}
				else
				{
					color2 = new Color32(0, 0, 0, 0);
					color3 = color2;
				}
				Vector2 vector = new Vector2(leftWidth, topWidth);
				Vector2 vector2 = new Vector2(center.x - radius.x, center.y);
				ushort num = vertexCount;
				mesh.SetNextVertex(new Vertex
				{
					position = new Vector3(center.x, center.y, posZ),
					tint = color,
					idsFlags = color2,
					uv = vector
				});
				mesh.SetNextVertex(new Vertex
				{
					position = new Vector3(vector2.x, vector2.y, posZ),
					tint = color,
					idsFlags = color3
				});
				vertexCount += 2;
				for (int i = 1; i < (int)Tessellation.kSubdivisions; i++)
				{
					float num2 = 1.5707964f * (float)i / (float)(Tessellation.kSubdivisions - 1);
					vector2 = center + new Vector2(-Mathf.Cos(num2), -Mathf.Sin(num2)) * radius;
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector2.x, vector2.y, posZ),
						tint = color,
						idsFlags = color3
					});
					vertexCount += 1;
					mesh.SetNextIndex(num);
					mesh.SetNextIndex((ushort)((int)num + i));
					mesh.SetNextIndex((ushort)((int)num + i + 1));
					indexCount += 3;
				}
				num += Tessellation.kSubdivisions + 1;
			}
		}

		private static void TessellateBorderedFan(Vector2 center, Vector2 outerRadius, float leftWidth, float topWidth, Color32 leftColor, Color32 topColor, float posZ, MeshWriteData mesh, ref ushort vertexCount, ref ushort indexCount, bool countOnly)
		{
			if (countOnly)
			{
				vertexCount += Tessellation.kSubdivisions * 2 + 2;
				indexCount += (Tessellation.kSubdivisions - 1) * 6;
			}
			else
			{
				Color32 color = new Color32(0, 0, 0, 5);
				Color32 color2 = new Color32(0, 0, 0, 0);
				Vector2 vector = new Vector2(outerRadius.x - leftWidth, outerRadius.y - topWidth);
				Vector2 vector2 = new Vector2(leftWidth, topWidth);
				Vector2 vector3 = new Vector2(leftWidth, topWidth);
				Vector2 vector4 = Tessellation.IntersectEllipseWithLine(outerRadius.x, outerRadius.y, vector3);
				Vector2 vector5 = Tessellation.IntersectEllipseWithLine(vector.x, vector.y, vector3);
				float centeredEllipseLineIntersectionTheta = Tessellation.GetCenteredEllipseLineIntersectionTheta(outerRadius.x, outerRadius.y, outerRadius - vector4);
				float centeredEllipseLineIntersectionTheta2 = Tessellation.GetCenteredEllipseLineIntersectionTheta(vector.x, vector.y, vector - vector5);
				float num = 0.5f * (centeredEllipseLineIntersectionTheta + centeredEllipseLineIntersectionTheta2);
				int num2 = (int)(Tessellation.kSubdivisions - 1);
				int num3 = Mathf.Clamp(Mathf.RoundToInt(num * 0.63661975f * (float)num2), 1, num2 - 1);
				int num4 = num2 - num3;
				float num5 = centeredEllipseLineIntersectionTheta / (float)num3;
				float num6 = centeredEllipseLineIntersectionTheta2 / (float)num3;
				Vector2 vector6 = new Vector2(center.x - outerRadius.x, center.y);
				Vector2 vector7 = new Vector2(center.x - vector.x, center.y);
				mesh.SetNextVertex(new Vertex
				{
					position = new Vector3(vector7.x, vector7.y, posZ),
					tint = leftColor,
					idsFlags = color,
					uv = vector2
				});
				mesh.SetNextVertex(new Vertex
				{
					position = new Vector3(vector6.x, vector6.y, posZ),
					tint = leftColor,
					idsFlags = color2
				});
				vertexCount += 2;
				for (int i = 1; i <= num3; i++)
				{
					float num7 = (float)i * num5;
					float num8 = (float)i * num6;
					vector6 = center - new Vector2(Mathf.Cos(num7), Mathf.Sin(num7)) * outerRadius;
					vector7 = center - new Vector2(Mathf.Cos(num8), Mathf.Sin(num8)) * vector;
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector7.x, vector7.y, posZ),
						tint = leftColor,
						idsFlags = color,
						uv = vector2
					});
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector6.x, vector6.y, posZ),
						tint = leftColor,
						idsFlags = color2
					});
					vertexCount += 2;
					mesh.SetNextIndex(vertexCount - 4);
					mesh.SetNextIndex(vertexCount - 3);
					mesh.SetNextIndex(vertexCount - 2);
					mesh.SetNextIndex(vertexCount - 3);
					mesh.SetNextIndex(vertexCount - 1);
					mesh.SetNextIndex(vertexCount - 2);
					indexCount += 6;
				}
				float num9 = (1.5707964f - centeredEllipseLineIntersectionTheta) / (float)num4;
				float num10 = (1.5707964f - centeredEllipseLineIntersectionTheta2) / (float)num4;
				color2 = new Color32(0, 0, 0, 0);
				color = color2;
				Vector2 vector8 = center - new Vector2(Mathf.Cos(centeredEllipseLineIntersectionTheta), Mathf.Sin(centeredEllipseLineIntersectionTheta)) * outerRadius;
				Vector2 vector9 = center - new Vector2(Mathf.Cos(centeredEllipseLineIntersectionTheta2), Mathf.Sin(centeredEllipseLineIntersectionTheta2)) * vector;
				mesh.SetNextVertex(new Vertex
				{
					position = new Vector3(vector9.x, vector9.y, posZ),
					tint = topColor,
					idsFlags = color,
					uv = vector2
				});
				mesh.SetNextVertex(new Vertex
				{
					position = new Vector3(vector8.x, vector8.y, posZ),
					tint = topColor,
					idsFlags = color2
				});
				vertexCount += 2;
				for (int j = 1; j <= num4; j++)
				{
					float num11 = centeredEllipseLineIntersectionTheta + (float)j * num9;
					float num12 = centeredEllipseLineIntersectionTheta2 + (float)j * num10;
					vector8 = center - new Vector2(Mathf.Cos(num11), Mathf.Sin(num11)) * outerRadius;
					vector9 = center - new Vector2(Mathf.Cos(num12), Mathf.Sin(num12)) * vector;
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector9.x, vector9.y, posZ),
						tint = topColor,
						idsFlags = color,
						uv = vector2
					});
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector8.x, vector8.y, posZ),
						tint = topColor,
						idsFlags = color2
					});
					vertexCount += 2;
					mesh.SetNextIndex(vertexCount - 4);
					mesh.SetNextIndex(vertexCount - 3);
					mesh.SetNextIndex(vertexCount - 2);
					mesh.SetNextIndex(vertexCount - 3);
					mesh.SetNextIndex(vertexCount - 1);
					mesh.SetNextIndex(vertexCount - 2);
					indexCount += 6;
				}
			}
		}

		private static void TessellateBorderedFan(Vector2 center, Vector2 radius, float leftWidth, float topWidth, Color32 color, float posZ, MeshWriteData mesh, ref ushort vertexCount, ref ushort indexCount, bool countOnly)
		{
			if (countOnly)
			{
				vertexCount += Tessellation.kSubdivisions * 2;
				indexCount += (Tessellation.kSubdivisions - 1) * 6;
			}
			else
			{
				Color32 color2 = new Color32(0, 0, 0, 5);
				Color32 color3 = new Color32(0, 0, 0, 0);
				Vector2 vector = new Vector2(leftWidth, topWidth);
				float num = radius.x - leftWidth;
				float num2 = radius.y - topWidth;
				Vector2 vector2 = new Vector2(center.x - radius.x, center.y);
				Vector2 vector3 = new Vector2(center.x - num, center.y);
				ushort num3 = vertexCount;
				mesh.SetNextVertex(new Vertex
				{
					position = new Vector3(vector3.x, vector3.y, posZ),
					tint = color,
					idsFlags = color2,
					uv = vector
				});
				mesh.SetNextVertex(new Vertex
				{
					position = new Vector3(vector2.x, vector2.y, posZ),
					tint = color,
					idsFlags = color3
				});
				vertexCount += 2;
				for (int i = 1; i < (int)Tessellation.kSubdivisions; i++)
				{
					float num4 = (float)i / (float)(Tessellation.kSubdivisions - 1);
					float num5 = 1.5707964f * num4;
					vector2 = center + new Vector2(-Mathf.Cos(num5), -Mathf.Sin(num5)) * radius;
					vector3 = center + new Vector2(-num * Mathf.Cos(num5), -num2 * Mathf.Sin(num5));
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector3.x, vector3.y, posZ),
						tint = color,
						idsFlags = color2,
						uv = vector
					});
					mesh.SetNextVertex(new Vertex
					{
						position = new Vector3(vector2.x, vector2.y, posZ),
						tint = color,
						idsFlags = color3
					});
					vertexCount += 2;
					int num6 = i * 2;
					mesh.SetNextIndex((ushort)((int)num3 + (num6 - 2)));
					mesh.SetNextIndex((ushort)((int)num3 + (num6 - 1)));
					mesh.SetNextIndex((ushort)((int)num3 + num6));
					mesh.SetNextIndex((ushort)((int)num3 + (num6 - 1)));
					mesh.SetNextIndex((ushort)((int)num3 + (num6 + 1)));
					mesh.SetNextIndex((ushort)((int)num3 + num6));
					indexCount += 6;
				}
				num3 += Tessellation.kSubdivisions * 2;
			}
		}

		private static void MirrorVertices(Rect rect, NativeSlice<Vertex> vertices, int vertexStart, int vertexCount, bool flipHorizontal)
		{
			if (flipHorizontal)
			{
				for (int i = 0; i < vertexCount; i++)
				{
					Vertex vertex = vertices[vertexStart + i];
					vertex.position.x = rect.xMax - (vertex.position.x - rect.xMax);
					vertex.uv.x = -vertex.uv.x;
					vertices[vertexStart + i] = vertex;
				}
			}
			else
			{
				for (int j = 0; j < vertexCount; j++)
				{
					Vertex vertex2 = vertices[vertexStart + j];
					vertex2.position.y = rect.yMax - (vertex2.position.y - rect.yMax);
					vertex2.uv.y = -vertex2.uv.y;
					vertices[vertexStart + j] = vertex2;
				}
			}
		}

		private static void FlipWinding(NativeSlice<ushort> indices, int indexStart, int indexCount)
		{
			for (int i = 0; i < indexCount; i += 3)
			{
				ushort num = indices[indexStart + i];
				indices[indexStart + i] = indices[indexStart + i + 1];
				indices[indexStart + i + 1] = num;
			}
		}

		private static void ComputeUVs(Rect tessellatedRect, Rect textureRect, Rect uvRegion, NativeSlice<Vertex> vertices)
		{
			Vector2 position = tessellatedRect.position;
			Vector2 vector = new Vector2(1f / tessellatedRect.width, 1f / tessellatedRect.height);
			for (int i = 0; i < vertices.Length; i++)
			{
				Vertex vertex = vertices[i];
				Vector2 vector2 = vertex.position;
				vector2 -= position;
				vector2 *= vector;
				vertex.uv.x = (vector2.x * textureRect.width + textureRect.xMin) * uvRegion.width + uvRegion.xMin;
				vertex.uv.y = ((1f - vector2.y) * textureRect.height + textureRect.yMin) * uvRegion.height + uvRegion.yMin;
				vertices[i] = vertex;
			}
		}

		internal static float kEpsilon = 0.001f;

		internal static ushort kSubdivisions = 6;

		private static ProfilerMarker s_MarkerTessellateRect = new ProfilerMarker("TessellateRect");

		private static ProfilerMarker s_MarkerTessellateBorder = new ProfilerMarker("TessellateBorder");

		private enum TessellationType
		{
			EdgeHorizontal,
			EdgeVertical,
			EdgeCorner,
			Content
		}
	}
}
