using System;

namespace ProcGen
{
	public class CloudSettings
	{
		public int countMin { get; private set; }

		public int countRange { get; private set; }

		public float rampMin { get; private set; }

		public float rampRange { get; private set; }

		public float sizeMin { get; private set; }

		public float sizeRange { get; private set; }

		public float temperatureMin { get; private set; }

		public float temperatureRange { get; private set; }

		public float massMin { get; private set; }

		public float massRange { get; private set; }
	}
}
