using System;

namespace ProcGen
{
	public class Temperature
	{
		public Temperature()
		{
			this.min = 0f;
			this.max = 0f;
		}

		public float min { get; private set; }

		public float max { get; private set; }

		public enum Range
		{
			ExtremelyCold,
			VeryCold,
			Cold,
			Chilly,
			Cool,
			Mild,
			Room,
			HumanWarm,
			HumanHot,
			Hot,
			VeryHot,
			ExtremelyHot
		}
	}
}
