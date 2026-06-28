using System;
using System.Collections.Generic;

namespace UnityEngine.UI.Extensions
{
	public class BezierPath
	{
		public BezierPath()
		{
			this.controlPoints = new List<Vector2>();
		}

		public void SetControlPoints(List<Vector2> newControlPoints)
		{
			this.controlPoints.Clear();
			this.controlPoints.AddRange(newControlPoints);
			this.curveCount = (this.controlPoints.Count - 1) / 3;
		}

		public void SetControlPoints(Vector2[] newControlPoints)
		{
			this.controlPoints.Clear();
			this.controlPoints.AddRange(newControlPoints);
			this.curveCount = (this.controlPoints.Count - 1) / 3;
		}

		public List<Vector2> GetControlPoints()
		{
			return this.controlPoints;
		}

		public void Interpolate(List<Vector2> segmentPoints, float scale)
		{
			this.controlPoints.Clear();
			if (segmentPoints.Count >= 2)
			{
				for (int i = 0; i < segmentPoints.Count; i++)
				{
					if (i == 0)
					{
						Vector2 vector = segmentPoints[i];
						Vector2 vector2 = segmentPoints[i + 1];
						Vector2 vector3 = vector2 - vector;
						Vector2 vector4 = vector + scale * vector3;
						this.controlPoints.Add(vector);
						this.controlPoints.Add(vector4);
					}
					else if (i == segmentPoints.Count - 1)
					{
						Vector2 vector5 = segmentPoints[i - 1];
						Vector2 vector6 = segmentPoints[i];
						Vector2 vector7 = vector6 - vector5;
						Vector2 vector8 = vector6 - scale * vector7;
						this.controlPoints.Add(vector8);
						this.controlPoints.Add(vector6);
					}
					else
					{
						Vector2 vector9 = segmentPoints[i - 1];
						Vector2 vector10 = segmentPoints[i];
						Vector2 vector11 = segmentPoints[i + 1];
						Vector2 normalized = (vector11 - vector9).normalized;
						Vector2 vector12 = vector10 - scale * normalized * (vector10 - vector9).magnitude;
						Vector2 vector13 = vector10 + scale * normalized * (vector11 - vector10).magnitude;
						this.controlPoints.Add(vector12);
						this.controlPoints.Add(vector10);
						this.controlPoints.Add(vector13);
					}
				}
				this.curveCount = (this.controlPoints.Count - 1) / 3;
			}
		}

		public void SamplePoints(List<Vector2> sourcePoints, float minSqrDistance, float maxSqrDistance, float scale)
		{
			if (sourcePoints.Count >= 2)
			{
				Stack<Vector2> stack = new Stack<Vector2>();
				stack.Push(sourcePoints[0]);
				Vector2 vector = sourcePoints[1];
				for (int i = 2; i < sourcePoints.Count; i++)
				{
					if ((vector - sourcePoints[i]).sqrMagnitude > minSqrDistance && (stack.Peek() - sourcePoints[i]).sqrMagnitude > maxSqrDistance)
					{
						stack.Push(vector);
					}
					vector = sourcePoints[i];
				}
				Vector2 vector2 = stack.Pop();
				Vector2 vector3 = stack.Peek();
				Vector2 normalized = (vector3 - vector).normalized;
				float magnitude = (vector - vector2).magnitude;
				float magnitude2 = (vector2 - vector3).magnitude;
				vector2 += normalized * ((magnitude2 - magnitude) / 2f);
				stack.Push(vector2);
				stack.Push(vector);
				this.Interpolate(new List<Vector2>(stack), scale);
			}
		}

		public Vector2 CalculateBezierPoint(int curveIndex, float t)
		{
			int num = curveIndex * 3;
			Vector2 vector = this.controlPoints[num];
			Vector2 vector2 = this.controlPoints[num + 1];
			Vector2 vector3 = this.controlPoints[num + 2];
			Vector2 vector4 = this.controlPoints[num + 3];
			return this.CalculateBezierPoint(t, vector, vector2, vector3, vector4);
		}

