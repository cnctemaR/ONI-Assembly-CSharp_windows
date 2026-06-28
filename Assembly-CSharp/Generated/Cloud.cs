using System;
using UnityEngine;

namespace Generated
{
	[Serializable]
	public class Cloud
	{
		[SerializeField]
		public float rampTime = 60f;

		[SerializeField]
		public float onLength = 20f;

		[SerializeField]
		public SimHashes element = SimHashes.ChlorineGas;

		[SerializeField]
		public float externalTemperatureK = 800f;

		[SerializeField]
		public float externalMassKg = 15f;
	}
}
