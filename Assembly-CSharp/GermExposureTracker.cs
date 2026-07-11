using System;
using System.Collections.Generic;
using KSerialization;
using ProcGen;

[SerializationConfig(MemberSerialization.OptIn)]
public class GermExposureTracker : KMonoBehaviour
{
	protected override void OnPrefabInit()
	{
		Debug.Assert(GermExposureTracker.Instance == null);
		GermExposureTracker.Instance = this;
	}

	protected override void OnSpawn()
	{
		this.rng = new SeededRandom(GameClock.Instance.GetCycle());
	}

	protected override void OnCleanUp()
	{
		GermExposureTracker.Instance = null;
	}

	public void AddExposure(GermExposureMonitor.ExposureType exposure_type, float amount)
	{
		float num;
		this.accumulation.TryGetValue(exposure_type.germ_id, out num);
		float num2 = num + amount;
		if (num2 > 1f)
		{
			foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
			{
				GermExposureMonitor.Instance smi = minionIdentity.GetSMI<GermExposureMonitor.Instance>();
				if (smi.GetExposureState(exposure_type.germ_id) == GermExposureMonitor.ExposureState.Exposed)
				{
					GermExposureMonitor.Instance smi2 = minionIdentity.GetSMI<GermExposureMonitor.Instance>();
					float exposureWeight = smi2.GetExposureWeight(exposure_type.germ_id);
					if (exposureWeight > 0f)
					{
						this.exposure_candidates.Add(new GermExposureTracker.WeightedExposure
						{
							weight = exposureWeight,
							monitor = smi
						});
					}
				}
			}
			while (num2 > 1f)
			{
				num2 -= 1f;
				if (this.exposure_candidates.Count > 0)
				{
					GermExposureTracker.WeightedExposure weightedExposure = WeightedRandom.Choose<GermExposureTracker.WeightedExposure>(this.exposure_candidates, this.rng);
					this.exposure_candidates.Remove(weightedExposure);
					weightedExposure.monitor.ContractGerms(exposure_type.germ_id);
				}
			}
		}
		this.accumulation[exposure_type.germ_id] = num2;
		this.exposure_candidates.Clear();
	}

	public static GermExposureTracker Instance;

	[Serialize]
	private Dictionary<HashedString, float> accumulation = new Dictionary<HashedString, float>();

	private SeededRandom rng;

	private List<GermExposureTracker.WeightedExposure> exposure_candidates = new List<GermExposureTracker.WeightedExposure>();

	private class WeightedExposure : IWeighted
	{
		public float weight { get; set; }

		public GermExposureMonitor.Instance monitor;
	}
}
