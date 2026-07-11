using System;
using UnityEngine.Playables;

namespace UnityEngine.Timeline
{
	internal static class WeightUtility
	{
		public static float NormalizeMixer(Playable mixer)
		{
			float num;
			if (!mixer.IsValid<Playable>())
			{
				num = 0f;
			}
			else
			{
				int inputCount = mixer.GetInputCount<Playable>();
				float num2 = 0f;
				for (int i = 0; i < inputCount; i++)
				{
					num2 += mixer.GetInputWeight(i);
				}
				if (num2 > Mathf.Epsilon && num2 < 1f)
				{
					for (int j = 0; j < inputCount; j++)
					{
						mixer.SetInputWeight(j, mixer.GetInputWeight(j) / num2);
					}
				}
				num = Mathf.Clamp01(num2);
			}
			return num;
		}
	}
}
