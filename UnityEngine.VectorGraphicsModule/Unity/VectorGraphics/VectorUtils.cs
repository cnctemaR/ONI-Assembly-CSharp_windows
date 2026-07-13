using System;
using System.Collections.Generic;
using LibTessDotNet;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.Rendering;
using UnityEngine.U2D;
using UnityEngine.UIElements;

namespace Unity.VectorGraphics
{
	public static class VectorUtils
	{
		internal static BezierPathSegment[] BuildEllipsePath(Vector2 p0, Vector2 p1, float rotation, float rx, float ry, bool largeArc, bool sweep)
		{
			bool flag = (p1 - p0).magnitude < VectorUtils.Epsilon;
			BezierPathSegment[] array;
			if (flag)
			{
				array = new BezierPathSegment[0];
			}
			else
			{
				Vector2 vector;
				float num;
				float num2;
				float num3;
				float num4;
				VectorUtils.ComputeEllipseParameters(p0, p1, rotation, rx, ry, largeArc, sweep, out vector, out num, out num2, out num3, out num4);
				bool flag2 = Mathf.Abs(num2) <= Mathf.Epsilon;
				BezierPathSegment[] array2;
				if (flag2)
				{
					array2 = VectorUtils.BezierSegmentToPath(VectorUtils.MakeLine(p0, p1));
				}
				else
				{
					array2 = VectorUtils.MakeArc(Vector2.zero, num, num2, 1f);
					Vector2 vector2 = new Vector2(num3, num4);
					array2 = VectorUtils.TransformBezierPath(array2, vector, rotation, vector2);
				}
				array = array2;
			}
			return array;
		}

		private static void ComputeEllipseParameters(Vector2 p0, Vector2 p1, float phi, float rx, float ry, bool fa, bool fs, out Vector2 c, out float theta1, out float sweepTheta, out float adjustedRx, out float adjustedRy)
		{
			float num = Mathf.Cos(phi);
			float num2 = Mathf.Sin(phi);
			Matrix2D identity = Matrix2D.identity;
			identity.m00 = num;
			identity.m01 = -num2;
			identity.m10 = num2;
			identity.m11 = num;
			Matrix2D matrix2D = identity;
			matrix2D.m01 = -matrix2D.m01;
			matrix2D.m10 = -matrix2D.m10;
			Vector2 vector = identity * new Vector2((p0.x - p1.x) / 2f, (p0.y - p1.y) / 2f);
			rx = Mathf.Abs(rx);
			ry = Mathf.Abs(ry);
			VectorUtils.EnsureRadiiAreLargeEnough(vector, ref rx, ref ry);
			adjustedRx = rx;
			adjustedRy = ry;
			float num3 = vector.x * vector.x;
			float num4 = vector.y * vector.y;
			float num5 = rx * rx;
			float num6 = ry * ry;
			Vector2 vector2 = new Vector2(rx * vector.y / ry, -(ry * vector.x) / rx);
			vector2 *= Mathf.Sqrt(Mathf.Abs((num5 * num6 - num5 * num4 - num6 * num3) / (num5 * num4 + num6 * num3)));
			bool flag = fa == fs;
			if (flag)
			{
				vector2 = -vector2;
			}
			c = matrix2D * vector2 + new Vector2((p0.x + p1.x) / 2f, (p0.y + p1.y) / 2f);
			theta1 = Vector2.SignedAngle(new Vector2(1f, 0f), new Vector2((vector.x - vector2.x) / rx, (vector.y - vector2.y) / ry)) % 360f;
			sweepTheta = Vector2.SignedAngle(new Vector2((vector.x - vector2.x) / rx, (vector.y - vector2.y) / ry), new Vector2((-vector.x - vector2.x) / rx, (-vector.y - vector2.y) / ry));
			bool flag2 = !fs && sweepTheta > 0f;
			if (flag2)
			{
				sweepTheta -= 360f;
			}
			bool flag3 = fs && sweepTheta < 0f;
			if (flag3)
			{
				sweepTheta += 360f;
			}
			theta1 *= 0.017453292f;
			sweepTheta *= 0.017453292f;
		}

		private static void EnsureRadiiAreLargeEnough(Vector2 p, ref float rx, ref float ry)
		{
			float num = p.x * p.x / (rx * rx) + p.y * p.y / (ry * ry);
			bool flag = num > 1f;
			if (flag)
			{
				float num2 = Mathf.Sqrt(num);
				rx *= num2;
				ry *= num2;
			}
		}

		public static VectorImage BuildVectorImage(IEnumerable<VectorUtils.Geometry> geoms, uint gradientResolution = 16U)
		{
			return VectorUtils.BuildVectorImage(geoms, Rect.zero, gradientResolution);
		}

		[VisibleToOtherModules(new string[] { "UnityEditor.VectorGraphicsModule" })]
		internal static VectorImage BuildVectorImage(IEnumerable<VectorUtils.Geometry> geoms, Rect rect, uint gradientResolution)
		{
			VectorImage vectorImage;
			Texture2D texture2D;
			VectorImageUtils.MakeVectorImageAsset(geoms, rect, gradientResolution, out vectorImage, out texture2D);
			return vectorImage;
		}

		public static VectorImage BuildVectorImage(SVGParser.SceneInfo sceneInfo)
		{
			VectorImage vectorImage2;
			using (Painter2D painter2D = new Painter2D())
			{
				SceneNode root = sceneInfo.Scene.Root;
				VectorUtils.DrawSceneWithPainter2D(root, root.Transform, painter2D, 1f, sceneInfo.NodeOpacity);
				VectorImage vectorImage = ScriptableObject.CreateInstance<VectorImage>();
				painter2D.SaveToVectorImage(vectorImage);
				vectorImage2 = vectorImage;
			}
			return vectorImage2;
		}

		private static void DrawSceneWithPainter2D(SceneNode node, Matrix2D matrix, Painter2D painter, float combinedOpacity, Dictionary<SceneNode, float> nodeOpacities)
		{
			bool flag = node == null;
			if (!flag)
			{
				bool flag2 = node.Shapes != null;
				if (flag2)
				{
					foreach (Shape shape in node.Shapes)
					{
						bool flag3 = shape.Contours == null || shape.Contours.Length == 0;
						if (flag3)
						{
							break;
						}
						painter.opacity = combinedOpacity;
						painter.BeginPath();
						foreach (BezierContour bezierContour in shape.Contours)
						{
							BezierPathSegment[] segments = bezierContour.Segments;
							bool flag4 = segments == null || segments.Length == 0;
							if (flag4)
							{
								break;
							}
							painter.MoveTo(matrix.MultiplyPoint(segments[0].P0));
							for (int j = 0; j < segments.Length - 1; j++)
							{
								BezierPathSegment bezierPathSegment = segments[j];
								BezierPathSegment bezierPathSegment2 = segments[j + 1];
								painter.BezierCurveTo(matrix.MultiplyPoint(bezierPathSegment.P1), matrix.MultiplyPoint(bezierPathSegment.P2), matrix.MultiplyPoint(bezierPathSegment2.P0));
							}
							bool closed = bezierContour.Closed;
							if (closed)
							{
								BezierPathSegment bezierPathSegment3 = segments[segments.Length - 1];
								BezierPathSegment bezierPathSegment4 = segments[0];
								painter.BezierCurveTo(matrix.MultiplyPoint(bezierPathSegment3.P1), matrix.MultiplyPoint(bezierPathSegment3.P2), matrix.MultiplyPoint(bezierPathSegment4.P0));
								painter.ClosePath();
							}
						}
						bool flag5 = shape.Fill != null;
						if (flag5)
						{
							SolidFill solidFill = shape.Fill as SolidFill;
							bool flag6 = solidFill != null;
							if (flag6)
							{
								painter.fillColor = solidFill.Color;
								painter.Fill((solidFill.Mode == FillMode.NonZero) ? FillRule.NonZero : FillRule.OddEven);
							}
							GradientFill gradientFill = shape.Fill as GradientFill;
							bool flag7 = gradientFill != null;
							if (flag7)
							{
								FillGradient fillGradient;
								Matrix2D matrix2D;
								VectorUtils.ComputeFillGradientFromGradientFill(node, shape, gradientFill, out fillGradient, out matrix2D);
								painter.fillGradient = fillGradient;
								painter.fillTransform = matrix2D.ToMatrix4x4();
								painter.Fill((gradientFill.Mode == FillMode.NonZero) ? FillRule.NonZero : FillRule.OddEven);
							}
						}
						bool flag8 = shape.PathProps.Stroke != null;
						if (flag8)
						{
							PathProperties pathProps = shape.PathProps;
							Stroke stroke = pathProps.Stroke;
							GradientFill gradientFill2 = stroke.Fill as GradientFill;
							bool flag9 = gradientFill2 != null;
							if (flag9)
							{
								FillGradient fillGradient2;
								Matrix2D matrix2D2;
								VectorUtils.ComputeFillGradientFromGradientFill(node, shape, gradientFill2, out fillGradient2, out matrix2D2);
								painter.strokeFillGradient = fillGradient2;
								painter.fillTransform = matrix2D2.ToMatrix4x4();
							}
							else
							{
								painter.strokeColor = stroke.Color;
							}
							painter.lineWidth = stroke.HalfThickness * 2f;
							painter.lineJoin = ((pathProps.Corners == PathCorner.Tipped) ? LineJoin.Miter : ((pathProps.Corners == PathCorner.Beveled) ? LineJoin.Bevel : LineJoin.Round));
							painter.lineCap = ((pathProps.Head == PathEnding.Round || pathProps.Tail == PathEnding.Round) ? LineCap.Round : LineCap.Butt);
							painter.dashPattern = stroke.Pattern;
							painter.dashOffset = stroke.PatternOffset;
							painter.Stroke();
						}
					}
				}
				bool flag10 = node.Children != null;
				if (flag10)
				{
					foreach (SceneNode sceneNode in node.Children)
					{
						float num = 1f;
						bool flag11 = nodeOpacities == null || !nodeOpacities.TryGetValue(sceneNode, out num);
						if (flag11)
						{
							num = 1f;
						}
						float num2 = combinedOpacity * num;
						Matrix2D matrix2D3 = matrix * sceneNode.Transform;
						VectorUtils.DrawSceneWithPainter2D(sceneNode, matrix2D3, painter, num2, nodeOpacities);
					}
				}
			}
		}

		private static void ComputeFillGradientFromGradientFill(SceneNode node, Shape shape, GradientFill gradientFill, out FillGradient outFillGradient, out Matrix2D outFillTransform)
		{
			Rect rect = VectorUtils.SceneNodeBounds(node);
			outFillTransform = Matrix2D.Translate(new Vector2(0f, 1f)) * Matrix2D.Scale(new Vector2(1f, -1f)) * shape.FillTransform * Matrix2D.Scale(new Vector2(1f / rect.width, 1f / rect.height)) * Matrix2D.Translate(-rect.position);
			AddressMode addressMode = AddressMode.Mirror;
			bool flag = gradientFill.Addressing == AddressMode.Wrap;
			if (flag)
			{
				addressMode = AddressMode.Wrap;
			}
			else
			{
				bool flag2 = gradientFill.Addressing == AddressMode.Clamp;
				if (flag2)
				{
					addressMode = AddressMode.Clamp;
				}
			}
			Gradient gradient = new Gradient();
			GradientColorKey[] array = new GradientColorKey[gradientFill.Stops.Length];
			GradientAlphaKey[] array2 = new GradientAlphaKey[gradientFill.Stops.Length];
			for (int i = 0; i < gradientFill.Stops.Length; i++)
			{
				GradientStop gradientStop = gradientFill.Stops[i];
				array[i] = new GradientColorKey
				{
					color = gradientStop.Color,
					time = gradientStop.StopPercentage
				};
				array2[i] = new GradientAlphaKey
				{
					alpha = gradientStop.Color.a,
					time = gradientStop.StopPercentage
				};
			}
			gradient.colorKeys = array;
			gradient.alphaKeys = array2;
			Vector2 position = rect.position;
			Vector2 vector = position + Vector2.right * rect.size;
			outFillGradient = FillGradient.MakeLinearGradient(gradient, position, vector, addressMode);
			bool flag3 = gradientFill.Type == GradientFillType.Radial;
			if (flag3)
			{
				outFillGradient.gradientType = GradientType.Radial;
				outFillGradient.focus = gradientFill.RadialFocus;
			}
		}

		private static Color SampleGradient(GradientStop[] stops, float u)
		{
			bool flag = stops == null;
			Color color;
			if (flag)
			{
				color = Color.white;
			}
			else
			{
				int i;
				for (i = 0; i < stops.Length; i++)
				{
					bool flag2 = u < stops[i].StopPercentage;
					if (flag2)
					{
						break;
					}
				}
				bool flag3 = i >= stops.Length;
				if (flag3)
				{
					color = stops[stops.Length - 1].Color;
				}
				else
				{
					bool flag4 = i == 0;
					if (flag4)
					{
						color = stops[0].Color;
					}
					else
					{
						float num = stops[i].StopPercentage - stops[i - 1].StopPercentage;
						bool flag5 = num > VectorUtils.Epsilon;
						if (flag5)
						{
							float num2 = (u - stops[i - 1].StopPercentage) / num;
							color = Color.LerpUnclamped(stops[i - 1].Color, stops[i].Color, num2);
						}
						else
						{
							color = stops[i - 1].Color;
						}
					}
				}
			}
			return color;
		}

		private static Vector2 RayUnitCircleFirstHit(Vector2 rayStart, Vector2 rayDir)
		{
			float num = Vector2.Dot(-rayStart, rayDir);
			float num2 = Vector2.Dot(rayStart, rayStart) - num * num;
			float num3 = Mathf.Sqrt(1f - num2);
			float num4 = num - num3;
			float num5 = num + num3;
			float num6 = Mathf.Min(num4, num5);
			bool flag = num6 < 0f;
			if (flag)
			{
				num6 = Mathf.Max(num4, num5);
			}
			return rayStart + rayDir * num6;
		}

		private static float RadialAddress(Vector2 uv, Vector2 focus)
		{
			uv = (uv - new Vector2(0.5f, 0.5f)) * 2f;
			Vector2 vector = VectorUtils.RayUnitCircleFirstHit(focus, (uv - focus).normalized);
			Vector2 vector2 = vector - focus;
			bool flag = Mathf.Abs(vector2.x) > VectorUtils.Epsilon;
			float num;
			if (flag)
			{
				num = (uv.x - focus.x) / vector2.x;
			}
			else
			{
				bool flag2 = Mathf.Abs(vector2.y) > VectorUtils.Epsilon;
				if (flag2)
				{
					num = (uv.y - focus.y) / vector2.y;
				}
				else
				{
					num = 0f;
				}
			}
			return num;
		}

		private static Color32[] RasterizeGradient(GradientFill gradient, int width, int height)
		{
			Color32[] array = new Color32[width * height];
			bool flag = gradient.Type == GradientFillType.Linear;
			if (flag)
			{
				int num = 0;
				for (int i = 0; i < width; i++)
				{
					array[num++] = VectorUtils.SampleGradient(gradient.Stops, (float)i / (float)(width - 1));
				}
				for (int j = 1; j < height; j++)
				{
					Array.Copy(array, 0, array, num, width);
					num += width;
				}
			}
			else
			{
				bool flag2 = gradient.Type == GradientFillType.Radial;
				if (flag2)
				{
					int num2 = 0;
					for (int k = 0; k < height; k++)
					{
						float num3 = (float)k / ((float)height - 1f);
						for (int l = 0; l < width; l++)
						{
							float num4 = (float)l / ((float)width - 1f);
							array[num2++] = VectorUtils.SampleGradient(gradient.Stops, VectorUtils.RadialAddress(new Vector2(num4, 1f - num3), gradient.RadialFocus));
						}
					}
				}
			}
			return array;
		}

		private static Color32[] RasterizeGradientStripe(GradientFill gradient, int width)
		{
			Color32[] array = new Color32[width];
			for (int i = 0; i < width; i++)
			{
				float num = (float)i / ((float)width - 1f);
				array[i] = VectorUtils.SampleGradient(gradient.Stops, num);
			}
			return array;
		}

		private static List<VectorUtils.PackRectItem> PackRects(IList<KeyValuePair<IFill, Vector2>> fillSizes, out Vector2 atlasDims)
		{
			List<VectorUtils.PackRectItem> list = new List<VectorUtils.PackRectItem>(fillSizes.Count);
			Dictionary<IFill, int> dictionary = new Dictionary<IFill, int>();
			atlasDims = new Vector2(1024f, 1024f);
			Vector2 vector = Vector2.zero;
			Vector2 zero = Vector2.zero;
			float num = 0f;
			int num2 = 1;
			foreach (KeyValuePair<IFill, Vector2> keyValuePair in fillSizes)
			{
				IFill key = keyValuePair.Key;
				Vector2 value = keyValuePair.Value;
				bool flag = atlasDims.y < zero.y + value.y;
				if (flag)
				{
					bool flag2 = atlasDims.y < value.y;
					if (flag2)
					{
						atlasDims.y = value.y;
					}
					bool flag3 = zero.y != 0f;
					if (flag3)
					{
						zero.x += num;
					}
					zero.y = 0f;
					num = value.x;
				}
				num = Mathf.Max(num, value.x);
				int num3 = 0;
				bool flag4 = key != null;
				if (flag4)
				{
					bool flag5 = !dictionary.TryGetValue(key, out num3);
					if (flag5)
					{
						num3 = num2++;
						dictionary[key] = num3;
					}
				}
				list.Add(new VectorUtils.PackRectItem
				{
					Position = zero,
					Size = value,
					Fill = key,
					SettingIndex = num3
				});
				vector = Vector2.Max(vector, zero + value);
				zero.y += value.y;
			}
			atlasDims = vector;
			return list;
		}