		public List<Vector2> GetDrawingPoints0()
		{
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < this.curveCount; i++)
			{
				if (i == 0)
				{
					list.Add(this.CalculateBezierPoint(i, 0f));
				}
				for (int j = 1; j <= this.SegmentsPerCurve; j++)
				{
					float num = (float)j / (float)this.SegmentsPerCurve;
					list.Add(this.CalculateBezierPoint(i, num));
				}
			}
			return list;
		}

		public List<Vector2> GetDrawingPoints1()
		{
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < this.controlPoints.Count - 3; i += 3)
			{
				Vector2 vector = this.controlPoints[i];
				Vector2 vector2 = this.controlPoints[i + 1];
				Vector2 vector3 = this.controlPoints[i + 2];
				Vector2 vector4 = this.controlPoints[i + 3];
				if (i == 0)
				{
					list.Add(this.CalculateBezierPoint(0f, vector, vector2, vector3, vector4));
				}
				for (int j = 1; j <= this.SegmentsPerCurve; j++)
				{
					float num = (float)j / (float)this.SegmentsPerCurve;
					list.Add(this.CalculateBezierPoint(num, vector, vector2, vector3, vector4));
				}
			}
			return list;
		}

		public List<Vector2> GetDrawingPoints2()
		{
			List<Vector2> list = new List<Vector2>();
			for (int i = 0; i < this.curveCount; i++)
			{
				List<Vector2> list2 = this.FindDrawingPoints(i);
				if (i != 0)
				{
					list2.RemoveAt(0);
				}
				list.AddRange(list2);
			}
			return list;
		}

		private List<Vector2> FindDrawingPoints(int curveIndex)
		{
			List<Vector2> list = new List<Vector2>();
			Vector2 vector = this.CalculateBezierPoint(curveIndex, 0f);
			Vector2 vector2 = this.CalculateBezierPoint(curveIndex, 1f);
			list.Add(vector);
			list.Add(vector2);
			this.FindDrawingPoints(curveIndex, 0f, 1f, list, 1);
			return list;
		}

		private int FindDrawingPoints(int curveIndex, float t0, float t1, List<Vector2> pointList, int insertionIndex)
		{
			Vector2 vector = this.CalculateBezierPoint(curveIndex, t0);
			Vector2 vector2 = this.CalculateBezierPoint(curveIndex, t1);
			int num;
			if ((vector - vector2).sqrMagnitude < this.MINIMUM_SQR_DISTANCE)
			{
				num = 0;
			}
			else
			{
				float num2 = (t0 + t1) / 2f;
				Vector2 vector3 = this.CalculateBezierPoint(curveIndex, num2);
				Vector2 normalized = (vector - vector3).normalized;
				Vector2 normalized2 = (vector2 - vector3).normalized;
				if (Vector2.Dot(normalized, normalized2) > this.DIVISION_THRESHOLD || Mathf.Abs(num2 - 0.5f) < 0.0001f)
				{
					int num3 = 0;
					num3 += this.FindDrawingPoints(curveIndex, t0, num2, pointList, insertionIndex);
					pointList.Insert(insertionIndex + num3, vector3);
					num3++;
					num3 += this.FindDrawingPoints(curveIndex, num2, t1, pointList, insertionIndex + num3);
					num = num3;
				}
				else
				{
					num = 0;
				}
			}
			return num;
		}

		private Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
		{
			float num = 1f - t;
			float num2 = t * t;
			float num3 = num * num;
			float num4 = num3 * num;
			float num5 = num2 * t;
			Vector2 vector = num4 * p0;
			vector += 3f * num3 * t * p1;
			vector += 3f * num * num2 * p2;
			return vector + num5 * p3;
		}

		public int SegmentsPerCurve = 10;

		public float MINIMUM_SQR_DISTANCE = 0.01f;

		public float DIVISION_THRESHOLD = -0.99f;

		private List<Vector2> controlPoints;

		private int curveCount;
	}
}
