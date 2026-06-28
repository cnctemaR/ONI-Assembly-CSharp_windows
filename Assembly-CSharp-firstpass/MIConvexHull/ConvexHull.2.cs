using System;
using System.Collections.Generic;

namespace MIConvexHull
{
	public class ConvexHull<TVertex, TFace> where TVertex : IVertex where TFace : ConvexFace<TVertex, TFace>, new()
	{
		internal ConvexHull()
		{
		}

		public IEnumerable<TVertex> Points { get; internal set; }

		public IEnumerable<TFace> Faces { get; internal set; }

		public static ConvexHull<TVertex, TFace> Create(IList<TVertex> data, double PlaneDistanceTolerance)
		{
			if (data == null)
			{
				throw new ArgumentNullException("The supplied data is null.");
			}
			return ConvexHullAlgorithm.GetConvexHull<TVertex, TFace>(data, PlaneDistanceTolerance);
		}
	}
}