		private static void BlitRawTexture(VectorUtils.RawTexture src, VectorUtils.RawTexture dest, int destX, int destY, bool rotate)
		{
			if (rotate)
			{
				for (int i = 0; i < src.Height; i++)
				{
					int num = i * src.Width;
					int num2 = destY * dest.Width + destX + i;
					for (int j = 0; j < src.Width; j++)
					{
						int num3 = num + j;
						int num4 = num2 + j * dest.Width;
						dest.Rgba[num4] = src.Rgba[num3];
					}
				}
			}
			else
			{
				for (int k = 0; k < src.Height; k++)
				{
					Array.Copy(src.Rgba, k * src.Width, dest.Rgba, (destY + k) * dest.Width + destX, src.Width);
				}
			}
		}

		internal static void WriteRawInt2Packed(VectorUtils.RawTexture dest, int v0, int v1, int destX, int destY)
		{
			byte b = (byte)(v0 / 255);
			byte b2 = (byte)(v0 - (int)(b * byte.MaxValue));
			byte b3 = (byte)(v1 / 255);
			byte b4 = (byte)(v1 - (int)(b3 * byte.MaxValue));
			int num = destY * dest.Width + destX;
			dest.Rgba[num] = new Color32(b, b2, b3, b4);
		}

		internal static void WriteRawFloat4Packed(VectorUtils.RawTexture dest, float f0, float f1, float f2, float f3, int destX, int destY)
		{
			byte b = (byte)(f0 * 255f + 0.5f);
			byte b2 = (byte)(f1 * 255f + 0.5f);
			byte b3 = (byte)(f2 * 255f + 0.5f);
			byte b4 = (byte)(f3 * 255f + 0.5f);
			int num = destY * dest.Width + destX;
			bool flag = num >= dest.Rgba.Length;
			if (flag)
			{
				int num2 = 0;
				num2++;
			}
			dest.Rgba[num] = new Color32(b, b2, b3, b4);
		}

		public static BezierContour BuildRectangleContour(Rect rect, Vector2 radiusTL, Vector2 radiusTR, Vector2 radiusBR, Vector2 radiusBL)
		{
			float x = rect.size.x;
			float y = rect.size.y;
			Vector2 vector = new Vector2(x / 2f, y / 2f);
			radiusTL = Vector2.Max(Vector2.Min(radiusTL, vector), Vector2.zero);
			radiusTR = Vector2.Max(Vector2.Min(radiusTR, vector), Vector2.zero);
			radiusBR = Vector2.Max(Vector2.Min(radiusBR, vector), Vector2.zero);
			radiusBL = Vector2.Max(Vector2.Min(radiusBL, vector), Vector2.zero);
			float num = y - (radiusBL.y + radiusTL.y);
			float num2 = x - (radiusTL.x + radiusTR.x);
			float num3 = y - (radiusBR.y + radiusTR.y);
			float num4 = x - (radiusBL.x + radiusBR.x);
			List<BezierPathSegment> list = new List<BezierPathSegment>(8);
			bool flag = num > VectorUtils.Epsilon;
			if (flag)
			{
				BezierPathSegment bezierPathSegment = VectorUtils.MakePathLine(new Vector2(0f, radiusTL.y + num), new Vector2(0f, radiusTL.y))[0];
				list.Add(bezierPathSegment);
			}
			bool flag2 = radiusTL.magnitude > VectorUtils.Epsilon;
			if (flag2)
			{
				BezierPathSegment[] array = VectorUtils.MakeArc(Vector2.zero, -3.1415927f, 1.5707964f, 1f);
				array = VectorUtils.TransformBezierPath(array, radiusTL, 0f, radiusTL);
				list.Add(array[0]);
			}
			bool flag3 = num2 > VectorUtils.Epsilon;
			if (flag3)
			{
				BezierPathSegment bezierPathSegment = VectorUtils.MakePathLine(new Vector2(radiusTL.x, 0f), new Vector2(radiusTL.x + num2, 0f))[0];
				list.Add(bezierPathSegment);
			}
			bool flag4 = radiusTR.magnitude > VectorUtils.Epsilon;
			if (flag4)
			{
				Vector2 vector2 = new Vector2(x - radiusTR.x, radiusTR.y);
				BezierPathSegment[] array2 = VectorUtils.MakeArc(Vector2.zero, -1.5707964f, 1.5707964f, 1f);
				array2 = VectorUtils.TransformBezierPath(array2, vector2, 0f, radiusTR);
				list.Add(array2[0]);
			}
			bool flag5 = num3 > VectorUtils.Epsilon;
			if (flag5)
			{
				BezierPathSegment bezierPathSegment = VectorUtils.MakePathLine(new Vector2(x, radiusTR.y), new Vector2(x, radiusTR.y + num3))[0];
				list.Add(bezierPathSegment);
			}
			bool flag6 = radiusBR.magnitude > VectorUtils.Epsilon;
			if (flag6)
			{
				Vector2 vector3 = new Vector2(x - radiusBR.x, y - radiusBR.y);
				BezierPathSegment[] array3 = VectorUtils.MakeArc(Vector2.zero, 0f, 1.5707964f, 1f);
				array3 = VectorUtils.TransformBezierPath(array3, vector3, 0f, radiusBR);
				list.Add(array3[0]);
			}
			bool flag7 = num4 > VectorUtils.Epsilon;
			if (flag7)
			{
				BezierPathSegment bezierPathSegment = VectorUtils.MakePathLine(new Vector2(x - radiusBR.x, y), new Vector2(x - (radiusBR.x + num4), y))[0];
				list.Add(bezierPathSegment);
			}
			bool flag8 = radiusBL.magnitude > VectorUtils.Epsilon;
			if (flag8)
			{
				Vector2 vector4 = new Vector2(radiusBL.x, y - radiusBL.y);
				BezierPathSegment[] array4 = VectorUtils.MakeArc(Vector2.zero, 1.5707964f, 1.5707964f, 1f);
				array4 = VectorUtils.TransformBezierPath(array4, vector4, 0f, radiusBL);
				list.Add(array4[0]);
			}
			for (int i = 0; i < list.Count; i++)
			{
				BezierPathSegment bezierPathSegment2 = list[i];
				bezierPathSegment2.P0 += rect.position;
				bezierPathSegment2.P1 += rect.position;
				bezierPathSegment2.P2 += rect.position;
				list[i] = bezierPathSegment2;
			}
			return new BezierContour
			{
				Segments = list.ToArray(),
				Closed = true
			};
		}

		public static List<VectorUtils.Geometry> TessellateScene(Scene scene, VectorUtils.TessellationOptions tessellationOptions, Dictionary<SceneNode, float> nodeOpacities = null)
		{
			VectorClip.ResetClip();
			return VectorUtils.TessellateNodeHierarchyRecursive(scene.Root, tessellationOptions, scene.Root.Transform, 1f, nodeOpacities);
		}

		private static List<VectorUtils.Geometry> TessellateNodeHierarchyRecursive(SceneNode node, VectorUtils.TessellationOptions tessellationOptions, Matrix2D worldTransform, float worldOpacity, Dictionary<SceneNode, float> nodeOpacities)
		{
			bool flag = node.Clipper != null;
			if (flag)
			{
				VectorClip.PushClip(VectorUtils.TraceNodeHierarchyShapes(node.Clipper, tessellationOptions), worldTransform);
			}
			List<VectorUtils.Geometry> list = new List<VectorUtils.Geometry>();
			bool flag2 = node.Shapes != null;
			if (flag2)
			{
				foreach (Shape shape in node.Shapes)
				{
					bool flag3 = shape.IsConvex && shape.Contours.Length == 1;
					VectorUtils.TessellateShape(shape, list, tessellationOptions, flag3);
				}
			}
			foreach (VectorUtils.Geometry geometry in list)
			{
				VectorUtils.Geometry geometry2 = geometry;
				geometry2.Color.a = geometry2.Color.a * worldOpacity;
				geometry.WorldTransform = worldTransform;
				geometry.UnclippedBounds = VectorUtils.Bounds(geometry.Vertices);
				VectorClip.ClipGeometry(geometry);
			}
			bool flag4 = node.Children != null;
			if (flag4)
			{
				foreach (SceneNode sceneNode in node.Children)
				{
					float num = 1f;
					bool flag5 = nodeOpacities == null || !nodeOpacities.TryGetValue(sceneNode, out num);
					if (flag5)
					{
						num = 1f;
					}
					Matrix2D matrix2D = worldTransform * sceneNode.Transform;
					float num2 = worldOpacity * num;
					List<VectorUtils.Geometry> list2 = VectorUtils.TessellateNodeHierarchyRecursive(sceneNode, tessellationOptions, matrix2D, num2, nodeOpacities);
					list.AddRange(list2);
				}
			}
			bool flag6 = node.Clipper != null;
			if (flag6)
			{
				VectorClip.PopClip();
			}
			return list;
		}

		internal static List<Vector2[]> TraceNodeHierarchyShapes(SceneNode root, VectorUtils.TessellationOptions tessellationOptions)
		{
			List<Vector2[]> list = new List<Vector2[]>();
			foreach (VectorUtils.SceneNodeWorldTransform sceneNodeWorldTransform in VectorUtils.WorldTransformedSceneNodes(root, null))
			{
				SceneNode node = sceneNodeWorldTransform.Node;
				bool flag = node.Shapes != null;
				if (flag)
				{
					foreach (Shape shape in node.Shapes)
					{
						foreach (BezierContour bezierContour in shape.Contours)
						{
							Vector2[] array = VectorUtils.TraceShape(bezierContour, shape.PathProps.Stroke, tessellationOptions);
							bool flag2 = array.Length != 0;
							if (flag2)
							{
								Vector2[] array2 = new Vector2[array.Length];
								for (int j = 0; j < array.Length; j++)
								{
									array2[j] = sceneNodeWorldTransform.WorldTransform * array[j];
								}
								list.Add(array2);
							}
						}
					}
				}
			}
			return list;
		}

		private static void TessellateShape(Shape vectorShape, List<VectorUtils.Geometry> geoms, VectorUtils.TessellationOptions tessellationOptions, bool isConvex)
		{
			bool flag = vectorShape.Fill != null && !(vectorShape.Fill is PatternFill);
			if (flag)
			{
				Color color = Color.white;
				bool flag2 = vectorShape.Fill is SolidFill;
				if (flag2)
				{
					color = ((SolidFill)vectorShape.Fill).Color;
				}
				color.a *= vectorShape.Fill.Opacity;
				bool flag3 = isConvex && vectorShape.Contours.Length == 1;
				if (flag3)
				{
					VectorUtils.TessellateConvexContour(vectorShape, vectorShape.PathProps.Stroke, color, geoms, tessellationOptions);
				}
				else
				{
					VectorUtils.TessellateShapeLibTess(vectorShape, color, geoms, tessellationOptions);
				}
			}
			Stroke stroke = vectorShape.PathProps.Stroke;
			bool flag4 = stroke != null && stroke.HalfThickness > VectorUtils.Epsilon;
			if (flag4)
			{
				IFill fill = stroke.Fill;
				Color color2 = Color.white;
				bool flag5 = fill is SolidFill;
				if (flag5)
				{
					color2 = ((SolidFill)fill).Color;
					fill = null;
				}
				foreach (BezierContour bezierContour in vectorShape.Contours)
				{
					Vector2[] array;
					ushort[] array2;
					VectorUtils.TessellatePath(bezierContour, vectorShape.PathProps, tessellationOptions, out array, out array2);
					VectorUtils.AdjustWinding(array, array2, VectorUtils.WindingDir.CCW);
					bool flag6 = array2.Length != 0;
					if (flag6)
					{
						geoms.Add(new VectorUtils.Geometry
						{
							Vertices = array,
							Indices = array2,
							Color = color2,
							Fill = fill,
							FillTransform = stroke.FillTransform
						});
					}
				}
			}
		}

		private static void TessellateConvexContour(Shape shape, Stroke stroke, Color color, List<VectorUtils.Geometry> geoms, VectorUtils.TessellationOptions tessellationOptions)
		{
			bool flag = shape.Contours.Length != 1 || shape.Contours[0].Segments.Length == 0;
			if (!flag)
			{
				BezierContour bezierContour = shape.Contours[0];
				Vector2 vector = Vector2.zero;
				foreach (BezierPathSegment bezierPathSegment in bezierContour.Segments)
				{
					vector += bezierPathSegment.P0;
				}
				vector /= (float)bezierContour.Segments.Length;
				Vector2[] array = VectorUtils.TraceShape(bezierContour, stroke, tessellationOptions);
				Vector2[] array2 = new Vector2[array.Length + 1];
				ushort[] array3 = new ushort[array.Length * 3];
				array2[0] = vector;
				for (int j = 0; j < array.Length; j++)
				{
					array2[j + 1] = array[j];
					array3[j * 3] = 0;
					array3[j * 3 + 1] = (ushort)(j + 1);
					array3[j * 3 + 2] = ((j + 2 >= array2.Length) ? 1 : ((ushort)(j + 2)));
				}
				geoms.Add(new VectorUtils.Geometry
				{
					Vertices = array2,
					Indices = array3,
					Color = color,
					Fill = shape.Fill,
					FillTransform = shape.FillTransform
				});
			}
		}

		private static void TessellateShapeLibTess(Shape vectorShape, Color color, List<VectorUtils.Geometry> geoms, VectorUtils.TessellationOptions tessellationOptions)
		{
			Tess tess = new Tess();
			float num = 0.7853982f;
			Matrix2D matrix2D = Matrix2D.RotateLH(num);
			Matrix2D matrix2D2 = Matrix2D.RotateLH(-num);
			foreach (BezierContour bezierContour in vectorShape.Contours)
			{
				List<Vector2> list = new List<Vector2>(100);
				foreach (Vector2 vector in VectorUtils.TraceShape(bezierContour, vectorShape.PathProps.Stroke, tessellationOptions))
				{
					list.Add(matrix2D.MultiplyPoint(vector));
				}
				ContourVertex[] array2 = new ContourVertex[list.Count];
				for (int k = 0; k < list.Count; k++)
				{
					Vector2 vector2 = list[k];
					array2[k] = new ContourVertex
					{
						Position = new Vec3
						{
							X = vector2.x,
							Y = vector2.y
						}
					};
				}
				tess.AddContour(array2, ContourOrientation.Original);
			}
			WindingRule windingRule = ((vectorShape.Fill.Mode == FillMode.OddEven) ? WindingRule.EvenOdd : WindingRule.NonZero);
			try
			{
				tess.Tessellate(windingRule, ElementType.Polygons, 3);
			}
			catch (Exception)
			{
				Debug.LogWarning("Shape tessellation failed, skipping...");
				return;
			}
			ushort[] array3 = new ushort[tess.Elements.Length];
			for (int l = 0; l < tess.Elements.Length; l++)
			{
				array3[l] = (ushort)tess.Elements[l];
			}
			Vector2[] array4 = new Vector2[tess.Vertices.Length];
			for (int m = 0; m < tess.Vertices.Length; m++)
			{
				ContourVertex contourVertex = tess.Vertices[m];
				array4[m] = matrix2D2.MultiplyPoint(new Vector2(contourVertex.Position.X, contourVertex.Position.Y));
			}
			bool flag = array3.Length != 0;
			if (flag)
			{
				geoms.Add(new VectorUtils.Geometry
				{
					Vertices = array4,
					Indices = array3,
					Color = color,
					Fill = vectorShape.Fill,
					FillTransform = vectorShape.FillTransform
				});
			}
		}

		internal static Vector2[] GenerateShapeUVs(Vector2[] verts, Rect bounds, Matrix2D uvTransform)
		{
			uvTransform = Matrix2D.Translate(new Vector2(0f, 1f)) * Matrix2D.Scale(new Vector2(1f, -1f)) * uvTransform * Matrix2D.Scale(new Vector2(1f / bounds.width, 1f / bounds.height)) * Matrix2D.Translate(-bounds.position);
			Vector2[] array = new Vector2[verts.Length];
			int num = verts.Length;
			for (int i = 0; i < num; i++)
			{
				array[i] = uvTransform * verts[i];
			}
			return array;
		}

		private static void SwapXY(ref Vector2 v)
		{
			float x = v.x;
			v.x = v.y;
			v.y = x;
		}

		public static VectorUtils.TextureAtlas GenerateAtlasAndFillUVs(IEnumerable<VectorUtils.Geometry> geoms, uint rasterSize)
		{
			VectorUtils.TextureAtlas textureAtlas = VectorUtils.GenerateAtlas(geoms, rasterSize, true, true, true);
			bool flag = textureAtlas != null;
			if (flag)
			{
				VectorUtils.FillUVs(geoms, textureAtlas);
			}
			return textureAtlas;
		}

		private static int NextPOT(int v)
		{
			bool flag = v <= 0;
			int num;
			if (flag)
			{
				num = 0;
			}
			else
			{
				v--;
				v |= v >> 1;
				v |= v >> 2;
				v |= v >> 4;
				v |= v >> 8;
				v |= v >> 16;
				v = (num = v + 1);
			}
			return num;
		}

