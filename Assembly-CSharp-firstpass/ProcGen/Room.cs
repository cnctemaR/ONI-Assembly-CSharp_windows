using System;
using System.Collections.Generic;
using KSerialization.Converters;

namespace ProcGen
{
	public class Room : SampleDescriber
	{
		public Room()
		{
			this.mobs = new List<WeightedMob>();
		}

		[StringEnumConverter]
		public Room.Shape shape { get; private set; }

		[StringEnumConverter]
		public Room.Selection mobselection { get; private set; }

		public List<WeightedMob> mobs { get; private set; }

		public void ResetMobs(SeededRandom rnd)
		{
			if (this.mobselection == Room.Selection.WeightedBucket)
			{
				if (this.bucket == null)
				{
					this.bucket = new List<WeightedMob>();
					for (int i = 0; i < this.mobs.Count; i++)
					{
						int num = 0;
						while ((float)num < this.mobs[i].weight)
						{
							this.bucket.Add(new WeightedMob(this.mobs[i].tag, 1f));
							num++;
						}
					}
				}
				this.bucket.ShuffleSeeded<WeightedMob>(rnd.RandomSource());
				this.mobIter = this.bucket.GetEnumerator();
				return;
			}
			this.mobIter = this.mobs.GetEnumerator();
		}

		public WeightedMob GetNextMob(SeededRandom rnd)
		{
			WeightedMob weightedMob = null;
			switch (this.mobselection)
			{
			case Room.Selection.OneOfEach:
			case Room.Selection.WeightedBucket:
				if (this.mobIter.MoveNext())
				{
					weightedMob = this.mobIter.Current;
				}
				break;
			case Room.Selection.Weighted:
				weightedMob = WeightedRandom.Choose<WeightedMob>(this.mobs, rnd);
				break;
			}
			return weightedMob;
		}

		private List<WeightedMob>.Enumerator mobIter;

		private List<WeightedMob> bucket;

		public enum Shape
		{
			Circle,
			Oval,
			Blob,
			Line,
			Square,
			TallThin,
			ShortWide,
			Template,
			PhysicalLayout,
			Splat
		}

		public enum Selection
		{
			None,
			OneOfEach,
			NOfEach,
			Weighted,
			WeightedBucket,
			WeightedResample,
			PickOneWeighted
		}
	}
}
