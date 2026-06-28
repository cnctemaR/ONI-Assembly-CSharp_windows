using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGen
{
	[Serializable]
	public class Cloud
	{
		public static void GenerateClouds(List<Cloud> clouds, SeededRandom rnd, CloudSettings settings)
		{
			int num = settings.countMin + (int)((float)settings.countRange * rnd.RandomValue());
			for (int i = 0; i < num; i++)
			{
				Cloud cloud = new Cloud();
				cloud.rampTime = settings.rampMin + settings.rampRange * rnd.RandomValue();
				cloud.onLength = settings.sizeMin + settings.sizeRange * rnd.RandomValue();
				cloud.externalTemperatureK = settings.temperatureMin + settings.temperatureRange * rnd.RandomValue();
				List<SimHashes> nonSolidAtTargetTemperature = Cloud.GetNonSolidAtTargetTemperature(cloud.externalTemperatureK);
				nonSolidAtTargetTemperature.ShuffleSeeded<SimHashes>(rnd.RandomSource());
				cloud.element = nonSolidAtTargetTemperature[0];
				cloud.externalMassKg = settings.massMin + settings.massRange * rnd.RandomValue();
				clouds.Add(cloud);
			}
		}

		private static List<SimHashes> GetNonSolidAtTargetTemperature(float temperature)
		{
			List<SimHashes> list = new List<SimHashes>();
			for (int i = 0; i < ElementLoader.elements.Count; i++)
			{
				Element element = ElementLoader.elements[i];
				if (!element.IsSolid && !element.IsVacuum && element.lowTemp < temperature)
				{
					list.Add(element.id);
				}
			}
			return list;
		}

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