		public static VectorUtils.TextureAtlas GenerateAtlas(IEnumerable<VectorUtils.Geometry> geoms, uint rasterSize, bool generatePOTTexture = true, bool encodeSettings = true, bool linear = true)
		{
			Dictionary<IFill, VectorUtils.AtlasEntry> dictionary = new Dictionary<IFill, VectorUtils.AtlasEntry>();
			int num = 0;
			foreach (VectorUtils.Geometry geometry in geoms)
			{
				bool flag = geometry.Fill is GradientFill;
				VectorUtils.RawTexture rawTexture;
				if (flag)
				{
					rawTexture = new VectorUtils.RawTexture
					{
						Width = (int)rasterSize,
						Height = 1,
						Rgba = VectorUtils.RasterizeGradientStripe((GradientFill)geometry.Fill, (int)rasterSize)
					};
					num++;
				}
				else
				{
					bool flag2 = geometry.Fill is TextureFill;
					if (!flag2)
					{
						continue;
					}
					Texture2D texture = ((TextureFill)geometry.Fill).Texture;
					rawTexture = new VectorUtils.RawTexture
					{
						Rgba = texture.GetPixels32(),
						Width = texture.width,
						Height = texture.height
					};
					num++;
				}
				dictionary[geometry.Fill] = new VectorUtils.AtlasEntry
				{
					Texture = rawTexture
				};
			}
			bool flag3 = dictionary.Count == 0;
			VectorUtils.TextureAtlas textureAtlas;
			if (flag3)
			{
				textureAtlas = null;
			}
			else
			{
				List<KeyValuePair<IFill, Vector2>> list = new List<KeyValuePair<IFill, Vector2>>(dictionary.Count);
				foreach (KeyValuePair<IFill, VectorUtils.AtlasEntry> keyValuePair in dictionary)
				{
					list.Add(new KeyValuePair<IFill, Vector2>(keyValuePair.Key, new Vector2((float)keyValuePair.Value.Texture.Width, (float)keyValuePair.Value.Texture.Height)));
				}
				list.Add(new KeyValuePair<IFill, Vector2>(null, new Vector2(2f, 2f)));
				Vector2 vector;
				List<VectorUtils.PackRectItem> list2 = VectorUtils.PackRects(list, out vector);
				if (encodeSettings)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						VectorUtils.PackRectItem packRectItem = list2[i];
						packRectItem.Position.x = packRectItem.Position.x + 3f;
						list2[i] = packRectItem;
					}
					vector.x += 3f;
				}
				int num2 = 0;
				foreach (VectorUtils.PackRectItem packRectItem2 in list2)
				{
					num2 = Math.Max(num2, packRectItem2.SettingIndex);
				}
				int num3 = (encodeSettings ? 3 : 0);
				int num4 = (encodeSettings ? (num2 + 1) : num2);
				vector.x = (float)Math.Max(num3, (int)vector.x);
				vector.y = (float)Math.Max(num4, (int)vector.y);
				int num5 = (int)vector.x;
				int num6 = (int)vector.y;
				if (generatePOTTexture)
				{
					num5 = VectorUtils.NextPOT(num5);
					num6 = VectorUtils.NextPOT(num6);
				}
				Color32[] array = new Color32[num5 * num6];
				for (int j = 0; j < num5 * num6; j++)
				{
					array[j] = Color.black;
				}
				Vector2 vector2 = new Vector2(1f / (float)num5, 1f / (float)num6);
				Vector2 position = list2[list2.Count - 1].Position;
				int k = 0;
				VectorUtils.RawTexture rawTexture2 = new VectorUtils.RawTexture
				{
					Rgba = array,
					Width = num5,
					Height = num6
				};
				foreach (VectorUtils.AtlasEntry atlasEntry in dictionary.Values)
				{
					VectorUtils.PackRectItem packRectItem3 = list2[k++];
					atlasEntry.AtlasLocation = packRectItem3;
					VectorUtils.BlitRawTexture(atlasEntry.Texture, rawTexture2, (int)packRectItem3.Position.x, (int)packRectItem3.Position.y, packRectItem3.Rotated);
				}
				VectorUtils.RawTexture rawTexture3 = new VectorUtils.RawTexture
				{
					Width = 2,
					Height = 2,
					Rgba = new Color32[4]
				};
				for (k = 0; k < rawTexture3.Rgba.Length; k++)
				{
					rawTexture3.Rgba[k] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				}
				VectorUtils.BlitRawTexture(rawTexture3, rawTexture2, (int)position.x, (int)position.y, false);
				if (encodeSettings)
				{
					VectorUtils.EncodeSettings(geoms, dictionary, rawTexture2, position);
				}
				Texture2D texture2D = new Texture2D(num5, num6, TextureFormat.ARGB32, false, linear);
				texture2D.wrapModeU = TextureWrapMode.Clamp;
				texture2D.wrapModeV = TextureWrapMode.Clamp;
				texture2D.wrapModeW = TextureWrapMode.Clamp;
				texture2D.SetPixels32(array);
				texture2D.Apply(false, true);
				textureAtlas = new VectorUtils.TextureAtlas
				{
					Texture = texture2D,
					Entries = list2
				};
			}
			return textureAtlas;
		}

		private static void EncodeSettings(IEnumerable<VectorUtils.Geometry> geoms, Dictionary<IFill, VectorUtils.AtlasEntry> fills, VectorUtils.RawTexture rawAtlasTex, Vector2 whiteTexelsScreenPos)
		{
			VectorUtils.WriteRawFloat4Packed(rawAtlasTex, 0f, 0f, 0f, 0f, 0, 0);
			VectorUtils.WriteRawInt2Packed(rawAtlasTex, (int)whiteTexelsScreenPos.x + 1, (int)whiteTexelsScreenPos.y + 1, 1, 0);
			VectorUtils.WriteRawInt2Packed(rawAtlasTex, 0, 0, 2, 0);
			HashSet<int> hashSet = new HashSet<int>();
			hashSet.Add(0);
			foreach (VectorUtils.Geometry geometry in geoms)
			{
				int num = geometry.Vertices.Length;
				VectorUtils.AtlasEntry atlasEntry;
				bool flag = geometry.Fill != null && fills.TryGetValue(geometry.Fill, out atlasEntry);
				if (flag)
				{
					int settingIndex = atlasEntry.AtlasLocation.SettingIndex;
					bool flag2 = hashSet.Contains(settingIndex);
					if (!flag2)
					{
						hashSet.Add(settingIndex);
						int num2 = 0;
						int num3 = settingIndex;
						GradientFill gradientFill = geometry.Fill as GradientFill;
						bool flag3 = gradientFill != null;
						if (flag3)
						{
							Vector2 vector = gradientFill.RadialFocus;
							vector += Vector2.one;
							vector /= 2f;
							vector.y = 1f - vector.y;
							VectorUtils.WriteRawFloat4Packed(rawAtlasTex, (float)gradientFill.Type / 255f, (float)gradientFill.Addressing / 255f, vector.x, vector.y, num2++, num3);
						}
						TextureFill textureFill = geometry.Fill as TextureFill;
						bool flag4 = textureFill != null;
						if (flag4)
						{
							VectorUtils.WriteRawFloat4Packed(rawAtlasTex, 0f, (float)textureFill.Addressing / 255f, 0f, 0f, num2++, num3);
						}
						Vector2 position = atlasEntry.AtlasLocation.Position;
						Vector2 vector2 = new Vector2((float)(atlasEntry.Texture.Width - 1), (float)(atlasEntry.Texture.Height - 1));
						VectorUtils.WriteRawInt2Packed(rawAtlasTex, (int)position.x, (int)position.y, num2++, num3);
						VectorUtils.WriteRawInt2Packed(rawAtlasTex, (int)vector2.x, (int)vector2.y, num2++, num3);
					}
				}
			}
		}

		public static void FillUVs(IEnumerable<VectorUtils.Geometry> geoms, VectorUtils.TextureAtlas texAtlas)
		{
			Dictionary<IFill, VectorUtils.PackRectItem> dictionary = new Dictionary<IFill, VectorUtils.PackRectItem>();
			foreach (VectorUtils.PackRectItem packRectItem in texAtlas.Entries)
			{
				bool flag = packRectItem.Fill != null;
				if (flag)
				{
					dictionary[packRectItem.Fill] = packRectItem;
				}
			}
			VectorUtils.PackRectItem packRectItem2 = default(VectorUtils.PackRectItem);
			foreach (VectorUtils.Geometry geometry in geoms)
			{
				int num = 0;
				bool flag2 = geometry.Fill != null && dictionary.TryGetValue(geometry.Fill, out packRectItem2);
				if (flag2)
				{
					num = packRectItem2.SettingIndex;
				}
				geometry.UVs = VectorUtils.GenerateShapeUVs(geometry.Vertices, geometry.UnclippedBounds, geometry.FillTransform);
				geometry.SettingIndex = num;
			}
		}

		public static Sprite BuildSprite(List<VectorUtils.Geometry> geoms, float svgPixelsPerUnit, VectorUtils.Alignment alignment, Vector2 customPivot, ushort gradientResolution, bool flipYAxis = false)
		{
			return VectorUtils.BuildSprite(geoms, Rect.zero, svgPixelsPerUnit, alignment, customPivot, gradientResolution, flipYAxis);
		}

		public static Sprite BuildSprite(List<VectorUtils.Geometry> geoms, Rect rect, float svgPixelsPerUnit, VectorUtils.Alignment alignment, Vector2 customPivot, ushort gradientResolution, bool flipYAxis = false)
		{
			VectorUtils.TextureAtlas textureAtlas = VectorUtils.GenerateAtlasAndFillUVs(geoms, (uint)gradientResolution);
			List<Vector2> list;
			List<ushort> list2;
			List<Color> list3;
			List<Vector2> list4;
			List<Vector2> list5;
			VectorUtils.FillVertexChannels(geoms, 1f, textureAtlas != null, out list, out list2, out list3, out list4, out list5, flipYAxis);
			Texture2D texture2D = ((textureAtlas != null) ? textureAtlas.Texture : null);
			bool flag = rect == Rect.zero;
			if (flag)
			{
				rect = VectorUtils.Bounds(list);
				VectorUtils.RealignVerticesInBounds(list, rect, flipYAxis);
			}
			else if (flipYAxis)
			{
				VectorUtils.FlipVerticesInBounds(list, rect);
				VectorUtils.ClampVerticesInBounds(list, rect);
			}
			Vector2 pivot = VectorUtils.GetPivot(alignment, customPivot, rect, flipYAxis);
			Sprite sprite = Sprite.Create(rect, pivot, svgPixelsPerUnit, texture2D);
			sprite.OverrideGeometry(list.ToArray(), list2.ToArray());
			bool flag2 = list3 != null;
			if (flag2)
			{
				Color32[] array = new Color32[list3.Count];
				for (int i = 0; i < list3.Count; i++)
				{
					array[i] = list3[i];
				}
				using (NativeArray<Color32> nativeArray = new NativeArray<Color32>(array, Allocator.Temp))
				{
					sprite.SetVertexAttribute<Color32>(VertexAttribute.Color, nativeArray);
				}
			}
			bool flag3 = list4 != null;
			if (flag3)
			{
				using (NativeArray<Vector2> nativeArray2 = new NativeArray<Vector2>(list4.ToArray(), Allocator.Temp))
				{
					sprite.SetVertexAttribute<Vector2>(VertexAttribute.TexCoord0, nativeArray2);
				}
				using (NativeArray<Vector2> nativeArray3 = new NativeArray<Vector2>(list5.ToArray(), Allocator.Temp))
				{
					sprite.SetVertexAttribute<Vector2>(VertexAttribute.TexCoord2, nativeArray3);
				}
			}
			return sprite;
		}

		public static void FillMesh(global::UnityEngine.Mesh mesh, List<VectorUtils.Geometry> geoms, float svgPixelsPerUnit, bool flipYAxis = false)
		{
			bool flag = false;
			foreach (VectorUtils.Geometry geometry in geoms)
			{
				bool flag2 = geometry.UVs != null;
				if (flag2)
				{
					flag = true;
					break;
				}
			}
			List<Vector2> list;
			List<ushort> list2;
			List<Color> list3;
			List<Vector2> list4;
			List<Vector2> list5;
			VectorUtils.FillVertexChannels(geoms, svgPixelsPerUnit, flag, out list, out list2, out list3, out list4, out list5, flipYAxis);
			if (flipYAxis)
			{
				VectorUtils.FlipYAxis(list);
			}
			mesh.Clear();
			Vector3[] array = new Vector3[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				array[i] = list[i];
			}
			int[] array2 = new int[list2.Count];
			for (int j = 0; j < list2.Count; j++)
			{
				array2[j] = (int)list2[j];
			}
			mesh.SetVertices(array);
			mesh.SetTriangles(array2, 0);
			bool flag3 = list3 != null;
			if (flag3)
			{
				mesh.SetColors(list3);
			}
			bool flag4 = list4 != null;
			if (flag4)
			{
				mesh.SetUVs(0, list4);
			}
			bool flag5 = list5 != null;
			if (flag5)
			{
				mesh.SetUVs(2, list5);
			}
		}

		private static void FlipYAxis(IList<Vector2> vertices)
		{
			Rect rect = VectorUtils.Bounds(vertices);
			float height = rect.height;
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 vector = vertices[i];
				vector.y -= rect.position.y;
				vector.y = height - vector.y;
				vector.y += rect.position.y;
				vertices[i] = vector;
			}
		}

		private static void FillVertexChannels(List<VectorUtils.Geometry> geoms, float pixelsPerUnit, bool hasUVs, out List<Vector2> vertices, out List<ushort> indices, out List<Color> colors, out List<Vector2> uvs, out List<Vector2> settingIndices, bool flipYAxis)
		{
			int num = 0;
			int num2 = 0;
			foreach (VectorUtils.Geometry geometry in geoms)
			{
				bool flag = geometry.Indices.Length != 0;
				if (flag)
				{
					num2 += geometry.Indices.Length;
					num += geometry.Vertices.Length;
				}
			}
			vertices = new List<Vector2>(num);
			indices = new List<ushort>(num2);
			colors = new List<Color>(num);
			uvs = (hasUVs ? new List<Vector2>(num) : null);
			settingIndices = (hasUVs ? new List<Vector2>(num) : null);
			foreach (VectorUtils.Geometry geometry2 in geoms)
			{
				int count = indices.Count;
				int num3 = count + geometry2.Indices.Length;
				int count2 = vertices.Count;
				for (int i = 0; i < geometry2.Indices.Length; i++)
				{
					indices.Add((ushort)((int)geometry2.Indices[i] + count2));
				}
				for (int j = 0; j < geometry2.Vertices.Length; j++)
				{
					vertices.Add(geometry2.WorldTransform * geometry2.Vertices[j] / pixelsPerUnit);
				}
				for (int k = 0; k < geometry2.Vertices.Length; k++)
				{
					colors.Add(geometry2.Color);
				}
				VectorUtils.FlipRangeIfNecessary(vertices, indices, count, num3, flipYAxis);
				bool flag2 = uvs != null;
				if (flag2)
				{
					uvs.AddRange(geometry2.UVs);
					for (int l = 0; l < geometry2.UVs.Length; l++)
					{
						settingIndices.Add(new Vector2((float)geometry2.SettingIndex, 0f));
					}
				}
			}
		}

		internal static void AdjustWinding(Vector2[] vertices, ushort[] indices, VectorUtils.WindingDir dir)
		{
			int num = indices.Length;
			for (int i = 0; i < num; i += 3)
			{
				ushort num2 = indices[i];
				ushort num3 = indices[i + 1];
				ushort num4 = indices[i + 2];
				Vector3 vector = vertices[(int)num2];
				Vector3 vector2 = vertices[(int)num3];
				Vector3 vector3 = vertices[(int)num4];
				Vector3 vector4 = vector2 - vector;
				Vector3 vector5 = vector3 - vector;
				float num5 = vector4.x * vector5.y - vector4.y * vector5.x;
				bool flag = ((dir == VectorUtils.WindingDir.CCW) ? (num5 < 0f) : (num5 > 0f));
				bool flag2 = flag;
				if (flag2)
				{
					ushort num6 = indices[i];
					indices[i] = indices[i + 1];
					indices[i + 1] = num6;
				}
			}
		}

		private static void FlipRangeIfNecessary(List<Vector2> vertices, List<ushort> indices, int indexStart, int indexEnd, bool flipYAxis)
		{
			bool flag = false;
			for (int i = indexStart; i < indexEnd - 2; i += 3)
			{
				Vector3 vector = vertices[(int)indices[i]];
				Vector3 vector2 = vertices[(int)indices[i + 1]];
				Vector3 vector3 = vertices[(int)indices[i + 2]];
				Vector3 normalized = (vector2 - vector).normalized;
				Vector3 normalized2 = (vector3 - vector).normalized;
				float num = Vector3.Dot(normalized, normalized2);
				bool flag2 = normalized == Vector3.zero || normalized2 == Vector3.zero || num > 0.99f || num < -0.99f;
				if (!flag2)
				{
					Vector3 vector4 = Vector3.Cross(normalized, normalized2);
					bool flag3 = vector4.sqrMagnitude < 0.001f;
					if (!flag3)
					{
						flag = (flipYAxis ? (vector4.z < 0f) : (vector4.z > 0f));
						break;
					}
				}
			}
			bool flag4 = flag;
			if (flag4)
			{
				for (int j = indexStart; j < indexEnd - 2; j += 3)
				{
					ushort num2 = indices[j + 1];
					indices[j + 1] = indices[j + 2];
					indices[j + 2] = num2;
				}
			}
		}

		internal static void RenderFromArrays(Vector2[] vertices, ushort[] indices, Vector2[] uvs, Color[] colors, Vector2[] settings, Texture2D texture, Material mat, bool clear = true)
		{
			mat.SetTexture("_MainTex", texture);
			mat.SetPass(0);
			if (clear)
			{
				GL.Clear(true, true, Color.clear);
			}
			GL.PushMatrix();
			GL.LoadOrtho();
			GL.Color(new Color(1f, 1f, 1f, 1f));
			GL.Begin(4);
			foreach (ushort num in indices)
			{
				Vector2 vector = vertices[(int)num];
				Vector2 vector2 = uvs[(int)num];
				GL.TexCoord2(vector2.x, vector2.y);
				bool flag = settings != null;
				if (flag)
				{
					Vector2 vector3 = settings[(int)num];
					GL.MultiTexCoord2(2, vector3.x, vector3.y);
				}
				bool flag2 = colors != null;
				if (flag2)
				{
					GL.Color(colors[(int)num]);
				}
				GL.Vertex3(vector.x, vector.y, 0f);
			}
			GL.End();
			GL.PopMatrix();
			mat.SetTexture("_MainTex", null);
		}

		public static void RenderSprite(Sprite sprite, Material mat, bool clear = true)
		{
			float width = sprite.rect.width;
			float height = sprite.rect.height;
			float num = sprite.rect.width / sprite.bounds.size.x;
			Vector2[] uv = sprite.uv;
			ushort[] triangles = sprite.triangles;
			Vector2 pivot = sprite.pivot;
			Vector2[] array = new Vector2[sprite.vertices.Length];
			for (int i = 0; i < sprite.vertices.Length; i++)
			{
				Vector2 vector = sprite.vertices[i];
				array[i] = new Vector2((vector.x * num + pivot.x) / width, (vector.y * num + pivot.y) / height);
			}
			Color[] array2 = null;
			bool flag = sprite.HasVertexAttribute(VertexAttribute.Color);
			if (flag)
			{
				NativeSlice<Color32> vertexAttribute = sprite.GetVertexAttribute<Color32>(VertexAttribute.Color);
				array2 = new Color[vertexAttribute.Length];
				for (int j = 0; j < vertexAttribute.Length; j++)
				{
					array2[j] = vertexAttribute[j];
				}
			}
			Vector2[] array3 = null;
			bool flag2 = sprite.HasVertexAttribute(VertexAttribute.TexCoord2);
			if (flag2)
			{
				array3 = sprite.GetVertexAttribute<Vector2>(VertexAttribute.TexCoord2).ToArray();
			}
			VectorUtils.RenderFromArrays(array, sprite.triangles, sprite.uv, array2, array3, sprite.texture, mat, clear);
		}

		private static Material CreateMaterialForShaderName(string shaderName)
		{
			Shader shader = Shader.Find(shaderName);
			bool flag = shader == null;
			Material material;
			if (flag)
			{
				material = null;
			}
			else
			{
				material = new Material(shader);
			}
			return material;
		}

		public static Texture2D RenderSpriteToTexture2D(Sprite sprite, int width, int height, Material mat, int antiAliasing = 1, bool expandEdges = false)
		{
			bool flag = width <= 0 || height <= 0;
			Texture2D texture2D;
			if (flag)
			{
				texture2D = null;
			}
			else
			{
				RenderTexture active = RenderTexture.active;
				RenderTextureDescriptor renderTextureDescriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.ARGB32, 0)
				{
					msaaSamples = antiAliasing,
					sRGB = (QualitySettings.activeColorSpace == ColorSpace.Linear)
				};
				RenderTexture temporary = RenderTexture.GetTemporary(renderTextureDescriptor);
				RenderTexture.active = temporary;
				GL.Clear(true, true, Color.clear);
				VectorUtils.RenderSprite(sprite, mat, true);
				bool flag2 = VectorUtils.s_DemulMat == null;
				if (flag2)
				{
					VectorUtils.s_DemulMat = VectorUtils.CreateMaterialForShaderName("Hidden/VectorGraphics/VectorDemultiply");
				}
				renderTextureDescriptor.msaaSamples = 1;
				RenderTexture temporary2 = RenderTexture.GetTemporary(renderTextureDescriptor);
				RenderTexture.active = temporary2;
				GL.Clear(true, true, Color.clear);
				Graphics.Blit(temporary, temporary2, VectorUtils.s_DemulMat);
				RenderTexture.ReleaseTemporary(temporary);
				RenderTexture renderTexture = temporary2;
				if (expandEdges)
				{
					bool flag3 = VectorUtils.s_ExpandEdgesMat == null;
					if (flag3)
					{
						VectorUtils.s_ExpandEdgesMat = VectorUtils.CreateMaterialForShaderName("Hidden/VectorGraphics/VectorExpandEdges");
					}
					RenderTexture temporary3 = RenderTexture.GetTemporary(renderTextureDescriptor);
					RenderTexture.active = temporary3;
					GL.Clear(false, true, Color.clear);
					Graphics.Blit(temporary2, temporary3, VectorUtils.s_ExpandEdgesMat);
					bool flag4 = VectorUtils.s_BlendMat == null;
					if (flag4)
					{
						VectorUtils.s_BlendMat = VectorUtils.CreateMaterialForShaderName("Hidden/VectorGraphics/VectorBlendMax");
					}
					Graphics.Blit(temporary2, temporary3, VectorUtils.s_BlendMat);
					RenderTexture.ReleaseTemporary(temporary2);
					renderTexture = temporary3;
				}
				RenderTexture.active = renderTexture;
				Texture2D texture2D2 = new Texture2D(width, height, TextureFormat.RGBA32, false);
				texture2D2.hideFlags = HideFlags.HideAndDontSave;
				texture2D2.ReadPixels(new Rect(0f, 0f, (float)width, (float)height), 0, 0);
				texture2D2.Apply();
				RenderTexture.active = active;
				RenderTexture.ReleaseTemporary(renderTexture);
				texture2D = texture2D2;
			}
			return texture2D;
		}

		internal static Vector2 GetPivot(VectorUtils.Alignment alignment, Vector2 customPivot, Rect bbox, bool flipYAxis)
		{
			Vector2 vector;
			switch (alignment)
			{
			case VectorUtils.Alignment.Center:
				vector = new Vector2(0.5f, 0.5f);
				break;
			case VectorUtils.Alignment.TopLeft:
				vector = new Vector2(0f, 1f);
				break;
			case VectorUtils.Alignment.TopCenter:
				vector = new Vector2(0.5f, 1f);
				break;
			case VectorUtils.Alignment.TopRight:
				vector = new Vector2(1f, 1f);
				break;
			case VectorUtils.Alignment.LeftCenter:
				vector = new Vector2(0f, 0.5f);
				break;
			case VectorUtils.Alignment.RightCenter:
				vector = new Vector2(1f, 0.5f);
				break;
			case VectorUtils.Alignment.BottomLeft:
				vector = new Vector2(0f, 0f);
				break;
			case VectorUtils.Alignment.BottomCenter:
				vector = new Vector2(0.5f, 0f);
				break;
			case VectorUtils.Alignment.BottomRight:
				vector = new Vector2(1f, 0f);
				break;
			case VectorUtils.Alignment.Custom:
				vector = customPivot;
				break;
			case VectorUtils.Alignment.SVGOrigin:
			{
				Vector2 vector2 = -bbox.position / bbox.size;
				if (flipYAxis)
				{
					vector2.y = 1f - vector2.y;
				}
				vector = vector2;
				break;
			}
			default:
				vector = Vector2.zero;
				break;
			}
			return vector;
		}

		public static void TessellatePath(BezierContour contour, PathProperties pathProps, VectorUtils.TessellationOptions tessellateOptions, out Vector2[] vertices, out ushort[] indices)
		{
			bool flag = tessellateOptions.StepDistance < VectorUtils.Epsilon;
			if (flag)
			{
				throw new Exception("stepDistance too small");
			}
			bool flag2 = contour.Segments.Length < 2;
			if (flag2)
			{
				vertices = new Vector2[0];
				indices = new ushort[0];
			}
			else
			{
				tessellateOptions.MaxCordDeviation = Mathf.Max(0.0001f, tessellateOptions.MaxCordDeviation);
				tessellateOptions.MaxTanAngleDeviation = Mathf.Max(0.0001f, tessellateOptions.MaxTanAngleDeviation);
				float[] array = VectorUtils.SegmentsLengths(contour.Segments, contour.Closed, 0.001f);
				float num = 0f;
				foreach (float num2 in array)
				{
					num += num2;
				}
				int num3 = Math.Max((int)(num / tessellateOptions.StepDistance + 0.5f), 2);
				bool flag3 = pathProps.Stroke.Pattern != null;
				if (flag3)
				{
					num3 += pathProps.Stroke.Pattern.Length * 2;
				}
				List<Vector2> list = new List<Vector2>(num3 * 2 + 32);
				List<ushort> list2 = new List<ushort>((int)((float)list.Capacity * 1.5f));
				PathPatternIterator pathPatternIterator = new PathPatternIterator(pathProps.Stroke.Pattern, pathProps.Stroke.PatternOffset);
				PathDistanceForwardIterator pathDistanceForwardIterator = new PathDistanceForwardIterator(contour.Segments, contour.Closed, tessellateOptions.MaxCordDeviationSquared, tessellateOptions.MaxTanAngleDeviationCosine, tessellateOptions.SamplingStepSize);
				VectorUtils.JoiningInfo[] array3 = new VectorUtils.JoiningInfo[2];
				VectorUtils.HandleNewSegmentJoining(pathDistanceForwardIterator, pathPatternIterator, array3, pathProps.Stroke.HalfThickness, array);
				int num4 = 0;
				while (!pathDistanceForwardIterator.Ended)
				{
					bool isSolid = pathPatternIterator.IsSolid;
					if (isSolid)
					{
						VectorUtils.TessellateRange(pathPatternIterator.SegmentLength, pathDistanceForwardIterator, pathPatternIterator, pathProps, tessellateOptions, array3, array, num, num4++, list, list2);
					}
					else
					{
						VectorUtils.SkipRange(pathPatternIterator.SegmentLength, pathDistanceForwardIterator, pathPatternIterator, pathProps, array3, array);
					}
					pathPatternIterator.Advance();
				}
				vertices = list.ToArray();
				indices = list2.ToArray();
			}
		}

		private static Vector2[] TraceShape(BezierContour contour, Stroke stroke, VectorUtils.TessellationOptions tessellateOptions)
		{
			bool flag = tessellateOptions.StepDistance < VectorUtils.Epsilon;
			if (flag)
			{
				throw new Exception("stepDistance too small");
			}
			bool flag2 = contour.Segments.Length < 2;
			Vector2[] array;
			if (flag2)
			{
				array = new Vector2[0];
			}
			else
			{
				float[] array2 = VectorUtils.SegmentsLengths(contour.Segments, contour.Closed, 0.001f);
				float num = 0f;
				foreach (float num2 in array2)
				{
					num += num2;
				}
				int num3 = Math.Max((int)(num / tessellateOptions.StepDistance + 0.5f), 2);
				float[] array4 = ((stroke != null) ? stroke.Pattern : null);
				float num4 = ((stroke != null) ? stroke.PatternOffset : 0f);
				bool flag3 = array4 != null;
				if (flag3)
				{
					num3 += array4.Length * 2;
				}
				List<Vector2> list = new List<Vector2>(num3);
				PathPatternIterator pathPatternIterator = new PathPatternIterator(array4, num4);
				PathDistanceForwardIterator pathDistanceForwardIterator = new PathDistanceForwardIterator(contour.Segments, true, tessellateOptions.MaxCordDeviationSquared, tessellateOptions.MaxTanAngleDeviationCosine, tessellateOptions.SamplingStepSize);
				list.Add(pathDistanceForwardIterator.EvalCurrent());
				while (!pathDistanceForwardIterator.Ended)
				{
					float segmentLength = pathPatternIterator.SegmentLength;
					float lengthSoFar = pathDistanceForwardIterator.LengthSoFar;
					float num5 = Mathf.Min(tessellateOptions.StepDistance, segmentLength);
					bool flag4 = false;
					for (;;)
					{
						PathDistanceForwardIterator.Result result = pathDistanceForwardIterator.AdvanceBy(num5, out num5);
						bool flag5 = result == PathDistanceForwardIterator.Result.Ended;
						if (flag5)
						{
							goto Block_7;
						}
						bool flag6 = result == PathDistanceForwardIterator.Result.NewSegment;
						if (flag6)
						{
							list.Add(pathDistanceForwardIterator.EvalCurrent());
						}
						bool flag7 = num5 <= VectorUtils.Epsilon && !VectorUtils.TryGetMoreRemainingUnits(ref num5, pathDistanceForwardIterator, lengthSoFar, segmentLength, tessellateOptions.StepDistance);
						if (flag7)
						{
							break;
						}
						bool flag8 = result == PathDistanceForwardIterator.Result.Stepped;
						if (flag8)
						{
							list.Add(pathDistanceForwardIterator.EvalCurrent());
						}
					}
					IL_01C3:
					bool flag9 = flag4;
					if (flag9)
					{
						break;
					}
					list.Add(pathDistanceForwardIterator.EvalCurrent());
					pathPatternIterator.Advance();
					continue;
					Block_7:
					flag4 = true;
					goto IL_01C3;
				}
				bool flag10 = (list[0] - list[list.Count - 1]).sqrMagnitude < VectorUtils.Epsilon;
				if (flag10)
				{
					list.RemoveAt(list.Count - 1);
				}
				array = list.ToArray();
			}
			return array;
		}

		private static bool TryGetMoreRemainingUnits(ref float unitsRemaining, PathDistanceForwardIterator pathIt, float startingLength, float distance, float stepDistance)
		{
			float num = pathIt.LengthSoFar - startingLength;
			float num2 = Math.Max(VectorUtils.Epsilon, distance * VectorUtils.Epsilon * 100f);
			bool flag = distance - num <= num2;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = num + stepDistance > distance;
				if (flag3)
				{
					unitsRemaining = distance - num;
				}
				else
				{
					unitsRemaining = stepDistance;
				}
				flag2 = true;
			}
			return flag2;
		}

		private static void HandleNewSegmentJoining(PathDistanceForwardIterator pathIt, PathPatternIterator patternIt, VectorUtils.JoiningInfo[] joiningInfo, float halfThickness, float[] segmentLengths)
		{
			joiningInfo[0] = joiningInfo[1];
			joiningInfo[1] = null;
			bool flag = !patternIt.IsSolidAt(pathIt.LengthSoFar + segmentLengths[pathIt.CurrentSegment]);
			if (!flag)
			{
				bool flag2 = pathIt.Closed && pathIt.Segments.Count <= 2;
				if (!flag2)
				{
					bool closed = pathIt.Closed;
					if (closed)
					{
						bool flag3 = pathIt.CurrentSegment == 0 || pathIt.CurrentSegment == pathIt.Segments.Count - 2;
						if (flag3)
						{
							VectorUtils.JoiningInfo joiningInfo2 = VectorUtils.ForeseeJoining(VectorUtils.PathSegmentAtIndex(pathIt.Segments, pathIt.Segments.Count - 2), VectorUtils.PathSegmentAtIndex(pathIt.Segments, 0), halfThickness, segmentLengths[pathIt.Segments.Count - 2]);
							bool flag4 = pathIt.CurrentSegment == 0;
							if (!flag4)
							{
								joiningInfo[1] = joiningInfo2;
								return;
							}
							joiningInfo[0] = joiningInfo2;
						}
						else
						{
							bool flag5 = pathIt.CurrentSegment > pathIt.Segments.Count - 2;
							if (flag5)
							{
								return;
							}
						}
					}
					else
					{
						bool flag6 = pathIt.CurrentSegment >= pathIt.Segments.Count - 2;
						if (flag6)
						{
							return;
						}
					}
					joiningInfo[1] = VectorUtils.ForeseeJoining(VectorUtils.PathSegmentAtIndex(pathIt.Segments, pathIt.CurrentSegment), VectorUtils.PathSegmentAtIndex(pathIt.Segments, pathIt.CurrentSegment + 1), halfThickness, segmentLengths[pathIt.CurrentSegment]);
				}
			}
		}

		private static void SkipRange(float distance, PathDistanceForwardIterator pathIt, PathPatternIterator patternIt, PathProperties pathProps, VectorUtils.JoiningInfo[] joiningInfo, float[] segmentLengths)
		{
			float num = distance;
			while (num > VectorUtils.Epsilon)
			{
				switch (pathIt.AdvanceBy(num, out num))
				{
				case PathDistanceForwardIterator.Result.Stepped:
				{
					bool flag = num < VectorUtils.Epsilon;
					if (flag)
					{
						return;
					}
					break;
				}
				case PathDistanceForwardIterator.Result.NewSegment:
					VectorUtils.HandleNewSegmentJoining(pathIt, patternIt, joiningInfo, pathProps.Stroke.HalfThickness, segmentLengths);
					break;
				case PathDistanceForwardIterator.Result.Ended:
					return;
				}
			}
		}

		private static void TessellateRange(float distance, PathDistanceForwardIterator pathIt, PathPatternIterator patternIt, PathProperties pathProps, VectorUtils.TessellationOptions tessellateOptions, VectorUtils.JoiningInfo[] joiningInfo, float[] segmentLengths, float totalLength, int rangeIndex, List<Vector2> verts, List<ushort> inds)
		{
			bool flag = pathIt.Closed && pathIt.CurrentSegment == 0 && pathIt.CurrentT == 0f;
			bool flag2 = flag && joiningInfo[0] != null;
			if (flag2)
			{
				VectorUtils.GenerateJoining(joiningInfo[0], pathProps.Corners, pathProps.Stroke.HalfThickness, pathProps.Stroke.TippedCornerLimit, tessellateOptions, verts, inds);
			}
			else
			{
				PathEnding pathEnding = pathProps.Head;
				bool flag3 = pathIt.Closed && rangeIndex == 0 && patternIt.IsSolidAt(pathIt.CurrentT) && patternIt.IsSolidAt(totalLength);
				if (flag3)
				{
					pathEnding = PathEnding.Chop;
				}
				VectorUtils.GenerateTip(VectorUtils.PathSegmentAtIndex(pathIt.Segments, pathIt.CurrentSegment), true, pathIt.CurrentT, pathEnding, pathProps.Stroke.HalfThickness, tessellateOptions, verts, inds);
			}
			float lengthSoFar = pathIt.LengthSoFar;
			float num = Mathf.Min(tessellateOptions.StepDistance, distance);
			bool flag4 = false;
			for (;;)
			{
				PathDistanceForwardIterator.Result result = pathIt.AdvanceBy(num, out num);
				bool flag5 = result == PathDistanceForwardIterator.Result.Ended;
				if (flag5)
				{
					break;
				}
				bool flag6 = result == PathDistanceForwardIterator.Result.NewSegment;
				if (flag6)
				{
					bool flag7 = joiningInfo[1] != null;
					if (flag7)
					{
						VectorUtils.GenerateJoining(joiningInfo[1], pathProps.Corners, pathProps.Stroke.HalfThickness, pathProps.Stroke.TippedCornerLimit, tessellateOptions, verts, inds);
					}
					else
					{
						VectorUtils.AddSegment(VectorUtils.PathSegmentAtIndex(pathIt.Segments, pathIt.CurrentSegment), pathIt.CurrentT, pathProps.Stroke.HalfThickness, null, pathIt.SegmentLengthSoFar, verts, inds);
					}
					VectorUtils.HandleNewSegmentJoining(pathIt, patternIt, joiningInfo, pathProps.Stroke.HalfThickness, segmentLengths);
				}
				bool flag8 = num <= VectorUtils.Epsilon && !VectorUtils.TryGetMoreRemainingUnits(ref num, pathIt, lengthSoFar, distance, tessellateOptions.StepDistance);
				if (flag8)
				{
					goto Block_13;
				}
				bool flag9 = result == PathDistanceForwardIterator.Result.Stepped;
				if (flag9)
				{
					VectorUtils.AddSegment(VectorUtils.PathSegmentAtIndex(pathIt.Segments, pathIt.CurrentSegment), pathIt.CurrentT, pathProps.Stroke.HalfThickness, joiningInfo, pathIt.SegmentLengthSoFar, verts, inds);
				}
			}
			flag4 = true;
			Block_13:
			bool flag10 = flag4 && pathIt.Closed;
			if (flag10)
			{
				inds.Add(0);
				inds.Add(1);
				inds.Add((ushort)(verts.Count - 2));
				inds.Add((ushort)(verts.Count - 1));
				inds.Add((ushort)(verts.Count - 2));
				inds.Add(1);
			}
			else
			{
				VectorUtils.AddSegment(VectorUtils.PathSegmentAtIndex(pathIt.Segments, pathIt.CurrentSegment), pathIt.CurrentT, pathProps.Stroke.HalfThickness, joiningInfo, pathIt.SegmentLengthSoFar, verts, inds);
				VectorUtils.GenerateTip(VectorUtils.PathSegmentAtIndex(pathIt.Segments, pathIt.CurrentSegment), false, pathIt.CurrentT, pathProps.Tail, pathProps.Stroke.HalfThickness, tessellateOptions, verts, inds);
			}
		}

		private static void AddSegment(BezierSegment segment, float toT, float halfThickness, VectorUtils.JoiningInfo[] joinInfo, float segmentLengthSoFar, List<Vector2> verts, List<ushort> inds)
		{
			Vector2 vector2;
			Vector2 vector3;
			Vector2 vector = VectorUtils.EvalFull(segment, toT, out vector2, out vector3);
			Vector2 vector4 = vector + vector3 * halfThickness;
			Vector2 vector5 = vector + vector3 * -halfThickness;
			bool flag = joinInfo != null;
			if (flag)
			{
				bool flag2 = joinInfo[0] != null && segmentLengthSoFar < joinInfo[0].InnerCornerDistFromStart;
				if (flag2)
				{
					bool roundPosThickness = joinInfo[0].RoundPosThickness;
					if (roundPosThickness)
					{
						vector5 = joinInfo[0].InnerCornerVertex;
					}
					else
					{
						vector4 = joinInfo[0].InnerCornerVertex;
					}
				}
				bool flag3 = joinInfo[1] != null && segmentLengthSoFar > joinInfo[1].InnerCornerDistToEnd;
				if (flag3)
				{
					bool roundPosThickness2 = joinInfo[1].RoundPosThickness;
					if (roundPosThickness2)
					{
						vector5 = joinInfo[1].InnerCornerVertex;
					}
					else
					{
						vector4 = joinInfo[1].InnerCornerVertex;
					}
				}
			}
			int num = verts.Count - 2;
			verts.Add(vector4);
			verts.Add(vector5);
			inds.Add((ushort)num);
			inds.Add((ushort)(num + 3));
			inds.Add((ushort)(num + 1));
			inds.Add((ushort)num);
			inds.Add((ushort)(num + 2));
			inds.Add((ushort)(num + 3));
		}

		private static VectorUtils.JoiningInfo ForeseeJoining(BezierSegment end, BezierSegment start, float halfThickness, float endSegmentLength)
		{
			VectorUtils.JoiningInfo joiningInfo = new VectorUtils.JoiningInfo();
			joiningInfo.JoinPos = end.P3;
			joiningInfo.TanAtEnd = VectorUtils.EvalTangent(end, 1f);
			joiningInfo.NormAtEnd = Vector2.Perpendicular(joiningInfo.TanAtEnd);
			joiningInfo.TanAtStart = VectorUtils.EvalTangent(start, 0f);
			joiningInfo.NormAtStart = Vector2.Perpendicular(joiningInfo.TanAtStart);
			float num = Vector2.Dot(joiningInfo.TanAtEnd, joiningInfo.TanAtStart);
			joiningInfo.SimpleJoin = Mathf.Approximately(Mathf.Abs(num), 1f);
			bool simpleJoin = joiningInfo.SimpleJoin;
			VectorUtils.JoiningInfo joiningInfo2;
			if (simpleJoin)
			{
				joiningInfo2 = null;
			}
			else
			{
				joiningInfo.PosThicknessEnd = joiningInfo.JoinPos + joiningInfo.NormAtEnd * halfThickness;
				joiningInfo.NegThicknessEnd = joiningInfo.JoinPos - joiningInfo.NormAtEnd * halfThickness;
				joiningInfo.PosThicknessStart = joiningInfo.JoinPos + joiningInfo.NormAtStart * halfThickness;
				joiningInfo.NegThicknessStart = joiningInfo.JoinPos - joiningInfo.NormAtStart * halfThickness;
				bool simpleJoin2 = joiningInfo.SimpleJoin;
				if (simpleJoin2)
				{
					joiningInfo.PosThicknessClosingPoint = Vector2.LerpUnclamped(joiningInfo.PosThicknessEnd, joiningInfo.PosThicknessStart, 0.5f);
					joiningInfo.NegThicknessClosingPoint = Vector2.LerpUnclamped(joiningInfo.NegThicknessEnd, joiningInfo.NegThicknessStart, 0.5f);
				}
				else
				{
					joiningInfo.PosThicknessClosingPoint = VectorUtils.IntersectLines(joiningInfo.PosThicknessEnd, joiningInfo.PosThicknessEnd + joiningInfo.TanAtEnd, joiningInfo.PosThicknessStart, joiningInfo.PosThicknessStart + joiningInfo.TanAtStart);
					joiningInfo.NegThicknessClosingPoint = VectorUtils.IntersectLines(joiningInfo.NegThicknessEnd, joiningInfo.NegThicknessEnd + joiningInfo.TanAtEnd, joiningInfo.NegThicknessStart, joiningInfo.NegThicknessStart + joiningInfo.TanAtStart);
					bool flag = float.IsInfinity(joiningInfo.PosThicknessClosingPoint.x) || float.IsInfinity(joiningInfo.PosThicknessClosingPoint.y);
					if (flag)
					{
						joiningInfo.PosThicknessClosingPoint = joiningInfo.JoinPos;
					}
					bool flag2 = float.IsInfinity(joiningInfo.NegThicknessClosingPoint.x) || float.IsInfinity(joiningInfo.NegThicknessClosingPoint.y);
					if (flag2)
					{
						joiningInfo.NegThicknessClosingPoint = joiningInfo.JoinPos;
					}
				}
				joiningInfo.RoundPosThickness = VectorUtils.PointOnTheLeftOfLine(Vector2.zero, joiningInfo.TanAtEnd, joiningInfo.TanAtStart);
				Vector2[] array = null;
				Vector2[] array2 = null;
				Vector2 zero = Vector2.zero;
				Vector2 zero2 = Vector2.zero;
				bool flag3 = !joiningInfo.SimpleJoin;
				if (flag3)
				{
					BezierSegment bezierSegment = VectorUtils.FlipSegment(end);
					Vector2 vector = (joiningInfo.RoundPosThickness ? joiningInfo.PosThicknessClosingPoint : joiningInfo.NegThicknessClosingPoint);
					Vector2 p = end.P3;
					Vector2 vector2 = p + (vector - p) * 10f;
					array = VectorUtils.LineBezierThicknessIntersect(start, joiningInfo.RoundPosThickness ? (-halfThickness) : halfThickness, p, vector2, out joiningInfo.InnerCornerDistFromStart, out zero);
					array2 = VectorUtils.LineBezierThicknessIntersect(bezierSegment, joiningInfo.RoundPosThickness ? halfThickness : (-halfThickness), p, vector2, out joiningInfo.InnerCornerDistToEnd, out zero2);
				}
				bool flag4 = false;
				bool flag5 = array != null && array2 != null;
				if (flag5)
				{
					Vector2 vector3 = VectorUtils.IntersectLines(array[0], array[1], array2[0], array2[1]);
					bool flag6 = VectorUtils.PointOnLineIsWithinSegment(array[0], array[1], vector3);
					bool flag7 = VectorUtils.PointOnLineIsWithinSegment(array2[0], array2[1], vector3);
					bool flag8 = !float.IsInfinity(vector3.x) && flag6 && flag7;
					if (flag8)
					{
						Vector2 vector4 = zero - vector3;
						Vector2 vector5 = zero2 - vector3;
						joiningInfo.InnerCornerDistFromStart += ((vector4 == Vector2.zero) ? 0f : vector4.magnitude);
						joiningInfo.InnerCornerDistToEnd += ((vector5 == Vector2.zero) ? 0f : vector5.magnitude);
						joiningInfo.InnerCornerDistToEnd = endSegmentLength - joiningInfo.InnerCornerDistToEnd;
						joiningInfo.InnerCornerVertex = vector3;
						flag4 = true;
					}
				}
				bool flag9 = !flag4;
				if (flag9)
				{
					joiningInfo.InnerCornerVertex = joiningInfo.JoinPos + ((joiningInfo.TanAtStart - joiningInfo.TanAtEnd) / 2f).normalized * halfThickness;
					joiningInfo.InnerCornerDistFromStart = 0f;
					joiningInfo.InnerCornerDistToEnd = endSegmentLength;
				}
				joiningInfo2 = joiningInfo;
			}
			return joiningInfo2;
		}

		private static Vector2[] LineBezierThicknessIntersect(BezierSegment seg, float thickness, Vector2 lineFrom, Vector2 lineTo, out float distanceToIntersection, out Vector2 intersection)
		{
			Vector2 vector = VectorUtils.EvalTangent(seg, 0f);
			Vector2 vector2 = Vector2.Perpendicular(vector);
			Vector2 vector3 = seg.P0 + vector2 * thickness;
			distanceToIntersection = 0f;
			intersection = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
			float num = 0.01f;
			float num2 = 0f;
			while (num2 < 1f)
			{
				num2 += num;
				Vector2 vector4 = VectorUtils.EvalFull(seg, num2, out vector, out vector2) + vector2 * thickness;
				intersection = VectorUtils.IntersectLines(lineFrom, lineTo, vector3, vector4);
				bool flag = VectorUtils.PointOnLineIsWithinSegment(vector3, vector4, intersection);
				if (flag)
				{
					distanceToIntersection += (vector3 - intersection).magnitude;
					return new Vector2[] { vector3, vector4 };
				}
				distanceToIntersection += (vector3 - vector4).magnitude;
				vector3 = vector4;
			}
			return null;
		}

		private static bool PointOnLineIsWithinSegment(Vector2 lineFrom, Vector2 lineTo, Vector2 point)
		{
			Vector2 normalized = (lineTo - lineFrom).normalized;
			bool flag = Vector2.Dot(point - lineFrom, normalized) < -VectorUtils.Epsilon;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = Vector2.Dot(point - lineTo, normalized) > VectorUtils.Epsilon;
				flag2 = !flag3;
			}
			return flag2;
		}

		private static void GenerateJoining(VectorUtils.JoiningInfo joinInfo, PathCorner corner, float halfThickness, float tippedCornerLimit, VectorUtils.TessellationOptions tessellateOptions, List<Vector2> verts, List<ushort> inds)
		{
			bool flag = verts.Count == 0;
			if (flag)
			{
				verts.Add(joinInfo.RoundPosThickness ? joinInfo.PosThicknessEnd : joinInfo.InnerCornerVertex);
				verts.Add(joinInfo.RoundPosThickness ? joinInfo.InnerCornerVertex : joinInfo.NegThicknessEnd);
			}
			int num = verts.Count - 2;
			bool flag2 = corner == PathCorner.Tipped && tippedCornerLimit >= 1f;
			if (flag2)
			{
				float num2 = Vector2.Angle(-joinInfo.TanAtEnd, joinInfo.TanAtStart) * 0.017453292f;
				float num3 = 1f / Mathf.Sin(num2 / 2f);
				bool flag3 = num3 > tippedCornerLimit;
				if (flag3)
				{
					corner = PathCorner.Beveled;
				}
			}
			bool simpleJoin = joinInfo.SimpleJoin;
			if (!simpleJoin)
			{
				bool flag4 = corner == PathCorner.Tipped;
				if (flag4)
				{
					verts.Add(joinInfo.PosThicknessClosingPoint);
					verts.Add(joinInfo.NegThicknessClosingPoint);
					verts.Add(joinInfo.RoundPosThickness ? joinInfo.PosThicknessStart : joinInfo.InnerCornerVertex);
					verts.Add(joinInfo.RoundPosThickness ? joinInfo.InnerCornerVertex : joinInfo.NegThicknessStart);
					inds.Add((ushort)num);
					inds.Add((ushort)(num + 3));
					inds.Add((ushort)(num + 1));
					inds.Add((ushort)num);
					inds.Add((ushort)(num + 2));
					inds.Add((ushort)(num + 3));
					inds.Add((ushort)(num + 4));
					inds.Add((ushort)(num + 3));
					inds.Add((ushort)(num + 2));
					inds.Add((ushort)(num + 4));
					inds.Add((ushort)(num + 5));
					inds.Add((ushort)(num + 3));
					return;
				}
				bool flag5 = corner == PathCorner.Beveled;
				if (flag5)
				{
					verts.Add(joinInfo.RoundPosThickness ? joinInfo.PosThicknessEnd : joinInfo.InnerCornerVertex);
					verts.Add(joinInfo.RoundPosThickness ? joinInfo.InnerCornerVertex : joinInfo.NegThicknessEnd);
					verts.Add(joinInfo.RoundPosThickness ? joinInfo.PosThicknessStart : joinInfo.InnerCornerVertex);
					verts.Add(joinInfo.RoundPosThickness ? joinInfo.InnerCornerVertex : joinInfo.NegThicknessStart);
					inds.Add((ushort)num);
					inds.Add((ushort)(num + 2));
					inds.Add((ushort)(num + 1));
					inds.Add((ushort)(num + 1));
					inds.Add((ushort)(num + 2));
					inds.Add((ushort)(num + 3));
					bool roundPosThickness = joinInfo.RoundPosThickness;
					if (roundPosThickness)
					{
						inds.Add((ushort)(num + 2));
						inds.Add((ushort)(num + 4));
						inds.Add((ushort)(num + 3));
					}
					else
					{
						inds.Add((ushort)(num + 3));
						inds.Add((ushort)(num + 2));
						inds.Add((ushort)(num + 5));
					}
					return;
				}
			}
			bool flag6 = corner == PathCorner.Round;
			if (flag6)
			{
				float num4 = Mathf.Acos(Vector2.Dot(joinInfo.NormAtEnd, joinInfo.NormAtStart));
				bool flag7 = false;
				bool flag8 = !VectorUtils.PointOnTheLeftOfLine(Vector2.zero, joinInfo.NormAtEnd, joinInfo.NormAtStart);
				if (flag8)
				{
					num4 = -num4;
					flag7 = true;
				}
				ushort num5 = (ushort)verts.Count;
				verts.Add(joinInfo.InnerCornerVertex);
				int num6 = VectorUtils.CalculateArcSteps(halfThickness, 0f, num4, tessellateOptions);
				for (int i = 0; i <= num6; i++)
				{
					float num7 = num4 * ((float)i / (float)num6);
					Vector2 vector = Matrix2D.RotateLH(num7) * joinInfo.NormAtEnd;
					bool flag9 = flag7;
					if (flag9)
					{
						vector = -vector;
					}
					verts.Add(vector * halfThickness + joinInfo.JoinPos);
					bool flag10 = i == 0;
					if (flag10)
					{
						inds.Add((ushort)num);
						inds.Add((ushort)(num + 3));
						inds.Add((ushort)(num + (joinInfo.RoundPosThickness ? 2 : 1)));
						inds.Add((ushort)num);
						inds.Add((ushort)(num + 2));
						inds.Add((ushort)(num + (joinInfo.RoundPosThickness ? 1 : 3)));
					}
					else
					{
						bool roundPosThickness2 = joinInfo.RoundPosThickness;
						if (roundPosThickness2)
						{
							inds.Add((ushort)(num + i + (flag7 ? 3 : 2)));
							inds.Add((ushort)(num + i + (flag7 ? 2 : 3)));
							inds.Add(num5);
						}
						else
						{
							inds.Add((ushort)(num + i + (flag7 ? 3 : 2)));
							inds.Add((ushort)(num + i + (flag7 ? 2 : 3)));
							inds.Add(num5);
						}
					}
				}
				int count = verts.Count;
				bool roundPosThickness3 = joinInfo.RoundPosThickness;
				if (roundPosThickness3)
				{
					verts.Add(joinInfo.PosThicknessStart);
					verts.Add(joinInfo.InnerCornerVertex);
				}
				else
				{
					verts.Add(joinInfo.InnerCornerVertex);
					verts.Add(joinInfo.NegThicknessStart);
				}
				inds.Add((ushort)(count - 1));
				inds.Add((ushort)count);
				inds.Add(num5);
			}
		}

		private static void GenerateTip(BezierSegment segment, bool atStart, float t, PathEnding ending, float halfThickness, VectorUtils.TessellationOptions tessellateOptions, List<Vector2> verts, List<ushort> inds)
		{
			Vector2 vector2;
			Vector2 vector3;
			Vector2 vector = VectorUtils.EvalFull(segment, t, out vector2, out vector3);
			int count = verts.Count;
			switch (ending)
			{
			case PathEnding.Chop:
				if (atStart)
				{
					verts.Add(vector + vector3 * halfThickness);
					verts.Add(vector - vector3 * halfThickness);
				}
				break;
			case PathEnding.Square:
				if (atStart)
				{
					verts.Add(vector + vector3 * halfThickness - vector2 * halfThickness);
					verts.Add(vector - vector3 * halfThickness - vector2 * halfThickness);
					verts.Add(vector + vector3 * halfThickness);
					verts.Add(vector - vector3 * halfThickness);
					inds.Add((ushort)count);
					inds.Add((ushort)(count + 3));
					inds.Add((ushort)(count + 1));
					inds.Add((ushort)count);
					inds.Add((ushort)(count + 2));
					inds.Add((ushort)(count + 3));
				}
				else
				{
					verts.Add(vector + vector3 * halfThickness + vector2 * halfThickness);
					verts.Add(vector - vector3 * halfThickness + vector2 * halfThickness);
					inds.Add((ushort)(count - 2));
					inds.Add((ushort)(count + 3 - 2));
					inds.Add((ushort)(count + 1 - 2));
					inds.Add((ushort)(count - 2));
					inds.Add((ushort)(count + 2 - 2));
					inds.Add((ushort)(count + 3 - 2));
				}
				break;
			case PathEnding.Round:
			{
				float num = (float)(atStart ? (-1) : 1);
				int num2 = VectorUtils.CalculateArcSteps(halfThickness, 0f, 3.1415927f, tessellateOptions);
				for (int i = 1; i < num2; i++)
				{
					float num3 = 3.1415927f * ((float)i / (float)num2);
					verts.Add(vector + Matrix2D.RotateLH(num3) * vector3 * halfThickness * num);
				}
				if (atStart)
				{
					int count2 = verts.Count;
					verts.Add(vector + vector3 * halfThickness);
					verts.Add(vector - vector3 * halfThickness);
					for (int j = 1; j < num2; j++)
					{
						inds.Add((ushort)(count2 + 1));
						inds.Add((ushort)(count + j - 1));
						inds.Add((ushort)(count + j));
					}
				}
				else
				{
					inds.Add((ushort)(count - 1));
					inds.Add((ushort)(count - 2));
					inds.Add((ushort)count);
					for (int k = 1; k < num2 - 1; k++)
					{
						inds.Add((ushort)(count - 1));
						inds.Add((ushort)(count + k - 1));
						inds.Add((ushort)(count + k));
					}
				}
				break;
			}
			}
		}

		private static int CalculateArcSteps(float radius, float fromAngle, float toAngle, VectorUtils.TessellationOptions tessellateOptions)
		{
			float num = float.MaxValue;
			bool flag = tessellateOptions.StepDistance != float.MaxValue;
			if (flag)
			{
				num = tessellateOptions.StepDistance / radius;
			}
			bool flag2 = tessellateOptions.MaxCordDeviation != float.MaxValue;
			if (flag2)
			{
				float num2 = radius - tessellateOptions.MaxCordDeviation;
				float num3 = Mathf.Sqrt(radius * radius - num2 * num2);
				float num4 = Mathf.Min(num, Mathf.Asin(num3 / radius));
				bool flag3 = num4 > VectorUtils.Epsilon;
				if (flag3)
				{
					num = num4;
				}
			}
			bool flag4 = tessellateOptions.MaxTanAngleDeviation < 1.5707964f;
			if (flag4)
			{
				num = Mathf.Min(num, tessellateOptions.MaxTanAngleDeviation * 2f);
			}
			float num5 = 6.2831855f / num;
			float num6 = Mathf.Abs(fromAngle - toAngle) / 6.2831855f;
			return (int)Mathf.Max(num5 * num6 + 0.5f, 3f);
		}

		public static void TessellateRect(Rect rect, out Vector2[] vertices, out ushort[] indices)
		{
			vertices = new Vector2[]
			{
				new Vector2(rect.xMin, rect.yMin),
				new Vector2(rect.xMax, rect.yMin),
				new Vector2(rect.xMax, rect.yMax),
				new Vector2(rect.xMin, rect.yMax)
			};
			indices = new ushort[] { 1, 0, 2, 2, 0, 3 };
		}

		public static void TessellateRectBorder(Rect rect, float halfThickness, out Vector2[] vertices, out ushort[] indices)
		{
			List<Vector2> list = new List<Vector2>(16);
			List<ushort> list2 = new List<ushort>(24);
			Vector2 vector = new Vector2(rect.x, rect.y + rect.height);
			Vector2 vector2 = new Vector2(rect.x, rect.y);
			Vector2 vector3 = vector + new Vector2(-halfThickness, halfThickness);
			Vector2 vector4 = vector2 + new Vector2(-halfThickness, -halfThickness);
			Vector2 vector5 = vector2 + new Vector2(halfThickness, halfThickness);
			Vector2 vector6 = vector + new Vector2(halfThickness, -halfThickness);
			list.Add(vector3);
			list.Add(vector4);
			list.Add(vector5);
			list.Add(vector6);
			list2.Add(0);
			list2.Add(3);
			list2.Add(2);
			list2.Add(2);
			list2.Add(1);
			list2.Add(0);
			vector = new Vector2(rect.x, rect.y);
			vector2 = new Vector2(rect.x + rect.width, rect.y);
			vector3 = vector + new Vector2(-halfThickness, -halfThickness);
			vector4 = vector2 + new Vector2(halfThickness, -halfThickness);
			vector5 = vector2 + new Vector2(-halfThickness, halfThickness);
			vector6 = vector + new Vector2(halfThickness, halfThickness);
			list.Add(vector3);
			list.Add(vector4);
			list.Add(vector5);
			list.Add(vector6);
			list2.Add(4);
			list2.Add(7);
			list2.Add(6);
			list2.Add(6);
			list2.Add(5);
			list2.Add(4);
			vector = new Vector2(rect.x + rect.width, rect.y);
			vector2 = new Vector2(rect.x + rect.width, rect.y + rect.height);
			vector3 = vector + new Vector2(halfThickness, -halfThickness);
			vector4 = vector2 + new Vector2(halfThickness, halfThickness);
			vector5 = vector2 + new Vector2(-halfThickness, -halfThickness);
			vector6 = vector + new Vector2(-halfThickness, halfThickness);
			list.Add(vector3);
			list.Add(vector4);
			list.Add(vector5);
			list.Add(vector6);
			list2.Add(8);
			list2.Add(11);
			list2.Add(10);
			list2.Add(10);
			list2.Add(9);
			list2.Add(8);
			vector = new Vector2(rect.x + rect.width, rect.y + rect.height);
			vector2 = new Vector2(rect.x, rect.y + rect.height);
			vector3 = vector + new Vector2(halfThickness, halfThickness);
			vector4 = vector2 + new Vector2(-halfThickness, halfThickness);
			vector5 = vector2 + new Vector2(halfThickness, -halfThickness);
			vector6 = vector + new Vector2(-halfThickness, -halfThickness);
			list.Add(vector3);
			list.Add(vector4);
			list.Add(vector5);
			list.Add(vector6);
			list2.Add(12);
			list2.Add(15);
			list2.Add(14);
			list2.Add(14);
			list2.Add(13);
			list2.Add(12);
			vertices = list.ToArray();
			indices = list2.ToArray();
		}

		public static BezierPathSegment[] BezierSegmentToPath(BezierSegment segment)
		{
			return new BezierPathSegment[]
			{
				new BezierPathSegment
				{
					P0 = segment.P0,
					P1 = segment.P1,
					P2 = segment.P2
				},
				new BezierPathSegment
				{
					P0 = segment.P3
				}
			};
		}

		public static BezierPathSegment[] BezierSegmentsToPath(BezierSegment[] segments)
		{
			bool flag = segments.Length == 0;
			BezierPathSegment[] array;
			if (flag)
			{
				array = new BezierPathSegment[0];
			}
			else
			{
				int num = segments.Length;
				List<BezierPathSegment> list = new List<BezierPathSegment>(segments.Length * 2 + 1);
				for (int i = 0; i < num; i++)
				{
					BezierSegment bezierSegment = segments[i];
					list.Add(new BezierPathSegment
					{
						P0 = bezierSegment.P0,
						P1 = bezierSegment.P1,
						P2 = bezierSegment.P2
					});
					bool flag2 = i == num - 1;
					if (flag2)
					{
						list.Add(new BezierPathSegment
						{
							P0 = bezierSegment.P3
						});
					}
					else
					{
						BezierSegment bezierSegment2 = segments[i + 1];
						bool flag3 = bezierSegment.P3 != bezierSegment2.P0;
						if (flag3)
						{
							BezierSegment bezierSegment3 = VectorUtils.MakeLine(bezierSegment.P3, bezierSegment2.P0);
							list.Add(new BezierPathSegment
							{
								P0 = bezierSegment3.P0,
								P1 = bezierSegment3.P1,
								P2 = bezierSegment3.P2
							});
						}
					}
				}
				array = list.ToArray();
			}
			return array;
		}

		public static BezierSegment PathSegmentAtIndex(IList<BezierPathSegment> path, int index)
		{
			bool flag = index < 0 || index >= path.Count - 1;
			if (flag)
			{
				throw new IndexOutOfRangeException("Invalid index passed to PathSegmentAtIndex");
			}
			return new BezierSegment
			{
				P0 = path[index].P0,
				P1 = path[index].P1,
				P2 = path[index].P2,
				P3 = path[index + 1].P0
			};
		}

		public static bool PathEndsPerfectlyMatch(IList<BezierPathSegment> path)
		{
			bool flag = path.Count < 2;
			bool flag2;
			if (flag)
			{
				flag2 = false;
			}
			else
			{
				bool flag3 = (path[0].P0 - path[path.Count - 1].P0).sqrMagnitude > VectorUtils.Epsilon;
				flag2 = !flag3;
			}
			return flag2;
		}

		public static void MakeRectangleShape(Shape rectShape, Rect rect)
		{
			VectorUtils.MakeRectangleShape(rectShape, rect, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero);
		}

		public static void MakeRectangleShape(Shape rectShape, Rect rect, Vector2 radiusTL, Vector2 radiusTR, Vector2 radiusBR, Vector2 radiusBL)
		{
			BezierContour bezierContour = VectorUtils.BuildRectangleContour(rect, radiusTL, radiusTR, radiusBR, radiusBL);
			bool flag = rectShape.Contours == null || rectShape.Contours.Length != 1;
			if (flag)
			{
				rectShape.Contours = new BezierContour[1];
			}
			rectShape.Contours[0] = bezierContour;
			rectShape.IsConvex = true;
		}

		public static void MakeEllipseShape(Shape ellipseShape, Vector2 pos, float radiusX, float radiusY)
		{
			Rect rect = new Rect(pos.x - radiusX, pos.y - radiusY, radiusX + radiusX, radiusY + radiusY);
			Vector2 vector = new Vector2(radiusX, radiusY);
			VectorUtils.MakeRectangleShape(ellipseShape, rect, vector, vector, vector, vector);
		}

		public static void MakeCircleShape(Shape circleShape, Vector2 pos, float radius)
		{
			VectorUtils.MakeEllipseShape(circleShape, pos, radius, radius);
		}

		public static Rect Bounds(BezierPathSegment[] path)
		{
			Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
			foreach (BezierSegment bezierSegment in VectorUtils.SegmentsInPath(path, false))
			{
				Vector2 vector3;
				Vector2 vector4;
				VectorUtils.Bounds(bezierSegment, out vector3, out vector4);
				vector = Vector2.Min(vector, vector3);
				vector2 = Vector2.Max(vector2, vector4);
			}
			return (vector.x != float.MaxValue) ? new Rect(vector, vector2 - vector) : Rect.zero;
		}

		public static Rect Bounds(IEnumerable<Vector2> vertices)
		{
			Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
			foreach (Vector2 vector3 in vertices)
			{
				vector = Vector2.Min(vector, vector3);
				vector2 = Vector2.Max(vector2, vector3);
			}
			return (vector.x != float.MaxValue) ? new Rect(vector, vector2 - vector) : Rect.zero;
		}

		public static BezierSegment MakeLine(Vector2 from, Vector2 to)
		{
			return new BezierSegment
			{
				P0 = from,
				P1 = (to - from) / 3f + from,
				P2 = (to - from) * 2f / 3f + from,
				P3 = to
			};
		}

		public static BezierSegment QuadraticToCubic(Vector2 p0, Vector2 p1, Vector2 p2)
		{
			float num = 0.6666667f;
			return new BezierSegment
			{
				P0 = p0,
				P1 = p0 + num * (p1 - p0),
				P2 = p2 + num * (p1 - p2),
				P3 = p2
			};
		}

		public static BezierPathSegment[] MakePathLine(Vector2 from, Vector2 to)
		{
			return new BezierPathSegment[]
			{
				new BezierPathSegment
				{
					P0 = from,
					P1 = (to - from) / 3f + from,
					P2 = (to - from) * 2f / 3f + from
				},
				new BezierPathSegment
				{
					P0 = to
				}
			};
		}

		internal static BezierSegment MakeArcQuarter(Vector2 center, float startAngleRads, float sweepAngleRads)
		{
			float num = Mathf.Sin(sweepAngleRads);
			float num2 = Mathf.Cos(sweepAngleRads);
			Matrix2D matrix2D = Matrix2D.RotateLH(startAngleRads);
			matrix2D.m02 = center.x;
			matrix2D.m12 = center.y;
			float num3 = 0.55191505f;
			return new BezierSegment
			{
				P0 = matrix2D * new Vector2(1f, 0f),
				P1 = matrix2D * new Vector2(1f, num3),
				P2 = matrix2D * new Vector2(num2 + num3 * num, num),
				P3 = matrix2D * new Vector2(num2, num)
			};
		}

		public static BezierPathSegment[] MakeArc(Vector2 center, float startAngleRads, float sweepAngleRads, float radius)
		{
			bool flag = false;
			bool flag2 = sweepAngleRads < 0f;
			if (flag2)
			{
				startAngleRads += sweepAngleRads;
				sweepAngleRads = -sweepAngleRads;
				flag = true;
			}
			sweepAngleRads = Mathf.Min(sweepAngleRads, 6.2831855f);
			List<BezierSegment> list = new List<BezierSegment>();
			int num = VectorUtils.QuadrantAtAngle(sweepAngleRads);
			for (int i = 0; i <= num; i++)
			{
				BezierSegment bezierSegment = VectorUtils.ArcSegmentForQuadrant(i);
				Vector2 zero = Vector2.zero;
				Vector2 vector = new Vector2(2f, 0f);
				float[] array = VectorUtils.FindBezierLineIntersections(bezierSegment, zero, vector);
				bool flag3 = i != 3 && array.Length != 0;
				if (flag3)
				{
					BezierSegment bezierSegment2;
					BezierSegment bezierSegment3;
					VectorUtils.SplitSegment(bezierSegment, array[0], out bezierSegment2, out bezierSegment3);
					bezierSegment = bezierSegment3;
				}
				vector = new Vector2(Mathf.Cos(sweepAngleRads), Mathf.Sin(sweepAngleRads)) * 2f;
				array = VectorUtils.FindBezierLineIntersections(bezierSegment, zero, vector);
				bool flag4 = array.Length != 0;
				if (flag4)
				{
					BezierSegment bezierSegment2;
					BezierSegment bezierSegment3;
					VectorUtils.SplitSegment(bezierSegment, array[0], out bezierSegment2, out bezierSegment3);
					bezierSegment = bezierSegment2;
				}
				bool flag5 = !VectorUtils.IsEmptySegment(bezierSegment);
				if (flag5)
				{
					list.Add(bezierSegment);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				list[j] = VectorUtils.TransformSegment(list[j], center, -startAngleRads, Vector2.one * radius);
			}
			bool flag6 = flag;
			if (flag6)
			{
				for (int k = 0; k < list.Count / 2; k++)
				{
					int num2 = list.Count - k - 1;
					BezierSegment bezierSegment4 = VectorUtils.FlipSegment(list[k]);
					BezierSegment bezierSegment5 = VectorUtils.FlipSegment(list[num2]);
					list[k] = bezierSegment5;
					list[num2] = bezierSegment4;
				}
				bool flag7 = list.Count % 2 == 1;
				if (flag7)
				{
					int num3 = list.Count / 2;
					list[num3] = VectorUtils.FlipSegment(list[num3]);
				}
			}
			return VectorUtils.BezierSegmentsToPath(list.ToArray());
		}

		internal static int QuadrantAtAngle(float angle)
		{
			angle %= 6.2831855f;
			bool flag = angle < 0f;
			if (flag)
			{
				angle = 6.2831855f + angle;
			}
			bool flag2 = angle <= 1.5707964f;
			int num;
			if (flag2)
			{
				num = 0;
			}
			else
			{
				bool flag3 = angle <= 3.1415927f;
				if (flag3)
				{
					num = 1;
				}
				else
				{
					bool flag4 = angle <= 4.712389f;
					if (flag4)
					{
						num = 2;
					}
					else
					{
						num = 3;
					}
				}
			}
			return num;
		}

		internal static BezierSegment ArcSegmentForQuadrant(int quadrant)
		{
			BezierSegment bezierSegment;
			switch (quadrant)
			{
			case 0:
				bezierSegment = VectorUtils.MakeArcQuarter(Vector2.zero, 0f, 1.5707964f);
				break;
			case 1:
				bezierSegment = VectorUtils.MakeArcQuarter(Vector2.zero, -1.5707964f, 1.5707964f);
				break;
			case 2:
				bezierSegment = VectorUtils.MakeArcQuarter(Vector2.zero, -3.1415927f, 1.5707964f);
				break;
			case 3:
				bezierSegment = VectorUtils.MakeArcQuarter(Vector2.zero, -4.712389f, 1.5707964f);
				break;
			default:
				bezierSegment = default(BezierSegment);
				break;
			}
			return bezierSegment;
		}

		public static BezierSegment FlipSegment(BezierSegment segment)
		{
			BezierSegment bezierSegment = segment;
			Vector2 vector = bezierSegment.P0;
			bezierSegment.P0 = bezierSegment.P3;
			bezierSegment.P3 = vector;
			vector = bezierSegment.P1;
			bezierSegment.P1 = bezierSegment.P2;
			bezierSegment.P2 = vector;
			return bezierSegment;
		}

		public static void Bounds(BezierSegment segment, out Vector2 min, out Vector2 max)
		{
			min = Vector2.Min(segment.P0, segment.P3);
			max = Vector2.Max(segment.P0, segment.P3);
			Vector2 vector = 3f * segment.P3 - 9f * segment.P2 + 9f * segment.P1 - 3f * segment.P0;
			Vector2 vector2 = 6f * segment.P2 - 12f * segment.P1 + 6f * segment.P0;
			Vector2 vector3 = 3f * segment.P1 - 3f * segment.P0;
			float[] array = new float[4];
			VectorUtils.SolveQuadratic(vector.x, vector2.x, vector3.x, out array[0], out array[1]);
			VectorUtils.SolveQuadratic(vector.y, vector2.y, vector3.y, out array[2], out array[3]);
			foreach (float num in array)
			{
				bool flag = float.IsNaN(num) || num < 0f || num > 1f;
				if (!flag)
				{
					Vector2 vector4 = VectorUtils.Eval(segment, num);
					min = Vector2.Min(min, vector4);
					max = Vector2.Max(max, vector4);
				}
			}
		}

		public static Vector2 Eval(BezierSegment segment, float t)
		{
			float num = t * t;
			float num2 = num * t;
			return (segment.P3 - 3f * segment.P2 + 3f * segment.P1 - segment.P0) * num2 + (3f * segment.P2 - 6f * segment.P1 + 3f * segment.P0) * num + (3f * segment.P1 - 3f * segment.P0) * t + segment.P0;
		}

		public static Vector2 EvalTangent(BezierSegment segment, float t)
		{
			Vector2 vector = (segment.P3 - 3f * segment.P2 + 3f * segment.P1 - segment.P0) * 3f * t * t + (3f * segment.P2 - 6f * segment.P1 + 3f * segment.P0) * 2f * t + (3f * segment.P1 - 3f * segment.P0);
			bool flag = vector.sqrMagnitude < VectorUtils.Epsilon;
			if (flag)
			{
				bool flag2 = t > 0.5f;
				if (flag2)
				{
					vector = VectorUtils.Eval(segment, t) - VectorUtils.Eval(segment, t - 0.01f);
				}
				else
				{
					vector = VectorUtils.Eval(segment, t + 0.01f) - VectorUtils.Eval(segment, t);
				}
			}
			return vector.normalized;
		}

		public static Vector2 EvalNormal(BezierSegment segment, float t)
		{
			return Vector2.Perpendicular(VectorUtils.EvalTangent(segment, t));
		}

		public static Vector2 EvalFull(BezierSegment segment, float t, out Vector2 tangent)
		{
			float num = t * t;
			float num2 = num * t;
			Vector2 vector = segment.P3 - 3f * segment.P2 + 3f * segment.P1 - segment.P0;
			Vector2 vector2 = 3f * segment.P2 - 6f * segment.P1 + 3f * segment.P0;
			Vector2 vector3 = 3f * segment.P1 - 3f * segment.P0;
			Vector2 p = segment.P0;
			Vector2 vector4 = vector * num2 + vector2 * num + vector3 * t + p;
			tangent = 3f * vector * num + 2f * vector2 * t + vector3;
			bool flag = tangent.sqrMagnitude < VectorUtils.Epsilon;
			if (flag)
			{
				bool flag2 = t > 0.5f;
				if (flag2)
				{
					tangent = vector4 - VectorUtils.Eval(segment, t - 0.01f);
				}
				else
				{
					tangent = VectorUtils.Eval(segment, t + 0.01f) - vector4;
				}
			}
			tangent = tangent.normalized;
			return vector4;
		}

		public static Vector2 EvalFull(BezierSegment segment, float t, out Vector2 tangent, out Vector2 normal)
		{
			Vector2 vector = VectorUtils.EvalFull(segment, t, out tangent);
			normal = Vector2.Perpendicular(tangent);
			return vector;
		}

		public static float[] SegmentsLengths(IList<BezierPathSegment> segments, bool closed, float precision = 0.001f)
		{
			float[] array = new float[segments.Count - 1 + (closed ? 1 : 0)];
			int num = 0;
			foreach (BezierSegment bezierSegment in VectorUtils.SegmentsInPath(segments, closed))
			{
				array[num++] = VectorUtils.SegmentLength(bezierSegment, precision);
			}
			return array;
		}

		public static float SegmentsLength(IList<BezierPathSegment> segments, bool closed, float precision = 0.001f)
		{
			bool flag = segments.Count < 2;
			float num;
			if (flag)
			{
				num = 0f;
			}
			else
			{
				float num2 = 0f;
				foreach (BezierSegment bezierSegment in VectorUtils.SegmentsInPath(segments, false))
				{
					num2 += VectorUtils.SegmentLength(bezierSegment, precision);
				}
				if (closed)
				{
					num2 += (segments[segments.Count - 1].P0 - segments[0].P0).magnitude;
				}
				num = num2;
			}
			return num;
		}

		public static float SegmentLength(BezierSegment segment, float precision = 0.001f)
		{
			bool flag = VectorUtils.HasLargeCoordinates(segment);
			float num2;
			if (flag)
			{
				int num = Math.Min(100, (int)(1f / precision));
				num2 = VectorUtils.SegmentLengthIterative(segment, num);
			}
			else
			{
				float num3 = 0f;
				float num4;
				while ((num4 = VectorUtils.AdaptiveQuadraticApproxSplitPoint(segment, precision)) < 1f)
				{
					BezierSegment bezierSegment;
					BezierSegment bezierSegment2;
					VectorUtils.SplitSegment(segment, num4, out bezierSegment, out bezierSegment2);
					float num5 = VectorUtils.MidPointQuadraticApproxLength(bezierSegment);
					bool flag2 = float.IsNaN(num5);
					if (flag2)
					{
						num5 = VectorUtils.SegmentLengthIterative(bezierSegment, 10);
					}
					num3 += num5;
					segment = bezierSegment2;
				}
				num3 += VectorUtils.MidPointQuadraticApproxLength(segment);
				num2 = num3;
			}
			return num2;
		}

		internal static float SegmentLengthIterative(BezierSegment segment, int steps = 10)
		{
			bool flag = steps <= 2;
			float num;
			if (flag)
			{
				num = (segment.P3 - segment.P0).magnitude;
			}
			else
			{
				float num2 = 0f;
				Vector2 vector = segment.P0;
				for (int i = 1; i <= steps; i++)
				{
					float num3 = (float)i / (float)steps;
					Vector2 vector2 = VectorUtils.Eval(segment, num3);
					num2 += (vector2 - vector).magnitude;
					vector = vector2;
				}
				num = num2;
			}
			return num;
		}

		internal static bool HasLargeCoordinates(BezierSegment segment)
		{
			return segment.P0.x > 10000f || segment.P0.y > 10000f || segment.P1.x > 10000f || segment.P1.y > 10000f || segment.P2.x > 10000f || segment.P2.y > 10000f || segment.P3.x > 10000f || segment.P3.y > 10000f;
		}

		private static float AdaptiveQuadraticApproxSplitPoint(BezierSegment segment, float precision)
		{
			float num = (segment.P3 - 3f * segment.P2 + 3f * segment.P1 - segment.P0).magnitude * 0.5f;
			return Mathf.Pow(18f / Mathf.Sqrt(3f) * precision / num, 0.33333334f);
		}

		private static float MidPointQuadraticApproxLength(BezierSegment segment)
		{
			Vector2 p = segment.P0;
			Vector2 vector = (3f * segment.P2 - segment.P3 + 3f * segment.P1 - segment.P0) / 4f;
			Vector2 p2 = segment.P3;
			bool flag = p == p2;
			float num;
			if (flag)
			{
				num = ((p == vector) ? 0f : (p - vector).magnitude);
			}
			else
			{
				bool flag2 = vector == p || vector == p2;
				if (flag2)
				{
					num = (p - p2).magnitude;
				}
				else
				{
					Vector2 vector2 = vector - p;
					Vector2 vector3 = p - 2f * vector + p2;
					bool flag3 = vector3 != Vector2.zero;
					if (flag3)
					{
						double num2 = (double)(4f * Vector2.Dot(vector3, vector3));
						double num3 = (double)(8f * Vector2.Dot(vector2, vector3));
						double num4 = (double)(4f * Vector2.Dot(vector2, vector2));
						double num5 = 4.0 * num4 * num2 - num3 * num3;
						double num6 = 2.0 * num2 + num3;
						double num7 = num2 + num3 + num4;
						double num8 = 0.25 / num2 * (num6 * Math.Sqrt(num7) - num3 * Math.Sqrt(num4));
						bool flag4 = Math.Abs(num5) <= (double)VectorUtils.Epsilon;
						if (flag4)
						{
							num = (float)num8;
						}
						else
						{
							double num9 = num5 / (8.0 * Math.Pow(num2, 1.5)) * (Math.Log(2.0 * Math.Sqrt(num2 * num7) + num6) - Math.Log(2.0 * Math.Sqrt(num2 * num4) + num3));
							num = (float)(num8 + num9);
						}
					}
					else
					{
						num = 2f * vector2.magnitude;
					}
				}
			}
			return num;
		}

		public static void SplitSegment(BezierSegment segment, float t, out BezierSegment b1, out BezierSegment b2)
		{
			Vector2 vector = Vector2.LerpUnclamped(segment.P0, segment.P1, t);
			Vector2 vector2 = Vector2.LerpUnclamped(segment.P1, segment.P2, t);
			Vector2 vector3 = Vector2.LerpUnclamped(segment.P2, segment.P3, t);
			Vector2 vector4 = Vector2.LerpUnclamped(vector, vector2, t);
			Vector2 vector5 = Vector2.LerpUnclamped(vector2, vector3, t);
			Vector2 vector6 = VectorUtils.Eval(segment, t);
			b1 = new BezierSegment
			{
				P0 = segment.P0,
				P1 = vector,
				P2 = vector4,
				P3 = vector6
			};
			b2 = new BezierSegment
			{
				P0 = vector6,
				P1 = vector5,
				P2 = vector3,
				P3 = segment.P3
			};
		}

		public static BezierSegment TransformSegment(BezierSegment segment, Vector2 translation, float rotation, Vector2 scaling)
		{
			Matrix2D matrix2D = Matrix2D.RotateLH(rotation);
			return new BezierSegment
			{
				P0 = matrix2D * Vector2.Scale(segment.P0, scaling) + translation,
				P1 = matrix2D * Vector2.Scale(segment.P1, scaling) + translation,
				P2 = matrix2D * Vector2.Scale(segment.P2, scaling) + translation,
				P3 = matrix2D * Vector2.Scale(segment.P3, scaling) + translation
			};
		}

		public static BezierSegment TransformSegment(BezierSegment segment, Matrix2D matrix)
		{
			return new BezierSegment
			{
				P0 = matrix * segment.P0,
				P1 = matrix * segment.P1,
				P2 = matrix * segment.P2,
				P3 = matrix * segment.P3
			};
		}

		public static BezierPathSegment[] TransformBezierPath(BezierPathSegment[] path, Vector2 translation, float rotation, Vector2 scaling)
		{
			Matrix2D matrix2D = Matrix2D.RotateLH(rotation);
			BezierPathSegment[] array = new BezierPathSegment[path.Length];
			for (int i = 0; i < array.Length; i++)
			{
				BezierPathSegment bezierPathSegment = path[i];
				array[i] = new BezierPathSegment
				{
					P0 = matrix2D * Vector2.Scale(bezierPathSegment.P0, scaling) + translation,
					P1 = matrix2D * Vector2.Scale(bezierPathSegment.P1, scaling) + translation,
					P2 = matrix2D * Vector2.Scale(bezierPathSegment.P2, scaling) + translation
				};
			}
			return array;
		}

		public static BezierPathSegment[] TransformBezierPath(BezierPathSegment[] path, Matrix2D matrix)
		{
			BezierPathSegment[] array = new BezierPathSegment[path.Length];
			for (int i = 0; i < array.Length; i++)
			{
				BezierPathSegment bezierPathSegment = path[i];
				array[i] = new BezierPathSegment
				{
					P0 = matrix * bezierPathSegment.P0,
					P1 = matrix * bezierPathSegment.P1,
					P2 = matrix * bezierPathSegment.P2
				};
			}
			return array;
		}

		public static IEnumerable<SceneNode> SceneNodes(SceneNode root)
		{
			yield return root;
			bool flag = root.Children != null;
			if (flag)
			{
				foreach (SceneNode c in root.Children)
				{
					foreach (SceneNode i in VectorUtils.SceneNodes(c))
					{
						yield return i;
						i = null;
					}
					IEnumerator<SceneNode> enumerator2 = null;
					c = null;
				}
				List<SceneNode>.Enumerator enumerator = default(List<SceneNode>.Enumerator);
			}
			yield break;
			yield break;
		}

		private static IEnumerable<VectorUtils.SceneNodeWorldTransform> WorldTransformedSceneNodes(SceneNode child, Dictionary<SceneNode, float> nodeOpacities, VectorUtils.SceneNodeWorldTransform parent)
		{
			float childOpacity = 1f;
			bool flag = nodeOpacities == null || !nodeOpacities.TryGetValue(child, out childOpacity);
			if (flag)
			{
				childOpacity = 1f;
			}
			VectorUtils.SceneNodeWorldTransform childWorldTransform = new VectorUtils.SceneNodeWorldTransform
			{
				Node = child,
				WorldTransform = parent.WorldTransform * child.Transform,
				WorldOpacity = parent.WorldOpacity * childOpacity,
				Parent = parent.Node
			};
			yield return childWorldTransform;
			bool flag2 = child.Children != null;
			if (flag2)
			{
				foreach (SceneNode c in child.Children)
				{
					foreach (VectorUtils.SceneNodeWorldTransform i in VectorUtils.WorldTransformedSceneNodes(c, nodeOpacities, childWorldTransform))
					{
						yield return i;
						i = default(VectorUtils.SceneNodeWorldTransform);
					}
					IEnumerator<VectorUtils.SceneNodeWorldTransform> enumerator2 = null;
					c = null;
				}
				List<SceneNode>.Enumerator enumerator = default(List<SceneNode>.Enumerator);
			}
			yield break;
			yield break;
		}

		public static IEnumerable<VectorUtils.SceneNodeWorldTransform> WorldTransformedSceneNodes(SceneNode root, Dictionary<SceneNode, float> nodeOpacities)
		{
			VectorUtils.SceneNodeWorldTransform sceneNodeWorldTransform = new VectorUtils.SceneNodeWorldTransform
			{
				Node = root,
				WorldTransform = Matrix2D.identity,
				WorldOpacity = 1f,
				Parent = null
			};
			return VectorUtils.WorldTransformedSceneNodes(root, nodeOpacities, sceneNodeWorldTransform);
		}

		public static void RealignVerticesInBounds(IList<Vector2> vertices, Rect bounds, bool flip)
		{
			Vector2 position = bounds.position;
			float height = bounds.height;
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 vector = vertices[i];
				vector -= position;
				if (flip)
				{
					vector.y = height - vector.y;
				}
				vertices[i] = vector;
			}
		}

		public static void FlipVerticesInBounds(IList<Vector2> vertices, Rect bounds)
		{
			float height = bounds.height;
			for (int i = 0; i < vertices.Count; i++)
			{
				Vector2 vector = vertices[i];
				vector.y = height - vector.y;
				vertices[i] = vector;
			}
		}

		internal static void ClampVerticesInBounds(IList<Vector2> vertices, Rect bounds)
		{
			for (int i = 0; i < vertices.Count; i++)
			{
				vertices[i] = Vector2.Max(bounds.min, Vector2.Min(bounds.max, vertices[i]));
			}
		}

		public static IEnumerable<BezierSegment> SegmentsInPath(IEnumerable<BezierPathSegment> segments, bool closed = false)
		{
			IEnumerator<BezierPathSegment> e = segments.GetEnumerator();
			bool flag = !e.MoveNext();
			if (flag)
			{
				yield break;
			}
			BezierPathSegment s = e.Current;
			bool flag2 = !e.MoveNext();
			if (flag2)
			{
				yield break;
			}
			do
			{
				BezierPathSegment s2 = e.Current;
				yield return new BezierSegment
				{
					P0 = s.P0,
					P1 = s.P1,
					P2 = s.P2,
					P3 = s2.P0
				};
				s = s2;
				s2 = default(BezierPathSegment);
			}
			while (e.MoveNext());
			if (closed)
			{
				Vector2 first = Vector2.zero;
				using (IEnumerator<BezierPathSegment> enumerator = segments.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						BezierPathSegment seg = enumerator.Current;
						first = seg.P0;
					}
				}
				IEnumerator<BezierPathSegment> enumerator = null;
				yield return new BezierSegment
				{
					P0 = s.P0,
					P1 = s.P1,
					P2 = s.P2,
					P3 = first
				};
				first = default(Vector2);
			}
			yield break;
		}

		private static void SolveQuadratic(float a, float b, float c, out float s1, out float s2)
		{
			float num = b * b - 4f * a * c;
			bool flag = num < 0f;
			if (flag)
			{
				s1 = (s2 = float.NaN);
			}
			else
			{
				float num2 = Mathf.Sqrt(num);
				s1 = (-b + num2) / (2f * a);
				bool flag2 = Mathf.Abs(a) > float.Epsilon;
				if (flag2)
				{
					s2 = (-b - num2) / (2f * a);
				}
				else
				{
					s2 = float.NaN;
				}
			}
		}

		public static Vector2 IntersectLines(Vector2 line1Pt1, Vector2 line1Pt2, Vector2 line2Pt1, Vector2 line2Pt2)
		{
			float num = line1Pt2.y - line1Pt1.y;
			float num2 = line1Pt1.x - line1Pt2.x;
			float num3 = line2Pt2.y - line2Pt1.y;
			float num4 = line2Pt1.x - line2Pt2.x;
			float num5 = num * num4 - num3 * num2;
			bool flag = Mathf.Abs(num5) <= VectorUtils.Epsilon;
			Vector2 vector;
			if (flag)
			{
				vector = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
			}
			else
			{
				float num6 = num * line1Pt1.x + num2 * line1Pt1.y;
				float num7 = num3 * line2Pt1.x + num4 * line2Pt1.y;
				float num8 = 1f / num5;
				vector = new Vector2((num4 * num6 - num2 * num7) * num8, (num * num7 - num3 * num6) * num8);
			}
			return vector;
		}

		public static Vector2 IntersectLineSegments(Vector2 line1Pt1, Vector2 line1Pt2, Vector2 line2Pt1, Vector2 line2Pt2)
		{
			float num = (line1Pt1.x - line2Pt2.x) * (line1Pt2.y - line2Pt2.y) - (line1Pt1.y - line2Pt2.y) * (line1Pt2.x - line2Pt2.x);
			float num2 = (line1Pt1.x - line2Pt1.x) * (line1Pt2.y - line2Pt1.y) - (line1Pt1.y - line2Pt1.y) * (line1Pt2.x - line2Pt1.x);
			bool flag = num * num2 <= 0f;
			if (flag)
			{
				float num3 = (line2Pt1.x - line1Pt1.x) * (line2Pt2.y - line1Pt1.y) - (line2Pt1.y - line1Pt1.y) * (line2Pt2.x - line1Pt1.x);
				float num4 = num3 + num2 - num;
				bool flag2 = num3 * num4 <= 0f;
				if (flag2)
				{
					float num5 = num3 / (num3 - num4);
					return line1Pt1 + num5 * (line1Pt2 - line1Pt1);
				}
			}
			return new Vector2(float.PositiveInfinity, float.PositiveInfinity);
		}

		private static bool PointOnTheLeftOfLine(Vector2 lineFrom, Vector2 lineTo, Vector2 point)
		{
			return (lineFrom.x - lineTo.x) * (point.y - lineTo.y) - (lineFrom.y - lineTo.y) * (point.x - lineTo.x) > 0f;
		}

		public static float[] FindBezierLineIntersections(BezierSegment segment, Vector2 p0, Vector2 p1)
		{
			float num = p1.y - p0.y;
			float num2 = p0.x - p1.x;
			float num3 = p0.x * (p0.y - p1.y) + p0.y * (p1.x - p0.x);
			Vector2[] array = VectorUtils.BezierCoefficients(segment);
			float[] array2 = new float[]
			{
				num * array[0].x + num2 * array[0].y,
				num * array[1].x + num2 * array[1].y,
				num * array[2].x + num2 * array[2].y,
				num * array[3].x + num2 * array[3].y + num3
			};
			float[] array3 = VectorUtils.CubicRoots((double)array2[0], (double)array2[1], (double)array2[2], (double)array2[3]);
			List<float> list = new List<float>(array3.Length);
			foreach (float num4 in array3)
			{
				float num5 = num4 * num4;
				float num6 = num5 * num4;
				Vector2 vector = array[0] * num6 + array[1] * num5 + array[2] * num4 + array[3];
				bool flag = Mathf.Abs(p1.x - p0.x) > VectorUtils.Epsilon;
				float num7;
				if (flag)
				{
					num7 = (vector.x - p0.x) / (p1.x - p0.x);
				}
				else
				{
					num7 = (vector.y - p0.y) / (p1.y - p0.y);
				}
				bool flag2 = num4 >= 0f && num4 <= 1f && num7 >= 0f && num7 <= 1f;
				if (flag2)
				{
					list.Add(num4);
				}
			}
			return list.ToArray();
		}

		private static float[] CubicRoots(double a, double b, double c, double d)
		{
			double num = b / a;
			double num2 = c / a;
			double num3 = d / a;
			double num4 = (3.0 * num2 - Math.Pow(num, 2.0)) / 9.0;
			double num5 = (9.0 * num * num2 - 27.0 * num3 - 2.0 * Math.Pow(num, 3.0)) / 54.0;
			double num6 = Math.Pow(num4, 3.0) + Math.Pow(num5, 2.0);
			List<double> list = new List<double>(3);
			list.AddRange(new double[] { -1.0, -1.0, -1.0 });
			bool flag = num6 >= 0.0;
			if (flag)
			{
				double num7 = Math.Sqrt(num6);
				double num8 = (double)Math.Sign(num5 + num7) * Math.Pow(Math.Abs(num5 + num7), 0.3333333333333333);
				double num9 = (double)Math.Sign(num5 - num7) * Math.Pow(Math.Abs(num5 - num7), 0.3333333333333333);
				list[0] = -num / 3.0 + (num8 + num9);
				list[1] = -num / 3.0 - (num8 + num9) / 2.0;
				list[2] = list[1];
				double num10 = Math.Abs(Math.Sqrt(3.0) * (num8 - num9) / 2.0);
				bool flag2 = Math.Abs(num10) > (double)VectorUtils.Epsilon;
				if (flag2)
				{
					list[1] = -1.0;
					list[2] = -1.0;
				}
			}
			else
			{
				double num11 = Math.Acos(num5 / Math.Sqrt(-Math.Pow(num4, 3.0)));
				double num12 = Math.Sqrt(-num4);
				list[0] = 2.0 * num12 * Math.Cos(num11 / 3.0) - num / 3.0;
				list[1] = 2.0 * num12 * Math.Cos((num11 + 6.283185307179586) / 3.0) - num / 3.0;
				list[2] = 2.0 * num12 * Math.Cos((num11 + 12.566370614359172) / 3.0) - num / 3.0;
			}
			for (int i = 0; i < 3; i++)
			{
				bool flag3 = list[i] < 0.0 || list[i] > 1.0;
				if (flag3)
				{
					list[i] = -1.0;
				}
			}
			list.RemoveAll((double x) => Math.Abs(x + 1.0) < (double)VectorUtils.Epsilon);
			float[] array = new float[list.Count];
			for (int j = 0; j < list.Count; j++)
			{
				array[j] = (float)list[j];
			}
			return array;
		}

		private static Vector2[] BezierCoefficients(BezierSegment segment)
		{
			return new Vector2[]
			{
				-segment.P0 + 3f * segment.P1 + -3f * segment.P2 + segment.P3,
				3f * segment.P0 - 6f * segment.P1 + 3f * segment.P2,
				-3f * segment.P0 + 3f * segment.P1,
				segment.P0
			};
		}

		public static Rect SceneNodeBounds(SceneNode root)
		{
			Vector2 vector = new Vector2(float.MaxValue, float.MaxValue);
			Vector2 vector2 = new Vector2(float.MinValue, float.MinValue);
			foreach (VectorUtils.SceneNodeWorldTransform sceneNodeWorldTransform in VectorUtils.WorldTransformedSceneNodes(root, null))
			{
				Vector2 vector3 = new Vector2(float.MaxValue, float.MaxValue);
				Vector2 vector4 = new Vector2(float.MinValue, float.MinValue);
				bool flag = sceneNodeWorldTransform.Node.Shapes != null;
				if (flag)
				{
					foreach (Shape shape in sceneNodeWorldTransform.Node.Shapes)
					{
						foreach (BezierContour bezierContour in shape.Contours)
						{
							Rect rect = VectorUtils.Bounds(VectorUtils.TransformBezierPath(bezierContour.Segments, sceneNodeWorldTransform.WorldTransform));
							vector3 = Vector2.Min(vector3, rect.min);
							vector4 = Vector2.Max(vector4, rect.max);
						}
					}
				}
				bool flag2 = vector3.x != float.MaxValue;
				if (flag2)
				{
					vector = Vector2.Min(vector, vector3);
					vector2 = Vector2.Max(vector2, vector4);
				}
			}
			return (vector.x != float.MaxValue) ? new Rect(vector, vector2 - vector) : Rect.zero;
		}

		public static Rect ApproximateSceneNodeBounds(SceneNode root)
		{
			List<Vector2> list = new List<Vector2>(100);
			foreach (VectorUtils.SceneNodeWorldTransform sceneNodeWorldTransform in VectorUtils.WorldTransformedSceneNodes(root, null))
			{
				bool flag = sceneNodeWorldTransform.Node.Shapes != null;
				if (flag)
				{
					foreach (Shape shape in sceneNodeWorldTransform.Node.Shapes)
					{
						foreach (BezierContour bezierContour in shape.Contours)
						{
							foreach (BezierPathSegment bezierPathSegment in VectorUtils.TransformBezierPath(bezierContour.Segments, sceneNodeWorldTransform.WorldTransform))
							{
								list.Add(bezierPathSegment.P0);
								list.Add(bezierPathSegment.P1);
								list.Add(bezierPathSegment.P2);
							}
						}
					}
				}
			}
			return VectorUtils.Bounds(list);
		}

		internal static bool IsEmptySegment(BezierSegment bs)
		{
			return (bs.P0 - bs.P1).sqrMagnitude <= VectorUtils.Epsilon && (bs.P0 - bs.P2).sqrMagnitude <= VectorUtils.Epsilon && (bs.P0 - bs.P3).sqrMagnitude <= VectorUtils.Epsilon;
		}

		private static Material s_ExpandEdgesMat;

		private static Material s_DemulMat;

		private static Material s_BlendMat;

		public static readonly float Epsilon = 1E-06f;

		public struct PackRectItem
		{
			public Vector2 Position;

			public Vector2 Size;

			public bool Rotated;

			public IFill Fill;

			internal int SettingIndex;
		}

		public class Geometry
		{
			public Vector2[] Vertices;

			public Vector2[] UVs;

			public ushort[] Indices;

			public Color Color;

			public Matrix2D WorldTransform;

			public IFill Fill;

			public Matrix2D FillTransform;

			public Rect UnclippedBounds;

			public int SettingIndex;
		}

		internal struct RawTexture
		{
			public Color32[] Rgba;

			public int Width;

			public int Height;
		}

		private class AtlasEntry
		{
			public VectorUtils.RawTexture Texture;

			public VectorUtils.PackRectItem AtlasLocation;
		}

		public class TextureAtlas
		{
			public Texture2D Texture { get; set; }

			public List<VectorUtils.PackRectItem> Entries { get; set; }
		}

		public enum Alignment
		{
			Center,
			TopLeft,
			TopCenter,
			TopRight,
			LeftCenter,
			RightCenter,
			BottomLeft,
			BottomCenter,
			BottomRight,
			Custom,
			SVGOrigin
		}

		internal enum WindingDir
		{
			CW,
			CCW
		}

		public struct TessellationOptions
		{
			public float StepDistance { readonly get; set; }

			public float MaxCordDeviation
			{
				get
				{
					return this.m_MaxCordDev;
				}
				set
				{
					this.m_MaxCordDev = Mathf.Max(value, 0f);
					this.m_MaxCordDevSq = ((this.m_MaxCordDev == float.MaxValue) ? float.MaxValue : (this.m_MaxCordDev * this.m_MaxCordDev));
				}
			}

			internal float MaxCordDeviationSquared
			{
				get
				{
					return this.m_MaxCordDevSq;
				}
			}

			public float MaxTanAngleDeviation
			{
				get
				{
					return this.m_MaxTanAngleDev;
				}
				set
				{
					this.m_MaxTanAngleDev = Mathf.Clamp(value, VectorUtils.Epsilon, 1.5707964f);
					this.m_MaxTanAngleDevCosine = Mathf.Cos(this.m_MaxTanAngleDev);
				}
			}

			internal float MaxTanAngleDeviationCosine
			{
				get
				{
					return this.m_MaxTanAngleDevCosine;
				}
			}

			public float SamplingStepSize
			{
				get
				{
					return this.m_StepSize;
				}
				set
				{
					this.m_StepSize = Mathf.Clamp(value, VectorUtils.Epsilon, 1f);
				}
			}

			private float m_MaxCordDev;

			private float m_MaxCordDevSq;

			private float m_MaxTanAngleDev;

			private float m_MaxTanAngleDevCosine;

			private float m_StepSize;
		}

		private class JoiningInfo
		{
			public Vector2 JoinPos;

			public Vector2 TanAtEnd;

			public Vector2 TanAtStart;

			public Vector2 NormAtEnd;

			public Vector2 NormAtStart;

			public Vector2 PosThicknessStart;

			public Vector2 NegThicknessStart;

			public Vector2 PosThicknessEnd;

			public Vector2 NegThicknessEnd;

			public Vector2 PosThicknessClosingPoint;

			public Vector2 NegThicknessClosingPoint;

			public bool RoundPosThickness;

			public bool SimpleJoin;

			public Vector2 InnerCornerVertex;

			public float InnerCornerDistToEnd;

			public float InnerCornerDistFromStart;
		}

		public struct SceneNodeWorldTransform
		{
			public SceneNode Node;

			public SceneNode Parent;

			public Matrix2D WorldTransform;

			public float WorldOpacity;
		}
	}
}
