using System;
using UnityEngine;

namespace Unity.VectorGraphics
{
	internal class PathPatternIterator
	{
		public PathPatternIterator(float[] pattern, float patternOffset = 0f)
		{
			bool flag = pattern != null;
			if (flag)
			{
				foreach (float num in pattern)
				{
					this.patternLength += num;
				}
			}
			bool flag2 = this.patternLength < VectorUtils.Epsilon;
			if (flag2)
			{
				this.segmentLength = float.MaxValue;
			}
			else
			{
				this.pattern = pattern;
				this.patternOffset = patternOffset;
				bool flag3 = patternOffset == 0f;
				if (flag3)
				{
					this.segmentLength = pattern[0];
				}
				else
				{
					this.solid = this.IsSolidAt(0f, out this.currentSegment, out this.segmentLength);
				}
			}
		}

		public void Advance()
		{
			bool flag = this.pattern == null;
			if (!flag)
			{
				this.currentSegment++;
				bool flag2 = this.currentSegment >= this.pattern.Length;
				if (flag2)
				{
					this.currentSegment = 0;
				}
				this.solid = !this.solid;
				this.segmentLength = this.pattern[this.currentSegment];
			}
		}

		public bool IsSolidAt(float unitsFromPathStart)
		{
			int num;
			float num2;
			return this.IsSolidAt(unitsFromPathStart, out num, out num2);
		}

		public bool IsSolidAt(float unitsFromPathStart, out int patternSegmentIndex, out float patternSegmentLength)
		{
			patternSegmentIndex = 0;
			patternSegmentLength = 0f;
			bool flag = this.pattern == null;
			bool flag2;
			if (flag)
			{
				flag2 = true;
			}
			else
			{
				bool flag3 = true;
				unitsFromPathStart += this.patternOffset;
				int num = (int)(Mathf.Abs(unitsFromPathStart) / this.patternLength);
				bool flag4 = unitsFromPathStart < 0f;
				if (flag4)
				{
					unitsFromPathStart = this.patternLength - -unitsFromPathStart % this.patternLength;
					bool flag5 = (this.pattern.Length & 1) == 1;
					if (flag5)
					{
						flag3 = (num & 1) == 0;
					}
				}
				else
				{
					unitsFromPathStart %= this.patternLength;
					bool flag6 = (this.pattern.Length & 1) == 1;
					if (flag6)
					{
						flag3 = (num & 1) == 1;
					}
				}
				while (unitsFromPathStart > this.pattern[patternSegmentIndex])
				{
					float num2 = unitsFromPathStart;
					float[] array = this.pattern;
					int num3 = patternSegmentIndex;
					patternSegmentIndex = num3 + 1;
					unitsFromPathStart = num2 - array[num3];
					flag3 = !flag3;
				}
				patternSegmentLength = this.pattern[patternSegmentIndex] - unitsFromPathStart;
				flag2 = flag3;
			}
			return flag2;
		}

		public float SegmentLength
		{
			get
			{
				return this.segmentLength;
			}
		}

		public bool IsSolid
		{
			get
			{
				return this.solid;
			}
		}

		private float[] pattern;

		private int currentSegment;

		private bool solid = true;

		private float segmentLength;

		private float patternLength;

		private float patternOffset;
	}
}
