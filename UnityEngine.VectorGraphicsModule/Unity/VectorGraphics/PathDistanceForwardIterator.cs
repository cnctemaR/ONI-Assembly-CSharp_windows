using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.VectorGraphics
{
	internal class PathDistanceForwardIterator
	{
		public PathDistanceForwardIterator(IList<BezierPathSegment> pathSegments, bool closed, float maxCordDeviationSq, float maxTanAngleDevCosine, float stepSizeT)
		{
			bool flag = pathSegments.Count < 2;
			if (flag)
			{
				throw new Exception("Cannot iterate a path with no segments in it");
			}
			IList<BezierPathSegment> list;
			if (!closed || VectorUtils.PathEndsPerfectlyMatch(pathSegments))
			{
				list = pathSegments;
			}
			else
			{
				IList<BezierPathSegment> list2 = new PathDistanceForwardIterator.BezierLoop(pathSegments);
				list = list2;
			}
			this.Segments = list;
			this.closed = closed;
			this.needTangentsDuringEval = maxTanAngleDevCosine < 1f;
			this.maxCordDeviationSq = maxCordDeviationSq;
			this.maxTanAngleDevCosine = maxTanAngleDevCosine;
			this.stepSizeT = stepSizeT;
			this.currentBezSeg = new BezierSegment
			{
				P0 = pathSegments[0].P0,
				P1 = pathSegments[0].P1,
				P2 = pathSegments[0].P2,
				P3 = pathSegments[1].P0
			};
			this.lastPointEval = pathSegments[0].P0;
			this.currentTTangent = (this.needTangentsDuringEval ? VectorUtils.EvalTangent(this.currentBezSeg, 0f) : Vector2.zero);
		}

		private float PointToLineDistanceSq(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
		{
			float sqrMagnitude = (lineEnd - lineStart).sqrMagnitude;
			bool flag = sqrMagnitude < VectorUtils.Epsilon;
			float num;
			if (flag)
			{
				num = (point - lineStart).sqrMagnitude;
			}
			else
			{
				float num2 = (lineEnd.y - lineStart.y) * point.x - (lineEnd.x - lineStart.x) * point.y + lineEnd.x * lineStart.y - lineEnd.y * lineStart.x;
				num = num2 * num2 / sqrMagnitude;
			}
			return num;
		}

		public PathDistanceForwardIterator.Result AdvanceBy(float units, out float unitsRemaining)
		{
			unitsRemaining = units;
			bool ended = this.Ended;
			PathDistanceForwardIterator.Result result;
			if (ended)
			{
				result = PathDistanceForwardIterator.Result.Ended;
			}
			else
			{
				float num = this.currentT;
				Vector2 vector = this.lastPointEval;
				float num2;
				Vector2 zero;
				bool flag7;
				do
				{
					num2 = Mathf.Min(num + this.stepSizeT, 1f);
					zero = Vector2.zero;
					Vector2 vector2 = (this.needTangentsDuringEval ? VectorUtils.EvalFull(this.currentBezSeg, num2, out zero) : VectorUtils.Eval(this.currentBezSeg, num2));
					bool flag = false;
					bool flag2 = this.needTangentsDuringEval;
					if (flag2)
					{
						float num3 = Vector2.Dot(zero, this.currentTTangent);
						flag = num3 < this.maxTanAngleDevCosine;
					}
					bool flag3 = !flag && this.maxCordDeviationSq != float.MaxValue;
					if (flag3)
					{
						Vector2 vector3 = vector;
						float sqrMagnitude = (vector2 - vector3).sqrMagnitude;
						bool flag4 = sqrMagnitude > VectorUtils.Epsilon;
						if (flag4)
						{
							Vector2 vector4 = VectorUtils.Eval(this.currentBezSeg, Mathf.Min((num2 - this.currentT) * 2f + this.currentT, 1f));
							float num4 = this.PointToLineDistanceSq(vector2, vector3, vector4);
							flag = num4 >= this.maxCordDeviationSq;
						}
					}
					float num5 = (vector2 - this.lastPointEval).magnitude;
					bool flag5 = num5 > unitsRemaining;
					if (flag5)
					{
						num2 = num + this.stepSizeT * (unitsRemaining / num5);
						num5 = unitsRemaining;
						vector2 = VectorUtils.Eval(this.currentBezSeg, num2);
					}
					this.segmentLengthSoFar += num5;
					this.lengthSoFar += num5;
					unitsRemaining -= num5;
					this.lastPointEval = vector2;
					num = num2;
					bool flag6 = num2 < 1f;
					if (!flag6)
					{
						goto IL_01E5;
					}
					flag7 = unitsRemaining > 0f && !flag;
				}
				while (flag7);
				this.currentT = num2;
				this.currentTTangent = zero;
				return PathDistanceForwardIterator.Result.Stepped;
				IL_01E5:
				bool flag8 = this.currentSegment + 1 == this.Segments.Count - 1;
				if (flag8)
				{
					this.currentT = 1f;
					result = PathDistanceForwardIterator.Result.Ended;
				}
				else
				{
					this.currentSegment++;
					this.currentBezSeg = new BezierSegment
					{
						P0 = this.Segments[this.currentSegment].P0,
						P1 = this.Segments[this.currentSegment].P1,
						P2 = this.Segments[this.currentSegment].P2,
						P3 = this.Segments[this.currentSegment + 1].P0
					};
					this.segmentLengthSoFar = 0f;
					this.currentT = 0f;
					this.currentTTangent = zero;
					this.lastPointEval = this.currentBezSeg.P0;
					result = PathDistanceForwardIterator.Result.NewSegment;
				}
			}
			return result;
		}

		public IList<BezierPathSegment> Segments { get; }

		public bool Closed
		{
			get
			{
				return this.closed;
			}
		}

		public int CurrentSegment
		{
			get
			{
				return this.currentSegment;
			}
		}

		public float CurrentT
		{
			get
			{
				return this.currentT;
			}
		}

		public float LengthSoFar
		{
			get
			{
				return this.lengthSoFar;
			}
		}

		public float SegmentLengthSoFar
		{
			get
			{
				return this.segmentLengthSoFar;
			}
		}

		public bool Ended
		{
			get
			{
				return this.currentT == 1f && this.currentSegment + 1 == this.Segments.Count - 1;
			}
		}

		public Vector2 EvalCurrent()
		{
			return VectorUtils.Eval(this.currentBezSeg, this.currentT);
		}

		private readonly bool closed;

		private readonly bool needTangentsDuringEval;

		private readonly float maxCordDeviationSq;

		private readonly float maxTanAngleDevCosine;

		private readonly float stepSizeT;

		private int currentSegment;

		private float currentT;

		private float segmentLengthSoFar;

		private float lengthSoFar;

		private Vector2 lastPointEval;

		private Vector2 currentTTangent;

		private BezierSegment currentBezSeg;

		private class BezierLoop : IList<BezierPathSegment>, ICollection<BezierPathSegment>, IEnumerable<BezierPathSegment>, IEnumerable
		{
			public BezierLoop(IList<BezierPathSegment> openPath)
			{
				this.OpenPath = openPath;
			}

			public BezierPathSegment this[int index]
			{
				get
				{
					bool flag = index == this.OpenPath.Count;
					BezierPathSegment bezierPathSegment;
					if (flag)
					{
						bezierPathSegment = this.OpenPath[0];
					}
					else
					{
						bezierPathSegment = this.OpenPath[index];
					}
					return bezierPathSegment;
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			public int Count
			{
				get
				{
					return this.OpenPath.Count + 1;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					return true;
				}
			}

			public void Add(BezierPathSegment item)
			{
				throw new NotSupportedException();
			}

			public void Clear()
			{
			}

			public bool Contains(BezierPathSegment item)
			{
				throw new NotImplementedException();
			}

			public void CopyTo(BezierPathSegment[] array, int arrayIndex)
			{
				throw new NotImplementedException();
			}

			public IEnumerator<BezierPathSegment> GetEnumerator()
			{
				throw new NotImplementedException();
			}

			public int IndexOf(BezierPathSegment item)
			{
				throw new NotImplementedException();
			}

			public void Insert(int index, BezierPathSegment item)
			{
				throw new NotSupportedException();
			}

			public bool Remove(BezierPathSegment item)
			{
				throw new NotSupportedException();
			}

			public void RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				throw new NotImplementedException();
			}

			private IList<BezierPathSegment> OpenPath;
		}

		public enum Result
		{
			Stepped,
			NewSegment,
			Ended
		}
	}
}
