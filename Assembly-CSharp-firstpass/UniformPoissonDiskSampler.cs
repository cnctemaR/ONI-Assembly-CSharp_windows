using System;
using System.Collections.Generic;
using UnityEngine;

public class UniformPoissonDiskSampler
{
	public UniformPoissonDiskSampler(SeededRandom seed)
	{
		this.myRandom = seed;
	}

	public List<Vector2> SampleCircle(Vector2 center, float radius, float minimumDistance)
	{
		return this.SampleCircle(center, radius, minimumDistance, 30);
	}

	public List<Vector2> SampleCircle(Vector2 center, float radius, float minimumDistance, int pointsPerIteration)
	{
		return this.Sample(center - new Vector2(radius, radius), center + new Vector2(radius, radius), new float?(radius), minimumDistance, pointsPerIteration);
	}

	public List<Vector2> SampleRectangle(Vector2 topLeft, Vector2 lowerRight, float minimumDistance)
	{
		return this.SampleRectangle(topLeft, lowerRight, minimumDistance, 30);
	}

	public List<Vector2> SampleRectangle(Vector2 topLeft, Vector2 lowerRight, float minimumDistance, int pointsPerIteration)
	{
		return this.Sample(topLeft, lowerRight, null, minimumDistance, pointsPerIteration);
	}

	private List<Vector2> Sample(Vector2 topLeft, Vector2 lowerRight, float? rejectionDistance, float minimumDistance, int pointsPerIteration)
	{
		UniformPoissonDiskSampler.Settings settings = new UniformPoissonDiskSampler.Settings
		{
			TopLeft = topLeft,
			LowerRight = lowerRight,
			Dimensions = lowerRight - topLeft,
			Center = (topLeft + lowerRight) / 2f,
			CellSize = minimumDistance / UniformPoissonDiskSampler.SquareRootTwo,
			MinimumDistance = minimumDistance,
			RejectionSqDistance = ((rejectionDistance != null) ? ((rejectionDistance == null) ? null : new float?(rejectionDistance.GetValueOrDefault() * rejectionDistance.GetValueOrDefault())) : null)
		};
		settings.GridWidth = (int)(settings.Dimensions.x / settings.CellSize) + 1;
		settings.GridHeight = (int)(settings.Dimensions.y / settings.CellSize) + 1;
		UniformPoissonDiskSampler.State state = new UniformPoissonDiskSampler.State
		{
			Grid = new Vector2?[settings.GridWidth, settings.GridHeight],
			ActivePoints = new List<Vector2>(),
			Points = new List<Vector2>()
		};
		this.AddFirstPoint(ref settings, ref state);
		while (state.ActivePoints.Count != 0)
		{
			int num = this.myRandom.RandomRange(0, state.ActivePoints.Count - 1);
			Vector2 vector = state.ActivePoints[num];
			bool flag = false;
			for (int i = 0; i < pointsPerIteration; i++)
			{
				flag |= this.AddNextPoint(vector, ref settings, ref state);
			}
			if (!flag)
			{
				state.ActivePoints.RemoveAt(num);
			}
		}
		return state.Points;
	}

	private void AddFirstPoint(ref UniformPoissonDiskSampler.Settings settings, ref UniformPoissonDiskSampler.State state)
	{
		bool flag = false;
		while (!flag)
		{
			float num = this.myRandom.RandomValue();
			float num2 = settings.TopLeft.x + settings.Dimensions.x * num;
			num = this.myRandom.RandomValue();
			float num3 = settings.TopLeft.y + settings.Dimensions.y * num;
			Vector2 vector = new Vector2(num2, num3);
			float? rejectionSqDistance = settings.RejectionSqDistance;
			if (rejectionSqDistance != null)
			{
				float? rejectionSqDistance2 = settings.RejectionSqDistance;
				if (Vector2.SqrMagnitude(settings.Center - vector) > rejectionSqDistance2)
				{
					continue;
				}
			}
			flag = true;
			Vector2 vector2 = UniformPoissonDiskSampler.Denormalize(vector, settings.TopLeft, (double)settings.CellSize);
			state.Grid[(int)vector2.x, (int)vector2.y] = new Vector2?(vector);
			state.ActivePoints.Add(vector);
			state.Points.Add(vector);
		}
	}

	private bool AddNextPoint(Vector2 point, ref UniformPoissonDiskSampler.Settings settings, ref UniformPoissonDiskSampler.State state)
	{
		bool flag = false;
		Vector2 vector = this.GenerateRandomAround(point, settings.MinimumDistance);
		if (vector.x >= settings.TopLeft.x && vector.x < settings.LowerRight.x && vector.y > settings.TopLeft.y && vector.y < settings.LowerRight.y)
		{
			float? rejectionSqDistance = settings.RejectionSqDistance;
			if (rejectionSqDistance != null)
			{
				float? rejectionSqDistance2 = settings.RejectionSqDistance;
				if (!(Vector2.SqrMagnitude(settings.Center - vector) <= rejectionSqDistance2))
				{
					return flag;
				}
			}
			Vector2 vector2 = UniformPoissonDiskSampler.Denormalize(vector, settings.TopLeft, (double)settings.CellSize);
			bool flag2 = false;
			int num = (int)Math.Max(0f, vector2.x - 2f);
			while ((float)num < Math.Min((float)settings.GridWidth, vector2.x + 3f) && !flag2)
			{
				int num2 = (int)Math.Max(0f, vector2.y - 2f);
				while ((float)num2 < Math.Min((float)settings.GridHeight, vector2.y + 3f) && !flag2)
				{
					if (state.Grid[num, num2] != null && Vector2.Distance(state.Grid[num, num2].Value, vector) < settings.MinimumDistance)
					{
						flag2 = true;
					}
					num2++;
				}
				num++;
			}
			if (!flag2)
			{
				flag = true;
				state.ActivePoints.Add(vector);
				state.Points.Add(vector);
				state.Grid[(int)vector2.x, (int)vector2.y] = new Vector2?(vector);
			}
		}
		return flag;
	}

	private Vector2 GenerateRandomAround(Vector2 center, float minimumDistance)
	{
		float num = this.myRandom.RandomValue();
		float num2 = minimumDistance + minimumDistance * num;
		num = this.myRandom.RandomValue();
		float num3 = 6.2831855f * num;
		float num4 = num2 * (float)Math.Sin((double)num3);
		float num5 = num2 * (float)Math.Cos((double)num3);
		return new Vector2(center.x + num4, center.y + num5);
	}

	private static Vector2 Denormalize(Vector2 point, Vector2 origin, double cellSize)
	{
		return new Vector2((float)((int)((double)(point.x - origin.x) / cellSize)), (float)((int)((double)(point.y - origin.y) / cellSize)));
	}

	public const int DefaultPointsPerIteration = 30;

	private static readonly float SquareRootTwo = (float)Math.Sqrt(2.0);

	private SeededRandom myRandom = null;

	private struct Settings
	{
		public Vector2 TopLeft;

		public Vector2 LowerRight;

		public Vector2 Center;

		public Vector2 Dimensions;

		public float? RejectionSqDistance;

		public float MinimumDistance;

		public float CellSize;

		public int GridWidth;

		public int GridHeight;
	}

	private struct State
	{
		public Vector2?[,] Grid;

		public List<Vector2> ActivePoints;

		public List<Vector2> Points;
	}
}
